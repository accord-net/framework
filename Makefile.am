SUBDIRS = 
DIST_SUBDIRS = 

DIR_DEBUG = $(srcdir)/Debug
DIR_RELEASE = $(srcdir)/Release
LIBDIR = $(srcdir)/Release/mono

ACCORD_ASSEMBLY_PC = $(srcdir)/$(PACKAGE_NAME).pc

DIR_TEST = "$(srcdir)/Unit Tests"
NUNIT    = "$(srcdir)/Externals/NUnit/NUnit.Console-3.6.1/nunit3-console.exe"

LIBFILES = \
	$(LIBDIR)/Accord.Vision.dll \
	$(LIBDIR)/Accord.Video.Ximea.dll \
	$(LIBDIR)/Accord.Video.VFW.dll \
	$(LIBDIR)/Accord.Video.Kinect.dll \
	$(LIBDIR)/Accord.Video.dll \
	$(LIBDIR)/Accord.Video.DirectShow.dll \
	$(LIBDIR)/Accord.Neuro.dll \
	$(LIBDIR)/Accord.Math.dll \
	$(LIBDIR)/Accord.Math.Core.dll \
	$(LIBDIR)/Accord.MachineLearning.dll \
	$(LIBDIR)/Accord.Text.dll \
	$(LIBDIR)/Accord.IO.dll \
	$(LIBDIR)/Accord.DataSets.dll \
	$(LIBDIR)/Accord.Imaging.dll \
	$(LIBDIR)/Accord.Genetic.dll \
	$(LIBDIR)/Accord.Fuzzy.dll \
	$(LIBDIR)/Accord.dll \
	$(LIBDIR)/Accord.Controls.Vision.dll \
	$(LIBDIR)/Accord.Controls.Imaging.dll \
	$(LIBDIR)/Accord.Controls.Audio.dll \
	$(LIBDIR)/Accord.Audition.dll \
	$(LIBDIR)/Accord.Audio.dll 

TESTFILES = \
	${DIR_TEST}/Accord.Tests.Controls/bin/mono/AnyCPU/Accord.Tests.Controls.dll \
	${DIR_TEST}/Accord.Tests.Core/bin/mono/AnyCPU/Accord.Tests.Core.dll \
	${DIR_TEST}/Accord.Tests.Imaging/bin/mono/AnyCPU/Accord.Tests.Imaging.dll \
	${DIR_TEST}/Accord.Tests.IO/bin/mono/AnyCPU/Accord.Tests.IO.dll \
	${DIR_TEST}/Accord.Tests.MachineLearning/bin/mono/AnyCPU/Accord.Tests.MachineLearning.dll \
	${DIR_TEST}/Accord.Tests.Math/bin/mono/AnyCPU/Accord.Tests.Math.dll \
	${DIR_TEST}/Accord.Tests.Neuro/bin/mono/AnyCPU/Accord.Tests.Neuro.dll \
	${DIR_TEST}/Accord.Tests.Vision/bin/mono/AnyCPU/Accord.Tests.Vision.dll \
	${DIR_TEST}/Accord.Tests.Statistics/bin/mono/AnyCPU/Accord.Tests.Statistics.dll 


DEBUG_FILES = $(addsuffix .mdb, $(LIBFILES))
RELEASE_FILE = $(PACKAGE)-$(VERSION)


#magic automake variables
pkgconfigdir = $(LIBDIR)/pkgconfig
pkgconfig_DATA = $(ACCORD_ASSEMBLY_PC)
#end



# target: all - Default target: build
all: build

# target: help - Display callable targets.
help:
	egrep "^# target:" Makefile | sed 's/^# target:/make/'

# target: build - Build Accord.NET Framework
build: Sources/Accord.NET.sln
	$(XBUILD) /p:Configuration=Mono Sources/Accord.NET.sln

# target: samples - Build sample applications
samples: Samples/Samples.sln
	$(XBUILD) /p:Configuration=Mono /p:Platform=x86 Samples/Samples.sln

# target: docs - Build documentation
docs:
	mdoc update --lib=Release/mono/ --out=Docs/monodoc Release/mono/*.dll
	mdoc export-html -o htmldocs docfiles

# target: test - Run unit tests
# Notes:
#    Timeout: 30 min = 30 * 60 * 10000 = 1800000
#    First line runs as 64-bit, second line as 32-bits
#
test: build
	$(MONO) --runtime=v4.0 ${NUNIT} --labels=All --timeout=1800000 \
		--where "cat != Serialization && cat != Intensive && cat != WinForms && cat != Office && cat != MonoNotSupported && cat != Random" \
		${TESTFILES}
	$(MONO) --runtime=v4.0 ${NUNIT} --labels=All --timeout=1800000 --x86 \
		--where "cat != Serialization && cat != Intensive && cat != WinForms && cat != Office && cat != MonoNotSupported && cat != Random" \
		${TESTFILES}
		
test-reduced: build
	$(MONO) --runtime=v4.0 ${NUNIT} --labels=All --timeout=1800000 \
		--where "cat != Serialization && cat != Intensive && cat != WinForms && cat != Office && cat != MonoNotSupported && cat != Slow && cat != Random" \
		${TESTFILES}
	$(MONO) --runtime=v4.0 ${NUNIT} --labels=All --timeout=1800000 --x86 \
		--where "cat != Serialization && cat != Intensive && cat != WinForms && cat != Office && cat != MonoNotSupported && cat != Slow && cat != Random" \
		${TESTFILES}

# target: clean - remove all obj and bin directories
clean-local: 
	rm -rvf $(LIBDIR)/*
	rm -rvf $(DIR_RELEASE)/*
	rm -rvf $(DIR_DEBUG)/*
	find $(srcdir)/ -type d -name obj | xargs rm -rvf

maintainer-clean-local:
	rm -rf autom4te.cache

install-exec-local:
	mkdir -p $(DESTDIR)$(libdir)/$(PACKAGE)
	$(INSTALL_SCRIPT) -t $(DESTDIR)$(libdir)/$(PACKAGE)/ $(LIBFILES)

uninstall-local: 
	rm -rf $(DESTDIR)$(libdir)/$(PACKAGE)

MAINTAINERCLEANFILES = Makefile.in \
		       install-sh \
		       configure \
		       aclocal.m4 \
		       missing \
		       compile

.PHONY: install uninstall cleanbin cleanobj 
