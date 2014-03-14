#ifndef GEODETICCALCULATOR_H
#define GEODETICCALCULATOR_H
#define MAX_DIS 9999999999.0f
#include <vector>
#include <math.h>
#include "Mesh.h"
#include "DijkstraSet.h"
#include "AStarOpenSet.h"

class GeodeticCalculator_Dijk
{
private:
	Mesh& mesh;
	int stIndex;
	int edIndex;
	std::vector<bool> flagMap_Close;//indicates if the s-path is found return true if is closed node
	DijkstraSet* set_Open;//current involved open vertices, every vertex in set has a path to start point with distance<MAX_DIS but may not be s-distance.
	std::vector<int> previus;//previus vertex on each vertex's s-path
	std::vector<bool> visited;//record visited vertices; not necessary
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
		this->visited.resize(mesh.Vertices.size(),false);
		this->set_Open=new DijkstraSet_Heap(this->mesh.Vertices.size());
		this->previus.resize(mesh.Vertices.size(),-1);
		this->flagMap_Close.resize(mesh.Vertices.size(),false);
	}
	~GeodeticCalculator_Dijk()
	{
		if(set_Open!=0) 
			delete set_Open;
	}
	bool Execute()
	{
		set_Open->Add(stIndex);
		set_Open->UpdateDistance(stIndex,0);
		while(set_Open->GetCount()!=0)
		{
			int pindexnewlyfound=set_Open->ExtractMin();// vertex with index "pindexnewlyfound" found its s-path
			flagMap_Close[pindexnewlyfound]=true;//mark it as closed
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
	void UpdateMinDistance(int pindex)
	{
		std::vector<int>& nlist=GetNeighbourList(pindex);
		for(size_t i=0;i<nlist.size();i++ )
		{
			int neighbourindex=nlist[i];
			visited[neighbourindex]=true;//just for recording , not necessary
			if(flagMap_Close[neighbourindex])//if Close Nodes
			{
				continue;
			}
			else
			{
				float distancePassp=set_Open->GetDistance(pindex)+GetWeight(neighbourindex,pindex);//calculate distance if the path passes p
				if(set_Open->GetDistance(neighbourindex)==MAX_DIS) //if unvisited
				{
					set_Open->Add(neighbourindex);//newly approached vertex is pushed into set
					set_Open->UpdateDistance(neighbourindex,distancePassp);
					previus[neighbourindex]=pindex;// record parent
				}
				else// if is open node
				{
					if(distancePassp<set_Open->GetDistance(neighbourindex))//test if it's s-distance can be updated
					{
						set_Open->UpdateDistance(neighbourindex,distancePassp);// vertex with index "nindex" update its s-distance,so it's position in heap should also be updated.
						previus[neighbourindex]=pindex;// record parent
					}
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
	//std::vector<float> fMap;
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
		previus.resize(mesh.Vertices.size(),-1);
		flagMap_Close.resize(mesh.Vertices.size(),false);
		this->set_Open=new AStarSet_Heap(this->mesh.Vertices.size());
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
	void UpdateMinDistance(int pindex)
	{
		std::vector<int>& neightbourlist=GetNeighbourList(pindex);
		for(size_t i=0;i<neightbourlist.size();i++ )
		{
			int neighbourindex=neightbourlist[i];
			visited[neighbourindex]=true;
			if (flagMap_Close[neighbourindex])
			{
				continue;
			}
			else
			{
				float gPassp = gMap[pindex] + GetWeight(pindex, neighbourindex);
				if (set_Open->GetDistance(neighbourindex)==MAX_DIS)
				{
					if (gPassp < gMap[neighbourindex])
					{
						gMap[neighbourindex] = gPassp;
						set_Open->UpdateDistance(neighbourindex,gPassp + GetH(neighbourindex));
						previus[neighbourindex] = pindex;
					}
				}
				else
				{
					gMap[neighbourindex] = gPassp;
					set_Open->UpdateDistance(neighbourindex,gPassp + GetH(neighbourindex));
					previus[neighbourindex] = pindex;
					set_Open->Add(neighbourindex);
				}
			}
		}
	}
};

#endif