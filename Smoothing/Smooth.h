#ifndef SMOOTH_H
#define SMOOTH_H
#include "Base.h"
#include "Mesh.h"
#include <math.h>
class Smoothing
{
private:
	Mesh* mesh;
public:
	Smoothing(Mesh* m)
	{
		this->mesh=m;
		
		m->InitPerVertexVertexAdj();
	}
	~Smoothing()
	{
		this->mesh=NULL;
	}
public:
	void Laplacian(int iterationTime)
	{
		Point3d* tempList=new Point3d[mesh->Vertices.size()];
		for(int c=0;c<iterationTime;c++)
		{
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				tempList[i]=GetSmoothedVertex_Laplacian(i);
			}
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				mesh->Vertices[i]=tempList[i];
			}
		}
		delete[] tempList;
	}
	void ScaleDependentLaplacian(int iterationTime)
	{
		Point3d* tempList=new Point3d[mesh->Vertices.size()];
		for(int c=0;c<iterationTime;c++)
		{
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				tempList[i]=GetSmoothedVertex_ScaleDependentLaplacian(i);
			}
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				mesh->Vertices[i]=tempList[i];
			}
		}
		delete[] tempList;
	}
	void Taubin(int iterationTime,float lambda,float mu)
	{
		Point3d* tempList=new Point3d[mesh->Vertices.size()];
		for(int c=0;c<iterationTime;c++)
		{
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				tempList[i]=GetSmoothedVertex_Taubin(i,lambda);
			}
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				mesh->Vertices[i]=tempList[i];
			}
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				tempList[i]=GetSmoothedVertex_Taubin(i,mu);
			}
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				mesh->Vertices[i]=tempList[i];
			}
		}
		delete[] tempList;
	}
	void CotWeightedLaplacian(int iterationTime)
	{
		if(!mesh->GetIsPerVertexTriangleInfoEnabled())
			mesh->InitPerVertexTriangleAdj();
		Point3d* tempList=new Point3d[mesh->Vertices.size()];
		for(int c=0;c<iterationTime;c++)
		{
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				tempList[i]=GetSmoothedVertex_CotWeightedLaplacian(i);
			}
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				mesh->Vertices[i]=tempList[i];
			}
		}
		delete[] tempList;
	}
	void HCLaplacian(int iterationTime,float factor1,float factor2)
	{
		std::vector<Point3d> point_vector;
		std::vector<Point3d> startPoint;
		point_vector.resize(mesh->Vertices.size());
		startPoint.resize(mesh->Vertices.size());
		for(size_t i=0;i<mesh->Vertices.size();i++)
		{
			startPoint[i].X=mesh->Vertices[i].X;
			startPoint[i].Y=mesh->Vertices[i].Y;
			startPoint[i].Z=mesh->Vertices[i].Z;
		}
		for(int c=0;c<iterationTime;c++)
		{
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				float dx = 0, dy = 0, dz = 0;
				std::vector<long>& adV=*(mesh->AdjInfos[i].VertexAdjacencyList);
				for (size_t j=0;j<adV.size();j++)
				{
					Point3d& t=mesh->Vertices[adV[j]];
					dx += t.X;
					dy += t.Y;
					dz += t.Z;
				}
				dx = dx / adV.size();
				dy = dy / adV.size();
				dz = dz / adV.size();
				point_vector[i].X=dx - (factor1 * startPoint[i].X + (1 - factor1) *mesh->Vertices[i].X) ;
				point_vector[i].Y=dy - (factor1 * startPoint[i].Y + (1 - factor1) *mesh->Vertices[i].Y) ;
				point_vector[i].Z=dz - (factor1 * startPoint[i].Z + (1 - factor1) *mesh->Vertices[i].Z) ;
				startPoint[i].X=dx;
				startPoint[i].Y=dy;
				startPoint[i].Z=dz;
			}
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				mesh->Vertices[i].X=point_vector[i].X;
				mesh->Vertices[i].Y=point_vector[i].Y;
				mesh->Vertices[i].Z=point_vector[i].Z;
			}
			//////////////////Step
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				float dx = 0, dy = 0, dz = 0;
				std::vector<long>& adV=*(mesh->AdjInfos[i].VertexAdjacencyList);
				for (size_t j=0;j<adV.size();j++)
				{
					Point3d& t=mesh->Vertices[adV[j]];
					dx += t.X;
					dy += t.Y;
					dz += t.Z;
				}
				dx = (1.0f - factor2) * dx / adV.size();
				dy = (1.0f - factor2) * dy / adV.size();
				dz = (1.0f - factor2) * dz / adV.size();
				//smoothed_point.SetX( start_point.GetX() - (smooth_factor * actual_vertex->GetX() + dx) );
				point_vector[i].X=startPoint[i].X - (factor2*mesh->Vertices[i].X +dx);
				point_vector[i].Y=startPoint[i].Y - (factor2*mesh->Vertices[i].Y +dy);
				point_vector[i].Z=startPoint[i].Z - (factor2*mesh->Vertices[i].Z +dz);
			}
			////
			for(size_t i=0;i<mesh->Vertices.size();i++)
			{
				mesh->Vertices[i].X=point_vector[i].X;
				mesh->Vertices[i].Y=point_vector[i].Y;
				mesh->Vertices[i].Z=point_vector[i].Z;
			}
		}
	}

