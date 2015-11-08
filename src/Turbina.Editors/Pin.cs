using System;
using System.Windows;
using System.Windows.Controls;
using Turbina.Editors.Utils;
using Turbina.Editors.ViewModels;

namespace Turbina.Editors
{
    public class Pin : Control
    {
        private bool _dontPulse;
        private FrameworkElement _bullet;

        static Pin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pin), new FrameworkPropertyMetadata(typeof(Pin)));
        }

        public Pin()
        {
            DataContextChanged += OnDataContextChanged;
            LayoutUpdated += OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, EventArgs eventArgs)
        {
            var compositeNodeEditor = VisualTreeUtils.FindParent<CompositeNodeEditor>(this);
            var nodeEditor = VisualTreeUtils.FindParent<NodeEditor>(this);
            ViewModel.Point.Point = (Vector)compositeNodeEditor.TranslatePosition(nodeEditor.GetPinPoint(this, compositeNodeEditor));
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            FlowDirection = ViewModel.Pin.Direction == PinDirection.Input ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
        }

        public PinViewModel ViewModel => (PinViewModel) DataContext;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _bullet = (FrameworkElement)GetTemplateChild("PART_Bullet");
        }

        public Point GetPinPoint(FrameworkElement relativeTo)
        {
            return _bullet.TranslatePoint(new Point(_bullet.ActualWidth/2, _bullet.ActualHeight/2), relativeTo);
        }

        private void OnValueChanged(object sender, EventArgs eventArgs)
        {
//            var value = ValuePresenter.Value;
//            if (NodePin.Type == typeof(Uri) && value is string)
//            {
//                value = new Uri((string) value);
//            }
//            NodePin.SetValue(value);
//            if (!_dontPulse)
//            {
//                Node.Pulse();
//            }
        }

        public void Update()
        {
//            _dontPulse = true;
//            try
//            {
//                ValuePresenter.SetCurrentValue(PinEditorControl.ValueProperty, NodePin.GetValue());
//            }
//            finally
//            {
//                _dontPulse = false;
//            }
        }
    }
}