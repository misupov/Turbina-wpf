using System.Collections.ObjectModel;
using System.Windows;

namespace Turbina.Editors
{
    public interface IPinConnectable
    {
        ObservableCollection<Pin> InputPins { get; }

        ObservableCollection<Pin> OutputPins { get; }

        Point GetInputPinCoordinates(Pin pin);

        Point GetOutputPinCoordinates(Pin pin);
    }
}