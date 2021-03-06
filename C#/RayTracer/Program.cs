﻿using System;
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

        static private int Width = 3840;
        static private int Height = 2160;
        //static private int Width = 1280;
        //static private int Height = 720;
        //static private int Width = 300;
        //static private int Height = 300;
        //static private int Width = 10;
        //static private int Height = 10;

        //static private UInt64 rpp = int.MaxValue / (3840*2160); // Ray Per Pixel
        static private UInt64 rpp = 16;
        //static private UInt64 rpp = (UInt64)(int.MaxValue / (Width * Height)); // Ray Per Pixel

        static ColourRGBA whiteLight = new ColourRGBA(1.0f, 1.0f, 1.0f, 1.0f);
        static ColourRGBA greenLight = new ColourRGBA(255 / 2, 255, 255 / 2, 255);
        static ColourRGBA gray = new ColourRGBA(255 / 255 / 2, 255, 255 / 2, 255);

        static Vector lightPos = new Vector(5, -9, 5);

        static Light mainLight = new Light(lightPos, new Vector());

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
            mat.disfuse = new ColourRGBA(255.0f, 125.0f, 136.0f, 255.0f);
            //mat.disfuse = new ColourRGBA(1.0f, 1.0f, 1.0f, 1.0f);
            mat.disfuse.Normalize();
            mat.reflect = 0.1f;
            worldPlane.SetMat(mat);

            SurfaceSphere worldSphere = new SurfaceSphere();
            Material smat = new Material();
            smat.disfuse = new ColourRGBA(0.0f, 0.0f, 1.0f, 1.0f);
            smat.reflect = 0.9f;
            worldSphere.SetMat(smat);

            SurfaceSphere worldSphere2 = new SurfaceSphere();
            worldSphere2.pos.m_x = -2.0f;
            worldSphere2.pos.m_y = -2.0f;
            worldSphere2.pos.m_z = 2.0f;

            SurfaceSphere worldSphere3 = new SurfaceSphere();
            worldSphere3.pos.m_x = 2.0f;
            worldSphere3.pos.m_y = -2.0f;
            worldSphere3.pos.m_z = 2.0f;

            SurfaceSphere worldSphere4 = new SurfaceSphere();
            worldSphere4.pos.m_x = 0.0f;
            worldSphere4.pos.m_y = 2.0f;
            worldSphere4.pos.m_z = 2.0f;

            Material smat2 = new Material();
            smat2.disfuse = new ColourRGBA(0.5f, 1.0f, 0.5f, 1.0f);
            //smat2.reflect = 0.5f;

            Material smat3 = new Material();
            smat3.disfuse = new ColourRGBA(0.5f, 0.5f, 0.5f, 1.0f);
            

            worldSphere2.SetMat(smat2);
            worldSphere3.SetMat(smat2);
            worldSphere4.SetMat(smat3);

            Surface[] Walls = new Surface[] { worldPlane, worldSphere, worldSphere2 , worldSphere3 };

            Console.WriteLine("Casting Rays");
            Console.WriteLine("Casting: " + ((UInt64)(Width * Height) * rpp).ToString() + " rays!");

            //Image to save
            Bitmap img = new Bitmap(Width, Height);
            Bitmap highestHits = new Bitmap(Width, Height);

            //Cast the ray
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
            DateTime startTime = DateTime.Now;
            Console.Write("Render Started: ");
            Console.WriteLine(startTime.ToString());
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

                float halfPixW = 0.5f / Width;
                float halfPixH = 0.5f / Height;

                float testRays = RayColourContrib * rpp;
                Ray ray = new Ray();
                //Start casting rays
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        //Work out where we are casting this ray from.
                        float filmX = -1.0f + 2.0f * ((float)x / (float)Width); ;
                        float filmY = -1.0f + 2.0f * ((float)y / (float)Height);

                        //Vector result = new Vector(0.0f, 0.0f, 0.0f);

                        float a = 1.0f;
                        Vector rayColour = new Vector();
                        
                        //build up our colour
                        for (UInt64 r = 0; r < rpp; r++)
                        {
                            //Jittering AA
                            float offX = filmX + Helpers.Rand.RandomBilateral() * halfPixW;
                            float offY = filmY + Helpers.Rand.RandomBilateral() * halfPixH;
                            Vector filmP = FilmCenter + offX * halffilmW * camera.m_rotation.CameraX + offY * halffilmH * camera.m_rotation.CameraY;

                            ray.orgin = camera.GetPositon();
                            ray.dir = (filmP - camera.GetPositon());

                            float coef = 1.0f;
                            Vector result = new Vector(0.0f, 0.0f, 0.0f);
                            bool hitLight = false;
                            //rayColour += RayColourContrib * (ray.Trace(ray, Walls, lights, 0, ref coef, ref result, ref hitLight).ColourToVector(ref a));
                            ray.Trace(ray, Walls, lights, 0, ref coef, ref result, ref hitLight);
                            rayColour += RayColourContrib * result;
                            rayCount++;
                        }
                        //ColourRGBA colour = ray.Trace(ray, Walls, 0, ref att, ref result);
                        //rayColour.Normalize
                        ColourRGBA colour = new ColourRGBA(rayColour, a);
                        if(!looped)
                        {
                            float p = (float)rayCount / (float)((UInt64)(Width * Height) * rpp);
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
                            /*
                            colour.r = Lin2srgb(colour.r);
                            colour.g = Lin2srgb(colour.g);
                            colour.b = Lin2srgb(colour.b);
                            */
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
            DateTime endTime = DateTime.Now;
            Console.WriteLine("Saved Image");
            Console.Write("Image render completed in(h.m.s):");
            Console.WriteLine((endTime - startTime).ToString());
            Console.ReadKey();

        }
    }
}
