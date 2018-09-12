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


                        ColourRGBA rc = new ColourRGBA(closest.GetMat().GetColour());
                        rc.Normalize();
                        //Vector d = closest.CalculateSurfaceNormal().Scale(ray.orgin.DotProduct(closest.CalculateSurfaceNormal()));

                        Vector cosine = new Vector(1, 1, 1);

                        //for each light wee need to check if there is anything in the way the new ray orgin and the light.
                        //check to see if this position has a visable light.
                        foreach (Light ls in lights)
                        {
                            bool lightBlocker = false;
                            foreach (Surface s in quads)
                            {
                                float ld = 0;
                                Vector sl = (ls.pos - ray.orgin).Normalize();
                                Ray lsr = new Ray(ray.orgin, sl);
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
}
