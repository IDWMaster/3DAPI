using System;
using System.Collections.Generic;
using _3DAPI;
using SlimDX;
using SlimDX.Windows;
using SlimDX.Direct3D9;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
namespace DirectXLib
{
    #region Input
    class DMouse : Mouse
    {

     
        SlimDX.DirectInput.Mouse umouse;
        public bool isactive = true;
        public DMouse(SlimDX.DirectInput.Mouse mouse)
        {
            umouse = mouse;
            mouse.Acquire();
            System.Threading.Thread mthread = new System.Threading.Thread(thetar);
            mthread.Start();
        }
        SlimDX.DirectInput.MouseState prevstate;
        bool[] prevbtns;
        void thetar()
        {
            while (isactive)
            {
                try
                {
                    SlimDX.DirectInput.MouseState currentstate = umouse.GetCurrentState();
                    if (prevstate == null)
                    {
                        prevstate = new SlimDX.DirectInput.MouseState();
                        prevbtns = new bool[currentstate.GetButtons().Length];
                    }
                    bool[] currentbtns = currentstate.GetButtons();
                    for (int i = 0; i < currentbtns.Length; i++)
                    {
                        if (prevbtns[i] != currentbtns[i])
                        {
                            if (currentbtns[i])
                            {
                                ntfyMouseDown((MouseButton)i, currentstate.X, currentstate.Y);
                            }
                            else
                            {
                                ntfyMouseUp((MouseButton)i, currentstate.X, currentstate.Y);
                            }
                        }
                    }
                    if (prevstate.X != currentstate.X || prevstate.Y != currentstate.Y)
                    {
                        ntfyMouseMove(MouseButton.None, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
                    }
                    prevbtns = currentbtns;
                    System.Threading.Thread.Sleep(1);
                }
                catch (Exception er)
                {
                
                }
            }
        }
    }
    class DKeyboard : Keyboard
    {

      
        SlimDX.DirectInput.Keyboard mboard;
        public bool isactive = true;
        public DKeyboard(SlimDX.DirectInput.Keyboard underlyingKeyboard)
        {
            
            underlyingKeyboard.Acquire();
            mboard = underlyingKeyboard;
            System.Threading.Thread mthread = new System.Threading.Thread(thetar);
            mthread.Start();
        }
        SlimDX.DirectInput.KeyboardState laststate = null;
        void thetar()
        {
            while (isactive)
            {
                try
                {
                    SlimDX.DirectInput.KeyboardState currentstate = mboard.GetCurrentState();
                    if (laststate == null)
                    {
                        laststate = new SlimDX.DirectInput.KeyboardState();
                    }
                    foreach (SlimDX.DirectInput.Key et in currentstate.PressedKeys)
                    {
                        if (!laststate.PressedKeys.Contains(et))
                        {
                            ntfyKeyDown(et.ToString());
                        }
                    }
                    foreach (SlimDX.DirectInput.Key et in currentstate.ReleasedKeys)
                    {
                        if (!laststate.ReleasedKeys.Contains(et))
                        {
                            ntfyKeyUp(et.ToString());
                        }
                    }
                    laststate = currentstate;
                    System.Threading.Thread.Sleep(1);
                }
                catch (Exception er)
                {
                
                }
            }
        }
    }
    #endregion
    struct Vertex
    {
        public Vector3 position;
        public Vector3 Normal;
        public Vector2 texcoord;
        public Vertex(Vector3D Position, Vector3D normal, Vector2D Texture)
        {
            position = new Vector3(Position.X,Position.Y,Position.Z);
            Normal = new Vector3(normal.X,normal.Y,normal.Z);
            texcoord = new Vector2(Texture.X,Texture.Y);

        }
        public static int getSize()
        {
            int possize = sizeof(float) * 3;
            int normsize = sizeof(float) * 3;
            int texsize = sizeof(float) * 2;
            return possize + normsize + texsize;
        }
    }
    class DXTexture : Texture2D
    {

