using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Turbina.Editors.Utils;

namespace Turbina.Editors
{
    public class PinPanel : Panel
    {
        private NodeEditor _nodeEditor;
        private readonly HashSet<Pin> _inputPins = new HashSet<Pin>();
        private readonly HashSet<Pin> _outputPins = new HashSet<Pin>();

        #region [DP] public double CollapseCoefficient { get; }

        public static readonly DependencyProperty CollapseCoefficientProperty = DependencyProperty<PinPanel>.Register(
            control => control.CollapseCoefficient,
            1.0,
            control => control.CollapseCoefficientChanged,
            flags: FrameworkPropertyMetadataOptions.AffectsArrange);

        public double CollapseCoefficient
        {
            get { return (double)GetValue(CollapseCoefficientProperty); }
            set { SetValue(CollapseCoefficientProperty, value); }
        }

        private void CollapseCoefficientChanged(DependencyPropertyChangedEventArgs<double> e)
        {
            IsHitTestVisible = e.NewValue > 0;
        }

        #endregion

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (_nodeEditor != null)
            {
                _nodeEditor.InputPins.CollectionChanged -= OnPinsCollectionChanged;
                _nodeEditor.OutputPins.CollectionChanged -= OnPinsCollectionChanged;
            }
            _nodeEditor = VisualTreeUtils.FindParent<NodeEditor>(this);
            _nodeEditor.InputPins.CollectionChanged += OnPinsCollectionChanged;
            _nodeEditor.OutputPins.CollectionChanged += OnPinsCollectionChanged;

            RefreshPins();
        }

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            RefreshPins();
        }

        private void RefreshPins()
        {
            Children.Clear();
            _inputPins.Clear();
            _outputPins.Clear();

            foreach (var pin in _nodeEditor.InputPins)
            {
                Children.Add(pin);
                _inputPins.Add(pin);
            }

            foreach (var pin in _nodeEditor.OutputPins)
            {
                Children.Add(pin);
                _outputPins.Add(pin);
            }

            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = 300;
            }

            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = 300;
            }

            foreach (var pin in _inputPins)
            {
                pin.Measure(availableSize);
            }

            foreach (var pin in _outputPins)
            {
                pin.Measure(availableSize);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var collapseCoefficient = CollapseCoefficient;

            foreach (var pin in _inputPins)
            {
                var coordinates = _nodeEditor.GetInputPinCoordinates(pin);
                var finalRect = new Rect(
                    3 - pin.DesiredSize.Width/2,
                    (1 - collapseCoefficient) * finalSize.Height / 2 + collapseCoefficient*coordinates.Y - pin.DesiredSize.Height / 2,
                    pin.DesiredSize.Width,
                    pin.DesiredSize.Height);
                pin.Arrange(finalRect);
            }

            foreach (var pin in _outputPins)
            {
                var coordinates = _nodeEditor.GetOutputPinCoordinates(pin);
                var finalRect = new Rect(
                    -3 + finalSize.Width - pin.DesiredSize.Width/2,
                    (1 - collapseCoefficient) * finalSize.Height / 2 + collapseCoefficient * coordinates.Y - pin.DesiredSize.Height / 2,
                    pin.DesiredSize.Width,
                    pin.DesiredSize.Height);
                pin.Arrange(finalRect);
            }

            return finalSize;
        }
    }
}