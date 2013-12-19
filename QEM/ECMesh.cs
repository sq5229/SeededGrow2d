using System;
using System.Collections.Generic;
using System.Text;

namespace QEM
{
    public class ECMesh
    {
        public Box3Float Bounds;
        public int ValidVerticesCount;
        public int ValidFacesCount;
        public int ValidEdgesCount;
        public List<ECVertex> Vertices;
        public List<ECEdge> Edges;
        public List<ECFace> Faces;
        List<ECFace> temp;
        public ECMesh()
        {
            Bounds = new Box3Float();
            Vertices = new List<ECVertex>();
            Edges = new List<ECEdge>();
            Faces = new List<ECFace>();
            temp = new List<ECFace>();
            ValidFacesCount = 0;
            ValidVerticesCount = 0;
            ValidEdgesCount = 0;
        }
        public int AddVertex(Point3d p)
        {
            ECVertex v = CreateVertex(p.X, p.Y, p.Z);
            Bounds.UpdataRange(p.X, p.Y, p.Z);
            return Vertices.Count - 1;
        }
        public int AddFace(int p0i,int p1i,int p2i)
        {
            ECFace face = CreateFace(Vertices[p0i], Vertices[p1i], Vertices[p2i]);
            return Faces.Count - 1;
        }

        public ECVertex CreateVertex(float x,float y,float z)
        {
            ECVertex v = new ECVertex(x, y, z);
            Vertices.Add(v);
            v.UId = Vertices.Count - 1;
            ValidVerticesCount++;
            return v;
        }
        public ECEdge CreateEdge(ECVertex v1, ECVertex v2)
        {
            ECEdge e = new ECEdge(v1, v2);
            Edges.Add(e);
            e.UId = Edges.Count - 1;
            e.Twin().UId = e.UId;
            ValidEdgesCount++;
            return e;
        }
        public ECEdge GetEdge(ECVertex org,ECVertex v)
        {
            List<ECEdge> eu = org.AdjEdges;
            for (int i = 0; i < eu.Count; i++)
            {
                if (eu[i].DestV() == v)
                    return eu[i];
            }
            ECEdge e = CreateEdge(org, v);
            return e;
        }
        public ECFace CreateFace(ECVertex v1, ECVertex v2,ECVertex v3)
        {
            ECEdge e0 = this.GetEdge(v1, v2);
            ECEdge e1 = this.GetEdge(v2, v3);
            ECEdge e2 = this.GetEdge(v3, v1);
            ECFace f = new ECFace(e0, e1, e2);
            Faces.Add(f);
            f.UId = Faces.Count - 1;
            ValidFacesCount++;
            return f;
        }

        public void KillVertex(ECVertex v)
        {
            if (v.IsValid())
            {
                v.Kill();
                ValidVerticesCount--;
            }
        }
        public void KillEdge(ECEdge e)
        {
            if (e.IsValid())
            {
                e.Kill();
                ValidEdgesCount--;
            }
        }
        public void KillFace(ECFace face)
        {
            if (face.IsValid())
            {
                face.Kill();
                ValidFacesCount--;
            }
        }

        public void ReplaceVertex(ECVertex from, ECVertex to)
        {
            from.ReplaceBy(to);
        }
        public void RelocateVertex(ECVertex v, float x, float y, float z)
        {
            v.X = x;
            v.Y = y;
            v.Z = z;
        }
        public void MayFixFace(ECFace face)
        {
            ECVertex v0 = face.GetVertex(0);
            ECVertex v1 = face.GetVertex(1);
            ECVertex v2 = face.GetVertex(2);
            ECEdge e0 = face.GetEdge(0);
            ECEdge e1 = face.GetEdge(1);
            ECEdge e2 = face.GetEdge(2);
            bool a = (v0 == v1);
            bool b = (v0 == v2);
            bool c = (v1 == v2);
            if (a && c)
            {
                KillEdge(e0);
                KillEdge(e1);
                KillEdge(e2);
                KillFace(face);
            }
            else if (a)
            {
                KillEdge(e0);
                e1.ReplaceBy(e2.Twin());
                KillFace(face);
            }
            else if (b)
            {
                KillEdge(e2);
                e0.ReplaceBy(e1.Twin());
                KillFace(face);
            }
            else if (c)
            {
                KillEdge(e1);
                e0.ReplaceBy(e2.Twin());
                KillFace(face);
            }

        }
        public void RemoveDegenerateFaces(List<ECFace> tempFaces)
        {
            for (int i = 0; i < tempFaces.Count; i++)
            {
                MayFixFace(tempFaces[i]);
            }
        }
        public void ContractRegion(ECVertex v1, ECVertex v2, List<ECFace> tempList)
        {
            tempList.Clear();
            ECFace.UnTagFaceLoop(v1);
            ECFace.UnTagFaceLoop(v2);
            ECFace.CollectFaceLoop(v1, tempList);
            ECFace.CollectFaceLoop(v2, tempList);
        }
        public void Contract(ECVertex v1, ECVertex v2, Point3d to)
        {
            temp.Clear();
            ContractRegion(v1, v2, temp);
            RelocateVertex(v1, to.X, to.Y, to.Z);
            v2.ReplaceBy(v1);
            RemoveDegenerateFaces(temp);
        }

        public static ECMesh GetECMesh(Mesh m)
        {
            ECMesh ecm = new ECMesh();
            for (int i = 0; i < m.Vertices.Count; i++)
            {
                ecm.AddVertex(m.Vertices[i]);
            }
            for (int i = 0; i < m.Faces.Count; i++)
            {
                ecm.AddFace(m.Faces[i].P0Index, m.Faces[i].P1Index, m.Faces[i].P2Index);
            }
            ecm.InitTypes();
            return ecm;
        }

        public Mesh GetMesh()
        {
            Mesh m = new Mesh();
            int[] map=new int[Vertices.Count];
            for (int i = 0; i < Vertices.Count; i++)
                map[i] = -1;
            for (int i = 0; i < Vertices.Count; i++)
            {
                ECVertex v = Vertices[i];
                if (v.IsValid())
                {
                    map[i]=m.AddVertex(new Point3d(v.X, v.Y, v.Z));
                }
            }
            for (int i = 0; i < Faces.Count; i++)
            {
                ECFace f = Faces[i];
                if (f.IsValid())
                {
                    Triangle t=new Triangle(map[f.GetVertex(0).UId],map[f.GetVertex(1).UId],map[f.GetVertex(2).UId]);
                    m.AddFace(t);
                }
            }

            return m;
        }

        public void InitTypes()
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                VertexType type=ECVertex.ClassifyVertex(Vertices[i]);
                Vertices[i].Type = type;
            }
            for (int i = 0; i < Edges.Count; i++)
            {
                EdgeType type = ECEdge.ClassifyEdge(Edges[i]);
                Edges[i].Type = type;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("  Vertices :"+Vertices.Count+"\n");
            for (int i = 0; i < Vertices.Count; i++)
            {
                sb.Append(Vertices[i]+"\n");
            }
            sb.Append("  Edges:" + Edges.Count + "\n");
            for (int i = 0; i < Edges.Count; i++)
            {
                sb.Append(Edges[i] + "\n");
            }
            sb.Append("  Faces:" + Faces.Count + "\n");
            for (int i = 0; i < Faces.Count; i++)
            {
                sb.Append(Faces[i] + "\n");
            }
            return sb.ToString();
        }
    }
}
