using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Events;
using SpeedSlidingTrainer.Application.Infrastructure;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.SessionStatistics
{
    public sealed class SessionStatisticsService : ISessionStatisticsService
    {
        [NotNull]
        private readonly Queue<SolveStatistics> lastSolvesQueue = new Queue<SolveStatistics>();

        [NotNull]
        private IReadOnlyList<SolveStatistics> lastSolves = new SolveStatistics[0];

        public SessionStatisticsService([NotNull] IMessageBus messageBus)
        {
            if (messageBus == null)
            {
                throw new ArgumentNullException(nameof(messageBus));
            }

            messageBus.Subscribe<SolveCompleted>(this.OnSolveCompleted);
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void OnSolveCompleted(SolveCompleted message)
        {
            this.lastSolvesQueue.Enqueue(message.Statistics);
            while (this.lastSolvesQueue.Count > 5)
            {
                this.lastSolvesQueue.Dequeue();
            }

            this.LastSolves = this.lastSolvesQueue.Reverse().ToList();
        }
    }
}