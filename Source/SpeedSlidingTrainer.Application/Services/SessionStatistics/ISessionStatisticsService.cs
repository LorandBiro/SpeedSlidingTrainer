using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.SessionStatistics
{
    public interface ISessionStatisticsService : INotifyPropertyChanged
    {
        [NotNull]
        IReadOnlyList<SolveStatistics> LastSolves { get; }

        [NotNull]
        IAggregatedSolveStatistics Last5Statistics { get; }

        [NotNull]
        IAggregatedSolveStatistics Last12Statistics { get; }

        [NotNull]
        IAggregatedSolveStatistics Last50Statistics { get; }

        [NotNull]
        IAggregatedSolveStatistics Last100Statistics { get; }

        [NotNull]
        IAggregatedSolveStatistics FullStatistics { get; }

        void Clear();
    }
}
