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

        private readonly AggregatedSolveStatistics last5Statistics = new AggregatedSolveStatistics(5);

        private readonly AggregatedSolveStatistics last12Statistics = new AggregatedSolveStatistics(12);

        private readonly AggregatedSolveStatistics last50Statistics = new AggregatedSolveStatistics(50);

        private readonly AggregatedSolveStatistics last100Statistics = new AggregatedSolveStatistics(100);

        private readonly AggregatedSolveStatistics fullStatistics = new AggregatedSolveStatistics(null);

        [NotNull]
        private IReadOnlyList<SolveStatistics> lastSolves = new SolveStatistics[0];

        public SessionStatisticsService([NotNull] IMessageBus messageBus)
        {
            if (messageBus == null)
            {
                throw new ArgumentNullException(nameof(messageBus));
            }

            messageBus.Subscribe<SolveCompleted>(this.OnSolveCompleted);
            messageBus.Subscribe<DrillChanged>(this.OnDrillChanged);
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

        public IAggregatedSolveStatistics Last5Statistics => this.last5Statistics;

        public IAggregatedSolveStatistics Last12Statistics => this.last12Statistics;

        public IAggregatedSolveStatistics Last50Statistics => this.last50Statistics;

        public IAggregatedSolveStatistics Last100Statistics => this.last100Statistics;

        public IAggregatedSolveStatistics FullStatistics => this.fullStatistics;

        public void Clear()
        {
            this.lastSolvesQueue.Clear();
            this.LastSolves = new SolveStatistics[0];

            this.last5Statistics.Clear();
            this.last12Statistics.Clear();
            this.last50Statistics.Clear();
            this.last100Statistics.Clear();
            this.fullStatistics.Clear();
        }

        private void OnSolveCompleted(SolveCompleted message)
        {
            this.lastSolvesQueue.Enqueue(message.Statistics);
            while (this.lastSolvesQueue.Count > 5)
            {
                this.lastSolvesQueue.Dequeue();
            }

            this.LastSolves = this.lastSolvesQueue.Reverse().ToList();

            this.last5Statistics.Append(message.Statistics);
            this.last12Statistics.Append(message.Statistics);
            this.last50Statistics.Append(message.Statistics);
            this.last100Statistics.Append(message.Statistics);
            this.fullStatistics.Append(message.Statistics);
        }

        private void OnDrillChanged(DrillChanged message)
        {
            this.Clear();
        }
    }
}