
using System;
using System.IO;
using System.Collections.Generic;

using OpenTK.Math;
using OpenTK.Graphics;
using System.Drawing;

public class ObjMesh
{
	ObjMesh() {
	
	}
	public static ObjMesh deserialize(BinaryReader mreader) {

	ObjMesh mymesh = new ObjMesh();
	bool hasimage = mreader.ReadBoolean();
		if(hasimage) {
		int len = (int)mreader.ReadInt64();
			byte[] image = mreader.ReadBytes(len);
			MemoryStream mstream = new MemoryStream();
			mstream.Write(image,0,image.Length);
			mstream.Position = 0;
            try
            {
                mymesh.Material = new Bitmap(mstream);
            }
            catch (Exception)
            {
            
            }
		}
        
		mymesh.name = mreader.ReadString();
		mymesh.quads = new ObjMesh.ObjQuad[mreader.ReadInt32()];
		for(int i = 0;i<mymesh.quads.Length;i++) {
		ObjQuad currentQuad = new ObjQuad();
			currentQuad.Index0 = mreader.ReadInt32();
			currentQuad.Index1 = mreader.ReadInt32();
			currentQuad.Index2 = mreader.ReadInt32();
			currentQuad.Index3 = mreader.ReadInt32();
			mymesh.quads[i] = currentQuad;
		}
		mymesh.triangles = new ObjMesh.ObjTriangle[mreader.ReadInt32()];
		for(int i = 0;i<mymesh.triangles.Length;i++) {
		ObjTriangle currentQuad = new ObjTriangle();
			currentQuad.Index0 = mreader.ReadInt32();
			currentQuad.Index1 = mreader.ReadInt32();
			currentQuad.Index2 = mreader.ReadInt32();
			mymesh.triangles[i] = currentQuad;
		}
		mymesh.vertices = new ObjMesh.ObjVertex[mreader.ReadInt32()];
		for(int i = 0;i<mymesh.vertices.Length;i++) {
	    ObjVertex mtex = new ObjVertex();
			mtex.Normal.X = mreader.ReadSingle();
			mtex.Normal.Y = mreader.ReadSingle();
			mtex.Normal.Z = mreader.ReadSingle();
			mtex.TexCoord.X = mreader.ReadSingle();
			mtex.TexCoord.Y = mreader.ReadSingle();
			mtex.Vertex.X = mreader.ReadSingle();
			mtex.Vertex.Y = mreader.ReadSingle();
			mtex.Vertex.Z = mreader.ReadSingle();
			mymesh.vertices[i] = mtex;
		}
		
		return mymesh;
	}
	public void Serialize(BinaryWriter mwriter) {
		MemoryStream mstream = new MemoryStream();
		if(Material == null) {
		mwriter.Write(false);
		} else {
		mwriter.Write(true);
			Material.Save(mstream,System.Drawing.Imaging.ImageFormat.Png);
		mwriter.Write(mstream.Length);
			mstream.Position = 0;
			byte[] data = new byte[mstream.Length];
			mstream.Read(data,0,data.Length);
			mwriter.Write(data);
			
		}
	mwriter.Write(name);
		mwriter.Write(quads.Length);
		foreach(ObjQuad et in quads) {
		mwriter.Write(et.Index0);
			mwriter.Write(et.Index1);
			mwriter.Write(et.Index2);
			mwriter.Write(et.Index3);
		}
		mwriter.Write(triangles.Length);
		foreach(ObjTriangle et in triangles) {
		mwriter.Write(et.Index0);
			mwriter.Write(et.Index1);
			mwriter.Write(et.Index2);
		}
		mwriter.Write(vertices.Length);
		foreach(ObjVertex et in vertices) {
		mwriter.Write(et.Normal.X);
			mwriter.Write(et.Normal.Y);
			mwriter.Write(et.Normal.Z);
			mwriter.Write(et.TexCoord.X);
			mwriter.Write(et.TexCoord.Y);
			mwriter.Write(et.Vertex.X);
			mwriter.Write(et.Vertex.Y);
			mwriter.Write(et.Vertex.Z);
			
		}
		
	}
	public string name = "";
    Vector3 translation = new Vector3();
	internal int vertexPositionOffset;
    CollisionBounds[] origtree = null;
	public void Transform(float X, float Y, float Z) {
        if (collisionTree.Count < 1)
        {
            testCollision(0, 0, 0);
        }
        translation = new Vector3(X, Y, Z);
        return;
        float x = X;
        float y = Y;
        float z = Z;
        //TODO: Somehow update collisiontree
        //return;
        if (origtree == null)
        {
            origtree = new CollisionBounds[collisionTree.Count];
            for (int i = 0; i < origtree.Length; i++)
            {
                origtree[i] = new CollisionBounds(null, null);
            }
            for (int i = 0; i < origtree.Length; i++)
            {
                List<ObjVertex> vt = new List<ObjVertex>();
                foreach (ObjVertex et in collisionTree[i].Key)
                {
                    vt.Add(new ObjVertex() { Vertex = et.Vertex });

                }
                origtree[i].Key = vt.ToArray();
                float[] data = new float[collisionTree[i].Value.Length];
                Buffer.BlockCopy(collisionTree[i].Value, 0, data, 0, data.Length * sizeof(float));
                origtree[i].Value = data;

            }
        }
        CollisionBounds[] tempray = new CollisionBounds[origtree.Length];

        for (int i = 0; i < origtree.Length; i++)
        {
            tempray[i] = new CollisionBounds(null, null);
        }
        for (int i = 0; i < origtree.Length; i++)
        {
            List<ObjVertex> vt = new List<ObjVertex>();
            foreach (ObjVertex et in origtree[i].Key)
            {
                vt.Add(new ObjVertex() { Vertex = et.Vertex });

            }
            tempray[i].Key = vt.ToArray();
            float[] data = new float[origtree[i].Value.Length];
            Buffer.BlockCopy(origtree[i].Value, 0, data, 0, data.Length * sizeof(float));
            tempray[i].Value = data;

        }
        collisionTree = new List<CollisionBounds>(tempray);
            
        foreach (CollisionBounds et in collisionTree)
        {
            foreach (ObjVertex ett in et.Key)
            {
                ett.Vertex.X -= x;
                ett.Vertex.Y -= y;
                ett.Vertex.Z -= z;
            }
            et.Value[0] -= x;
            et.Value[1] -= y;
            et.Value[2] -= z;
            et.Value[3] -= x;
            et.Value[4] -= y;
            et.Value[5] -= z;

        }
        return;
		Vector3 transformation = new Vector3(x,y,z);
	foreach(ObjQuad e in quads) {
		ObjVertex vertex = vertices[e.Index0];
			vertex.Vertex.X+=transformation.X;
			vertex.Vertex.Y+=transformation.Y;
			vertex.Vertex.Z+=transformation.Z;
			vertices[e.Index0] = vertex;
			vertex = vertices[e.Index1];
			vertex.Vertex.X+=transformation.X;
			vertex.Vertex.Y+=transformation.Y;
			vertex.Vertex.Z+=transformation.Z;
			vertices[e.Index1] = vertex;
			vertex = vertices[e.Index2];
			vertex.Vertex.X+=transformation.X;
			vertex.Vertex.Y+=transformation.Y;
			vertex.Vertex.Z+=transformation.Z;
			vertices[e.Index2] = vertex;
			vertex = vertices[e.Index3];
			vertex.Vertex.X+=transformation.X;
			vertex.Vertex.Y+=transformation.Y;
			vertex.Vertex.Z+=transformation.Z;
			vertices[e.Index3] = vertex;
		}
		foreach(ObjTriangle e in triangles) {
		ObjVertex vertex = vertices[e.Index0];
			vertex.Vertex.X+=transformation.X;
			vertex.Vertex.Y+=transformation.Y;
			vertex.Vertex.Z+=transformation.Z;
			vertex = vertices[e.Index1];
			vertex.Vertex.X+=transformation.X;
			vertex.Vertex.Y+=transformation.Y;
			vertex.Vertex.Z+=transformation.Z;
			vertex = vertices[e.Index2];
			vertex.Vertex.X+=transformation.X;
			vertex.Vertex.Y+=transformation.Y;
			vertex.Vertex.Z+=transformation.Z;
		}
	}
	
