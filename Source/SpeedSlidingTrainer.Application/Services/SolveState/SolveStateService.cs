using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        [NotNull]
        private IReadOnlyList<SolveStatistics> lastSolves = new SolveStatistics[0];

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
            this.messageQueue.Subscribe<SolveStarted>(this.OnSolveStarted);
            this.messageQueue.Subscribe<SolveCompleted>(this.OnSolveCompleted);
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

        private void OnTick()
        {
            if (this.Status != SolveStatus.InProgress)
            {
                return;
            }

            this.Duration = DateTime.UtcNow - this.startedAt;
        }

        private void OnSlideHappened(SlideHappened message)
        {
            if (this.Status == SolveStatus.NotStarted)
            {
                this.Status = SolveStatus.InProgress;
                this.messageQueue.Publish(new SolveStarted());
            }

            if (this.Status == SolveStatus.InProgress)
            {
                this.StepCount++;
                if (message.BoardSolved)
                {
                    this.Status = SolveStatus.Completed;
                    this.messageQueue.Publish(new SolveCompleted());
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
    }
}
