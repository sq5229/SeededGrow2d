#include <stdio.h>
#include "Base.h"
#include "Erode.h"
#include "Dilate.h"
#define main3 main

int main1_Astar()
{
	Bitmap2d bmp(512,512,0);
	bmp.ReadRaw("D://VTKproj//engine_slice512.raw");
	ErodeProcessor ep(bmp,6,SQUARE);
	Bitmap2d* ret=ep.Execute();
	printf("method1_%d\n",bmp.visit_count+ret->visit_count);
	bmp.visit_count=0;
	Bitmap2d* ret2=ep.Execute2();
	printf("method2_%d\n",bmp.visit_count+ret2->visit_count);
	bmp.visit_count=0;
	Bitmap2d* ret3=ep.Execute3();
	printf("method3_%d\n",bmp.visit_count+ret3->visit_count);
	bmp.visit_count=0;
	Bitmap2d* ret4=ep.Execute4();
	printf("method4_%d\n",bmp.visit_count+ret4->visit_count);

	ret->SaveRaw("D://VTKproj//engine_slice512_ero.raw");
	ret2->SaveRaw("D://VTKproj//engine_slice512_ero2.raw");
	ret3->SaveRaw("D://VTKproj//engine_slice512_ero3.raw");
	ret4->SaveRaw("D://VTKproj//engine_slice512_ero4.raw");
	system("pause");
	return 0;
}
int main2_Dijk()
{
	Bitmap2d bmp(512,512,0);
	bmp.ReadRaw("D://VTKproj//engine_slice512.raw");
	DilateProcessor ep(bmp,6,SQUARE2);
	Bitmap2d* ret=ep.Execute();
	printf("method1_%d\n",bmp.visit_count+ret->visit_count);
	bmp.visit_count=0;
	Bitmap2d* ret2=ep.Execute2();
	printf("method2_%d\n",bmp.visit_count+ret2->visit_count);
	bmp.visit_count=0;
	Bitmap2d* ret3=ep.Execute3();
	printf("method3_%d\n",bmp.visit_count+ret3->visit_count);


	ret->SaveRaw("D://VTKproj//engine_slice512_dia.raw");
	ret2->SaveRaw("D://VTKproj//engine_slice512_dia2.raw");
	ret3->SaveRaw("D://VTKproj//engine_slice512_dia3.raw");
	system("pause");
	return 0;
}
int main3()
{
	main1_Astar();
	//main2();
}