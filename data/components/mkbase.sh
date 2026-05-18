#!/bin/bash

scriptpath=`dirname "$(realpath $0)"`
if [ ! -d "$scriptpath" ]; then
    echo "Script path $scriptpath does not exist!?"
    exit 1
fi

grep -E 'item_Name_?(COOL|POWR|SHLD|QDRV|RADR)_' "$scriptpath/../global.ini" > "$scriptpath/components.ini"
