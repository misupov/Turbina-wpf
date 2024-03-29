﻿using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;

namespace Turbina.Nodes
{
    public class TimerNode : Node
    {
        private IDisposable _disposable;
        private bool _isEnabled;
        private TimeSpan _interval;
        private bool _highPrecision;

        [Input]
        public bool IsEnabled { get; set; }

        [Input]
        public TimeSpan Interval { get; set; }

        [Input]
        public bool HighPrecision { get; set; }

        [Output]
        public int Counter { get; private set; }

        [Output]
        public DateTimeOffset TickTime { get; private set; }

        public TimerNode()
        {
            _disposable = Disposable.Empty;
        }

        private void TimerCallback()
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
                var interval = Interval >= TimeSpan.FromMilliseconds(1) ? Interval : TimeSpan.FromMilliseconds(1);
                if (!_isEnabled || _interval != interval || _highPrecision != HighPrecision)
                {
                    _isEnabled = true;
                    _interval = interval;
                    _highPrecision = HighPrecision;
                    _disposable.Dispose();
                    if (HighPrecision)
                    {
                        _disposable = new NewThreadScheduler(start => new Thread(start) {IsBackground = true}).SchedulePeriodic(interval, TimerCallback);
                    }
                    else
                    {
                        Debug.WriteLine("Enable Timer");
                        _disposable = new Timer(state => TimerCallback(), null, interval, interval);
                    }
                    context.DoNotPulseFurther = true;
                }
                else
                {
                    Counter++;
                    TickTime = DateTimeOffset.Now;
                }
            }
            else if (_isEnabled)
            {
                Debug.WriteLine("Disable Timer");
                _isEnabled = false;
                _disposable.Dispose();
                context.DoNotPulseFurther = true;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _isEnabled = false;
            _disposable.Dispose();
        }
    }
}