#ifndef BYTEMATRIX_H
#define BYTEMATRIX_H
#include <string.h>
#define byte unsigned char


struct Box2d
{
	int XMin;
	int XMax;
	int YMin;
	int YMax;
	Box2d()
	{
		XMax=-1;
		YMax=-1;
		XMin=9999;
		YMin=9999;
	}
	void UpdateRange(int x,int y)
	{
		if(x<XMin)
			XMin=x;
		if(y<YMin)
			YMin=y;
		if(x>XMax)
			XMax=x;
		if(y>YMax)
			YMax=y;
	}
};
struct Point2d
{
	int X;
	int Y;
	Point2d():X(-1),Y(-1)
	{
	}
	Point2d(int x,int y):X(x),Y(y)
	{
	}
};
class ByteMatrix
{
public:
	int width;
	int height;
	ByteMatrix(int width,int height,byte value)
	{
		this->width=width;
		this->height=height;
		this->data=new byte[width*height];
		memset(data,0,width*height);
	}
	~ByteMatrix()
	{
		delete[] data;
	}
	inline void SetValue(int x,int y, byte v)
	{
		data[x+width*y]=v;
	}
	inline byte GetValue(int x,int y)
	{
		return data[x+width*y];
	}
	void SaveRaw(const char* fileName)
	{
			FILE *const nfile = fopen(fileName,"wb");
			fwrite(data,sizeof(unsigned char),width*height,nfile);
			fclose(nfile);
			return;
	}
private:
	byte* data;
};
#endif