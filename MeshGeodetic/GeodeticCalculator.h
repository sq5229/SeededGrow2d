#ifndef GEODETICCALCULATOR_H
#define GEODETICCALCULATOR_H
#define MAX_Dis 9999999999.0f
#include <vector>
#include <math.h>
#include "Mesh.h"
class DijkstraSet_Linear
{
private:
	std::vector<int> reachedSet;
	std::vector<float>* values;
	std::vector<bool> inSetFlag;
public:
	DijkstraSet_Linear(int maxsize,std::vector<float>* values)
	{
		this->values=values;
		this->inSetFlag.resize(maxsize,false);
	}
	~DijkstraSet_Linear(){}
	void Add(int pindex)
	{
		this->reachedSet.push_back(pindex);
		inSetFlag[pindex]=true;
	}
	bool Exist(int pindex)
	{
		return inSetFlag[pindex];
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
private:
	void RemoveAt(int index)
	{
		reachedSet[index]=reachedSet[reachedSet.size()-1];
		reachedSet.pop_back();
		inSetFlag[index]=false;
	}
	int GetMinIndex()
	{
		std::vector<float>& distanceMap=*values;
		float min=MAX_Dis;
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
class DijkstraSet_Heap
{
private:
	std::vector<int> heapArray;
	std::vector<float>* values;
	std::vector<bool> inreachedSet;
public:
	DijkstraSet_Heap(int maxsize,std::vector<float>* values)
	{
		this->values=values;
		this->inreachedSet.resize(maxsize,false);
	}
	~DijkstraSet_Heap(){}
	void Add(int pindex)
	{
		this->heapArray.push_back(pindex);
		ShiftUp(heapArray.size()-1);
		inreachedSet[pindex]=true;
	}
	bool Exist(int pindex)
	{
		return inreachedSet[pindex];
	}
	int ExtractMin()
	{
		if(GetCount()==0)
			return -1;
		int pindex=heapArray[0];
		Swap(0,heapArray.size()-1);
		heapArray.pop_back();
		ShiftDown(0);
		inreachedSet[pindex]=false;
		return pindex;
	}
	int GetCount()
	{
		return heapArray.size();
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
	DijkstraSet_Heap* set;//current involved vertices which have path to
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
		this->set=new DijkstraSet_Heap(this->mesh.Vertices.size(),&distenceMap);
		previus.resize(mesh.Vertices.size(),-1);
		flagMap.resize(mesh.Vertices.size(),false);
		distenceMap.resize(mesh.Vertices.size(),MAX_Dis);
		set->Add(stIndex);
		distenceMap[stIndex]=0;
		while(set->GetCount()!=0)
		{
			int pindexnewlyfound=set->ExtractMin();
			flagMap[pindexnewlyfound]=true;
			if(pindexnewlyfound==edIndex)
				return true;
			UpdateMinDistance(pindexnewlyfound);
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
				if(!set->Exist(nindex))
				{
					set->Add(nindex);
				}
				if(distenceMap[nindex]>distenceMap[newlyfoundpIndex]+GetWeight(nindex,newlyfoundpIndex))
				{
					distenceMap[nindex]=distenceMap[newlyfoundpIndex]+GetWeight(nindex,newlyfoundpIndex);
					previus[nindex]=newlyfoundpIndex;
				}
			}
		}
	}
};



#endif