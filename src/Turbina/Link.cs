using System.Collections.Concurrent;
using System.Diagnostics;
using QuickGraph;

namespace Turbina
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public class Link : IEdge<Node>
    {
        private readonly BlockingCollection<object> _values = new BlockingCollection<object>(20);

        public Link(Node source, IPin sourcePin, Node target, IPin targetPin)
        {
            Source = source;
            SourcePin = sourcePin;
            Target = target;
            TargetPin = targetPin;
        }

        public void PushValue()
        {
            _values.Add(SourcePin.GetValue());
        }

        public void PullValue(NodeDispatcher dispatcher)
        {
            if (dispatcher == Source.Dispatcher)
            {
                object value;
                if (!_values.TryTake(out value))
                {
                    value = SourcePin.GetValue();
                }

                TargetPin.SetValue(value);
            }
            else
            {
                TargetPin.SetValue(_values.Take());
            }
        }

        public Node Source { get; }
        public IPin SourcePin { get; set; }
        public Node Target { get; }
        public IPin TargetPin { get; set; }

        private string GetDebuggerDisplay()
        {
            return $"{Source.Title}.{SourcePin.Name} -> {Target.Title}.{TargetPin.Name}";
        }
    }
}