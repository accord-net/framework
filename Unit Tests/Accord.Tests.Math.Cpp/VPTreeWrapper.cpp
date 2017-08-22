#pragma once

#include "VPTreeWrapper.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace AccordTestsMathCpp2;

[assembly:AssemblyKeyFileAttribute("Accord.snk")];


VPTreeWrapper::VPTreeWrapper()
{
	tree = new VpTree<DataPoint, euclidean_distance>();
}

void VPTreeWrapper::create(array<DataPointWrapper^>^ items)
{
	data = new std::vector<DataPoint>();
	for (int i = 0; i < items->Length; i++)
		data->push_back(*items[i]->p);
	tree->create(*data);
}

void VPTreeWrapper::search(DataPointWrapper^ target, int k,
	List<DataPointWrapper^>^ results,
	List<double>^ distances)
{
	std::vector<DataPoint>* _results = new std::vector<DataPoint>();
	std::vector<double>* _distances = new std::vector<double>();
	tree->search(*target->p, k, _results, _distances);

	for (auto r : *_results)
		results->Add(gcnew DataPointWrapper(new DataPoint(r)));
	for (auto d : *_distances)
		distances->Add(d);
}

VPTreeWrapper::~VPTreeWrapper()
{
	if (tree != NULL)
		delete tree;
	if (data != NULL)
		delete tree;
}

DataPointWrapper::DataPointWrapper()
{
	p = new DataPoint();
}

DataPointWrapper::DataPointWrapper(int D, int ind, array<double>^ x)
{
	auto t = new double[D];
	Marshal::Copy(x, 0, IntPtr((void *)t), D);
	p = new DataPoint(D, ind, t);
	delete t;
}

DataPointWrapper::~DataPointWrapper()
{
	if (p != NULL)
		delete p;
}

int DataPointWrapper::index()
{
	return p->index();
}

int DataPointWrapper::dimensionality()
{
	return p->dimensionality();
}

double DataPointWrapper::x(int d)
{
	return p->x(d);
}


void VPTreeWrapper::nth_element(array<int>^ values, int n)
{
	std::vector<int> it(values->Length);
	pin_ptr<int> pin(&values[0]);
	int *first(pin), *last(pin + values->Length);

	std::nth_element(first, first + n, last);
}
