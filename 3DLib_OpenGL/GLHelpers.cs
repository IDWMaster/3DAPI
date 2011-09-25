using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using OpenTK;

namespace _3DUI_Windows
{
	
    class GLHelpers
    {
		
        #region Geometric primitives
        /// <summary>
        /// [OBSOLETE] This function has been deprecated
        /// </summary>
        /// <param name="Center"></param>
        /// <param name="Radius"></param>
        /// <param name="Precision"></param>
        public static void DrawSphere(Vector3 Center, float Radius, uint Precision)
        {
            if (Radius < 0f)
                Radius = -Radius;
            if (Radius == 0f)
                throw new DivideByZeroException("DrawSphere: Radius cannot be 0f.");
            if (Precision == 0)
                throw new DivideByZeroException("DrawSphere: Precision of 8 or greater is required.");

            const float HalfPI = (float)(Math.PI * 0.5);
            float OneThroughPrecision = 1.0f / Precision;
            float TwoPIThroughPrecision = (float)(Math.PI * 2.0 * OneThroughPrecision);

            float theta1, theta2, theta3;
            Vector3 Normal, Position;

            for (uint j = 0; j < Precision / 2; j++)
            {
                theta1 = (j * TwoPIThroughPrecision) - HalfPI;
                theta2 = ((j + 1) * TwoPIThroughPrecision) - HalfPI;

                GL.Begin(BeginMode.TriangleStrip);
                for (uint i = 0; i <= Precision; i++)
                {
                    theta3 = i * TwoPIThroughPrecision;

                    Normal.X = (float)(Math.Cos(theta2) * Math.Cos(theta3));
                    Normal.Y = (float)Math.Sin(theta2);
                    Normal.Z = (float)(Math.Cos(theta2) * Math.Sin(theta3));
                    Position.X = Center.X + Radius * Normal.X;
                    Position.Y = Center.Y + Radius * Normal.Y;
                    Position.Z = Center.Z + Radius * Normal.Z;

                    GL.Normal3(Normal);
                    GL.TexCoord2(i * OneThroughPrecision, 2.0f * (j + 1) * OneThroughPrecision);
                    GL.Vertex3(Position);

                    Normal.X = (float)(Math.Cos(theta1) * Math.Cos(theta3));
                    Normal.Y = (float)Math.Sin(theta1);
                    Normal.Z = (float)(Math.Cos(theta1) * Math.Sin(theta3));
                    Position.X = Center.X + Radius * Normal.X;
                    Position.Y = Center.Y + Radius * Normal.Y;
                    Position.Z = Center.Z + Radius * Normal.Z;

                    GL.Normal3(Normal);
                    GL.TexCoord2(i * OneThroughPrecision, 2.0f * j * OneThroughPrecision);
                    GL.Vertex3(Position);
                }
                GL.End();
            }
        }
        /// <summary>
        /// [DEPRECATED] This function has been deprecated
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="position"></param>
        public static void DrawCube(float width, float height, float depth, Vector3 position)
        {
          
            Vector3 objPos = position;
            
            GL.Begin(BeginMode.Quads);
            //Front face of cube
            
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3((0f - width) + objPos.X, (0f - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3((1f + width) + objPos.X, (0f - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3((1f + width) + objPos.X, (1f + height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3((0f - width) + objPos.X, (1f + height) + objPos.Y, (0 - depth) + objPos.Z);
           
            //Back face of cube
            
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3((0f - width) + objPos.X, 0f - height, 1 + depth);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3((1f + width) + objPos.X, (0f - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3((1f + width) + objPos.X, (1f + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3((0f - width) + objPos.X, (1f + height) + objPos.Y, (1 + depth) + objPos.Z);
            
            //Left face of cube
            
            GL.TexCoord2(0, 1); GL.Vertex3((0 - width) + objPos.X, (0 - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 1); GL.Vertex3((0 - width) + objPos.X, (0 - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(1, 0); GL.Vertex3((0 - width) + objPos.X, (1 + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0, 0); GL.Vertex3((0 - width) + objPos.X, (1 + height) + objPos.Y, (0 - depth) + objPos.Z);
           
            //Right face of cube
            
            GL.TexCoord2(0, 1); GL.Vertex3((1 + width) + objPos.X, (0 - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 1); GL.Vertex3((1 + width) + objPos.X, (0 - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(1, 0); GL.Vertex3((1 + width) + objPos.X, (1 + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0, 0); GL.Vertex3((1 + width) + objPos.X, (1 + height) + objPos.Y, (0 - depth) + objPos.Z);
            
            //Bottom face of cube
            GL.TexCoord2(0, 1); GL.Vertex3((0 - width) + objPos.X, (0 - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 1); GL.Vertex3((1 + width) + objPos.X, (0 - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 0); GL.Vertex3((1 + width) + objPos.X, (0 - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0, 0); GL.Vertex3((0 - width) + objPos.X, (0 - height) + objPos.Y, (1 + depth) + objPos.Z);
            //Top face of cube
            
            GL.TexCoord2(0, 1); GL.Vertex3((0 - width) + objPos.X, (1 + height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 1); GL.Vertex3((1 + width) + objPos.X, (1 + height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 0); GL.Vertex3((1 + width) + objPos.X, (1 + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0, 0); GL.Vertex3((0 - width) + objPos.X, (1 + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.End();
            
        }
        /// <summary>
        /// [DEPRECATED] This function has been deprecated
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="position"></param>
        /// <param name="front"></param>
        /// <param name="back"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
		public static void DrawCube(float width, float height, float depth, Vector3 position, int front, int back, int left, int right, int bottom, int top)
        {
           
            Vector3 objPos = position;
	GL.BindTexture(TextureTarget.Texture2D,front);
            GL.Begin(BeginMode.Quads);
            //Front face of cube
           // Vector3 offset = new Vector3(-.05f, 0, 0);
           // objPos += offset;
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3((0f - width) + objPos.X, (0f - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3((1f + width) + objPos.X, (0f - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3((1f + width) + objPos.X, (1f + height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3((0f - width) + objPos.X, (1f + height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.End();
            //objPos -= offset;
			//Back face of cube
            GL.BindTexture(TextureTarget.Texture2D,back);
			  GL.Begin(BeginMode.Quads);
			GL.TexCoord2(0.0f, 1.0f); GL.Vertex3((0f - width) + objPos.X, 0f - height, 1 + depth);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3((1f + width) + objPos.X, (0f - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3((1f + width) + objPos.X, (1f + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3((0f - width) + objPos.X, (1f + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.End();
			//Left face of cube
			GL.BindTexture(TextureTarget.Texture2D,left);
              GL.Begin(BeginMode.Quads);
			GL.TexCoord2(0, 1); GL.Vertex3((0 - width) + objPos.X, (0 - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 1); GL.Vertex3((0 - width) + objPos.X, (0 - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(1, 0); GL.Vertex3((0 - width) + objPos.X, (1 + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0, 0); GL.Vertex3((0 - width) + objPos.X, (1 + height) + objPos.Y, (0 - depth) + objPos.Z);
			GL.End();
            //Right face of cube
			GL.BindTexture(TextureTarget.Texture2D,right);
			  GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 1); GL.Vertex3((1 + width) + objPos.X, (0 - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 1); GL.Vertex3((1 + width) + objPos.X, (0 - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(1, 0); GL.Vertex3((1 + width) + objPos.X, (1 + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0, 0); GL.Vertex3((1 + width) + objPos.X, (1 + height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.End();
			//Bottom face of cube
           // offset = new Vector3(0, -.05f, 0);
           // objPos += offset;
            GL.BindTexture(TextureTarget.Texture2D,bottom);
			  GL.Begin(BeginMode.Quads);
			GL.TexCoord2(0, 1); GL.Vertex3((0 - width) + objPos.X, (0 - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 1); GL.Vertex3((1 + width) + objPos.X, (0 - height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 0); GL.Vertex3((1 + width) + objPos.X, (0 - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0, 0); GL.Vertex3((0 - width) + objPos.X, (0 - height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.End();
           // objPos -= offset;
			//Top face of cube
			GL.BindTexture(TextureTarget.Texture2D,top);
              GL.Begin(BeginMode.Quads);
			GL.TexCoord2(0, 1); GL.Vertex3((0 - width) + objPos.X, (1 + height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 1); GL.Vertex3((1 + width) + objPos.X, (1 + height) + objPos.Y, (0 - depth) + objPos.Z);
            GL.TexCoord2(1, 0); GL.Vertex3((1 + width) + objPos.X, (1 + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.TexCoord2(0, 0); GL.Vertex3((0 - width) + objPos.X, (1 + height) + objPos.Y, (1 + depth) + objPos.Z);
            GL.End();



        }
        #endregion
		static Dictionary<Bitmap,BitmapData> locks = new Dictionary<Bitmap, BitmapData>();
		public static void DrawPixels(Bitmap image) {
		
		if(!locks.ContainsKey(image)) {
		BitmapData mdata = image.LockBits(new System.Drawing.Rectangle(0,0,image.Width,image.Height),ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			
			                                  GL.DrawPixels(image.Width,image.Height,OpenTK.Graphics.OpenGL.PixelFormat.Bgra,PixelType.UnsignedByte,mdata.Scan0);
		locks.Add(image,mdata);
			} else {
			BitmapData mdata = locks[image];
			
			                                  GL.DrawPixels(image.Width,image.Height,OpenTK.Graphics.OpenGL.PixelFormat.Bgra,PixelType.UnsignedByte,mdata.Scan0);
		
			}
			
		}
        public static Bitmap TakeScreenshot(int width, int height)
        {
            Bitmap mmap = new Bitmap(width, height);
            BitmapData mdat = mmap.LockBits(new Rectangle(0, 0, mmap.Width, mmap.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, mdat.Scan0);
            mmap.UnlockBits(mdat);
            mmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return mmap;
        }
        public static void ReplaceTexture(Bitmap image, int texture)
        {
            
            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

           GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        }
        public static void ReplaceTexture(IntPtr image, int texture, int width, int height)
        {

            GL.BindTexture(TextureTarget.Texture2D, texture);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, image);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        }
        public static void TexConfig()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        }
       public static int LoadTexture(Bitmap image)
        {
            int texture = GL.GenTexture();
			
            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			try {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			}catch(Exception) {
			
			}
            image.UnlockBits(data);
       
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
   
			
           return texture;
        }
    }
}
