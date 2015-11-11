# Deployment

## Writing a derivation

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
