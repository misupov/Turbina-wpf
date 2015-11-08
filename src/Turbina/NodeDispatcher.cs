using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Turbina
{
    public class NodeDispatcher
    {
        private readonly IScheduler _scheduler;

        public NodeDispatcher(string threadName = null)
        {
            _scheduler = new EventLoopScheduler(start => new Thread(start) {Name = threadName, IsBackground = true});
        }

        public void Invoke(Action action)
        {
            var eventSlim = new ManualResetEventSlim();
            _scheduler.Schedule(() =>
            {
                action();
                eventSlim.Set();
            });
            eventSlim.Wait();
        }

        public TResult Invoke<TResult>(Func<TResult> action)
        {
            var result = default(TResult);
            var eventSlim = new ManualResetEventSlim();
            _scheduler.Schedule(() =>
            {
                result = action();
                eventSlim.Set();
            });
            eventSlim.Wait();
            return result;
        }

        public void InvokeAsync(Action action)
        {
            _scheduler.Schedule(action);
        }
    }
}