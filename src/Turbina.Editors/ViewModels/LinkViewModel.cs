using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Turbina.Editors.ViewModels
{
    public class LinkViewModel : INotifyPropertyChanged, IDisposable
    {
        private CanvasPoint _beginPoint = new CanvasPoint();
        private CanvasPoint _endPoint = new CanvasPoint();
        private PinViewModel _sourcePinViewModel;
        private PinViewModel _targetPinViewModel;

        public LinkViewModel(PinViewModel sourcePinViewModel, PinViewModel targetPinViewModel, IControlTypesResolver controlTypesResolver)
        {
            SourcePinViewModel = sourcePinViewModel;
            TargetPinViewModel = targetPinViewModel;
            ControlTypesResolver = controlTypesResolver;
        }

        public IControlTypesResolver ControlTypesResolver { get; }

        public PinViewModel SourcePinViewModel
        {
            get { return _sourcePinViewModel; }
            set
            {
                if (_sourcePinViewModel != value)
                {
                    _sourcePinViewModel = value;
                    BeginPoint = _sourcePinViewModel != null ? _sourcePinViewModel.Point : new CanvasPoint();
                    OnPropertyChanged();
                }
            }
        }

        public PinViewModel TargetPinViewModel
        {
            get { return _targetPinViewModel; }
            set
            {
                if (_targetPinViewModel != value)
                {
                    _targetPinViewModel = value;
                    EndPoint = _targetPinViewModel != null ? _targetPinViewModel.Point : new CanvasPoint();
                    OnPropertyChanged();
                }
            }
        }

        public CanvasPoint BeginPoint
        {
            get { return _beginPoint; }
            set
            {
                if (_beginPoint != value)
                {
                    _beginPoint = value;
                    OnPropertyChanged();
                }
            }
        }

        public CanvasPoint EndPoint
        {
            get { return _endPoint; }
            set
            {
                if (_endPoint != value)
                {
                    _endPoint = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Dispose()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}