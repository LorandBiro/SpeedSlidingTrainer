using System;
using System.ComponentModel;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Application.Services.Game
{
    public interface IGameService : INotifyPropertyChanged
    {
        event EventHandler<SlidEventArgs> Slid;

        event EventHandler SolveStarted;

        event EventHandler SolveCompleted;

        event EventHandler Resetted;

        event EventHandler Scrambled;

        SolveStatus Status { get; }

        [NotNull]
        BoardState StartState { get; }

        [NotNull]
        BoardState BoardState { get; }

        [NotNull]
        Drill Drill { get; }

        void SlideLeft();

        void SlideUp();

        void SlideRight();

        void SlideDown();

        void SetDrill([NotNull] Drill drill);

        void Scramble();

        void Reset();
    }
}