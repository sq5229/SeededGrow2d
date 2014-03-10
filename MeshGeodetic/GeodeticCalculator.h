#ifndef GEODETICCALCULATOR_H
#define GEODETICCALCULATOR_H
#define MAX_DIS 9999999999.0f
#include <vector>
#include <math.h>
#include "Mesh.h"

class DijkstraSet
{
public:
	virtual void Add(int pindex)=0;// required heap operations
	virtual  int ExtractMin()=0;// required heap operations
	virtual  int GetCount()=0;// required heap operations
	virtual void DecreaseKey(int pindex)=0;// required heap operations, when update shorter distance :dist[v]=dist[u] + dist_between(u, v) ;
};
/**
 \brief	Dijkstra set linear.. using O(n) linear search to find min element in set
 */
class DijkstraSet_Linear:public DijkstraSet
{
private:
	std::vector<int> reachedSet;
	std::vector<float>* values;
public:
	DijkstraSet_Linear(int maxsize,std::vector<float>* values)
	{
		this->values=values;
	}
	~DijkstraSet_Linear(){}
	void Add(int pindex)
	{
		this->reachedSet.push_back(pindex);
	}
	int ExtractMin()
	{
		int insetIndex=GetMinIndex();
		int ret=reachedSet[insetIndex];
		RemoveAt(insetIndex);
		return ret;
	}
	int GetCount()
	{
		return reachedSet.size();
	}
	void DecreaseKey(int pindex)
	{
		//do nothing cuz this element distribution won't change with the key
	}
private:
	void RemoveAt(int index)
	{
		reachedSet[index]=reachedSet[reachedSet.size()-1];
		reachedSet.pop_back();
	}
	int GetMinIndex()
	{
		std::vector<float>& distanceMap=*values;
		float min=MAX_DIS;
		int index=-1;
		for(size_t i=0;i<reachedSet.size();i++)
		{
			if(distanceMap[reachedSet[i]]<min)
			{
				min=distanceMap[reachedSet[i]];
				index=i;
			}
		}
		return index;
	}
};
/**
 \brief	Dijkstra set heap.. using O(logn) operation to extract min element in set
 */
class DijkstraSet_Heap:public DijkstraSet
{
private:
	std::vector<int> heapArray;
	std::vector<float>* values;
	std::vector<int> setIndexMap;// stores the index to heapArray for each vertexIndex, -1 if not exist
public:
	DijkstraSet_Heap(int maxsize,std::vector<float>* values)
	{
		this->values=values;
		this->setIndexMap.resize(maxsize,-1);
	}
	~DijkstraSet_Heap(){}
	void Add(int pindex)
	{
		this->heapArray.push_back(pindex);
		setIndexMap[pindex]=heapArray.size()-1;
		ShiftUp(heapArray.size()-1);
	}
	int ExtractMin()
	{
		if(GetCount()==0)
			return -1;
		int pindex=heapArray[0];
		Swap(0,heapArray.size()-1);
		heapArray.pop_back();
		ShiftDown(0);
		setIndexMap[pindex]=-1;
		return pindex;
	}
	int GetCount()
	{
		return heapArray.size();
	}
	void DecreaseKey(int pindex)
	{
		ShiftUp(setIndexMap[pindex]);
	}
private:
	int GetParent(int index)
	{
		return (index-1)/2;
	}
	int GetLeftChild(int index)
	{
		return 2*index+1;
	}
	int GetRightChild(int index)
	{
		return 2*index+2;
	}
	bool IsLessThan(int index0,int index1)
	{
		return (*values)[heapArray[index0]]<(*values)[heapArray[index1]];
	}
	void ShiftUp(int i)
	{
		if (i == 0)
			return;
		else
		{
			int parent = GetParent(i);
			if (IsLessThan(i,parent))
			{
				Swap(i, parent);
				ShiftUp(parent);
			}
		}
	}
	void ShiftDown(int i)
	{
		if (i >= GetCount()) return;
		int min = i;
		int lc = GetLeftChild(i);
		int rc = GetRightChild(i);
		if (lc < GetCount() && IsLessThan(lc,min))
			min = lc;
		if (rc < GetCount() && IsLessThan(rc,min))
			min = rc;
		if (min != i)
		{
			Swap(i, min);
			ShiftDown(min);
		}
	}
	void Swap(int i, int j)
	{
		int temp = heapArray[i];
		heapArray[i] = heapArray[j];
		heapArray[j] = temp;
		setIndexMap[heapArray[i]]=i;//record new position
		setIndexMap[heapArray[j]]=j;//record new position
	}
};
class GeodeticCalculator
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
	GeodeticCalculator(Mesh& m,int vstIndex,int vedIndex)
		:mesh(m),stIndex(vstIndex),edIndex(vedIndex)
	{
		set=0;
		visited.resize(mesh.Vertices.size(),false);
	}
	~GeodeticCalculator()
	{
		if(set!=0) 
			delete set;
	}
	bool ExecuteDijikstra()
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
	std::vector<bool>& GetVisitedFlags()
	{
		return visited;
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



#endif