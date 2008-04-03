assemblydir=$(prefix)/lib/monodoc-widgets

ASSEMBLY = Monodoc.Widgets.dll
TARGETS = $(ASSEMBLY) $(ASSEMBLY).mdb

CLEANFILES =  $(TARGETS) 
noinst_DATA = $(TARGETS)

DISTCLEANFILES = AssemblyInfo.cs 

sources = 				\
	$(srcdir)/BannerItem.cs		\
	$(srcdir)/Catalog.cs		\
	$(srcdir)/CatalogStore.cs	\
	$(srcdir)/DocNode.cs		\
	$(srcdir)/DocsElement.cs	\
	$(srcdir)/DocTree.cs		\
	$(srcdir)/DocView.cs		\
	$(srcdir)/DocViewItem.cs	\
	$(srcdir)/HeadingItem.cs	\
	$(srcdir)/KitchenSinkItem.cs	\
	$(srcdir)/LabelItem.cs		\
	$(srcdir)/LinkItem.cs		\
	$(srcdir)/MarkupItem.cs		\
	$(srcdir)/MemberNode.cs		\
	$(srcdir)/MembersNode.cs	\
	$(srcdir)/MemberSubsetNode.cs	\
	$(srcdir)/NamespaceNode.cs	\
	$(srcdir)/OverloadNode.cs	\
	$(srcdir)/RemarksItem.cs	\
	$(srcdir)/SummaryTableItem.cs	\
	$(srcdir)/TypeNode.cs
	

references = $(GTK_SHARP_LIBS) $(MONODOC_LIBS) 

EXTRA_DIST = 				\
	$(sources)			\
	AssemblyInfo.cs.in

$(TARGETS): $(sources) AssemblyInfo.cs 
	$(GMCS) -debug -target:library -out:$(ASSEMBLY) $(sources) AssemblyInfo.cs $(references) 