	/// <summary>
	///Gets the bounds of this mesh 
	/// </summary>
	/// <returns>
	/// A <see cref="System.Single[]"/>
	/// </returns>
	public float[] getBounds() {
	float minx = 5000000;
				float miny = 5000000;
				float minz = 5000000;
				float maxx = -5000000;
				float maxy = -5000000;
				float maxz = -5000000;
	
		//Experimental - Simulate GPU processing of verticies in triangles/quads
		Console.WriteLine("Parsed "+quads.Length+" quads");
		for(int i = 0;i<quads.Length;i++) {
		
			ObjQuad et = quads[i];
		ObjVertex e = vertices[et.Index0];
		ObjVertex ey = new ObjVertex();
			
			ey.Vertex.X = e.Vertex.X*-1;
			ey.Vertex.Y = e.Vertex.Y*-1;
			ey.Vertex.Z = e.Vertex.Z*-1;
			e = ey;
		if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			if(e.Vertex.Z<minz) {
			minz = e.Vertex.Z;
			}
			if(e.Vertex.Z>maxz) {
			maxz = e.Vertex.Z;
			}
			e = vertices[et.Index1];
		    ey = new ObjVertex();
			ey.Vertex.X = e.Vertex.X*-1;
			ey.Vertex.Y = e.Vertex.Y*-1;
			ey.Vertex.Z = e.Vertex.Z*-1;
		e = ey;
			if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			if(e.Vertex.Z<minz) {
			minz = e.Vertex.Z;
			}
			if(e.Vertex.Z>maxz) {
			maxz = e.Vertex.Z;
			}
			e = vertices[et.Index2];
		ey = new ObjVertex();
			ey.Vertex.X = e.Vertex.X*-1;
			ey.Vertex.Y = e.Vertex.Y*-1;
			ey.Vertex.Z = e.Vertex.Z*-1;
			e = ey;
		if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			if(e.Vertex.Z<minz) {
			minz = e.Vertex.Z;
			}
			if(e.Vertex.Z>maxz) {
			maxz = e.Vertex.Z;
			}
			e = vertices[et.Index3];
		ey = new ObjVertex();
			
			ey.Vertex.X = e.Vertex.X*-1;
			ey.Vertex.Y = e.Vertex.Y*-1;
			ey.Vertex.Z = e.Vertex.Z*-1;
		e = ey;
			if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			if(e.Vertex.Z<minz) {
			minz = e.Vertex.Z;
			}
			if(e.Vertex.Z>maxz) {
			maxz = e.Vertex.Z;
			}
			
			
			
		}
		//And do the same for each triangle
		foreach(ObjTriangle et in triangles) {
		ObjVertex e = vertices[et.Index0];
		ObjVertex ey = new ObjVertex();
			ey.Vertex.X = e.Vertex.X*-1;
			ey.Vertex.Y = e.Vertex.Y*-1;
			ey.Vertex.Z = e.Vertex.Z*-1;
		e = ey;
			if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			if(e.Vertex.Z<minz) {
			minz = e.Vertex.Z;
			}
			if(e.Vertex.Z>maxz) {
			maxz = e.Vertex.Z;
			}
			e = vertices[et.Index1];
		ey = new ObjVertex();
			ey.Vertex.X = e.Vertex.X*-1;
			ey.Vertex.Y = e.Vertex.Y*-1;
			ey.Vertex.Z = e.Vertex.Z*-1;
			e = ey;
		if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			if(e.Vertex.Z<minz) {
			minz = e.Vertex.Z;
			}
			if(e.Vertex.Z>maxz) {
			maxz = e.Vertex.Z;
			}
			e = vertices[et.Index2];
		ey = new ObjVertex();
			ey.Vertex.X = e.Vertex.X*-1;
			ey.Vertex.Y = e.Vertex.Y*-1;
			ey.Vertex.Z = e.Vertex.Z*-1;
		e = ey;
			if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			if(e.Vertex.Z<minz) {
			minz = e.Vertex.Z;
			}
			if(e.Vertex.Z>maxz) {
			maxz = e.Vertex.Z;
			}
			
			
			e = vertices[et.Index0];
		ey = new ObjVertex();
			ey.Vertex.X = e.Vertex.X*-1;
			ey.Vertex.Y = e.Vertex.Y*-1;
			ey.Vertex.Z = e.Vertex.Z*-1;
			e = ey;
		if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			
		}
		minx-=1;
		miny-=1;
		minz-=1;
		maxx+=1;
		maxy+=1;
		maxz+=1;
		#if DEBUG
				Console.WriteLine("MinY: "+miny.ToString());
#endif
		#if DEBUG
				Console.WriteLine("MaxY: "+maxy.ToString());
#endif
		return new float[] {minx,miny,minz,maxx,maxy,maxz};
		
	}
	public float[] getBounds(ObjVertex[] verts) {
	
	float minx = 5000000;
				float miny = 5000000;
				float minz = 5000000;
				float maxx = -5000000;
				float maxy = -5000000;
				float maxz = -5000000;
	
		//Experimental - Simulate GPU processing of verticies in triangles/quads
		ObjVertex ey;
		ObjVertex e;
		foreach(ObjVertex et in verts) {
		ey = new ObjVertex();
			ey.Vertex.X = et.Vertex.X*-1;
			ey.Vertex.Y = et.Vertex.Y*-1;
			ey.Vertex.Z = et.Vertex.Z*-1;
		e = ey;
			if(e.Vertex.X<minx) {
			minx = e.Vertex.X;
			}
			if(e.Vertex.X>maxx) {
			maxx = e.Vertex.X;
			}
			if(e.Vertex.Y<miny) {
			miny = e.Vertex.Y;

			}
			if(e.Vertex.Y>maxy) {
				maxy = e.Vertex.Y;

			}
			if(e.Vertex.Z<minz) {
			minz = e.Vertex.Z;
			}
			if(e.Vertex.Z>maxz) {
			maxz = e.Vertex.Z;
			}
		}
		minx-=1;
		miny-=1;
		minz-=1;
		maxx+=1;
		maxy+=1;
		maxz+=1;
		#if DEBUG
				Console.WriteLine("MinY: "+miny.ToString());
#endif
		#if DEBUG
				Console.WriteLine("MaxY: "+maxy.ToString());
#endif
		return new float[] {minx,miny,minz,maxx,maxy,maxz};
		
	}
	bool testCollisionTriangle(ObjVertex[] verti, Vector3 coord) {
	Vector3 max = new Vector3(-5000,-5000,-5000);
		Vector3 min = new Vector3(5000,5000,5000);
		
		foreach(ObjVertex et in verti) {
			ObjVertex e = new ObjVertex();
			e.Vertex.X = et.Vertex.X*-1;
			e.Vertex.Y = et.Vertex.Y*-1;
			e.Vertex.Z = et.Vertex.Z*-1;
	    if(e.Vertex.X<min.X) {
			min.X = e.Vertex.X;
			}
			if(e.Vertex.X>max.X) {
			max.X = e.Vertex.X;
			}
			if(e.Vertex.Y<min.Y) {
			min.Y = e.Vertex.Y;
			}
			if(e.Vertex.Y>max.Y) {
			max.Y = e.Vertex.Y;
			}
			if(e.Vertex.Z>max.Z) {
			max.Z = e.Vertex.Z;
			}
			if(e.Vertex.Z<min.Z) {
			min.Z = e.Vertex.Z;
			}
			
		}
       Vector3 sval = new Vector3(.9f,.9f,.9f);
		
		
		min-=sval;
		max+=sval;
		
		if(coord.X<max.X& coord.X>min.X&coord.Y>min.Y&coord.Y<max.Y&coord.Z>min.Z&coord.Z<max.Z) {
		return true;
				
		} else {
			
			
		return false;
		}
	}
	public bool testCollision(float x, float y, float z) {
		ObjVertex[] asarray = new ObjVertex[0];
	List<ObjVertex> currentArray = new List<ObjVertex>();
		
		if(collisionTree.Count == 0) {
		foreach(ObjTriangle e in triangles) {
			List<ObjVertex> usedverticies = new List<ObjVertex>();
		usedverticies.Add(vertices[e.Index0]);
			usedverticies.Add(vertices[e.Index1]);
			                  usedverticies.Add(vertices[e.Index2]);
				currentArray.AddRange(usedverticies.ToArray());
		if(currentArray.Count>1) {
            foreach (ObjVertex vtx in currentArray)
            {
                vtx.Vertex = new Vector3(vtx.Vertex.X + translation.X, vtx.Vertex.Y + translation.Y, vtx.Vertex.Z + translation.Z);
            }
				 asarray = currentArray.ToArray();
				collisionTree.Add(new CollisionBounds(asarray,getBounds(asarray)));
				
					currentArray.Clear();
				}
		}
        foreach (ObjVertex vtx in currentArray)
        {
            vtx.Vertex = new Vector3(vtx.Vertex.X + translation.X, vtx.Vertex.Y + translation.Y, vtx.Vertex.Z + translation.Z);
        }
			asarray = currentArray.ToArray();
				collisionTree.Add(new CollisionBounds(asarray,getBounds(asarray)));
				
					currentArray.Clear();
			
				
			//Quads
			foreach(ObjQuad e in quads) {
			List<ObjVertex> usedverticies = new List<ObjVertex>();
		Vector3 transform = new Vector3(5,5,5);
			//Original verticies
			usedverticies.Add(vertices[e.Index0]);
			usedverticies.Add(vertices[e.Index1]);
			usedverticies.Add(vertices[e.Index2]);
			usedverticies.Add(vertices[e.Index3]);
				
			currentArray.AddRange(usedverticies.ToArray());
			if(currentArray.Count>1) {
                foreach (ObjVertex vtx in currentArray)
                {
                    vtx.Vertex = new Vector3(vtx.Vertex.X + translation.X, vtx.Vertex.Y + translation.Y, vtx.Vertex.Z + translation.Z);
                }
				 asarray = currentArray.ToArray();
				collisionTree.Add(new CollisionBounds(asarray,getBounds(asarray)));
				
					currentArray.Clear();
				}
			
			
		}
            foreach (ObjVertex vtx in currentArray)
            {
                vtx.Vertex = new Vector3(vtx.Vertex.X + translation.X, vtx.Vertex.Y + translation.Y, vtx.Vertex.Z + translation.Z);
            }
		asarray = currentArray.ToArray();
				collisionTree.Add(new CollisionBounds(asarray,getBounds(asarray)));
			
					currentArray.Clear();
		}
		foreach(CollisionBounds et in collisionTree) {
		//minx, miny, minz, maxx,maxy,maxz
			if(x>et.Value[0] & y>et.Value[1] & z>et.Value[2] & x<et.Value[3] & y<et.Value[4]&z<et.Value[5]) {
			if(testCollisionTriangle(et.Key,new Vector3(x,y,z))) {
				return true;
				}
			}
		}
		return false;
			//End quads
			
			
		}
	
		
		
		
	
