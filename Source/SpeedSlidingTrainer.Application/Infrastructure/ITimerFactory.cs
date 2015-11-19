using System;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Application.Infrastructure
{
    public interface ITimerFactory
    {
        [NotNull]
        ITimer Create(TimeSpan interval, [NotNull] Action onTick);
    }
}
