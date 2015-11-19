using System;
using System.ComponentModel;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;
using SpeedSlidingTrainer.Core.Services;

namespace SpeedSlidingTrainer.Application.Services.Game
{
    public class GameService : IGameService
    {
        [NotNull]
        private readonly IBoardGeneratorService boardGeneratorService;

        [NotNull]
        private Drill drill;

        [NotNull]
        private Board board;

        private SolveStatus status;

        [NotNull]
        private BoardState boardState;

        public GameService([NotNull] IBoardGeneratorService boardGeneratorService)
        {
            if (boardGeneratorService == null)
            {
                throw new ArgumentNullException(nameof(boardGeneratorService));
            }

            this.boardGeneratorService = boardGeneratorService;

            this.StartState = BoardState.CreateCompleted(4, 4);
            this.drill = Drill.CreateNew(
                "Default",
                new BoardTemplate(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 13, 0, 0, 0 }),
                new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 13, 14, 0, 0 }));

            this.board = new Board(this.StartState, this.drill.Goal);
            this.boardState = this.board.State;

            this.Status = SolveStatus.Completed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<SlidEventArgs> Slid;

        public event EventHandler SolveStarted;

        public event EventHandler SolveCompleted;

        public event EventHandler Resetted;

        public event EventHandler Scrambled;

        public SolveStatus Status
        {
            get
            {
                return this.status;
            }

            private set
            {
                if (this.Status == value)
                {
                    return;
                }

                this.status = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Status)));
            }
        }

        public BoardState StartState { get; private set; }

        public BoardState BoardState
        {
            get
            {
                return this.boardState;
            }

            private set
            {
                this.boardState = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.BoardState)));
            }
        }

        public Drill Drill
        {
            get
            {
                return this.drill;
            }

            private set
            {
                this.drill = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Drill)));
            }
        }

        public void SlideLeft()
        {
            if (!this.board.CanSlideLeft)
            {
                return;
            }

            this.board.SlideLeft();
            this.OnSlide(Step.Left);
        }

        public void SlideUp()
        {
            if (!this.board.CanSlideUp)
            {
                return;
            }

            this.board.SlideUp();
            this.OnSlide(Step.Up);
        }

        public void SlideRight()
        {
            if (!this.board.CanSlideRight)
            {
                return;
            }

            this.board.SlideRight();
            this.OnSlide(Step.Right);
        }

        public void SlideDown()
        {
            if (!this.board.CanSlideDown)
            {
                return;
            }

            this.board.SlideDown();
            this.OnSlide(Step.Down);
        }

        public void SetDrill(Drill drill)
        {
            if (drill == null)
            {
                throw new ArgumentNullException(nameof(drill));
            }

            this.Drill = drill;
            this.Scramble();
        }

        public void Scramble()
        {
            this.StartState = this.boardGeneratorService.Generate(this.drill.Template);
            this.Reset();
            this.Scrambled?.Invoke(this, EventArgs.Empty);
        }

        public void Reset()
        {
            this.board = new Board(this.StartState, this.drill.Goal);
            this.BoardState = this.board.State;

            this.Status = SolveStatus.NotStarted;
            this.Resetted?.Invoke(this, EventArgs.Empty);
        }

        private void OnSlide(Step step)
        {
            if (this.Status == SolveStatus.NotStarted)
            {
                this.Status = SolveStatus.InProgress;
                this.SolveStarted?.Invoke(this, EventArgs.Empty);
            }

            this.Slid?.Invoke(this, new SlidEventArgs(step));
            this.BoardState = this.board.State;

            if (this.Status == SolveStatus.InProgress)
            {
                if (this.board.IsComplete)
                {
                    this.Status = SolveStatus.Completed;
                    this.SolveCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
