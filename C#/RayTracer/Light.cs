using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class Light
    {

        public Light()
        {
            pos = new Vector(0, 0,0);
            dir = new Vector(0, 0,0);
            intesity = new ColourRGBA(1.0f, 1.0f, 1.0f, 1.0f);
        }

        public Light(Vector pos, Vector dir)
        {
            this.pos = pos;
            this.dir = dir;
            intesity = new ColourRGBA(1.0f, 1.0f, 1.0f, 1.0f);
        }

        public Light(Vector pos, Vector dir, ColourRGBA intesity)
        {
            this.pos = pos;
            this.dir = dir;
            this.intesity = intesity;
        }

        public Vector pos;
        public Vector dir;
        public ColourRGBA intesity;
        //public Material mat;

    }
}
