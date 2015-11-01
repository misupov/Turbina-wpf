using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Turbina
{
    [DebuggerDisplay("Node: \"{Title,nq}\" ({GetType().Name,nq})")]
    public abstract class Node
    {
        private IReadOnlyDictionary<string, IPin> _inputPinsCache;
        private IReadOnlyDictionary<string, IPin> _outputPinsCache;
        private NodeDispatcher _dispatcher;

        public event EventHandler Processed;

        public string Title { get; set; }

        public CompositeNode Parent { get; internal set; }

        public NodeDispatcher Dispatcher
        {
            get { return _dispatcher ?? (_dispatcher = Parent?.Dispatcher); }
            set { _dispatcher = value; }
        }

        public virtual IReadOnlyDictionary<string, IPin> InputPins => _inputPinsCache ?? (_inputPinsCache = CollectInputPins());

        public virtual IReadOnlyDictionary<string, IPin> OutputPins => _outputPinsCache ?? (_outputPinsCache = CollectOutputPins());

        protected internal virtual void Reset()
        {
            foreach (var pin in OutputPins.Values)
            {
                pin.SetValue(GetDefault(pin.Type));
            }
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
                ProcessInternal(new ProcessingContext());
            }
        }

        private IReadOnlyDictionary<string, IPin> CollectInputPins()
        {
            return CollectPins(typeof (InputAttribute));
        }

        private IReadOnlyDictionary<string, IPin> CollectOutputPins()
        {
            return CollectPins(typeof(OutputAttribute));
        }

        private IReadOnlyDictionary<string, IPin> CollectPins(Type pinType)
        {
            var pins = GetType()
                .GetProperties()
                .Where(propertyInfo => propertyInfo.GetCustomAttributes(pinType, true).Any())
                .Select(propertyInfo => new PropertyPin(this, propertyInfo))
                .Cast<IPin>()
                .ToDictionary(pin => pin.Name);

            return new ReadOnlyDictionary<string, IPin>(pins);
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}