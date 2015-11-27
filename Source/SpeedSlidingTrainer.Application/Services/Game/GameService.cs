using System;
using System.ComponentModel;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Events;
using SpeedSlidingTrainer.Application.Infrastructure;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;
using SpeedSlidingTrainer.Core.Services.BoardGenerator;

namespace SpeedSlidingTrainer.Application.Services.Game
{
    public class GameService : IGameService
    {
        [NotNull]
        private readonly IMessageQueue messageQueue;

        [NotNull]
        private readonly IBoardGeneratorService boardGeneratorService;

        [NotNull]
        private Drill drill;

        [NotNull]
        private BoardState boardState;

        public GameService([NotNull] IMessageQueue messageQueue, [NotNull] IBoardGeneratorService boardGeneratorService)
        {
            if (messageQueue == null)
            {
                throw new ArgumentNullException(nameof(messageQueue));
            }

            if (boardGeneratorService == null)
            {
                throw new ArgumentNullException(nameof(boardGeneratorService));
            }

            this.messageQueue = messageQueue;
            this.boardGeneratorService = boardGeneratorService;

            this.drill = Drill.CreateNew("Default", BoardTemplate.CreateEmpty(4, 4), BoardGoal.CreateCompleted(4, 4));
            this.InitialState = this.boardGeneratorService.Generate(this.drill.Template, this.drill.Goal);
            this.boardState = this.InitialState;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BoardState InitialState { get; private set; }

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
            if (!this.BoardState.CanSlideLeft)
            {
                return;
            }

            this.BoardState = this.BoardState.SlideLeft();
            this.messageQueue.Publish(new SlideHappened(Step.Left, this.BoardState.Satisfies(this.Drill.Goal)));
        }

        public void SlideUp()
        {
            if (!this.BoardState.CanSlideUp)
            {
                return;
            }

            this.BoardState = this.BoardState.SlideUp();
            this.messageQueue.Publish(new SlideHappened(Step.Up, this.BoardState.Satisfies(this.Drill.Goal)));
        }

        public void SlideRight()
        {
            if (!this.BoardState.CanSlideRight)
            {
                return;
            }

            this.BoardState = this.BoardState.SlideRight();
            this.messageQueue.Publish(new SlideHappened(Step.Right, this.BoardState.Satisfies(this.Drill.Goal)));
        }

        public void SlideDown()
        {
            if (!this.BoardState.CanSlideDown)
            {
                return;
            }

            this.BoardState = this.BoardState.SlideDown();
            this.messageQueue.Publish(new SlideHappened(Step.Down, this.BoardState.Satisfies(this.Drill.Goal)));
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
            this.InitialState = this.boardGeneratorService.Generate(this.drill.Template, this.drill.Goal);
            this.BoardState = this.InitialState;

            this.messageQueue.Publish(new BoardScrambled());
        }

        public void Reset()
        {
            this.BoardState = this.InitialState;

            this.messageQueue.Publish(new BoardResetted());
        }
    }
}
