using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Turbina.Editors.PinControls;

namespace Turbina.Editors
{
    public class Pin : Control
    {
        static Pin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pin), new FrameworkPropertyMetadata(typeof(Pin)));
        }

        #region [DP] public string Title { get; set; }

        public static readonly DependencyProperty TitleProperty = DependencyProperty<Pin>.Register(pin => pin.Title);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region [DP] public PinEditorControl ValuePresenter { get; set; }

        private static readonly DependencyPropertyKey ValuePresenterPropertyKey = DependencyProperty<Pin>.RegisterReadOnly(pin => pin.ValuePresenter);

        public static readonly DependencyProperty ValuePresenterProperty = ValuePresenterPropertyKey.DependencyProperty;
        private readonly IPin _nodePin;
        private readonly Node _node;
        private bool _dontPulse;

        public PinEditorControl ValuePresenter
        {
            get { return (PinEditorControl) GetValue(ValuePresenterProperty); }
            private set { SetValue(ValuePresenterPropertyKey, value); }
        }

        #endregion

        public Pin(PinEditorControl presenter, IPin pin, Node node, FlowDirection flowDirection)
        {
            FlowDirection = flowDirection;
            _nodePin = pin;
            _node = node;
            ValuePresenter = presenter;
            ValuePresenter.ValueChanged += OnValueChanged;

            Title = typeof (Pin).ToString();
            Loaded += OnLoaded;
        }

        private void OnValueChanged(object sender, EventArgs eventArgs)
        {
            var value = ValuePresenter.Value;
            if (_nodePin.Type == typeof(Uri) && value is string)
            {
                value = new Uri((string) value);
            }
            _nodePin.SetValue(value);
            if (!_dontPulse)
            {
                _node.Pulse();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var inAdorner = new Bullet();
            var adorner = new ControlAdorner(this)
            {
                Child = inAdorner,
            };
            AdornerLayer.GetAdornerLayer(this).Add(adorner);
        }

        public void Update()
        {
            _dontPulse = true;
            try
            {
                ValuePresenter.SetCurrentValue(PinEditorControl.ValueProperty, _nodePin.GetValue());
            }
            finally
            {
                _dontPulse = false;
            }
        }
    }

    public class Bullet : Control
    {
        static Bullet()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Bullet), new FrameworkPropertyMetadata(typeof(Bullet)));
        }
    }

    internal class ControlAdorner : Adorner
    {
        private Control _child;

        public ControlAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _child;

        public Control Child
        {
            get { return _child; }
            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                }
                _child = value;
                if (_child != null)
                {
                    AddVisualChild(_child);
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var adornedElement = (FrameworkElement)AdornedElement;

            var x = -finalSize.Width - 1.5;
            var y = (adornedElement.ActualHeight - finalSize.Height)/2;

            var finalRect = new Rect(new Point(x, y), finalSize);

            _child.Arrange(finalRect);
            return new Size(_child.ActualWidth, _child.ActualHeight);
        }
    }
}