        protected override void uploadbitmap(Bitmap tmap)
        {
            IntPtr hDC = internsure.GetSurfaceLevel(0).GetDC();
            Graphics mfix = Graphics.FromHdc(hDC);
            mfix.DrawImage(tmap, new Rectangle(0, 0, Width, Height));
            mfix.Dispose();
            //TODO: Is this necessary?
            internsure.GetSurfaceLevel(0).ReleaseDC(hDC);
        }
        public DXTexture(DirectEngine ngine, int width, int height):base(width,height)
        {
            engine = ngine;
            _width = width;
            _height = height;
        }
        public DXTexture(DirectEngine ngine, Bitmap bitmap):base(bitmap.Width,bitmap.Height)
        {
            engine = ngine;
            this.bitmap = bitmap;
            
        }
        int _width;
        int _height;
       Bitmap bitmap;
        DirectEngine engine;
       internal Texture internsure;
        protected override void LoadTex()
        {
            if (bitmap != null)
            {
                
                MemoryStream mstream = new MemoryStream();
                //This should be the fastest?
                bitmap.Save(mstream, System.Drawing.Imaging.ImageFormat.Bmp);
                mstream.Position = 0;
                internsure = Texture.FromStream(engine.graphicsDevice, mstream);
                Console.WriteLine(internsure.GetSurfaceLevel(0).Description.Format);
                
            }
            else
            {
                try
                {
                    internsure = new Texture(engine.graphicsDevice, _width, _height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

                    
                }
                catch (Direct3D9Exception er)
                {
                    Console.WriteLine(er);
                }
            }
        }

        protected override void DrawTex()
        {
            engine.texture = internsure;
            engine.graphicsDevice.SetTexture(0, internsure);
            engine.graphicsDevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
            engine.graphicsDevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
            engine.graphicsDevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
            
            engine.graphicsDevice.SetRenderState(RenderState.CullMode, Cull.None);
            engine.graphicsDevice.SetRenderState(RenderState.ZEnable, true);
           
        }
    }
    class BasicShader:Shader
    {
        public BasicShader(DirectEngine engine)
        {
            StreamReader mreader = new StreamReader("basicdxshader.txt");
            shader = mreader.ReadToEnd();
            this.engine = engine;
            
        }
        string shader;
        DirectEngine engine;
        internal void doRender()
        {
            ApplyShader();
        }
        protected override void ApplyShader()
        {
            Effect mfect = Effect.FromString(engine.graphicsDevice, shader, ShaderFlags.None);
            engine.shader = this;
            engine.effect = mfect;
        }
    }
    class DXVertBuffer : _3DAPI.VertexBuffer
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
        bool hasloaded = false;
        protected override void _Dispose()
        {
            internbuffer.Dispose();
        }
        protected override void RenderBuffer()
        {
            if (!hasloaded)
            {
                
                internbuffer = new SlimDX.Direct3D9.VertexBuffer(engine.graphicsDevice, Vertex.getSize() * verts.Length, Usage.None, VertexFormat.None, Pool.Managed);
                List<Vertex> mtex = new List<Vertex>();
                for (int i = 0; i < verts.Length; i++)
                {
                    mtex.Add(new Vertex(verts[i], norms[i], texas[i]));
                }
                DataStream mstr = internbuffer.Lock(0, 0, LockFlags.None);
                mstr.WriteRange(mtex.ToArray());
                internbuffer.Unlock();
                List<VertexElement> elements = new List<VertexElement>();
                
                elements.Add(new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0));
                elements.Add(new VertexElement(0, sizeof(float)*3, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0));
                elements.Add(new VertexElement(0, sizeof(float)*3+sizeof(float)*3, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0));
                elements.Add(VertexElement.VertexDeclarationEnd);
              
                    declaration = new VertexDeclaration(engine.graphicsDevice, elements.ToArray());
                
                hasloaded = true;
                
            }
           
              
                engine.graphicsDevice.VertexDeclaration = declaration;
            
                engine.graphicsDevice.SetStreamSource(0, internbuffer, 0, Vertex.getSize());
           
