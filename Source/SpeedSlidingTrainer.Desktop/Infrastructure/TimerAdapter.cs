using System;
using System.Windows.Threading;
using SpeedSlidingTrainer.Application.Infrastructure;

namespace SpeedSlidingTrainer.Desktop.Infrastructure
{
    public sealed class TimerAdapter : ITimer
    {
        private readonly DispatcherTimer timer;

        public TimerAdapter(TimeSpan interval, Action onTick)
        {
            this.timer = new DispatcherTimer { Interval = interval };
            this.timer.Tick += (sender, e) => onTick();
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
        }
    }
}
