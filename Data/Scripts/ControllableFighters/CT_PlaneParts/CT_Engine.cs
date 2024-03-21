using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Entity;
using VRageMath;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.PlaneParts
{
    internal class CT_Engine
    {
        public float Throttle = 0f;
        float Thrust;

        public CT_Engine(float thrust)
        {
            Thrust = thrust;
        }

        public void ApplyForce(ControllablePlane plane, float delta = 1/60f)
        {
            if (Throttle > 0)
                plane.Physics.ApplyImpulse(plane.WorldMatrix.Forward * (Throttle * Thrust) * delta, plane.PositionComp.GetPosition());
        }
    }
}
