/* timer.f -- translated by f2c (version 20100827).
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

/*  L-BFGS-B is released under the “New BSD License” (aka “Modified BSD License */
/*  or “3-clause license”) */
/*  Please read attached file License.txt */

/* Subroutine */ int timer_(doublereal *ttime)
{
    extern /* Subroutine */ int cpu_time__(real *);
    static real temp;



/*     This routine computes cpu time in double precision; it makes use of */
/*     the intrinsic f90 cpu_time therefore a conversion type is */
/*     needed. */

/*           J.L Morales  Departamento de Matematicas, */
/*                        Instituto Tecnologico Autonomo de Mexico */
/*                        Mexico D.F. */

/*           J.L Nocedal  Department of Electrical Engineering and */
/*                        Computer Science. */
/*                        Northwestern University. Evanston, IL. USA */

/*                        January 21, 2011 */

    temp = (real) (*ttime);
    //cpu_time__(&temp);
    *ttime = (doublereal) 0;
    return 0;
} /* timer_ */

}