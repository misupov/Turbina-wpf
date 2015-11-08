using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Turbina.Editors.ViewModels
{
    public class PinViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _disposable = Disposable.Empty;
        private string _name;
        private Type _type;
        private object _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public PinViewModel(NodeViewModel nodeViewModel, IPin pin, IControlTypesResolver controlTypesResolver)
        {
            ControlTypesResolver = controlTypesResolver;
            NodeViewModel = nodeViewModel;
            Pin = pin;
            Name = pin.Name;
            Type = pin.Type;
            Point = new CanvasPoint();

            var changablePin = pin as INotifyPropertyChanged;
            if (changablePin != null)
            {
                changablePin.PropertyChanged += OnPinPropertyChanged;
                _disposable = Disposable.Create(() => changablePin.PropertyChanged -= OnPinPropertyChanged);
            }

            UpdateValue();
        }

        public NodeViewModel NodeViewModel { get; }

        public CanvasPoint Point { get; }

        public IPin Pin { get; private set; }

        public IControlTypesResolver ControlTypesResolver { get; }

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

        public Type Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsDead
        {
            get { return Pin == null; }
            private set
            {
                Pin = null;
                Type = typeof(object);
                OnPropertyChanged();
                UpdateValue();
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (IsDead)
                {
                    value = null;
                }

                if (_value != value)
                {
                    _value = value;

                    if (!IsDead)
                    {
                        NodeViewModel.SetValue(Pin, value);
                    }

                    OnPropertyChanged();
                }
            }
        }

        public void MarkAsDead()
        {
            IsDead = true;
        }

        public void UpdateValue()
        {
            Value = IsDead ? null : NodeViewModel.GetValue(Pin);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPinPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            switch (eventArgs.PropertyName)
            {
                case nameof(IPin.Name):
                    Name = NodeViewModel.Node.Dispatcher != null ? NodeViewModel.Node.Dispatcher.Invoke(() => Pin.Name) : Pin.Name;
                    break;
                case nameof(IPin.Type):
                    Type = NodeViewModel.Node.Dispatcher != null ? NodeViewModel.Node.Dispatcher.Invoke(() => Pin.Type) : Pin.Type;
                    break;
            }
        }
    }
}