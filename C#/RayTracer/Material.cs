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
            m_colour = new ColourRGBA();
        }
        public Material(float r, float g, float b, float a)
        {
            m_colour = new ColourRGBA(r,g,b,a);
        }

        public ColourRGBA GetColour()
        {
            return m_colour;
        }

        public Material(Material mat)
        {
            m_colour = mat.m_colour;
        }

        public void SetColour(ColourRGBA colour)
        {
            m_colour = colour;
        }

        public void SetColour(float r, float g, float b, float a)
        {
            m_colour.SetColour(r, g, b, a);
        }

        private ColourRGBA m_colour;
    }
}
