with import <nixpkgs> {};
with lib;

# THE IMPURE WAY

stdenv.mkDerivation {
  name = "paperscraper-0.0.1";

  src = fetchgit {
    url = "https://github.com/krgn/developing-fsharp-on-nixos.git";
    rev = "fb7a361ae6235307c58ac033a57654004d680a50";
    sha256 = "dacb806aff014bd892d68d4f51205c44dd89c9751f542c1b3ba83d6cd67a4dad";
  };

  buildInputs = [ fsharp mono curl strace ];

  phases = [ "unpackPhase" "buildPhase" "installPhase" ];

  buildPhase = ''
    patchShebangs .
    curl http://google.com

    sh sample/build.sh
  '';

  installPhase = ''
    mkdir -p "$out/bin" "$out/lib"

    cp -r "bin/PaperScraper" "$out/lib"

    echo "#!/usb/bin/env bash" > "$out/bin/paperscraper"
    echo "mono $out/lib/PaperScraper/PaperScraper.exe" >> "$out/bin/paperscraper"
  '';

  meta = {
    description = "";
    homepage = "http://paperscraper.ioctl.it";
    license = "GPL";
  };
}
