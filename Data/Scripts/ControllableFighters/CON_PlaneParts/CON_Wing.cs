using Controllable_Fighters.Data.Scripts.ControllableFighters.PlaneParts;
using Sandbox.ModAPI;
using VRageMath;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.CON_PlaneParts
{
    internal class CON_Wing
    {
        Vector3D Position, Normal;
        double Wingspan, Area, AspectRatio;

        public CON_Wing(Vector3D position, double span, double chord, Vector3D normal)
        {
            Position = position;
            Area = span * chord;
            Wingspan = span;
            Normal = normal;
            AspectRatio = span * span / Area;
        }

        public void ApplyForce(CON_ControllablePlane plane)
        {
            Vector3D localVelocity = Vector3D.Rotate(plane.Physics.GetVelocityAtPoint(Vector3D.Transform(Position, plane.WorldMatrix)), plane.WorldMatrix);
            double speed = localVelocity.Length();

            if (speed <= 1)
                return;



            //plane.Physics.AddForce(MyPhysicsForceType.APPLY_WORLD_FORCE, plane.WorldMatrix.Forward * (Throttle * Thrust), plane.PositionComp.GetPosition(), null);
        }
    }
}
