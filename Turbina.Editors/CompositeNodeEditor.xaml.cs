using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Autofac;
using Turbina.Editors.Ropes;

namespace Turbina.Editors
{
    /// <summary>
    /// Interaction logic for CompositeNodeEditor.xaml
    /// </summary>
    public partial class CompositeNodeEditor : UserControl
    {
        private Point? _dragStartPoint;
        public static IContainer autofacContainer;
        private Rope _rope;
        private Node _prevNode;
        //        private Dictionary<Node, List<ConnectionPoint>> _nodeInputPins = new Dictionary<Node, List<ConnectionPoint>>();
        //        private Dictionary<Node, List<ConnectionPoint>> _nodeOutputPins = new Dictionary<Node, List<ConnectionPoint>>();

        public CompositeNodeEditor()
        {
            InitializeComponent();

            ContextMenu = new ContextMenu();
            
            var nodeTypes = autofacContainer != null ? autofacContainer.ResolveNamed<IEnumerable<Type>>("NodeTypes") : Enumerable.Empty<Type>();

            foreach (var nodeType in nodeTypes)
            {
                var menuItem = new MenuItem {Header = nodeType.Name};
                menuItem.Click += (sender, args) => MenuItemOnClick(nodeType, TranslatePosition(menuItem.TranslatePoint(new Point(), this)));
                ContextMenu.Items.Add(menuItem);
            }
        }

        #region [DP] public double Scale { get; set; }

        public static DependencyProperty ScaleProperty = DependencyProperty<CompositeNodeEditor>.Register(
            editor => editor.Scale,
            1.0,
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        #endregion

        #region [DP] public Point TopLeftCorner { get; set; }

        public static DependencyProperty TopLeftCornerProperty = DependencyProperty<CompositeNodeEditor>.Register(
            editor => editor.TopLeftCorner,
            new Point(),
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange);

        public Point TopLeftCorner
        {
            get { return (Point)GetValue(TopLeftCornerProperty); }
            set { SetValue(TopLeftCornerProperty, value); }
        }

        #endregion

        public CompositeNode Node => DataContext as CompositeNode;

        public ObservableCollection<object> InnerNodes { get; } = new ObservableCollection<object>();

        private void MenuItemOnClick(Type type, Point translatePoint)
        {
            var node = (Node) Activator.CreateInstance(type);
            node.Title = type.Name;
            Node.AddNode(node);
            InnerNodes.Add(node);
            if (_prevNode != null)
            {
                foreach (var keyValuePair in node.InputPins)
                {
                    Node.Link(_prevNode, _prevNode.OutputPins.First().Value, node, keyValuePair.Value);
                }
            }
            _prevNode = node;
            SetupNode(node, translatePoint);

//            var rope = new Rope();
//            if (_rope == null)
//            {
//                _rope = rope;
//            }
//            InnerNodes.Add(rope);
        }

        private void SetupNode(Node node, Point point)
        {
            var contentPresenter = (ContentPresenter)CanvasItemsControl.ItemContainerGenerator.ContainerFromItem(node);
            contentPresenter.Loaded += (sender, args) =>
            {
                ScalableCanvas.SetPosition(contentPresenter, point);
                contentPresenter.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(NodeEditorOnDragDelta));
                contentPresenter.SizeChanged += ContainerOnSizeChanged;

//                SetupNode2(contentPresenter, node);
            };
        }

//        private void SetupNode2(ContentPresenter contentPresenter, Node node)
//        {
//            var nodeEditor = GetNodeEditor(node);
////            var pinConnectable = nodeEditor as IPinConnectable;
////            if (pinConnectable != null)
////            {
////                foreach (var inputPin in pinConnectable.InputPins)
////                {
////                    var pin = new ConnectionPoint(inputPin);
////                    PlaceConnectionPoint(pin, pinConnectable, ScalableCanvas.GetPosition(contentPresenter));
////                    InnerNodes.Add(pin);
////                    List<ConnectionPoint> list;
////                    if (!_nodeInputPins.TryGetValue(node, out list))
////                    {
////                        _nodeInputPins[node] = list = new List<ConnectionPoint>();
////                    }
////                    list.Add(pin);
////                }
////            }
//        }

//        private static void PlaceConnectionPoint(Pin pin, IPinConnectable pinConnectable, Point nodePosition)
//        {
//            var coordinates = pinConnectable.GetInputPinCoordinates(pin);
//            var collapseCoefficient = 1.0;
//            var finalSize = new Rect(100, 100, 100, 100);
//
//            var element = pin;
//            var finalRect = new Rect(
//                3 - element.DesiredSize.Width / 2,
//                (1 - collapseCoefficient) * finalSize.Height / 2 + collapseCoefficient * coordinates.Y - element.DesiredSize.Height / 2,
//                element.DesiredSize.Width,
//                element.DesiredSize.Height);
////            element.Arrange(finalRect);
//
//            coordinates.X = finalRect.Left;
//            coordinates.Y = finalRect.Top;
//            coordinates.Offset(nodePosition.X, nodePosition.Y);
//
//            ScalableCanvas.SetPosition(pin, coordinates);
//        }

