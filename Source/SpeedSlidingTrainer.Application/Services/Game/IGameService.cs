using System.ComponentModel;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Application.Services.Game
{
    public interface IGameService : INotifyPropertyChanged
    {
        [NotNull]
        BoardState InitialState { get; }

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