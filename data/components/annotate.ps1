#!/usr/bin/pwsh

$fixes = @{"EcoFlow" = "Eco-Flow"}
$remove = @("gmisl_s03_cs_fski_arrester")
$add = @{"Arrester III-G"=@{"size"=3; "class"=$null; "grade"="A"; "type"="Missile"}}

$data = @{}
ls $PSScriptRoot/*.json |% {
    $finfo = $_

    $j = Get-Content $finfo -Raw | ConvertFrom-Json
    $j |% {
        $c = $_

        if ($remove -contains $c.localName) {
            return
        }

        if ($data.ContainsKey($c.data.name)) {
            [Console]::Error.WriteLine("Found duplicate component name: $($c.data.name)")
            $data[$c.data.name].size ??= $c.data.size
            $data[$c.data.name].class ??= $c.data.class
            $data[$c.data.name].grade ??= $c.data.grade
            $data[$c.data.name].type ??= $c.data.type
        } else {
            $data.Add($c.data.name, @{"size"=$c.data.size; "class"=$c.data.class; "grade"=$c.data.grade; "type"=$c.data.type})
        }
    }
}
$add.Keys |% {
    $data.Add($_, $add[$_])
}

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
        $component = $data[$seek]
        $annotation = "S$($component.size ?? '?') $($component.class ?? 'Unk') $($component.grade ?? '?')"

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
