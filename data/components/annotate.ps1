#!/usr/bin/pwsh

$fixes = @{
    "item_NameCOOL_AEGS_S04_Idirs"="item_NameCOOL_AEGS_S04_Idris";
}

$classAbbrev = @{
    "Military"="Mil";
    "Civilian"="Civ";
    "Competition"="Comp";
    "Industrial"="Ind";
    "Stealth"="Slth";
}

$data = @{}
[IO.File]::ReadAllLines("$PSScriptRoot/components-desc.ini") |% {
    $key,$value = $_.Split("=")
    $value = $value.Replace("\n", "`n")

    if ($key.EndsWith("_Default") -or $key.EndsWith("_Default,P") -or $key.EndsWith("_Controller") -or $key.EndsWith("_Controller,P")) {
        return
    }

    $rSize = [regex]"\b\s*Size:\s*(\d*)\b"
    $rSizeBackup = [regex]"_S(\d*)_"
    $rGrade = [regex]"\b\s*Grade:\s*(N/A|[A-Z]*)\b"
    $rClass = [regex]"\b\s*Class:\s*(\w*)\b"

    $mSize = $rSize.Match($value)
    $mSizeBackup = $rSizeBackup.Match($key)
    $mGrade = $rGrade.Match($value)
    $mClass = $rClass.Match($value)

    $size = $mSize.Groups[1].Value
    if ([string]::IsNullOrWhiteSpace($size)) {
        $iSize = 0
        if ([int]::TryParse($mSizeBackup.Groups[1].Value, [ref]$iSize)) {
            $size = $iSize.ToString()
        } else {
            $size = $null
        }
    }
    $grade = $mGrade.Groups[1].Value
    if ($grade -eq "") { $grade = $null }
    $class = $mClass.Groups[1].Value
    if ($class -eq "") { $class = $null }

    if ($class -and $classAbbrev.ContainsKey($class)) {
        $class = $classAbbrev[$class]
    }

    $rDictKey = [regex]"^[Ii]tem_[Dd]esc_?(.*?)(?:_SCItem)?(?:,P)?$"
    $mDictKey = $rDictKey.Match($key)
    if (!$mDictKey.Success) {
        Write-Error "Could not match dict key: $key"
        exit -1
    }
    $dictKey = $mDictKey.Groups[1].Value

    #Write-Host "$key => $dictKey  Size: $size  Grade: $grade  Class: $class"
    $data[$dictKey] = @{"size"=$size; "class"=$class; "grade"=$grade}
}

[IO.File]::ReadAllLines("$PSScriptRoot/components.ini") |% {
    $key,$value = $_.Split("=")
    $seek = $key

    if ($fixes.ContainsKey($seek)) {
        $seek = $fixes[$seek]
    }

    if ($key.EndsWith("_Default") -or $key.EndsWith("_Controller")) {
        return
    }

    $rDictKey = [regex]"^[Ii]tem_[Nn]ame_?(.*?)(?:_SCItem)?(?:,P)?$"
    $mDictKey = $rDictKey.Match($seek)
    if (!$mDictKey.Success) {
        Write-Error "Could not match dict key: $seek"
        exit -1
    }
    $dictKey = $mDictKey.Groups[1].Value

    $component = $null
    $annotation = $null
    if ($data.ContainsKey($dictKey)) {
        $component = $data[$dictKey]
    } elseif ($data.ContainsKey("$dictKey,P")) {
        $component = $data["$dictKey,P"]
    } elseif ($data.ContainsKey($dictKey.Replace("_SCItem", ""))) {
        $component = $data[$dictKey.Replace("_SCItem", "")]
    }
    if ($component) {
        $parts = New-Object System.Collections.Generic.List[string]
        if ($component.size) {
            $parts.Add("S$($component.size)")
        }
        if ($component.class) {
            $parts.Add($component.class)
        }
        if ($component.grade) {
            $parts.Add($component.grade)
        }

        $annotation = [string]::Join(" ", $parts)
    }

    if (!$annotation) {
        "$key=$value [Unk]"
        return
    }

    "$key=$value [$annotation]"
}
