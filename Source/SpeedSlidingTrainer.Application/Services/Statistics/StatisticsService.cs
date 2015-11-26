using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Events;
using SpeedSlidingTrainer.Application.Infrastructure;
using SpeedSlidingTrainer.Application.Services.Game;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.Statistics
{
    public sealed class StatisticsService : IStatisticsService
    {
        [NotNull]
        private readonly IGameService gameService;

        [NotNull]
        private readonly ITimer timer;

        private int stepCount;

        private int? optimalStepCount;

        private DateTime startedAt = DateTime.MinValue;

        private DateTime completedAt = DateTime.MinValue;

        private TimeSpan duration;

        [NotNull]
        private IReadOnlyList<SolveStatistics> lastSolves = new SolveStatistics[0];

        public StatisticsService([NotNull] IMessageQueue messageQueue, [NotNull] IGameService gameService, [NotNull] ITimerFactory timerFactory)
        {
            if (messageQueue == null)
            {
                throw new ArgumentNullException(nameof(messageQueue));
            }

            if (gameService == null)
            {
                throw new ArgumentNullException(nameof(gameService));
            }

            this.gameService = gameService;
            this.timer = timerFactory.Create(TimeSpan.FromMilliseconds(25), this.OnTick);

            messageQueue.Subscribe<BoardScrambled>(this.OnBoardScrambled);
            messageQueue.Subscribe<BoardResetted>(this.OnBoardResetted);
            messageQueue.Subscribe<SolveStarted>(this.OnSolveStarted);
            messageQueue.Subscribe<SolveCompleted>(this.OnSolveCompleted);
            messageQueue.Subscribe<SlideHappened>(this.OnSlideHappened);
            messageQueue.Subscribe<SolutionsFound>(this.OnSolutionFound);
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

        private void OnSolveStarted(SolveStarted message)
        {
            this.startedAt = DateTime.UtcNow;
            this.timer.Start();
        }

        private void OnSolveCompleted(SolveCompleted message)
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

        private void OnBoardResetted(BoardResetted message)
        {
            this.StepCount = 0;
            this.Duration = TimeSpan.Zero;
        }

        private void OnBoardScrambled(BoardScrambled message)
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

        private void OnSlideHappened(SlideHappened message)
        {
            if (this.gameService.Status == SolveStatus.InProgress)
            {
                this.StepCount++;
            }
        }

        private void OnSolutionFound(SolutionsFound message)
        {
            if (message.State.Equals(this.gameService.StartState))
            {
                this.OptimalStepCount = message.Solutions[0].Length;
            }
        }
    }
}
