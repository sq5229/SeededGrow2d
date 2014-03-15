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
	AbstractGraph& graph;
	int stIndex;
	int edIndex;
	std::vector<bool> flagMap_Close;//indicates if the s-path is found return true if is closed node
	DijkstraSet* set_Open;//current involved open vertices, every vertex in set has a path to start point with distance<MAX_DIS but may not be s-distance.
	std::vector<int> previus;//previus vertex on each vertex's s-path
	std::vector<bool> visited;//record visited vertices; not necessary
	std::vector<int> resultPath;//result path from start to end 
public:
	GeodeticCalculator_Dijk(AbstractGraph& g,int vstIndex,int vedIndex):graph(g),stIndex(vstIndex),edIndex(vedIndex)
	{
		this->visited.resize(g.GetNodeCount(),false);
		this->set_Open=new DijkstraSet_Heap(g.GetNodeCount());
		this->previus.resize(g.GetNodeCount(),-1);
		this->flagMap_Close.resize(g.GetNodeCount(),false);
	}
	~GeodeticCalculator_Dijk()
	{ 
		delete set_Open;
	}
	bool Execute()
	{
		set_Open->Add(stIndex);
		set_Open->UpdateDistance(stIndex,0);
		while(set_Open->GetCount()!=0)
		{
			int pindex=set_Open->ExtractMin();// vertex with index "pindexnewlyfound" found its s-path
			flagMap_Close[pindex]=true;//mark it as closed
			if(pindex==edIndex)
				return true;
			UpdateNeighbourMinDistance(pindex);// update its neighbour's s-distance
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
			float dis=graph.GetWeight(resultPath[i],resultPath[i+1]);
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
	void UpdateNeighbourMinDistance(int pindex)
	{
		std::vector<int>& nlist=graph.GetNeighbourList(pindex);
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
				float distancePassp=set_Open->GetDistance(pindex)+graph.GetWeight(neighbourindex,pindex);//calculate distance if the path passes p
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
	AbstractGraph& graph;
	int stIndex;
	int edIndex;
	std::vector<bool> flagMap_Close;//indicates if the s-path is found
	std::vector<float> gMap;
	AStarSet_Heap* set_Open;//current involved vertices, every vertex in set has a path to start point with distance<MAX_DIS but may not be s-distance.
	std::vector<int> previus;//previus vertex on each vertex's s-path
	std::vector<bool> visited;//record visited vertices;
	std::vector<int> resultPath;//result path from start to end 
public:
	GeodeticCalculator_AStar(AbstractGraph& g,int vstIndex,int vedIndex):graph(g),stIndex(vstIndex),edIndex(vedIndex)
	{
		visited.resize(graph.GetNodeCount(),false);
		gMap.resize(graph.GetNodeCount(),MAX_DIS);
		previus.resize(graph.GetNodeCount(),-1);
		flagMap_Close.resize(graph.GetNodeCount(),false);
		this->set_Open=new AStarSet_Heap(graph.GetNodeCount());
	}
	~GeodeticCalculator_AStar()
	{
			delete set_Open;
	}
	bool Execute()
	{
		set_Open->Add(stIndex);
		gMap[stIndex]=0;
		set_Open->UpdateDistance(stIndex,GetH(stIndex));
		while(set_Open->GetCount()!=0)
		{
			int pindex=set_Open->ExtractMin();// vertex with index "pindexnewlyfound" found its s-path
			flagMap_Close[pindex]=true;//mark it
			if(pindex==edIndex)
				return true;
			UpdateNeighbourMinDistance(pindex);// update its neighbour's s-distance
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
			float dis=graph.GetWeight(resultPath[i],resultPath[i+1]);
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
		return graph.GetEvaDistance(p1,edIndex);
	}
	void UpdateNeighbourMinDistance(int pindex)
	{
		std::vector<int>& neightbourlist=graph.GetNeighbourList(pindex);
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
				float gPassp = gMap[pindex] + graph.GetWeight(pindex, neighbourindex);
				if (set_Open->GetDistance(neighbourindex)==MAX_DIS)
				{
					set_Open->Add(neighbourindex);
					gMap[neighbourindex] = gPassp;
					set_Open->UpdateDistance(neighbourindex,gPassp + GetH(neighbourindex));
					previus[neighbourindex] = pindex;
				}
				else
				{
					if (gPassp < gMap[neighbourindex])
					{
						gMap[neighbourindex] = gPassp;
						set_Open->UpdateDistance(neighbourindex,gPassp + GetH(neighbourindex));
						previus[neighbourindex] = pindex;
					}
				}
			}
		}
	}
};

#endif