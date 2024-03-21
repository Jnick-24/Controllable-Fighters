using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.PlaneParts
{
    internal class ControllablePlane : ControllableShip
    {
        CT_Engine Engine = new CT_Engine(100);
        List<CT_Wing> Wings;

        const float mass = 10000.0f;
        const float thrust = 50000.0f;

        const float wing_offset = -1.0f;
        const float tail_offset = -6.6f;

        CT_Airfoil NACA_0012 = new CT_Airfoil(CT_Data.NACA_0012_data);
        CT_Airfoil NACA_2412 = new CT_Airfoil(CT_Data.NACA_2412_data);

        public override void Init()
        {
            base.Init();
            NACA_0012 = new CT_Airfoil(CT_Data.NACA_0012_data);
            NACA_2412 = new CT_Airfoil(CT_Data.NACA_2412_data);

            Wings = new List<CT_Wing>()
            {
                new CT_Wing(new Vector3D(wing_offset, 0.0f, -2.7f), 6.96f, 2.50f, NACA_2412, Vector3D.Up),             // left wing
                //new CT_Wing(new Vector3D(wing_offset - 1.5f, 0.0f, -2.0f), 3.80f, 1.26f, NACA_0012, Vector3D.Up),      // left aileron
                //new CT_Wing(new Vector3D(wing_offset - 1.5f, 0.0f, 2.0f), 3.80f, 1.26f, NACA_0012, Vector3D.Up),       // right aileron
                new CT_Wing(new Vector3D(wing_offset, 0.0f, +2.7f), 6.96f, 2.50f, NACA_2412, Vector3D.Up),             // right wing
                //new CT_Wing(new Vector3D(tail_offset, -0.1f, 0.0f), 6.54f, 2.70f, NACA_0012, Vector3D.Up),             // elevator
                new CT_Wing(new Vector3D(tail_offset, 0.0f, 0.0f), 5.31f, 3.10f, NACA_0012, Vector3D.Right),  // rudder
            };
        }

        public void Update()
        {
            Engine.ApplyForce(this);

            Wings[0].ApplyForce(this, debug: true);
            Wings[1].ApplyForce(this);

            //foreach (var wing in Wings)
            //{
            //    wing.ApplyForce(this);
            //}
        }
    }
}
