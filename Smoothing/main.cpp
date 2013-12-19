#define _CRT_SECURE_NO_DEPRECATE
#include <iostream>
#include "Smooth.h"
#include "PlyManager.h"

int main()
{
	Mesh m;
	PlyManager::ReadFile(m,"D:\\VTKproj\\engine.ply");
	Smoothing sm(&m);
	sm.HCLaplacian(3,0.2f,0.2f);
	PlyManager::Output(m,"test.ply");
	return 0;
}