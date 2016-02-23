using System;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV5
{
    public sealed class BoardStateV5
    {
        private readonly int eye;

        public BoardStateV5(int width, int height, int[] values, int eye)
        {
            this.Width = width;
            this.Height = height;
            this.Values = values;
            this.eye = eye;
        }

        public int Width { get; }

        public int Height { get; }

        public int[] Values { get; }

        public bool CanSlideLeft
        {
            get
            {
                return (this.eye % this.Width) < this.Width - 1;
            }
        }

        public bool CanSlideUp
        {
            get
            {
                return (this.eye / this.Width) < this.Height - 1;
            }
        }

        public bool CanSlideRight
        {
            get
            {
                return (this.eye % this.Width) > 0;
            }
        }

        public bool CanSlideDown
        {
            get
            {
                return (this.eye / this.Width) > 0;
            }
        }

        public bool Satisfies(BoardGoal goal)
        {
            if (goal == null)
            {
                throw new ArgumentNullException(nameof(goal));
            }

            for (int i = 0; i < goal.TileCount; i++)
            {
                if (goal[i] != 0 && goal[i] != this.Values[i])
                {
                    return false;
                }
            }

            return true;
        }

        public BoardStateV5 SlideLeft()
        {
            return this.Slide(this.eye + 1);
        }

        public BoardStateV5 SlideUp()
        {
            return this.Slide(this.eye + this.Width);
        }

        public BoardStateV5 SlideRight()
        {
            return this.Slide(this.eye - 1);
        }

        public BoardStateV5 SlideDown()
        {
            return this.Slide(this.eye - this.Width);
        }

        private BoardStateV5 Slide(int newEyeIndex)
        {
            int[] newValues = (int[])this.Values.Clone();
            newValues[this.eye] = this.Values[newEyeIndex];
            newValues[newEyeIndex] = this.Values[this.eye];

            return new BoardStateV5(this.Width, this.Height, newValues, newEyeIndex);
        }
    }
}
