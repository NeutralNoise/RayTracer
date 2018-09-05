using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class Config
    {
        static public float Tolerance = 0.0001f;
        static public Vector Orgin = new Vector(0.00f, 0.00f, 0.00f);
        static public Vector UP = new Vector(0.0f, 0.0f, 1.0f);
        static public float MinTolerance = 0.0001f;
        static public float MinHitDistance = 0.001f; //Clipingplan.

        public class Ray
        {
            static public UInt64 MaxBounce = 3;
        }


    }
}
