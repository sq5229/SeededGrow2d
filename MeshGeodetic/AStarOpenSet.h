#ifndef ASTAROPENSET_H
#define ASTAROPENSET_H
#include <vector>
#include <math.h>

class AStarSet_Heap :public DijkstraSet
{
private:
	std::vector<int> heapArray;
	std::vector<float> f_key;
	std::vector<int> indexInHeap;// stores the index to heapArray for each vertexIndex, -1 if not exist
public:
	AStarSet_Heap(int maxsize)
	{
		this->f_key.resize(maxsize,MAX_DIS);
		this->indexInHeap.resize(maxsize,-1);
	}
	~AStarSet_Heap(){}
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
	
	bool Exist(int pindex)
	{
		return indexInHeap[pindex]!=-1;
	}
	float GetDistance(int pindex)
	{
		return f_key[pindex];
	}
	void UpdateDistance(int pindex,float newdistance)
	{
		f_key[pindex]=newdistance;
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
		return f_key[heapArray[index0]]<f_key[heapArray[index1]];
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