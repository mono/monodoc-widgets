TARGETS = browser.exe 

references=$(GTK_SHARP_LIBS) $(MONODOC_LIBS) /r:Monodoc.Widgets.dll

noinst_SCRIPTS = $(TARGETS)
CLEANFILES = $(TARGETS) Monodoc.Widgets.dll

Monodoc.Widgets.dll: $(top_builddir)/src/Monodoc.Widgets.dll
	cp $(top_builddir)/src/Monodoc.Widgets.dll .

browser.exe: $(srcdir)/Browser.cs Monodoc.Widgets.dll
	$(GMCS) /out:browser.exe $(references) $(srcdir)/Browser.cs

run: browser.exe
	mono browser.exe $(top_srcdir)/doc/en

EXTRA_DIST = 				\
	Browser.cs

