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
            m_emitColour = new ColourRGBA(0, 0, 0, 255);
        }
        public Material(float r, float g, float b, float a)
        {
            m_colour = new ColourRGBA(r,g,b,a);
            m_emitColour = new ColourRGBA(0, 0, 0, 255);
        }

        public Material(ColourRGBA c, ColourRGBA ec)
        {
            m_colour = c;
            m_emitColour = ec;
        }

        public ColourRGBA GetColour()
        {
            return m_colour;
        }

        public ColourRGBA GetEmitColour()
        {
            return m_emitColour;
        }

        public Material(Material mat)
        {
            m_colour = mat.m_colour;
        }

        public void SetEmitColour(ColourRGBA colour)
        {
            m_emitColour = colour;
        }

        public void SetEmitColour(float r, float g, float b, float a)
        {
            m_emitColour.SetColour(r, g, b, a);
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
        private ColourRGBA m_emitColour;
        public float reflect = 0.9f;
    }
}
