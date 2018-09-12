using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class Material
    {
        public Material()
        {
            reflect = 1.0f;
            disfuse = new ColourRGBA(1.0f, 1.0f, 1.0f, 1.0f);
        }
        public Material(float r, float g, float b, float a)
        {
            reflect = 1.0f;
            disfuse = new ColourRGBA(r, b, b, a);
        }

        public Material(float r, ColourRGBA d)
        {
            reflect = r;
            disfuse = d;
        }

        public Material(Material mat)
        {
            reflect = mat.reflect;
            disfuse = mat.disfuse;
        }

        public float reflect;
        public ColourRGBA disfuse = new ColourRGBA(1.0f, 1.0f, 1.0f, 1.0f);
    }
}
