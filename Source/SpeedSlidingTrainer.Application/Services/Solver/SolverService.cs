using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        private readonly IGameService gameService;

        [NotNull]
        private readonly IBoardSolverService boardSolverService;

        [NotNull]
        private readonly IDispatcher dispatcher;

        private SolverServiceStatus status;

        [CanBeNull]
        private BackgroundJob currentBackgroundJob;

        private IReadOnlyCollection<IReadOnlyList<SolutionStep>> solutions;

        private int solutionLength;

        private int nextStepIndex;

        public SolverService(IGameService gameService, IBoardSolverService boardSolverService, IDispatcher dispatcher)
        {
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

            this.gameService = gameService;
            this.gameService.Scrambled += this.GameServiceOnScrambled;
            this.gameService.Slid += this.GameServiceOnSlid;
            this.gameService.Resetted += this.GameServiceOnResetted;
            this.boardSolverService = boardSolverService;
            this.dispatcher = dispatcher;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public IReadOnlyCollection<IReadOnlyList<SolutionStep>> Solutions
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

        public int SolutionLength
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
                State = this.gameService.StartState,
                Goal = this.gameService.Drill.Goal,
                CancellationTokenSource = new CancellationTokenSource()
            };

            Task.Factory.StartNew(() => this.BackgroundThreadMain(this.currentBackgroundJob), this.currentBackgroundJob.CancellationTokenSource.Token);
        }

        private void GameServiceOnSlid(object sender, SlidEventArgs slidEventArgs)
        {
            if (this.Status != SolverServiceStatus.Solved)
            {
                return;
            }

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

                if (solution[this.nextStepIndex].Step == slidEventArgs.Step)
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

        private void GameServiceOnResetted(object sender, EventArgs eventArgs)
        {
            if (this.Solutions == null)
            {
                return;
            }

            this.nextStepIndex = 0;
            foreach (IReadOnlyList<SolutionStep> solution in this.Solutions)
            {
                foreach (SolutionStep solutionStep in solution)
                {
                    solutionStep.Status = SolutionStepStatus.NotSteppedYet;
                }
            }
        }

        private void GameServiceOnScrambled(object sender, EventArgs eventArgs)
        {
            if (this.Status == SolverServiceStatus.Solving)
            {
                this.currentBackgroundJob.CancellationTokenSource.Cancel();
            }

            this.Status = SolverServiceStatus.NotSolved;
            this.Solutions = null;
            if (this.AutoSolve)
            {
                this.StartSolveCurrentBoard();
            }
        }

        private void BackgroundThreadMain(BackgroundJob job)
        {
            try
            {
                Step[][] solutions = this.boardSolverService.GetSolution(job.State, job.Goal, job.CancellationTokenSource.Token);
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
            this.Solutions = solutions.Select(solution => solution.Select(x => new SolutionStep(x, SolutionStepStatus.NotSteppedYet)).ToList()).ToList();
            this.SolutionLength = solutions[0].Length;
            this.nextStepIndex = 0;
        }

        private class BackgroundJob
        {
            public BoardState State { get; set; }

            public BoardGoal Goal { get; set; }

            public CancellationTokenSource CancellationTokenSource { get; set; }
        }
    }
}
