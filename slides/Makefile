slides:
	@cd src && pandoc -t revealjs --no-highlight -V hlss=zenburn -V theme=night --slide-level 2 -s *_*.md -o ../out/slides.html
	@cp -r src/img out

pdf:
	@cd src && pandoc --latex-engine=xelatex -t beamer *_*.md -o ../out/slides.pdf

