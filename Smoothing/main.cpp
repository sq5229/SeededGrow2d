#define _CRT_SECURE_NO_DEPRECATE
#include <iostream>
#include "Smooth.h"
#include "PlyManager.h"


void Test1()
{
	Mesh m;
	PlyManager::ReadFile(m,"D:\\VTKproj\\engine.ply");
	Smoothing sm(&m);
	sm.ScaleDependentLaplacian(3);
	PlyManager::Output(m,"test1.ply");
}
void Test2()
{
	Mesh m;
	PlyManager::ReadFile(m,"D:\\VTKproj\\engine.ply");
	Smoothing sm(&m);
	sm.CotWeightedLaplacian(3);
	PlyManager::Output(m,"test2.ply");
}
void Test3()
{
	Mesh m;
	PlyManager::ReadFile(m,"D:\\VTKproj\\engine.ply");
	Smoothing sm(&m);
	sm.Taubin(5,0.5f,-0.5f);
	PlyManager::Output(m,"test3.ply");
}
void Test4()
{
	Mesh m;
	PlyManager::ReadFile(m,"D:\\VTKproj\\engine.ply");
	Smoothing sm(&m);
	sm.HCLaplacian(3,0.2f,0.2f);
	PlyManager::Output(m,"test4.ply");
}

int main()
{
	Test1();
	Test2();
	Test3();
	Test4();
	return 0;
}