#!/bin/bash

scriptpath=`dirname "$(realpath $0)"`
if [ ! -d "$scriptpath" ]; then
    echo "Script path $scriptpath does not exist!?"
    exit 1
fi

if [ -d "$scriptpath/output" ]; then
    echo "Cleaning up previous outputs..."
    rm -rf "$scriptpath/output"
fi
mkdir "$scriptpath/output"

"$scriptpath/publish/sc-localizer-linux" -b ~/Games/star-citizen/StarCitizen/LIVE/Data.p4k -m "$scriptpath/data/components-annotated.ini" -o "$scriptpath/output/global.ini"
