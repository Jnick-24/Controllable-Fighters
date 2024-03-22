using Controllable_Fighters.Data.Scripts.ControllableFighters.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.GameServices;
using VRageMath;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.PlaneParts
{
    internal class ControllablePlane : ControllableShip
    {
        CT_Engine Engine;
        List<CT_Wing> Wings;
        internal SubpartManager SubpartManager = new SubpartManager();

        const float mass = 10000.0f;
        const float Thrust = 1000000.0f;

        const float wing_offset = 1.0f;
        const float tail_offset = 6.6f;

        CT_Airfoil NACA_0012 = new CT_Airfoil(CT_Data.NACA_0012_data);
        CT_Airfoil NACA_2412 = new CT_Airfoil(CT_Data.NACA_2412_data);

        public float Pitch = 0, Yaw = 0, Roll = 0;

        public float Throttle
        {
            get
            {
                return Engine.Throttle;
            }
            set
            {
                Engine.Throttle = MathHelper.Clamp(value, 0, 1);
            }
        }

        public override void Init(IMyModContext ModContext, string model)
        {
            base.Init(ModContext, model);

            Engine = new CT_Engine(Thrust);

            NACA_0012 = new CT_Airfoil(CT_Data.NACA_0012_data);
            NACA_2412 = new CT_Airfoil(CT_Data.NACA_2412_data);

            Wings = new List<CT_Wing>()
            {
                new CT_Wing("LeftWing",     new Vector3D(-2.7f, 0.0f, wing_offset),        6.96f, 2.50f, NACA_2412, Vector3D.Up, Vector3D.Right), // left wing
                new CT_Wing("LeftAileron",  new Vector3D(-2.0f, 0.0f, wing_offset + 1.5f), 3.80f, 1.26f, NACA_0012, Vector3D.Up, Vector3D.Right), // left aileron
                new CT_Wing("RightAileron", new Vector3D(2.0f, 0.0f, wing_offset + 1.5f),  3.80f, 1.26f, NACA_0012, Vector3D.Up, Vector3D.Right), // right aileron
                new CT_Wing("RightWing",    new Vector3D(+2.7f, 0.0f, wing_offset),        6.96f, 2.50f, NACA_2412, Vector3D.Up, Vector3D.Right), // right wing
                new CT_Wing("Elevator",     new Vector3D(0.0f, -0.1f, tail_offset),        6.54f, 2.70f, NACA_0012, Vector3D.Up, Vector3D.Right), // elevator
                new CT_Wing("Rudder",       new Vector3D(0.0f, 0.0f, tail_offset),         5.31f, 3.10f, NACA_0012, Vector3D.Right, Vector3D.Up), // rudder
            };
        }

        public void Update(bool debug)
        {
            Engine.ApplyForce(this);

            Pitch = MathHelper.Clamp(Pitch, -1, 1);
            Yaw = MathHelper.Clamp(Yaw, -1, 1);
            Roll = MathHelper.Clamp(Roll, -1, 1);

            foreach (var wing in Wings)
            {
                wing.ApplyForce(this, debug: debug);

                switch (wing.Name)
                {
                    case "LeftAileron":
                        wing.SetAngle(Roll);
                        break;
                    case "RightAileron":
                        wing.SetAngle(-Roll);
                        break;
                    case "Elevator":
                        wing.SetAngle(Pitch * 0.5f);
                        break;
                    case "Rudder":
                        wing.SetAngle(Yaw * 0.5f);
                        break;
                }
            }

            Debug.DebugDraw.AddGPS("Fighter", PositionComp.GetPosition(), 1 / 60f);
        }
    }
}
