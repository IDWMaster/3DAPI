using System;
using System.Collections.Generic;
using _3DAPI;
using _3DLib_OpenGL;
using DirectXLib;
namespace GlTestApp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			DirectEngine renderer = new DirectEngine();
            Texture2D mtex = renderer.createTexture(512, 512);
            mtex.Draw();
			Console.WriteLine("Renderer is active");
            //Shader myder = renderer.createBasicShader();
	
            //myder.Draw();
		
            //List<Vector3D> points = new List<Vector3D>();
            //Vector3D top = new Vector3D();
            //top.X = 0;
            //top.Y = 0;
            //top.Z = 0;
            //Vector3D bottom = new Vector3D();
            //bottom.X = 1;
            //bottom.Y = 0;
            //bottom.Z = 0;
            //Vector3D right = new Vector3D();
            //right.X = 1;
            //right.Y = 1;
            //right.Z = 0;
            //points.Add(top);
            //points.Add(bottom);
            //points.Add(right);
            //List<Vector2D> texcoords = new List<Vector2D>();
            //texcoords.Add(new Vector2D() { X = 0, Y = 0});
            //texcoords.Add(new Vector2D() { X = 1, Y = 0});
            //texcoords.Add(new Vector2D() { X = 1, Y = 1});
            ////VertexBuffer mbuffer = renderer.CreateVertexBuffer(points.ToArray(),texcoords.ToArray(),points.ToArray());
            ////mbuffer.Draw();
            //Primitives.createRectangle(renderer,0,-.5f,0,1,1).Draw();
		}
	}
}

