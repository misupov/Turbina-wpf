using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Turbina.Nodes
{
    public class StringConstNode : Node
    {
        [Output]
        [Input]
        public string Value { get; set; }

        protected override void InitializePins()
        {
            base.InitializePins();

            AddOutputPin(new StrPin());
            AddOutputPin(new StrPin());
            AddOutputPin(new StrPin());
            AddOutputPin(new StrPin());
            AddOutputPin(new StrPin());
            AddOutputPin(new StrPin());
        }

        protected override void Process(ProcessingContext context)
        {
        }
    }

    public class StrPin : IPin, INotifyPropertyChanged
    {
        private string _value;
        private string _name;

        public StrPin()
        {
            Name = "fake";
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public Type Type { get { return typeof (string); } }

        public PinDirection Direction { get; } = PinDirection.Output;

        public Func<object> GetValue { get { return () => _value; } }

        public Action<object> SetValue
        {
            get
            {
                return o =>
                {
                    _value = (string) o;
                    Name = _value;
                };
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}