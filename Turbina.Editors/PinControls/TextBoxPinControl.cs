using System;
using System.Windows;
using System.Windows.Controls;

namespace Turbina.Editors.PinControls
{
    public class PinEditorControl : Control
    {
        public event EventHandler ValueChanged;

        #region [DP] public bool IsCollapsed { get; set; }

        public static readonly DependencyProperty ValueProperty = DependencyProperty<PinEditorControl>.Register(
            control => control.Value,
            control => control.OnValueChanged);

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private void OnValueChanged(DependencyPropertyChangedEventArgs<object> e)
        {
            if (!Equals(e.OldValue, e.NewValue))
            {
                ValueChanged?.Invoke(this, e);
            }
        }

        #endregion
    }

    public class TextBoxPinControl : PinEditorControl
    {
        static TextBoxPinControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxPinControl), new FrameworkPropertyMetadata(typeof(TextBoxPinControl)));
        }
    }

    public class TimeSpanPinControl : PinEditorControl
    {
        static TimeSpanPinControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanPinControl), new FrameworkPropertyMetadata(typeof(TimeSpanPinControl)));
        }
    }

    public class OnOffPinControl : PinEditorControl
    {
        static OnOffPinControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OnOffPinControl), new FrameworkPropertyMetadata(typeof(OnOffPinControl)));
        }
    }

    public class TextBlockPinControl : PinEditorControl
    {
        static TextBlockPinControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBlockPinControl), new FrameworkPropertyMetadata(typeof(TextBlockPinControl)));
        }
    }
}