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

        //static private int Width = 3840;
        //static private int Height = 2160;
        //static private int Width = 1280;
        //static private int Height = 720;
        static private int Width = 300;
        static private int Height = 300;


        static private int rpp = 1; // Ray Per Pixel

        static void Main(string[] args)
        {
            Camera camera = new Camera();
            camera.SetPositon(new Vector(0.00f, -10.0f, 1.00f));
            camera.SetRotation();
            
            if (camera.GetFov() != 45.0f)
            {
                camera.SetFov(45.0f);
            }

            SurfacePlane worldPlane = new SurfacePlane();
            Material mat = new Material();
            mat.SetColour(255.0f, 0.0f, 0.0f, 255.0f);
            worldPlane.SetMat(mat);

            SurfaceSphere worldSphere = new SurfaceSphere();
            Material smat = new Material();
            smat.SetColour(0.0f, 0.0f, 255.0f, 255.0f);
            worldSphere.SetMat(smat);

            SurfaceSphere worldSphere2 = new SurfaceSphere();
            worldSphere2.pos.m_x = 2.0f;
            
            Material smat2 = new Material();
            smat2.SetColour(0.0f, 255.0f, 0.0f, 255.0f);
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

                //Start casting rays
                for (int r = 0; r < rpp; r++)
                {
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

                            ColourRGBA colour = ray.Trace(ray, Walls, 0);
                            rayCount++;

                            if(!looped)
                            {
                                float p = (float)rayCount / (float)((Width * Height) * rpp);
                                Console.Write(((int)(p * 100)).ToString() + " perenct complete\r");
                            }
                            //Nothing intersects with this ray so black
                            if(colour == null)
                            {
                                colour = new ColourRGBA(0.0f, 0.0f, 0.0f, 255.0f);
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
                        worldSphere.pos.PrintVector();
                        Console.Write("\n");
                    }
                    else
                    {
                        Console.Write("Count: " + count.ToString() + " Hits: " + thc.ToString() + "_y_" + camera.GetPositon().m_y.ToString() + "\n");
                    }
                    img.Save("hit/hit_" + thc.ToString() + "_c_" + count.ToString() + "_y_" + camera.GetPositon().m_y.ToString() + ".bmp");
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
