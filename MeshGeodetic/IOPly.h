#ifndef IOPLY_H
#define IOPLY_H
#include "Mesh.h"
class PlyManager
{
public:
	static void Output(Mesh& mesh,const char* filename);
	static void OutputEx(Mesh& mesh,const char* filename);
	static void ReadFile(Mesh& mesh,const char* fileName);
	static void ReadFileEx(Mesh& mesh,const char* fileName);
private:
	static void AWriteV(FILE* sw, double v1, double v2, double v3,unsigned char r,unsigned char g,unsigned char b);
	static void AWriteF(FILE* sw, int i1, int i2, int i3);
};
void PlyManager::AWriteV(FILE* file, double v1, double v2, double v3,unsigned char r,unsigned char g,unsigned char b)
{
	fprintf(file,"%.2f %.2f %.2f %d %d %d\n",v1,v2,v3,r,g,b);
}
void PlyManager::AWriteF(FILE* file, int i1, int i2, int i3)
{
	fprintf(file,"%d %d %d %d\n",3,i1,i2,i3);
}
void PlyManager::ReadFile(Mesh& mesh,const char* fileName)
{
	int vcount=0;
	int fcount=0;
	FILE * nfile = fopen(fileName,"r");
	fscanf(nfile,"ply\nformat ascii 1.0\ncomment VCGLIB generated\nelement vertex %d\n",&vcount);
	fscanf(nfile,"property float x\nproperty float y\nproperty float z\nproperty uchar red\nproperty uchar green\nproperty uchar blue\nelement face %d\n",&fcount);
	fscanf(nfile,"property list int int vertex_indices\nend_header\n");
	float v1=0,v2=0,v3=0;
	int r=0,g=0,b=0;
	int i1=0,i2=0,i3=0;
	for(int i=0;i<vcount;i++)
	{
		fscanf(nfile,"%f %f %f %d %d %d\n",&v1,&v2,&v3,&r,&g,&b);
		Point3d p3d(v1,v2,v3);
		mesh.AddVertex(p3d);
	}
	for(int j=0;j<fcount;j++)
	{
		fscanf(nfile,"3 %d %d %d\n",&i1,&i2,&i3);
		Triangle t(i1,i2,i3);
		mesh.AddFace(t);
	}
	fclose(nfile);
}
void PlyManager::ReadFileEx(Mesh& mesh,const char* fileName)
{
	int vcount=0;
	int fcount=0;
	FILE * nfile = fopen(fileName,"r");
	fscanf(nfile,"ply\nformat ascii 1.0\ncomment VCGLIB generated\nelement vertex %d\n",&vcount);
	fscanf(nfile,"property float x\nproperty float y\nproperty float z\nproperty uchar red\nproperty uchar green\nproperty uchar blue\nproperty uchar alpha\nelement face %d\n",&fcount);
	fscanf(nfile,"property list uchar int vertex_indices\nend_header\n");
	float v1=0,v2=0,v3=0;
	int a=0,r=0,g=0,b=0;
	int i1=0,i2=0,i3=0;
	for(int i=0;i<vcount;i++)
	{
		fscanf(nfile,"%f %f %f %d %d %d %d\n",&v1,&v2,&v3,&r,&g,&b,&a);
		Point3d p3d(v1,v2,v3);
		Color c(r,g,b);
		mesh.AddVertex(p3d,c);
	}
	for(int j=0;j<fcount;j++)
	{
		fscanf(nfile,"3 %d %d %d\n",&i1,&i2,&i3);
		Triangle t(i1,i2,i3);
		mesh.AddFace(t);
	}
	fclose(nfile);
}
void PlyManager::Output(Mesh& mesh,const char* filename)
{
	FILE * nfile = fopen(filename,"wb");
	fprintf(nfile,"ply\n");
	fprintf(nfile,"format ascii 1.0\n");
	fprintf(nfile,"comment VCGLIB generated\n");
	fprintf(nfile,"element vertex %d\n",mesh.Vertices.size());
	fprintf(nfile,"property float x\n");
	fprintf(nfile,"property float y\n");
	fprintf(nfile,"property float z\n");
	fprintf(nfile,"property uchar red\n");
	fprintf(nfile,"property uchar green\n");
	fprintf(nfile,"property uchar blue\n");
	fprintf(nfile,"element face %d\n",mesh.Faces.size());
	fprintf(nfile,"property list int int vertex_indices\n");
	fprintf(nfile,"end_header\n");
	for(size_t i=0;i<mesh.Vertices.size();i++)
	{
		AWriteV(nfile,mesh.Vertices[i].X,mesh.Vertices[i].Y,mesh.Vertices[i].Z,255,255,255);
	}
	for(size_t i=0;i<mesh.Faces.size();i++)
	{
		AWriteF(nfile,mesh.Faces[i].P0Index,mesh.Faces[i].P1Index,mesh.Faces[i].P2Index);
	}
	fclose(nfile);
}
void PlyManager::OutputEx(Mesh& mesh,const char* filename)
{
	FILE * nfile = fopen(filename,"wb");
	fprintf(nfile,"ply\n");
	fprintf(nfile,"format ascii 1.0\n");
	fprintf(nfile,"comment VCGLIB generated\n");
	fprintf(nfile,"element vertex %d\n",mesh.Vertices.size());
	fprintf(nfile,"property float x\n");
	fprintf(nfile,"property float y\n");
	fprintf(nfile,"property float z\n");
	fprintf(nfile,"property uchar red\n");
	fprintf(nfile,"property uchar green\n");
	fprintf(nfile,"property uchar blue\n");
	fprintf(nfile,"element face %d\n",mesh.Faces.size());
	fprintf(nfile,"property list int int vertex_indices\n");
	fprintf(nfile,"end_header\n");
	for(size_t i=0;i<mesh.Vertices.size();i++)
	{
		AWriteV(nfile,mesh.Vertices[i].X,mesh.Vertices[i].Y,mesh.Vertices[i].Z,mesh.Vcolors[i].R,mesh.Vcolors[i].G,mesh.Vcolors[i].B);
	}
	for(size_t i=0;i<mesh.Faces.size();i++)
	{
		AWriteF(nfile,mesh.Faces[i].P0Index,mesh.Faces[i].P1Index,mesh.Faces[i].P2Index);
	}
	fclose(nfile);
}
#endif