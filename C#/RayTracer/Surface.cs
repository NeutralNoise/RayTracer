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

        protected Material m_mat;
        //TODO: remove these.
        public float m_dist; //Distance from casting orgin. This is updated every ray.
        public Ray m_lastRay;
        public virtual bool Intersect(Ray r, ref float dist)
        {
            return false;
        }

        public virtual Ray Reflect(Vector vec, Vector dir, float dist, Ray ray)
        {
            return null;
        }

        public virtual Vector CalculateSurfaceNormal()
        {
            return null;
        }

        public void SetMat(Material mat)
        {
            m_mat = mat;
        }

        public Material GetMat()
        {
            return m_mat;
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

        public override Ray Reflect(Vector vec, Vector dir, float dist, Ray ray)
        {

            Vector nextOrgin = new Vector(ray.orgin + dist * ray.dir);
            //Vector nextOrgin = dist * ray.dir;
            Vector nextNormal = norm;

            return SurfaceFunc.Reflect(m_mat, nextOrgin, ray.dir, nextNormal);
        }

        public override Vector CalculateSurfaceNormal()
        {
            return norm;
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
            if(d < Config.MinHitDistance)
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

                if(tn > Config.MinHitDistance && tn < tp)
                {
                    t = tn;
                    dist = t;
                    return true;
                }
                else if(tp > Config.MinHitDistance)
                {
                    dist = t;
                    return true;
                }
                
                return false;
            }

            return false;
            
        }

        public override Ray Reflect(Vector vec, Vector dir, float dist, Ray ray)
        {

            //dir = (dist * ray.dir + (ray.orgin - pos)).Normalize();

            //Vector nextOrgin = new Vector(vec + dist * ray.dir);
            Vector nextOrgin = new Vector(ray.orgin + dist * ray.dir);
            //Vector nextNormal = (nextOrgin - this.pos).Normalize();
            Vector nextNormal = (dist * ray.dir + (ray.orgin - pos)).Normalize();
            nextOrgin = nextOrgin + nextNormal * 0.01f;
            return SurfaceFunc.Reflect(m_mat, nextOrgin, ray.dir, nextNormal);
        }

        public override Vector CalculateSurfaceNormal()
        {
            return new Vector((m_dist * m_lastRay.dir + (m_lastRay.orgin - pos)).Normalize());
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

        static public Vector Reflect(Vector dir, Vector normal)
        {
            Vector bounce = dir - 2.0f * (dir.DotProduct(normal) * normal);

            return bounce;
        }

        static public Ray Reflect(Material mat, Vector nextOrgin ,Vector dir, Vector nextNormal)
        {

            Vector pureBounce = Reflect(dir, nextNormal);
            //this is a shit way todo this.
            //TODO this is a shit way todo this. so make it better

            Vector rv = new Vector(Helpers.Rand.RandomBilateral(),
                                    Helpers.Rand.RandomBilateral(),
                                    Helpers.Rand.RandomBilateral()
                                    );

            Vector RandomBounce = (nextNormal + rv).Normalize();
            nextNormal = RandomBounce.Lerp(pureBounce, mat.reflect).Normalize();
            //nextNormal = pureBounce;
            return new Ray(nextOrgin, nextNormal);
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

        public override bool Intersect(Ray r, ref float dist)
        {
           
            Vector norm = SurfaceFunc.CalculateSurfaceNormal(m_pos);
            norm.m_y = -norm.m_y;
            float facing = norm.DotProduct(r.orgin);
            if (facing <= Config.Tolerance && facing >= Config.MinTolerance)
            {
                //normal = norm;
                //closest = quad;
                dist = (float)SurfaceFunc.Vec3Distance(r.orgin, norm);
                if(dist < Config.MinHitDistance)
                {
                    return false;
                }
                return true;
            }
            return false;            
        }

        public override Ray Reflect(Vector vec, Vector normal, float dist, Ray ray)
        {
            //Bounce off the surface then just fuck off
            return new Ray(SurfaceFunc.Reflect(vec, normal)); ;
        }

        public override Vector CalculateSurfaceNormal()
        {
            return SurfaceFunc.CalculateSurfaceNormal(m_pos);
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

