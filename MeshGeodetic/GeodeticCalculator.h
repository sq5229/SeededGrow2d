#ifndef GEODETICCALCULATOR_H
#define GEODETICCALCULATOR_H
#define MAX_DIS 9999999999.0f
#include <vector>
#include <math.h>
#include "Mesh.h"
#include "DijkstraOpenSet.h"
#include "AStarOpenSet.h"

class GeodeticCalculator_Dijk
{
private:
	Mesh& mesh;
	int stIndex;
	int edIndex;
	std::vector<bool> flagMap;//indicates if the s-path is found
	std::vector<float> distenceMap;//current s-path length
	DijkstraSet* set;//current involved vertices, every vertex in set has a path to start point with distance<MAX_DIS but may not be s-distance.
	std::vector<int> previus;//previus vertex on each vertex's s-path
	std::vector<bool> visited;//record visited vertices;
	std::vector<int> resultPath;//result path from start to end 
public:
	static float Distence(Point3d& p1,Point3d& p2)
	{
		return sqrt((p1.X-p2.X)*(p1.X-p2.X)+(p1.Y-p2.Y)*(p1.Y-p2.Y)+(p1.Z-p2.Z)*(p1.Z-p2.Z));
	}
public:
	GeodeticCalculator_Dijk(Mesh& m,int vstIndex,int vedIndex)
		:mesh(m),stIndex(vstIndex),edIndex(vedIndex)
	{
		set=0;
		visited.resize(mesh.Vertices.size(),false);
	}
	~GeodeticCalculator_Dijk()
	{
		if(set!=0) 
			delete set;
	}
	bool Execute()
	{
		this->set=new DijkstraSet_Linear(this->mesh.Vertices.size(),&distenceMap);
		previus.resize(mesh.Vertices.size(),-1);
		flagMap.resize(mesh.Vertices.size(),false);
		distenceMap.resize(mesh.Vertices.size(),MAX_DIS);
		set->Add(stIndex);
		distenceMap[stIndex]=0;
		while(set->GetCount()!=0)
		{
			int pindexnewlyfound=set->ExtractMin();// vertex with index "pindexnewlyfound" found its s-path
			flagMap[pindexnewlyfound]=true;//mark it
			if(pindexnewlyfound==edIndex)
				return true;
			UpdateMinDistance(pindexnewlyfound);// update its neighbour's s-distance
		}
		return false;
	}
	std::vector<int>& GetPath()
	{
		int cur=edIndex;
		while(cur!=stIndex)
		{
			resultPath.push_back(cur);
			cur=previus[cur];
			if(cur==-1)
				throw std::exception();
		}
		resultPath.push_back(stIndex);
		std::reverse(resultPath.begin(),resultPath.end());
		return resultPath;
	}
	float PathLength()
	{
		float sum=0;
		for(size_t i=0;i<resultPath.size()-1;i++)
		{
			float dis=GetWeight(resultPath[i],resultPath[i+1]);
			sum+=dis;
		}
		return sum;
	}
	std::vector<bool>& GetVisitedFlags()
	{
		return visited;
	}
	int VisitedNodeCount()
	{
		return (int)std::count(visited.begin(),visited.end(),true);
	}
private:
	bool ExistNeighbour(int index,int neightbourindex)
	{
		return std::find(mesh.AdjacentVerticesPerVertex[index].begin(),mesh.AdjacentVerticesPerVertex[index].end(),neightbourindex)==mesh.AdjacentVerticesPerVertex[index].end();
	}
	float GetWeight(int p0Index,int p1Index)
	{
		if(p0Index==p1Index)
			return 0;
		return Distence(mesh.Vertices[p0Index],mesh.Vertices[p1Index]);
	}
	std::vector<int>& GetNeighbourList(int index)
	{
		return mesh.AdjacentVerticesPerVertex[index];
	}
	void UpdateMinDistance(int newlyfoundpIndex)
	{
		std::vector<int>& nlist=GetNeighbourList(newlyfoundpIndex);
		for(size_t i=0;i<nlist.size();i++ )
		{
			int nindex=nlist[i];
			visited[nindex]=true;
			if(!flagMap[nindex])
			{
				if(distenceMap[nindex]==MAX_DIS)
					set->Add(nindex);//newly approached vertex is pushed into set
				if(distenceMap[nindex]>distenceMap[newlyfoundpIndex]+GetWeight(nindex,newlyfoundpIndex))
				{
					distenceMap[nindex]=distenceMap[newlyfoundpIndex]+GetWeight(nindex,newlyfoundpIndex);
					set->DecreaseKey(nindex);// vertex with index "nindex" update its s-distance,so it's position in heap should also be updated.
					previus[nindex]=newlyfoundpIndex;
				}
			}
		}
	}
};