	public Bitmap Material = null;
	/// <summary>
	///Experimental: Scales this mesh 
	/// </summary>
	/// <param name="scaleFactor">
	/// A <see cref="System.Single"/>
	/// </param>
	public void Scale(float scaleFactor) {
	//TODO: Implement this function
		throw new NotImplementedException();
	}
    public ObjMesh(string fileName)
    {
        ObjMeshLoader.Load(this, fileName);
    }
	public ObjMesh(System.IO.StreamReader file, ObjMesh prevmesh) {
	ObjMeshLoader.Load2(this,file,prevmesh);
	}
    public ObjVertex[] Vertices
    {
        get { return vertices; }
        set { vertices = value; }
    }
    ObjVertex[] vertices;

    public ObjTriangle[] Triangles
    {
        get { return triangles; }
        set { triangles = value; }
    }
    ObjTriangle[] triangles;

    public ObjQuad[] Quads
    {
        get { return quads; }
        set { quads = value; }
    }
    ObjQuad[] quads;

    int verticesBufferId;
    int trianglesBufferId;
    int quadsBufferId;

    public void Prepare()
    {
//        if (verticesBufferId == 0)
//        {
//            GL.GenBuffers(1, out verticesBufferId);
//            GL.BindBuffer(BufferTarget.ArrayBuffer, verticesBufferId);
//            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf(typeof(ObjVertex))), vertices, BufferUsageHint.StaticDraw);
//        }
//
//        if (trianglesBufferId == 0)
//        {
//            GL.GenBuffers(1, out trianglesBufferId);
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, trianglesBufferId);
//            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf(typeof(ObjTriangle))), triangles, BufferUsageHint.StaticDraw);
//        }
//
//        if (quadsBufferId == 0)
//        {
//            GL.GenBuffers(1, out quadsBufferId);
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadsBufferId);
//            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf(typeof(ObjQuad))), quads, BufferUsageHint.StaticDraw);
//        }
    }

