﻿using System;
using System.ComponentModel;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.SolveState
{
    public interface ISolveStateService : INotifyPropertyChanged
    {
        SolveStatus Status { get; }

        int StepCount { get; }

        int? OptimalStepCount { get; }

        TimeSpan Duration { get; }
    }
}