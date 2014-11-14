# make -f Makefile_pdflatex

R = template
STY = *.cls
FIG = images/samplefigure.pdf

pdf: $(R).pdf

$(R).bbl: $(R).bib
	pdflatex $(R)
	bibtex $(R)
	pdflatex $(R)

$(R).pdf: *.tex $(STY) $(FIG) $(R).bbl
	pdflatex $(R)

clean:
	rm -f $(R).log $(R).aux $(R).bbl $(R).blg $(R).out
