#include "vptree.h"

double euclidean_distance(const DataPoint &t1, const DataPoint &t2) {
	double dd = .0;
	double* x1 = t1._x;
	double* x2 = t2._x;
	double diff;
	for (int d = 0; d < t1._D; d++) {
		diff = (x1[d] - x2[d]);
		dd += diff * diff;
	}
	return sqrt(dd);
}

