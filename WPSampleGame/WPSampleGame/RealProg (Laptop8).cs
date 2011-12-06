using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _3DAPI;
using _3DAPI.Physics;
namespace WPSampleGame
{
    
    class RealProg
    {
        PhysicalObject collisiontester;
        Mesh mainmesh;
        PhysicalObject theobject;

        public RealProg(Renderer renderer)
        {
            
            Shader myder = renderer.createBasicShader();
            World physicsworld = new World();
            //Uncomment for AABB collisions

            // physicsworld.testtype = CollisionTestType.AABB;
            myder.Draw();
            Texture2D mtex = renderer.createTexture(512, 512);
            mtex.Draw();


            Mesh[] meshes = Primitives.LoadMesh("test.obj");
            collisiontester = new PhysicalObject(meshes[0].meshverts, 5, CollisionType.Dynamic, physicsworld);


            mainmesh = meshes[0];
            foreach (Mesh mesh in meshes)
            {
                VertexBuffer tbuff = renderer.CreateVertexBuffer(mesh.meshverts, mesh.meshtexas, mesh.meshnorms);
                rotatingbuffer = tbuff;
                if (mesh.bitmap != null)
                {
                    Console.WriteLine("BITMAP RENDER");
                    Texture2D tt = renderer.createTextureFromBitmap(mesh.bitmap);
                    tt.Draw();
                }
                tbuff.Draw();
            }
            Mesh cube = Primitives.LoadMesh("playercube.obj")[0];

            theobject = new PhysicalObject(cube.meshverts, 9, CollisionType.Dynamic, physicsworld);


           
           
            collisiontester.ownedVBO = rotatingbuffer;
            theobject.ownedVBO = renderer.CreateVertexBuffer(cube.meshverts, cube.meshtexas, cube.meshnorms);

            theobject.ownedVBO.Draw();



            physicsworld.physicalobjects.Add(theobject);
            physicsworld.physicalobjects.Add(collisiontester);
            //theobject.Velocity = new Vector3D(-.01f, 0, 0);
            Mesh anothercube = Primitives.LoadMesh("playercube.obj")[0];
            PhysicalObject mobject = new PhysicalObject(anothercube.meshverts.Clone() as Vector3D[], 1, CollisionType.Dynamic, physicsworld);
            mobject.ownedVBO = renderer.CreateVertexBuffer(anothercube.meshverts, anothercube.meshtexas, anothercube.meshnorms);
            physicsworld.physicalobjects.Add(mobject);
            mobject.ownedVBO.Draw();
            mobject.Position = new Vector3D(30, 0, 0);
            //Set physics properties
            // collisiontester.Velocity = new Vector3D(.2f, 0, 0);
            theobject.Weight = 1;
            //Uncomment for an inelastic collision
            //collisiontester.mode = CollisionMode.Inelastic;
            collisiontester.Weight = 1;

            collisiontester.Velocity = new Vector3D(.05f, 0, 0);
            theobject.Position = new Vector3D(15, 0, 0);
            //collisiontester.Velocity = new Vector3D(-.2f, 0, 0);
            //End physics properties

            physicsworld.Start();
            List<Vector3D> vectors = new List<Vector3D>();
            List<Vector2D> texcoords = new List<Vector2D>();
            List<Vector3D> normals = new List<Vector3D>();
            for (int i = 0; i < 3; i++)
            {
                normals.Add(new Vector3D(1, 1, 1));
            }
            vectors.Add(new Vector3D(0, 0, 0));
            vectors.Add(new Vector3D(8, 0, 0));
            vectors.Add(new Vector3D(8, 8, 0));
            texcoords.Add(new Vector2D(0, 0));
            texcoords.Add(new Vector2D(1, 0));
            texcoords.Add(new Vector2D(1, 1));
            VertexBuffer tribuffer = renderer.CreateVertexBuffer(vectors.ToArray(), texcoords.ToArray(), normals.ToArray());
            tribuffer.DepthTesting = false;
            tribuffer.IsStatic = true;
            tribuffer.Draw();

            renderer.cameraPosition.Z = -25;
            renderer.cameraPosition.X = -16;
            //Primitives.createRectangle(renderer, 0, 0, 0, 10, 10).Draw();
            System.Threading.Thread mthread = new System.Threading.Thread(thetar);
            mthread.Start();
            
          
		}
        static VertexBuffer rotatingbuffer;
        static void thetar()
        {
            while (true)
            {
               
                System.Threading.Thread.Sleep(10);
            }
        }
        
    }
}
