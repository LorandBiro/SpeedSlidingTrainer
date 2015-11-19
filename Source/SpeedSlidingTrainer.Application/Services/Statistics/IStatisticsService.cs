using System;
using System.ComponentModel;

namespace SpeedSlidingTrainer.Application.Services.Statistics
{
    public interface IStatisticsService : INotifyPropertyChanged
    {
        int StepCount { get; }

        TimeSpan Duration { get; }
    }
}