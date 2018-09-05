using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{

    class ColourRGBA
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public ColourRGBA()
        {
            r = 0;
            g = 0;
            b = 0;
            a = 0;
        }

        public ColourRGBA(float r, float g, float b, float a) 
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public void SetColour(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public void Normalize()
        {
            if(r != 0)
            {
                r /= 255;
            }
            if (g != 0)
            {
                g /= 255;
            }
            if (b != 0)
            {
                b /= 255;
            }
            if (a != 0)
            {
                a /= 255;
            }

        }

        public override bool Equals(object obj)
        {
            var rGBA = obj as ColourRGBA;
            return rGBA != null &&
                   r == rGBA.r &&
                   g == rGBA.g &&
                   b == rGBA.b &&
                   a == rGBA.a;
        }

        public override int GetHashCode()
        {
            var hashCode = -490236692;
            hashCode = hashCode * -1521134295 + r.GetHashCode();
            hashCode = hashCode * -1521134295 + g.GetHashCode();
            hashCode = hashCode * -1521134295 + b.GetHashCode();
            hashCode = hashCode * -1521134295 + a.GetHashCode();
            return hashCode;
        }

        public static ColourRGBA operator +(ColourRGBA a, ColourRGBA b)
        {
            ColourRGBA rtn = new ColourRGBA();
            if((a.r + b.r) > 255)
            {
                rtn.r = 255;
            }
            else if((a.r + b.r) < 0)
            {
                rtn.r = 0;
            }
            else
            {
                rtn.r = a.r + b.r;
            }



            if ((a.g + b.g) > 255)
            {
                rtn.g = 255;
            }
            else if ((a.g + b.g) < 0)
            {
                rtn.g = 0;
            }
            else
            {
                rtn.g = a.g + b.g;
            }


            if ((a.b + b.b) > 255)
            {
                rtn.b = 255;
            }
            else if ((a.b + b.b) < 0)
            {
                rtn.b = 0;
            }
            else
            {
                rtn.b = a.b + b.b;
            }


            if ((a.a + b.a) > 255)
            {
                rtn.a = 255;
            }
            else if ((a.a + b.a) < 0)
            {
                rtn.a = 0;
            }
            else
            {
                rtn.a = a.a + b.a;
            }



            return rtn;
        }

        public static ColourRGBA operator /(ColourRGBA colour, float f)
        {
            return new ColourRGBA(colour.r / f, colour.g / f, colour.b / f, colour.a / f);
        }

        public static ColourRGBA operator /(float f, ColourRGBA colour)
        {
            return new ColourRGBA(colour.r / f, colour.g / f, colour.b / f, colour.a / f);
        }

    }

    class Vertex
    {

        public Vertex()
        {
            m_pos = new Vector();
            m_colour = new ColourRGBA();
        }

        public Vertex(Vector v)
        {
            m_pos = v;
            m_colour = new ColourRGBA();
        }


        public Vector m_pos;
        public ColourRGBA m_colour;

    }
}
