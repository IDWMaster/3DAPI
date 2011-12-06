using System;
using _3DAPI;
using System.Collections.Generic;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace WPhoneLib
{
    class WPBasicShader : _3DAPI.Shader
    {
        PhoneRenderer internder;
        public WPBasicShader(PhoneRenderer mder)
        {
            internder = mder;
        }
        BasicEffect mfect;
        bool firsttime = true;
        protected override void ApplyShader()
        {
            if (firsttime)
            {
                firsttime = false;
                mfect = new BasicEffect(internder.idevice);
                mfect.TextureEnabled = true;    
            }
            internder.shader = mfect;
        }
    }
    class WPVertexBuffer : _3DAPI.VertexBuffer
    {
        PhoneRenderer renderer;
        Vector3D[] vertices;
        Vector2D[] texcoords;
        Vector3D[] normals;
        public WPVertexBuffer(PhoneRenderer mder, Vector3D[] verts, Vector2D[] tex, Vector3D[] norms)
        {
            renderer = mder;
            vertices = verts;
            normals = norms;
            texcoords = tex;
        }
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
        bool isstatic = false;
        public override bool IsStatic
        {
            get
            {
                return isstatic;
            }
            set
            {
                isstatic = value;
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
        Microsoft.Xna.Framework.Graphics.VertexBuffer underlyingbuffer = null;
        protected override void RenderBuffer()
        {
            if (underlyingbuffer == null)
            {
                underlyingbuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(renderer.idevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
                List<VertexPositionNormalTexture> mlists = new List<VertexPositionNormalTexture>();
                for (int i = 0; i < vertices.Length; i++)
                {
                mlists.Add(new VertexPositionNormalTexture(new Vector3(vertices[i].X,vertices[i].Y,vertices[i].Z),new Vector3(normals[i].X,normals[i].Y,normals[i].Z),new Vector2(texcoords[i].X,texcoords[i].Y)));
                    
                }
                underlyingbuffer.SetData<VertexPositionNormalTexture>(mlists.ToArray());
                
            }
         renderer.shader.Texture = renderer.texture;
         renderer.shader.World = Matrix.Identity;//Matrix.CreateWorld(new Vector3(0, 0, 0), Vector3.Forward, Vector3.Up);
         renderer.shader.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, renderer.idevice.Viewport.AspectRatio, 1, 500);
            
            if (!isstatic)
            {
                renderer.shader.View = Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, 1), Vector3.Up) * Matrix.CreateTranslation(new Vector3(Position.X + renderer.cameraPosition.X, Position.Y + renderer.cameraPosition.Y, Position.Z + renderer.cameraPosition.Z)) * renderer.matrix;

            }
            else
            {
                renderer.shader.View = Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, Position.Z), new Vector3(0, 0, 0), Vector3.Up);

            }
            renderer.idevice.SetVertexBuffer(underlyingbuffer, 0);
            renderer.shader.CurrentTechnique.Passes[0].Apply();

            renderer.idevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Length / 3);
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
    class WPTexture : _3DAPI.Texture2D
    {
        public WPTexture(PhoneRenderer _renderer):base(512,512)
        {
            renderer = _renderer;
        }
        PhoneRenderer renderer;
        Microsoft.Xna.Framework.Graphics.Texture2D underlyingtexture;
        protected override void LoadTex()
        {
            underlyingtexture = renderer.manager.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("pic");
        }

        protected override void DrawTex()
        {
            renderer.texture = underlyingtexture;
        }
    }
    class XTouchpad : Touchpad
    {
        internal void ntfyTouchDown(int x, int y, int id)
        {
            ntfyTouchFound(id, x, y);

        }
        internal void ntfyTouchRelease(int x, int y, int id)
        {
            ntfyTouchRelease(x, y, id);
        }
    }
    public class PhoneRenderer : Renderer
    {
        internal Microsoft.Xna.Framework.Graphics.Texture2D texture;
        internal ContentManager manager;
        internal Game interngame;
        internal GraphicsDevice idevice;
        public void NtfyReadyRender()
        {
            Draw();
        }
        public PhoneRenderer(Game _game,ContentManager _manager, GraphicsDevice mdev)
        {
            interngame = _game;
            idevice = mdev;
            manager = _manager;
        }
        internal BasicEffect shader;
        public Matrix matrix;
        public void ntfyTouchDown(int x, int y, int id)
        {
            mpad.ntfyTouchDown(x, y, id);
            
        }
        public void ntfyTouchUp(int x, int y, int id)
        {
            mpad.ntfyTouchRelease(x, y, id);
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override Keyboard defaultKeyboard
        {
            get { throw new NotImplementedException(); }
        }

        public override Mouse defaultMouse
        {
            get { throw new NotImplementedException(); }
        }
        XTouchpad mpad = new XTouchpad();
        public override Touchpad defaultTouchpad
        {
            get { return mpad; }
        }

        public override _3DAPI.Texture2D createTextureFromBitmap(System.Drawing.Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public override _3DAPI.VertexBuffer CreateVertexBuffer(Vector3D[] vertices, Vector2D[] texcoords, Vector3D[] normals)
        {
            return new WPVertexBuffer(this, vertices, texcoords, normals);
        }

        public override _3DAPI.Texture2D createTexture(int Width, int Height)
        {
            return new WPTexture(this);
        }

        public override Shader createBasicShader()
        {
            return new WPBasicShader(this);
        }
    }
}