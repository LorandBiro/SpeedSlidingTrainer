using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Events
{
    public sealed class SlideHappened
    {
        public SlideHappened(Step step)
        {
            this.Step = step;
        }

        public Step Step { get; }
    }
}
