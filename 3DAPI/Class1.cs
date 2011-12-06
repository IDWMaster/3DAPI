using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;

namespace _3DAPI
{
	public struct Vector2D {
	public float X,Y;
		
		public Vector2D(float x, float y) {
		X = x;
			Y = y;
		}
		
	}
    public struct Vector3D {
        const double piover180 = Math.PI / 180;
        /// <summary>
        /// Computes a rotational direction
        /// </summary>
        /// <param name="rotation"></param>
        public static Vector3D ComputeRotation(Vector3D rotation)
        {
            Vector3D retval = new Vector3D();
            retval.X += (float)Math.Sin(-rotation.Y * piover180);
            retval.Z += (float)Math.Cos(rotation.Y * piover180);
            retval.Y += (float)Math.Sin(rotation.X * piover180);
            return retval;
        }
        /// <summary>
        /// Gets the center between two vectors
        /// </summary>
        /// <param name="othervect">The other vector to center</param>
        /// <returns></returns>
        public Vector3D Center(Vector3D othervect)
        {
            return new Vector3D((othervect.X + X) / 2, (othervect.Y + Y) / 2, (othervect.Z + Z) / 2);
        }
        public override string ToString()
        {
            return "X:"+X.ToString()+",Y:"+Y.ToString()+",Z:"+Z.ToString();
        }
        public Vector3D CrossProduct(Vector3D b)
        {
            return new Vector3D(Y * b.Z - Z * b.Y,Z*b.X-X*b.Z,X*b.Y-Y*b.X);


        }
        public Vector3D Normalize()
        {

            float mag = Magnitude;
            if (mag == 0)
            {
                return new Vector3D();
            }
            return new Vector3D(X / Magnitude, Y / Magnitude, Z / Magnitude);
        }
        public static Vector3D operator *(Vector3D vect, float multiplier)
        {
            return new Vector3D(vect.X * multiplier, vect.Y * multiplier, vect.Z * multiplier);
        }
        public static Vector3D operator /(Vector3D vect, float multiplier)
        {
            return new Vector3D(vect.X / multiplier, vect.Y / multiplier, vect.Z / multiplier);
        }
        public static Vector3D operator *(float multiplier, Vector3D vect)
        {
            return new Vector3D(vect.X * multiplier, vect.Y * multiplier, vect.Z * multiplier);
        }
        public static Vector3D operator *(Vector3D vect, Vector3D vectb)
        {
            return new Vector3D(vect.X * vectb.X, vect.Y * vectb.Y, vect.Z * vectb.Z);

        }
    public static Vector3D operator +(Vector3D vect, Vector3D vectb) {
        Vector3D vecta = vect;
        vecta.X += vectb.X;
        vecta.Y += vectb.Y;
        vecta.Z += vectb.Z;
        return vecta;
    }
    public static Vector3D operator -(Vector3D vect, Vector3D vectb)
    {
        Vector3D vecta = vect;
        vecta.X -= vectb.X;
        vecta.Y -= vectb.Y;
        vecta.Z -= vectb.Z;
        return vecta;
    }
    public float X,Y,Z;
    /// <summary>
    /// Gets the magnitude of this vector
    /// </summary>
        public float Magnitude
    {
        get
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }
    }
		public Vector3D(float x, float y, float z) {
		X = x;
			Y = y;
			Z = z;
            
		}
    }
    /// <summary>
    /// Represents a RenderCommand to be performed on the GUI thread. This should be initiated from another thread, then processed by the GUI thread
    /// </summary>
    public abstract class RenderCommand
    {
		internal static List<RenderCommand> removeList = new List<RenderCommand>();
        internal static List<RenderCommand> commandlist = new List<RenderCommand>();
     //Render will ALWAYS be called on the GUI thread
        protected abstract void Render();
        internal void Draw()
        {
            Render();
         
        }
        public void Remove()
        {
            lock (removeList)
            {
                removeList.Add(this);
            }
        }
        public RenderCommand()
        {
       
            lock (commandlist)
            {
                commandlist.Add(this);
            }
        }
    }
    enum TextureOperationType
    {
    Load, Render
    }
    class TextureOperation:RenderCommand
    {
        Texture2D underlyingtexture;
        protected override void Render()
        {
            if (_optype == TextureOperationType.Load)
            {
                underlyingtexture.doLoad();
                Remove();
            }
            if (_optype == TextureOperationType.Render)
            {
                underlyingtexture.doRender();
            }
        }
        TextureOperationType _optype;
        public TextureOperation(Texture2D _underlyingtexture, TextureOperationType optype)
        {
            underlyingtexture = _underlyingtexture;
            _optype = optype;
        }
    }
    public abstract class Texture2D:IDrawable
    {
        int _width;
        int _height;
        public int Width
        {
            get
            {
                return _width;
            }
        }
        public int Height
        {
            get
            {
                return _height;
            }
        }
        internal void doRender()
        {
            DrawTex();
        }
        internal void doLoad()
        {
            LoadTex();
        }
        public Texture2D(int width, int height)
        {
            _width = width;
            _height = height;
            TextureOperation mop = new TextureOperation(this, TextureOperationType.Load);
        }
        /// <summary>
        /// Loads the texture into GPU memory
        /// </summary>
        protected abstract void LoadTex();
        /// <summary>
        /// Draws the existing texture in GPU memory
        /// </summary>
        protected abstract void DrawTex();
        public override void Draw()
        {
            TextureOperation mtion = new TextureOperation(this,TextureOperationType.Render);
            
        }
        protected abstract void uploadbitmap(Bitmap tmap);
        internal void ___ubtmp(Bitmap z7jr_)
        {
            uploadbitmap(z7jr_);
            z7jr_.Dispose();
        }
        /// <summary>
        /// Replaces this texture with the specified bitmap, disposing of the bitmap when done.
        /// </summary>
        /// <param name="bitmap"></param>
        public void UploadBitmap(Bitmap bitmap)
        {
            UploadBitmapOperation mop = new UploadBitmapOperation(bitmap, this);
            
        }
    }
    class UploadBitmapOperation : RenderCommand
    {
        Bitmap bitmap;
        Texture2D texptr;
        public UploadBitmapOperation(Bitmap mmap, Texture2D tptr)
        {
            bitmap = mmap;
            texptr = tptr;

        }
        
        protected override void Render()
        {
            texptr.___ubtmp(bitmap);
            Remove();
        }
    }
    class DrawBufferCommand : RenderCommand
    {
        VertexBuffer internbuffer;
        protected override void Render()
        {
            internbuffer.Render();
        }
        public DrawBufferCommand(VertexBuffer buffer)
        {
            internbuffer = buffer;
        }
    }
    public abstract class IDrawable
    {
        public abstract void Draw();
    }
    class DisposeCommand : RenderCommand
    {

        VertexBuffer tbuff;
        public DisposeCommand(VertexBuffer mt)
        {
            tbuff = mt;

        }
        protected override void Render()
        {
            tbuff.CdLD();
            Remove();
        }
    }
    public abstract class VertexBuffer:IDrawable
    {
        /// <summary>
        /// Indicates whether or not depth testing is to be performed on this VertexBuffer. Useful for foreground text/images and stuff
        /// </summary>
        public abstract bool DepthTesting
        {
            get;
            set;
        }
        /// <summary>
        /// Indicates whether or not this VertexBuffer is static (static buffers do not move relative to the camera position)
        /// </summary>
        public abstract bool IsStatic
        {
            get;
            set;
        }
        //The constructor should create a Vertex Buffer Object (VBO) on the GPU, and any Get/Set operation should update that on the GPU
        //This class and all instance members should be thread-safe.
        public Vector3D rotation = new Vector3D();
        public Vector3D Position;
        /// <summary>
        /// Disposes of the VertexBuffer from graphics memory
        /// </summary>
        protected abstract void _Dispose();
        internal void CdLD()
        {
            _Dispose();
        }
        public void Dispose()
        {
            rcmd.Remove();
            DisposeCommand dspcd = new DisposeCommand(this);
        }
        DrawBufferCommand rcmd = null;
        public override void Draw()
        {
            rcmd = new DrawBufferCommand(this);
            
        }
        internal void Render()
        {
            RenderBuffer();
        }
        protected abstract void RenderBuffer();
        
        public abstract Vector3D[] VertexArray
        {
            get;
            set;
        }
    }
	public abstract class Shader:IDrawable {
	public override void Draw ()
		{
			ApplyShaderOperation op = new ApplyShaderOperation(this);
			op.evt.WaitOne();
			
		}

		internal void doRender() {
		ApplyShader();
            
		}
		protected abstract void ApplyShader();
	}
	class ApplyShaderOperation:RenderCommand {
		#region implemented abstract members of _3DAPI.RenderCommand
		protected override void Render ()
		{
			
			internshader.doRender();
			lock(RenderCommand.removeList) {
			RenderCommand.removeList.Add(this);
			}
			evt.Set();
		}
		Shader internshader;
		public ManualResetEvent evt = new ManualResetEvent(false);
		public ApplyShaderOperation(Shader shader) {
		internshader = shader;
		}
		
		#endregion
	
	}
    public class Mesh
    {
        public Vector3D[] meshverts;
        public Vector3D[] meshnorms;
        public Vector2D[] meshtexas;
        public System.Drawing.Bitmap bitmap;
    }
	public class Primitives {
        public static Mesh[] LoadMesh(string filename, bool fliptexcoords)
        {
            ObjMeshLoader.Clear();
            StreamReader[] readers = ObjMeshLoader.LoadMeshes(filename);
            ObjMesh prevmesh = null;
            List<Mesh> meshes = new List<Mesh>();
            foreach (StreamReader et in readers)
            {
                List<Vector3D> meshverts = new List<Vector3D>();
                List<Vector3D> meshnorms = new List<Vector3D>();
                List<Vector2D> meshtex = new List<Vector2D>();
                ObjMesh currentmesh = new ObjMesh();
             
                bool success = ObjMeshLoader.Load2(currentmesh, et, prevmesh);
                foreach (ObjTriangle ett in currentmesh.Triangles)
                {
                    meshverts.Add(new Vector3D(currentmesh.Vertices[(int)ett.Index0].Vertex.X, currentmesh.Vertices[(int)ett.Index0].Vertex.Y, currentmesh.Vertices[(int)ett.Index0].Vertex.Z));
                    meshverts.Add(new Vector3D(currentmesh.Vertices[(int)ett.Index1].Vertex.X, currentmesh.Vertices[(int)ett.Index1].Vertex.Y, currentmesh.Vertices[(int)ett.Index1].Vertex.Z));
                    meshverts.Add(new Vector3D(currentmesh.Vertices[(int)ett.Index2].Vertex.X, currentmesh.Vertices[(int)ett.Index2].Vertex.Y, currentmesh.Vertices[(int)ett.Index2].Vertex.Z));

                    //Normals
                    meshnorms.Add(new Vector3D(currentmesh.Vertices[(int)ett.Index0].Normal.X, currentmesh.Vertices[(int)ett.Index0].Normal.Y, currentmesh.Vertices[(int)ett.Index0].Normal.Z));
                    meshnorms.Add(new Vector3D(currentmesh.Vertices[(int)ett.Index1].Normal.X, currentmesh.Vertices[(int)ett.Index1].Normal.Y, currentmesh.Vertices[(int)ett.Index1].Normal.Z));
                    meshnorms.Add(new Vector3D(currentmesh.Vertices[(int)ett.Index2].Normal.X, currentmesh.Vertices[(int)ett.Index2].Normal.Y, currentmesh.Vertices[(int)ett.Index2].Normal.Z));

                    //Texcoords
                    if (fliptexcoords)
                    {
                        meshtex.Add(new Vector2D(currentmesh.Vertices[(int)ett.Index0].TexCoord.X, -currentmesh.Vertices[(int)ett.Index0].TexCoord.Y));
                        meshtex.Add(new Vector2D(currentmesh.Vertices[(int)ett.Index1].TexCoord.X, -currentmesh.Vertices[(int)ett.Index1].TexCoord.Y));
                        meshtex.Add(new Vector2D(currentmesh.Vertices[(int)ett.Index2].TexCoord.X, -currentmesh.Vertices[(int)ett.Index2].TexCoord.Y));
                    }
                    else
                    {
                        meshtex.Add(new Vector2D(currentmesh.Vertices[(int)ett.Index0].TexCoord.X, currentmesh.Vertices[(int)ett.Index0].TexCoord.Y));
                        meshtex.Add(new Vector2D(currentmesh.Vertices[(int)ett.Index1].TexCoord.X, currentmesh.Vertices[(int)ett.Index1].TexCoord.Y));
                        meshtex.Add(new Vector2D(currentmesh.Vertices[(int)ett.Index2].TexCoord.X, currentmesh.Vertices[(int)ett.Index2].TexCoord.Y));
                    }
                }
                Mesh realmesh = new Mesh();
                realmesh.meshverts = meshverts.ToArray();
                realmesh.meshnorms = meshnorms.ToArray();
                realmesh.meshtexas = meshtex.ToArray();
                realmesh.bitmap = currentmesh.Material;
                meshes.Add(realmesh);
            }

            return meshes.ToArray();
        }
		public static void plotRectangle(out Vector3D[] vertices,out Vector2D[] texcoords, float x, float y, float z, float width, float height) {
			List<Vector3D> points = new List<Vector3D>();
			//First triangle
			//0,0,0
			points.Add(new Vector3D(x,y,z));
			//0,1,0
			points.Add(new Vector3D(x,y+height,z));
			//1,1,0
			points.Add(new Vector3D(x+width,y+height,z));
			//Second triangle
			//1,1,0
			points.Add(new Vector3D(x+width,y+height,z));
			//1,0,0
			points.Add(new Vector3D(x+width,y,z));
			//0,0,0
			points.Add(new Vector3D(x,y,z));
			
			
			//Create texcoords
			List<Vector2D> t = new List<Vector2D>();
			//0,0
			t.Add(new Vector2D(0,0));
			//0,1
			t.Add(new Vector2D(0,.5f));
			//1,1
			t.Add(new Vector2D(.5f,.5f));
     
			
            //1,1
            t.Add(new Vector2D(1,1));
			//1,0
			t.Add(new Vector2D(1,.5f));
			//0,0
			t.Add(new Vector2D(.5f,.5f));
			vertices = points.ToArray();
			texcoords = t.ToArray();
			
		}
		public static VertexBuffer plotCube(float x, float y,float z,float width, float depth, float height) {
		Vector3D[] frontsideverts;
			Vector2D[] frontsidecoords;
			plotRectangle(out frontsideverts,out frontsidecoords,x,y,z,width,height);
			Vector3D[] backsideverts;
			Vector2D[] backsidecoords;
		throw new NotImplementedException();	
		}

        /// <summary>
        /// Creates a quad
        /// </summary>
        /// <param name="vertices">Input vertices specific to this quad</param>
        /// <param name="texcoords">Input texcoords specific to this quad</param>
        /// <param name="normals">Input normals specific to this quad</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="cz"></param>
        /// <param name="inputnormals"></param>
        /// <param name="inputtexcoords"></param>
        public static void CreateQuad(out Vector3D[] vertices, out Vector2D[] texcoords,out Vector3D[] normals, float x, float y, float z, float cx, float cy, float cz, Vector3D[] inputnormals, Vector2D[] inputtexcoords)
        {
            Vector3D[] verts;
            Vector2D[] faketexcoords;
            Vector3D[] norms;
            plotQuad(out verts, out faketexcoords, x, y, z, cx - x, cy - y, cz - z);
            vertices = verts;
            normals = inputnormals;
            //TODO: Calculate the NEW texcoords through subtraction
            List<Vector2D> texc = new List<Vector2D>();
            for(int i = 0;i<vertices.Length;i++)
            {
                texc.Add(new Vector2D(faketexcoords[i].X - inputtexcoords[i].X, faketexcoords[i].Y - inputtexcoords[i].Y));
            }
            texcoords = texc.ToArray();
        }
		public static void plotQuad(out Vector3D[] vertices,out Vector2D[] texcoords, float x, float y, float z, float width,float height, float depth) {
			List<Vector3D> points = new List<Vector3D>();
			//First triangle
			//0,0,0
			points.Add(new Vector3D(x,y,z));
			//0,1,1
			points.Add(new Vector3D(x,y+height,z+depth));
			//1,1,1
			points.Add(new Vector3D(x+width,y+height,z+depth));
			//Second triangle
			//1,1,1
			points.Add(new Vector3D(x+width,y+height,z+depth));
			//1,0,0
			points.Add(new Vector3D(x+width,y,z));
			//0,0,0
			points.Add(new Vector3D(x,y,z));
			
			
			//Create texcoords
			List<Vector2D> t = new List<Vector2D>();
			//0,0
			t.Add(new Vector2D(0,0));
			//0,1
			t.Add(new Vector2D(0,1));
			//1,1
			t.Add(new Vector2D(1,1));
     
			
            //1,1
            t.Add(new Vector2D(1,1));
			//1,0
			t.Add(new Vector2D(1,0));
			//0,0
			t.Add(new Vector2D(0,0));
			vertices = points.ToArray();
			texcoords = t.ToArray();
			
		}
        
		
	public static VertexBuffer createRectangle(Renderer renderer, float x, float y, float z, float width, float height) {
		//Create vertices
			List<Vector3D> points = new List<Vector3D>();
			//First triangle
			//0,0,0
			points.Add(new Vector3D(x,y,z));
			//0,1,0
			points.Add(new Vector3D(x,y+height,z));
			//1,1,0
			points.Add(new Vector3D(x+width,y+height,z));
			//Second triangle
			//1,1,0
			points.Add(new Vector3D(x+width,y+height,z));
			//1,0,0
			points.Add(new Vector3D(x+width,y,z));
			//0,0,0
			points.Add(new Vector3D(x,y,z));
			
			
			//Create texcoords
			List<Vector2D> t = new List<Vector2D>();
			//0,0
			t.Add(new Vector2D(0,0));
			//0,1
			t.Add(new Vector2D(0,1));
			//1,1
			t.Add(new Vector2D(1,1));
     
			
            //1,1
            t.Add(new Vector2D(1,1));
			//1,0
			t.Add(new Vector2D(1,0));
			//0,0
			t.Add(new Vector2D(0,0));
            List<Vector3D> nor = new List<Vector3D>();
            float ft = 1;
            for (int i = 0; i < 6; i++)
            {
            //Add the norfolk to the european re-union in America
                ft -= .1f;
                nor.Add(new Vector3D(1, 1, 1));
            }
			return renderer.CreateVertexBuffer(points.ToArray(),t.ToArray(),nor.ToArray());
		    
		}
	}
	public enum PrimitiveMode {
	TriangleList, TriangleStrip
	}
    public abstract class Renderer
    {
        List<Keyboard> extensionkeyboards = new List<Keyboard>();
        public void RegisterExtensionKeyboard(Keyboard extension)
        {
            extensionkeyboards.Add(extension);
        }
        /// <summary>
        /// Gets an extended keyboard
        /// </summary>
        /// <returns></returns>
        public Keyboard[] GetExtensionKeyboards()
        {
            List<Keyboard> keyboards = new List<Keyboard>();
            try
            {
                keyboards.Add(defaultKeyboard);
            }
            catch (Exception er)
            {
            }
            foreach (Keyboard et in extensionkeyboards)
            {
                keyboards.Add(et);
            }
            return keyboards.ToArray();
        }
        public Vector3D worldRotation = new Vector3D();
        public abstract void Dispose();
        public abstract void SetRenderTarget(Texture2D texture, Vector3D campos, Vector3D camrot);
        public abstract Keyboard defaultKeyboard
        {
            get;
            
        }
        public abstract Mouse defaultMouse
        {
            get;
        }
        public abstract Touchpad defaultTouchpad
        {
            get;
        }
        public abstract Texture2D createTextureFromBitmap(System.Drawing.Bitmap bitmap);
        public Vector3D cameraPosition = new Vector3D(0,0,0);
		/// <summary>
		///Creates a generic vertex buffer 
		/// </summary>
		/// <returns>
		/// A <see cref="VertexBuffer"/>
		/// </returns>
        public abstract VertexBuffer CreateVertexBuffer(Vector3D[] vertices, Vector2D[] texcoords, Vector3D[] normals);
		/// <summary>
		///Creates a generic texture with the specified width and height 
		/// </summary>
		/// <param name="Width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="Height">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="Texture2D"/>
		/// </returns>
        public abstract Texture2D createTexture(int Width, int Height);
		/// <summary>
		///Creates a basic shader, with default options 
		/// </summary>
		/// <returns>
		/// A <see cref="Shader"/>
		/// </returns>
		public abstract Shader createBasicShader();
        public Renderer()
        {
        
        }
		protected PrimitiveMode _imode;
		class SetPrimitiveModeOperation:RenderCommand {
			#region implemented abstract members of _3DAPI.RenderCommand
			protected override void Render ()
			{
				_mder.pmode = val;
			}
			PrimitiveMode val;
			Renderer _mder;
			public SetPrimitiveModeOperation(Renderer mder, PrimitiveMode mode) {
			_mder = mder;
				val = mode;
			}
			#endregion
		
		}
		internal PrimitiveMode pmode {
		get {
			return _imode;
			}set {
			_imode = value;
			}
		}
		public void SetPrimitiveMode(PrimitiveMode mode) {
		SetPrimitiveModeOperation mop = new SetPrimitiveModeOperation(this,mode);
		}
		protected void Draw() {
		lock(RenderCommand.commandlist) {
			foreach(RenderCommand et in RenderCommand.commandlist) {
				et.Draw();
				}
				lock(RenderCommand.removeList) {
				foreach(RenderCommand et in RenderCommand.removeList) {
				RenderCommand.commandlist.Remove(et);
				}
				}
				RenderCommand.removeList.Clear();
			}
		}
    }
}
