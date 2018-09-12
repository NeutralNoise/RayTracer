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
            if (c < Config.Ray.MaxBounce)
            {
                Surface closest = null;
                Vector normal = new Vector();
                float dist = 0;
                float closestDist = 0;
                foreach (Surface quad in quads)
                {

                    if (quad.Intersect(r, ref dist))
                    {
                        if (closest == null)
                        {
                            if (dist > 0)
                            {
                                closestDist = dist;
                                closest = quad;
                            }
                        }
                        else
                        {
                            if (dist > 0 && dist < closestDist)
                            {
                                closestDist = dist;
                                closest = quad;
                            }
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
                            return new ColourRGBA(result, 255);
                        }

                        //TODO: This is shit bro.
                        closest.m_dist = closestDist;
                        closest.m_lastRay = r;

                        normal = closest.CalculateSurfaceNormal();

                        //This is just some error check for when im adding things to the tracer.
                        if (normal == null)
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

                        Vector cosine = new Vector(1, 1, 1);
                        float coef = 1.0f;
                        Ray lsr = null;
                        //for each light we need to check if there is anything in the way the new ray orgin and the light.
                        //check to see if this position has a visable light.
                        foreach (Light ls in lights)
                        {
                            bool lightBlocker = false;
                            //check if anything is in the way
                            foreach (Surface s in quads)
                            {
                                float ld = 0;
                                Vector sl = (ls.pos - ray.orgin).Normalize();
                                lsr = new Ray(ray.orgin, sl);
                                if (s.Intersect(lsr, ref ld))
                                {
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
                                    float lambert = 0;
                                    if (lsr != null)
                                    {
                                        lambert = lsr.dir.DotProduct(closest.CalculateSurfaceNormal()) * coef;
                                    }
                                    else
                                    {
                                        //TODO Error
                                    }
                                    //Lambert disfuseion
                                    result.m_x += lambert * ls.intesity.r * closest.GetMat().disfuse.r;
                                    result.m_y += lambert * ls.intesity.g * closest.GetMat().disfuse.g;
                                    result.m_z += lambert * ls.intesity.b * closest.GetMat().disfuse.b;
                                }
                            }
                            coef *= closest.GetMat().reflect;

                            if(coef < 0.0f)
                            {
                                break;
                            }

                        }

                        ray.Trace(ray, quads, lights, c + 1, ref att, ref result, ref hitLight);
                        return new ColourRGBA(result, 255);
                    }
                }
                else
                {
                    return new ColourRGBA(result, 255);
                }
            }
            return null;
        }
    }
}
