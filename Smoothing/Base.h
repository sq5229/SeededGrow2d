// ***********************************************************************
// Assembly         : Smoothing
// Author           : chnhideyoshi
// Created          : 12-17-2013
//
// Last Modified By : chnhideyoshi
// Last Modified On : 01-08-2014
// ***********************************************************************
// <copyright file="Base.h" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#ifndef BASE_H
#define BASE_H
#include <vector>
/// <summary>
/// Struct Vector
/// </summary>
struct Vector
{
	/// <summary>
	/// The x
	/// </summary>
	float X;
	/// <summary>
	/// The y
	/// </summary>
	float Y;
	/// <summary>
	/// The z
	/// </summary>
	float Z;
	/// <summary>
	/// Initializes a new instance of the <see cref="Vector" /> struct.
	/// </summary>
	Vector()
	{
		X = 0;
		Y = 0;
		Z = 0;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="Vector" /> struct.
	/// </summary>
	/// <param name="x">The x.</param>
	/// <param name="y">The y.</param>
	/// <param name="z">The z.</param>
	Vector(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}
};
struct PointAttachmentInfo;
/// <summary>
/// Struct Point3d
/// </summary>
struct Point3d
{
public:
	/// <summary>
	/// The x
	/// </summary>
	float X;
	/// <summary>
	/// The y
	/// </summary>
	float Y;
	/// <summary>
	/// The z
	/// </summary>
	float Z;
	/// <summary>
	/// Initializes a new instance of the <see cref="Point3d" /> struct.
	/// </summary>
	Point3d() :X(0),Y(0),Z(0)
	{
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="Point3d" /> struct.
	/// </summary>
	/// <param name="x">The x.</param>
	/// <param name="y">The y.</param>
	/// <param name="z">The z.</param>
	Point3d(float x, float y, float z) :X(x),Y(y),Z(z)
	{
	}
};
/// <summary>
/// Struct Triangle
/// </summary>
struct Triangle
{
public :
	/// <summary>
	/// The p0 index
	/// </summary>
	long P0Index;
	/// <summary>
	/// The p1 index
	/// </summary>
	long P1Index;
	/// <summary>
	/// The p2 index
	/// </summary>
	long P2Index;
	/// <summary>
	/// Initializes a new instance of the <see cref="Triangle" /> struct.
	/// </summary>
	/// <param name="p0index">The p0index.</param>
	/// <param name="p1index">The p1index.</param>
	/// <param name="p2index">The p2index.</param>
	Triangle(long p0index, long p1index, long p2index):P0Index(p0index),P1Index(p1index),P2Index(p2index){}
	/// <summary>
	/// Initializes a new instance of the <see cref="Triangle" /> struct.
	/// </summary>
	Triangle():P0Index(-1),P1Index(-1),P2Index(-1){}
public:
	/// <summary>
	/// Determines whether the specified index has vertex.
	/// </summary>
	/// <param name="index">The index.</param>
	bool HasVertex(long index)
	{
		return P0Index==index||P1Index==index||P2Index==index;
	}
	/// <summary>
	/// Gets the index of the other.
	/// </summary>
	/// <param name="i1">The i1.</param>
	/// <param name="i2">The i2.</param>
	/// <returns>long.</returns>
	long GetOtherIndex(long i1,long i2)
	{
		return P0Index+P1Index+P2Index-i1-i2;
	}
};
/// <summary>
/// Struct PointAttachmentInfo
/// </summary>
struct PointAttachmentInfo
{
public:
	/// <summary>
	/// Initializes a new instance of the <see cref="PointAttachmentInfo" /> struct.
	/// </summary>
	PointAttachmentInfo()
	{
		VertexAdjacencyList=NULL;
		TriangleAdjacencyList=NULL;
	}
	/// <summary>
	/// Finalizes an instance of the <see cref="PointAttachmentInfo" /> class.
	/// </summary>
	~PointAttachmentInfo()
	{
		ClearVertexAdj();
		ClearTriangleAdj();
	}
	/// <summary>
	/// Clears the vertex adj.
	/// </summary>
	void ClearVertexAdj()
	{
		if(VertexAdjacencyList!=NULL)
		{
			delete VertexAdjacencyList;
			VertexAdjacencyList=NULL;
		}
	}
	/// <summary>
	/// Clears the triangle adj.
	/// </summary>
	void ClearTriangleAdj()
	{
		if(TriangleAdjacencyList!=NULL)
		{
			delete TriangleAdjacencyList;
			VertexAdjacencyList=NULL;
		}
	}
	/// <summary>
	/// The vertex adjacency list
	/// </summary>
	std::vector<long>* VertexAdjacencyList;
	/// <summary>
	/// The triangle adjacency list
	/// </summary>
	std::vector<long>* TriangleAdjacencyList;
};

#endif