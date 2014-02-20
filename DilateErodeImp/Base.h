#ifndef BASE_H
#define BASE_H
#define _CRT_SECURE_NO_WARNINGS
#define byte unsigned char
#include <string.h>
#include <stdio.h>
struct IntDouble
{
	int X;
	int Y;
	IntDouble(int x,int y):X(x),Y(y)
	{
	}
	IntDouble():X(0),Y(0)
	{
	}
};

class Bitmap2d
{
private:
	byte* data;
	int width;
	int height;
public:
	int visit_count;
	Bitmap2d(int width,int height,byte v)
	{
		this->data=new byte[width*height];
		memset(data,v,width*height*sizeof(byte));
		this->width=width;
		this->height=height;
		this->visit_count=0;
	}
	Bitmap2d(Bitmap2d& bitmap)
	{
		this->width=bitmap.Width();
		this->height=bitmap.Height();
		this->data=new byte[width*height];
		memcpy(this->data,bitmap.data,sizeof(byte)*Length());
		this->visit_count=0;
	}
	~Bitmap2d()
	{
		delete[] data;
	}
	inline byte GetValue(int x,int y)
	{
		visit_count++;
		return data[x+y*width];
	}
	inline void SetValue(int x,int y,byte v)
	{
		visit_count++;
		data[x+y*width]=v;
	}
	inline int Width()
	{
		return width;
	}
	inline int Height()
	{
		return height;
	}
	inline int Length()
	{
		return width*height;
	}
	inline bool InRange(int x,int y)
	{
		return x>=0&&x<width&&y>=0&&y<height;
	}
	void ReadRaw(const char* filename)
	{
		FILE* file=fopen(filename,"rb");
		fread(data,sizeof(byte),Length(),file);
		fclose(file);
	}
	void SaveRaw(const char* filename)
	{
		FILE *const file = fopen(filename,"wb");
		fwrite(data,sizeof(byte),Length(),file);
		fclose(file);
	}
};
class DistenceMap
{
private:
	int* data;
	int width;
	int height;
public:
	int visit_count;
	DistenceMap(int width,int height,int v)
	{
		this->data=new int[width*height];
		for(int i=0;i<width*height;i++)
			data[i]=v;

		this->width=width;
		this->height=height;
		this->visit_count=0;
	}
	~DistenceMap()
	{
		delete[] data;
	}
	inline int GetValue(int x,int y)
	{
		visit_count++;
		return data[x+y*width];
	}
	inline void SetValue(int x,int y,int v)
	{
		visit_count++;
		data[x+y*width]=v;
	}
	inline int Width()
	{
		return width;
	}
	inline int Height()
	{
		return height;
	}
	inline int Length()
	{
		return width*height;
	}
};

#endif