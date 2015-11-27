using System;
using System.ComponentModel;

namespace SpeedSlidingTrainer.Application.Services.SessionStatistics
{
    public interface IAggregatedSolveStatistics : INotifyPropertyChanged
    {
        TimeSpan? Time { get; }

        double? Moves { get; }

        double? Tps { get; }

        double? Efficiency { get; }
    }
}