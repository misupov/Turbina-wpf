using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using Microsoft.Expression.Media.Effects;

namespace Turbina.UI
{
    public partial class MainWindow
    {
        private readonly Brush _background;
        private DispatcherTimer _timer;
        private Stopwatch _sw;

        public MainWindow()
        {
            InitializeComponent();
            _background = new SolidColorBrush(Color.FromRgb(0x24, 0x27, 0x28));

//            _background =
//                new ImageBrush(new BitmapImage(new Uri(@"Images/grid.jpg", UriKind.Relative)))
//                {
//                    Stretch = Stretch.None,
//                    TileMode = TileMode.Tile,
//                    AlignmentX = AlignmentX.Left,
//                    AlignmentY = AlignmentY.Top,
//                    Viewport = new Rect(0, 0, 128, 128),
//                    ViewportUnits = BrushMappingMode.Absolute
//                };
//
            Application.Current.Resources["Dunno"] = Application.Current.Resources["Dunno1"];

            var compositeNode = TestNodes.Test3(new NodeDispatcher("Graph Dispatcher"));
            MainNode.DataContext = compositeNode;
//            var nodeEditorAttribute = compositeNode.GetType().GetCustomAttributes<EditorForAttribute>().FirstOrDefault();
//            nodeEditorAttribute.nodeType

            //            if (nodeEditorAttribute != null)
            {
//                var dnd = new DragableControl();
//                dnd.DragDelta += (sender, args) =>
//                {
//                    var c = (UIElement) sender;
//                    System.Windows.Controls.Canvas.SetLeft(c, System.Windows.Controls.Canvas.GetLeft(c) + args.HorizontalChange);
//                    System.Windows.Controls.Canvas.SetTop(c, System.Windows.Controls.Canvas.GetTop(c) + args.VerticalChange);
//                };

//                Canvas.Items.Add(compositeNode);

//                var element = Activator.CreateInstance(nodeEditorAttribute.NodeType) as NodeEditor;

                //                System.Windows.Controls.Canvas.SetLeft(dnd, 0);
                //                System.Windows.Controls.Canvas.SetTop(dnd, 0);
                //                dnd.Content = element;
                //                element.DragDelta += ControlOnDragDelta;

                //                Canvas.Children.Add(dnd);
            }

//            Grid.Background = _background;

//            _rope = new Rope();
//            _rope.Stroke = Brushes.Blue;
//            _rope.StrokeThickness = 3;
//            Canvas.Children.Add(_rope);
//
//            //            _control1 = new NodeEditor();
//            //            NodeSurface.SetLeft(_control1, r.Next(0, 800));
//            //            NodeSurface.SetTop(_control1, r.Next(0, 800));
//            //            Canvas.Children.Add(_control1);
//
//            //            _control2 = new NodeEditor();
//            //            NodeSurface.SetLeft(_control2, r.Next(0, 800));
//            //            NodeSurface.SetTop(_control2, r.Next(0, 800));
//            //            Canvas.Children.Add(_control2);
//
//            //            DependencyPropertyDescriptor.FromProperty(NodeSurface.LeftProperty, typeof(NodeEditor)).AddValueChanged(_control1, Handler);
//            //            DependencyPropertyDescriptor.FromProperty(NodeSurface.TopProperty, typeof(NodeEditor)).AddValueChanged(_control1, Handler);
//            //            DependencyPropertyDescriptor.FromProperty(NodeSurface.LeftProperty, typeof(NodeEditor)).AddValueChanged(_control2, Handler);
//            //            DependencyPropertyDescriptor.FromProperty(NodeSurface.TopProperty, typeof(NodeEditor)).AddValueChanged(_control2, Handler);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var bitmap = new RenderTargetBitmap((int)Math.Ceiling(Grid.RenderSize.Width), (int)Math.Ceiling(Grid.RenderSize.Height), 96, 96, PixelFormats.Default);
            bitmap.Render(Grid);

            Grid.Effect = new CircleRevealTransitionEffect
            {
                OldImage = new ImageBrush(bitmap),
                Reverse = !ToggleButton.IsChecked.Value
            };

            TaskbarItemInfo = ToggleButton.IsChecked == true
                ? new TaskbarItemInfo {ProgressState = TaskbarItemProgressState.Indeterminate}
                : new TaskbarItemInfo {ProgressState = TaskbarItemProgressState.None};

            Application.Current.Resources["Dunno"] = ToggleButton.IsChecked == true ? Application.Current.Resources["Dunno2"] : Application.Current.Resources["Dunno1"];

            Grid.Background = ToggleButton.IsChecked == true ? Brushes.Transparent : _background;
            OnAirImage.Visibility = ToggleButton.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

            _sw = Stopwatch.StartNew();
            _timer?.Stop();
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Background, Callback, Dispatcher);
        }

        private void Callback(object sender, EventArgs eventArgs)
        {
            var progress = _sw.ElapsedMilliseconds / 200.0;

            if (progress > 1)
            {
                Grid.Effect = null;
                _timer.Stop();
                return;
            }

            var effect = Grid.Effect as CircleRevealTransitionEffect;
            if (effect != null)
            {
                effect.Progress = progress;
            }
        }

        private void CollapseAllButtonClick(object sender, RoutedEventArgs e)
        {
//            Canvas.CollapseAll();
        }

        private void ExpandAllButtonClick(object sender, RoutedEventArgs e)
        {
//            Canvas.ExpandAll();
        }
    }
}
