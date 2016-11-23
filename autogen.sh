#! /bin/sh

# Compares software version numbers
# 10 means EQUAL
# 11 means GREATER THAN
# 9 means LESS THAN
check_version() {
	test -z "$1" && return 1
	local ver1=$1
	while test `echo $ver1 | egrep -c [^0123456789.]` -gt 0 ; do
		char=`echo $ver1 | sed 's/.*\([^0123456789.]\).*/\1/'`
		char_dec=`echo -n "$char" | od -b | head -1 | awk {'print $2'}`
		ver1=`echo $ver1 | sed "s/$char/.$char_dec/g"`
	done	
	test -z "$2" && return 1
	local ver2=$2
	while test `echo $ver2 | egrep -c [^0123456789.]` -gt 0 ; do
		char=`echo $ver2 | sed 's/.*\([^0123456789.]\).*/\1/'`
		char_dec=`echo -n "$char" | od -b | head -1 | awk {'print $2'}`
		ver2=`echo $ver2 | sed "s/$char/.$char_dec/g"`
	done	

	ver1=`echo $ver1 | sed 's/\.\./.0/g'`
	ver2=`echo $ver2 | sed 's/\.\./.0/g'`

	do_version_check "$ver1" "$ver2"
}

do_version_check() {
	
	test "$1" -eq "$2" && return 10

	ver1front=`echo $1 | cut -d "." -f -1`
	ver1back=`echo $1 | cut -d "." -f 2-`
	ver2front=`echo $2 | cut -d "." -f -1`
	ver2back=`echo $2 | cut -d "." -f 2-`

	if test "$ver1front" != "$1"  || test "$ver2front" != "$2" ; then
		test "$ver1front" -gt "$ver2front" && return 11
		test "$ver1front" -lt "$ver2front" && return 9

		test "$ver1front" -eq "$1" || test -z "$ver1back" && ver1back=0
		test "$ver2front" -eq "$2" || test -z "$ver2back" && ver2back=0
		do_version_check "$ver1back" "$ver2back"
		return $?
	else
		test "$1" -gt "$2" && return 11 || return 9
	fi
}

PROJECT=Accord.NET
FILE=
CONFIGURE=configure.ac

: ${AUTOCONF=autoconf}
: ${AUTOHEADER=autoheader}
: ${AUTOMAKE=automake}
: ${ACLOCAL=aclocal}
: ${MONO=mono}
: ${XBUILD=xbuild}

DIE=0

($AUTOCONF --version) < /dev/null > /dev/null 2>&1 || {
        echo
        echo "You must have autoconf installed to compile $PROJECT."
        echo "Download the appropriate package for your distribution,"
        echo "or get the source tarball at ftp://ftp.gnu.org/pub/gnu/"
		echo 
		echo "On apt-get systems, you might want to run 'sudo apt-get install autoconf'"
        DIE=1
}

($AUTOMAKE --version) < /dev/null > /dev/null 2>&1 || {
        echo
        echo "You must have automake installed to compile $PROJECT."
        echo "Get ftp://sourceware.cygnus.com/pub/automake/automake-1.4.tar.gz"
        echo "(or a newer version if it is available)"
		echo
		echo "On apt-get systems, you might want to run 'sudo apt-get install automake'"
        DIE=1
}

($MONO --version) < /dev/null > /dev/null 2>&1 || {
        echo
        echo "You must have mono runtime installed to compile $PROJECT."
		echo
		echo "On apt-get systems, you might want to run 'sudo apt-get install mono-complete'"
        DIE=1
}

($XBUILD /version) < /dev/null > /dev/null 2>&1 || {
        echo
        echo "You must have mono-xbuild installed to compile $PROJECT."
		echo
		echo "On apt-get systems, you might want to run 'sudo apt-get install mono-xbuild'"
        DIE=1
}

if test "$DIE" -eq 1; then
        exit 1
fi

unset GREP_OPTIONS

xbuild_version=`xbuild /version | grep '^XBuild' | egrep -o '([0-9]+\.?){2,}'`
check_version "$xbuild_version" "2.4" 2> /dev/null
if test $? -eq 9; then
	echo
	echo "A newer version of XBuild is required to build $PROJECT ( >= 2.4 )"
	exit 1
fi

mono_version=`xbuild /version | grep '^Mono' | egrep -o '([0-9]+\.?){2,}'`
check_version "$xbuild_version" "2.4" 2> /dev/null
if test $? -eq 9; then
	echo
	echo "A newer version of Mono is required to run $PROJECT ( >= 2.4 )"
	exit 1
fi


#Check directoy 
srcdir=`dirname $0`
test -z "$srcdir" && srcdir=.

ORIGDIR=`pwd`
cd $srcdir
TEST_TYPE=-f
aclocalinclude="-I . $ACLOCAL_FLAGS"

                                                                     
test $TEST_TYPE $FILE || {
        echo "You must run this script in the top-level $PROJECT directory"
        exit 1
}

if test -z "$*"; then
        echo "I am going to run ./configure with no arguments - if you wish "
        echo "to pass any to it, please specify them on the $0 command line."
fi

echo "Running $ACLOCAL $aclocalinclude ..."
$ACLOCAL $aclocalinclude

echo "Running $AUTOMAKE --gnu $am_opt ..."
$AUTOMAKE --add-missing --gnu $am_opt

echo "Running $AUTOCONF ..."
$AUTOCONF

echo Running $srcdir/configure $conf_flags "$@" ...
$srcdir/configure $conf_flags "$@"