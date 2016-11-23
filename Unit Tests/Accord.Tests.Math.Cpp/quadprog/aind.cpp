/* aind.f -- translated by f2c (version 20100827).
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
/*  Copyright (C) 1997, 1998 */
/*  Berwin A. Turlach <bturlach@stats.adelaide.edu.au> */
/*  $Id: aind.f,v 1.3 1998/07/23 05:06:32 bturlach Exp $ */

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

/*  this routine checks whether Aind has valid entries, i.e., */
/*    1) 1<= Aind(1,i) <= n for i=1,...,q (number of constraints) */
/*    2) 1<= Aind(j,i) <= n for j=2,...,Aind(1,i)+1, i=1,...,q */

/*  Aind is a m times q matrix */

/* Subroutine */ int aind_(integer *ind, integer *m, integer *q, integer *n, 
	logical *ok)
{
    /* System generated locals */
    integer ind_dim1, ind_offset, i__1, i__2;

    /* Local variables */
    static integer i__, j;

    /* Parameter adjustments */
    ind_dim1 = *m;
    ind_offset = 1 + ind_dim1;
    ind -= ind_offset;

    /* Function Body */
    *ok = FALSE_;
    i__1 = *q;
    for (i__ = 1; i__ <= i__1; ++i__) {
	if (ind[i__ * ind_dim1 + 1] < 1 || ind[i__ * ind_dim1 + 1] > *n) {
	    return 0;
	}
	i__2 = ind[i__ * ind_dim1 + 1] + 1;
	for (j = 2; j <= i__2; ++j) {
	    if (ind[j + i__ * ind_dim1] < 1 || ind[j + i__ * ind_dim1] > *n) {
		return 0;
	    }
	}
    }
    *ok = TRUE_;
    return 0;
} /* aind_ */
}

