using System;
using System.Windows.Threading;
using SpeedSlidingTrainer.Application.Infrastructure;

namespace SpeedSlidingTrainer.Desktop.Infrastructure
{
    public sealed class WpfDispatcher : IDispatcher
    {
        public void BeginInvoke(Action action)
        {
            App.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
