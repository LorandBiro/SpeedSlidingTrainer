using System;

namespace SpeedSlidingTrainer.Core.Model
{
    public sealed class SolveStatistics
    {
        public SolveStatistics(TimeSpan time, int moves, int? optimalMoves)
        {
            this.Time = time;
            this.Moves = moves;
            this.Tps = moves / time.TotalSeconds;
            if (optimalMoves.HasValue)
            {
                this.Efficiency = optimalMoves.Value / (double)moves;
            }
        }

        public TimeSpan Time { get; private set; }

        public int Moves { get; private set; }

        public double Tps { get; private set; }

        public double? Efficiency { get; private set; }
    }
}
