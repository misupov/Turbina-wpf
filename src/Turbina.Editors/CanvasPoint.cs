using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Turbina.Editors
{
    public class CanvasPoint : INotifyPropertyChanged
    {
        private Vector _point;

        public CanvasPoint(Vector point = default(Vector))
        {
            _point = point;
        }

        public Vector Point
        {
            get { return _point; }
            set
            {
                if (_point != value)
                {
                    _point = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}