#ifndef DILATE_H
#define DILATE_H
#include "Base.h"
#include <vector>
#include "Erode.h"

class DilateProcessor
{
private:
	Bitmap2d& bmp;
	int radius;
	std::vector<IntDouble> winOffsets;
public:
	DilateProcessor(Bitmap2d& bitmap,int radius,WindowMode mode):bmp(bitmap)
	{
		this->radius=radius;
		InitWindowOffsets(mode);
	}
	~DilateProcessor(){}
	Bitmap2d* Execute()
	{
		Bitmap2d* newBmp=new Bitmap2d(bmp.Width(),bmp.Height(),0);
		for(int j=0;j<bmp.Height();j++)
		{
			for(int i=0;i<bmp.Width();i++)
			{
				if(HasWhiteInWindow(i,j))
					newBmp->SetValue(i,j,255);
				else
					newBmp->SetValue(i,j,0);
			}
		}
		return newBmp;
	}
	Bitmap2d* Execute2()
	{
		Bitmap2d* newBmp=new Bitmap2d(bmp);
		for(int j=0;j<bmp.Height();j++)
		{
			for(int i=0;i<bmp.Width();i++)
			{
				if(bmp.GetValue(i,j)==255&&HasBlackAdjacencyPixel(i,j))
				{
					SetWindowValue(*newBmp,i,j,255);
				}
			}
		}
		return newBmp;
	}
	Bitmap2d* Execute3()
	{
		Bitmap2d* newBmp=new Bitmap2d(bmp);
		DistenceMap* dmap=GetDistenceMap();
		for (int i=0; i<bmp.Width(); i++)
		{
			for (int j=0; j<bmp.Height(); j++)
			{
				byte v=dmap->GetValue(i,j)<=radius?255:0;
				newBmp->SetValue(i,j,v);
			}
		}
		newBmp->visit_count+=dmap->visit_count;
		delete dmap;
		return newBmp;
	}

private:
	inline int Min(int a,int b)
	{
		return a>b?b:a;
	}
	void InitWindowOffsets(WindowMode mod)
	{
		if(mod==SQUARE)
		{
			for (int i = 0; i < 2 * radius + 1; i++)
			{
				for (int j = 0; j < 2 * radius + 1; j++)
				{
					IntDouble t(i-radius,j-radius);
					this->winOffsets.push_back(t);
				}
			}
		}
		if(mod==CIRCLE)
		{
			for (int i = 0; i < 2 * radius + 1; i++)
			{
				for (int j = 0; j < 2 * radius + 1; j++)
				{
					int d2=(i-radius)*(i-radius)+(j-radius)*(j-radius);
					if(d2<=radius*radius)
					{
						IntDouble t(i-radius,j-radius);
						this->winOffsets.push_back(t);
					}
				}
			}
		}
		if(mod==SQUARE2)
		{
			for(int i=-radius;i<=radius;i++)
			{
				for(int j=-radius;j<=radius;j++)
				{
					int absi=i>=0?i:-i;
					int absj=j>=0?j:-j;
					if(absi+absj<=radius)
					{
						winOffsets.push_back(IntDouble(i,j));
					}
				}
			}
		}
	}
	bool HasWhiteInWindow(int i,int j)
	{
		for(size_t k=0;k<winOffsets.size();k++)
		{
			int tx=i+winOffsets[k].X;
			int ty=j+winOffsets[k].Y;
			if(!bmp.InRange(tx,ty))
				continue;
			if(bmp.GetValue(tx,ty)==255)
			{
				return true;
			}
		}
		return false;
	}
	bool HasBlackAdjacencyPixel(int i,int j)
	{
		if(i>0&&bmp.GetValue(i-1,j)==0)
			return true;
		if(i<bmp.Width()-1&&bmp.GetValue(i+1,j)==0)
			return true;
		if(j>0&&bmp.GetValue(i,j-1)==0)
			return true;
		if(j<bmp.Height()-1&&bmp.GetValue(i,j+1)==0)
			return true;
		return false;
	}
	void SetWindowValue(Bitmap2d& bmp,int i,int j,byte v)
	{
		for(size_t k=0;k<winOffsets.size();k++)
		{
			int tx=i+winOffsets[k].X;
			int ty=j+winOffsets[k].Y;
			if(!bmp.InRange(tx,ty))
				continue;
			bmp.SetValue(tx,ty,v);
		}
	}
	DistenceMap* GetDistenceMap()
	{
		DistenceMap* distenceMap=new DistenceMap(this->bmp.Width(),this->bmp.Height(),0);
		for (int i=0; i<bmp.Width(); i++)
		{
			for (int j=0; j<bmp.Height(); j++)
			{
				if (bmp.GetValue(i, j) == 255)
				{
					distenceMap->SetValue(i,j,0);
				} 
				else
				{
					distenceMap->SetValue(i,j, bmp.Width()+bmp.Height());
					if (i>0) 
						distenceMap->SetValue(i,j,Min(distenceMap->GetValue(i,j),distenceMap->GetValue(i-1,j)+1));
					if (j>0) 
						distenceMap->SetValue(i,j,Min(distenceMap->GetValue(i,j), distenceMap->GetValue(i,j-1)+1));
				}
			}
		}

		for (int i=bmp.Width()-1; i>=0; i--)
		{
			for (int j=bmp.Height()-1; j>=0; j--)
			{
				if (i+1<bmp.Width())
					distenceMap->SetValue(i,j,Min(distenceMap->GetValue(i,j), distenceMap->GetValue(i+1,j)+1));
				if (j+1<bmp.Height()) 
					distenceMap->SetValue(i,j,Min(distenceMap->GetValue(i,j), distenceMap->GetValue(i,j+1)+1));
			}
		}
		return distenceMap;
	}
};



#endif