private:
	float GetDistence(Point3d& p1,Point3d& p2)
	{
		return (float)sqrt((p1.X-p2.X)*(p1.X-p2.X)+(p1.Y-p2.Y)*(p1.Y-p2.Y)+(p1.Z-p2.Z)*(p1.Z-p2.Z));
	}
	float GetCotWeight(size_t index,int adjindex,std::vector<long>& adjVertices,std::vector<long>& adjFaces)
	{
		float w=0;
		int count=0;
		for(size_t i=0;i<adjFaces.size();i++)
		{
			Triangle& t=mesh->Faces[adjFaces[i]];
			if(t.HasVertex(adjindex))
			{
				long otherIndex=t.GetOtherIndex(index,adjindex);
				float cot=GetCot(t,otherIndex);
				w+=cot;
				count++;
			}
		}
		if(count==0)
			return 0;
		w=w/count;
		return w;
	}
	float GetCot(Triangle& t,long index)
	{
		std::vector<Point3d>& v=mesh->Vertices;
		float cos;
		if(t.P0Index==index)
		{
			cos=GetCos(v[t.P0Index],v[t.P1Index],v[t.P2Index]);
		}else
		if(t.P1Index==index)
		{
			cos=GetCos(v[t.P1Index],v[t.P0Index],v[t.P2Index]);
		}else
		if(t.P2Index==index)
		{
			cos=GetCos(v[t.P2Index],v[t.P1Index],v[t.P0Index]);
		}

		return cos/sqrt(1-cos*cos);
	}
	float GetCos(Point3d& ps,Point3d& pe1,Point3d& pe2)
	{
		Vector pse1(pe1.X-ps.X,pe1.Y-ps.Y,pe1.Z-ps.Z);
		Vector pse2(pe2.X-ps.X,pe2.Y-ps.Y,pe2.Z-ps.Z);
		float mo1=sqrt(pse1.X*pse1.X+pse1.Y*pse1.Y+pse1.Z*pse1.Z);
		float mo2=sqrt(pse2.X*pse2.X+pse2.Y*pse2.Y+pse2.Z*pse2.Z);
		float mul=pse1.X*pse2.X+pse1.Y*pse2.Y+pse1.Z*pse2.Z;
		return mul/(mo1*mo2);
	}

	Point3d GetSmoothedVertex_Laplacian(size_t index)
	{
		float dx=0,dy=0,dz=0;
		std::vector<long>& adjVertices=*(this->mesh->AdjInfos[index].VertexAdjacencyList);
		if(adjVertices.size()==0)
			return mesh->Vertices[index];
		for(size_t i=0;i<adjVertices.size();i++)
		{
			dx+=mesh->Vertices[adjVertices[i]].X-mesh->Vertices[index].X;
			dy+=mesh->Vertices[adjVertices[i]].Y-mesh->Vertices[index].Y;
			dz+=mesh->Vertices[adjVertices[i]].Z-mesh->Vertices[index].Z;
		}
		dx/=adjVertices.size();
		dy/=adjVertices.size();
		dz/=adjVertices.size();
		float newx=dx+mesh->Vertices[index].X;
		float newy=dy+mesh->Vertices[index].Y;
		float newz=dz+mesh->Vertices[index].Z;
		return Point3d(newx,newy,newz);
	}
	Point3d GetSmoothedVertex_ScaleDependentLaplacian(size_t index)
	{
		float dx=0,dy=0,dz=0;
		std::vector<long>& adjVertices=*(this->mesh->AdjInfos[index].VertexAdjacencyList);
		Point3d& p=mesh->Vertices[index];
		if(adjVertices.size()==0)
			return mesh->Vertices[index];
		float sumweight=0;
		for(size_t i=0;i<adjVertices.size();i++)
		{
			Point3d& t=mesh->Vertices[adjVertices[i]];
			float distence=GetDistence(p,t);
			dx+=(1.0f/distence)*(t.X-p.X);
			dy+=(1.0f/distence)*(t.Y-p.Y);
			dz+=(1.0f/distence)*(t.Z-p.Z);
			sumweight+=(1.0f/distence);
		}
		dx/=sumweight;
		dy/=sumweight;
		dz/=sumweight;
		float newx=dx+p.X;
		float newy=dy+p.Y;
		float newz=dz+p.Z;
		return Point3d(newx,newy,newz);
	}
	Point3d GetSmoothedVertex_Taubin(size_t index,float lambda)
	{
		float dx=0,dy=0,dz=0;
		std::vector<long>& adjVertices=*(this->mesh->AdjInfos[index].VertexAdjacencyList);
		Point3d& p=mesh->Vertices[index];
		if(adjVertices.size()==0)
			return mesh->Vertices[index];
		for(size_t i=0;i<adjVertices.size();i++)
		{
			Point3d& t=mesh->Vertices[adjVertices[i]];
			dx+=(t.X-p.X);
			dy+=(t.Y-p.Y);
			dz+=(t.Z-p.Z);
		}
		dx/=adjVertices.size();
		dy/=adjVertices.size();
		dz/=adjVertices.size();
		float newx=lambda*dx+p.X;
		float newy=lambda*dy+p.Y;
		float newz=lambda*dz+p.Z;
		return Point3d(newx,newy,newz);
	}
	Point3d GetSmoothedVertex_CotWeightedLaplacian(size_t index)
	{
		float dx=0,dy=0,dz=0;
		std::vector<long>& adjVertices=*(this->mesh->AdjInfos[index].VertexAdjacencyList);
		std::vector<long>& adjFaces=*(this->mesh->AdjInfos[index].TriangleAdjacencyList);
		Point3d& p=mesh->Vertices[index];
		if(adjVertices.size()==0||adjVertices.size()!=adjFaces.size())
			return mesh->Vertices[index];
		float sumweight=0;
		for(size_t i=0;i<adjVertices.size();i++)
		{
			Point3d& t=mesh->Vertices[adjVertices[i]];
			float cotWeight=GetCotWeight(index,adjVertices[i],adjVertices,adjFaces);
			dx+=cotWeight*(t.X-p.X);
			dy+=cotWeight*(t.Y-p.Y);
			dz+=cotWeight*(t.Z-p.Z);
			sumweight+=cotWeight;
		}
		dx/=sumweight;
		dy/=sumweight;
		dz/=sumweight;
		float newx=dx+p.X;
		float newy=dy+p.Y;
		float newz=dz+p.Z;
		return Point3d(newx,newy,newz);
	}

};

#endif