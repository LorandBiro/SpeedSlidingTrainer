using System;
using System.ComponentModel;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.Solver
{
    public sealed class SolutionStep : INotifyPropertyChanged
    {
        private SolutionStepStatus status;

        public SolutionStep(Step step, SolutionStepStatus status)
        {
            this.Step = step;
            this.status = status;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Step Step { get; }

        public SolutionStepStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                if (this.status == value)
                {
                    return;
                }

                this.status = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Status)));
            }
        }
    }
}
