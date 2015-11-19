namespace SpeedSlidingTrainer.Core.Model
{
    using System;
    using JetBrains.Annotations;
    using SpeedSlidingTrainer.Core.Model.State;

    public sealed class Board
    {
        public Board([NotNull] BoardState state, [NotNull] BoardGoal goal)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            if (goal == null)
            {
                throw new ArgumentNullException("goal");
            }

            this.State = state;
            this.Goal = goal;
        }

        [NotNull]
        public BoardState State { get; private set; }

        [NotNull]
        public BoardGoal Goal { get; set; }

        public int Width
        {
            get
            {
                return this.State.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.State.Height;
            }
        }

        public bool CanSlideLeft
        {
            get
            {
                return this.State.CanSlideLeft;
            }
        }

        public bool CanSlideUp
        {
            get
            {
                return this.State.CanSlideUp;
            }
        }

        public bool CanSlideRight
        {
            get
            {
                return this.State.CanSlideRight;
            }
        }

        public bool CanSlideDown
        {
            get
            {
                return this.State.CanSlideDown;
            }
        }

        public bool IsComplete
        {
            get
            {
                return this.State.Satisfies(this.Goal);
            }
        }

        public void SlideLeft()
        {
            this.State = this.State.SlideLeft();
        }

        public void SlideUp()
        {
            this.State = this.State.SlideUp();
        }

        public void SlideRight()
        {
            this.State = this.State.SlideRight();
        }

        public void SlideDown()
        {
            this.State = this.State.SlideDown();
        }

        public override string ToString()
        {
            return this.State.ToString();
        }
    }
}
