#pragma once

#include "bhtsne\vptree.h"
#include <vector>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;



namespace AccordTestsMathCpp2
{


	public ref class DataPointWrapper
	{
	internal:
		DataPoint* p = NULL;

		DataPointWrapper(DataPoint* p) 
		{
			this->p = p;
		}

	public:
		DataPointWrapper();

		DataPointWrapper(int D, int ind, array<double>^ x);

		~DataPointWrapper();

		int index();
		int dimensionality();
		double x(int d);
	};



	public ref class VPTreeWrapper
	{
		VpTree<DataPoint, euclidean_distance>* tree = NULL;
		std::vector<DataPoint>* data = NULL;

	public:
		VPTreeWrapper();

		void create(array<DataPointWrapper^>^ items);

		void search(DataPointWrapper^ target, int k,
			List<DataPointWrapper^>^ results,
			List<double>^ distances);

		static void nth_element(array<int>^ values, int n);

		~VPTreeWrapper();
	};

}
