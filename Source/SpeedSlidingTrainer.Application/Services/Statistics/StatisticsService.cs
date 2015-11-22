using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Infrastructure;
using SpeedSlidingTrainer.Application.Services.Game;
using SpeedSlidingTrainer.Application.Services.Solver;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.Statistics
{
    public sealed class StatisticsService : IStatisticsService
    {
        [NotNull]
        private readonly IGameService gameService;

        [NotNull]
        private readonly ISolverService solverService;

        [NotNull]
        private readonly ITimer timer;

        private int stepCount;

        private int? optimalStepCount;

        private DateTime startedAt = DateTime.MinValue;

        private DateTime completedAt = DateTime.MinValue;

        private TimeSpan duration;

        [NotNull]
        private IReadOnlyList<SolveStatistics> lastSolves = new SolveStatistics[0];

        public StatisticsService([NotNull] IGameService gameService, [NotNull] ISolverService solverService, [NotNull] ITimerFactory timer)
        {
            if (gameService == null)
            {
                throw new ArgumentNullException(nameof(gameService));
            }

            if (solverService == null)
            {
                throw new ArgumentNullException(nameof(solverService));
            }

            this.gameService = gameService;
            this.solverService = solverService;
            this.timer = timer.Create(TimeSpan.FromMilliseconds(25), this.OnTick);

            this.gameService.SolveStarted += this.GameServiceOnSolveStarted;
            this.gameService.SolveCompleted += this.GameServiceOnSolveCompleted;
            this.gameService.Resetted += this.GameServiceOnResetted;
            this.gameService.Scrambled += this.GameServiceOnScrambled;
            this.gameService.Slid += this.GameServiceOnSlid;

            this.solverService.BoardSolved += this.SolverServiceOnBoardSolved;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int StepCount
        {
            get
            {
                return this.stepCount;
            }

            private set
            {
                if (this.stepCount == value)
                {
                    return;
                }

                this.stepCount = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.StepCount)));
            }
        }

        public int? OptimalStepCount
        {
            get
            {
                return this.optimalStepCount;
            }

            private set
            {
                if (this.optimalStepCount == value)
                {
                    return;
                }

                this.optimalStepCount = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.OptimalStepCount)));
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }

            private set
            {
                this.duration = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Duration)));
            }
        }

        public IReadOnlyList<SolveStatistics> LastSolves
        {
            get
            {
                return this.lastSolves;
            }

            private set
            {
                this.lastSolves = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.LastSolves)));
            }
        }

        private void GameServiceOnSolveStarted(object sender, EventArgs eventArgs)
        {
            this.startedAt = DateTime.UtcNow;
            this.timer.Start();
        }

        private void GameServiceOnSolveCompleted(object sender, EventArgs eventArgs)
        {
            this.completedAt = DateTime.UtcNow;
            this.timer.Stop();
            this.Duration = this.completedAt - this.startedAt;

            SolveStatistics statistics = new SolveStatistics(this.Duration, this.StepCount, this.OptimalStepCount);
            List<SolveStatistics> temp = this.LastSolves.ToList();
            temp.Insert(0, statistics);
            while (temp.Count > 5)
            {
                temp.RemoveAt(temp.Count - 1);
            }

            this.LastSolves = temp;
        }

        private void GameServiceOnResetted(object sender, EventArgs eventArgs)
        {
            this.StepCount = 0;
            this.Duration = TimeSpan.Zero;
        }

        private void GameServiceOnScrambled(object sender, EventArgs eventArgs)
        {
            this.StepCount = 0;
            this.OptimalStepCount = null;
            this.Duration = TimeSpan.Zero;
        }

        private void OnTick()
        {
            if (this.gameService.Status != SolveStatus.InProgress)
            {
                return;
            }

            this.Duration = DateTime.UtcNow - this.startedAt;
        }

        private void GameServiceOnSlid(object sender, SlidEventArgs e)
        {
            if (this.gameService.Status == SolveStatus.InProgress)
            {
                this.StepCount++;
            }
        }

        private void SolverServiceOnBoardSolved(object sender, BoardSolvedEventArgs e)
        {
            if (e.State.Equals(this.gameService.StartState))
            {
                this.OptimalStepCount = e.Solutions[0].Length;
            }
        }
    }
}
