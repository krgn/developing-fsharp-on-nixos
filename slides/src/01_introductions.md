% Developing F# on NixOS
% Karsten Gebbert
% 27/10/2015

# Hi.

#### Introductions

***** 

### Who? Why?

> - I am not a Nix(OS) or F\# expert or .NET veteran
> - this talk is aimed at people with experience level roughly to my own
> - sharing my personal experience and impressions
> - it is my first talk, so please criticize me (gently :))

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
</div>

*****

### What?

> - explore some basic aspects of F#
> - look at a library for creating http services, Suave.IO
> - give an overview over recoll, a file indexer
> - construct a small service to query recoll via HTTP
> - deploy that service using nix
