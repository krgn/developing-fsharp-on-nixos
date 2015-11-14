#### Deployment

<div class="notes">
* very minimalistic solution
* happy to learn about the full-blown approaches nixops/disnix
</div>

*****

#### Nixyfying the project

*****

```{.fragment}
cd /path/to/project/
```

```{.fragment}
Paket2Nix
```

<div class="notes">
* download all dependencies
* checksums each dependency
* generates Nix derivations
* mention `master.tar.gz` limitation
</div>

*****

#### Thats it!


<div class="notes">
* show directory structure and top-level derivation
* mention currently directory is hard-coded
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
