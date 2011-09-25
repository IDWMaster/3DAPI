using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Text;
using _3DAPI;
using System.Threading;
namespace _3DLib_OpenGL
{
    class GlVertexBuffer : VertexBuffer
    {
        public GlVertexBuffer()
        {
            
        
        }
        protected override void RenderBuffer()
        {
            throw new NotImplementedException();
        }

        public override Vector3D[] VertexArray
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
	class GLWindow:GameWindow {
	public GLWindow(GLRenderer renderer) {
		Title = "OpenGL application";
			WindowState = WindowState.Fullscreen;
		renderclass = renderer;
		}
		public int programID;
		protected override void OnLoad (EventArgs e)
		{
			
			base.OnLoad (e);
			programID = GL.CreateProgram();
			renderclass.eventcontrol.Set();
		}
		GLRenderer renderclass;
		protected override void OnRenderFrame (FrameEventArgs e)
		{
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            renderclass.ntfyReady();
			
			SwapBuffers();
			base.OnRenderFrame (e);
		}
	}
	public class BasicShader:Shader {
		#region implemented abstract members of _3DAPI.Shader
		protected override void ApplyShader ()
		{
			
			vertindex = GL.CreateShader(ShaderType.VertexShader);
				
				fragindex = GL.CreateShader(ShaderType.FragmentShader);	
			
				GL.ShaderSource(vertindex,vertsrc);
				GL.CompileShader(vertindex);
			Console.WriteLine(GL.GetShaderInfoLog(vertindex));
				GL.ShaderSource(fragindex,fragsrc);
			GL.CompileShader(fragindex);
			Console.WriteLine(GL.GetShaderInfoLog(fragindex));
			GL.Arb.AttachObject(prog,vertindex);
			GL.Arb.AttachObject(prog,fragindex);
			
			GL.Arb.LinkProgram(prog);
			
			GL.Arb.UseProgramObject(prog);
			Console.WriteLine("Shaders active");
			
		}
		int vertindex = -1;
		int fragindex = -1;
		
		/// <summary>
		///Creates a basic shader. 
		/// </summary>
		string vertsrc;
		string fragsrc;
		int prog;
		public BasicShader(int program) {
		StringBuilder mbuilder = new StringBuilder();
			mbuilder.AppendLine("varying vec2 texture_coordinate;");
			mbuilder.AppendLine("void main() {");
			mbuilder.AppendLine("gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;");
			mbuilder.AppendLine("}");
			vertsrc = mbuilder.ToString();
			mbuilder = new StringBuilder();
			mbuilder.AppendLine("varying vec2 texture_coordinate;\nuniform sampler2D my_color_texture;\nvoid main() {\ngl_FragColor = texture2D(my_color_texture,texture_coordinate);\n}");
			fragsrc = mbuilder.ToString();
			prog = program;
			
		}	
		#endregion
	
	}
    public class GLRenderer:Renderer
    {
		internal void ntfyReady() {
		Draw();
		}
		GLWindow mwind;
		public GLRenderer() {
		System.Threading.Thread mthread = new System.Threading.Thread(thetar);
			mthread.Start();
			eventcontrol.WaitOne();
		}
		internal ManualResetEvent eventcontrol = new ManualResetEvent(false);
		void thetar() {
		mwind = new GLWindow(this);
			mwind.Run();
		}
		
		public override Shader createBasicShader ()
		{
			
			return new BasicShader(mwind.programID);
		}
		public override Texture2D createTexture (int Width, int Height)
		{
			throw new NotImplementedException ();
		}
        public override VertexBuffer CreateVertexBuffer()
        {
            GlVertexBuffer mbuff = new GlVertexBuffer();
			return mbuff;
        }

    }
}
