using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SpeedSlidingTrainer.Application.Services.Statistics
{
    public interface IStatisticsService : INotifyPropertyChanged
    {
        int StepCount { get; }

        int? OptimalStepCount { get; }

        TimeSpan Duration { get; }

        IReadOnlyList<SolveStatistics> LastSolves { get; }
    }
}