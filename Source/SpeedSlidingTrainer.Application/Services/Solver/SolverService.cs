using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Events;
using SpeedSlidingTrainer.Application.Infrastructure;
using SpeedSlidingTrainer.Application.Services.Game;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;
using SpeedSlidingTrainer.Core.Services.BoardSolver;

namespace SpeedSlidingTrainer.Application.Services.Solver
{
    public sealed class SolverService : ISolverService
    {
        [NotNull]
        private readonly IMessageBus messageBus;

        [NotNull]
        private readonly IGameService gameService;

        [NotNull]
        private readonly IBoardSolverService boardSolverService;

        [NotNull]
        private readonly IDispatcher dispatcher;

        private SolverServiceStatus status;

        [CanBeNull]
        private BackgroundJob currentBackgroundJob;

        private IReadOnlyList<IReadOnlyList<SolutionStep>> solutions;

        private int? solutionLength;

        private int nextStepIndex;

        [CanBeNull]
        private BoardState solvedBoardState;

        public SolverService(
            [NotNull] IMessageBus messageBus,
            [NotNull] IGameService gameService,
            [NotNull] IBoardSolverService boardSolverService,
            [NotNull] IDispatcher dispatcher)
        {
            if (messageBus == null)
            {
                throw new ArgumentNullException(nameof(messageBus));
            }

            if (gameService == null)
            {
                throw new ArgumentNullException(nameof(gameService));
            }

            if (boardSolverService == null)
            {
                throw new ArgumentNullException(nameof(boardSolverService));
            }

            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            this.messageBus = messageBus;
            this.gameService = gameService;
            this.boardSolverService = boardSolverService;
            this.dispatcher = dispatcher;

            this.messageBus.Subscribe<BoardScrambled>(this.OnBoardScrambled);
            this.messageBus.Subscribe<BoardResetted>(this.OnBoardResetted);
            this.messageBus.Subscribe<SlideHappened>(this.OnSlideHappened);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [UsedImplicitly]
        public bool AutoSolve { get; set; }

        public SolverServiceStatus Status
        {
            get
            {
                return this.status;
            }

            private set
            {
                if (this.status == value)
                {
                    return;
                }

                this.status = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Status)));
            }
        }

        public IReadOnlyList<IReadOnlyList<SolutionStep>> Solutions
        {
            get
            {
                return this.solutions;
            }

            private set
            {
                this.solutions = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Solutions)));
            }
        }

        public int? SolutionLength
        {
            get
            {
                return this.solutionLength;
            }

            private set
            {
                this.solutionLength = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SolutionLength)));
            }
        }

        public void StartSolveCurrentBoard()
        {
            if (this.Status != SolverServiceStatus.NotSolved)
            {
                return;
            }

            this.Status = SolverServiceStatus.Solving;
            this.Solutions = null;
            this.currentBackgroundJob = new BackgroundJob
            {
                InitialState = this.gameService.InitialState,
                StateToSolve = this.gameService.BoardState,
                Goal = this.gameService.Drill.Goal,
                CancellationTokenSource = new CancellationTokenSource()
            };

            Task.Factory.StartNew(() => this.BackgroundThreadMain(this.currentBackgroundJob), this.currentBackgroundJob.CancellationTokenSource.Token);
        }

        private void OnSlideHappened(SlideHappened message)
        {
            if (this.Solutions == null)
            {
                return;
            }

            this.Status = SolverServiceStatus.NotSolved;
            if (this.nextStepIndex >= this.SolutionLength)
            {
                return;
            }

            foreach (IReadOnlyList<SolutionStep> solution in this.Solutions)
            {
                if (solution[this.nextStepIndex].Status == SolutionStepStatus.Misstepped)
                {
                    continue;
                }

                if (solution[this.nextStepIndex].Step == message.Step)
                {
                    solution[this.nextStepIndex].Status = SolutionStepStatus.Stepped;
                }
                else
                {
                    for (int i = this.nextStepIndex; i < solution.Count; i++)
                    {
                        solution[i].Status = SolutionStepStatus.Misstepped;
                    }
                }
            }

            this.nextStepIndex++;
        }

        private void OnBoardResetted(BoardResetted message)
        {
            if (this.Solutions == null)
            {
                return;
            }

            if (this.gameService.BoardState.Equals(this.solvedBoardState))
            {
                this.nextStepIndex = 0;
                foreach (IReadOnlyList<SolutionStep> solution in this.Solutions)
                {
                    foreach (SolutionStep solutionStep in solution)
                    {
                        solutionStep.Status = SolutionStepStatus.NotSteppedYet;
                    }
                }
            }
            else
            {
                this.Status = SolverServiceStatus.NotSolved;
                this.Solutions = null;
                this.SolutionLength = null;
                if (this.AutoSolve)
                {
                    this.StartSolveCurrentBoard();
                }
            }
        }

        private void OnBoardScrambled(BoardScrambled message)
        {
            if (this.Status == SolverServiceStatus.Solving)
            {
                this.currentBackgroundJob.CancellationTokenSource.Cancel();
            }

            this.Status = SolverServiceStatus.NotSolved;
            this.Solutions = null;
            this.SolutionLength = null;
            if (this.AutoSolve)
            {
                this.StartSolveCurrentBoard();
            }
        }

        private void BackgroundThreadMain(BackgroundJob job)
        {
            try
            {
                Step[][] solutions = this.boardSolverService.GetSolution(job.StateToSolve, job.Goal, job.CancellationTokenSource.Token);
                this.dispatcher.BeginInvoke(() => this.OnSolved(job, solutions));
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void OnSolved(BackgroundJob job, Step[][] solutions)
        {
            if (job.CancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            this.Status = SolverServiceStatus.Solved;
            this.Solutions = solutions
                .OrderBy(solution => string.Join(",", solution))
                .Select(solution => solution.Select(step => new SolutionStep(step, SolutionStepStatus.NotSteppedYet)).ToList())
                .ToList();

            this.SolutionLength = solutions[0].Length;
            this.nextStepIndex = 0;
            this.solvedBoardState = job.StateToSolve;

            this.messageBus.Publish(new SolutionsFound(job.InitialState, job.StateToSolve, solutions));
        }

        private class BackgroundJob
        {
            public BoardState InitialState { get; set; }

            public BoardState StateToSolve { get; set; }

            public BoardGoal Goal { get; set; }

            public CancellationTokenSource CancellationTokenSource { get; set; }
        }
    }
}