            engine.effect.Technique = engine.effect.GetTechnique("technique0");
            EffectHandle lightdirection = engine.effect.GetParameter(null, "LightDirection");
            EffectHandle lightenabled = engine.effect.GetParameter(null, "LightEnabled");
            engine.effect.SetValue<bool>(lightenabled, engine.LightingEnabled);
            engine.effect.SetValue<Vector3>(lightdirection, new Vector3(0, 1.4f, -2f));
            EffectHandle texhandle = engine.effect.GetParameter(null, "shaderTexture");
            engine.effect.SetTexture(texhandle, engine.texture);
           
           // Matrix mtrix = Matrix.Identity*Matrix.Translation(new Vector3(2,.5f,-9.4f))*Matrix.RotationY(rotation);
            Matrix worldrotation;
            bool mtr = false;
            if (!engine.rtarget)
            {
                worldrotation = Matrix.Transformation(new Vector3(), Quaternion.Identity, new Vector3(1, 1, 1), new Vector3(0, 0, 0), Quaternion.RotationMatrix(Matrix.RotationAxis(Vector3.UnitY,MathHelpers.DegreesToRadians(engine.worldRotation.Y))*Matrix.RotationAxis(Vector3.UnitX,MathHelpers.DegreesToRadians(engine.worldRotation.X))*Matrix.RotationAxis(Vector3.UnitZ, MathHelpers.DegreesToRadians(engine.worldRotation.Z))), new Vector3(0, 0, 0));//Matrix.RotationX(engine.worldRotation.X) * Matrix.RotationY(engine.worldRotation.Y) * Matrix.RotationZ(engine.worldRotation.Z);
            }
            else
            {
               
                    worldrotation = Matrix.Transformation(new Vector3(), Quaternion.Identity, new Vector3(1, 1, 1), new Vector3(0, 0, 0), Quaternion.RotationMatrix(Matrix.RotationAxis(Vector3.UnitY,MathHelpers.DegreesToRadians(RenderTargetChange.mchange.camrot.Y))*Matrix.RotationAxis(Vector3.UnitX, MathHelpers.DegreesToRadians(RenderTargetChange.mchange.camrot.X))*Matrix.RotationAxis(Vector3.UnitZ, MathHelpers.DegreesToRadians(RenderTargetChange.mchange.camrot.Z))), new Vector3(0, 0, 0));//Matrix.RotationX(engine.worldRotation.X) * Matrix.RotationY(engine.worldRotation.Y) * Matrix.RotationZ(engine.worldRotation.Z);
                    mtr = true;
                
            }
         
