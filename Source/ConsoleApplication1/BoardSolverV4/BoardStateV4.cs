using System;
using SpeedSlidingTrainer.Core.Model.State;

namespace ConsoleApplication1.BoardSolverV4
{
    public sealed class BoardStateV4
    {
        private readonly int eye;

        public BoardStateV4(int width, int height, int[] values, int eye)
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

        public BoardStateV4 SlideLeft()
        {
            return this.Slide(this.eye + 1);
        }

        public BoardStateV4 SlideUp()
        {
            return this.Slide(this.eye + this.Width);
        }

        public BoardStateV4 SlideRight()
        {
            return this.Slide(this.eye - 1);
        }

        public BoardStateV4 SlideDown()
        {
            return this.Slide(this.eye - this.Width);
        }

        private BoardStateV4 Slide(int newEyeIndex)
        {
            int[] newValues = (int[])this.Values.Clone();
            newValues[this.eye] = this.Values[newEyeIndex];
            newValues[newEyeIndex] = this.Values[this.eye];

            return new BoardStateV4(this.Width, this.Height, newValues, newEyeIndex);
        }
    }
}
