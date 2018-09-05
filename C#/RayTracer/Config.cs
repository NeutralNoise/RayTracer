﻿using System;
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
        static public float MinTolerance = 0.0001f;

        public class Ray
        {
            static public UInt64 MaxBounce = 3;
        }


    }
}
