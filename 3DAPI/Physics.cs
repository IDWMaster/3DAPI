using System;
using System.Collections.Generic;
using System.Text;

namespace _3DAPI
{
    public class MathHelpers
    {
        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180d);
        }
        public static float RadiansToDegrees(float radians)
        {
            return radians * (float)(180d / Math.PI);
        }
    }
    namespace Physics
    {
        public enum CollisionMode
        {
        Elastic, Inelastic
        }
        public enum CollisionType
        {
        Static, Dynamic
        }

        /// <summary>
        /// Represents a physical object in the world
        /// </summary>
        public class PhysicalObject
        {
            public CollisionMode mode = CollisionMode.Elastic;
            /// <summary>
            /// Sets the VBO that is owned by this object, or NULL
            /// </summary>
            public VertexBuffer ownedVBO = null;
            public float Distance(Vector3D point)
            {
                Vector3D dd = point - _position;
                return new Vector3D(Math.Abs(dd.X), Math.Abs(dd.Y), Math.Abs(dd.Z)).Magnitude;
            }
            Vector3D[][] interntriangles = null;
            /// <summary>
            /// The object embedded within this object during an inelastic collision
            /// </summary>
            internal PhysicalObject embeddedobject = null;
            /// <summary>
            /// Creates a new physical object
            /// </summary>
            /// <param name="Vertices">The vertices within the object</param>
            /// <param name="weight">The weight of the object</param>
            /// <param name="associatedWorld">The world to associated this object with. This DOES NOT add it to the world, but simply references it.</param>
            public PhysicalObject(Vector3D[] Vertices, float weight, CollisionType ctype, World associatedWorld)
            {
                Weight = weight;
                collisiontype = ctype;
                origverts = Vertices.Clone() as Vector3D[];
                internverts = Vertices.Clone() as Vector3D[];
                
                internworld = associatedWorld;
                List<Vector3D[]> Triangles = new List<Vector3D[]>();
                int tricount = internverts.Length / 3;
                int x = 0;
                for (int i = 0; i < tricount; i++)
                {

                    Vector3D[] triangle = new Vector3D[] { internverts[x], internverts[x + 1], internverts[x + 2] };
                    Triangles.Add(triangle);
                    x += 3;

                }
                interntriangles = Triangles.ToArray();
                Mesh mmesh = new Mesh();
				mmesh.meshverts = internverts;
                boundingBox = CollisionTesting.createBoundingBox(mmesh);
            }
            World internworld;
            object syncobj = new object();
            Vector3D _position = new Vector3D();
            public Vector3D Position
            {
                get
                {
                    
                    return _position;
                }
                set
                {
                    lock (syncobj)
                    {
                        
                        //Transform all vertices by the specified amount
                        internverts = origverts.Clone() as Vector3D[];
                        for (int i = 0; i < internverts.Length; i++)
                        {

                            internverts[i] += value;
                            
                        }
                        Vector3D changed = Position-value;
                        if (interntriangles != null)
                        {
                            for (int i = 0; i < interntriangles.Length; i++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    interntriangles[i][x] += changed;
                                }
                            }
                        }
                        Mesh mmesh = new Mesh();
						mmesh.meshverts = internverts;
                        boundingBox = CollisionTesting.createBoundingBox(mmesh);
                        _position = value;
                        if (ownedVBO != null)
                        {
                            ownedVBO.Position = _position;
                        }
                    }
                }
            }
            Vector3D _rotation;
            //TODO: I'm no math genuis, but I know this rotation algorithm doesn't work....
            public Vector3D Rotation
            {
                get
                {
                    return _rotation;
                }
                set
                {
                    
                    Vector3D changed = _rotation - value;
                    changed = new Vector3D(MathHelpers.DegreesToRadians(changed.X), MathHelpers.DegreesToRadians(changed.Y), MathHelpers.DegreesToRadians(changed.Z));
                    for (int i = 0; i < internverts.Length; i++)
                    {
                        Vector3D prevval = internverts[i];
                        //First, apply X rotation
                        internverts[i].Y = (float)(prevval.Y*Math.Cos(changed.X)-prevval.Z*Math.Sin(changed.X));
                        internverts[i].Z = (float)(prevval.Y*Math.Sin(changed.X)+prevval.Z*Math.Cos(changed.X));
                        //Now, apply Y rotation
                        prevval = internverts[i];
                        internverts[i].X = (float)(prevval.X*Math.Cos(changed.Y) + prevval.Z*Math.Sin(changed.Y));
                        internverts[i].Z = (float)(-prevval.X*Math.Sin(changed.Y) + prevval.Z*Math.Cos(changed.Y));
                        //Now, apply Z rotation
                        prevval = internverts[i];
                        internverts[i].X = (float)(prevval.X*Math.Cos(changed.Z)-prevval.Y*Math.Sin(changed.Z));
                        internverts[i].Y = (float)(prevval.X*Math.Sin(changed.Z)+prevval.Y*Math.Cos(changed.Z));
                        

                    }
					Mesh mmesh = new Mesh();
					mmesh.meshverts = internverts;
                    boundingBox = CollisionTesting.createBoundingBox(mmesh);
					
                   // throw new NotImplementedException("TODO: Implement this");
                    _rotation = value;
                }
            }
            public Vector3D Velocity = new Vector3D();
            bool MightIntersect(PhysicalObject pobject)
            {

                if (boundingBox.PointWithin(pobject.boundingBox.maxX) || boundingBox.PointWithin(pobject.boundingBox.maxY) || boundingBox.PointWithin(pobject.boundingBox.maxZ) || boundingBox.PointWithin(pobject.boundingBox.minX) || boundingBox.PointWithin(pobject.boundingBox.minY) || boundingBox.PointWithin(pobject.boundingBox.minZ) || boundingBox.PointWithin(pobject.boundingBox.minX.Center(pobject.boundingBox.maxX)) || boundingBox.PointWithin(pobject.boundingBox.minY.Center(pobject.boundingBox.maxY)) ||boundingBox.PointWithin(pobject.boundingBox.minZ.Center(pobject.boundingBox.maxZ)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Indicates whether or not this object is a cube. Set to TRUE to optimize collisions with this object using AABB
            /// </summary>
            public bool IsCube = false;
            bool testCollisionTriangle(Vector3D[] verti, Vector3D coord)
            {
                
                Vector3D max = new Vector3D(-999999999, -999999999, -999999999);
                Vector3D min = new Vector3D(999999999, 999999999, 999999999);

                foreach (Vector3D et in verti)
                {
                    ObjVertex e = new ObjVertex();
                    e.Vertex.X = et.X * -1;
                    e.Vertex.Y = et.Y * -1;
                    e.Vertex.Z = et.Z * -1;
                    if (e.Vertex.X < min.X)
                    {
                        min.X = e.Vertex.X;
                    }
                    if (e.Vertex.X > max.X)
                    {
                        max.X = e.Vertex.X;
                    }
                    if (e.Vertex.Y < min.Y)
                    {
                        min.Y = e.Vertex.Y;
                    }
                    if (e.Vertex.Y > max.Y)
                    {
                        max.Y = e.Vertex.Y;
                    }
                    if (e.Vertex.Z > max.Z)
                    {
                        max.Z = e.Vertex.Z;
                    }
                    if (e.Vertex.Z < min.Z)
                    {
                        min.Z = e.Vertex.Z;
                    }

                }
                Vector3D sval = new Vector3D(.9f, .9f, .9f);


                min -= sval;
                max += sval;

                if (coord.X < max.X & coord.X > min.X & coord.Y > min.Y & coord.Y < max.Y & coord.Z > min.Z & coord.Z < max.Z)
                {
                    return true;

                }
                else
                {


                    return false;
                }
            }
           
            public bool Contains(PhysicalObject pobject)
            {
                if (MightIntersect(pobject))
                {
                    if (internworld.testtype == CollisionTestType.AABB || IsCube)
                    {
                        return true;
                    }
                    
                    foreach (Vector3D vert in pobject.internverts)
                    {
                        if (Contains(vert))
                        {
                            return true;
                        }
                    }
                    
                }
                return false;
            }
           
            public bool Contains(Vector3D point)
            {
                //Create list of triangles
                
                List<Vector3D[]> Triangles = new List<Vector3D[]>();
                if (interntriangles == null)
                {
                    int tricount = internverts.Length / 3;
                    int x = 0;
                    for (int i = 0; i < tricount; i++)
                    {

                        Vector3D[] triangle = new Vector3D[] { internverts[x], internverts[x + 1], internverts[x + 2] };
                        Triangles.Add(triangle);
                        x += 3;

                    }
                    interntriangles = Triangles.ToArray();
                }
                if (!boundingBox.PointWithin(point))
                {
                    return false;
                }
                if (internworld.testtype == CollisionTestType.AABB || IsCube)
                {
                    return true;
                }
                foreach (Vector3D[] et in interntriangles)
                {
                    if (testCollisionTriangle(et, point))
                    {
                        
                        return true;
                    }
                }
                return false;
            }
         
            public BoundingBox boundingBox;
            Vector3D[] internverts;
            Vector3D[] origverts;
            public float Weight;
            public CollisionType collisiontype;
        }
        public enum CollisionTestType
        {
        AABB,Precision
        }
        /// <summary>
        /// Represents a VM in which physical interactions happen quite frequently. In fact; pretty much all the time.
        /// </summary>
        public class World
        {
            public CollisionTestType testtype = CollisionTestType.Precision;
            /// <summary>
            /// Creates a new world
            /// </summary>
            public World()
            {
               
            }
            public List<PhysicalObject> physicalobjects = new List<PhysicalObject>();
            /// <summary>
            /// Halts all physics calculations
            /// </summary>
            public void Stop()
            {
                isrunning = true;

            }
            /// <summary>
            /// Starts the physics calculations
            /// </summary>
            public void Start()
            {
                Console.WriteLine("Running simulation with " + physicalobjects.Count.ToString() + " objects");
                isrunning = true;
                System.Threading.Thread mthread = new System.Threading.Thread(thetar);
                mthread.Start();
            }
            /// <summary>
            /// The rate (in milliseconds) at which the timer ticks
            /// </summary>
            public int simulationinterval = 10;
            bool isrunning = false;
            public float multiplier = 10;
            public event System.Threading.ThreadStart physicsUpdateFrame;
            void thetar()
            {
                while (isrunning)
                {
                    if (physicsUpdateFrame != null)
                    {
                        physicsUpdateFrame.Invoke();
                    }
                    lock (physicalobjects)
                    {
                        //Calculate object velocity, based on gravity of surrounding objects
                        foreach (PhysicalObject et in physicalobjects)
                        {
                            if (et.collisiontype == CollisionType.Dynamic)
                            {
                                foreach (PhysicalObject ett in physicalobjects)
                                {

                                    if (ett != et & ! ett.Contains(et))
                                    {
                                        
                                        Vector3D direction = (ett.Position - et.Position).Normalize();
                                        float magnitude = (6.67f * (float)Math.Pow(10, -11)) * ((et.Weight * ett.Weight) / (et.Distance(ett.Position) * et.Distance(ett.Position)));
                                        if (magnitude == float.PositiveInfinity || float.IsNaN(magnitude))
                                        {
                                        }
                                        else
                                        {
                                            Vector3D nav = (direction * (magnitude / et.Weight)) * multiplier;
                                            et.Velocity += nav;
                                        }
                                    }
                                }
                            }
                        }

                        foreach (PhysicalObject et in physicalobjects)
                        {
                            Vector3D prevpos = et.Position;
                            
                            et.Position += et.Velocity;
                            foreach (PhysicalObject ett in physicalobjects)
                            {
                                
                                if (ett != et)
                                {
                                    
                                    if (ett.Contains(et))
                                    {
										et.Position = prevpos;
                                   
                                        if (ett.collisiontype == CollisionType.Static)
                                        {
                                            et.Position = prevpos;
                                        }
                                        else
                                        {
                                           // et.Position = prevpos;
                                          //TODO: Finish this part!!!!
                                            //Inelastic collision formula
                                       
                                            //(m1+m2)v = m1*v1+m2*v2
                                            //Elastic collision formula
                                            //Conservation of momentum
                                            //m1*u1+m2*u2 = m1*v1+m2*v2
                                            //Where m is mass, U is velocity before collision, v is velocity after collision
                                            if (ett.mode == CollisionMode.Elastic)
                                            {
                                                Vector3D u1 = et.Velocity;
                                                Vector3D u2 = ett.Velocity;
                                                float m1 = et.Weight;
                                                float m2 = ett.Weight;
                                                Vector3D v1 = (u1 * (m1 - m2) + 2 * m2 * u2) / (m1 + m2);
                                                Vector3D v2 = (u2 * (m2 - m1) + 2 * m1 * u1) / (m1 + m2);
                                                et.Velocity = v1;
                                                ett.Velocity = v2;
                                            }
                                            else
                                            {
                                                if (et.embeddedobject != ett)
                                                {

                                                    Vector3D u1 = et.Velocity;
                                                    Vector3D u2 = ett.Velocity;
                                                    float m1 = et.Weight;
                                                    float m2 = ett.Weight;
                                                    //(m1+m2)v = m1*v1+m2*v2
                                                    //v = (m1*v1+m2*v2)/(m1+m2)
                                                    Vector3D v = ((m1 * u1) + (m2 * u2)) / (m1 + m2);
                                                    et.Velocity = v;
                                 
                                                  //  ett.Velocity = v;
                                                  
                                                }
                                            }
                                        }
                                        
                                }
                                }
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(simulationinterval);
                }
            }
        }
        
    }
}
