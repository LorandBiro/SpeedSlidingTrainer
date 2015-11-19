namespace SpeedSlidingTrainer.Core.Model.State.Validation
{
    using System;

    public sealed class BoardPosition
    {
        public BoardPosition(int x, int y)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if (y < 0)
            {
                throw new ArgumentOutOfRangeException("y");
            }

            this.X = x;
            this.Y = y;
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public static BoardPosition FromIndex(int width, int height, int index)
        {
            return new BoardPosition(index % width, index / height);
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}", this.X, this.Y);
        }
    }
}
