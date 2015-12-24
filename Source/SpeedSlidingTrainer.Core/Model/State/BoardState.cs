using System;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.Core.Model.State
{
    public sealed class BoardState : BoardStateBase
    {
        private readonly int eye;

        public BoardState(int width, int height, [NotNull] int[] values)
            : base(width, height, values, ValidationType.BoardState)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 0)
                {
                    this.eye = i;
                    break;
                }
            }
        }

        private BoardState(int width, int height, [NotNull] int[] values, int eye)
            : base(width, height, values)
        {
            this.eye = eye;
        }

        public bool CanSlideLeft => (this.eye % this.Width) < this.Width - 1;

        public bool CanSlideUp => (this.eye / this.Width) < this.Height - 1;

        public bool CanSlideRight => (this.eye % this.Width) > 0;

        public bool CanSlideDown => (this.eye / this.Width) > 0;

        [NotNull]
        public static BoardState CreateCompleted(int width, int height)
        {
            if (width < 2)
            {
                throw new ArgumentException("Width must be at least 2.", nameof(width));
            }

            if (height < 2)
            {
                throw new ArgumentException("Height must be at least 2.", nameof(height));
            }

            int[] values = new int[width * height];
            for (int i = 0; i < values.Length - 1; i++)
            {
                values[i] = i + 1;
            }

            values[values.Length - 1] = 0;

            return new BoardState(width, height, values, values.Length - 1);
        }

        public bool Satisfies([NotNull] BoardGoal goal)
        {
            if (goal == null)
            {
                throw new ArgumentNullException(nameof(goal));
            }

            for (int i = 0; i < goal.TileCount; i++)
            {
                if (goal[i] != 0 && goal[i] != this[i])
                {
                    return false;
                }
            }

            return true;
        }

        [NotNull]
        public BoardState SlideLeft()
        {
            if (!this.CanSlideLeft)
            {
                throw new InvalidOperationException();
            }

            return this.Slide(this.eye + 1);
        }

        [NotNull]
        public BoardState SlideUp()
        {
            if (!this.CanSlideUp)
            {
                throw new InvalidOperationException();
            }

            return this.Slide(this.eye + this.Width);
        }

        [NotNull]
        public BoardState SlideRight()
        {
            if (!this.CanSlideRight)
            {
                throw new InvalidOperationException();
            }

            return this.Slide(this.eye - 1);
        }

        [NotNull]
        public BoardState SlideDown()
        {
            if (!this.CanSlideDown)
            {
                throw new InvalidOperationException();
            }

            return this.Slide(this.eye - this.Width);
        }

        [NotNull]
        private BoardState Slide(int newEyeIndex)
        {
            int[] newValues = this.GetValues();
            newValues[this.eye] = this[newEyeIndex];
            newValues[newEyeIndex] = this[this.eye];

            return new BoardState(this.Width, this.Height, newValues, newEyeIndex);
        }
    }
}
