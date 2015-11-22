using System;

namespace SpeedSlidingTrainer.Desktop.Converters
{
    public sealed class SwitchConverterException : Exception
    {
        public SwitchConverterException(string message)
            : base(message)
        {
        }
    }
}
