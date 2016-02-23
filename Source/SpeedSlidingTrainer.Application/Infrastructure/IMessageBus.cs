using System;

namespace SpeedSlidingTrainer.Application.Infrastructure
{
    public interface IMessageBus
    {
        void Subscribe<TMessage>(Action<TMessage> handler);

        void Publish<TMessage>(TMessage message);
    }
}
