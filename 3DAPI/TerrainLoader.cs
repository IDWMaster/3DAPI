using System;
using System.Collections.Generic;
using System.Drawing;
namespace _3DAPI
{
	public class TerrainLoader
	{
		
		public List<Vector3D> vertices = new List<Vector3D>();
			public List<Vector2D> texcoords = new List<Vector2D>();
			public List<Vector3D> normals = new List<Vector3D>();
			int ShortColor(Color mlor) {
		byte[] bgr = new byte[4];
			bgr[0] = mlor.B;
			bgr[1] = mlor.G;
			bgr[2] = mlor.R;
			return BitConverter.ToInt32(bgr,0);
			
			
		}
		int maxval() {
		return ShortColor(Color.White);
		}
		public TerrainLoader (Bitmap _terrain, float MAP_SCALE)
		{
			float[][][] terrain = new float[_terrain.Width][][];
			for(int i = 0;i<_terrain.Width;i++) {
			terrain[i] = new float[_terrain.Height][];
			
			}
			for(int i = 0;i<_terrain.Width;i++) {
			for(int it = 0;it<_terrain.Height;it++) {
				terrain[i][it] = new float[3];
				}
			}
			//BGRA
			for(float y = 0;y<_terrain.Height-1;y++) {
			for(float x = 0;x<_terrain.Width-1;x++) {
					Color mlor = _terrain.GetPixel((int)x,(int)y);
					int ix = (int)x;
					int iy = (int)y;
					terrain[ix][iy][0] = (x*MAP_SCALE)/(float)_terrain.Width;
					terrain[ix][iy][1] = (((float)ShortColor(mlor)/(float)maxval())*MAP_SCALE);
				    terrain[ix][iy][2] = (y*MAP_SCALE)/(float)_terrain.Height;
					
					
				}
				
			}
			
			
			for(int z = 0;z<_terrain.Height-1;z++) {
			for(int x = 0;x<_terrain.Width-1;x++) {
				//0
					normals.Add(new Vector3D(terrain[x][z][1]/255.0f, terrain[x][z][1], terrain[x][z][1]/255.0f));
			texcoords.Add(new Vector2D(0,0));
			vertices.Add(new Vector3D(terrain[x][z][0], terrain[x][z][1], terrain[x][z][2]));
				//1	
					normals.Add(new Vector3D(terrain[x+1][z][1]/255.0f, terrain[x+1][z][1], terrain[x+1][z][1]/255.0f));
			texcoords.Add(new Vector2D(1,0));
			vertices.Add(new Vector3D(terrain[x+1][z][0], terrain[x+1][z][1], terrain[x+1][z][2]));
					//2
					normals.Add(new Vector3D(terrain[x][z+1][1]/255.0f, terrain[x][z+1][1], terrain[x][z+1][1]/255.0f));
			texcoords.Add(new Vector2D(1,0));
			
			vertices.Add(new Vector3D(terrain[x][z+1][0], terrain[x][z+1][1], terrain[x][z+1][2]));
					//3
					normals.Add(new Vector3D(terrain[x+1][z+1][1]/255.0f, terrain[x+1][z+1][1], terrain[x][z+1][1]/255.0f));
			texcoords.Add(new Vector2D(1,1));
			vertices.Add(new Vector3D(terrain[x+1][z+1][0], terrain[x+1][z+1][1], terrain[x+1][z+1][2]));
				
				}
			}
			
			Console.WriteLine(vertices.Count);
			
			
		}
	}
}

