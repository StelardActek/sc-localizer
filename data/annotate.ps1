[IO.File]::ReadAllLines("$PWD/components.ini") |% {
    $key,$value = $_.Split("=")

    foreach ($file in (gci "$PWD/*.json")) {
        $annotation = $(cat $file.FullName | jq -j ".[] | select(.data.name == `"$value`") | .data.class, `" `", .data.grade")

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