                Matrix worldmatrix = Matrix.LookAtLH(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            Matrix projectionmatrix = worldrotation*Matrix.PerspectiveFovLH(MathHelpers.DegreesToRadians(45), (float)engine.graphicsDevice.Viewport.Width / (float)engine.graphicsDevice.Viewport.Height, 1, 5000000);
            Matrix viewmatrix = Matrix.RotationX(rotation.X) * Matrix.RotationY(rotation.Y) * Matrix.RotationZ(rotation.Z);
             Matrix mtrix;
            if (!sta)
            {
                if (mtr)
                {
                    mtrix = viewmatrix * worldmatrix * Matrix.Translation(new Vector3(Position.X - RenderTargetChange.mchange.campos.X, Position.Y - RenderTargetChange.mchange.campos.Y, Position.Z - RenderTargetChange.mchange.campos.Z)) * projectionmatrix;
                }
                else
                {
                    mtrix = viewmatrix * worldmatrix * Matrix.Translation(new Vector3(Position.X - engine.cameraPosition.X, Position.Y - engine.cameraPosition.Y, Position.Z - engine.cameraPosition.Z)) * projectionmatrix;
                }
                }
            else
            {
                
                mtrix = viewmatrix * worldmatrix * Matrix.Translation(new Vector3(Position.X, Position.Y, Position.Z)) * projectionmatrix;
            }
            EffectHandle mathandle = engine.effect.GetParameter(null,"WorldViewProj");
            engine.effect.SetValue<Matrix>(mathandle, mtrix);
            if (!depthtest)
            {
                engine.graphicsDevice.SetRenderState(RenderState.ZEnable, ZBufferType.DontUseZBuffer);
            }
            engine.effect.Begin();
            engine.effect.BeginPass(0);
            if (engine.PMode == PrimitiveMode.TriangleList)
            {
                engine.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, verts.Length / 3);
            }
            else
            {
                engine.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, verts.Length);
            }
            engine.effect.EndPass();
            engine.effect.End();
            if (!depthtest)
            {
                engine.graphicsDevice.SetRenderState(RenderState.ZEnable, ZBufferType.UseZBuffer);
            }
        }
        VertexDeclaration declaration;
        Vector3D[] verts;
        Vector3D[] norms;
        Vector2D[] texas;
        public DXVertBuffer(DirectEngine mgine,Vector3D[] vertices,Vector3D[] normals, Vector2D[] texcoords)
        {
            engine = mgine;
            verts = vertices;
            norms = normals;
            texas = texcoords;
        }
        DirectEngine engine;
        SlimDX.Direct3D9.VertexBuffer internbuffer;
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
    class RenderTargetChange
    {
        internal static RenderTargetChange mchange;
        public RenderTargetChange(DXTexture target, DirectEngine engine, Vector3D _campos, Vector3D _camrot)
        {
            _engine = engine;
            interntarget = target;
            if (mchange != null)
            {
                hasran = true;
                previous = mchange.previous;
            }
            mchange = this;
            
            prevpos = _engine.cameraPosition;
            prevrot = _engine.worldRotation;
            campos = _campos;
            camrot = _camrot;

        }
        internal Vector3D campos;
        internal Vector3D camrot;
        Vector3D prevpos;
        Vector3D prevrot;
        DirectEngine _engine;
        DXTexture interntarget;
       internal bool hasran = false;
        Surface previous;
        
        public void Draw()
        {
            if (!hasran)
            {
                hasran = true;
                previous = _engine.graphicsDevice.GetRenderTarget(0);
               
                try
                {
                    _engine.graphicsDevice.SetRenderTarget(0, interntarget.internsure.GetSurfaceLevel(0));
                }
                catch (Exception er)
                {
                    Console.WriteLine(er);
                }
              
               
                
                _engine.rtarget = true;       
                   
                
            }else {
                //Reset render target
                _engine.graphicsDevice.SetRenderTarget(0, previous);
                
                
                hasran = false;
                //mchange = null;
                _engine.rtarget = false;
                
                
            }
        }

    }
    public class DirectEngine:Renderer
    {
        internal PrimitiveMode PMode
        {
            get
            {
                return _imode;
            }
        }
        public override void SetRenderTarget(Texture2D texture, Vector3D campos, Vector3D camrot)
        {
            
            RenderTargetChange mchange = new RenderTargetChange(texture as DXTexture, this,campos,camrot);
        
        }
        public override void Dispose()
        {
            mboard.isactive = false;
            mmouse.isactive = false;
            needsclose = true;
        }
        bool needsclose = false;
        internal BasicShader shader;
        Exception inception;
        PresentParameters presentation;
        void thetar()
        {
            try
            {
                internform = new RenderForm("DirectX - SlimDX library");
                Direct3D my3d = new Direct3D();
                presentation = new PresentParameters();
                presentation.BackBufferHeight = my3d.Adapters[0].CurrentDisplayMode.Height;
                presentation.BackBufferWidth = my3d.Adapters[0].CurrentDisplayMode.Width;
                presentation.DeviceWindowHandle = internform.Handle;
                presentation.Windowed = false;
                int adapter = 0;
                foreach (AdapterInformation et in my3d.Adapters)
                {
                    if (et.Details.Description.ToLower().Contains("perfhud"))
                    {
                        adapter = et.Adapter;
                    }
                }
                if (adapter == 0)
                {
                    
                    
                        graphicsDevice = new Device(my3d, 0, DeviceType.Hardware, internform.Handle, CreateFlags.HardwareVertexProcessing, presentation);
                    
                }
                else
                {
                    //presentation.Windowed = true;
                    //presentation.BackBufferHeight = internform.Height;
                    //presentation.BackBufferWidth = internform.Width;
                    graphicsDevice = new Device(my3d, adapter, DeviceType.Reference, internform.Handle, CreateFlags.HardwareVertexProcessing, presentation);
                }
               
                SlimDX.DirectInput.DirectInput input = new SlimDX.DirectInput.DirectInput();
                SlimDX.DirectInput.Keyboard keyboard = new SlimDX.DirectInput.Keyboard(input);
                keyboard.SetCooperativeLevel(internform.Handle, SlimDX.DirectInput.CooperativeLevel.Foreground | SlimDX.DirectInput.CooperativeLevel.Exclusive);
                mboard = new DKeyboard(keyboard);
                SlimDX.DirectInput.Mouse mouse = new SlimDX.DirectInput.Mouse(input);
                mouse.SetCooperativeLevel(internform.Handle, SlimDX.DirectInput.CooperativeLevel.Foreground | SlimDX.DirectInput.CooperativeLevel.Nonexclusive);
                mmouse = new DMouse(mouse);

                tvent.Set();
                MessagePump.Run(internform, messenger);
            }
            catch (Exception er)
            {
                
                inception = er;
                tvent.Set();
            }
        }
        public override Texture2D createTextureFromBitmap(Bitmap bitmap)
        {
            return new DXTexture(this, bitmap);
        }
        DKeyboard mboard;
        DMouse mmouse;
        System.Threading.ManualResetEvent tvent = new System.Threading.ManualResetEvent(false);
        public override Mouse defaultMouse
        {
            get { return mmouse; }
        }
        public override Touchpad defaultTouchpad
        {
            get { throw new NotImplementedException(); }
        }
        public override Keyboard defaultKeyboard
        {
            get { return mboard; }
        }
        
