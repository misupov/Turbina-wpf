using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Turbina.Editors.Ropes
{
    public class Rope : Shape
    {
        private readonly DispatcherTimer _timer;
        private Stopwatch _sw;
        private Vector _goto1;
        private Vector _goto2;
        private readonly RopeSimulation _ropeSimulation = new RopeSimulation(
            40, // 40 Particles (Masses)
            1f, // Each Particle Has A Weight Of 50 Grams
            10000.0f, // springConstant In The Rope
            0.0f, // Normal Length Of Springs In The Rope
            0.0002f, // Spring Inner Friction Constant
            new Vector(0, 0), // Gravitational Acceleration
            5); // Height Of Ground

        public Rope()
        {
            Stroke = Brushes.Blue;
            StrokeThickness = 3;
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.Render, Callback, Dispatcher.CurrentDispatcher);
            _timer.Start();
            _sw = Stopwatch.StartNew();
        }

        private void Callback(object sender, EventArgs eventArgs)
        {
            InvalidateVisual();
        }

        public void Goto1(Vector position)
        {
            var gotoVector = new Vector((position.X - ActualWidth / 2), (position.Y - ActualHeight / 2));
            _goto1 = gotoVector;

            if (!_timer.IsEnabled)
            {
                _timer.Start();
                _sw.Restart();
            }
        }

        public void Goto2(Vector position)
        {
            var gotoVector = new Vector((position.X - ActualWidth / 2), (position.Y - ActualHeight / 2));
            _goto2 = gotoVector;

            if (!_timer.IsEnabled)
            {
                _timer.Start();
                _sw.Restart();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            ReleaseMouseCapture();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                var speed = 5;

                var seconds = _sw.Elapsed.TotalSeconds * speed;
                _sw = Stopwatch.StartNew();

                _ropeSimulation.RopeConnectionVel1 = (_goto1 - _ropeSimulation.Mass1Position) * speed;
                _ropeSimulation.RopeConnectionVel2 = (_goto2 - _ropeSimulation.Mass2Position) * speed;

                float maxPossible_dt = 0.002f; // Maximum Possible dt Is 0.002 Seconds
                                               // This Is Needed To Prevent Pass Over Of A Non-Precise dt Value

                // Calculate Number Of Iterations To Be Made At This Update Depending On maxPossible_dt And dt
                int numOfIterations = Math.Max((int)(seconds / maxPossible_dt) + 1, 10);

                var dt = seconds / numOfIterations;

                for (int a = 0; a < numOfIterations; ++a) // We Need To Iterate Simulations "numOfIterations" Times
                {
                    _ropeSimulation.Operate(dt);
                }

                var velocity = new Vector();

                var points = _ropeSimulation
                    .Masses
                    .Select(mass => new Point(RenderSize.Width / 2 + mass.Pos.X, RenderSize.Height / 2 + mass.Pos.Y))
                    .ToList();

                velocity = _ropeSimulation.Masses.Aggregate(velocity, (current, mass) => current + mass.Vel);

                if (velocity.Length < 0.5)
                {
                    _timer.Stop();
//                    points = new List<Point> {points.First(), points.Last()};
                }

                var polyLineSegment = new PolyLineSegment(points.Skip(1), true);
                var pf = new PathFigure(points.First(), new [] { polyLineSegment }, false);
                return new PathGeometry {Figures = {pf}};
            }
        }
    }
}