//This code has been released into the public domain
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using _3DAPI;


public class ObjMeshLoader
{
	public static StreamReader[] LoadMeshes(string fileName) {
		StreamReader mreader = new StreamReader(File.OpenRead(fileName));
		MemoryStream current = null;
		List<MemoryStream> mstreams = new List<MemoryStream>();
		StreamWriter mwriter = null;
		while(!mreader.EndOfStream) {
		string cmd = mreader.ReadLine();
				string line = cmd;
		line = line.Trim(splitCharacters);
            line = line.Replace("  ", " ");

            string[] parameters = line.Split(splitCharacters);
			if(parameters[0] == "mtllib") {
			loadMaterials(parameters[1]);
			}
			if(parameters[0] == "o") {
			if(mwriter!=null) {
				mwriter.Flush();
					current.Position = 0;
				}
				current = new MemoryStream();
				mwriter = new StreamWriter(current);
				mwriter.WriteLine(parameters[1]);
				mstreams.Add(current);
			} else {
				if(mwriter!=null) {
			mwriter.WriteLine(cmd);
					mwriter.Flush();
				}
			}
		}
		mwriter.Flush();
		current.Position = 0;
		List<StreamReader> readers = new List<StreamReader>();
		foreach(MemoryStream e in mstreams) {
			e.Position = 0;
		StreamReader sreader = new StreamReader(e);
			readers.Add(sreader);
		}
		return readers.ToArray();
	}
    public static bool Load(ObjMesh mesh, string fileName)
    {
        try
        {
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                Load(mesh, streamReader);
                streamReader.Close();
                return true;
            }
        }
        catch { return false; }
    }
	public static bool Load2(ObjMesh mesh, StreamReader streamReader, ObjMesh prevmesh)
    {
		if(prevmesh !=null) {
		//mesh.Vertices = prevmesh.Vertices;
			
		}
        try
        {
        //streamReader.BaseStream.Position = 0;
                Load(mesh, streamReader);
                streamReader.Close();
#if DEBUG
			Console.WriteLine("Loaded "+mesh.Triangles.Length.ToString()+" triangles and"+mesh.Quads.Length.ToString()+" quadrilaterals parsed, with a grand total of "+mesh.Vertices.Length.ToString()+" vertices.");
#endif
                return true;
            
        }
        catch(Exception er) { Console.WriteLine(er);return false; }
    }
    static char[] splitCharacters = new char[] { ' ' };

    static List<Vector3D> vertices;
    static List<Vector3D> normals;
    static List<Vector2D> texCoords;
    static Dictionary<ObjVertex, int> objVerticesIndexDictionary;
    static List<ObjVertex> objVertices;
    static List<ObjTriangle> objTriangles;
    static List<ObjQuad> objQuads;

	static Dictionary<string,Bitmap> materials = new Dictionary<string, Bitmap>();
	static void loadMaterials(string path) {
	StreamReader mreader = new StreamReader(File.OpenRead(path));
		string current = "";
		bool isfound = false;
		while(!mreader.EndOfStream) {
			string line = mreader.ReadLine();
		line = line.Trim(splitCharacters);
            line = line.Replace("  ", " ");

            string[] parameters = line.Split(splitCharacters);
            try
            {
                if (parameters[0] == "newmtl")
                {
                    if (materials.ContainsKey(parameters[1]))
                    {
                        isfound = true;
                    }
                    else
                    {
                        current = parameters[1];
                    }
                }
            }
            catch (Exception er)
            {
            }
			if(parameters[0] == "map_Kd") {
			if(!isfound) {
					string filename = "";
					for(int i = 1;i<parameters.Length;i++) {
					filename+=parameters[i];
					}
					string searcher = "\\"+"\\";
					filename.Replace(searcher,"\\");
                    filename = filename.Substring(filename.LastIndexOf("\\") + 1);
                    try
                    {
                        Bitmap mymap = new Bitmap(filename);
                        materials.Add(current, mymap);
                    }
                    catch (Exception er)
                    {
                        Bitmap mymap = new Bitmap(512, 512);
                        Graphics mfix = Graphics.FromImage(mymap);
                        mfix.Clear(Color.Red);
                        mfix.DrawString("Unable to load file\n" + filename, new Font(FontFamily.GenericMonospace, 24), Brushes.Yellow, new RectangleF(0,0,512,512));
                        mfix.Dispose();
                    }
					isfound = false;
				}
			}
		}
	}
    static void Load(ObjMesh mesh, StreamReader textReader)
    {
		
		try {
		//vertices = null;
			//objVertices = null;
			if(vertices == null) {
        vertices = new List<Vector3D>();
			}
			if(normals == null) {
        normals = new List<Vector3D>();
			}
			if(texCoords == null) {
        texCoords = new List<Vector2D>();
			}
			if(objVerticesIndexDictionary == null) {
        objVerticesIndexDictionary = new Dictionary<ObjVertex, int>();
			}
			if(objVertices == null) {
        objVertices = new List<ObjVertex>();
			}
        objTriangles = new List<ObjTriangle>();
        objQuads = new List<ObjQuad>();
			mesh.vertexPositionOffset = vertices.Count;
        string line;
        while ((line = textReader.ReadLine()) != null)
        {
			if(line.Length<2) {
			break;
			}
            line = line.Trim(splitCharacters);
            line = line.Replace("  ", " ");

            string[] parameters = line.Split(splitCharacters);

            switch (parameters[0])
            {
				case "usemtl":
					//Material specification
					try {
					mesh.Material = materials[parameters[1]];
					}catch(KeyNotFoundException) {
					Console.WriteLine("WARNING: Texture parse failure: "+parameters[1]);
					}
					break;
                case "p": // Point
                    break;

                case "v": // Vertex
                    float x = float.Parse(parameters[1]);
                    float y = float.Parse(parameters[2]);
                    float z = float.Parse(parameters[3]);
                    vertices.Add(new Vector3D(x, y, z));
                    break;

                case "vt": // TexCoord
                    float u = float.Parse(parameters[1]);
                    float v = float.Parse(parameters[2]);
                    texCoords.Add(new Vector2D(u, v));
                    break;

                case "vn": // Normal
                    float nx = float.Parse(parameters[1]);
                    float ny = float.Parse(parameters[2]);
                    float nz = float.Parse(parameters[3]);
                    normals.Add(new Vector3D(nx, ny, nz));
                    break;

                case "f":
                    switch (parameters.Length)
                    {
                        case 4:
                            ObjTriangle objTriangle = new ObjTriangle();
                            objTriangle.Index0 = ParseFaceParameter(parameters[1]);
                            objTriangle.Index1 = ParseFaceParameter(parameters[2]);
                            objTriangle.Index2 = ParseFaceParameter(parameters[3]);
                            objTriangles.Add(objTriangle);
                            break;

                        case 5:
                            ObjQuad objQuad = new ObjQuad();
                            objQuad.Index0 = ParseFaceParameter(parameters[1]);
                            objQuad.Index1 = ParseFaceParameter(parameters[2]);
                            objQuad.Index2 = ParseFaceParameter(parameters[3]);
                            objQuad.Index3 = ParseFaceParameter(parameters[4]);
                            objQuads.Add(objQuad);
                            break;
                    }
                    break;
            }
			
			}
		}catch(Exception er) {
			Console.WriteLine(er);
			Console.WriteLine("Successfully recovered. Bounds/Collision checking may fail though");
        }

        mesh.Vertices = objVertices.ToArray();
        mesh.Triangles = objTriangles.ToArray();
        mesh.Quads = objQuads.ToArray();

      
    }
	public static void Clear() {
		  objVerticesIndexDictionary = null;
        vertices = null;
        normals = null;
        texCoords = null;
        objVertices = null;
        objTriangles = null;
        objQuads = null;
	}
    static char[] faceParamaterSplitter = new char[] { '/' };
    static int ParseFaceParameter(string faceParameter)
    {
        Vector3D vertex = new Vector3D();
        Vector2D texCoord = new Vector2D();
        Vector3D normal = new Vector3D();

        string[] parameters = faceParameter.Split(faceParamaterSplitter);

        int vertexIndex = int.Parse(parameters[0]);
        if( vertexIndex < 0 ) vertexIndex = vertices.Count + vertexIndex;
        else vertexIndex = vertexIndex -1;
        //Hmm. This seems to be broken.
		try {
		vertex = vertices[vertexIndex];
		}catch(Exception) {
		throw new Exception("Vertex recognition failure at "+vertexIndex.ToString());
		}
        if (parameters.Length > 1)
        {
            int texCoordIndex = int.Parse(parameters[1]);
            if (texCoordIndex < 0) texCoordIndex = texCoords.Count + texCoordIndex;
            else texCoordIndex = texCoordIndex - 1;
		try {
            texCoord = texCoords[texCoordIndex];
				}catch(Exception) {
		Console.WriteLine("ERR: Vertex "+vertexIndex+" not found. ");
			throw new DllNotFoundException(vertexIndex.ToString());
		}
			
        }

        if (parameters.Length > 2)
        {
            int normalIndex = int.Parse(parameters[2]);
            if (normalIndex < 0) normalIndex = normals.Count + normalIndex;
            else normalIndex = normalIndex - 1;
            normal = normals[normalIndex];
        }

        return FindOrAddObjVertex(ref vertex, ref texCoord, ref normal);
    }
    static int FindOrAddObjVertex(ref Vector3D vertex, ref Vector2D texCoord, ref Vector3D normal)
    {
        ObjVertex newObjVertex = new ObjVertex();
        newObjVertex.Vertex = vertex;
        newObjVertex.TexCoord = texCoord;
        newObjVertex.Normal = normal;

        int index;
        if (objVerticesIndexDictionary.TryGetValue(newObjVertex, out index))
        {
            return index;
        }
        else
        {
            objVertices.Add(newObjVertex);
            objVerticesIndexDictionary[newObjVertex] = objVertices.Count - 1;
            return objVertices.Count - 1;
        }
    }
}