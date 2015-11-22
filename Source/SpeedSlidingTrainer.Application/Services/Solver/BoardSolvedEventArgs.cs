using System;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Application.Services.Solver
{
    public sealed class BoardSolvedEventArgs : EventArgs
    {
        public BoardSolvedEventArgs([NotNull] BoardState state, [NotNull] Step[][] solutions)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (solutions == null)
            {
                throw new ArgumentNullException(nameof(solutions));
            }

            this.State = state;
            this.Solutions = solutions;
        }

        [NotNull]
        public BoardState State { get; }

        [NotNull]
        public Step[][] Solutions { get; }
    }
}
