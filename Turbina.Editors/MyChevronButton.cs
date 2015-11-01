using System.Windows;
using System.Windows.Controls.Primitives;

namespace Turbina.Editors
{
    public class MyChevronButton : ToggleButton
    {
        static MyChevronButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (MyChevronButton), new FrameworkPropertyMetadata(typeof (MyChevronButton)));
        }
    }
}