class GeodeticCalculator_AStar
{
private:
	Mesh& mesh;
	int stIndex;
	int edIndex;
	std::vector<bool> flagMap_Close;//indicates if the s-path is found
	std::vector<float> gMap;
	std::vector<float> fMap;
	AStarSet_Heap* set_Open;//current involved vertices, every vertex in set has a path to start point with distance<MAX_DIS but may not be s-distance.
	std::vector<int> previus;//previus vertex on each vertex's s-path
	std::vector<bool> visited;//record visited vertices;
	std::vector<int> resultPath;//result path from start to end 
public:
	static float Distence(Point3d& p1,Point3d& p2)
	{
		return sqrt((p1.X-p2.X)*(p1.X-p2.X)+(p1.Y-p2.Y)*(p1.Y-p2.Y)+(p1.Z-p2.Z)*(p1.Z-p2.Z));
	}
public:
	GeodeticCalculator_AStar(Mesh& m,int vstIndex,int vedIndex)
		:mesh(m),stIndex(vstIndex),edIndex(vedIndex)
	{
		set_Open=0;
		visited.resize(mesh.Vertices.size(),false);
		gMap.resize(mesh.Vertices.size(),MAX_DIS);
		fMap.resize(mesh.Vertices.size(),MAX_DIS);
		previus.resize(mesh.Vertices.size(),-1);
		flagMap_Close.resize(mesh.Vertices.size(),false);
		this->set_Open=new AStarSet_Heap(this->mesh.Vertices.size(),&fMap);
	}
	~GeodeticCalculator_AStar()
	{
		if(set_Open!=0) 
			delete set_Open;
	}
	bool Execute()
	{
		set_Open->Add(stIndex);
		gMap[stIndex]=0;
		while(set_Open->GetCount()!=0)
		{
			int pindexnewlyfound=set_Open->ExtractMin();// vertex with index "pindexnewlyfound" found its s-path
			flagMap_Close[pindexnewlyfound]=true;//mark it
			if(pindexnewlyfound==edIndex)
				return true;
			UpdateMinDistance(pindexnewlyfound);// update its neighbour's s-distance
		}
		return false;
	}
	std::vector<int>& GetPath()
	{
		int cur=edIndex;
		while(cur!=stIndex)
		{
			resultPath.push_back(cur);
			cur=previus[cur];
			if(cur==-1)
				throw std::exception();
		}
		resultPath.push_back(stIndex);
		std::reverse(resultPath.begin(),resultPath.end());
		return resultPath;
	}
	float PathLength()
	{
		float sum=0;
		for(size_t i=0;i<resultPath.size()-1;i++)
		{
			float dis=GetWeight(resultPath[i],resultPath[i+1]);
			sum+=dis;
		}
		return sum;
	}
	int VisitedNodeCount()
	{
		return (int)std::count(visited.begin(),visited.end(),true);
	}
	std::vector<bool>& GetVisitedFlags()
	{
		return visited;
	}
private:
	float GetF(int p1)
	{
		return gMap[p1]+GetH(p1);
	}
	float GetH(int p1)
	{
		return GetEvaDistance(p1,edIndex);
	}
	float GetEvaDistance(int p1,int p2)
	{
		return Distence(mesh.Vertices[p1],mesh.Vertices[p2]);
	}
	bool ExistNeighbour(int index,int neightbourindex)
	{
		return std::find(mesh.AdjacentVerticesPerVertex[index].begin(),mesh.AdjacentVerticesPerVertex[index].end(),neightbourindex)==mesh.AdjacentVerticesPerVertex[index].end();
	}
	float GetWeight(int p0Index,int p1Index)
	{
		if(p0Index==p1Index)
			return 0;
		return Distence(mesh.Vertices[p0Index],mesh.Vertices[p1Index]);
	}
	std::vector<int>& GetNeighbourList(int index)
	{
		return mesh.AdjacentVerticesPerVertex[index];
	}
	void UpdateMinDistance(int newlyfoundpIndex)
	{
		std::vector<int>& nlist=GetNeighbourList(newlyfoundpIndex);
		for(size_t i=0;i<nlist.size();i++ )
		{
			int nindex=nlist[i];
			visited[nindex]=true;
			if (flagMap_Close[nindex])
			{
				continue;
			}
			float gPassp = gMap[newlyfoundpIndex] + GetWeight(newlyfoundpIndex, nindex);
			if (set_Open->Exist(nindex))
			{
				if (gPassp < gMap[nindex])
				{
					float oldvalue=fMap[nindex];
					gMap[nindex] = gPassp;
					fMap[nindex] = gPassp + GetH(nindex);
					set_Open->UpdateKey(nindex,oldvalue);
					previus[nindex] = newlyfoundpIndex;
				}
			}
			else
			{
				gMap[nindex] = gPassp;
				fMap[nindex] = gPassp + GetH(nindex);
				previus[nindex] = newlyfoundpIndex;
				set_Open->Add(nindex);
			}
		}
	}
};

#endif