
#!/bin/bash

curl 'https://raw.githubusercontent.com/Winetricks/winetricks/master/src/winetricks' --output ./winetricks
chmod +x ./winetricks
WINEPREFIX=$HOME/winedotnet452
wineboot --init
./winetricks --unattended dotnet452 corefonts
