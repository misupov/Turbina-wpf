using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Turbina.Editors.Utils;

namespace Turbina.Editors
{
    public class ScalableCanvas : Panel
    {
//        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
//            "Position",
//            typeof(Vector),
//            typeof(ScalableCanvas),
//            new FrameworkPropertyMetadata(new Vector(), FrameworkPropertyMetadataOptions.Inherits, OnPositioningChanged));
//
//        private static void OnPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            var uIElement = d as UIElement;
//            if (uIElement != null)
//            {
//                var canvas = VisualTreeUtils.FindParent<ScalableCanvas>(uIElement);
//                canvas?.InvalidateArrange();
//            }
//        }
//
//        public static Vector GetPosition(UIElement element)
//        {
//            Argument.NotNull(element, nameof(element));
//
//            return (Vector)element.GetValue(PositionProperty);
//        }
//
//        public static void SetPosition(UIElement element, Vector position)
//        {
//            Argument.NotNull(element, nameof(element));
//
//            element.SetValue(PositionProperty, position);
//        }

        public ScalableCanvas()
        {
            ClipToBounds = true;
        }

        #region [DP] public double Scale { get; set; }

        public static DependencyProperty ScaleProperty = DependencyProperty<ScalableCanvas>.Register(
            canvas => canvas.Scale,
            1.0,
            canvas => canvas.ScaleChanged,
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        private void ScaleChanged(DependencyPropertyChangedEventArgs<double> e)
        {
            LayoutTransform = new ScaleTransform(Scale, Scale);
        }

        #endregion

        #region [DP] public Point TopLeftCorner { get; set; }

        public static DependencyProperty TopLeftCornerProperty = DependencyProperty<ScalableCanvas>.Register(
            canvas => canvas.TopLeftCorner,
            new Point(),
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange);

        public Point TopLeftCorner
        {
            get { return (Point)GetValue(TopLeftCornerProperty); }
            set { SetValue(TopLeftCornerProperty, value); }
        }

        #endregion

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            var draggable = (visualAdded as FrameworkElement)?.DataContext as IDraggable;
            if (draggable != null)
            {
                draggable.Location.PropertyChanged += LocationOnPropertyChanged;
            }

            draggable = (visualRemoved as FrameworkElement)?.DataContext as IDraggable;
            if (draggable != null)
            {
                draggable.Location.PropertyChanged -= LocationOnPropertyChanged;
            }
        }

        private void LocationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            InvalidateArrange();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement uIElement in InternalChildren)
            {
                uIElement?.Measure(availableSize);
            }
            return default(Size);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement uIElement in InternalChildren)
            {
                if (uIElement != null)
                {
                    var draggable = (uIElement as FrameworkElement)?.DataContext as IDraggable;

                    var position = draggable?.Location.Point ?? new Vector();
                    uIElement.Arrange(new Rect(new Point(position.X - TopLeftCorner.X, position.Y - TopLeftCorner.Y), uIElement.DesiredSize));
                }
            }
            return arrangeSize;
        }
    }

    public interface IDraggable
    {
        CanvasPoint Location { get; }
    }
}