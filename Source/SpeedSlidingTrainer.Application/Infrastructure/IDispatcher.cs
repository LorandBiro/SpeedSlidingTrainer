using System;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Application.Infrastructure
{
    public interface IDispatcher
    {
        void BeginInvoke([NotNull] Action action);
    }
}
