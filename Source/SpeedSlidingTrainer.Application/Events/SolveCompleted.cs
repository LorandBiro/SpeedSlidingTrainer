using System;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Events
{
    public sealed class SolveCompleted
    {
        public SolveCompleted([NotNull] SolveStatistics statistics)
        {
            if (statistics == null)
            {
                throw new ArgumentNullException(nameof(statistics));
            }

            this.Statistics = statistics;
        }

        [NotNull]
        public SolveStatistics Statistics { get; }
    }
}
