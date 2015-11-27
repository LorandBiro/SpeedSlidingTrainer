using System;
using System.ComponentModel;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Events;
using SpeedSlidingTrainer.Application.Infrastructure;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.SolveState
{
    public sealed class SolveStateService : ISolveStateService
    {
        [NotNull]
        private readonly IMessageQueue messageQueue;

        [NotNull]
        private readonly ITimer timer;

        private SolveStatus status;

        private int stepCount;

        private int? optimalStepCount;

        private DateTime startedAt = DateTime.MinValue;

        private DateTime completedAt = DateTime.MinValue;

        private TimeSpan duration;

        public SolveStateService([NotNull] IMessageQueue messageQueue, [NotNull] ITimerFactory timerFactory)
        {
            if (messageQueue == null)
            {
                throw new ArgumentNullException(nameof(messageQueue));
            }

            this.messageQueue = messageQueue;
            this.timer = timerFactory.Create(TimeSpan.FromMilliseconds(25), this.OnTick);

            this.messageQueue.Subscribe<BoardScrambled>(this.OnBoardScrambled);
            this.messageQueue.Subscribe<BoardResetted>(this.OnBoardResetted);
            this.messageQueue.Subscribe<SlideHappened>(this.OnSlideHappened);
            this.messageQueue.Subscribe<SolutionsFound>(this.OnSolutionFound);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SolveStatus Status
        {
            get
            {
                return this.status;
            }

            private set
            {
                if (this.Status == value)
                {
                    return;
                }

                this.status = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Status)));
            }
        }

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

        private void OnBoardResetted(BoardResetted message)
        {
            this.Status = SolveStatus.NotStarted;
            this.StepCount = 0;
            this.Duration = TimeSpan.Zero;
        }

        private void OnBoardScrambled(BoardScrambled message)
        {
            this.Status = SolveStatus.NotStarted;
            this.StepCount = 0;
            this.OptimalStepCount = null;
            this.Duration = TimeSpan.Zero;
        }

        private void OnSlideHappened(SlideHappened message)
        {
            if (this.Status == SolveStatus.NotStarted)
            {
                this.Status = SolveStatus.InProgress;
                this.startedAt = DateTime.UtcNow;
                this.timer.Start();
            }

            if (this.Status == SolveStatus.InProgress)
            {
                this.StepCount++;
                if (message.BoardSolved)
                {
                    this.Status = SolveStatus.Completed;
                    this.completedAt = DateTime.UtcNow;
                    this.timer.Stop();
                    this.Duration = this.completedAt - this.startedAt;

                    SolveStatistics solveStatistics = new SolveStatistics(this.Duration, this.StepCount, this.OptimalStepCount);
                    this.messageQueue.Publish(new SolveCompleted(solveStatistics));
                }
            }
        }

        private void OnSolutionFound(SolutionsFound message)
        {
            if (message.SolvedState.Equals(message.InitialState))
            {
                this.OptimalStepCount = message.Solutions[0].Length;
            }
        }

        private void OnTick()
        {
            if (this.Status != SolveStatus.InProgress)
            {
                return;
            }

            this.Duration = DateTime.UtcNow - this.startedAt;
        }
    }
}
