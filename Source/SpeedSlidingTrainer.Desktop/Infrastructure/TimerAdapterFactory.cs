using System;
using SpeedSlidingTrainer.Application.Infrastructure;

namespace SpeedSlidingTrainer.Desktop.Infrastructure
{
    public sealed class TimerAdapterFactory : ITimerFactory
    {
        public ITimer Create(TimeSpan interval, Action onTick)
        {
            return new TimerAdapter(interval, onTick);
        }
    }
}
