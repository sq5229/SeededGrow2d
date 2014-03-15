#ifndef MESH_H
#define MESH_H
#include <vector>
#include <math.h>
#include <algorithm>
struct Point3d
{
public:
	float X;
	float Y;
	float Z;
	Point3d():X(0.0f),Y(0.0f),Z(0.0f){}
	Point3d(float x, float y, float z):X(x),Y(y),Z(z){}
public:
	static float Distence(Point3d& p1,Point3d& p2)
	{
		return sqrt((p1.X-p2.X)*(p1.X-p2.X)+(p1.Y-p2.Y)*(p1.Y-p2.Y)+(p1.Z-p2.Z)*(p1.Z-p2.Z));
	}
};
struct Triangle
{
public :
	int P0Index;
	int P1Index;
	int P2Index;
	Triangle(int p0index, int p1index, int p2index):P0Index(p0index),P1Index(p1index),P2Index(p2index){}
	Triangle():P0Index(-1),P1Index(-1),P2Index(-1){}
};
struct Color
{
public:
	unsigned char R;
	unsigned char G;
	unsigned char B;
	Color():R(255),G(255),B(255){}
	Color(unsigned char r, unsigned char g, unsigned char b):R(r),G(g),B(b){}
};
struct Box3Float
{
public:
	float Min3[3];
	float Max3[3];
	Box3Float()
	{
		Min3[0]=2147483646;
		Min3[1]=2147483646;
		Min3[2]=2147483646;
		Max3[0]=-99999;
		Max3[1]=-99999;
		Max3[2]=-99999;
	}
	~Box3Float()
	{

	}
	bool IsValid()
	{
		return Min3[0]<=Max3[0]&&Min3[1]<=Max3[1]&&Min3[2]<=Max3[2];
	}
	Box3Float(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
	{
		Min3[0] = minX;
		Min3[1] = minY;
		Min3[2] = minZ;
		Max3[0] = maxX;
		Max3[1] = maxY;
		Max3[2] = maxZ;
	}
	void UpdateRange(float x, float y, float z)
	{
		if (x < Min3[0])
			Min3[0] = x;
		if (y < Min3[1])
			Min3[1] = y;
		if (z < Min3[2])
			Min3[2] = z;

		if (x > Max3[0])
			Max3[0] = x;
		if (y > Max3[1])
			Max3[1] = y;
		if (z > Max3[2])
			Max3[2] = z;
	}
	float GetXLength()
	{
		return Max3[0]-Min3[0];
	}
	float GetYLength()
	{
		return Max3[1]-Min3[1];
	}
	float GetZLength()
	{
		return Max3[2]-Min3[2];
	}
};


class AbstractGraph
{
public:
	virtual float GetWeight(int p0Index,int p1Index)=0;
	virtual float GetEvaDistance(int p0Index,int p1Index)=0;
	virtual std::vector<int>& GetNeighbourList(int pindex)=0;
	virtual int GetNodeCount()=0;
};

class Mesh: public AbstractGraph
{
public:
	std::vector<Point3d> Vertices;
	std::vector<Color> Vcolors;
	std::vector<Triangle> Faces;
	std::vector<std::vector<int>> AdjacentVerticesPerVertex;
	int AddVertex(Point3d& toAdd)
	{
		int index = Vertices.size();
		Vertices.push_back(toAdd);
		return index;
	}
	int AddVertex(Point3d& toAdd,Color& color)
	{
		int index = Vertices.size();
		Vertices.push_back(toAdd);
		Vcolors.push_back(color);
		return index;
	}
	int AddFace(Triangle& tri)
	{
		int index = Faces.size();
		Faces.push_back(tri);
		return index;
	}
	void CaculateAdjacentVerticesPerVertex()
	{
		AdjacentVerticesPerVertex.resize(Vertices.size());
		for (size_t i = 0; i < Faces.size(); i++)
		{
			Triangle &t = Faces[i];
			std::vector<int> &p0list= AdjacentVerticesPerVertex[t.P0Index];
			std::vector<int> &p1list= AdjacentVerticesPerVertex[t.P1Index];
			std::vector<int> &p2list= AdjacentVerticesPerVertex[t.P2Index];
			if (std::find(p0list.begin(), p0list.end(), t.P1Index)==p0list.end())
				p0list.push_back(t.P1Index);
			if (std::find(p0list.begin(), p0list.end(), t.P2Index)==p0list.end())
				p0list.push_back(t.P2Index);
			if (std::find(p1list.begin(), p1list.end(), t.P0Index)==p1list.end())
				p1list.push_back(t.P0Index);
			if (std::find(p1list.begin(), p1list.end(), t.P2Index)==p1list.end())
				p1list.push_back(t.P2Index);
			if (std::find(p2list.begin(), p2list.end(), t.P0Index)==p2list.end())
				p2list.push_back(t.P0Index);
			if (std::find(p2list.begin(), p2list.end(), t.P1Index)==p2list.end())
				p2list.push_back(t.P1Index);
		}
	}
public:
	float GetWeight(int p0Index,int p1Index)
	{
		if(p0Index==p1Index)
			return 0;
		return Point3d::Distence(Vertices[p0Index],Vertices[p1Index]);
	}
	float GetEvaDistance(int p0Index,int p1Index)
	{
		return Point3d::Distence(Vertices[p0Index],Vertices[p1Index]);
	}
	std::vector<int>& GetNeighbourList(int index)
	{
		return AdjacentVerticesPerVertex[index];
	}
	int GetNodeCount()
	{
		return (int)Vertices.size();
	}

};


#endif