#ifndef GEODETICCALCULATOR_DIJKSTRA_H
#define GEODETICCALCULATOR_DIJKSTRA_H
#include <vector>
#include <math.h>
#include "Mesh.h"
#include "DijkstraSet.h"
class GeodeticCalculator_Dijk
{
private:
	AbstractGraph& graph;
	int startIndex;
	int endIndex;
	std::vector<int> previus;//previus vertex on each vertex's s-path
	std::vector<float> distance;//current s-distances for each node
	
	std::vector<bool> flagMap_Close;//indicates if the s-path is found, return true if is closed node
	DijkstraSet* set_Open;//current involved open vertices, every vertex in set has a path to start point with distance<MAX_DIS but may not be s-distance.
	
	std::vector<bool> visited;//record visited vertices; not necessary
	std::vector<int> resultPath;//result path from start to end 
public:
	GeodeticCalculator_Dijk(AbstractGraph& g,int vstIndex,int vedIndex):graph(g),startIndex(vstIndex),endIndex(vedIndex)
	{
		set_Open=0;
	}
	~GeodeticCalculator_Dijk()
	{ 
		if(set_Open!=0) delete set_Open;
	}
	//core functions
	bool Execute()//main function execute Dijkstra, return true if the end point is reached,false if path to end not exist 
	{
		this->distance.resize(graph.GetNodeCount(),MAX_DIS);
		this->previus.resize(graph.GetNodeCount(),-1);
		this->flagMap_Close.resize(graph.GetNodeCount(),false);
		this->visited.resize(graph.GetNodeCount(),false);
		this->set_Open=new DijkstraSet_Heap(graph.GetNodeCount(),&distance);
		set_Open->Add(startIndex);
		distance[startIndex]=0;
		while(!set_Open->IsEmpty())
		{
			int pindex=set_Open->ExtractMin();// vertex with index "pindex" found its s-path
			flagMap_Close[pindex]=true;//mark it as closed
			if(pindex==endIndex)//if found end point
				return true;
			UpdateNeighborMinDistance(pindex);// update its neighbor's s-distance
		}
		return false;
	}
private:
	//core functions
	void UpdateNeighborMinDistance(int pindex)// for neighbors of pindex ,execute relaxation operation
	{
		std::vector<int>& nlist=graph.GetNeighbourList(pindex);
		for(size_t i=0;i<nlist.size();i++ )
		{
			int neighborindex=nlist[i];
			visited[neighborindex]=true;//just for recording , not necessary
			if(flagMap_Close[neighborindex])//if Close Nodes,Type A
			{
				continue;
			}
			else
			{
				float distancePassp=distance[pindex]+graph.GetWeight(neighborindex,pindex);//calculate distance if the path passes p
				if(distance[neighborindex]==MAX_DIS) //if unvisited nodes ,Type C
				{
					distance[neighborindex]=distancePassp;//update distance
					previus[neighborindex]=pindex;// record parent
					set_Open->Add(neighborindex);//newly approached vertex is pushed into set
				}
				else// if is open node ,Type B
				{
					if(distancePassp<distance[neighborindex])//test if it's s-distance can be updated
					{
						distance[neighborindex]=distancePassp;//update distance
						previus[neighborindex]=pindex;// record parent
						set_Open->DecreaseKey(neighborindex);// vertex with index "neighborindex" update its s-distance
						//,since the distance value is a key in heap, it's position in heap should also be updated.
						// if the set is not implemented as a heap, this operation can be omitted
					}
				}
			}
		}
	}
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
		return distance[endIndex];
	}//return the length of the path form result path
	std::vector<bool>& GetVisitedFlags()
	{
		return visited;
	}//return the visited flags of the nodes
	int VisitedNodeCount()
	{
		return (int)std::count(visited.begin(),visited.end(),true);
	}//return the visited nodes count
};
#endif