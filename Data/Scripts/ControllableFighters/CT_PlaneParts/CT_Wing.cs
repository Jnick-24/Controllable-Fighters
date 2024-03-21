using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;
using Controllable_Fighters.Data.Scripts.Debug;
using Sandbox.ModAPI;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.PlaneParts
{
    // https://www.jakobmaier.at/posts/flight-simulation/

    internal class CT_Wing
    {
        const float EfficencyRatio = 1;

        CT_Airfoil Airfoil;
        Vector3D CenterOfPressure;
        float Area;
        float Wingspan;
        Vector3D Normal;
        float AspectRatio;
        float FlapRatio;

        float ControlInput = 0;

        public CT_Wing(Vector3D position, float span, float chord, CT_Airfoil airfoil, Vector3D normal, float flapRatio = 0.25f)
        { 
            Airfoil = airfoil;
            CenterOfPressure = position;
            Area = span * chord;
            Wingspan = span;
            Normal = normal;
            AspectRatio = span * span / Area;
            FlapRatio = flapRatio;
        }

        public void SetControlInput(float input)
        {
            ControlInput = MathHelper.Clamp(input, -1, 1);
        }

        public void ApplyForce(ControllablePlane plane, float delta = 1/60f, bool debug = false)
        {
            Vector3D relativePos = CenterOfPressure + plane.PositionComp.GetPosition();

            Vector3 localVelocity = Vector3D.Rotate(plane.Physics.GetVelocityAtPoint(plane.PositionComp.GetPosition() + Vector3D.Rotate(CenterOfPressure, plane.WorldMatrix)), plane.WorldMatrix);
            double speed = localVelocity.Length();

            if (speed <= 1)
                return;

            // drag acts in the opposite direction of velocity
            Vector3D dragDirection = -localVelocity.Normalized();

            // lift is always perpendicular to drag
            Vector3D liftDirection = Vector3D.Cross(Vector3D.Cross(dragDirection, Normal), dragDirection).Normalized();

            if ((liftDirection + localVelocity).Length() > speed)
                liftDirection = -liftDirection;

            // angle between chord line and air flow
            double angleOfAttack = MathHelper.ToDegrees(Math.Asin(Vector3D.Dot(dragDirection, Normal)));

            // sample aerodynamic coefficients
            double liftCoefficient = Airfoil.SampleCl((float) angleOfAttack);
            double dragCoefficient = Airfoil.SampleCd((float) angleOfAttack);

            if (FlapRatio > 0)
            {
                double deflectionRatio = ControlInput;

                // lift coefficient changes based on flap deflection
                double delta_lift_coeff = Math.Sqrt(FlapRatio) * Airfoil.ClMax * deflectionRatio;
                liftCoefficient += delta_lift_coeff;
            }

            // induced drag, increases with lift
            double inducedDragCoefficient = liftCoefficient * liftCoefficient / (Math.PI * AspectRatio * EfficencyRatio);
            dragCoefficient += inducedDragCoefficient;

            double airDensity = 1; // TODO

            double dynamicPressure = 0.5 * speed * speed * airDensity * Area;

            Vector3 lift = liftDirection * (liftCoefficient * dynamicPressure);
            Vector3 drag = dragDirection * (dragCoefficient * dynamicPressure);

            if (debug)
            {
                DebugDraw.DrawLineZT(Vector3D.Zero, localVelocity, Color.Green, 0.25f);
                DebugDraw.DrawLineZT(Vector3D.Zero, Vector3D.Rotate(lift, plane.WorldMatrix), Color.Blue, 0.25f);
                DebugDraw.DrawLineZT(Vector3D.Zero, Vector3D.Rotate(drag, plane.WorldMatrix), Color.Red, 0.25f);
                MyAPIGateway.Utilities.ShowNotification($"AoA: {Math.Round(angleOfAttack, 1)}", 1000/60);
            }

            plane.Physics.AddForce(VRage.Game.Components.MyPhysicsForceType.APPLY_WORLD_FORCE, Vector3D.Rotate(lift + drag, plane.WorldMatrix), plane.PositionComp.GetPosition() + Vector3D.Rotate(CenterOfPressure, plane.WorldMatrix), null);
        }
    }
}
