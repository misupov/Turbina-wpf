using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Turbina.Editors.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged, IDraggable, IDisposable
    {
        private readonly IDisposable _disposable;
        private readonly ObservableCollection<PinViewModel> _inputPins = new ObservableCollection<PinViewModel>();
        private readonly ObservableCollection<PinViewModel> _outputPins = new ObservableCollection<PinViewModel>();
        private readonly HashSet<IPin> _inputPinsCache = new HashSet<IPin>();
        private readonly HashSet<IPin> _outputPinsCache = new HashSet<IPin>();
        private string _title;
        private bool _isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public NodeViewModel(Node node, Vector location, IControlTypesResolver controlTypesResolver)
        {
            Node = node;
            Title = node.Title;
            Location = new CanvasPoint(location);
            ControlTypesResolver = controlTypesResolver;

            foreach (var pin in node.InputPins)
            {
                AddInputPin(pin);
            }

            foreach (var pin in node.OutputPins)
            {
                AddOutputPin(pin);
            }

            node.Processed += OnNodeProcessed;
            node.PinsChanged += OnNodePinsChanged;

            _disposable = Disposable.Create(() =>
            {
                node.PinsChanged -= OnNodePinsChanged;
                node.Processed -= OnNodeProcessed;
            });
        }

        public CanvasPoint Location { get; }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public Node Node { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public IReadOnlyList<PinViewModel> InputPins => _inputPins;

        public IReadOnlyList<PinViewModel> OutputPins => _outputPins;

        public IControlTypesResolver ControlTypesResolver { get; }

        public void Pulse()
        {
            Node.Pulse();
        }

        public void Dispose()
        {
            _disposable.Dispose();

            var pinViewModels = _inputPins.Concat(_outputPins).ToArray();

            _inputPins.Clear();
            _outputPins.Clear();

            foreach (var pinViewModel in pinViewModels)
            {
                pinViewModel.Dispose();
            }
        }

        private void AddInputPin(IPin pin)
        {
            if (_inputPinsCache.Add(pin))
            {
                _inputPins.Add(new PinViewModel(this, pin, ControlTypesResolver));
            }
        }

        private void AddOutputPin(IPin pin)
        {
            if (_outputPinsCache.Add(pin))
            {
                _outputPins.Add(new PinViewModel(this, pin, ControlTypesResolver));
            }
        }

        private void RemoveInputPin(IPin pin)
        {
            if (_inputPinsCache.Remove(pin))
            {
                var pinViewModel = _inputPins.FirstOrDefault(model => model.Pin == pin);
                if (pinViewModel != null)
                {
                    pinViewModel.Dispose();
                    _inputPins.Remove(pinViewModel);
                }
            }
        }

        private void RemoveOutputPin(IPin pin)
        {
            if (_outputPinsCache.Remove(pin))
            {
                var pinViewModel = _outputPins.FirstOrDefault(model => model.Pin == pin);
                if (pinViewModel != null)
                {
                    pinViewModel.Dispose();
                    _outputPins.Remove(pinViewModel);
                }
            }
        }

        private void OnNodeProcessed(object sender, EventArgs e)
        {
            UpdatePinViewModelValues();
        }

        private void OnNodePinsChanged(object sender, EventArgs e)
        {
            var alivePins = new HashSet<IPin>();

            foreach (var pin in Node.InputPins)
            {
                alivePins.Add(pin);
                AddInputPin(pin);
            }

            foreach (var pin in Node.OutputPins)
            {
                alivePins.Add(pin);
                AddOutputPin(pin);
            }

            _inputPinsCache.IntersectWith(alivePins);
            _outputPinsCache.IntersectWith(alivePins);

            foreach (var pinViewModel in _inputPins.Where(model => !model.IsDead).Where(pinViewModel => !alivePins.Contains(pinViewModel.Pin)))
            {
                pinViewModel.MarkAsDead();
            }

            foreach (var pinViewModel in _outputPins.Where(model => !model.IsDead).Where(pinViewModel => !alivePins.Contains(pinViewModel.Pin)))
            {
                pinViewModel.MarkAsDead();
            }
        }

        private void UpdatePinViewModelValues()
        {
            foreach (var pinViewModel in _inputPins)
            {
                pinViewModel.UpdateValue();
            }

            foreach (var pinViewModel in _outputPins)
            {
                pinViewModel.UpdateValue();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetValue(IPin pin, object value)
        {
            Node.SetValue(pin, value);
        }

        public object GetValue(IPin pin)
        {
            return Node.GetValue(pin);
        }

        public PinViewModel GetPinViewModel(IPin pin)
        {
            switch (pin.Direction)
            {
                case PinDirection.Input:
                    return _inputPins.FirstOrDefault(model => model.Pin == pin);
                case PinDirection.Output:
                    return _outputPins.FirstOrDefault(model => model.Pin == pin);
            }
            return null;
        }
    }
}