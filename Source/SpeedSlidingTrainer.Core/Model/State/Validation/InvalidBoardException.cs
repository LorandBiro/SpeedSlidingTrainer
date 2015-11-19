namespace SpeedSlidingTrainer.Core.Model.State.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    public sealed class InvalidBoardException : Exception
    {
        public InvalidBoardException([NotNull] [ItemNotNull] IEnumerable<BoardValidationError> errors)
            : base("The board is invalid.")
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            this.Errors = errors.ToList();
        }

        [NotNull]
        [ItemNotNull]
        public IReadOnlyCollection<BoardValidationError> Errors { get; private set; }
    }
}
