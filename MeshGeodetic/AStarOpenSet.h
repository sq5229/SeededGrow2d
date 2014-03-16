#ifndef ASTAROPENSET_H
#define ASTAROPENSET_H
#include <vector>
#include <math.h>
#include "DijkstraSet.h"
class AStarSet_Heap:DijkstraSet
{
private:
	std::vector<int> heapArray;
	std::vector<float> *f_key;
	std::vector<int> indexInHeap;// stores the index to heapArray for each vertexIndex, -1 if not exist
public:
	AStarSet_Heap(int maxsize,std::vector<float> *key)
	{
		this->f_key=key;
		this->indexInHeap.resize(maxsize,-1);
	}
	~AStarSet_Heap(){f_key=0;}
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
		return (*f_key)[heapArray[index0]]<(*f_key)[heapArray[index1]];
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