using System;
using System.Collections.Generic;
using System.Text;

namespace _3DAPI
{
    public sealed class BoundingBox
    {
        public static bool OptimizationEnabled = true;
        //TODO
        public Vector3D minX = new Vector3D(99999999999999, 99999999999999, 99999999999999);

        public Vector3D maxX = new Vector3D(-99999999999999, -99999999999999, -99999999999999);
        public Vector3D minY = new Vector3D(99999999999999, 99999999999999, 99999999999999);

        public Vector3D maxY = new Vector3D(-99999999999999, -99999999999999, -99999999999999);
        public Vector3D minZ = new Vector3D(99999999999999, 99999999999999, 99999999999999);

        public Vector3D maxZ = new Vector3D(-99999999999999, -99999999999999, -99999999999999);
        
        //End TODO



        /// <summary>
        /// Checks if a given point is within a bounding box
        /// </summary>
        /// <param name="point">The point to test</param>
        /// <returns></returns>
        public bool PointWithin(Vector3D _point)
        {
            if (!OptimizationEnabled)
            {
                return true;
            }
            
           // Vector3D training = new Vector3D(0, 0, 10.9f);
            Vector3D point = _point;// + training;
            if (point.X >= minX.X && point.Y >= minY.Y && point.Z >= minZ.Z && point.X <= maxX.X && point.Y <= maxY.Y && point.Z <= maxZ.Z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class CollisionTesting
    {
        public static BoundingBox createBoundingBox(Mesh mesh)
        {
            BoundingBox mbox = new BoundingBox();

            foreach (Vector3D et in mesh.meshverts)
            {
                if (et.X > mbox.maxX.X)
                {
                    mbox.maxX = et;
                }
                if (et.Y > mbox.maxY.Y)
                {
                    mbox.maxY = et;
                }
                if (et.Z > mbox.maxZ.Z)
                {
                    mbox.maxZ = et;
                }
                if (et.X < mbox.minX.X)
                {
                    mbox.minX = et;
                }
                if (et.Y < mbox.minY.Y)
                {
                    mbox.minY = et;
                }
                if (et.Z < mbox.minZ.Z)
                {
                    mbox.minZ = et;
                }


            }
          
            return mbox;
        }
    }
}
