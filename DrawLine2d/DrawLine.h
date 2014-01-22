#ifndef DRAWLINE_H
#define DRAWLINE_H
#include "ByteMatrix.h"
#include <algorithm>
#include <vector>
#include <queue>
#include <math.h>
class DrawLine
{
public:
static void DrawLine_DDA(ByteMatrix& bmp,Point2d p0,Point2d p1)
{
	int dx=p1.X-p0.X;
	int dy=p1.Y-p0.Y;
	if(abs(dx)>abs(dy))
	{
		if(p0.X>p1.X)
		{
			Point2d temp=p1;
			p1=p0;
			p0=temp;
		}
		for(int i=p0.X;i<=p1.X;i++)
		{
			float y=dy*(i-p0.X)/dx+p0.Y;
			bmp.SetValue(i,Trunc(y),255);
		}
	}
	else
	{
		if(p0.Y>p1.Y)
		{
			Point2d temp=p1;
			p1=p0;
			p0=temp;
		}
		for(int i=p0.Y;i<=p1.Y;i++)
		{
			float x=dx*(i-p0.Y)/dy+p0.X;
			bmp.SetValue(Trunc(x),i,255);
		}
	}
}
static void DrawLine_Bresenham(ByteMatrix& bmp,Point2d p0,Point2d p1)
{
	int y1=p0.Y;
	int x1=p0.X;
	int y2=p1.Y;
	int x2=p1.X;
	// Bresenham's line algorithm
	const bool steep = (abs(y2 - y1) > abs(x2 - x1));
	if(steep)
	{
		std::swap(x1, y1);
		std::swap(x2, y2);
	}

	if(x1 > x2)
	{
		std::swap(x1, x2);
		std::swap(y1, y2);
	}

	const float dx = x2 - x1;
	const float dy = abs(y2 - y1);

	float error = dx / 2.0f;
	const int ystep = (y1 < y2) ? 1 : -1;
	int y = (int)y1;

	const int maxX = (int)x2;

	for(int x=(int)x1; x<maxX; x++)
	{
		if(steep)
		{
			bmp.SetValue(y,x, 255);
		}
		else
		{
			bmp.SetValue(x,y, 255);
		}

		error -= dy;
		if(error < 0)
		{
			y += ystep;
			error += dx;
		}
	}
}
static void DrawSuperCoverLine_Simple(ByteMatrix& bmp,Point2d p0,Point2d p1)
{
	int dx=p1.X-p0.X;
	int dy=p1.Y-p0.Y;
	if(abs(dx)>abs(dy))
	{
		if(p0.X>p1.X)
		{
			Point2d temp=p1;
			p1=p0;
			p0=temp;
		}
		for(float i=p0.X+0.5f;i<=p1.X;i+=1.0f)
		{
			float y=dy*(i-p0.X)/dx+p0.Y;
			bmp.SetValue((int)(i-0.5f),Trunc(y),255);
			bmp.SetValue((int)(i+0.5f),Trunc(y),255);
		}
	}
	else
	{
		if(p0.Y>p1.Y)
		{
			Point2d temp=p1;
			p1=p0;
			p0=temp;
		}
		for(float i=p0.Y+0.5f;i<=p1.Y;i+=1.0f)
		{
			float x=dx*(i-p0.Y)/dy+p0.X;
			bmp.SetValue(Trunc(x),(int)(i-0.5f),255);
			bmp.SetValue(Trunc(x),(int)(i+0.5f),255);
		}
	}
}
static void DrawSuperCoverLine_Bresenham(ByteMatrix& bmp,Point2d p0,Point2d p1)
{
	int y1=p0.Y;
	int x1=p0.X;
	int y2=p1.Y;
	int x2=p1.X;
	int i;               // loop counter 
	int ystep, xstep;    // the step on y and x axis 
	int error;           // the error accumulated during the increment 
	int errorprev;       // *vision the previous value of the error variable 
	int y = y1, x = x1;  // the line points 
	int ddy, ddx;        // compulsory variables: the double values of dy and dx 
	int dx = x2 - x1; 
	int dy = y2 - y1; 
	bmp.SetValue(x1, y1,255);  // first point 
	// NB the last point can't be here, because of its previous point (which has to be verified) 
	if (dy < 0){ 
		ystep = -1; 
		dy = -dy; 
	}else 
		ystep = 1; 
	if (dx < 0){ 
		xstep = -1; 
		dx = -dx; 
	}else 
		xstep = 1; 
	ddy = 2 * dy;  // work with double values for full precision 
	ddx = 2 * dx; 
	if (ddx >= ddy){  // first octant (0 <= slope <= 1) 
		// compulsory initialization (even for errorprev, needed when dx==dy) 
		errorprev = error = dx;  // start in the middle of the square 
		for (i=0 ; i < dx ; i++){  // do not use the first point (already done) 
			x += xstep; 
			error += ddy; 
			if (error > ddx){  // increment y if AFTER the middle ( > ) 
				y += ystep; 
				error -= ddx; 
				// three cases (octant == right->right-top for directions below): 
				if (error + errorprev < ddx)  // bottom square also 
					bmp.SetValue(x,y-ystep,255); 
				else if (error + errorprev > ddx)  // left square also 
					bmp.SetValue(x-xstep,y ,255); 
				else{  // corner: bottom and left squares also 
					bmp.SetValue(x,y-ystep,255); 
					bmp.SetValue(x-xstep,y,255); 
				} 
			} 
			bmp.SetValue(x,y,255); 
			errorprev = error; 
		} 
	}else{  // the same as above 
		errorprev = error = dy; 
		for (i=0 ; i < dy ; i++){ 
			y += ystep; 
			error += ddx; 
			if (error > ddy){ 
				x += xstep; 
				error -= ddy; 
				if (error + errorprev < ddy) 
					bmp.SetValue(x-xstep,y,255); 
				else if (error + errorprev > ddy) 
					bmp.SetValue(x,y-ystep,255); 
				else{ 
					bmp.SetValue( x-xstep,y,255); 
					bmp.SetValue( x,y-ystep,255); 
				} 
			} 
			bmp.SetValue( x,y,255); 
			errorprev = error; 
		} 
	} 
	// assert ((y == y2) && (x == x2));  // the last point (y2,x2) has to be the same with the last point of the algorithm 
}

private:
	static int Trunc(float n)
	{
		return (int)(n+0.5f);
	}
};

