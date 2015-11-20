using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        private readonly object locker = new object();

        private SolverServiceStatus status;

        [CanBeNull]
        private CancellationTokenSource cts;

        private ObservableCollection<SolutionStep> solution = new ObservableCollection<SolutionStep>();

        private int solutionLength;

        private int nextStepIndex;

        public SolverService(IGameService gameService, IBoardSolverService boardSolverService)
        {
            if (gameService == null)
            {
                throw new ArgumentNullException(nameof(gameService));
            }

            if (boardSolverService == null)
            {
                throw new ArgumentNullException(nameof(boardSolverService));
            }

            this.gameService = gameService;
            this.gameService.Scrambled += this.GameServiceOnScrambled;
            this.gameService.Slid += this.GameServiceOnSlid;
            this.gameService.Resetted += this.GameServiceOnResetted;
            this.boardSolverService = boardSolverService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            CancellationToken currentCancellationToken;
            BoardState initialState;
            BoardGoal boardGoal;

            lock (this.locker)
            {
                if (this.Status != SolverServiceStatus.NotSolved)
                {
                    return;
                }

                this.Status = SolverServiceStatus.Solving;
                this.Solution = null;
                this.cts = new CancellationTokenSource();

                currentCancellationToken = this.cts.Token;
                initialState = this.gameService.StartState;
                boardGoal = this.gameService.Drill.Goal;
            }

            Task.Factory.StartNew(
                () =>
                    {
                        try
                        {
                            Step[] result = this.boardSolverService.GetSolution(initialState, boardGoal, currentCancellationToken)[0];

                            lock (this.locker)
                            {
                                if (currentCancellationToken.IsCancellationRequested)
                                {
                                    return;
                                }

                                this.Status = SolverServiceStatus.Solved;
                                this.Solution = new ObservableCollection<SolutionStep>(
                                    result.Select(x => new SolutionStep(x, SolutionStepStatus.NotSteppedYet)));
                                this.SolutionLength = result.Length;
                                this.nextStepIndex = 0;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    },
                currentCancellationToken);
        }

        private void GameServiceOnSlid(object sender, SlidEventArgs slidEventArgs)
        {
            lock (this.locker)
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
        }

        private void GameServiceOnResetted(object sender, EventArgs eventArgs)
        {
            lock (this.locker)
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
        }

        private void GameServiceOnScrambled(object sender, EventArgs eventArgs)
        {
            lock (this.locker)
            {
                if (this.Status == SolverServiceStatus.Solving)
                {
                    this.cts?.Cancel();
                }

                this.Status = SolverServiceStatus.NotSolved;
                this.Solution = null;
            }
        }
    }
}
