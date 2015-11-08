using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Turbina
{
    [DebuggerDisplay("Node: {Title,nq} ({GetType().Name,nq})")]
    public abstract class Node : IDisposable
    {
        private NodeDispatcher _dispatcher;
        private readonly List<IPin> _inputPins = new List<IPin>();
        private readonly List<IPin> _outputPins = new List<IPin>();

        public event EventHandler Processed;
        public event EventHandler PinsChanged;

        protected Node()
        {
            InitializePins();
        }

        protected virtual void InitializePins()
        {
            foreach (var pin in CollectPins(GetType(), PinDirection.Input))
            {
                AddInputPin(pin);
            }

            foreach (var pin in CollectPins(GetType(), PinDirection.Output))
            {
                AddOutputPin(pin);
            }
        }

        public CompositeNode Parent { get; internal set; }

        public string Title { get; set; }

        public NodeDispatcher Dispatcher
        {
            get { return _dispatcher ?? (_dispatcher = Parent?.Dispatcher); }
            set { _dispatcher = value; }
        }

        public IReadOnlyList<IPin> InputPins => _inputPins;

        public IReadOnlyList<IPin> OutputPins => _outputPins;

        public object GetValue(IPin pin)
        {
            if (Dispatcher == null)
            {
                return pin.GetValue();
            }

            return Dispatcher.Invoke(() => pin.GetValue());
        }

        public void SetValue(IPin pin, object value)
        {
            if (Dispatcher == null)
            {
                pin.SetValue(value);
            }
            else
            {
                Dispatcher.InvokeAsync(() => pin.SetValue(value));
            }
        }

        protected void RaisePinChanged()
        {
            PinsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void AddInputPin(IPin pin)
        {
            if (Dispatcher != null)
            {
                Dispatcher.Invoke(() =>
                {
                    _inputPins.Add(pin);
                });
            }
            else
            {
                _inputPins.Add(pin);
            }
        }

        protected void AddOutputPin(IPin pin)
        {
            if (Dispatcher != null)
            {
                Dispatcher.Invoke(() =>
                {
                    _outputPins.Add(pin);
                });
            }
            else
            {
                _outputPins.Add(pin);
            }
        }

        protected internal virtual void Reset()
        {
//            foreach (var pin in OutputPins.Values)
//            {
//                pin.SetValue(GetDefault(pin.Type));
//            }
        }

        protected internal abstract void Process(ProcessingContext context);

        internal void ProcessInternal(ProcessingContext context)
        {
            Process(context);
            Processed?.Invoke(this, EventArgs.Empty);
        }

        public void Pulse()
        {
            if (Parent != null)
            {
                Parent.Pulse(this);
            }
            else
            {
                ProcessInternal(new ProcessingContext(new ExecutionPath(new[] {this})));
            }
        }

        private IReadOnlyList<IPin> CollectPins(Type nodeType, PinDirection direction)
        {
            var pinAttributeType = direction == PinDirection.Input ? typeof (InputAttribute) : typeof (OutputAttribute);

            var pins = nodeType
                .GetProperties()
                .Where(propertyInfo => propertyInfo.GetCustomAttributes(pinAttributeType, true).Any())
                .Select(propertyInfo => (IPin)new PropertyPin(this, direction, propertyInfo))
                .ToArray();

            return new ReadOnlyCollection<IPin>(pins);
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public virtual void Dispose()
        {
        }
    }
}