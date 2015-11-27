using System;

namespace SpeedSlidingTrainer.Desktop.Converters
{
    [Serializable]
    public sealed class SwitchConverterException : Exception
    {
        public SwitchConverterException(string message)
            : base(message)
        {
        }
    }
}
