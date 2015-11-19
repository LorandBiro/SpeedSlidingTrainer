namespace SpeedSlidingTrainer.Core.Model
{
    using System;

    public sealed class SolveStats
    {
        public SolveStats(DateTime date, TimeSpan duration, int moveCount)
        {
            this.Date = date;
            this.Duration = duration;
            this.MoveCount = moveCount;
        }

        public DateTime Date { get; private set; }

        public TimeSpan Duration { get; private set; }

        public int MoveCount { get; private set; }
    }
}
