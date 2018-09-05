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


        public ColourRGBA Trace(Ray r, Surface[] quads, float c, ref Vector att, ref Vector result)
        {

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
                    {
                        /*
                        Vector norm = SurfaceFunc.CalculateSurfaceNormal(quad.m_pos);
                        norm.m_y = -norm.m_y;
                        facing = norm.DotProduct(r.vec);
                        if (closest == null)
                        {
                            if (facing <= Config.Tolerance && facing >= Config.MinTolerance)
                            {
                                normal = norm;
                                closest = quad;
                                dist = (float)SurfaceFunc.Vec3Distance(r.vec, norm);
                            }
                        }
                        else
                        {
                            if (facing <= Config.Tolerance && facing >= Config.MinTolerance)
                            {
                                if ((float)SurfaceFunc.Vec3Distance(r.vec, norm) < dist)
                                {
                                    dist = (float)SurfaceFunc.Vec3Distance(r.vec, norm);
                                    closest = quad;
                                }
                            }
                        }
                        */
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
                            result.Scale(255);
                            return new ColourRGBA(result.Scale(255), af);
                        }

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

                        ColourRGBA resultColour;
                        float a = 0; //this isn't used
                        
                        result += att * closest.GetMat().GetEmitColour().ColourToVector(ref a);
                        ColourRGBA rc = new ColourRGBA(closest.GetMat().GetColour());
                        rc.Normalize();
                        att = att * rc.ColourToVector(ref a);
                        ray.Trace(ray, quads, c + 1, ref att, ref result);
                        resultColour = new ColourRGBA(result, a);
                        /*
                        if (bounceColour == null) {
                            //we have hit the sky
                            bounceColour = Config.Ray.SkyColour;
                        }
                        
                        //ColourRGBA mat = new ColourRGBA(0, 0, 255, 255);
                        ColourRGBA mat = closest.GetMat().GetColour();

                        //Convert the colour to a Vec 3
                        Vector vc = new Vector(mat.r, mat.g, mat.b);
                        float trans = mat.a;

                        Vector vc2 = new Vector(bounceColour.r, bounceColour.g, bounceColour.b);
                        trans = bounceColour.a + trans;

                        //Clamp the transperancy
                        if(trans > 255)
                        {
                            trans = 255;
                        }
                        else if (trans < 0)
                        {
                            trans = 0;
                        }
                        
                        Vector cnorm = (vc + vc2).Normalize();

                        cnorm  = cnorm.Scale(255.0f);

                        bounceColour = new ColourRGBA(cnorm.m_x, cnorm.m_y, cnorm.m_z, trans);
                        */
                        //return bounceColour;
                        return resultColour;
                    }
                }
                else
                {
                    float a = 0;
                    result += att * Config.Ray.SkyMaterial.GetEmitColour().ColourToVector(ref a).Normalize();
                    result.Scale(255);
                    return new ColourRGBA(result.Scale(255), a);
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

    }
}
