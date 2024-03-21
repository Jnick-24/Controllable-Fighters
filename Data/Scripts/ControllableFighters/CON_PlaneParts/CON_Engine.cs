using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Components;
using VRageMath;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.CON_PlaneParts
{
    internal class CON_Engine
    {
        public float Throttle = 1;
        public float Thrust = 0;

        public CON_Engine(float thrust)
        {
            Thrust = thrust;
        }

        public void ApplyForce(CON_ControllablePlane plane)
        {
            plane.Physics.AddForce(MyPhysicsForceType.APPLY_WORLD_FORCE, plane.WorldMatrix.Forward * (Throttle * Thrust), plane.PositionComp.GetPosition(), null);
        }
    }
}
