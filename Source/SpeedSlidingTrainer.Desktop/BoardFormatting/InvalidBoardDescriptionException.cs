using System;

namespace SpeedSlidingTrainer.Desktop.BoardFormatting
{
    [Serializable]
    public sealed class InvalidBoardDescriptionException : Exception
    {
        public InvalidBoardDescriptionException(string message)
            : base(message)
        {
        }
    }
}
