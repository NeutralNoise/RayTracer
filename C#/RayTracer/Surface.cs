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
            m_mat = new Material();
        }

        public Material m_mat;

        public virtual bool Intersect(Ray r, ref float dist)
        {
            return false;
        }

        public virtual Ray Reflect(Vector vec, Vector normal)
        {
            return null;
        }

    }

    class SurfacePlane : Surface
    {

        public SurfacePlane()
        {
            d = 0;
            norm = Config.UP;
        }

        public SurfacePlane(float d, Vector n) 
        {
            this.d = d;
            norm = n;
        }

        //How far down the normal is the plane?
        //Well thats what I think this does.
        public float d;
        //Which way is up
        public Vector norm;

        public override bool Intersect(Ray r, ref float dist)
        {
            //Check if the ray hits the plane
            //for the ray
            //f(t) = ro + t * rd
            // ro is ray orgin
            // rd is ray direction
            // where t is marching down the ray dir from the orgin

            //N(ro + t * rd) + d = 0;
            float Denom = norm.DotProduct(r.dir);
            if ((Denom < -Config.Tolerance) || (Denom > Config.Tolerance))
            {
                dist = (-d - norm.DotProduct(r.orgin)) / Denom;
                return true;
            }

            return false;
        }

        public override Ray Reflect(Vector vec, Vector normal)
        {
            return null;
        }

    }

    class SurfaceSphere : Surface
    {
        public SurfaceSphere()
        {
            r = 1.0f;
            pos = new Vector(0.0f, 0.0f, 0.0f);
        }

        public SurfaceSphere(float r, Vector pos)
        {
            this.r = r;
            this.pos = pos;
        }

        public float r = 1.0f;
        public Vector pos;

        public override bool Intersect(Ray ray, ref float dist)
        {
            //The sphere normal this is used when bouncing.
            //Vector normal = (ray.orgin - pos).Normalize();

            //move everything relitve to the sphere
            Vector LocalSphereRayOrgin = ray.orgin - pos;

            float a = ray.dir.DotProduct(ray.dir);
            float b = 2.0f * ray.dir.DotProduct(LocalSphereRayOrgin);
            //float c = ray.orgin.DotProduct(LocalSphereRayOrgin) - r * r;
            float c = LocalSphereRayOrgin.DotProduct(LocalSphereRayOrgin) - r * r;

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
        //TODO: This should be moved into a diffeerent class that this inherets from.
        public Vertex[] m_pos;
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
        //TODO: This should be moved into a diffeerent class that this inherets from.
        public Vertex[] m_pos;
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

