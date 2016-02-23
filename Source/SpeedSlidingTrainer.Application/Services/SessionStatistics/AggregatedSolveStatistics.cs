using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.SessionStatistics
{
    internal sealed class AggregatedSolveStatistics : IAggregatedSolveStatistics
    {
        private readonly Queue<SolveStatistics> solveStatisticsQueue = new Queue<SolveStatistics>();

        private readonly int? limit;

        private TimeSpan? time;

        private double? moves;

        private double? tps;

        private double? efficiency;

        public AggregatedSolveStatistics(int? limit)
        {
            this.limit = limit;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan? Time
        {
            get
            {
                return this.time;
            }

            private set
            {
                this.time = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Time)));
            }
        }

        public double? Moves
        {
            get
            {
                return this.moves;
            }

            private set
            {
                this.moves = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Moves)));
            }
        }

        public double? Tps
        {
            get
            {
                return this.tps;
            }

            private set
            {
                this.tps = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Tps)));
            }
        }

        public double? Efficiency
        {
            get
            {
                return this.efficiency;
            }

            private set
            {
                this.efficiency = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Efficiency)));
            }
        }

        public void Append([NotNull] SolveStatistics solveStatistics)
        {
            if (solveStatistics == null)
            {
                throw new ArgumentNullException(nameof(solveStatistics));
            }

            this.solveStatisticsQueue.Enqueue(solveStatistics);
            if (this.limit.HasValue)
            {
                while (this.solveStatisticsQueue.Count > this.limit.Value)
                {
                    this.solveStatisticsQueue.Dequeue();
                }
            }

            this.Time = TimeSpan.FromSeconds(this.solveStatisticsQueue.Average(x => x.Time.TotalSeconds));
            this.Moves = this.solveStatisticsQueue.Average(x => x.Moves);
            this.Tps = this.solveStatisticsQueue.Average(x => x.Tps);
            this.Efficiency = this.solveStatisticsQueue.All(x => x.Efficiency.HasValue)
                                  ? (double?)this.solveStatisticsQueue.Average(x => x.Efficiency.Value)
                                  : null;
        }

        public void Clear()
        {
            this.solveStatisticsQueue.Clear();
            this.Time = null;
            this.Moves = null;
            this.Tps = null;
            this.Efficiency = null;
        }
    }
}