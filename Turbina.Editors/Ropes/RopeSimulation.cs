using System.Collections.Generic;
using System.Windows;

namespace Turbina.Editors.Ropes
{
    internal class RopeSimulation
    {
        private readonly List<Spring> _springs = new List<Spring>();
        private readonly Vector _gravitation; //gravitational acceleration (gravity will be applied to all masses)
        private Vector _ropeConnectionPos1; //A point in space that is used to set the position of the first mass in the system (mass with index 0)
        private Vector _ropeConnectionPos2; //A point in space that is used to set the position of the first mass in the system (mass with index 0)
        private readonly double _airFrictionConstant; //a constant of air friction applied to masses

        public RopeSimulation( //a long long constructor with 11 parameters starts here
            int numOfMasses, //1. the number of masses
            double m, //2. weight of each mass
            double springConstant, //3. how stiff the springs are
            double springLength, //4. the length that a spring does not exert any force
            double springFrictionConstant, //5. inner friction constant of spring
            Vector gravitation, //6. gravitational acceleration
            double airFrictionConstant //11. height of the ground (y position)
            )
        {
            _gravitation = gravitation;
            _airFrictionConstant = airFrictionConstant;

            for (int i = 0; i < numOfMasses; i++)
            {
                Masses.Add(new Mass(m));
            }

            var index = 1;
            while (index < Masses.Count)
            {
                _springs.Add(new Spring(Masses[index - 1], Masses[index], springConstant, springLength, springFrictionConstant));

                index++;
            }
        }

        public List<Mass> Masses { get; } = new List<Mass>();

        public Vector Mass1Position => Masses[0].Pos;

        public Vector Mass2Position => Masses[Masses.Count - 1].Pos;

        public Vector RopeConnectionVel1 { get; set; } //a variable to move the ropeConnectionPos1 (by this, we can swing the rope)

        public Vector RopeConnectionVel2 { get; set; } //a variable to move the ropeConnectionPos2 (by this, we can swing the rope)

        public void Init()
        {
            foreach (var mass in Masses)
            {
                mass.Init();
            }
        }

        public void Solve()
        {
            foreach (var spring in _springs)
            {
                spring.Solve();
            }

//            var i = 0.0;
//            var count = Masses.Count;

            foreach (var mass in Masses)
            {
                mass.ApplyForce(_gravitation * mass.M); //The gravitational force

                mass.ApplyForce(-mass.Vel * _airFrictionConstant); //The air friction

//                i++;
//                mass.ApplyForce(new Vector((count - i) / count * 2000, 0)); //The air friction
            }
        }

        public void Simulate(double dt)
        {
            foreach (var mass in Masses)
            {
                mass.Simulate(dt); // Iterate the mass and obtain new position and new velocity
            }

            _ropeConnectionPos1 += RopeConnectionVel1 * dt; // iterate the positon of ropeConnectionPos1
            _ropeConnectionPos2 += RopeConnectionVel2 * dt; // iterate the positon of ropeConnectionPos2

            Masses[0].Pos = _ropeConnectionPos1; // mass with index "0" shall position at ropeConnectionPos1
            Masses[0].Vel = RopeConnectionVel1; // the mass's velocity is set to be equal to RopeConnectionVel1

            Masses[Masses.Count - 1].Pos = _ropeConnectionPos2;
            Masses[Masses.Count - 1].Vel = RopeConnectionVel2;
        }

        public void Operate(double dt) // The complete procedure of simulation
        {
            Init(); // Step 1: reset forces to zero
            Solve(); // Step 2: apply forces
            Simulate(dt); // Step 3: iterate the masses by the change in time
        }
    }
}