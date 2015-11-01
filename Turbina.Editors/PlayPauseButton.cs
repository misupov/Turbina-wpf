using System.Windows;
using System.Windows.Controls.Primitives;

namespace Turbina.Editors
{
    public class PlayPauseButton : ToggleButton
    {
        static PlayPauseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlayPauseButton), new FrameworkPropertyMetadata(typeof(PlayPauseButton)));
        }
    }
}