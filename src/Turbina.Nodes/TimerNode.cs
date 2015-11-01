using System;
using System.Diagnostics;
using System.Threading;

namespace Turbina.Nodes
{
    public class TimerNode : Node
    {
        private readonly Timer _timer;
        private bool _isEnabled;
        private TimeSpan _interval;

        [Input]
        public bool IsEnabled { get; set; }

        [Input]
        public TimeSpan Interval { get; set; }

        [Output]
        public int Counter { get; private set; }

        [Output]
        public DateTimeOffset TickTime { get; private set; }

        public TimerNode()
        {
            _timer = new Timer(TimerCallback);
        }

        private void TimerCallback(object state)
        {
            Pulse();
        }

        protected override void Reset()
        {
            Counter = 0;
            TickTime = default(DateTimeOffset);
        }

        protected override void Process(ProcessingContext context)
        {
            if (IsEnabled)
            {
                if (!_isEnabled || _interval != Interval)
                {
                    _isEnabled = true;
                    _interval = Interval;
                    _timer.Change(TimeSpan.Zero, Interval);
                    context.DoNotPulseFurther = true;
                }
                else
                {
                    Counter++;
                    TickTime = DateTimeOffset.Now;
                    Debug.WriteLine($"[{Thread.CurrentThread.Name}] {Counter}");
                }
            }
            else if (_isEnabled)
            {
                _isEnabled = false;
                _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                context.DoNotPulseFurther = true;
            }
        }
    }
}
