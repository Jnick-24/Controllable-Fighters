using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters.PlaneParts
{
    internal class CT_Airfoil
    {
        float min_alpha, max_alpha;

        public float ClMax = 0;

        Curve CurveCl = new Curve();
        Curve CurveCd = new Curve();
        //Vector3D[] data;

        /// <summary>
        /// Populates from a Vector3 array with contents: Alpha, Cl, Cd
        /// </summary>
        /// <param name="data"></param>
        public CT_Airfoil(Vector3[] data) : base()
        {
            min_alpha = data[0].X;
            max_alpha = data[data.Length - 1].X;

            foreach (var item in data)
            {
                CurveCl.Keys.Add(new CurveKey(item.X, item.Y));
                CurveCd.Keys.Add(new CurveKey(item.X, item.Z));

                if (item.Y > ClMax)
                    ClMax = item.Y;
            }
        }

        public float SampleCl(float alpha)
        {
            return CurveCl.Evaluate(alpha);
        }

        public float SampleCd(float alpha)
        {
            return CurveCd.Evaluate(alpha);
        }
    }
}