        private FrameworkElement GetNodeEditor(Node node)
        {
            var element = (FrameworkElement) CanvasItemsControl.ItemContainerGenerator.ContainerFromItem(node);
            return (FrameworkElement) VisualTreeHelper.GetChild(element, 0);
        }

        private void ContainerOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var contentPresenter = (ContentPresenter) sender;
            var nodeEditor = GetNodeEditor((Node) contentPresenter.Content);
            var pinConnectable = nodeEditor as IPinConnectable;

            if (pinConnectable != null)
            {
//                pinConnectable.InputPins[0].

//                foreach (var inputPin in pinConnectable.InputPins)
//                {
//                    var inputPinCoordinates = pinConnectable.GetInputPinCoordinates(inputPin);
//                    PlaceConnectionPoint();
//                    var translatePoint = contentPresenter.TranslatePoint(inputPinCoordinates, CanvasItemsControl);
//                }
            }
        }

        private void NodeEditorOnDragDelta(object sender, DragDeltaEventArgs eventArgs)
        {
            var element = sender as ContentPresenter;
            var pos = ScalableCanvas.GetPosition(element);
            ScalableCanvas.SetPosition(element, new Point(pos.X + eventArgs.HorizontalChange, pos.Y + eventArgs.VerticalChange));

            UpdatePins((Node) element.Content);
        }

        private void UpdatePins(Node node)
        {
//            var element = (FrameworkElement)CanvasItemsControl.ItemContainerGenerator.ContainerFromItem(node);
//            var nodeEditor = GetNodeEditor(node);
//            var pinConnectable = nodeEditor as IPinConnectable;
//            if (pinConnectable != null)
//            {
//                foreach (var pin in _nodeInputPins[node])
//                {
//                    PlaceConnectionPoint(pin, pinConnectable, ScalableCanvas.GetPosition(element));
//                }
////
////                foreach (var inputPin in pinConnectable.InputPins)
////                {
////                    var pin = new ConnectionPoint(inputPin);
////                    PlaceConnectionPoint(pin, pinConnectable, inputPin);
////                    ConnectionPoints.Add(pin);
////                    List<ConnectionPoint> list;
////                    if (!_nodeInputPins.TryGetValue(node, out list))
////                    {
////                        _nodeInputPins[node] = list = new List<ConnectionPoint>();
////                    }
////                    list.Add(pin);
////                }
//            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var position = e.GetPosition(this);

            Zoom(position, Math.Pow(1.1, e.Delta / 120.0));

            //            var cnt = 10;
            //            DispatcherTimer t = null;
            //            t = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.Normal, (sender, args) =>
            //            {
            //                Zoom(position, Math.Pow(1.02, e.Delta/120.0));
            //                if (cnt-- == 0)
            //                {
            //                    t.Stop();
            //                }
            //            },
            //                Dispatcher);
            //            t.Start();
        }

        private void Zoom(Point position, double factor)
        {
            SetCurrentValue(ScaleProperty, Scale * factor);

            var newTopLeftCorner = (Vector)TopLeftCorner + (Vector)position / Scale * (factor - 1);

            SetCurrentValue(TopLeftCornerProperty, (Point)newTopLeftCorner);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (!e.Handled && e.ChangedButton == MouseButton.Middle)
            {
                _dragStartPoint = TranslatePosition(e.GetPosition(this));
                CaptureMouse();
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            ReleaseMouseCapture();
            _dragStartPoint = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!e.Handled && _dragStartPoint != null)
            {
                SetCurrentValue(TopLeftCornerProperty, _dragStartPoint.Value - (TranslatePosition(e.GetPosition(this)) - TopLeftCorner));
            }

            if (_rope != null)
            {
                var position = TranslatePosition(e.GetPosition(null));

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _rope.Goto1(new Vector(position.X, position.Y));
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    _rope.Goto2(new Vector(position.X, position.Y));
                }
            }
        }

        private Point TranslatePosition(Point position)
        {
            return Vector.Divide((Vector) position, Scale) + TopLeftCorner;
        }
    }
}
