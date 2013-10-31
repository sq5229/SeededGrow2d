using System;
using System.Collections.Generic;
using System.Text;

namespace SMC.SMC
{
class SMCProcessor
{
    BitMap3d bmp;
    public SMCProcessor(BitMap3d bitmap)
    {
        this.bmp = bitmap;
    }
    private byte GetConfig(ref Cube cube)
    {
        byte value = 0;
        for (int i = 0; i < 8; i++)
        {
            if (IsInside(cube.cubeImageIndices[i].X, cube.cubeImageIndices[i].Y, cube.cubeImageIndices[i].Z))
            {
                value |= Cube.PointIndexToFlag[i];
            }
        }
        return value;
    }//get copnfig
    private void ExtractTriangles(ref Cube cube, byte value, MeshBuilder_IntegerVertex builder)
    {
        if (SMCTable.TableFat[value, 0] != -1)
        {
            int index = 0;
            while (SMCTable.TableFat[value, index] != -1)
            {
                Int16Triple t0 = cube.cubeImageIndices[SMCTable.TableFat[value, index]];
                Int16Triple t1 = cube.cubeImageIndices[SMCTable.TableFat[value, index + 1]];
                Int16Triple t2 = cube.cubeImageIndices[SMCTable.TableFat[value, index + 2]];
                builder.AddTriangle(t0, t1,t2);
                //builder.AddTriangle(new Point3d(t0.X,t0.Y,t0.Z), new Point3d(t1.X,t1.Y,t1.Z), new Point3d(t2.X,t2.Y,t2.Z ));
                index += 3;
            }
        }
    }//extract triangles and put them into meshbuilder
    public Mesh GenerateSurface()
    {
        MeshBuilder_IntegerVertex builder = new MeshBuilder_IntegerVertex(bmp.width + 2, bmp.height + 2, bmp.depth + 2);// this class can build mesh from independent triangles
        for (int k = 0; k < bmp.depth - 1; k++)
        {
            for (int j = 0; j < bmp.height - 1; j++)
            {
                for (int i = 0; i < bmp.width-1; i++)
                {
                    Cube cell = new Cube(i, j, k);//builde Cube for Cell at i j k
                    byte config = GetConfig(ref cell);// get byte config for the cell
                    ExtractTriangles(ref cell, config, builder);// extract triangles from cell and push into
                }
            }
        }
        return builder.GetMesh();
    }
    public virtual bool IsInside(int x, int y, int z)
    {
        if (x <=0 || y <=0 || z <=0 || x > bmp.width || y > bmp.height || z > bmp.depth)
            return false;
        else
        {
            return bmp.GetPixel(x, y, z) == BitMap3d.WHITE;
        }
    }//judge if a voxel is inside the surface
}
    //class PointHashTable
    //{
    //    public PointHashTable(int width, int height, int depth)
    //    {

    //    }
    //    public void SetHashed(int x,int y,int z,int ei,int value)
    //    {

    //    }
    //    public bool GetHashed(int x, int y, int z, ref int value)
    //    {
    //        return false;
    //    }
    //}
    
}
