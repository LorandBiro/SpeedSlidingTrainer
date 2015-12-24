using System;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.Core.Model.State
{
    public sealed class BoardTemplate : BoardStateBase
    {
        public BoardTemplate(int width, int height, [NotNull] int[] values)
            : base(width, height, values, ValidationType.BoardTemplate)
        {
        }

        public static BoardTemplate CreateEmpty(int width, int height)
        {
            return new BoardTemplate(width, height, new int[width * height]);
        }

        public bool Satisfies([NotNull] BoardGoal goal)
        {
            if (goal == null)
            {
                throw new ArgumentNullException(nameof(goal));
            }

            for (int i = 0; i < this.TileCount; i++)
            {
                if (goal[i] != 0 && goal[i] != this[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
