using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RayTracer
{
    class Program
    {
        //Gamma Correction
        //TODO move this
        static private float Lin2srgb(float L)
        {
            if(L < 0)
            {
                L = 0;
            }
            if(L > 1)
            {
                L = 1;
            }
            float magicsrgb = 0.0031308f;
            float magicsrgb2 = 12.92f;
            float S = 0;
            if (L <= magicsrgb)
            {
                S = L * magicsrgb2;
            }
            else
            {
                S = 1.055f * (float)Math.Pow(L, 1.0f / 2.4) - 0.055f;
            }
            return S;
        }

        //static private int Width = 3840;
        //static private int Height = 2160;
        //static private int Width = 1280;
        //static private int Height = 720;
        static private int Width = 300;
        static private int Height = 300;
        //static private int Width = 10;
        //static private int Height = 10;

        static private int rpp = 20; // Ray Per Pixel

        static ColourRGBA whiteLight = new ColourRGBA(255, 255, 255, 255);
        static ColourRGBA greenLight = new ColourRGBA(255 / 2, 255, 255 / 2, 255);
        static ColourRGBA gray = new ColourRGBA(255 / 255 / 2, 255, 255 / 2, 255);

        static Vector lightPos = new Vector(5, -9, 5);

        static Material whiteLightMat = new Material(whiteLight, whiteLight);

        static Light mainLight = new Light(lightPos, new Vector(), whiteLightMat);

        static Light[] lights = new Light[] { mainLight };

        static void Main(string[] args)
        {
            Camera camera = new Camera();
            camera.SetPositon(new Vector(0.00f, -10.0f, 1.0f));
            camera.SetRotation();
            
            if (camera.GetFov() != 45.0f)
            {
                camera.SetFov(45.0f);
            }

            SurfacePlane worldPlane = new SurfacePlane();
            Material mat = new Material();
            mat.SetColour(255.0f, 125.0f, 136.0f, 255.0f);
            //mat.SetColour(255.0f, 0.0f, 0.0f, 255.0f);
            mat.reflect = 0.1f;
            //mat.SetEmitColour(255.0f, 125.0f, 136.0f, 255.0f);
            worldPlane.SetMat(mat);

            SurfaceSphere worldSphere = new SurfaceSphere();
            Material smat = new Material();
            //smat.SetColour(220.0f, 200.0f, 255.0f, 255.0f);
            //smat.SetColour(512.0f, 0.0f, 0.0f, 255.0f);
            //smat.SetColour(255.0f, 0.0f, 0.0f, 255.0f);
            smat.SetColour(0.0f, 0.0f, 255.0f, 255.0f);
            //smat.reflect = Helpers.Rand.RandomBilateral();
            //smat.SetEmitColour(255.0f, 255.0f, 255.0f, 255.0f);
            smat.SetEmitColour(0.0f, 0.0f, 255.0f, 0.0f);
            worldSphere.SetMat(smat);

            SurfaceSphere worldSphere2 = new SurfaceSphere();
            worldSphere2.pos.m_x = -2.0f;
            worldSphere2.pos.m_y = -2.0f;
            worldSphere2.pos.m_z = 2.0f;

            Material smat2 = new Material();
            smat2.SetColour(125.0f, 255.0f, 125.0f, 255.0f);
            //smat2.SetColour(128,128,128,255);
            //smat2.reflect = Helpers.Rand.RandomBilateral();
            worldSphere2.SetMat(smat2);

            //Surface[] Walls = new Surface[] { worldPlane };
            Surface[] Walls = new Surface[] { worldPlane, worldSphere, worldSphere2 };

            Console.WriteLine("Casting Rays");
            Console.WriteLine("Casting: " + ((Width * Height) * rpp).ToString() + " rays!");

            //Image to save
            Bitmap img = new Bitmap(Width, Height);
            Bitmap highestHits = new Bitmap(Width, Height);

            //Cast the ray
            //Uncomment this if you want more rays per pixel
            bool hit = false;
            bool exit = false;

            float filmDist = 1.0f;

            //Aspect ratio
            float filmW = 1.0f;
            float filmH = 1.0f;

            if(Width > Height)
            {
                filmH = filmW * ((float)Height / (float)Width);
            }
            else if (Height > Width)
            {
                filmW = filmH * ((float)Width / (float)Height);
            }

            float halffilmW = 0.5f * filmW;
            float halffilmH = 0.5f * filmH;

            bool looped = false;

            UInt64 hc = 0;
            UInt64 count = 0;
            while (!hit && !exit)
            {
                if (!looped)
                {
                    exit = true;
                }
                else
                {

                    if (camera.GetPositon().m_y <= -100)
                    {
                        break;
                    }
                    else
                    {
                        Vector p = camera.GetPositon();
                        p.m_y -= 1;
                        camera.SetPositon(p);
                    }

                    camera.SetRotation();
                }
                Vector FilmCenter = camera.GetPositon() - filmDist * camera.m_rotation.CameraZ;

                UInt64 thc = 0;
                count++;
                Console.Write("Count: " + count.ToString() + "\r");
                UInt64 rayCount = 0;
                float RayColourContrib = 1.0f / rpp;

                float testRays = RayColourContrib * rpp;

                //Start casting rays
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        //Work out where we are casting this ray from.
                        float filmX = -1.0f + 2.0f * ((float)x / (float)Width); ;
                        float filmY = -1.0f + 2.0f * ((float)y / (float)Height);

                        Vector filmP = FilmCenter + filmX * halffilmW * camera.m_rotation.CameraX + filmY * halffilmH * camera.m_rotation.CameraY;

                        Ray ray = new Ray();
                        ray.orgin = camera.GetPositon();
                        ray.dir = (filmP - camera.GetPositon());

                        //Vector result = new Vector(0.0f, 0.0f, 0.0f);
                        
                        float a = 0;
                        Vector rayColour = new Vector();
                        //build up our colour
                        for (int r = 0; r < rpp; r++)
                        {
                            Vector att = new Vector(1.0f, 1.0f, 1.0f);
                            Vector result = new Vector(0.0f, 0.0f, 0.0f);
                            bool hitLight = false;
                            rayColour += RayColourContrib * (ray.Trace(ray, Walls, lights, 0, ref att, ref result, ref hitLight).ColourToVector(ref a));
                            rayCount++;
                        }
                        //ColourRGBA colour = ray.Trace(ray, Walls, 0, ref att, ref result);
                        //rayColour.Normalize
                        ColourRGBA colour = new ColourRGBA(rayColour, a);
                        if(!looped)
                        {
                            float p = (float)rayCount / (float)((Width * Height) * rpp);
                            Console.Write(((int)(p * 100)).ToString() + " perenct complete\r");
                        }
                        //Nothing intersects with this ray so black
                        if(colour == null)
                        {
                            //colour = new ColourRGBA(0.0f, 0.0f, 0.0f, 255.0f);
                            colour = Config.Ray.SkyColour;
                        }
                        else
                        {
                            //Correct the gamma.
                            colour.r = Lin2srgb(colour.r);
                            colour.g = Lin2srgb(colour.g);
                            colour.b = Lin2srgb(colour.b);
                            //colour.Normalize();
                            colour.Scale(255);
                            colour.Clamp();


                        }
                        //Convert our colour to a colour windows C# understands
                        Color wColour = Color.FromArgb((int)colour.a, (int)colour.r, (int)colour.g, (int)colour.b);

                        //If its not black we hit something
                        if (colour.r != 0 || colour.g != 0 || colour.b != 0)
                        {
                            thc++;
                        }

                        img.SetPixel(x, y, wColour);
                    }
                }

                if(hc < thc)
                {
                    hc = thc;
                    highestHits = img;
                }
                if(thc > 0)
                {
                    //For when we are displaying the percent complete
                    if(!looped)
                    {
                        Console.WriteLine("");
                        //Console.Write("Hits: " + thc.ToString() + "_y_" + camera.GetPositon().m_y.ToString() + "\n");
                        Console.Write("Hits: " + thc.ToString() + "\n");
                        //worldSphere.pos.PrintVector();
                        Console.Write("\n");
                    }
                    else
                    {
                        Console.Write("Count: " + count.ToString() + " Hits: " + thc.ToString() + "\n");
                    }
                    img.Save("hit/hit_" + thc.ToString() + "_c_" + count.ToString() + ".bmp");
                }

            }
            
            if(hit)
            {
                highestHits.Save("hit.bmp");
                Console.WriteLine("Hit Something");
            }
            else
            {
                img.Save("shit.bmp");
            }
            Console.WriteLine("Saved Image");
            Console.ReadKey();

        }
    }
}