        public DirectEngine()
        {
            System.Threading.Thread mthread = new System.Threading.Thread(thetar);
            mthread.SetApartmentState(System.Threading.ApartmentState.STA);
            mthread.Start();
            tvent.WaitOne();
            if (inception != null)
            {
                throw inception;
            }
        }
        int rendered = 0;
        void messenger()
        {
            if (!lostdevice)
            {
              
                    if (RenderTargetChange.mchange != null)
                    {
                        RenderTargetChange.mchange.Draw();
                    }
                    
                        graphicsDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Blue, 1, 0);
                    
                    
                    graphicsDevice.BeginScene();
                    Draw();
                    graphicsDevice.EndScene();
                    try
                    {
                        if (!rtarget)
                        {
                            graphicsDevice.Present();
                        }
                    }
                    catch (Exception)
                    {
                        lostdevice = true;
                    }
                
            }
            else
            {
                if (graphicsDevice.TestCooperativeLevel().Code != ResultCode.DeviceLost.Code)
                {
                    try
                    {
                        graphicsDevice.Reset(presentation);
                        shader.doRender();
                        lostdevice = false;
                        
                    }
                    catch (Exception)
                    {
                    
                    }
                }
            }

            if (needsclose)
            {
                internform.Close();
                
            }
        }
        bool lostdevice = false;
        internal Texture texture;
        internal Device graphicsDevice;
        internal Form internform;
        internal Effect effect;
        public override _3DAPI.VertexBuffer CreateVertexBuffer(Vector3D[] vertices, Vector2D[] texcoords, Vector3D[] normals)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3D(-vertices[i].X, vertices[i].Y, vertices[i].Z);
            }
            return new DXVertBuffer(this, vertices, normals, texcoords);
        }

        public override Texture2D createTexture(int Width, int Height)
        {
            return new DXTexture(this,Width, Height);
        }
        internal bool rtarget = false;
        public override Shader createBasicShader()
        {
            return new BasicShader(this);
        }
    }
}