using System;
using System.Collections.Generic;
using System.Text;

namespace QEM
{
    public enum VertexType
    {
        INTERIOR = 0,
        BORDER = 1,
        BORDER_ONLY = 2
    }
    public enum EdgeType
    {
        BOGUS = 0, BORDER = 1, MANIFOLD = 2, NONMANIFOLD = 3
    }
    public struct Plane
    {
        public float A;
        public float B;
        public float C;
        public float D;
        public Plane(ECVertex v0, ECVertex v1, ECVertex v2)
        {
            A = 0; B = 0; C = 0; D = 0; isValid = true;
            CaculateFrom3Verts(v0, v1, v2);
        }
        public bool isValid;
        public void CaculateFrom3Verts(ECVertex v0, ECVertex v1, ECVertex v2)
        {
            A = 0;
            B = 0;
            C = 0;
            D = 0;
        }
    }

    public class ECVertex
    {
        public float X;
        public float Y;
        public float Z;
        public int UId;
        public List<ECEdge> AdjEdges;
        public VertexType Type;
        public ECVertex(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.AdjEdges = new List<ECEdge>(6);
        }
        public bool IsValid()
        {
            return UId >= 0;
        }
        public void MarkInvalid()
        {
            if (UId >= 0)
                UId = -UId - 1;
        }
        public void Kill()
        {
            MarkInvalid();
            AdjEdges.Clear();
        }
        public void LinkEdge(ECEdge edge)
        {
            AdjEdges.Add(edge);
        }
        public void UnlinkEdge(ECEdge edge)
        {
            AdjEdges.Remove(edge);
            if (AdjEdges.Count == 0)
                Kill();
        }
        public void ReplaceBy(ECVertex v)
        {
            if (v != this)
            {
                for (int i = 0; i < AdjEdges.Count; i++)
                {
                    AdjEdges[i].ReplacePoint(this, v);
                }
                Kill();
            }
        }
        public static VertexType ClassifyVertex(ECVertex v)
        {
            int count = 0;
            for (int i = 0; i < v.AdjEdges.Count; i++)
            {
                if (ECEdge.ClassifyEdge(v.AdjEdges[i]) == EdgeType.BORDER)
                    count++;
            }
            if (count == v.AdjEdges.Count)
                return VertexType.BORDER_ONLY;
            else if (count == 0)
                return VertexType.INTERIOR;
            else if (count > 0)
                return VertexType.BORDER;
            return VertexType.BORDER_ONLY;
        }
        public override string ToString()
        {
            StringBuilder sb=new StringBuilder();
            for(int i=0;i<AdjEdges.Count;i++)
            {
                sb.Append(AdjEdges[i]+",");
            }
            return string.Format("[{0},{1},{2}] edges{3}:{4} id:{5}",X,Y,Z,AdjEdges.Count,sb.ToString(),UId);
        }
    }
    public class ECEdge
    {
        public int UId;
        public EdgeType Type;
        ECVertex v1;
        public List<ECFace> AdjFace;
        ECEdge twin;
        public ECEdge(ECVertex v1, ECVertex v2)
        {
            this.v1 = v1;
            v1.LinkEdge(this);
            AdjFace = new List<ECFace>(2);
            twin = new ECEdge(this, v2);
        }
        private ECEdge(ECEdge twin, ECVertex org)
        {
            this.v1 = org;
            this.v1.LinkEdge(this);
            this.AdjFace = twin.AdjFace;
            this.twin = twin;
        }
        public ECVertex OrgV()
        {
            return v1;
        }
        public ECVertex DestV()
        {
            return twin.OrgV();
        }
        public ECEdge Twin()
        {
            return twin;
        }

        public bool IsValid()
        {
            return UId >= 0;
        }
        public void MarkInvalid()
        {
            if (UId >= 0)
                UId = -UId - 1;
        }

