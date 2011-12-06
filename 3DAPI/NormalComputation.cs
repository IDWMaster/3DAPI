/*
 * NOTICE: Math taken from: http://www.codeguru.com/cpp/g-m/opengl/article.php/c2681
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace _3DAPI
{
    public sealed class NormalComputation
    {
        class Triangle
        {
            public Vector3D vertex1;
            public Vector3D vertex2;
            public Vector3D vertex3;
            public Triangle(Vector3D v1, Vector3D v2, Vector3D v3)
            {
                vertex1 = v1;
                vertex2 = v2;
                vertex3 = v3;
            }
        }
        public static Vector3D[] ComputeNormals(Vector3D[] input)
        {
            //http://stackoverflow.com/questions/1777206/3d-surface-normal-extractor
            Vector3D[] normals = new Vector3D[input.Length];
            List<Triangle> faces = new List<Triangle>();
            int count = input.Length / 3;
            for (int i = 0; i < count; i++)
            {
                faces.Add(new Triangle(input[(i*3)], input[(i*3) + 1], input[(i*3) + 2]));
            }
            // loop over all faces to calculate vertex normals
            for (int i = 0; i < faces.Count; i++)
            {
                Vector3D v1 = faces[i].vertex1;
                Vector3D v2 = faces[i].vertex2;
                Vector3D v3 = faces[i].vertex3;

                Vector3D edge1 = v2 - v1;
                Vector3D edge2 = v3 - v1;
                Vector3D normal = edge1.CrossProduct(edge2);  // or edge2.CrossProduct(edge1)
                normal = normal.Normalize();

                normals[i] += normal;
                normals[i+1] += normal;
                normals[i+2] += normal;
            }

            // vertex normals need to be normalised
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = normals[i].Normalize();
            }
            return normals;
        }
        static Vector3D VectorGetNormal(Vector3D a, Vector3D b)
        {
            Vector3D output = new Vector3D();
            output.X = a.Y * b.Z - a.Z * b.Y;
            output.Y = a.Z * b.X - a.X * b.Z;
            output.Z = a.X * b.Y - a.Y * b.X;
            return output;
        }
        static float sqr(float inval)
        {
            return inval * inval;
        }
        static Vector3D VectorNormalize(Vector3D input)
        {
            float len = (float)Math.Sqrt(sqr(input.X) + sqr(input.Y) + sqr(input.Z));
            Vector3D output = new Vector3D();
            output.X = input.X / len;
            output.Y = input.Y / len;
            output.Z = input.Z / len;
            return output;
        }
        public static Vector3D ComputeFaceNormal(Vector3D p1, Vector3D p2, Vector3D p3)
        {
            Vector3D a = p3 - p2;
            Vector3D b = p1 - p2;
            //Compute cross product
            Vector3D pn = VectorGetNormal(a, b);
            return VectorNormalize(pn);
        }
    }
}