//    public void Render()
//    {
//        Prepare();
//
//        GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
//        GL.EnableClientState(EnableCap.VertexArray);
//        GL.BindBuffer(BufferTarget.ArrayBuffer, verticesBufferId);
//        GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf(typeof(ObjVertex)), IntPtr.Zero);
//
//        GL.BindBuffer(BufferTarget.ElementArrayBuffer, trianglesBufferId);
//        GL.DrawElements(BeginMode.Triangles, triangles.Length * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);
//
//        if (quads.Length > 0)
//        {
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadsBufferId);
//            GL.DrawElements(BeginMode.Quads, quads.Length * 4, DrawElementsType.UnsignedInt, IntPtr.Zero);
//        }
//
//        GL.PopClientAttrib();
//    }
	#region Experimental - Renders a special mesh
	DrawableInformation createInformation(Vector3 normal, Vector2 texcoord, Vector3 vertex) {
	DrawableInformation mfo = new DrawableInformation();
		mfo.Vertex = new IV3() { X = vertex.X, Y = vertex.Y, Z = vertex.Z};
		mfo.TexCoord = new IV2() { X = texcoord.X, Y = texcoord.Y};
		mfo.Normal = new IV3() {X = normal.X, Y = normal.Y, Z = normal.Z};
		//mfo.Vertex.X = mfo.Vertex.X*-1;
		//mfo.Vertex.Y = mfo.Vertex.Y*-1;
		//mfo.Vertex.Z = mfo.Vertex.Z*-1;
		return mfo;
		
	}
	
	public DrawableInfo[] RenderSpecial() {
	
		
		List<DrawableInfo> tfo = new List<DrawableInfo>();
		List<DrawableInformation> myfo = new List<DrawableInformation>();	
		foreach(ObjQuad e in quads) {
		myfo.Add(createInformation(vertices[e.Index0].Normal,vertices[e.Index0].TexCoord,vertices[e.Index0].Vertex));
	    myfo.Add(createInformation(vertices[e.Index1].Normal,vertices[e.Index1].TexCoord,vertices[e.Index1].Vertex));
	    myfo.Add(createInformation(vertices[e.Index2].Normal,vertices[e.Index2].TexCoord,vertices[e.Index2].Vertex));
	    myfo.Add(createInformation(vertices[e.Index3].Normal,vertices[e.Index3].TexCoord,vertices[e.Index3].Vertex));
	    tfo.Add(new DrawableInfo(myfo.ToArray()));
			myfo = new List<DrawableInformation>();
		}
	
		foreach(ObjTriangle e in triangles) {
		myfo.Add(createInformation(vertices[e.Index0].Normal,vertices[e.Index0].TexCoord,vertices[e.Index0].Vertex));
	    myfo.Add(createInformation(vertices[e.Index1].Normal,vertices[e.Index1].TexCoord,vertices[e.Index1].Vertex));
	    myfo.Add(createInformation(vertices[e.Index2].Normal,vertices[e.Index2].TexCoord,vertices[e.Index2].Vertex));
		tfo.Add(new DrawableInfo(myfo.ToArray()));
			myfo = new List<DrawableInformation>();
		}
		
		return tfo.ToArray();
	}
	#endregion
    List<CollisionBounds> collisionTree = new List<CollisionBounds>();
    public class ObjVertex
    {
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Vertex;
		
    }

 
    public class ObjTriangle
    {
		
        public int Index0;
        public int Index1;
        public int Index2;
    }
	public class CollisionBounds {
		public CollisionBounds(ObjVertex[] verts, float[] __) {
		Key = verts;
			Value = __;
		}
	public ObjVertex[] Key;
		public float[] Value;
	}
    public class ObjQuad
    {
		
        public int Index0;
        public int Index1;
        public int Index2;
        public int Index3;
    }
}
public class DrawableInformation {
	public IV3 Normal;
	public IV2 TexCoord;
	public IV3 Vertex;
}
public class DrawableInfo {
	public DrawableInfo(DrawableInformation[] intinfo) {
	info = intinfo;
	}
	public DrawableInformation[] info;
	
}
public struct IV3 {
	public float X, Y, Z;
}
public struct IV2 {
	public float X, Y;
}