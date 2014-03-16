#ifndef GEODETICCALCULATOR_ASTAR_H
#define GEODETICCALCULATOR_ASTAR_H
#include <vector>
#include <math.h>
#include "Mesh.h"
#include "AStarOpenSet.h"
class GeodeticCalculator_AStar
{
private:
	AbstractGraph& graph;
	int startIndex;
	int endIndex;	
	std::vector<float> gMap;//current s-distances for each node
	std::vector<float> fMap;//current f-distances for each node
	std::vector<int> previus;//previus vertex on each vertex's s-path

	AStarSet_Heap* set_Open;//current involved vertices, every vertex in set has a path to start point with distance<MAX_DIS but may not be s-distance.
	std::vector<bool> flagMap_Close;//indicates if the s-path is found
	
	std::vector<bool> visited;//record visited vertices;
	std::vector<int> resultPath;//result path from start to end 
public:
	GeodeticCalculator_AStar(AbstractGraph& g,int vstIndex,int vedIndex):graph(g),startIndex(vstIndex),endIndex(vedIndex)
	{
		set_Open=0;
	}
	~GeodeticCalculator_AStar()
	{
		if(set_Open!=0) delete set_Open;
	}
	//core functions
	bool Execute()//main function execute AStar, return true if the end point is reached,false if path to end not exist 
	{
		visited.resize(graph.GetNodeCount(),false);
		gMap.resize(graph.GetNodeCount(),MAX_DIS);
		fMap.resize(graph.GetNodeCount(),MAX_DIS);
		previus.resize(graph.GetNodeCount(),-1);
		flagMap_Close.resize(graph.GetNodeCount(),false);
		this->set_Open=new AStarSet_Heap(graph.GetNodeCount(),&fMap);
		set_Open->Add(startIndex);
		gMap[startIndex]=0;
		fMap[startIndex]=GetH(startIndex);
		while(!set_Open->IsEmpty())
		{
			int pindex=set_Open->ExtractMin();// vertex with index "pindex" found its s-path
			flagMap_Close[pindex]=true;//mark it
			if(pindex==endIndex)
				return true;
			UpdateNeighborMinDistance(pindex);// update its neighbor's s-distance
		}
		return false;
	}
private:
	//core functions
	float GetH(int p1)
	{
		return graph.GetEvaDistance(p1,endIndex);
	}//calculate h[p1] when needed, not necessary to create an array to store
	void UpdateNeighborMinDistance(int pindex)
	{
		std::vector<int>& neightbourlist=graph.GetNeighbourList(pindex);
		for(size_t i=0;i<neightbourlist.size();i++ )
		{
			int neighbourindex=neightbourlist[i];
			visited[neighbourindex]=true;//just for recording , not necessary
			if (flagMap_Close[neighbourindex])//if Close Nodes,Type A
			{
				continue;
			}
			else
			{
				float gPassp = gMap[pindex] + graph.GetWeight(pindex, neighbourindex);
				if (fMap[neighbourindex]==MAX_DIS)//if unvisited nodes ,Type C
				{
					//same operation as in Dijkstra except the assignment of fMap
					gMap[neighbourindex] = gPassp;
					fMap[neighbourindex]=gPassp + GetH(neighbourindex);
					previus[neighbourindex] = pindex;
					set_Open->Add(neighbourindex);
				}
				else
				{
					//same operation as in Dijkstra except the assignment of fMap
					if (gPassp < gMap[neighbourindex])//Type B
					{
						gMap[neighbourindex] = gPassp;
						fMap[neighbourindex]=gPassp + GetH(neighbourindex);
						previus[neighbourindex] = pindex;
						set_Open->DecreaseKey(neighbourindex);
					}
				}
			}
		}
	}// for neighbors of pindex ,execute relaxation operation
public:
	//extra functions
	std::vector<int>& GetPath()
	{
		int cur=endIndex;
		while(cur!=startIndex)
		{
			resultPath.push_back(cur);
			cur=previus[cur];
		}
		resultPath.push_back(startIndex);
		std::reverse(resultPath.begin(),resultPath.end());
		return resultPath;
	}// reconstruct path from prev[]
	float PathLength()
	{
		return gMap[endIndex];
	}//return the length of the path form result path
	int VisitedNodeCount()
	{
		return (int)std::count(visited.begin(),visited.end(),true);
	}//return the visited nodes count
	std::vector<bool>& GetVisitedFlags()
	{
		return visited;
	}//return the visited flags of the nodes
};
#endif