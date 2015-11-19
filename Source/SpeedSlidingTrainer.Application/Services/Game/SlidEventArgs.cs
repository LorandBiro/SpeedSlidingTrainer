namespace SpeedSlidingTrainer.Application.Services.Game
{
    using System;
    using SpeedSlidingTrainer.Core.Model;

    public sealed class SlidEventArgs : EventArgs
    {
        public SlidEventArgs(Step step)
        {
            this.Step = step;
        }

        public Step Step { get; private set; }
    }
}
