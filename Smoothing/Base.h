#ifndef BASE_H
#define BASE_H
#include <vector>
struct Vector
{
	float X;
	float Y;
	float Z;
	Vector()
	{
		X = 0;
		Y = 0;
		Z = 0;
	}
	Vector(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}
};
struct PointAttachmentInfo;
struct Point3d
{
public:
	float X;
	float Y;
	float Z;
	Point3d() 
	{
		X = 0;
		Y = 0;
		Z = 0;
	}
	Point3d(float x, float y, float z) 
	{
		X = x;
		Y = y;
		Z = z;
	}
};
struct Triangle
{
public :
	long P0Index;
	long P1Index;
	long P2Index;
	Triangle(long p0index, long p1index, long p2index)
	{
		P0Index=p0index;
		P1Index=p1index;
		P2Index=p2index;
	}
	Triangle()
	{
		P0Index=-1;
		P1Index=-1;
		P2Index=-1;
	}
public:
	bool HasVertex(long index)
	{
		return P0Index==index||P1Index==index||P2Index==index;
	}
	long GetOtherIndex(long i1,long i2)
	{
		return P0Index+P1Index+P2Index-i1-i2;
	}
};
struct PointAttachmentInfo
{
public:
	PointAttachmentInfo()
	{
		VertexAdjacencyList=NULL;
		TriangleAdjacencyList=NULL;
	}
	~PointAttachmentInfo()
	{
		ClearVertexAdj();
		ClearTriangleAdj();
	}
	void ClearVertexAdj()
	{
		if(VertexAdjacencyList!=NULL)
		{
			delete VertexAdjacencyList;
			VertexAdjacencyList=NULL;
		}
	}
	void ClearTriangleAdj()
	{
		if(TriangleAdjacencyList!=NULL)
		{
			delete TriangleAdjacencyList;
			VertexAdjacencyList=NULL;
		}
	}
	std::vector<long>* VertexAdjacencyList;
	std::vector<long>* TriangleAdjacencyList;
};

#endif