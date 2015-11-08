//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.Windows.Media;
//
//namespace Turbina.Editors
//{
//    public class NodeSurface : Panel
//    {
//        #region [DP] public double Scale { get; set; }
//
//        public static DependencyProperty ScaleProperty = DependencyProperty<NodeSurface>.Register(
//            surface => surface.Scale,
//            1.0,
//            surface => surface.ScaleChanged,
//            flags:
//                FrameworkPropertyMetadataOptions.AffectsArrange
//                | FrameworkPropertyMetadataOptions.AffectsRender
//                | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
//            );
//
//        public double Scale
//        {
//            get { return (double)GetValue(ScaleProperty); }
//            set { SetValue(ScaleProperty, value); }
//        }
//
//        private void ScaleChanged(DependencyPropertyChangedEventArgs<double> e)
//        {
//            foreach (UIElement child in InternalChildren)
//            {
//                child.RenderTransform = new ScaleTransform(Scale, Scale);
//            }
//        }
//
//        #endregion
//
//        #region [DP] public Point Center { get; set; }
//
//        public static DependencyProperty CenterProperty = DependencyProperty<NodeSurface>.Register(
//            surface => surface.Center,
//            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
//            );
//
//        public Point Center
//        {
//            get { return (Point)GetValue(CenterProperty); }
//            set { SetValue(CenterProperty, value); }
//        }
//
//        #endregion
//
//        #region [AP] double Left { get; set; }
//
//        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached(
//            "Left",
//            typeof(double),
//            typeof(NodeSurface),
//            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange));
//
//        public static double GetLeft(UIElement element)
//        {
//            return (double)element.GetValue(LeftProperty);
//        }
//
//        public static void SetLeft(UIElement element, double value)
//        {
//            element.SetValue(LeftProperty, value);
//        }
//
//        #endregion
//
//        #region [AP] double Top { get; set; }
//
//        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached(
//            "Top",
//            typeof(double),
//            typeof(NodeSurface),
//            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange));
//
//        public static double GetTop(UIElement element)
//        {
//            return (double)element.GetValue(TopProperty);
//        }
//
//        public static void SetTop(UIElement element, double value)
//        {
//            element.SetValue(TopProperty, value);
//        }
//
//        #endregion
//
//        protected override Size MeasureOverride(Size constraint)
//        {
//            foreach (UIElement child in InternalChildren)
//            {
//                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
//            }
//            return constraint;
//        }
//
//        protected override Size ArrangeOverride(Size arrangeBounds)
//        {
//            foreach (UIElement child in InternalChildren)
//            {
//                child.Arrange(
//                    new Rect(
//                        (GetLeft(child) - Center.X) * Scale,
//                        (GetTop(child) - Center.Y) * Scale,
//                        child.DesiredSize.Width,
//                        child.DesiredSize.Height));
//            }
//            return arrangeBounds;
//        }
//
//        protected override void OnMouseWheel(MouseWheelEventArgs e)
//        {
//            base.OnMouseWheel(e);
//
//            var position = e.GetPosition(this);
//            Zoom(position, Math.Pow(1.1, e.Delta / 120.0));
//        }
//
//        public void Zoom(Point coordinates, double scale)
//        {
//            SetCurrentValue(ScaleProperty, Scale * scale);
//            var x = Center.X + (coordinates.X / Scale) * (scale - 1);
//            var y = Center.Y + (coordinates.Y / Scale) * (scale - 1);
//            SetCurrentValue(CenterProperty, new Point(x, y));
//        }
//
//        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
//        {
//            return base.HitTestCore(hitTestParameters) ?? new PointHitTestResult(this, hitTestParameters.HitPoint);
//        }
//    }
//}