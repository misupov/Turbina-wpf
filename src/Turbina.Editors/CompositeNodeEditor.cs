using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Turbina.Editors.Ropes;
using Turbina.Editors.Utils;
using Turbina.Editors.ViewModels;

namespace Turbina.Editors
{
    public class CompositeNodeEditor : Control
    {
        private Point? _canvasDragStartPoint;
        private Vector _nodeDragLatestPoint;
        private NodeViewModel _nodeDraged;
        private ItemsControl _itemsControl;
        private CanvasPoint _endLinkPoint;

        static CompositeNodeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompositeNodeEditor), new FrameworkPropertyMetadata(typeof(CompositeNodeEditor)));
        }

        public CompositeNodeViewModel ViewModel => (CompositeNodeViewModel)DataContext;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsControl = (ItemsControl) GetTemplateChild("PART_ItemsControl");
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var position = e.GetPosition(this);

//            Zoom(position, Math.Pow(1.1, e.Delta / 120.0));

            var cnt = 10;
            DispatcherTimer t = null;
            t = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.Normal, (sender, args) =>
            {
                Zoom(position, Math.Pow(1.02, e.Delta/120.0));
                if (cnt-- == 0)
                {
                    t.Stop();
                }
            },
            Dispatcher);

            t.Start();
        }

        private void Zoom(Point position, double factor)
        {
            ViewModel.Scale *= factor;

            ViewModel.TopLeftCorner = (Point) ((Vector)ViewModel.TopLeftCorner + (Vector)position / ViewModel.Scale * (factor - 1));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (!e.Handled)
            {
                if (e.ChangedButton == MouseButton.Middle)
                {
                    _canvasDragStartPoint = TranslatePosition(e.GetPosition(this));
                    CaptureMouse();
                }
                else if (e.ChangedButton == MouseButton.Left)
                {
                    var originalSource = e.OriginalSource as FrameworkElement;

                    var bullet = VisualTreeUtils.FindParent<Bullet>(originalSource);
                    if (bullet != null)
                    {
                        var pinViewModel = VisualTreeUtils.FindDataContext<PinViewModel>(bullet);

                        if (pinViewModel != null)
                        {
                            OnPreviewPinMouseDown(pinViewModel, e);
                            return;
                        }
                    }

                    var nodeViewModel = VisualTreeUtils.FindDataContext<NodeViewModel>(originalSource);
                    if (nodeViewModel != null && nodeViewModel != ViewModel)
                    {
                        _nodeDragLatestPoint = (Vector) TranslatePosition(e.GetPosition(this));
                        _nodeDraged = nodeViewModel;
                        ViewModel.BringToFront(nodeViewModel.Node);
                        var ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                        if (ctrlPressed)
                        {
                            nodeViewModel.IsSelected = !ViewModel.SelectedNodeViewModels.Contains(nodeViewModel);
                        }
                        else
                        {
                            nodeViewModel.IsSelected = true;
                            foreach (var viewModel in ViewModel.SelectedNodeViewModels.ToArray())
                            {
                                if (viewModel != nodeViewModel && viewModel.IsSelected)
                                {
                                    viewModel.IsSelected = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnPreviewPinMouseDown(PinViewModel pinViewModel, MouseButtonEventArgs e)
        {
            if (pinViewModel.Pin.Direction != PinDirection.Input)
            {
                _endLinkPoint = ViewModel.BeginLinkage(pinViewModel);
                CaptureMouse();
            }
            else
            {
                _endLinkPoint = ViewModel.BeginRelinkage(pinViewModel);
                CaptureMouse();
            }
        }

        private void OnPreviewPinMouseUp(PinViewModel pinViewModel, MouseButtonEventArgs e)
        {
            ViewModel.EndLinkage(pinViewModel);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if (_canvasDragStartPoint != null)
            {
                ReleaseMouseCapture();
                _canvasDragStartPoint = null;
            }

            var hitTestResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            var originalSource = hitTestResult.VisualHit;

            if (_endLinkPoint != null)
            {
                ReleaseMouseCapture();
                PinViewModel pinViewModel = null;

                var bullet = VisualTreeUtils.FindParent<Bullet>(originalSource);
                if (bullet != null)
                {
                    pinViewModel = VisualTreeUtils.FindDataContext<PinViewModel>(bullet);
                }

                OnPreviewPinMouseUp(pinViewModel, e);
                _endLinkPoint = null;
            }

            if (_nodeDraged != null)
            {
                ReleaseMouseCapture();
                _nodeDraged = null;
            }

            //            if (_currentRope != null)
            //            {
            //                var hitTestResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            //                var pin = VisualTreeUtils.FindParent<Pin>(hitTestResult.VisualHit);
            //                var ropeAdded = false;
            //                if (pin != null)
            //                {
            //                    if (_currentRope.EndPoint == _currentRopeFreePoint)
            //                    {
            //                        if (pin.IsInputPin)
            //                        {
            //                            if (InnerControls.OfType<Rope>().All(rope => rope.EndPoint != _pinLocations[pin]))
            //                            {
            //                                _currentRope.EndPoint = _pinLocations[pin];
            //                                Node.Link(_sourcePin.Node, _sourcePin.NodePin, pin.Node, pin.NodePin);
            //                                pin.Node.Pulse();
            //                                ropeAdded = true;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if (!pin.IsInputPin)
            //                        {
            //                            _currentRope.BeginPoint = _pinLocations[pin];
            //                            Node.Link(pin.Node, pin.NodePin, _targetPin.Node, _targetPin.NodePin);
            //                            pin.Node.Pulse();
            //                            ropeAdded = true;
            //                        }
            //                    }
            //                }
            //
            //                if (!ropeAdded)
            //                {
            //                    InnerControls.Remove(_currentRope);
            //                }
            //
            //                _currentRope = null;
            //                _currentRopeFreePoint = null;
            //            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!e.Handled && _canvasDragStartPoint != null)
            {
                ViewModel.TopLeftCorner = _canvasDragStartPoint.Value - (TranslatePosition(e.GetPosition(this)) - ViewModel.TopLeftCorner);
            }
            else if (_endLinkPoint != null)
            {
                var position = TranslatePosition(e.GetPosition(null));
                _endLinkPoint.Point = (Vector) position;
            }
            else if (_nodeDraged != null)
            {
                var point = TranslatePosition(e.GetPosition(null));
                var position = (Vector)point;
                position -= _nodeDragLatestPoint;
                _nodeDraged.Location.Point += position;
                _nodeDragLatestPoint = (Vector) point;
            }
        }
//
//        private void NodeEditorOnDragDelta(object sender, DragDeltaEventArgs eventArgs)
//        {
//            var element = eventArgs.OriginalSource as FrameworkElement;
//            element = VisualTreeUtils.FindParent<ContentPresenter>(element);
//            var pos = ScalableCanvas.GetPosition(element);
//            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
//            {
//                var grid = 10;
//                ScalableCanvas.SetPosition(element,
//                    new Vector(Math.Round((pos.X + eventArgs.HorizontalChange) / grid) * grid, Math.Round((pos.Y + eventArgs.VerticalChange) / grid) * grid));
//            }
//            else
//            {
//                ScalableCanvas.SetPosition(element,
//                    new Vector(pos.X + eventArgs.HorizontalChange, pos.Y + eventArgs.VerticalChange));
//            }
//        }

        public Point TranslatePosition(Point position)
        {
            return Vector.Divide((Vector)position, ViewModel.Scale) + ViewModel.TopLeftCorner;
        }
    }
}