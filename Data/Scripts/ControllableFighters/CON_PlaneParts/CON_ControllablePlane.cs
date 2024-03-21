using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.CON_PlaneParts
{
    internal class CON_ControllablePlane : ControllableShip
    {
        CON_Engine Engine = new CON_Engine(50000.0f);
        CON_Wing Wing = new CON_Wing(Vector3D.Zero, 6.96f, 2.50f, Vector3D.Up);

        public void Update()
        {
            //Engine.ApplyForce(this);
            Wing.ApplyForce(this);
        }
    }
}
