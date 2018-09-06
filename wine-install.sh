
#!/bin/bash
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.1
dpkg --add-architecture i386 
wget -nc https://dl.winehq.org/wine-builds/Release.key
sudo apt-key add Release.key
sudo apt-add-repository https://dl.winehq.org/wine-builds/ubuntu/
sudo apt-get update
sudo apt-get install cabextract
sudo apt-get install --install-recommends winehq-stable
curl 'https://raw.githubusercontent.com/Winetricks/winetricks/master/src/winetricks' --output ./winetricks
chmod +x ./winetricks
WINEPREFIX=$HOME/winedotnet452
WINEARCH=win32
wineboot --init
./winetricks --unattended dotnet452 corefonts

