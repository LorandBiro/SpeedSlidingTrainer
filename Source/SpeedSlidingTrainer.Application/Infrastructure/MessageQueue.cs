using System;
using System.Collections.Concurrent;

namespace SpeedSlidingTrainer.Application.Infrastructure
{
    public sealed class MessageQueue : IMessageQueue
    {
        private readonly ConcurrentDictionary<Type, Action<object>[]> handlersByType = new ConcurrentDictionary<Type, Action<object>[]>();

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            Action<object> objectHandler = eventObject => handler((TEvent)eventObject);
            this.handlersByType.AddOrUpdate(typeof(TEvent), eventType => new[] { objectHandler }, (eventType, current) => AppendToArray(current, objectHandler));
        }

        public void Publish<TEvent>(TEvent message)
        {
            Action<object>[] handlers;
            if (this.handlersByType.TryGetValue(typeof(TEvent), out handlers))
            {
                foreach (Action<object> handler in handlers)
                {
                    handler(message);
                }
            }
        }

        private static T[] AppendToArray<T>(T[] array, T newItem)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = newItem;
            return newArray;
        }
    }
}