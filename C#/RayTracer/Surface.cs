using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class Surface
    {

        public Surface()
        {

        }

        public float d = 0;
        //This is for defining a plane.
        public Vector norm = new Vector(0.0f, 0.0f, 1.0f);
        public Material m_mat;
        public Vertex[] m_pos;

        public virtual bool Intersect(Ray r, ref float dist)
        {
            //Check if the ray hits the plane
            //for the ray
            //f(t) = ro + t * rd
            // ro is ray orgin
            // rd is ray direction
            // where t is marching down the ray dir from the orgin

            //N(ro + t * rd) + d = 0;
            float Denom = norm.DotProduct(r.dir);
            if((Denom < -Config.Tolerance) || (Denom > Config.Tolerance))
            {
                dist = (-d - norm.DotProduct(r.orgin)) / Denom;
                return true;
            }

            return false;
        }

        public virtual Ray Reflect(Vector vec, Vector normal)
        {
            return null;
        }

    }

    class SurfaceSphere : Surface
    {

        public float r = 1.0f;
        public Vector pos = new Vector(0, 0.0f, 0.0f);

        public override bool Intersect(Ray ray, ref float dist)
        {
            /*
            float a = ray.dir.DotProduct(ray.dir);
            float b = 2.0f * ray.dir.DotProduct(ray.vec);
            //float b = ray.dir * (2.0f * (ray.vec - pos));
            float c = ray.vec.DotProduct(ray.vec) - r * r;
            //float d = b * b - 4.0f * a * c;
            float d = 2.0f * a;

            //Ray can not intersect.
            if (d < 0.0f)
            {
                return false;
            }

            d = (float)Math.Sqrt(d);

            // Ray can intersect the sphere, solve the closer hitpoint
            float t = (-0.5f) * (b + d) / a;

            if(t > 0.0f)
            {
                dist = (float)Math.Sqrt(a) * t;
            }
            else
            {
                return false;
            }

            return true;
            */


            //The sphere normal
            Vector normal = (ray.orgin - pos).Normalize();

            //move everything relitve to the sphere
            Vector LocalSphereRayOrgin = ray.orgin - pos;

            float a = ray.dir.DotProduct(ray.dir);
            float b = 2.0f * ray.dir.DotProduct(LocalSphereRayOrgin);
            float c = ray.orgin.DotProduct(LocalSphereRayOrgin) - r * r;

            float Denom = 2.0f * a;
            float RootTerm = (float)Math.Sqrt(b * b - 4.00f * a * c);
            float d = (b * b - 4.00f * a * c);
            if(d < 0.0f)
            {
                return false;
            }
            if (RootTerm > Config.Tolerance)
            {               
                float tp = (-b + RootTerm) / Denom;
                float tn = (-b - RootTerm) / Denom;

                if (tp == 0 || tn == 0)
                {
                    dist = 0;
                    return false;
                }

                float t = tp;

                if(tn > 0 && tn < tp)
                {
                    t = tn;
                    dist = t;
                    return true;
                }
                else if(tp > 0)
                {
                    dist = t;
                    return true;
                }
                
                return false;
            }

            return false;
            
        }

        public override Ray Reflect(Vector vec, Vector normal)
        {
            return null;
        }
    }

    struct SurfaceFunc
    {
        static public Vector CalculateSurfaceNormal(Vertex[] v)
        {
            Vector rtn = new Vector();
            for (int i = 0; i < v.Length; i++) {

                Vector current = v[i].m_pos;
                Vector next = v[(i + 1) % v.Length].m_pos;

                rtn.m_x = rtn.m_x + ((current.m_y - next.m_y) * (current.m_z + next.m_z));
                rtn.m_y = rtn.m_y + ((current.m_z - next.m_z) * (current.m_x + next.m_x));
                rtn.m_z = rtn.m_z + ((current.m_x - next.m_x) * (current.m_y + next.m_y));
                //rtn.PrintVector();
            }
            rtn = rtn.Normalize();

            return rtn;
            //return rtn;
        }

        static public double Vec2Distance(Vector v1, Vector v2)
        {
            return Math.Sqrt(((v1.m_x - v2.m_x) * (v1.m_x - v2.m_x))
                    + ((v1.m_y - v2.m_y) * (v1.m_y - v2.m_y)));
        }

        static public double Vec3Distance(Vector v1, Vector v2)
        {
            return Math.Sqrt(((v1.m_x - v2.m_x) * (v1.m_x - v2.m_x))
                    + ((v1.m_y - v2.m_y) * (v1.m_y - v2.m_y))
                    + ((v1.m_z - v2.m_z) * (v1.m_z - v2.m_z)));
        }

        static public Vector Reflect(Vector vec, Vector normal)
        {
            //Vector d = normal.Scale(vec.DotProduct(normal));
            Vector d = normal.Scale(vec.Normalize().DotProduct(normal));
            Vector s = d.Scale(2);

            return s - vec;
        }

    }
    
    class SurfaceTriangle : Surface
    {

        public SurfaceTriangle()
        {
            m_pos = new Vertex[] {new Vertex(), new Vertex(), new Vertex() };
        }

        public SurfaceTriangle(Vector v1, Vector v2, Vector v3)
        {
            m_pos = new Vertex[] { new Vertex(v1), new Vertex(v2), new Vertex(v3) };
        }

        public override bool Equals(object obj)
        {
            var triangle = obj as SurfaceTriangle;
            return triangle != null &&
                   EqualityComparer<Vertex[]>.Default.Equals(m_pos, triangle.m_pos);
        }

        public override int GetHashCode()
        {
            return -444094959 + EqualityComparer<Vertex[]>.Default.GetHashCode(m_pos);
        }
    }

    class SurfaceQuad : Surface
    {

        public SurfaceQuad()
        {
            m_pos = new Vertex[] {new Vertex(), new Vertex() , new Vertex() , new Vertex() } ; 
        }

        public SurfaceQuad(Vector v1, Vector v2, Vector v3, Vector v4)
        {
            m_pos = new Vertex[] {new Vertex(v1), new Vertex(v2), new Vertex(v3), new Vertex(v4) };
        } 

        public override bool Equals(object obj)
        {
            var quad = obj as SurfaceQuad;
            return quad != null &&
                   EqualityComparer<Vertex[]>.Default.Equals(m_pos, quad.m_pos);
        }

        public override int GetHashCode()
        {
            return -444094959 + EqualityComparer<Vertex[]>.Default.GetHashCode(m_pos);
        }
    }
}

