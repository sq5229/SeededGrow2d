#include <stdio.h>
using namespace std;
#include "DrawLine.h"

int main()
{
	ByteMatrix bmp(30,30,0);
	Point2d p0(0,0);
	Point2d p1(22,19);
	Point2d p2(4,23);
	FillTriangle::FillTriangle_Alg_3(bmp,p0,p1,p2);
	bmp.SaveRaw("FillTriangle_3.raw");

	return 0;
}