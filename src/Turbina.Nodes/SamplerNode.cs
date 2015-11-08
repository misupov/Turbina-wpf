using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;

namespace Turbina.Nodes
{
    public class SamplerNode : Node
    {
        private IDisposable _disposable;
        private bool _isEnabled;
        private TimeSpan _interval;
        private bool _needUpdate;

        [Input]
        public bool IsEnabled { get; set; }

        [Input]
        public TimeSpan Interval { get; set; }

        [Input]
        public object Input { get; set; }

        [Output]
        public object Output { get; private set; }

        public SamplerNode()
        {
            _disposable = Disposable.Empty;
        }

        private void TimerCallback()
        {
            _needUpdate = true;
            Pulse();
        }

        protected override void Reset()
        {
            Output = null;
        }

        protected override void Process(ProcessingContext context)
        {
            if (IsEnabled)
            {
                if (!_isEnabled || _interval != Interval)
                {
                    _isEnabled = true;
                    _interval = Interval;
                    _disposable.Dispose();
                    _disposable = new NewThreadScheduler(start => new Thread(start) {IsBackground = true}).SchedulePeriodic(Interval, TimerCallback);
                    context.DoNotPulseFurther = true;
                }
                else
                {
                    if (_needUpdate)
                    {
                        Output = Input;
                        _needUpdate = false;
                    }
                    else
                    {
                        context.DoNotPulseFurther = true;
                    }
                }
            }
            else if (_isEnabled)
            {
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