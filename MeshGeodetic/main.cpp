#include <stdio.h>
#include "Mesh.h"
#include "IOPly.h"
#include "GeodeticCalculator_AStar.h"
#include "GeodeticCalculator_Dijkstra.h"


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

int main1_Astar()
{
	Mesh m;
	PlyManager::ReadFileEx(m,"sampleMapC2.ply");
	printf("vcount:%d ,fcount:%d\n",m.Vertices.size(),m.Faces.size());
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

int main2_Dijk()
{
	Mesh m;
	PlyManager::ReadFileEx(m,"sampleMapC2.ply");
	printf("vcount:%d ,fcount:%d\n",m.Vertices.size(),m.Faces.size());
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
	/*std::vector<float> distance;
	distance.push_back(5.0);
	distance.push_back(6.0);
	distance.push_back(2.5);
	distance.push_back(3.2);
	distance.push_back(4.8);
	distance.push_back(8.7);
	distance.push_back(7.5);
	DijkstraSet_Heap heap(7,&distance);
	heap.Add(6);
	heap.Add(5);
	heap.Add(4);
	heap.Add(3);
	heap.Add(2);
	heap.Add(1);
	heap.Add(0);*/

	
	main1_Astar();
	main2_Dijk();
	system("pause");
	return 0;
}