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
        static PhysicalObject collisiontester;
        static World physicsworld = new World();
        static bool flip = false;
        static Texture2D rtex;
       
        public RealProg(Renderer _renderer)
        {

            renderer = _renderer;
            renderer.cameraPosition.Z = -15;
            
            Shader myder = renderer.createBasicShader();

            //Uncomment for AABB collisions

            // physicsworld.testtype = CollisionTestType.AABB;
            myder.Draw();

            Bitmap mmap = new Bitmap("pic.jpg");
            Texture2D mtex = renderer.createTextureFromBitmap(mmap);
            mtex.Draw();
            rtex = renderer.createTexture(512, 512);

            rtex.Draw();
            Mesh[] meshes = Primitives.LoadMesh("playercube.obj", flip);

            collisiontester = new PhysicalObject(meshes[0].meshverts, 5, CollisionType.Dynamic, physicsworld);
            collisiontester.Position = new Vector3D(-5, 0, 0);

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
            mtex.Draw();
            Mesh cube = Primitives.LoadMesh("playercube.obj", flip)[0];

            theobject = new PhysicalObject(cube.meshverts, 9, CollisionType.Dynamic, physicsworld);


            collisiontester.ownedVBO = rotatingbuffer;
            theobject.ownedVBO = renderer.CreateVertexBuffer(cube.meshverts, cube.meshtexas, cube.meshnorms);

            theobject.ownedVBO.Draw();



            physicsworld.physicalobjects.Add(theobject);
            physicsworld.physicalobjects.Add(collisiontester);

            Mesh anothercube = Primitives.LoadMesh("playercube.obj", flip)[0];
            PhysicalObject mobject = new PhysicalObject(anothercube.meshverts.Clone() as Vector3D[], 1, CollisionType.Dynamic, physicsworld);
            mobject.ownedVBO = renderer.CreateVertexBuffer(anothercube.meshverts, anothercube.meshtexas, anothercube.meshnorms);
            physicsworld.physicalobjects.Add(mobject);
            mobject.ownedVBO.Draw();
            mobject.Position = new Vector3D(30, 0, 0);
            //Set physics properties

            theobject.Weight = 1;

            collisiontester.Weight = 1;

            //collisiontester.Velocity = new Vector3D(.05f, 0, 0);
            theobject.Position = new Vector3D(15, 0, 0);
            theobject.IsCube = true;
            theobject.Weight = 9999999;
            mobject.IsCube = true;

            //End physics properties

            physicsworld.Start();

            physicsworld.physicsUpdateFrame += new System.Threading.ThreadStart(physicsworld_physicsUpdateFrame);

            System.Threading.Thread mthread = new System.Threading.Thread(thetar);
            mthread.Start();
#if PHONE
            renderer.defaultTouchpad.onTouchMoved += new TouchEvent(defaultTouchpad_onTouchMoved);
            renderer.defaultTouchpad.onTouchFound += new TouchEvent(defaultTouchpad_onTouchFound);
            renderer.defaultTouchpad.onTouchLost += new TouchEvent(defaultTouchpad_onTouchLost);
#endif
#if !PHONE
            renderer.defaultKeyboard.onKeyDown += new keyboardeventargs(defaultKeyboard_onKeyDown);
            renderer.defaultKeyboard.onKeyUp += new keyboardeventargs(defaultKeyboard_onKeyUp);
            renderer.defaultMouse.onMouseMove += new mouseEvent(defaultMouse_onMouseMove);
            renderer.defaultMouse.onMouseDown += new mouseEvent(defaultMouse_onMouseDown);
#endif
        }

        void defaultTouchpad_onTouchLost(int touchpoint, int x, int y)
        {
         
               touches.Remove(touchpoint);
           
            if (touches.Count == 0)
            {
                left = false;
                right = false;
                up = false;
                down = false;
            }
        }

        void defaultTouchpad_onTouchFound(int touchpoint, int x, int y)
        {

            touches.Add(touchpoint);
            if (touches.Count == 2)
            {
                defaultMouse_onMouseDown(MouseButton.Right, 0, 0);
            }
        }
        List<int> touches = new List<int>();
        void defaultTouchpad_onTouchMoved(int touchpoint, int x, int y)
        {
            
                if (x > 120)
                {
                    left = true;
                }
                else
                {
                    left = false;
                }
                if (x < 50)
                {
                    right = true;

                }
                else
                {
                    right = false;
                }
                if (y > 120)
                {
                    down = true;
                }
                else
                {
                    down = false;
                }
                if (y < 50)
                {
                    up = true;
                }
                else
                {
                    up = false;
                }
            
        }

        static void physicsworld_physicsUpdateFrame()
        {

            renderer.SetRenderTarget(rtex, collisiontester.Position, new Vector3D(180, 0, 0));
        }

        static void defaultMouse_onMouseDown(MouseButton btn, int x, int y)
        {
            lock (physicsworld.physicalobjects)
            {
                Mesh tmesh = Primitives.LoadMesh("playercube.obj", flip)[0];
                PhysicalObject mobject = new PhysicalObject(tmesh.meshverts, 1, CollisionType.Dynamic, physicsworld);
                mobject.IsCube = true;
                mobject.ownedVBO = renderer.CreateVertexBuffer(tmesh.meshverts, tmesh.meshtexas, tmesh.meshnorms);
                mobject.ownedVBO.Draw();

                physicsworld.physicalobjects.Add(mobject);
                mobject.Position = renderer.cameraPosition;
                if (btn == MouseButton.Left)
                {

                    mobject.Velocity = Vector3D.ComputeRotation(renderer.worldRotation);

                }
                else
                {

                    mobject.Weight = 3f;
                }

            }
        }
        static Vector2D prevval = new Vector2D(0, 0);
        static bool move = false;
        static void defaultMouse_onMouseMove(MouseButton btn, int x, int y)
        {
            if (prevval.X == 0)
            {
                prevval.X = x;
                prevval.Y = y;
            }
            if (move)
            {
                renderer.worldRotation.Y += (prevval.X - x) / 10;
                renderer.worldRotation.X += (prevval.Y - y) / 10;

                //Windows Phone doesn't support setting the cursor position
#if !PHONE
                System.Windows.Forms.Cursor.Position = new Point((int)prevval.X, (int)prevval.Y);
#endif

                move = false;
            }
            else
            {
                move = true;
            }
        }
        static bool ctrl = false;
        static void defaultKeyboard_onKeyUp(string KeyName)
        {
            if (KeyName.ToLower().Contains("control"))
            {
                ctrl = false;
            }
            if (KeyName.ToLower().Contains("up"))
            {

                up = false;
            }
            if (KeyName.ToLower().Contains("down"))
            {
                down = false;
            }
            if (KeyName.ToLower().Contains("left"))
            {
                left = false;
            }
            if (KeyName.ToLower().Contains("right"))
            {
                right = false;
            }

        }

        static void defaultKeyboard_onKeyDown(string KeyName)
        {
            if (KeyName.ToLower().Contains("control"))
            {
                ctrl = true;
            }
            if (!ctrl)
            {
                if (KeyName.ToLower().Contains("up"))
                {

                    up = true;
                }
                if (KeyName.ToLower().Contains("down"))
                {
                    down = true;
                }
                if (KeyName.ToLower().Contains("left"))
                {
                    left = true;
                }
                if (KeyName.ToLower().Contains("right"))
                {
                    right = true;
                }
                if (KeyName.ToLower().Contains("f10"))
                {
                    renderer.Dispose();
                    isrunning = false;
                }
            }

        }
        static Mesh mainmesh;
        static bool isrunning = true;
        static Renderer renderer;
        static bool left = false;
        static bool right = false;
        static bool up = false;
        static bool down = false;
        static void thetar()
        {
            renderer.cameraPosition.Z -= 5f;

            while (isrunning)
            {
                //theobject.ownedVBO.rotation.X += .01f;
                Vector3D prevpos = renderer.cameraPosition;
                //rotatingbuffer.rotation.Y = MathHelpers.DegreesToRadians(-90);
                //rotatingbuffer.rotation.Y += .01f;
                Vector3D cameraPosition = new Vector3D(0, 0, 0);

                Vector3D direction = Vector3D.ComputeRotation(new Vector3D(renderer.worldRotation.X, renderer.worldRotation.Y, renderer.worldRotation.Z));
                direction.Y *= -1;
                Vector3D direction2 = Vector3D.ComputeRotation(new Vector3D(renderer.worldRotation.X, renderer.worldRotation.Y - 90, renderer.worldRotation.Z));

                if (left)
                {

                    cameraPosition.X -= .1f * direction2.X;
                    cameraPosition.Z -= .1f * direction2.Z;
                }
                if (right)
                {
                    cameraPosition.X += .1f * direction2.X;
                    cameraPosition.Z += .1f * direction2.Z;
                }
                if (up)
                {
                    cameraPosition.Z += .1f * direction.Z;
                    cameraPosition.X += .1f * direction.X;
                }
                if (down)
                {
                    cameraPosition.Z -= .1f * direction.Z;
                    cameraPosition.X -= .1f * direction.X;
                }

                // collisiontester.Rotation = rotatingbuffer.rotation;
                renderer.cameraPosition += cameraPosition;
                if (collisiontester.Contains(renderer.cameraPosition))
                {
                    renderer.cameraPosition = prevpos;
                }

                System.Threading.Thread.Sleep(10);
            }
        }
        static PhysicalObject theobject;
        static VertexBuffer rotatingbuffer;
    }
}
