#!/bin/bash

scriptpath=`dirname "$(realpath $0)"`
if [ ! -d "$scriptpath" ]; then
    echo "Script path $scriptpath does not exist!?"
    exit 1
fi

if [ -d "$scriptpath/sc-localizer/bin/Release" ]; then
    echo "Cleaning up previous release builds..."
    rm -rf "$scriptpath/sc-localizer/bin/Release"
fi

if [ -d "$scriptpath/publish" ]; then
    echo "Cleaning up previous releases..."
    rm -rf "$scriptpath/publish"
fi

targets="linux win"

for target in $targets; do
    pubdir="$scriptpath/publish"

    dotnet publish sc-localizer/sc-localizer.csproj -c Release --sc --os "$target" -p:PublishSingleFile=true -p:DebugSymbols=false -p:TargetOperatingSystem="$target" -o "$pubdir"
done
