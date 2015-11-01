using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Turbina.Editors
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class EditableTextBlock : Control
    {
        private TextBox _textBox;
        private string _textBeforeEditing;

        static EditableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTextBlock), new FrameworkPropertyMetadata(typeof(EditableTextBlock)));
        }

        #region [DP] public string Text { get; set; }

        public static readonly DependencyProperty TextProperty = DependencyProperty<EditableTextBlock>.Register(
            control => control.Text,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region [DP] public Brush SelectionBrush { get; set; }

        public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty<EditableTextBlock>.Register(
            control => control.SelectionBrush);

        public Brush SelectionBrush
        {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        #endregion

        #region [DP] public Brush CaretBrush { get; set; }

        public static readonly DependencyProperty CaretBrushProperty = DependencyProperty<EditableTextBlock>.Register(
            control => control.CaretBrush);

        public Brush CaretBrush
        {
            get { return (Brush)GetValue(CaretBrushProperty); }
            set { SetValue(CaretBrushProperty, value); }
        }

        #endregion

        #region [DP] public bool IsInReadOnlyMode { get; }

        private static readonly DependencyPropertyKey IsInReadOnlyModePropertyKey = DependencyProperty<EditableTextBlock>.RegisterReadOnly(
            control => control.IsInReadOnlyMode,
            true,
            control => control.IsInReadOnlyModeChanged);

        public static readonly DependencyProperty IsInReadOnlyModeProperty = IsInReadOnlyModePropertyKey.DependencyProperty;

        public bool IsInReadOnlyMode
        {
            get { return (bool)GetValue(IsInReadOnlyModeProperty); }
            private set { SetValue(IsInReadOnlyModePropertyKey, value); }
        }

        private void IsInReadOnlyModeChanged(DependencyPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                SetCurrentValue(TextProperty, _textBox.Text);
                _textBox.CaretIndex = 0;
                var window = Window.GetWindow(_textBox);
                if (window != null)
                {
                    window.PreviewMouseDown -= OnPreviewWindowMouseDown;
                }
            }
            else
            {
                var window = Window.GetWindow(_textBox);
                if (window != null)
                {
                    window.PreviewMouseDown += OnPreviewWindowMouseDown;
                }
            }
        }

        private void OnPreviewWindowMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var position = mouseButtonEventArgs.GetPosition(_textBox);
            if (position.X < 0 || position.X > _textBox.ActualWidth || position.Y < 0 ||
                position.Y > _textBox.ActualHeight)
            {
                IsInReadOnlyMode = true;
            }
        }

        #endregion

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            _textBeforeEditing = _textBox.Text;
            IsInReadOnlyMode = false;
            _textBox.Focus();
            _textBox.SelectAll();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = (TextBox)GetTemplateChild("PART_TextBox");
            _textBox.KeyDown += TextBoxOnKeyDown;
            _textBox.LostFocus += TextBoxOnLostFocus;
            _textBox.LostKeyboardFocus += TextBoxOnLostFocus;
        }

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            IsInReadOnlyMode = true;
        }

        private void TextBoxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter)
            {
                IsInReadOnlyMode = true;
            }
            if (keyEventArgs.Key == Key.Escape)
            {
                _textBox.SetCurrentValue(TextBox.TextProperty, _textBeforeEditing);
                IsInReadOnlyMode = true;
            }
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}