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



*****

## INTRODUCINT PAKET2NIX

## NIX-YFYING THE PROJECT
  
```
cd /path/to/project/
Paket2Nix
```

Thats it. The `nix` directory now contains a top-level expression `top.nix`
which you can use to build the package and all other expressions.

## BUILDING 

```
➜  nix git:(master) ✗ nix-build top.nix -A PaperScraper
```

will at this point produce: 

```
➜  nix git:(master) ✗ nix-build top.nix -A PaperScraper

/nix/store/328ccq2dw1dq8i0dlmlzf0iknb1pad28-paperscraper-0.0.1
```

which we can install into our environment


```
➜  nix git:(master) ✗ nix-env -i -f ./top.nix -A PaperScraper
replacing old ‘paperscraper-0.0.1’
installing ‘paperscraper-0.0.1’
building path(s) ‘/nix/store/0d4vh1vaw41v3xn9vq66yydck142z02d-user-environment’
created 19003 symlinks in user environment
```

## Deploying system-wide
  
## mention hydra
## package up our expressions

   ```
   echo "import ./top.nix" >> nix/default.nix
   tar cjvf nixexprs.tar.bz2 nix/
   mkdir -p ~/tmp/mycache
   mv nixexprs.tar.bz2 ~/tmp/mycache
   ```

## create a simple cache for our expressions 

   ```
   cd ~/tmp/mycache
   nix-push --dest . --manifest /nix/store/328ccq2dw1dq8i0dlmlzf0iknb1pad28-paperscraper-0.0.1/
   ```
   creates a binary cache of all expressions
   
## using the channel
   
   ```
   nix-channel --add file:///home/k/tmp/mycache krgn
   nix-channel --update

   nix-env -qaP | grep -i PaperScraper
   krgn.PaperScraper                                             paperscraper-0.0.1
   ```
## creating a custom module

```
{config, pkgs, lib, ...}:

let
  cfg = config.services.paperscraper;
  krgn = import <krgn>;
in

with lib;

{
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

and import this in `configuration.nix`


```
imports = [
   ./services/paperscraper.nix
  ]
  
...
services.paperscraper = {
  enable = true;
  user = "k";
};
```
