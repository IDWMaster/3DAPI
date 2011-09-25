using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace _3DAPI
{
    public class ObjVertex
    {
        public Vector3D Vertex;
        public Vector2D TexCoord;
        public Vector3D Normal;
    }
    public class ObjTriangle
    {
        public float Index0;
        public float Index1;
        public float Index2;
        
    }
    public class ObjQuad
    {
        public float Index0;
        public float Index1;
        public float Index2;
        public float Index3;
    }
    public class ObjMesh
    {
        public ObjTriangle[] Triangles;
        public ObjQuad[] Quads;
        public int vertexPositionOffset;
        public Bitmap Material;
        public ObjVertex[] Vertices;
        
    }
}
