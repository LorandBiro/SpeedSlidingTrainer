using System;
using System.Collections.ObjectModel;
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

        [NotNull]
        private readonly object locker = new object();

        private SolverServiceStatus status;

        [CanBeNull]
        private BackgroundJob currentBackgroundJob;

        private ObservableCollection<SolutionStep> solution = new ObservableCollection<SolutionStep>();

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

        public ObservableCollection<SolutionStep> Solution
        {
            get
            {
                return this.solution;
            }

            private set
            {
                this.solution = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Solution)));
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
            this.Solution = null;
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

            if (this.Solution[this.nextStepIndex].Step == slidEventArgs.Step)
            {
                this.Solution[this.nextStepIndex].Status = SolutionStepStatus.Stepped;
                this.nextStepIndex++;
            }
            else
            {
                for (int i = this.nextStepIndex; i < this.Solution.Count; i++)
                {
                    this.Solution[i].Status = SolutionStepStatus.Misstepped;
                }

                this.nextStepIndex = this.SolutionLength;
            }
        }

        private void GameServiceOnResetted(object sender, EventArgs eventArgs)
        {
            if (this.Solution == null)
            {
                return;
            }

            this.nextStepIndex = 0;
            foreach (SolutionStep solutionStep in this.Solution)
            {
                solutionStep.Status = SolutionStepStatus.NotSteppedYet;
            }
        }

        private void GameServiceOnScrambled(object sender, EventArgs eventArgs)
        {
            if (this.Status == SolverServiceStatus.Solving)
            {
                this.currentBackgroundJob.CancellationTokenSource.Cancel();
            }

            this.Status = SolverServiceStatus.NotSolved;
            this.Solution = null;
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

            Step[] result = solutions[0];
            this.Status = SolverServiceStatus.Solved;
            this.Solution = new ObservableCollection<SolutionStep>(result.Select(x => new SolutionStep(x, SolutionStepStatus.NotSteppedYet)));
            this.SolutionLength = result.Length;
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
