using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.Core.Model.State
{
    public sealed class BoardGoal : BoardStateBase
    {
        public BoardGoal(int width, int height, [NotNull] int[] values)
            : base(width, height, values, ValidationType.BoardGoal)
        {
        }

        public static BoardGoal CreateCompleted(int width, int height)
        {
            int[] values = new int[width * height];
            for (int i = 0; i < values.Length - 1; i++)
            {
                values[i] = i + 1;
            }

            values[values.Length - 1] = 0;

            return new BoardGoal(width, height, values);
        }
    }
}
