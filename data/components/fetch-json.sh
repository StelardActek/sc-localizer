#!/bin/bash

scriptpath=`dirname "$(realpath $0)"`
if [ ! -d "$scriptpath" ]; then
    echo "Script path $scriptpath does not exist!?"
    exit 1
fi

curl 'https://server.erkul.games/live/coolers' -H 'Accept: application/json, text/plain, */*' -H 'Referer: https://www.erkul.games/' -H 'Origin: https://www.erkul.games' -o "$scriptpath/coolers.json"
#curl 'https://server.erkul.games/live/missiles' -H 'Accept: application/json, text/plain, */*' -H 'Referer: https://www.erkul.games/' -H 'Origin: https://www.erkul.games' -o "$scriptpath/missiles.json"
curl 'https://server.erkul.games/live/power-plants' -H 'Accept: application/json, text/plain, */*' -H 'Referer: https://www.erkul.games/' -H 'Origin: https://www.erkul.games' -o "$scriptpath/power-plants.json"
curl 'https://server.erkul.games/live/qdrives' -H 'Accept: application/json, text/plain, */*' -H 'Referer: https://www.erkul.games/' -H 'Origin: https://www.erkul.games' -o "$scriptpath/qdrives.json"
curl 'https://server.erkul.games/live/radars' -H 'Accept: application/json, text/plain, */*' -H 'Referer: https://www.erkul.games/' -H 'Origin: https://www.erkul.games' -o "$scriptpath/radars.json"
curl 'https://server.erkul.games/live/shields' -H 'Accept: application/json, text/plain, */*' -H 'Referer: https://www.erkul.games/' -H 'Origin: https://www.erkul.games' -o "$scriptpath/shields.json"
#curl 'https://server.erkul.games/live/weapons' -H 'Accept: application/json, text/plain, */*' -H 'Referer: https://www.erkul.games/' -H 'Origin: https://www.erkul.games' -o "$scriptpath/weapons.json"
