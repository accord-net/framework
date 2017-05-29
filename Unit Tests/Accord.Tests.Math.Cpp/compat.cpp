#include <io.h>
#include <stdio.h>

extern "C" { FILE __iob_func[3] = { *stdin, *stdout, *stderr }; }

