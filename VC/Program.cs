using System;
using System.Collections.Generic;
using System.Text;

namespace VC
{
    class Program
    {
        static void Main(string[] args)
        {
            Mesh m = new Mesh();
            PlyManager.ReadMeshFile(m,@"D:\VTKproj\engine.ply");
            VertexCluster vc = new VertexCluster(m);
            vc.ExecuteSimplification(1.5f);
            PlyManager.Output(m, @"D:\VTKproj\engine_vc.ply");

        }
    }
}
