using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Events
{
    public sealed class SlideHappened
    {
        public SlideHappened(Step step, bool boardSolved)
        {
            this.Step = step;
            this.BoardSolved = boardSolved;
        }

        public Step Step { get; }

        public bool BoardSolved { get; }
    }
}
