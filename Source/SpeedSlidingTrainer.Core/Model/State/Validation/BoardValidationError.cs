using System;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Core.Model.State.Validation
{
    public sealed class BoardValidationError
    {
        public BoardValidationError(BoardValidationErrorType errorType, string message)
            : this(errorType, message, null)
        {
        }

        public BoardValidationError(BoardValidationErrorType errorType, [NotNull] string message, [CanBeNull] BoardPosition position)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.ErrorType = errorType;
            this.Message = message;
            this.Position = position;
        }

        public BoardValidationErrorType ErrorType { get; private set; }

        [NotNull]
        public string Message { get; private set; }

        [CanBeNull]
        public BoardPosition Position { get; private set; }

        public override string ToString()
        {
            return string.Format("ErrorType: {0}, Message: {1}, Position: {2}", this.ErrorType, this.Message, this.Position);
        }
    }
}
