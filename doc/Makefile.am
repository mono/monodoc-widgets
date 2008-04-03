ASSEMBLY = $(top_builddir)/src/Monodoc.Widgets.dll

monodocdir = $(prefix)/lib/monodoc/sources
noinst_DATA = monodoc-widgets-docs.zip monodoc-widgets-docs.tree monodoc-widgets-docs.source

update:
	$(MONODOCER) -path:en -pretty --delete -assembly:$(ASSEMBLY)

monodoc-widgets-docs.zip monodoc-widgets-docs.tree: $(srcdir)/en/*/*.xml $(srcdir)/en/*.xml
	$(MDASSEMBLER) --ecma $(srcdir)/en -o monodoc-widgets-docs

CLEANFILES = monodoc-widgets-docs.zip monodoc-widgets-docs.tree

EXTRA_DIST = monodoc-widgets-docs.source

NAMESPACES = Monodoc.Widgets

dist-hook:
	mkdir -p $(distdir)/en
	cp $(srcdir)/en/*.xml $(distdir)/en/
	for i in $(NAMESPACES); do				\
		mkdir -p $(distdir)/en/$$i;			\
		cp $(srcdir)/en/$$i/*.xml $(distdir)/en/$$i;	\
	done

