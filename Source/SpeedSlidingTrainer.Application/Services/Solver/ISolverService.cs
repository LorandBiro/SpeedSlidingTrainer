using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SpeedSlidingTrainer.Application.Services.Solver
{
    public interface ISolverService : INotifyPropertyChanged
    {
        SolverServiceStatus Status { get; }

        ObservableCollection<SolutionStep> Solution { get; }

        int SolutionLength { get; }

        void StartSolveCurrentBoard();
    }
}
