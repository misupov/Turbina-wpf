using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using Autofac;
using Microsoft.Expression.Media.Effects;
using Turbina.Editors;
using Turbina.Editors.ViewModels;

namespace Turbina.UI
{
    public partial class MainWindow
    {
        private readonly Brush _background;
        private DispatcherTimer _timer;
        private Stopwatch _sw;
        private readonly CompositeNodeViewModel _compositeNodeViewModel;

        public MainWindow(IContainer container)
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

            Application.Current.Resources["Dunno"] = Application.Current.Resources["Dunno1"];

            var compositeNode = new CompositeNode(new NodeDispatcher("Graph Dispatcher"));// TestNodes.Test3(new NodeDispatcher("Graph Dispatcher"));
            _compositeNodeViewModel = new CompositeNodeViewModel(compositeNode, new Vector(), new ControlTypesResolver());
            MainNode.DataContext = _compositeNodeViewModel;

            var contextMenu = new ContextMenu();
            MainNode.ContextMenu = contextMenu;

            var nodeTypes = container != null ? container.ResolveNamed<IEnumerable<Type>>("NodeTypes") : Enumerable.Empty<Type>();

            foreach (var nodeType in nodeTypes)
            {
                var menuItem = new MenuItem { Header = nodeType.Name };
                menuItem.Click += (sender, args) => MenuItemOnClick(nodeType, MainNode.TranslatePosition(menuItem.TranslatePoint(new Point(), this)));
                contextMenu.Items.Add(menuItem);
            }

        }

        private void MenuItemOnClick(Type type, Point translatePoint)
        {
            var node = (Node)Activator.CreateInstance(type);
            var title = type.Name;
            if (title.EndsWith("Node") && title.Length > 4)
            {
                title = title.Substring(0, title.Length - 4);
            }
            node.Title = title;
            _compositeNodeViewModel.AddNode(node, new Vector(translatePoint.X, translatePoint.Y));
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

            _compositeNodeViewModel.ResetNodes();

            _sw = Stopwatch.StartNew();
            _timer?.Stop();
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Normal, Callback, Dispatcher);
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
    }
}
