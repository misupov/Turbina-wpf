using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Turbina.Editors
{
    public class Bullet : Control
    {
        static Bullet()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Bullet), new FrameworkPropertyMetadata(typeof(Bullet)));
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }
    }
}