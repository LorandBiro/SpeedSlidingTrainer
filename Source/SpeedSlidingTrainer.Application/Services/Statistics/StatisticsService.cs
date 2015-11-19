using System.ComponentModel;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.Statistics
{
    using System;
    using JetBrains.Annotations;
    using SpeedSlidingTrainer.Application.Infrastructure;
    using SpeedSlidingTrainer.Application.Services.Game;

    public sealed class StatisticsService : IStatisticsService
    {
        [NotNull]
        private readonly IGameService gameService;

        [NotNull]
        private readonly ITimer timer;

        private int stepCount;

        private DateTime startedAt = DateTime.MinValue;

        private DateTime completedAt = DateTime.MinValue;

        private TimeSpan duration;

        public StatisticsService([NotNull] IGameService gameService, [NotNull] ITimerFactory timer)
        {
            if (gameService == null)
            {
                throw new ArgumentNullException(nameof(gameService));
            }

            this.gameService = gameService;
            this.timer = timer.Create(TimeSpan.FromMilliseconds(25), this.OnTick);

            this.gameService.SolveStarted += this.GameServiceOnSolveStarted;
            this.gameService.SolveCompleted += this.GameServiceOnSolveCompleted;
            this.gameService.Resetted += this.GameServiceOnResetted;
            this.gameService.Slid += this.GameServiceOnSlid;
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
        }

        private void GameServiceOnResetted(object sender, EventArgs eventArgs)
        {
            this.StepCount = 0;
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
    }
}
