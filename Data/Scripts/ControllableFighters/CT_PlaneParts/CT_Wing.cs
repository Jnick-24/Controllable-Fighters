﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;
using Controllable_Fighters.Data.Scripts.Debug;
using Sandbox.ModAPI;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using VRage.Input;
using VRage.Game.Entity;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.PlaneParts
{
    // https://www.jakobmaier.at/posts/flight-simulation/

    internal class CT_Wing
    {
        const float EfficencyRatio = 1f;

        public string Name;
        CT_Airfoil Airfoil;
        Vector3D CenterOfPressure;
        float Area;
        float Wingspan;
        public Vector3D Normal;
        public readonly Vector3D RestNormal;
        public readonly Vector3D RotationAxis;
        float AspectRatio;
        float FlapRatio;

        float ControlInput = 0;

        public CT_Wing(string name, Vector3D position, float span, float chord, CT_Airfoil airfoil, Vector3D normal, Vector3 rotationAxis, float flapRatio = 0.25f)
        {
            Name = name;
            Airfoil = airfoil;
            CenterOfPressure = position;
            Area = span * chord;
            Wingspan = span;
            RestNormal = normal;
            RotationAxis = rotationAxis;
            Normal = normal;
            AspectRatio = span * span / Area;
            FlapRatio = flapRatio;
        }

        public void SetControlInput(float input)
        {
            ControlInput = MathHelper.Clamp(input, -1, 1);
        }

        public void SetAngle(float angle)
        {
            Normal = RestNormal.Rotate(RotationAxis, angle);
        }

        private void RotateSubpart(ControllablePlane plane)
        {
            MyEntitySubpart subpart = plane.SubpartManager.GetSubpart(plane, Name);
            if (subpart == null)
                return;
            Matrix matrix = Matrix.CreateFromDir(Normal.Cross(RotationAxis), Normal);
            plane.SubpartManager.LocalRotateSubpartAbs(subpart, matrix);
        }

        public void ApplyForce(ControllablePlane plane, float delta = 1/60f, bool debug = false)
        {
            RotateSubpart(plane);

            //Vector3 localVelocity = plane.Physics.GetVelocityAtPoint(LocalToWorld(CenterOfPressure, plane));
            Vector3 localVelocity = WorldToLocal(plane.Physics.LinearVelocity + plane.PositionComp.GetPosition(), plane);
            double speed = localVelocity.Length();

            if (speed <= 1)
                return;

            // drag acts in the opposite direction of velocity
            Vector3D dragDirection = -localVelocity.Normalized();

            // lift is always perpendicular to drag
            //Vector3D liftDirection = Vector3D.Cross(Vector3D.Cross(dragDirection, Normal), dragDirection).Normalized();
            Vector3D liftDirection = Normal;

            // angle between chord line and air flow
            double angleOfAttack = MathHelper.ToDegrees(Math.Asin(Vector3D.Dot(dragDirection, Normal)));

            // sample aerodynamic coefficients
            double liftCoefficient = Airfoil.SampleCl((float) angleOfAttack);
            double dragCoefficient = Airfoil.SampleCd((float) angleOfAttack);

            if (FlapRatio > 0)
            {
                // lift coefficient changes based on flap deflection
                double delta_lift_coeff = Math.Sqrt(FlapRatio) * Airfoil.ClMax * ControlInput;
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
                Vector3D actualPosition = LocalToWorld(CenterOfPressure, plane);
                
                DebugDraw.DrawLineZT(actualPosition + plane.Physics.LinearVelocity * 1 / 60f, LocalToWorld(Normal * 2 + CenterOfPressure, plane) + plane.Physics.LinearVelocity * 1 / 60f, Color.Green, 0.15f);
                DebugDraw.DrawLineZT(actualPosition + plane.Physics.LinearVelocity * 1 / 60f, LocalToWorld(lift/plane.Physics.Mass + CenterOfPressure, plane) + plane.Physics.LinearVelocity * 1 / 60f, Color.Blue, 0.25f);
                DebugDraw.DrawLineZT(actualPosition + plane.Physics.LinearVelocity * 1 / 60f, LocalToWorld(drag/plane.Physics.Mass + CenterOfPressure, plane) + plane.Physics.LinearVelocity * 1 / 60f, Color.Red, 0.25f);
                DebugDraw.AddGPS(Name, actualPosition, 1/60f);

                //MyAPIGateway.Utilities.ShowNotification($"AoA: {Math.Round(angleOfAttack, 1)} | Drag: {drag.Length()} | Lift: {lift.Length()}", 1000/60);
            }

            plane.Physics.ApplyImpulse(Vector3D.Rotate(lift + drag, plane.WorldMatrix) * delta, LocalToWorld(CenterOfPressure, plane));
            //plane.Physics.AngularVelocity = Vector3.Zero;
        }

        Vector3D WorldToLocal(Vector3D pos, ControllablePlane plane)
        {
            MatrixD inv = MatrixD.Invert(plane.WorldMatrix);
            return Vector3D.Rotate(pos - plane.PositionComp.GetPosition(), inv);
        }
        Vector3D LocalToWorld(Vector3D pos, ControllablePlane plane)
        {
            return Vector3D.Rotate(pos, plane.WorldMatrix) + plane.PositionComp.GetPosition();
        }
    }
}
