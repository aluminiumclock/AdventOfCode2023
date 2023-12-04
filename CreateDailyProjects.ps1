$Start_Location = $PSScriptRoot
Set-Location $Start_Location

for($i = 1; $i -le 25; $i++) {
    for($j = 1; $j -le 2; $j++) {
        $Name = ([string]::Format('Day{0:D2} Part{1}', $i, $j))
        if(-Not(Test-Path -Path ".\$Name" -PathType Container)) {        
            Copy-Item -Path '.\Day01 Part1' -Destination ".\$Name" -Recurse
            Rename-Item -Path ".\$Name\Day01 Part1.csproj" -NewName "$Name.csproj" 
        }
    }
}