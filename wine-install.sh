
#!/bin/bash
dpkg --add-architecture i386 
wget -nc https://dl.winehq.org/wine-builds/Release.key
apt-key add Release.key
apt-add-repository https://dl.winehq.org/wine-builds/ubuntu/
apt-get update
apt-get install --install-recommends winehq-stable
curl 'https://raw.githubusercontent.com/Winetricks/winetricks/master/src/winetricks' --output ./winetricks
chmod +x ./winetricks
WINEPREFIX=$HOME/winedotnet452
wineboot --init
./winetricks --unattended dotnet452 corefonts
