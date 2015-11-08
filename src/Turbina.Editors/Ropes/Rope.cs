using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Turbina.Editors.ViewModels;

namespace Turbina.Editors.Ropes
{
    public class Rope : Shape, IDisposable
    {
        private DispatcherTimer _timer;
        private Stopwatch _sw;
        private RopeSimulation _ropeSimulation;

        public Rope()
        {
            Stroke = Brushes.CornflowerBlue;
            StrokeThickness = 2;

            DataContextChanged += OnDataContextChanged;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Stroke = Brushes.Gold;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Stroke = Brushes.CornflowerBlue;
        }

        public void Dispose()
        {
            DataContext = null;
        }

        public LinkViewModel ViewModel => (LinkViewModel) DataContext;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            _timer?.Stop();

            var oldViewModel = eventArgs.OldValue as LinkViewModel;
            if (oldViewModel != null)
            {
                oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            var newViewModel = eventArgs.NewValue as LinkViewModel;
            if (newViewModel != null)
            {
                BeginPoint = newViewModel.BeginPoint;
                EndPoint = newViewModel.EndPoint;
                newViewModel.PropertyChanged += OnViewModelPropertyChanged;
                Init();
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            BeginPoint = ViewModel.BeginPoint;
            EndPoint = ViewModel.EndPoint;
        }

        private void Init()
        {
            _ropeSimulation = new RopeSimulation(
                40, // 40 Particles (Masses)
                1f, // Each Particle Has A Weight Of 1 Gram
                10000.0f, // springConstant In The Rope
                0.0f, // Normal Length Of Springs In The Rope
                0.0002f, // Spring Inner Friction Constant
                new Vector(0, 0), // Gravitational Acceleration
                5,
                new Vector(BeginPoint.Point.X, BeginPoint.Point.Y));

            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.Render, AnimationCallback, Dispatcher.CurrentDispatcher);
            _timer.Start();
            _sw = Stopwatch.StartNew();
        }

        #region [DP] public CanvasPoint BeginPoint { get; set; }

        public static DependencyProperty BeginPointProperty = DependencyProperty<Rope>.Register(
            rope => rope.BeginPoint,
            new CanvasPoint(),
            rope => rope.BeginPointChanged,
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

        public CanvasPoint BeginPoint
        {
            get { return (CanvasPoint)GetValue(BeginPointProperty); }
            set { SetValue(BeginPointProperty, value); }
        }

        private void BeginPointChanged(DependencyPropertyChangedEventArgs<CanvasPoint> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.PropertyChanged -= OnPointPropertyChanged;
            }
            if (e.NewValue != null)
            {
                e.NewValue.PropertyChanged += OnPointPropertyChanged;
            }
            RestartTimer();
        }

        #endregion

        #region [DP] public CanvasPoint EndPoint { get; set; }

        public static DependencyProperty EndPointProperty = DependencyProperty<Rope>.Register(
            editor => editor.EndPoint,
            new CanvasPoint(),
            rope => rope.EndPointChanged,
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

        public CanvasPoint EndPoint
        {
            get { return (CanvasPoint)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        private void EndPointChanged(DependencyPropertyChangedEventArgs<CanvasPoint> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.PropertyChanged -= OnPointPropertyChanged;
            }
            if (e.NewValue != null)
            {
                e.NewValue.PropertyChanged += OnPointPropertyChanged;
            }
            RestartTimer();
        }

        #endregion

        private void AnimationCallback(object sender, EventArgs eventArgs)
        {
            InvalidateVisual();
        }

        private void RestartTimer()
        {
            if (_timer != null && !_timer.IsEnabled)
            {
                _timer.Start();
                _sw.Restart();
            }
        }

        private void OnPointPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            RestartTimer();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                var speed = 5;

                var seconds = _sw.Elapsed.TotalSeconds*speed;
                _sw = Stopwatch.StartNew();

                var startPoint = new Vector(BeginPoint.Point.X, BeginPoint.Point.Y);
                var endPoint = new Vector(EndPoint.Point.X, EndPoint.Point.Y);

                _ropeSimulation.RopeConnectionVel1 = (startPoint - _ropeSimulation.Mass1Position)*speed;
                _ropeSimulation.RopeConnectionVel2 = (endPoint - _ropeSimulation.Mass2Position)*speed;

                float maxPossible_dt = 0.002f; // Maximum Possible dt Is 0.002 Seconds
                // This Is Needed To Prevent Pass Over Of A Non-Precise dt Value

                // Calculate Number Of Iterations To Be Made At This Update Depending On maxPossible_dt And dt
                int numOfIterations = Math.Max((int) (seconds/maxPossible_dt) + 1, 10);

                var dt = seconds/numOfIterations;

                for (int a = 0; a < numOfIterations; ++a) // We Need To Iterate Simulations "numOfIterations" Times
                {
                    _ropeSimulation.Operate(dt);
                }

                var velocity = new Vector();

                var points = _ropeSimulation
                    .Masses
                    .Select(mass => new Point(mass.Pos.X, mass.Pos.Y))
                    .ToList();

                velocity = _ropeSimulation.Masses.Aggregate(velocity, (current, mass) => current + mass.Vel);

                if (velocity.Length < 0.5)
                {
                    _timer.Stop();
//                    points = new List<Point> {points.First(), points.Last()};
                }

//                var p1 = endPoint - startPoint;
//                var p2 = points[points.Count - 1] - points[0];
//
////                if (Math.Abs(p2.X) > 10 && Math.Abs(p2.Y) > 10)
//                for (int i = 0; i < points.Count; i++)
//                {
//                    var point = points[i];
//                    var v = point - startPoint;
//                    v.X *= p1.X/p2.X;
//                    v.Y *= p1.Y/p2.Y;
//                    points[i] = startPoint + v;
//                }

                var polyLineSegment = new PolyLineSegment(points.Skip(1), true);
                var pf = new PathFigure(points.First(), new[] {polyLineSegment}, false);
                return new PathGeometry {Figures = {pf}};
            }
        }
    }
}