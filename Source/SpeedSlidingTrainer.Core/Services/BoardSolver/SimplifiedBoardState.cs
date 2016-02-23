using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Core.Services.BoardSolver
{
    internal sealed class SimplifiedBoardState
    {
        private readonly int[] values;

        private readonly int eye;

        private SimplifiedBoardState(int width, int height, int[] values, int eye)
        {
            this.Width = width;
            this.Height = height;
            this.values = values;
            this.eye = eye;
        }

        public int Width { get; }

        public int Height { get; }

        public int TileCount
        {
            get { return this.values.Length; }
        }

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

        public int this[int index]
        {
            get
            {
                return this.values[index];
            }
        }

        public static SimplifiedBoardState Create(BoardState state, BoardGoal goal)
        {
            int[] values = new int[state.TileCount];
            int eye = 0;

            for (int i = 0; i < state.TileCount; i++)
            {
                if (state[i] == 0)
                {
                    eye = i;
                    values[i] = -1;
                    continue;
                }

                for (int j = 0; j < goal.TileCount; j++)
                {
                    if (state[i] == goal[j])
                    {
                        values[i] = state[i];
                        break;
                    }
                }
            }

            return new SimplifiedBoardState(state.Width, state.Height, values, eye);
        }

        public SimplifiedBoardState SlideLeft()
        {
            return this.Slide(this.eye + 1);
        }

        public SimplifiedBoardState SlideUp()
        {
            return this.Slide(this.eye + this.Width);
        }

        public SimplifiedBoardState SlideRight()
        {
            return this.Slide(this.eye - 1);
        }

        public SimplifiedBoardState SlideDown()
        {
            return this.Slide(this.eye - this.Width);
        }

        private SimplifiedBoardState Slide(int newEyeIndex)
        {
            int[] newValues = (int[])this.values.Clone();
            newValues[this.eye] = this.values[newEyeIndex];
            newValues[newEyeIndex] = this.values[this.eye];

            return new SimplifiedBoardState(this.Width, this.Height, newValues, newEyeIndex);
        }
    }
}
