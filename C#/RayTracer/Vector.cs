using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{

    class Ray
    {

        public Ray()
        {

        }

        public Ray(Vector o)
        {
            orgin = o;
        }

        public Ray(Vector o, Vector d)
        {
            orgin = o;
            dir = d;
        }


        public Vector orgin;
        public Vector dir;


        public ColourRGBA Trace(Ray r, Surface[] quads, Light[] lights, float c, ref Vector att, ref Vector result, ref bool hitLight)
        {
            Light clight = null; //Current light
            if (c < Config.Ray.MaxBounce)
            {
                Surface closest = null;
                Vector normal = new Vector();
                float dist = 0;
                float closestDist = 0;
                foreach (Surface quad in quads) {

                    if (quad.Intersect(r, ref dist))
                    {
                        if (closest == null)
                        {
                            if(dist > 0)
                            {
                                closestDist = dist;
                                closest = quad;
                            }
                        }
                        else
                        {
                            if(dist > 0 && dist < closestDist)
                            {
                                closestDist = dist;
                                closest = quad;
                            }
                        }
                    }
                }
                //All we did was hit a light
                foreach (Light l in lights)
                {
                    //Intersect with a light
                    //TODO light intersections
                    //Get distance from light to ray.
                    float ldist = (float)SurfaceFunc.Vec3Distance(r.orgin, l.pos);

                    //Check to see if the light is closer then the closest object.
                    if(closest != null)
                    {

                        if(closestDist > ldist)
                        {

                            //Get the normal from the light to the ray orgin.
                            Vector lNormal = (l.pos - r.orgin).Normalize();
                            //Vector lNormal = (r.orgin - l.pos).Normalize();

                            float dot = lNormal.DotProduct(r.dir.Normalize());
                            //we have hit something
                            //if(dot > 0 && dot < 0.04)
                            if (dot > 0)
                            {
                                clight = l;
                                //closest = null;
                                hitLight = true;
                            }
                        }
                    }
                    else
                    {
                        //Get the normal from the light to the ray orgin.
                        Vector lNormal = (l.pos - r.orgin).Normalize();
                        float dot = lNormal.DotProduct(r.dir.Normalize());
                        //we have hit something
                        if (dot > 0)
                        {
                            clight = l;
                            hitLight = true;
                        }
                    }
                }

                if (closest != null)
                {
                    //return closest.GetMat().GetColour();
                    {

                        //Bounce off the surface then just fuck off
                        //Ray ray = new Ray(SurfaceFunc.Reflect(r.vec, normal));
                        

                        if ((c + 1) > Config.Ray.MaxBounce)
                        {
                            float af = 0;
                            result += att * Config.Ray.SkyMaterial.GetEmitColour().ColourToVector(ref af).Normalize();
                            //result.Scale(255);
                            return new ColourRGBA(result.Scale(255), af);
                        }

                        //TODO: This is shit bro.
                        closest.m_dist = closestDist;
                        closest.m_lastRay = r;

                        normal = closest.CalculateSurfaceNormal();

                        //This is just some error check for when im adding things to the tracer.
                        if(normal == null)
                        {
                            Console.WriteLine("----------------------------------------");
                            Console.WriteLine(closest.ToString());
                            Console.WriteLine("This can't calculate the surface normal!");
                            Console.ReadKey();
                            Environment.Exit(1);
                        }

                        Ray ray = closest.Reflect(r.orgin, closest.CalculateSurfaceNormal(), closestDist, r);

                        if (ray == null)
                        {
                            Console.WriteLine("----------------------------------------");
                            Console.WriteLine(closest.ToString());
                            Console.WriteLine("This can not reflect!");
                            Console.ReadKey();
                            Environment.Exit(1);
                        }


                        ColourRGBA rc = new ColourRGBA(closest.GetMat().GetColour());
                        rc.Normalize();
                        //Vector d = closest.CalculateSurfaceNormal().Scale(ray.orgin.DotProduct(closest.CalculateSurfaceNormal()));
                        
                        Vector cosine = new Vector(1, 1, 1);

                        //for each light wee need to check if there is anything in the way the new ray orgin and the light.
                        //check to see if this position has a visable light.
                        foreach (Light ls in lights)
                        {
                            bool lightBlocker = false;
                            foreach(Surface s in quads)
                            {
                                float ld = 0;
                                Vector sl = (ls.pos - ray.orgin).Normalize();
                                Ray lsr = new Ray(ray.orgin, sl);
                                if (s.Intersect(lsr, ref ld)) {
                                    if (ld > 0.01)
                                    {
                                        lightBlocker = true;
                                        break;
                                    }
                                }
                            }
                            //check if there was a light blocker and if not add lighting to the ray point.
                            if (!lightBlocker)
                            {
                                //Get the distance to the light so we know if its hitting us.
                                float lightPointDist = (float)SurfaceFunc.Vec3Distance(ray.orgin, ls.pos);

                                if (lightPointDist < 1000)
                                {

                                    float la = 255.0f;
                                    ColourRGBA lColour = new ColourRGBA(ls.mat.GetColour());
                                    lColour.Normalize();
                                    Vector normColour = closest.GetMat().GetColour().ColourToVector(ref la) * lColour.ColourToVector(ref la);
                                    ColourRGBA nColour = new ColourRGBA(normColour, la);
                                    nColour.Normalize();
                                    normColour = nColour.ColourToVector(ref la);
                                    //int fdsfs = 9;
                                    //result += att * closest.GetMat().GetColour().ColourToVector(ref la).Scale(closest.GetMat().reflect);
                                    normColour = normColour.Scale(255);
                                    result += att * normColour.Scale(closest.GetMat().reflect);
                                    ColourRGBA tmpColour = new ColourRGBA(result, la);
                                    tmpColour.Normalize();
                                    result = tmpColour.ColourToVector(ref la);
                                    //result += att * normColour.Scale(1.0f);
                                    normColour = nColour.ColourToVector(ref la);
                                    //Vector sl = (ray.orgin - ls.pos).Normalize();
                                    Vector lightNormal = (ray.orgin - ls.pos).Normalize();
                                    Vector d = lightNormal.Scale(ray.orgin.DotProduct(lightNormal));
                                    //Vector cosine = (d.Scale(2) - ray.orgin).Normalize();
                                    att = att * (cosine * normColour).Normalize();
                                }
                            }
                            else
                            {
                                float bds = 255;
                                att = att * (1 * rc.ColourToVector(ref bds));//cosin * rc.ColourToVector(ref bds));
                                /*
                                if(att.m_x < 0)
                                {
                                    att.m_x = 0;
                                }
                                if (att.m_y < 0)
                                {
                                    att.m_y = 0;
                                }
                                if (att.m_z < 0)
                                {
                                    att.m_z = 0;
                                }
                                */
                                //return Config.Ray.Nothing;
                            }

                        }

                        float a = 255; //this isn't used
                        float b = 0;
                        //result += att * closest.GetMat().GetColour().ColourToVector(ref a).Scale(closest.GetMat().reflect);
                        //ColourRGBA rc = new ColourRGBA(closest.GetMat().GetColour());
                        //rc.Normalize();

                        
                        //att = att * rc.ColourToVector(ref a);

                        //Vector d = closest.CalculateSurfaceNormal().Scale(ray.orgin.DotProduct(closest.CalculateSurfaceNormal()));
                        //Vector pureBounce = SurfaceFunc.Reflect(ray.dir, closest.CalculateSurfaceNormal());
                        //float ba = r.dir.DotProduct(r.dir - ray.orgin);
                        //float ba = r.dir.DotProduct(r.dir);
                        //TODO: Cosin
                        //TODO: A better cosin
                        //Vector cosine = (d.Scale(2) - ray.orgin);
                        //Vector cosine = new Vector(1, 1, 1);
                        //float cosine = (float)Math.Cos(ba);
                        //float cosine = closest.CalculateSurfaceNormal().DotProduct(r.dir);

                        //att = att * (cosine * rc.ColourToVector(ref b));

                        //att = att * (ray.dir.DotProduct(closest.CalculateSurfaceNormal()) * rc.ColourToVector(ref a)).Normalize();
                        //att = att * (ray.dir.DotProduct(closest.CalculateSurfaceNormal()) * rc.ColourToVector(ref a).Normalize());

                        ray.Trace(ray, quads, lights, c + 1, ref att, ref result, ref hitLight);
                        return new ColourRGBA(result, a);
                    }
                }
                /*
                else if(clight != null)
                {
                    if ((c + 1) > Config.Ray.MaxBounce)
                    {
                        float af = 0;
                        ColourRGBA testCo = new ColourRGBA(0.0f, 255.0f, 0.0f, 255.0f);
                        //result += att * Config.Ray.SkyMaterial.GetEmitColour().ColourToVector(ref af).Normalize();
                        //result.Scale(255);
                        return new ColourRGBA(result.Scale(255), af);
                    }
                    float a = 255;
                    result += att * clight.mat.GetEmitColour().ColourToVector(ref a).Normalize();
 
                    //result += att * clight.mat.GetColour().ColourToVector(ref a).Normalize();
                    //result.Scale(255);
                    //return new ColourRGBA(result.Scale(255), a);
                    return new ColourRGBA(result, a);
                }*/
                else
                {
                    //float a = 0;
                    //The sky is not emitting a colour is has a colour tho
                    //result += att * Config.Ray.SkyMaterial.GetEmitColour().ColourToVector(ref a).Normalize();
                    //result += att * Config.Ray.SkyMaterial.GetColour().ColourToVector(ref a).Normalize();
                    //result += att * Config.Ray.Nothing.ColourToVector(ref a).Normalize();
                    return new ColourRGBA(result, 255);
                }
            }
            
            //return new ColourRGBA(255, 255, 255, 255);
            //return new ColourRGBA(0, 0, 0, 255);
            return null;
        }
    }

    class Vector
    {

        public Vector()
        {
            m_x = 0;
            m_y = 0;
            m_z = 0;
        }

        public Vector(float x, float y, float z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }

        public Vector(Vector vec)
        {
            m_x = vec.m_x;
            m_y = vec.m_y;
            m_z = vec.m_z;
        }

        // 90 = 0
        // acute angle = +1
        // obtruse angle = +1

        public float DotProduct(Vector vec)
        {
            return (m_x * vec.m_x) + (m_y * vec.m_y) +  (m_z * vec.m_z);
        }

        public Vector Scale(float t)
        {
            Vector s = this * t;
            return new Vector(s);
        }

        public float Scaler(Vector vec)
        {
            return DotProduct(vec);
        }

        public Vector CrossProduct(Vector vec)
        {
        //I do believe this is messed up.
        /*
        float x = m_y * vec.m_x - m_z * vec.m_y;
        float y = m_z * vec.m_x - m_x * vec.m_z;
        float z = m_x * vec.m_y - m_y * vec.m_x;
        */
        
            //github.com/tmcw/literate-raytracer/blob/gh-pages/vector.js
            float x = (m_y * vec.m_z) - (m_z * vec.m_y);
            float y = (m_z * vec.m_x) - (m_x * vec.m_z);
            float z = (m_x * vec.m_y) - (m_y * vec.m_x);
            
            return new Vector(x,y,z);
        }


        public float Magnitude()
        {
            return (float)Math.Sqrt(m_x * m_x + m_y * m_y + m_z * m_z);
        }

        public void PrintVector()
        {
            Console.Write("X:" + m_x.ToString() + " Y:" + m_y.ToString() + " Z:" + m_z.ToString() + "\n");
        }
        //Makes the Magnutude 1
        public Vector Normalize()
        {
            Vector Test = this;
            float inverse = 1/Magnitude();
            if (Magnitude() == 0)
            {
                inverse = 1 / 1;
            }
            
            Test.m_x *= inverse;
            Test.m_y *= inverse;
            Test.m_z *= inverse;

            return Test;
        }

        public static Vector operator+ (Vector a, Vector b)
        {
            return new Vector(a.m_x + b.m_x, a.m_y + b.m_y, a.m_z + b.m_z);
        }

        public static Vector operator +(Vector a, float s)
        {
            return new Vector(a.m_x + s, a.m_y + s, a.m_z + s);
        }

        public static Vector operator +(float s, Vector a)
        {
            return new Vector(a.m_x + s, a.m_y + s, a.m_z + s);
        }


        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.m_x - b.m_x, a.m_y - b.m_y, a.m_z - b.m_z);
        }

        public static Vector operator -(Vector a, float s)
        {
            return new Vector(a.m_x - s, a.m_y - s, a.m_z - s);
        }

        public static Vector operator -(float s, Vector a)
        {
            return new Vector(a.m_x - s, a.m_y - s, a.m_z - s);
        }

        public static Vector operator *(Vector a, Vector b)
        {
            return new Vector(a.m_x * b.m_x, a.m_y * b.m_y, a.m_z * b.m_z);
        }
        
        public static Vector operator *(Vector a, float s)
        {
            return new Vector(a.m_x * s, a.m_y * s, a.m_z * s);
        }

        public static Vector operator *(float s, Vector a)
        {
            return new Vector(a.m_x * s, a.m_y * s, a.m_z * s);
        }

        public static Vector operator /(Vector a, Vector b)
        {
            return new Vector(a.m_x / b.m_x, a.m_y / b.m_y, a.m_z / b.m_z);
        }

        public static Vector operator /(Vector a, float s)
        {
            return new Vector(a.m_x / s, a.m_y / s, a.m_z / s);
        }

        public static Vector operator /(float s, Vector a)
        {
            return new Vector(a.m_x / s, a.m_y / s, a.m_z / s);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            if(a != b)
            {
                return false;
            }
            return true;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            if((object)a != null && (object)b != null)
            {
                if (a.m_x != b.m_x && a.m_y != b.m_y && a.m_z != b.m_z)
                {
                    return true;
                }
            }
            else if ((object)a != null || (object)b != null)
            {
                return true;
            }
            
            return false;
        }

        public override bool Equals(object obj)
        {
            var vec = obj as Vector;
            return vec != null &&
                   m_x == vec.m_x &&
                   m_y == vec.m_y &&
                   m_z == vec.m_z;
        }

        public override int GetHashCode()
        {
            var hashCode = -889620072;
            hashCode = hashCode * -1521134295 + m_x.GetHashCode();
            hashCode = hashCode * -1521134295 + m_y.GetHashCode();
            hashCode = hashCode * -1521134295 + m_z.GetHashCode();
            return hashCode;
        }

        public float m_x;
        public float m_y;
        public float m_z;

        public Vector Lerp(Vector b, float t)
            //(float v0, float v1, float t)
        {
            float x = (1 - t) * m_x + t * b.m_x;
            float y = (1 - t) * m_y + t * b.m_y;
            float z = (1 - t) * m_z + t * b.m_z;

            return new Vector(x, y, z);
            //return (1 - t) * v0 + t * v1;
        }

        public Vector Negitve()
        {
            return new Vector(-m_x, -m_y, -m_z);
        }

    }
}
