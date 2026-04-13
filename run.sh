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

# Extract stock global.ini
"$scriptpath/publish/sc-localizer-linux" -b ~/Games/star-citizen/StarCitizen/LIVE/Data.p4k -o "$scriptpath/data/global.ini"

# Fun edits
grep -iE '=.*aluminum' "$scriptpath/data/global.ini" | sed 's/\(^[^=]*=.*\)\([Aa]\)luminum/\1\2luminium/g' > "$scriptpath/data/misc-annotated.ini"
grep -iE '=.*Ursa Medivac' "$scriptpath/data/global.ini" | sed 's/\(^[^=]*=.*\)Ursa Medivac/\1Nursa/g' >> "$scriptpath/data/misc-annotated.ini"

# Fetch external data
mkdir -p "$scriptpath/extdata"
curl https://raw.githubusercontent.com/MrKraken/StarStrings/refs/heads/master/contracts.ini -o "$scriptpath/extdata/contracts-annotated.ini"
curl https://raw.githubusercontent.com/MrKraken/StarStrings/refs/heads/master/mining.ini -o "$scriptpath/extdata/mining-annotated.ini"

# Process local data
merge=""
# External first so local trumps it
for m in $(ls $scriptpath/extdata/*-annotated.ini); do
    merge="$merge -m \"$m\""
done
for m in $(ls $scriptpath/data/*-annotated.ini); do
    merge="$merge -m \"$m\""
done

echo $merge | xargs "$scriptpath/publish/sc-localizer-linux" -b ~/Games/star-citizen/StarCitizen/LIVE/Data.p4k -o "$scriptpath/output/global.ini"
