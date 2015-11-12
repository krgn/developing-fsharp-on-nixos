# Deployment

#### Deployment

*****

#### Creating A Package*

(* or Derivation in Nix-lingo)

*****

#### The *Impure* way 

```
with import <nixpkgs> {};
with lib;


stdenv.mkDerivation {
  name = "paperscraper-$version";

  version = "0.0.1";

  src = fetchurl {
    url = "https://github.com/krgn/PaperScraper/tarball/6a8e0d29c90844fa18665278f5dd1fb96537d70";
    sha256 = "0l2jchn4p9bj157h94l6gi8ca3hafyacx5809nv9ssvhlk00ps87";
  };

  buildInputs = [ fsharp mono curl strace ];

  phases = [ "unpackPhase" "buildPhase" "installPhase" ];

  buildPhase = ''
    patchShebangs .
    build.sh
  '';

  installPhase = ''
    mkdir -p "$out/bin" "$out/lib/mono/packages/$name"

    cp -r "bin/PaperScraper" "$out/lib/mono/packages/$name/"

    cat >> "$out/bin/PaperScraper" <<-WRAPPER
    #!/bin/sh
    ${mono}/bin/mono $out/lib/PaperScraper/PaperScraper.exe
    WRAPPER

    chmod +x "$out/bin/PaperScraper"
  '';

  meta = {
    description = "its a service!";
    homepage = "https://github.com/krgn/PaperScraper";
  };
}
```

*****

Install it!

```
$ nix-env -i -f paperscraper.nix
```

*****

#### OH NO!

*****

#### Of course! In Nix, clocks tick differently.

> - purity!
> - i.e. not network, k?
> - no paket! no nuget!

<div class="notes">
* there is resolv.conf in the chroot/sandbox, so no network
* no paket and no nuget
* tl;dr it doestn't work
</div>

*****

#### The (obvious) solution

> - lift nuget packages into the store
> - link packages into the packages directory at build time
> - disable nuget/paket
> - use xbuild instead of FAKE

*****

#### Current Situation

> - some infrastructure exists (e.g. dotnetPackages)
> - packages managed manually
> - packaging from source is quite difficult
> - no GAC, so dependencies are not found during development

<div class="notes">
* out of date
* not many packages available
* packaging is difficult, due to the same contraints as discovered
</div>

*****

#### Paket2Nix

> - uses Paket metadata, the .lock file in particular
> - discovers projects and their metadata
> - generates Nix expressions for all dependencies project code (one per project)
> - creates wrapper scripts for executables

#### Motivation

> - currently for Paket is the Way To Go™ for development
> - solves common problems in .NET development (consistent dependency trees)
> - integreate with Paket for maximum flexibility

<div class="notes">
* References in XML project files
* inter-op with non-nix platforms
</div>

*****

## Let's use it :)

*****

```{.fragment}
cd /path/to/project/
```

```{.fragment}
Paket2Nix
```

***** 

> - download all dependencies
> - checksums each dependency
> - generates Nix derivations

<div class="notes">
* mention `master.tar.gz` limitation
</div>

*****

#### Thats it!

The `nix` directory now contains a top-level expression `default.nix`
which you can use to build the package and all other expressions.

<div class="notes">
* currently directory is hard-coded
</div>

*****

#### Building

```{.fragment}
nix-build default.nix -A PaperScraper
```

```{.fragment}
/nix/store/328ccq2dw1dq8i0dlmlzf0iknb1pad28-paperscraper-0.0.1
```

***** 

#### Installing

We can simply install the service into our user environment.

```
nix-env -i -f ./default.nix -A PaperScraper
```

*****

#### Modules

> - yay, declarative configuration!
> - needs to "know" about our package
> - system-wide (afaik)

<div class="notes">
* ask: are there other ways to use modules?
* ask: user-modules?
</div>

*****

#### Creating a custom module

```
{config, pkgs, lib, ...}:

let
  cfg = config.services.paperscraper;
  krgn = import <krgn>;
in

with lib;

{
```

*****

```
  options = {
    services.paperscraper = {
      enable = mkOption {
        default = false;
        type = with types; bool;
        description = ''
          Start the PaperScraper API service.
        '';
      };

      user = mkOption {
        default = "username";
        type = with types; uniq string;
        description = ''
          Name of the user to run as.
        '';
      };
    };
  };
```

*****

```
  config = mkIf cfg.enable {
    jobs.paperscraper = {
      description = "Start the paperscraper service.";
      startOn = "started network-interfaces";
      exec = ''/var/setuid-wrappers/sudo -u ${cfg.user} -- ${krgn.PaperScraper}/bin/PaperScraper'';
    };

    environment.systemPackages = [ krgn.PaperScraper pkgs.recoll ];
  };
}
```

*****

Save somewhere and import in `configuration.nix`:


```
imports = [
   ./services/paperscraper.nix
  ]
  

services.paperscraper = {
  enable = true;
  user = "k";
};
```

*****

#### BUT!

*****

#### PaperScraper not found! :(

(of course)

*****

#### (One) Solution: Custom Binary Cache!

<div class="notes">
* binary cache easily movable/hostable
* mention hydra
* more?
</div>

*****

#### Simple cache

```{.fragment}
mkdir -p ~/tmp/mycache
cd ~/tmp/mycache
```

```{.fragment}
nix-push --dest . --manifest /nix/store/328ccq2dw1dq8i0dlmlzf0iknb1pad28-paperscraper-0.0.1/
```

<div class="notes">
* creates a binary cache of all dependent expressions
* creates a manifest
* creates nar files (compressed archives)
</div>

*****

#### Providing our Packages

```{.fragment}
tar cjvf nixexprs.tar.bz2 nix/
```

```{.fragment}
mv nixexprs.tar.bz2 ~/tmp/mycache
```
  
#### Using the channel
   
```{.fragment}
nix-channel --add file:///home/k/tmp/mycache krgn
nix-channel --update
```

```{.fragment}
nix-env -qaP | grep -i PaperScraper
krgn.PaperScraper                                             paperscraper-0.0.1
```

*****

#### :D

*****

#### Deploying system-wide

```{.fragment}
nixos-rebuild switch --upgrade
```

```{.fragment}
systemctl status paperscraper
```

```{.fragment}
curl http://localhost:8083/search?term=monoid
```
