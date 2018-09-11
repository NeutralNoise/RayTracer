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
            mat = new Material(new ColourRGBA(255, 255, 255, 255), new ColourRGBA(255, 255, 255, 255));
        }

        public Light(Vector pos, Vector dir)
        {
            this.pos = pos;
            this.dir = dir;
            mat = new Material(new ColourRGBA(255, 255, 255, 255), new ColourRGBA(255, 255, 255, 255));
        }

        public Light(Vector pos, Vector dir, Material mat)
        {
            this.pos = pos;
            this.dir = dir;
            this.mat = mat;
        }

        public Vector pos;
        public Vector dir;

        public Material mat;

    }
}
