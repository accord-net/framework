/* achck.f -- translated by f2c (version 20100827).
   You must link the resulting object file with libf2c:
	on Microsoft Windows system, link with libf2c.lib;
	on Linux or Unix systems, link with .../path/to/libf2c.a -lm
	or, if you install libf2c.a in a standard place, with -lf2c -lm
	-- in that order, at the end of the command line, as in
		cc *.o -lf2c -lm
	Source for libf2c is in /netlib/f2c/libf2c.zip, e.g.,

		http://www.netlib.org/f2c/libf2c.zip
*/

#include "f2c.h"

extern "C"
{
/*  Copyright (C) 1998 */
/*  Berwin A. Turlach <bturlach@stats.adelaide.edu.au> */
/*  $Id: achck.f,v 1.2 1998/07/23 05:05:58 bturlach Exp $ */

/*  This library is free software; you can redistribute it and/or */
/*  modify it under the terms of the GNU Library General Public */
/*  License as published by the Free Software Foundation; either */
/*  version 2 of the License, or (at your option) any later version. */

/*  This library is distributed in the hope that it will be useful, */
/*  but WITHOUT ANY WARRANTY; without even the implied warranty of */
/*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU */
/*  Library General Public License for more details. */

/*  You should have received a copy of the GNU Library General Public */
/*  License along with this library; if not, write to the Free Software */
/*  Foundation, Inc., 59 Temple Place, Suite 330, Boston, */
/*  MA 02111-1307 USA */

/*  this routine checks whether all constraints are fulfilled. */

/* Subroutine */ int achck_(doublereal *sol, integer *n, doublereal *amat, 
	integer *aind, doublereal *bvec, integer *m, integer *q, integer *meq,
	 doublereal *prec, logical *ok)
{
    /* System generated locals */
    integer aind_dim1, aind_offset, amat_dim1, amat_offset, i__1, i__2;

    /* Local variables */
    static integer i__, j;
    static doublereal sum;

    /* Parameter adjustments */
    --sol;
    --bvec;
    aind_dim1 = *m + 1;
    aind_offset = 1 + aind_dim1;
    aind -= aind_offset;
    amat_dim1 = *m;
    amat_offset = 1 + amat_dim1;
    amat -= amat_offset;

    /* Function Body */
    *ok = FALSE_;
    i__1 = *q;
    for (i__ = 1; i__ <= i__1; ++i__) {
	sum = -bvec[i__];
	i__2 = aind[i__ * aind_dim1 + 1];
	for (j = 1; j <= i__2; ++j) {
	    sum += amat[j + i__ * amat_dim1] * sol[aind[j + 1 + i__ * 
		    aind_dim1]];
	}
	if (i__ <= *meq) {
	    sum = -abs(sum);
	}
	if (sum < -(*prec)) {
	    return 0;
	}
    }
    *ok = TRUE_;
    return 0;
} /* achck_ */
}
