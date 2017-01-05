dnl Process this file with autoconf to produce a configure script.
AC_INIT([accord], m4_esyscmd([tr -d '\n' < Version.txt]))
AM_INIT_AUTOMAKE([tar-ustar foreign])

AC_PROG_INSTALL

dnl pkg-config
PKG_PROG_PKG_CONFIG([0.20])

AC_PATH_PROG(XBUILD, xbuild, no)
AC_PATH_PROG(MONO, mono, no)

if test "x$XBUILD" = "xno"; then
	AC_MSG_ERROR([XBuild not found])
fi

if test "x$MONO" = "xno"; then
	AC_MSG_ERROR([No mono runtime found])
fi

# Check Mono version
MONO_MINIMUM_VERSION=2.8
PKG_CHECK_MODULES(MONO, mono >= $MONO_MINIMUM_VERSION)

AC_SUBST(XBUILD)
AC_SUBST(MONO)


AC_CONFIG_FILES([
Makefile
accord.pc
])

AC_OUTPUT

cat <<EOF

${PACKAGE}-${VERSION}

  Build Environment
    Install Prefix:     	${prefix}
    XBuild Path:		${XBUILD}
    Mono Runtime:       	${MONO}
    Installation Directory: 	${prefix}/lib/${PACKAGE}/

EOF
