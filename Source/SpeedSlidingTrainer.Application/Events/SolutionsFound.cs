using System;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Application.Events
{
    public sealed class SolutionsFound
    {
        public SolutionsFound([NotNull] BoardState initialState, [NotNull] BoardState solvedState, [NotNull] Step[][] solutions)
        {
            if (solvedState == null)
            {
                throw new ArgumentNullException(nameof(solvedState));
            }

            if (solutions == null)
            {
                throw new ArgumentNullException(nameof(solutions));
            }

            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            this.SolvedState = solvedState;
            this.Solutions = solutions;
            this.InitialState = initialState;
        }

        [NotNull]
        public BoardState InitialState { get; }

        [NotNull]
        public BoardState SolvedState { get; }

        [NotNull]
        public Step[][] Solutions { get; }
    }
}
