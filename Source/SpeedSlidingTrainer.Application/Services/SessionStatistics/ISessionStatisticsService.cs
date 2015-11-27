using System;
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
    }
}
