#ifndef DIJKSTRASET_H
#define DIJKSTRASET_H
#include <vector>
#include <math.h>
#define MAX_DIS 9999999999.0f
/**
\brief Dijkstra set. parameter pindex represents the id(index) of a certain graph node, 
 */
class DijkstraSet
{
public:
	virtual void Add(int pindex)=0;// required operations, add new pindex into set
	virtual  int ExtractMin()=0;// required operations, remove and return the pindex which has the minimum distance
	virtual  bool IsEmpty()=0;// required operations, return true if the element count in set==0
	virtual void DecreaseKey(int pindex)=0;// necessary if implemented as a heap, when update shorter distance :dist[v]=dist[u] + dist_between(u, v) ;
};
/**
 \brief	Dijkstra set linear.. using O(n) linear search to find min element in set
 */
class DijkstraSet_Linear:public DijkstraSet
{
private:
	std::vector<int> linearContainer;
    std::vector<float>* distance_key;
public:
	DijkstraSet_Linear(int maxsize,std::vector<float>* key)
	{
		this->distance_key=key;
	}
	~DijkstraSet_Linear(){distance_key=0;}
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
	bool IsEmpty()
	{
		return linearContainer.size()==0;
	}
	void DecreaseKey(int pindex)
	{
		// does nothing
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
			if((*distance_key)[linearContainer[i]]<min)
			{
				min=(*distance_key)[linearContainer[i]];
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
	std::vector<float>* distance_key;
	std::vector<int> indexInHeap;// stores the index to heapArray for each vertexIndex, -1 if not exist
public:
	DijkstraSet_Heap(int maxsize,std::vector<float>* key)
	{
		this->distance_key=key;
		this->indexInHeap.resize(maxsize,-1);
	}
	~DijkstraSet_Heap(){distance_key=0;}
	void Add(int pindex)
	{
		this->heapArray.push_back(pindex);
		indexInHeap[pindex]=heapArray.size()-1;
		ShiftUp(heapArray.size()-1);
	}
	int ExtractMin()
	{
		if(heapArray.size()==0)
			return -1;
		int pindex=heapArray[0];
		Swap(0,heapArray.size()-1);
		heapArray.pop_back();
		ShiftDown(0);
		indexInHeap[pindex]=-1;
		return pindex;
	}
	bool IsEmpty()
	{
		return heapArray.size()==0;
	}
	void DecreaseKey(int pindex)
	{
		ShiftUp(indexInHeap[pindex]);
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
		return (*distance_key)[heapArray[index0]]<(*distance_key)[heapArray[index1]];
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
		if (i >= heapArray.size()) return;
		int min = i;
		int lc = GetLeftChild(i);
		int rc = GetRightChild(i);
		if (lc < heapArray.size() && IsLessThan(lc,min))
			min = lc;
		if (rc < heapArray.size() && IsLessThan(rc,min))
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