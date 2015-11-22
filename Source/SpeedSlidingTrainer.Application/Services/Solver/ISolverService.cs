using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Application.Services.Solver
{
    public interface ISolverService : INotifyPropertyChanged
    {
        event EventHandler<BoardSolvedEventArgs> BoardSolved;

        SolverServiceStatus Status { get; }

        [CanBeNull]
        IReadOnlyList<IReadOnlyList<SolutionStep>> Solutions { get; }

        int? SolutionLength { get; }

        void StartSolveCurrentBoard();
    }
}
