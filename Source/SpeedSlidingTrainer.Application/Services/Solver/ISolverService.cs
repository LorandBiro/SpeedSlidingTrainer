using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Application.Services.Solver
{
    public interface ISolverService : INotifyPropertyChanged
    {
        SolverServiceStatus Status { get; }

        [CanBeNull]
        IReadOnlyCollection<IReadOnlyList<SolutionStep>> Solutions { get; }

        int SolutionLength { get; }

        void StartSolveCurrentBoard();
    }
}
