using System;

namespace SpeedSlidingTrainer.Core.Services.BoardGenerator
{
    public sealed class BoardGenerationException : Exception
    {
        public BoardGenerationException(string message)
            : base(message)
        {
        }
    }
}
