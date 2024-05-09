This is a demo project in progress to get more familiar with MVP pattern for UI development as my previous 
tools were more like prototypes (functional for multiple teams to use but still prototypes and not ideal for
easy re-use). All windows will include at least a View and presenter if not a model, view, presenter triad, with 
data transfer handled either directly by the presenter or via custom EventArgs. Communication between presenters
is via TinyMessenger through custom generic messages. The model directory and nCounter core classes are currently
separate as I'm thinking the nCounter core should become it's own DLL once completely stabilized.

The ostensible purpose of the tool is evaluation and automated analysis of Nanostring nCounter data. The tool 
can load RCCs, RLFs, and PKC files either directly or from zips (the output format of the Digital Analyzer or Sprint)
and can handle Gx (including cross RLF), miRNA, CNV, PlexSet (sample multiplexed), or nCounter readout from GeoMx 
protein experiments. RLF definition is flexible enough to only require loading of RLF files if necessary (e.g. for 
cross-codeset).

The main form is where data loading and initiating any analyses will start and displays any RCCs loaded, including
columns for default fields which can be modified by the user (option from context menu displayed when right clicking 
column headers). Left click of column headers will sort RCCs by the data in that column and holding down control while
clicking will allow secondary and teriary sorting. The table can be saved as a CSV via right click context menu option.

Raw counts and RCC header info can be collected in raw count tables from an option in the Review menu, which will be
the menu to go to for all data QC evaluation functionality. A "plate view" format will be available shortly for 
multiplex experiments to show QC info in context of how samples were hybridized, for troubleshooting purposes. Dynamic
range, linearity/correlation scatterplots, correlation heatmap, and PCA analyses will be available shortly to aid in evaluating 
flagged data for impacts on dynamic range and linearity.

A main focus is on streamlined workflow and inherent flexibility though further flexibility is continuously being added
through the preferences menu. 

Long run the tool will include GeNorm for annotated HK evaluation and an implementation of RefSeq, particularly for miRNA
data which, particularly for biofluid data, often presents normalization challenges. In addition a robust though streamlined 
differential expression analysis (neg binomial multiple regression model) will be included, and I'm looking at different 
dimension reduction methods to include. Ontology is generally already curated for Nanostring panels, given the biology-
specific bias in the content, so no ontology enrichment will be included.

The main purpose here is to show what I can do though this is being designed to be as useful as possible, particularly if
data analysis support for nCounter ends up getting dropped with Nanostring's recent acquisition. 
