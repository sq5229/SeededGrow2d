#include <stdio.h>
#include "Mesh.h"
#include "IOPly.h"
#include "GeodeticCalculator.h"


std::vector<int> GetStartPointIndex(Mesh& m)
{
	std::vector<int> temp;
	for(size_t i=0;i<m.Vertices.size();i++)
	{
		if(m.Vcolors[i].R==0&&m.Vcolors[i].G==0&&m.Vcolors[i].B==255)
		{
			temp.push_back(i);
		}
	}
	return temp;
}
std::vector<int> GetEndPointIndex(Mesh& m)
{
	std::vector<int> temp;
	for(size_t i=0;i<m.Vertices.size();i++)
	{
		if(m.Vcolors[i].R==255&&m.Vcolors[i].G==0&&m.Vcolors[i].B==0)
		{
			temp.push_back(i);
		}
	}
	return temp;
}

int main1()
{
	Mesh m;
	PlyManager::ReadFileEx(m,"sampleMapC2.ply");
	printf("vcount:%d ,fcount:%d\n",m.Vertices.size(),m.Faces.size());
	Box3Float box=m.GetBox3();
	printf("box: [%f,%f,%f][%f,%f,%f]\n",box.Min3[0],box.Min3[1],box.Min3[2],box.Max3[0],box.Max3[1],box.Max3[2]);
	std::vector<int> stindexs=GetStartPointIndex(m);
	std::vector<int> edindexs=GetEndPointIndex(m);
	printf("st:%d ,ed:%d\n",stindexs.size(),edindexs.size());
	int stindex=stindexs[0];
	Point3d stp=m.Vertices[stindex];
	int edindex=edindexs[0];
	Point3d edp=m.Vertices[edindex];
	printf("stp:%f,%f,%f ,ed:%f,%f,%f\n",stp.X,stp.Y,stp.Z,edp.X,edp.Y,edp.Z);
	m.CaculateAdjacentVerticesPerVertex();
	GeodeticCalculator_AStar gc(m,stindex,edindex);
	gc.Execute();
	std::vector<bool>& visited=gc.GetVisitedFlags();
	for(int i=0;i<m.Vertices.size();i++)
	{
		Color c=m.Vcolors[i];
		if(visited[i]&&c.R==255&&c.G==255&&c.B==255)
		{
			m.Vcolors[i].R=0;
			m.Vcolors[i].G=255;
			m.Vcolors[i].B=0;
		}
	}
	std::vector<int>& path=gc.GetPath();
	for(int i=0;i<path.size();i++)
	{
		int index=path[i];
		m.Vcolors[index].R=0;
		m.Vcolors[index].G=0;
		m.Vcolors[index].B=0;
	}
	printf("path length:%.2f , visited nodes: %d\n",gc.PathLength(),gc.VisitedNodeCount());
	PlyManager::OutputEx(m,"out_as.ply");
	return 0;
}

int main2()
{
	Mesh m;
	PlyManager::ReadFileEx(m,"sampleMapC2.ply");
	printf("vcount:%d ,fcount:%d\n",m.Vertices.size(),m.Faces.size());
	Box3Float box=m.GetBox3();
	printf("box: [%f,%f,%f][%f,%f,%f]\n",box.Min3[0],box.Min3[1],box.Min3[2],box.Max3[0],box.Max3[1],box.Max3[2]);
	std::vector<int> stindexs=GetStartPointIndex(m);
	std::vector<int> edindexs=GetEndPointIndex(m);
	printf("st:%d ,ed:%d\n",stindexs.size(),edindexs.size());
	int stindex=stindexs[0];
	Point3d stp=m.Vertices[stindex];
	int edindex=edindexs[0];
	Point3d edp=m.Vertices[edindex];
	printf("stp:%f,%f,%f ,ed:%f,%f,%f\n",stp.X,stp.Y,stp.Z,edp.X,edp.Y,edp.Z);
	m.CaculateAdjacentVerticesPerVertex();
	GeodeticCalculator_Dijk gc(m,stindex,edindex);
	gc.Execute();
	std::vector<bool>& visited=gc.GetVisitedFlags();
	for(int i=0;i<m.Vertices.size();i++)
	{
		Color c=m.Vcolors[i];
		if(visited[i]&&c.R==255&&c.G==255&&c.B==255)
		{
			m.Vcolors[i].R=0;
			m.Vcolors[i].G=255;
			m.Vcolors[i].B=0;
		}
	}
	std::vector<int>& path=gc.GetPath();
	for(int i=0;i<path.size();i++)
	{
		int index=path[i];
		m.Vcolors[index].R=0;
		m.Vcolors[index].G=0;
		m.Vcolors[index].B=0;
	}
	printf("path length:%.2f , visited nodes: %d\n",gc.PathLength(),gc.VisitedNodeCount());
	PlyManager::OutputEx(m,"out_dj.ply");
	return 0;
}

int main()
{
	main1();
	main2();
	system("pause");
	return 0;
}