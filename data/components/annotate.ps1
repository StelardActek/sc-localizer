#!/usr/bin/pwsh


$fixes = @{"EcoFlow" = "Eco-Flow"}

[IO.File]::ReadAllLines("$PSScriptRoot/components.ini") |% {
    $key,$value = $_.Split("=")
    $seek = $value.ToLower()

    if ($fixes.ContainsKey($seek)) {
        $seek = $fixes[$seek].ToLower()
    }

    if ($key.EndsWith("_Default") -or $key.EndsWith("_Controller")) {
        return
    }

    foreach ($file in (gci "$PSScriptRoot/*.json")) {
        $annotation = $(cat $file.FullName | jq -j ".[] | select(.data.name | ascii_downcase == `"$seek`") | `"S`", .data.size, `" `", .data.class, `" `", .data.grade")

        if ($annotation) {
            break
        }
    }

    if (!$annotation) {
        "$key=$value [Unk]"
        return
    }

    $annotation = $annotation.Replace("Military", "Mil").Replace("Civilian", "Civ").Replace("Competition", "Comp").Replace("Industrial", "Ind").Replace("Stealth", "Slth")

    "$key=$value [$annotation]"
}
