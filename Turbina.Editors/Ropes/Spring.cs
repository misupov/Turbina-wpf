using System;
using System.Windows;

namespace Turbina.Editors.Ropes
{
    /// <summary>
    /// An Object To Represent A Spring With Inner Friction Binding Two Masses.
    /// </summary>
    internal class Spring
    {
        private readonly Mass _mass1; // The First Mass At One Tip Of The Spring
        private readonly Mass _mass2; // The Second Mass At The Other Tip Of The Spring

        private readonly double _springConstant; // A Constant To Represent The Stiffness Of The Spring
        private readonly double _springLength; // The Length That The spring Does Not Exert Any Force
        private readonly double _frictionConstant; // A Constant To be Used For The Inner Friction Of The Spring 

        public Spring(Mass mass1, Mass mass2, double springConstant, double springLength, double frictionConstant)
        {
            _springConstant = springConstant; //set the springConstant
            _springLength = springLength; //set the springLength
            _frictionConstant = frictionConstant; //set the frictionConstant

            _mass1 = mass1; //set _mass1
            _mass2 = mass2; //set mass2
        }

        public void Solve() //Solve() method: the method where forces can be applied
        {
            var springVector = _mass1.Pos - _mass2.Pos; //vector between the two masses

            var r = springVector.Length; //distance between the two masses


            var force = new Vector(); //force initially has a zero value

            if (Math.Abs(r) > 0.001) //to avoid a division by zero check if r is zero
            {
                force = (springVector / r) * (r - _springLength) * (-_springConstant); //the spring force is added to the force
            }

            force += -(_mass1.Vel - _mass2.Vel) * _frictionConstant; //the friction force is added to the force
            //with this addition we obtain the net force of the spring

            _mass1.ApplyForce(force); //force is applied to _mass1
            _mass2.ApplyForce(-force); //the opposite of force is applied to mass2
        }
    }
}