class FillTriangle
{
public: 
	static void FillTriangle_Alg_1(ByteMatrix& bmp,Point2d p0,Point2d p1,Point2d p2)
	{
		Box2d box;
		box.UpdateRange(p0.X,p0.Y);
		box.UpdateRange(p1.X,p1.Y);
		box.UpdateRange(p2.X,p2.Y);
		for(int x=box.XMin;x<=box.XMax;x++)
		{
			for(int y=box.YMin;y<=box.YMax;y++)
			{
				if(PointInTriangle(Point2d(x,y),p0,p1,p2))
				{
					bmp.SetValue(x,y,255);
				}
			}
		}
	}
static void FillTriangle_Alg_2(ByteMatrix& bmp,Point2d p0,Point2d p1,Point2d p2)
{
	std::vector<Point2d> plist;
	Bresenham(bmp,p1,p2,plist);
	for(int i=0;i<plist.size();i++)
	{
		SuperCover(bmp,plist[i],p0);
	}
}
static void FillTriangle_Alg_3(ByteMatrix& bmp,Point2d p0,Point2d p1,Point2d p2)
{
	Box2d box;
	box.UpdateRange(p0.X,p0.Y);
	box.UpdateRange(p1.X,p1.Y);
	box.UpdateRange(p2.X,p2.Y);
	Point2d seed((box.XMin+box.XMax)/2,(box.YMin+box.YMax)/2);
	SuperCover(bmp,p0,p1);
	SuperCover(bmp,p0,p2);
	SuperCover(bmp,p1,p2);
	FloodFill(bmp,seed);
}
private:
	static void FloodFill(ByteMatrix& bmp,Point2d seed)
	{
		std::queue<Point2d> queue;
		queue.push(seed);
		bmp.SetValue(seed.X,seed.Y,255);
		Point2d temp[8];
		while(!queue.empty())
		{
			Point2d p=queue.front();
			queue.pop();
			Init8AdjPixel(temp,p);
			for(int i=0;i<8;i++)
			{
				Point2d t=temp[i];
				if(bmp.GetValue(t.X,t.Y)!=255)
				{
					bmp.SetValue(t.X,t.Y,255);
					queue.push(t);
				}
			}
		}
	}
static float sign(Point2d p1, Point2d p2, Point2d p3)
{
	return (float)((p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X- p3.X) * (p1.Y - p3.Y));
}
static bool PointInTriangle(Point2d pt, Point2d v1, Point2d v2, Point2d v3)
{
	bool b1, b2, b3;

	b1 = sign(pt, v1, v2) < 0.0f;
	b2 = sign(pt, v2, v3) < 0.0f;
	b3 = sign(pt, v3, v1) < 0.0f;

	return ((b1 == b2) && (b2 == b3));
}
	static void Bresenham(ByteMatrix& bmp,Point2d p0,Point2d p1,std::vector<Point2d>& plist)
	{
		int y1=p0.Y;
		int x1=p0.X;
		int y2=p1.Y;
		int x2=p1.X;
		// Bresenham's line algorithm
		const bool steep = (abs(y2 - y1) > abs(x2 - x1));
		if(steep)
		{
			std::swap(x1, y1);
			std::swap(x2, y2);
		}

		if(x1 > x2)
		{
			std::swap(x1, x2);
			std::swap(y1, y2);
		}

		const float dx = x2 - x1;
		const float dy = abs(y2 - y1);

		float error = dx / 2.0f;
		const int ystep = (y1 < y2) ? 1 : -1;
		int y = (int)y1;

		const int maxX = (int)x2;

		for(int x=(int)x1; x<maxX; x++)
		{
			if(steep)
			{
				bmp.SetValue(y,x, 255);
				plist.push_back(Point2d(y,x));
			}
			else
			{
				bmp.SetValue(x,y, 255);
				plist.push_back(Point2d(x,y));
			}

			error -= dy;
			if(error < 0)
			{
				y += ystep;
				error += dx;
			}
		}
	}
	static void SuperCover(ByteMatrix& bmp,Point2d p0,Point2d p1)
	{
		int y1=p0.Y;
		int x1=p0.X;
		int y2=p1.Y;
		int x2=p1.X;
		int i;               // loop counter 
		int ystep, xstep;    // the step on y and x axis 
		int error;           // the error accumulated during the increment 
		int errorprev;       // *vision the previous value of the error variable 
		int y = y1, x = x1;  // the line points 
		int ddy, ddx;        // compulsory variables: the double values of dy and dx 
		int dx = x2 - x1; 
		int dy = y2 - y1; 
		bmp.SetValue(x1, y1,255);  // first point 
		// NB the last point can't be here, because of its previous point (which has to be verified) 
		if (dy < 0){ 
			ystep = -1; 
			dy = -dy; 
		}else 
			ystep = 1; 
		if (dx < 0){ 
			xstep = -1; 
			dx = -dx; 
		}else 
			xstep = 1; 
		ddy = 2 * dy;  // work with double values for full precision 
		ddx = 2 * dx; 
		if (ddx >= ddy){  // first octant (0 <= slope <= 1) 
			// compulsory initialization (even for errorprev, needed when dx==dy) 
			errorprev = error = dx;  // start in the middle of the square 
			for (i=0 ; i < dx ; i++){  // do not use the first point (already done) 
				x += xstep; 
				error += ddy; 
				if (error > ddx){  // increment y if AFTER the middle ( > ) 
					y += ystep; 
					error -= ddx; 
					// three cases (octant == right->right-top for directions below): 
					if (error + errorprev < ddx)  // bottom square also 
						bmp.SetValue(x,y-ystep,255); 
					else if (error + errorprev > ddx)  // left square also 
						bmp.SetValue(x-xstep,y ,255); 
					else{  // corner: bottom and left squares also 
						bmp.SetValue(x,y-ystep,255); 
						bmp.SetValue(x-xstep,y,255); 
					} 
				} 
				bmp.SetValue(x,y,255); 
				errorprev = error; 
			} 
		}else{  // the same as above 
			errorprev = error = dy; 
			for (i=0 ; i < dy ; i++){ 
				y += ystep; 
				error += ddx; 
				if (error > ddy){ 
					x += xstep; 
					error -= ddy; 
					if (error + errorprev < ddy) 
						bmp.SetValue(x-xstep,y,255); 
					else if (error + errorprev > ddy) 
						bmp.SetValue(x,y-ystep,255); 
					else{ 
						bmp.SetValue( x-xstep,y,255); 
						bmp.SetValue( x,y-ystep,255); 
					} 
				} 
				bmp.SetValue( x,y,255); 
				errorprev = error; 
			} 
		} 
		// assert ((y == y2) && (x == x2));  // the last point (y2,x2) has to be the same with the last point of the algorithm 
	}
	static void Init8AdjPixel(Point2d* temp,Point2d p)
	{
		temp[0].X=p.X-1;
		temp[0].Y=p.Y-1;

		temp[1].X=p.X;
		temp[1].Y=p.Y-1;

		temp[2].X=p.X+1;
		temp[2].Y=p.Y-1;

		temp[3].X=p.X-1;
		temp[3].Y=p.Y;

		temp[4].X=p.X+1;
		temp[4].Y=p.Y;

		temp[5].X=p.X-1;
		temp[5].Y=p.Y+1;

		temp[6].X=p.X;
		temp[6].Y=p.Y+1;

		temp[7].X=p.X+1;
		temp[7].Y=p.Y+1;
	}
};

#endif

