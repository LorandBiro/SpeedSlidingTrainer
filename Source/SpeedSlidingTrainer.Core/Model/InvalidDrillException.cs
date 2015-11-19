using System;

namespace SpeedSlidingTrainer.Core.Model
{
    public sealed class InvalidDrillException : Exception
    {
        public InvalidDrillException(string message)
            : base(message)
        {
        }
    }
}
