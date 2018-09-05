using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class Helpers
    {
        public class Rand
        {
            static public float RandomBilateral()
            {
                return (float)ran.NextDouble();
            }

            static public float RandomUnilateral()
            {
                return ran.Next() / (float)System.Int32.MaxValue;
            }

            static Random ran = new Random();
        }
    }
}
