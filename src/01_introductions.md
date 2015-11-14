% Developing F# on NixOS
% Karsten Gebbert
% 27/10/2015

# Hi.

#### Introductions

***** 

### Who? Why?

> - this talk is meant as an introduction to developing F# on NixOS
> - I am not a Nix(OS) or F\# expert or .NET veteran (yet)
> - sharing my personal experience and impressions while getting some work done
> - approaches taken have a few rough edges, so please help me improve it!
> - it is my first experience of this kind, so please do criticize me (gently)

<div class="notes">
Introductions:

- introduce yourself
- work @ nsynk GmbH
- my background in arts, music, computing
- interests
    * functional programming
    * systems programming
    * distributed systems
    * web
    * audio/video
- a few words about iris
    * distributed system of VVVV renderers
    * used for playback of high-quality (4K) video streams, potentially with 3d
      rendered overlays and integration with all kinds of other systems
      (sensors, kinetics etc)
    * IAA 
- my usage of NixOS
    * mainly as a development platform
    * use VMs for Windows-based work
    * my experiences reflect a new user story
</div>

*****

### What?

> - explore some basic aspects of F#
> - look some F# libraries and code
> - show how construct a small service
> - packaging and deployment using nix

<div class="notes">
* 
</div>
