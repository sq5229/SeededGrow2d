#ifndef DIJKSTRASET_H
#define DIJKSTRASET_H
#include <vector>
#include <math.h>
class DijkstraSet
{
public:
	virtual void Add(int pindex)=0;// required heap operations, add new pindex into heap
	virtual  int ExtractMin()=0;// required heap operations, remove and return the min pindex
	virtual  int GetCount()=0;// required heap operations, return the element count
	virtual float GetDistance(int pindex)=0;// return distance[pindex] cuz distance is wrapped
	virtual void UpdateDistance(int pindex,float newdistance)=0;// required heap operations, when update shorter distance :dist[v]=dist[u] + dist_between(u, v) ;
};
/**
 \brief	Dijkstra set linear.. using O(n) linear search to find min element in set
 */
class DijkstraSet_Linear:public DijkstraSet
{
private:
	std::vector<int> linearContainer;
	std::vector<float> distance;
public:
	DijkstraSet_Linear(int maxsize)
	{
		this->distance.resize(maxsize,MAX_DIS);
	}
	~DijkstraSet_Linear(){}
	void Add(int pindex)
	{
		this->linearContainer.push_back(pindex);
	}
	int ExtractMin()
	{
		int insetIndex=GetMinIndex();
		int ret=linearContainer[insetIndex];
		RemoveAt(insetIndex);
		return ret;
	}
	int GetCount()
	{
		return linearContainer.size();
	}
	float GetDistance(int pindex)
	{
		return distance[pindex];
	}
	void UpdateDistance(int pindex,float newdistance)
	{
		distance[pindex]=newdistance;// just update , need not to change in-set position
	}
private:
	void RemoveAt(int index)
	{
		linearContainer[index]=linearContainer[linearContainer.size()-1];
		linearContainer.pop_back();
		//swap element to the end then pop it
	}
	int GetMinIndex()
	{
		//execute O(N) process to find min distance
		float min=MAX_DIS;
		int index=-1;
		for(size_t i=0;i<linearContainer.size();i++)
		{
			if(distance[linearContainer[i]]<min)
			{
				min=distance[linearContainer[i]];
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
	std::vector<float> distance_key;
	std::vector<int> indexInHeap;// stores the index to heapArray for each vertexIndex, -1 if not exist
public:
	DijkstraSet_Heap(int maxsize)
	{
		this->distance_key.resize(maxsize,MAX_DIS);
		this->indexInHeap.resize(maxsize,-1);
	}
	~DijkstraSet_Heap(){}
	void Add(int pindex)
	{
		this->heapArray.push_back(pindex);
		indexInHeap[pindex]=heapArray.size()-1;
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
		indexInHeap[pindex]=-1;
		return pindex;
	}
	int GetCount()
	{
		return heapArray.size();
	}
	float GetDistance(int pindex)
	{
		return distance_key[pindex];
	}
	void UpdateDistance(int pindex,float newdistance)
	{
		    distance_key[pindex]=newdistance;
			DecreaseKey(pindex);
	}
private:
	void DecreaseKey(int pindex)
	{
		ShiftUp(indexInHeap[pindex]);
	}
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
		return distance_key[heapArray[index0]]<distance_key[heapArray[index1]];
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
		indexInHeap[heapArray[i]]=i;//record new position
		indexInHeap[heapArray[j]]=j;//record new position
	}
};



#endif