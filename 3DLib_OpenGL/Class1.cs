using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Text;
using _3DAPI;
using System.Threading;
using _3DUI_Windows;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
namespace _3DLib_OpenGL
{
    #region Input
    class GLKeyboard : Keyboard
    {
   
        OpenTK.Input.KeyboardDevice mdev;
        public GLKeyboard(OpenTK.Input.KeyboardDevice keydev)
        {
            mdev = keydev;
            mdev.KeyDown += new EventHandler<OpenTK.Input.KeyboardKeyEventArgs>(mdev_KeyDown);
            mdev.KeyUp += new EventHandler<OpenTK.Input.KeyboardKeyEventArgs>(mdev_KeyUp);

        }

        void mdev_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            ntfyKeyUp(e.Key.ToString());

        }


        void mdev_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            ntfyKeyDown(e.Key.ToString());

        }
    }
    #endregion
    class GLTexture : Texture2D
    {
        public GLTexture(int width, int height)
            : base(width, height)
        {


            underlyingbitmap = new Bitmap(width, height);

            mset.Set();
        
        }
        public GLTexture(Bitmap mmap)
            : base(mmap.Width, mmap.Height)
        {
            underlyingbitmap = mmap;
            mset.Set();
        }
        ManualResetEvent mset = new ManualResetEvent(false);
        Bitmap underlyingbitmap;
        protected override void LoadTex()
        {

            mset.WaitOne();
            if (underlyingbitmap != null)
            {
                texid = GLHelpers.LoadTexture(underlyingbitmap);
            }
            else
            {
                texid = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texid);
                GLHelpers.TexConfig();
            }

        }
       internal int texid = -1;
        protected override void DrawTex()
        {
            GL.BindTexture(TextureTarget.Texture2D, texid);
        }
    }
    class GlVertexNotSupportedBuffer : VertexBuffer
    {
        bool depthtest = true;
        public override bool DepthTesting
        {
            get
            {
                return depthtest;
            }
            set
            {
                depthtest = value;
            }
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
        public GlVertexNotSupportedBuffer(Vector3D[] vertices, Vector2D[] texcoords, Vector3D[] normals, GLRenderer nder)
        {
            verts = vertices;
            texc = texcoords;
            norms = normals;
            renderer = nder;
        }
        Vector3D[] verts;
        Vector2D[] texc;
        Vector3D[] norms;
        GLRenderer renderer;
        protected override void RenderBuffer()
        {
            if (displaylist == -1)
            {
                displaylist = GL.GenLists(1);
                GL.NewList(displaylist, ListMode.Compile);
                GL.Begin(BeginMode.Triangles);
                for (int i = 0; i < verts.Length; i++)
                {



                    GL.TexCoord2(texc[i].X, texc[i].Y);
                    GL.Normal3(norms[i].X, norms[i].Y, norms[i].Z);
                    GL.Vertex3(verts[i].X, verts[i].Y, verts[i].Z);
                }
                GL.End();
                GL.EndList();
            }
            if (!sta)
            {
				if(!renderer.mt) {
                Matrix4 mat = Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.LookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0)) * Matrix4.CreateTranslation(Position.X - renderer.cameraPosition.X, Position.Y + renderer.cameraPosition.Y, -Position.Z + renderer.cameraPosition.Z)*Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-renderer.worldRotation.Y))*Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-renderer.worldRotation.Z))*Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-renderer.worldRotation.X)) * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)renderer.Width / (float)renderer.Height, 1, 5000);
				GL.LoadMatrix(ref mat);
				}else {
				Matrix4 mat = Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.LookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0)) * Matrix4.CreateTranslation(Position.X - renderer.spos.X, Position.Y + renderer.spos.Y, -Position.Z + renderer.spos.Z)*Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-renderer.srot.Y))*Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-renderer.srot.Z))*Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-renderer.srot.X)) * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)renderer.Width / (float)renderer.Height, 1, 5000);
				GL.LoadMatrix(ref mat);
				}
					
            }
            else
            {
                Matrix4 mat = Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.LookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0)) * Matrix4.CreateTranslation(Position.X, Position.Y, -Position.Z) * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)renderer.Width / (float)renderer.Height, 1, 5000);
              
                GL.LoadMatrix(ref mat);
            }
            if (!depthtest)
            {
                GL.Disable(EnableCap.DepthTest);
            }
            GL.CallList(displaylist);
            GL.Enable(EnableCap.DepthTest);
        }
        bool sta = false;
        public override bool IsStatic
        {
            get
            {
                return sta;
            }
            set
            {
                sta = value;
            }
        }
        int displaylist = -1;
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
    class GlVertexBuffer : VertexBuffer
    {
        bool depthtest = true;
        public override bool DepthTesting
        {
            get
            {
                return depthtest;
            }
            set
            {
                depthtest = value;
            }
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        
        }
        internal bool mt = false;
        
        GLRenderer renderer;
        public GlVertexBuffer(Vector3D[] vertices, Vector2D[] _texcoords, Vector3D[] _normals, GLRenderer _rend)
        {
            renderer = _rend;
            verts = vertices;
            texcoords = _texcoords;
            normals = _normals;
        }
        Vector3D[] verts;
        Vector2D[] texcoords;
        Vector3D[] normals;
        bool hasrendered = false;
        int[] bufferIDs;
        float cx = 0;
        bool sta = false;
        public override bool IsStatic
        {
            get
            {
                return sta;
            }
            set
            {
                sta = value;
            }
        }
        protected override void RenderBuffer()
        {

            if (!hasrendered)
            {
                
                hasrendered = true;
                try
                {
                    int[] buffers = new int[3];

                    GL.GenBuffers(3, buffers);
                    //Vertex buffer
                    GL.BindBuffer(BufferTarget.ArrayBuffer, buffers[0]);

                    GL.BufferData<Vector3D>(BufferTarget.ArrayBuffer, (IntPtr)((sizeof(float) * 3) * verts.Length), verts, BufferUsageHint.StaticDraw);
                    //Tex buffer
                    GL.BindBuffer(BufferTarget.ArrayBuffer, buffers[1]);
                    GL.BufferData<Vector2D>(BufferTarget.ArrayBuffer, (IntPtr)((sizeof(float) * 2) * texcoords.Length), texcoords, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, buffers[2]);
                    GL.BufferData<Vector3D>(BufferTarget.ArrayBuffer, (IntPtr)((sizeof(float) * 3) * normals.Length), normals, BufferUsageHint.StaticDraw);
                    bufferIDs = buffers;
                }
                catch (Exception er)
                {
                    Console.WriteLine(er);
                }

            }
            GL.Enable(EnableCap.DepthTest);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIDs[0]);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIDs[1]);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIDs[2]);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
            //Matrix4 mat = Matrix4.LookAt(new Vector3(0, 0, 20), new Vector3(0, 0, 0), new Vector3(0, 1, 0)) * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)renderer.Width / (float)renderer.Height, 1, 5000) * Matrix4.CreateTranslation(new Vector3(renderer.cameraPosition.X + Position.X, renderer.cameraPosition.Y + Position.Y, renderer.cameraPosition.Z + Position.Z)) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationZ(rotation.Z);
            cx -= .01f;
            Matrix4 mat;
            if (!sta)
            {
                if (!renderer.mt)
                {
                    mat = Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.CreateRotationX(rotation.X) * Matrix4.LookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0)) * Matrix4.CreateTranslation(Position.X - renderer.cameraPosition.X, Position.Y + renderer.cameraPosition.Y, -Position.Z + renderer.cameraPosition.Z) * Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-renderer.worldRotation.Y)) * Matrix4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(-renderer.worldRotation.Z)) * Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-renderer.worldRotation.X)) * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)renderer.Width / (float)renderer.Height, 1, 5000);
                }
                else
                {
                    mat = Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.LookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0)) * Matrix4.CreateTranslation(Position.X - renderer.spos.X, Position.Y + renderer.spos.Y, -Position.Z + renderer.spos.Z) * Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-renderer.srot.Y)) * Matrix4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(-renderer.srot.Z)) * Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-renderer.srot.X)) * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)renderer.Width / (float)renderer.Height, 1, 5000);
                 }
                }
            else
            {
                mat = Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.LookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0)) * Matrix4.CreateTranslation(Position.X, Position.Y, -Position.Z) * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)renderer.Width / (float)renderer.Height, 1, 5000);
            }
            if (BasicShader.shaderrunning)
            {
                int id = GL.GetUniformLocation(renderer.programID, "mvp");

                GL.UniformMatrix4(id, true, ref mat);

            }
            else
            {
                if (!BasicShader.supportsShaders)
                {
                    
                    Console.WriteLine("ILLEGAL OP - No shader support");
                    GL.LoadTransposeMatrix(ref mat);
                }
            }
            //DEPRECATED. DISABLE AFTER TESTING

            //GL.Begin(BeginMode.Quads);

            //GL.Vertex3(0f, 0f, 0f); GL.TexCoord2(0, 0);
            //GL.Vertex3(1f, 0f, 0f); GL.TexCoord2(1, 0);
            //GL.Vertex3(1f, 1f, 0f); GL.TexCoord2(1, 1);
            //GL.Vertex3(0f, 1f, 0f); GL.TexCoord2(0, 1);
            //GL.End();
            try
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                if (!depthtest)
                {
                    GL.Disable(EnableCap.DepthTest);
                }
                
                GL.DrawArrays(BeginMode.Triangles, 0, verts.Length);
                
                if (!depthtest)
                {
                    GL.Enable(EnableCap.DepthTest);
                }
                GL.DisableVertexAttribArray(0);
                GL.DisableVertexAttribArray(1);
            }
            catch (Exception er)
            {
                Console.WriteLine(er);
            }

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
    class GLWindow : GameWindow
    {
        public GLWindow(GLRenderer renderer)
        {
            Title = "OpenGL application";
            WindowState = WindowState.Fullscreen;
            renderclass = renderer;
        }
        public int programID;
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            programID = GL.CreateProgram();
            renderclass.programID = programID;
            renderclass.eventcontrol.Set();

            GL.Enable(EnableCap.Texture2D);

            //GL.Enable(EnableCap.VertexArray);
            GL.Enable(EnableCap.DepthTest);
            //  GL.Enable(EnableCap.NormalArray);
            string extensions = GL.GetString(StringName.Extensions);
            if(extensions.ToLower().Contains("framebuffer")) {
                
                GLHelpers.TexConfig();
                GL.Ext.GenFramebuffers(1, framebuffers);
                GL.Ext.GenRenderbuffers(1, renderbuffers);
            }
            else
            {
                FBOsupported = false;
               
                Console.WriteLine("WARNING: FBOs are not supported by this GPU");
            }

        }
       
        bool FBOsupported = true;
        GLRenderer renderclass;
        int[] framebuffers = new int[1];
        int[] renderbuffers = new int[1];
        bool renderbuffer = false;
		int unframecount = 30;
		void processbitmap(object sender) {
			object[] inbound = sender as object[];
			Bitmap tmap = inbound[0] as Bitmap;
			GLTexture texid = inbound[1] as GLTexture;
			
			Bitmap mmap = new Bitmap(renderclass.rendertexture.Width, renderclass.rendertexture.Height);
                    Graphics mfix = Graphics.FromImage(mmap);
                    mfix.DrawImage(tmap, new Rectangle(0, 0, mmap.Width, mmap.Height));
                    mfix.Dispose();
			inbound[0] = mmap;
			completedbitmaps.Add(inbound);
		}
		List<object[]> completedbitmaps = new List<object[]>();
        protected override void OnRenderFrame(FrameEventArgs e)
        {
           
            if (renderclass.rendertexture != null)
            {
                if (renderbuffer)
                {
                    //Bind FBO
                    if (FBOsupported)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, 0);

                        GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffers[0]);
                        GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, renderclass.rendertexture.texid, 0);
                        GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, renderbuffers[0]);
                        GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.DepthComponent24, renderclass.rendertexture.Width, renderclass.rendertexture.Height);
                        GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, renderbuffers[0]);
                    }
                }
                else
                {
                    if (FBOsupported)
                    {
                        GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
                        GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, 0);
                    }
                }
            }
          
                GL.ClearColor(Color.Blue);
                GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            

                if (!renderbuffer)
                {
                    renderclass.Width = Width;
                    renderclass.Height = Height;
                }
                else
                {
                    renderclass.Width = renderclass.rendertexture.Width;
                    renderclass.Height = renderclass.rendertexture.Height;
                }
                #region Debug
                //renderclass.spos = renderclass.cameraPosition;
                #endregion
                //if (FBOsupported)
               // {
                    GL.Viewport(0, 0, renderclass.Width, renderclass.Height);
               // }
                renderclass.ntfyReady();
            
            if (!renderbuffer)
            {
                try
                {

                    SwapBuffers();
                }
                catch (Exception er)
                {
                    Console.WriteLine(er);
                }
            }
            else
            {
                if (!FBOsupported & renderclass.rendertexture.texid !=0)
                {
                    GL.BindTexture(TextureTarget.Texture2D,renderclass.rendertexture.texid);
					GL.CopyTexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgb,0,0,renderclass.rendertexture.Width,renderclass.rendertexture.Height,0);
					
					
					unframecount = 0;
					
                }
            }
			if (renderclass.rendertexture != null)
            {
                if (renderbuffer)
                {
                    renderclass.mt = false;
                    renderbuffer = false;
                  
                }
                else
                {
                    renderbuffer = true;
                    renderclass.mt = true;

                    

                }
            }
            base.OnRenderFrame(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            renderclass.Width = Width;
            renderclass.Height = Height;
            base.OnResize(e);
        }
    }

    public class BasicShader : Shader
    {
        public static bool supportsShaders = true;
        #region implemented abstract members of _3DAPI.Shader
        bool isactive = false;
        internal static bool shaderrunning = false;
        protected override void ApplyShader()
        {


            vertindex = GL.CreateShader(ShaderType.VertexShader);

            fragindex = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertindex, vertsrc);
            GL.ShaderSource(fragindex, fragsrc);
            GL.CompileShader(vertindex);
            Console.WriteLine(GL.GetShaderInfoLog(vertindex));

            GL.CompileShader(fragindex);
            Console.WriteLine(GL.GetShaderInfoLog(fragindex));
            GL.AttachShader(prog, vertindex);
            GL.AttachShader(prog, fragindex);
            GL.BindAttribLocation(prog, 0, "in_vertex");
            GL.BindAttribLocation(prog, 1, "in_texcoord");
            GL.BindAttribLocation(prog, 2, "in_normal");
            try
            {
                GL.LinkProgram(prog);
            }
            catch (Exception er)
            {
                Console.WriteLine(er);

                supportsShaders = false;
                Console.WriteLine("WARNING: This GPU only supports the fixed-function pipeline. Shaders cannot be used");
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light0);
                GL.Light(LightName.Light0, LightParameter.Diffuse, new Vector4(1f, 1f, 1f, 1f));
                GL.Light(LightName.Light0, LightParameter.Position, new Vector4(0, -5, 0f, 1));

                return;
            }
            try
            {
                if (supportsShaders)
                {
                    Console.WriteLine(prog);

                    GL.UseProgram(prog);
                    shaderrunning = true;
                }
            }
            catch (Exception er)
            {
                Console.WriteLine(er);
                Console.WriteLine(GL.GetProgramInfoLog(prog));
            }
            Console.WriteLine(GL.GetProgramInfoLog(prog));
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
        public BasicShader(int program)
        {
            StringBuilder mbuilder = new StringBuilder();
            mbuilder.AppendLine("attribute vec3 in_normal;");
            mbuilder.AppendLine("varying vec3 normal;");
            mbuilder.AppendLine("varying vec3 lightdirection;");
            mbuilder.AppendLine("varying vec2 texture_coordinate;\nattribute vec3 in_vertex;\nattribute vec2 in_texcoord;");
            mbuilder.AppendLine("uniform mat4 mvp;");
            mbuilder.AppendLine("void main() {");
            
            mbuilder.AppendLine("gl_Position = vec4(in_vertex.x,in_vertex.y,in_vertex.z,1.0)*mvp;");
            mbuilder.AppendLine("texture_coordinate = in_texcoord;");
            mbuilder.AppendLine("normal = in_normal;");
            mbuilder.AppendLine("lightdirection = normalize(vec3(0,-5,100));");
            
            mbuilder.AppendLine("}");
            vertsrc = mbuilder.ToString();
            mbuilder = new StringBuilder();
            mbuilder.AppendLine("varying vec3 normal;");
            mbuilder.AppendLine("varying vec3 lightdirection;");
            mbuilder.AppendLine("varying vec2 texture_coordinate;\nuniform sampler2D my_color_texture;\nvoid main() {\ngl_FragColor = texture2D(my_color_texture,texture_coordinate)*clamp(dot(lightdirection,normal),0.0,1.0); \n}");
            fragsrc = mbuilder.ToString();
            prog = program;

        }
        #endregion

    }
    class GLMouse : Mouse
    {
        public GLMouse(OpenTK.Input.MouseDevice idev)
        {
            realmouse = idev;
            realmouse.Move += new EventHandler<OpenTK.Input.MouseMoveEventArgs>(realmouse_Move);
        realmouse.ButtonDown+= HandleRealmouseButtonDown;
			realmouse.ButtonUp+= HandleRealmouseButtonUp;
		}

        void HandleRealmouseButtonUp (object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
        	ntfyMouseUp((MouseButton)e.Button,e.X,e.Y);
        }

        void HandleRealmouseButtonDown (object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
        	ntfyMouseDown((MouseButton)e.Button,e.X,e.Y);
        }

        void realmouse_Move(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            ntfyMouseMove(MouseButton.None, System.Windows.Forms.Cursor.Position.X,System.Windows.Forms.Cursor.Position.Y);
        }
        OpenTK.Input.MouseDevice realmouse;
    }
    public class GLRenderer : Renderer
    {
        internal bool mt = false;
       internal GLTexture rendertexture;
       internal Vector3D spos;
       internal Vector3D srot;
        public override void SetRenderTarget(Texture2D texture, Vector3D cpos, Vector3D crot)
        {
            spos = cpos;
            srot = crot;
            rendertexture = texture as GLTexture;
            
        }
        public override Texture2D createTextureFromBitmap(Bitmap bitmap)
        {
            return new GLTexture(bitmap);
        }
        public override void Dispose()
        {

            mwind.Close();
        }
        GLMouse internmouse;
        GLKeyboard internwaterboard;
        public override Keyboard defaultKeyboard
        {
            get { return internwaterboard; }
        }
        public override Mouse defaultMouse
        {
            get { return internmouse; }
        }
        public override Touchpad defaultTouchpad
        {
            get { throw new NotImplementedException(); }
        }
        internal int Width;
        internal int Height;
        internal int programID;
        internal void ntfyReady()
        {
            Draw();
        }
        GLWindow mwind;
        public GLRenderer()
        {
            System.Threading.Thread mthread = new System.Threading.Thread(thetar);
            mthread.Start();
            eventcontrol.WaitOne();
            internwaterboard = new GLKeyboard(mwind.Keyboard);
            internmouse = new GLMouse(mwind.Mouse);
        }
        internal ManualResetEvent eventcontrol = new ManualResetEvent(false);
        void thetar()
        {
            mwind = new GLWindow(this);
            mwind.Run();
        }

        public override Shader createBasicShader()
        {

            return new BasicShader(mwind.programID);
        }
        public override Texture2D createTexture(int Width, int Height)
        {
            return new GLTexture(Width, Height);
        }
        public override VertexBuffer CreateVertexBuffer(Vector3D[] vertices, Vector2D[] texcoords, Vector3D[] normals)
        {
            if (BasicShader.supportsShaders)
            {
                Console.WriteLine("SHADER");
                GlVertexBuffer mbuff = new GlVertexBuffer(vertices, texcoords, normals, this);
                return mbuff;
            }
            else
            {
                Console.WriteLine("NSSS");
                GlVertexNotSupportedBuffer mbuff = new GlVertexNotSupportedBuffer(vertices, texcoords, normals, this);
                return mbuff;
            }
        }

    }
}
