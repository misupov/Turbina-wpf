using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Turbina.Editors
{
    [ContentProperty("Content")]
    public class ThumbWithContent : Thumb
    {
        static ThumbWithContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThumbWithContent), new FrameworkPropertyMetadata(typeof(ThumbWithContent)));
        }

        #region [DP] public object Content { get; set; }

        public static readonly DependencyProperty ContentProperty = DependencyProperty<ThumbWithContent>.Register(control => control.Content);

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion
    }
}