        public void Kill()
        {
            if (IsValid())
            {
                OrgV().UnlinkEdge(this);
                DestV().UnlinkEdge(Twin());
                MarkInvalid();
                twin.MarkInvalid();
                AdjFace.Clear();
            }
        }
        public void LinkFace(ECFace face)
        {
            this.AdjFace.Add(face);
        }
        public void UnlinkFace(ECFace face)
        {
            this.AdjFace.Remove(face);
            if (AdjFace.Count == 0)
                Kill();
        }
        public void ReplacePoint(ECVertex from, ECVertex to)
        {
            if (OrgV() == from)
            {
                v1 = to;
                to.LinkEdge(this);
            }
            else if (DestV() == from)
            {
                twin.v1 = to;
                to.LinkEdge(twin);
            }
            else
                throw new Exception();
            for (int i = 0; i < AdjFace.Count; i++)
            {
                AdjFace[i].MarkPlaneInvalid();
            }
        }
        public void ReplaceBy(ECEdge edge)
        {
            if (edge != this)
            {
                for (int i = 0; i < AdjFace.Count; i++)
                {
                    AdjFace[i].ReplaceEdge(this, edge);
                }
                Kill();
            }
        }

        public static EdgeType ClassifyEdge(ECEdge e)
        {
            int c = e.AdjFace.Count;
            if (c >= 3)
                return EdgeType.NONMANIFOLD;
            if (c == 2)
                return EdgeType.MANIFOLD;
            if (c == 1)
                return EdgeType.BORDER;
            return EdgeType.BOGUS;

        }

        public override string ToString()
        {
            return string.Format("e{0}to{1}",OrgV().UId,DestV().UId);
        }
    }
    public class ECFace
    {
        public int UId;
        bool tag;
        Plane P;
        ECEdge[] AdjEdges;
        public ECFace(ECEdge e0, ECEdge e1, ECEdge e2)
        {
            AdjEdges = new ECEdge[3];
            AdjEdges[0] = e0;
            AdjEdges[1] = e1;
            AdjEdges[2] = e2;
            AdjEdges[0].LinkFace(this);
            AdjEdges[1].LinkFace(this);
            AdjEdges[2].LinkFace(this);
            P = new Plane(e0.OrgV(), e1.OrgV(), e2.OrgV());
            tag = false;
        }
        public bool IsValid()
        {
            return UId >= 0;
        }
        public void MarkInvalid()
        {
            if (UId >= 0)
                UId = -UId - 1;
        }
        public void MarkPlaneInvalid()
        {
            P.isValid = false;
        }
        public void Tag()
        {
            tag = true;
        }
        public void UnTag()
        {
            tag = false;
        }
        public bool GetTag()
        {
            return tag;
        }
        public Plane GetPlane()
        {
            if (!P.isValid)
                P.CaculateFrom3Verts(GetVertex(0), GetVertex(1), GetVertex(2));
            return P;
        }
        public ECVertex GetVertex(int index)
        {
            return AdjEdges[index].OrgV();
        }
        public ECEdge GetEdge(int index)
        {
            return AdjEdges[index];
        }
        public void Kill()
        {
            if (IsValid())
            {
                if (AdjEdges[0].IsValid())
                    AdjEdges[0].UnlinkFace(this);
                if (AdjEdges[1].IsValid())
                    AdjEdges[1].UnlinkFace(this);
                if (AdjEdges[2].IsValid())
                    AdjEdges[2].UnlinkFace(this);
                MarkInvalid();
            }
        }
        public void ReplaceEdge(ECEdge from, ECEdge to)
        {
            for (int i = 0; i < 3; i++)
            {
                if (AdjEdges[i] == from)
                {
                    AdjEdges[i] = to;
                    to.LinkFace(this);
                }
                else if (AdjEdges[i] == from.Twin())
                {
                    AdjEdges[i] = to.Twin();
                    to.Twin().LinkFace(this);
                }
            }
            MarkPlaneInvalid();
        }

        public static void UnTagFaceLoop(ECVertex v)
        {
            for (int j = 0; j < v.AdjEdges.Count; j++)
            {
                List<ECFace> faces = v.AdjEdges[j].AdjFace;
                for (int k = 0; k < faces.Count; k++)
                {
                    faces[k].UnTag();
                }
            }
        }
        public static void CollectFaceLoop(ECVertex v, List<ECFace> loop)
        {
            for (int j = 0; j < v.AdjEdges.Count; j++)
            {
                List<ECFace> faces = v.AdjEdges[j].AdjFace;
                for (int k = 0; k < faces.Count; k++)
                {
                    if (!faces[k].GetTag())
                    {
                        loop.Add(faces[k]);
                        faces[k].Tag();
                    }
                }
            }
        }
        public override string ToString()
        {
            return string.Format("[{0},{1},{2}]", AdjEdges[0].OrgV().UId, AdjEdges[1].OrgV().UId, AdjEdges[2].OrgV().UId);
        }
    }
}
