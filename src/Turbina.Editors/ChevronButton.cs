using System.Windows;
using System.Windows.Controls.Primitives;

namespace Turbina.Editors
{
    public class ChevronButton : ToggleButton
    {
        static ChevronButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ChevronButton), new FrameworkPropertyMetadata(typeof (ChevronButton)));
        }
    }
}