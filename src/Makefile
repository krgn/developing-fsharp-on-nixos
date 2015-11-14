slides:
	@pandoc --template=./templates/reveal-template.html -t revealjs --no-highlight -V hlss=zenburn -V transition=none -V theme=white --slide-level 2 -s *_*.md -o ../out/slides.html
	@cp -r ./img ../out
	@cp -r ./jquery.min.js ../out
	@cp -r ./init.js ../out
	@cp -r ./highlight ../out/hl

pdf:
	@pandoc --latex-engine=xelatex -t beamer *_*.md -o ../out/slides.pdf

