using System.Windows;

namespace Turbina.Editors.Ropes
{
    internal class Mass
    {
        public Mass(double m)
        {
            M = m;
        }

        public double M { get; set; }

        public Vector Pos { get; set; }

        public Vector Vel { get; set; }

        public Vector Force { get; private set; }

        /// <summary>
        /// void ApplyForce(Vector force) method is used to add external force to the mass. 
        /// At an instance in time, several sources of force might affect the mass. The vector sum 
        /// of these forces make up the net force applied to the mass at the instance.
        /// </summary>
        public void ApplyForce(Vector force)
        {
            Force += force;					// The external force is added to the force of the mass
        }

        /*   void Init() method sets the force values to zero */
        public void Init()
        {
            Force = new Vector();
        }

        /*
          void Simulate(float dt) method calculates the new velocity and new position of 
          the mass according to change in time (dt). Here, a simulation method called
          "The Euler Method" is used. The Euler Method is not always accurate, but it is 
          simple. It is suitable for most of physical simulations that we know in common 
          computer and video games.
        */
        public void Simulate(double dt)
        {
            Vel += (Force / M) * dt;				// Change in velocity is added to the velocity.
            // The change is proportinal with the acceleration (force / m) and change in time

            Pos += Vel * dt;						// Change in position is added to the position.
            // Change in position is velocity times the change in time
        }
    }
}