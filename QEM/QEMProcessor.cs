using System;
using System.Collections.Generic;
using System.Text;

namespace QEM
{
    public class QEMProcessor
    {
       
        Mesh mesh;
        ECMesh ecm;
        RAHeap heap;

        public QEMProcessor(Mesh m)
        {
            this.mesh = m;
            ecm=ECMesh.GetECMesh(m);
            heap=new RAHeap(ecm.Edges.Count);
        }

        public void ExecuteSimplification(float deciRate)
        {
            int targetFaceCount = (int)(mesh.Faces.Count * (1 - deciRate));
        }

        //private void 

    }
}
