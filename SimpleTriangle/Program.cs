using System;
using System.Collections.Generic;
using System.Text;
using _3DAPI;
using _3DLib_OpenGL;
using DirectXLib;
using System.Drawing;
namespace _3DSample
{
    class Program
    {
        static Renderer renderer;
        static void Main(string[] args)
        {
            try
            {
                renderer = new DirectEngine();
            }
            catch (Exception)
            {
                renderer = new GLRenderer();
            }
            Shader mder = renderer.createBasicShader();
            mder.Draw();
            Bitmap mmap = new Bitmap("pic.jpg");
            
            Texture2D mtex = renderer.createTextureFromBitmap(mmap);
            mtex.Draw();
			Bitmap mbumpmap = new Bitmap("terrain2.bmp");
			TerrainLoader mloader = new TerrainLoader(mbumpmap,7);
            mloader.normals.Clear();
            for (int i = 0; i < mloader.vertices.Count; i++)
            {
                if (i < mloader.vertices.Count - 3)
                {
                    mloader.normals.Add(NormalComputation.ComputeFaceNormal(mloader.vertices[i], mloader.vertices[i + 1], mloader.vertices[i + 2]));
                }
                else
                {
                    mloader.normals.Add(NormalComputation.ComputeFaceNormal(mloader.vertices[i], mloader.vertices[i - 1], mloader.vertices[i - 2]));
                }
            }
			VertexBuffer mbuffer = renderer.CreateVertexBuffer(mloader.vertices.ToArray(),mloader.texcoords.ToArray(),mloader.normals.ToArray());
			renderer.SetPrimitiveMode(PrimitiveMode.TriangleStrip);
			mbuffer.Draw();
			//renderer.cameraPosition.Z = -5;
           
			renderer.defaultKeyboard.onKeyDown+= HandleRendererdefaultKeyboardonKeyDown;
			renderer.defaultKeyboard.onKeyUp+= HandleRendererdefaultKeyboardonKeyUp;
			float motionspeed = .1f;

            renderer.cameraPosition.Z = -10;
            renderer.cameraPosition.X = 5;
            renderer.cameraPosition.Y = 7;
			while (true)
            {
                //mbuffer.rotation.Y+=.01f;
				
				if(down) {
				renderer.cameraPosition.Z -=motionspeed;
					
				}
				if(up) {
				renderer.cameraPosition.Z +=motionspeed;
				}
				if(left) {
				renderer.cameraPosition.X-=motionspeed;
				}
				if(right) {
				renderer.cameraPosition.X+=motionspeed;
				}
				
                System.Threading.Thread.Sleep(10);
            }
			
            
        }

        static void HandleRendererdefaultKeyboardonKeyUp (string KeyName)
        {
        	if(KeyName.ToLower().Contains("up")) {
			up = false;
			}
			if(KeyName.ToLower().Contains("left")) {
			left = false;
			}if(KeyName.ToLower().Contains("right")) {
			right = false;
			}if(KeyName.ToLower().Contains("down")) {
			down = false;
			}
        }
		static bool down = false;
		static bool up = false;
		static bool left = false;
		static bool right = false;
        static void HandleRendererdefaultKeyboardonKeyDown (string KeyName)
        {
        	if(KeyName.ToLower().Contains("up")) {
			up = true;
			}
			if(KeyName.ToLower().Contains("left")) {
			left = true;
			}if(KeyName.ToLower().Contains("right")) {
			right = true;
			}if(KeyName.ToLower().Contains("down")) {
			down = true;
			}
        }
    }
}
