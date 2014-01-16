#ifndef MESH_H
#define MESH_H
#include "Base.h"
#include <vector>
#include <algorithm>
/// <summary>
/// 
/// </summary>
class Mesh
{
public:
	/// <summary>
	/// The vertices
	/// </summary>
	std::vector<Point3d> Vertices;
	/// <summary>
	/// The faces
	/// </summary>
	std::vector<Triangle> Faces;
	/// <summary>
	/// The adj infos
	/// </summary>
	std::vector<PointAttachmentInfo> AdjInfos;
	bool GetIsPerVertexVertexInfoEnabled();
	bool GetIsPerVertexTriangleInfoEnabled();
	Mesh();
	~Mesh();
	long AddVertex(Point3d& toAdd);
	long AddFace(Triangle& tri);
	void InitPerVertexVertexAdj();
	void InitPerVertexTriangleAdj();
	void ClearPerVertexVertexAdj();
	void ClearPerVertexTriangleAdj();
private:
	bool IsPerVertexVertexInfoEnabled;
	bool IsPerVertexTriangleInfoEnabled;
};

Mesh::Mesh()
{
	IsPerVertexTriangleInfoEnabled=false;
	IsPerVertexVertexInfoEnabled=false;
}
Mesh::~Mesh()
{
	if(IsPerVertexTriangleInfoEnabled)
		ClearPerVertexTriangleAdj();
	if(IsPerVertexVertexInfoEnabled)
		ClearPerVertexVertexAdj();
}

/// <summary>
/// Adds the vertex.
/// </summary>
/// <param name="toAdd">To add.</param>
/// <returns></returns>
long Mesh::AddVertex(Point3d& toAdd)
{
	long index = (long)Vertices.size();
	Vertices.push_back(toAdd);
	return index;
}
/// <summary>
/// Adds the face.
/// </summary>
/// <param name="tri">The tri.</param>
/// <returns></returns>
long  Mesh::AddFace(Triangle& tri)
{
	long  index = (long)Faces.size();
	Faces.push_back(tri);
	return index;
}


/// <summary>
/// Initializes the per vertex vertex adj.
/// </summary>
void Mesh::InitPerVertexVertexAdj()
{
	if(IsPerVertexVertexInfoEnabled)
		ClearPerVertexVertexAdj();
	IsPerVertexVertexInfoEnabled = true;
	if(AdjInfos.size()!=Vertices.size())
		AdjInfos.resize(Vertices.size());
	size_t vcount=Vertices.size();
	size_t fcount= Faces.size();
	for (size_t i = 0; i < vcount; i++)
	{
		std::vector<long>* vertexAdjacencyList= new (std::nothrow)std::vector<long>();
		if(vertexAdjacencyList==NULL){return;}
		vertexAdjacencyList->reserve(6);
		AdjInfos[i].VertexAdjacencyList=vertexAdjacencyList;
	}
	for (size_t i = 0; i < fcount; i++)
	{
		Triangle &t = Faces[i];
		std::vector<long> *p0list= AdjInfos[t.P0Index].VertexAdjacencyList;
		std::vector<long> *p1list= AdjInfos[t.P1Index].VertexAdjacencyList;
		std::vector<long> *p2list= AdjInfos[t.P2Index].VertexAdjacencyList;

		if (std::find(p0list->begin(), p0list->end(), t.P1Index)==p0list->end())
			p0list->push_back(t.P1Index);
		if (std::find(p0list->begin(), p0list->end(), t.P2Index)==p0list->end())
			p0list->push_back(t.P2Index);

		if (std::find(p1list->begin(), p1list->end(), t.P0Index)==p1list->end())
			p1list->push_back(t.P0Index);
		if (std::find(p1list->begin(), p1list->end(), t.P2Index)==p1list->end())
			p1list->push_back(t.P2Index);

		if (std::find(p2list->begin(), p2list->end(), t.P0Index)==p2list->end())
			p2list->push_back(t.P0Index);
		if (std::find(p2list->begin(), p2list->end(), t.P1Index)==p2list->end())
			p2list->push_back(t.P1Index);
	}
}
/// <summary>
/// Initializes the per vertex triangle adj.
/// </summary>
void Mesh::InitPerVertexTriangleAdj()
{
	if(IsPerVertexTriangleInfoEnabled)
		ClearPerVertexTriangleAdj();
	IsPerVertexTriangleInfoEnabled = true;
	if(AdjInfos.size()!=Vertices.size())
		AdjInfos.resize(Vertices.size());
	for (size_t i = 0; i < Vertices.size(); i++)
	{
		std::vector<long>* triangleAdjacencyList = new (std::nothrow)std::vector<long>();
		if(triangleAdjacencyList==NULL){return;}
		triangleAdjacencyList->reserve(6);
		AdjInfos[i].TriangleAdjacencyList=triangleAdjacencyList;
	}
	for (size_t i = 0; i < Faces.size(); i++)
	{
		Triangle& t = Faces[i];
		std::vector<long> *t0list= AdjInfos[t.P0Index].TriangleAdjacencyList;
		std::vector<long> *t1list= AdjInfos[t.P1Index].TriangleAdjacencyList;
		std::vector<long> *t2list= AdjInfos[t.P2Index].TriangleAdjacencyList;
		t0list->push_back(i);
		t1list->push_back(i);
		t2list->push_back(i);
	}
}
/// <summary>
/// Clears the per vertex vertex adj.
/// </summary>
void Mesh::ClearPerVertexVertexAdj()
{
	if(!IsPerVertexVertexInfoEnabled)
		return;
	for (size_t i = 0; i < Vertices.size(); i++)
	{
		if (AdjInfos[i].VertexAdjacencyList != NULL)
		{
			delete AdjInfos[i].VertexAdjacencyList;
			AdjInfos[i].VertexAdjacencyList=NULL;
		}
	}
	IsPerVertexVertexInfoEnabled = false;
}
/// <summary>
/// Clears the per vertex triangle adj.
/// </summary>
void Mesh::ClearPerVertexTriangleAdj()
{
	if(!IsPerVertexTriangleInfoEnabled)
		return;
	for (size_t i = 0; i < Vertices.size(); i++)
	{
		if (AdjInfos[i].TriangleAdjacencyList != NULL)
		{
			delete AdjInfos[i].TriangleAdjacencyList;
			AdjInfos[i].TriangleAdjacencyList=NULL;
		}
	}
	IsPerVertexTriangleInfoEnabled = false;
}

/// <summary>
/// Gets the is per vertex vertex information enabled.
/// </summary>
/// <returns></returns>
bool Mesh::GetIsPerVertexVertexInfoEnabled()
{
	return IsPerVertexVertexInfoEnabled;
}
/// <summary>
/// Gets the is per vertex triangle information enabled.
/// </summary>
/// <returns></returns>
bool Mesh::GetIsPerVertexTriangleInfoEnabled()
{
	return IsPerVertexTriangleInfoEnabled;
}


#endif