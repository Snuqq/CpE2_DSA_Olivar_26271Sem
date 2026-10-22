param([string]$Configuration = 'Debug', [string]$Unity = 'C:/Program Files/Unity/Hub/Editor/6000.3.26f1/Editor/Unity.exe', [switch]$InHubPackage, [string]$ResultFile, [switch]$Preview)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot
Set-Location $repo
New-Item -ItemType Directory -Path (Join-Path $repo 'obj') -Force | Out-Null
function BuildWinForms {
    $msbuild = 'C:/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/MSBuild.exe'
    & $msbuild CpE2_DSA_Olivar_26271Sem.csproj /t:Build "/p:Configuration=$Configuration" /p:UseSharedCompilation=false /nr:false /v:minimal /nologo
    if ($LASTEXITCODE -ne 0) { throw 'WinForms build failed.' }
}
trap {
    if ($ResultFile) { @{ success = $false; error = $_.Exception.Message } | ConvertTo-Json | Set-Content -LiteralPath $ResultFile }
    throw $_
}
if (!$InHubPackage) {
    & (Join-Path $repo 'tests/RunSearchChecks.ps1')
    if ($LASTEXITCODE -ne 0) { throw 'Search checks or Unity preview plan export failed.' }
    $hub = Get-AppxPackage -Name UnityTechnologies.UnityHub -ErrorAction SilentlyContinue
    if ($hub) {
        $result = Join-Path $repo ('obj/unity-build-' + [guid]::NewGuid().ToString('N') + '.json')
        $arguments = '-NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -File "{0}" -InHubPackage -Configuration "{1}" -Unity "{2}" -ResultFile "{3}"' -f $PSCommandPath, $Configuration, $Unity, $result
        if ($Preview) { $arguments += ' -Preview' }
        Invoke-CommandInDesktopPackage -PackageFamilyName $hub.PackageFamilyName -AppId UnityHub -Command 'powershell.exe' -Args $arguments -PreventBreakaway
        $deadline = [DateTime]::UtcNow.AddMinutes(15)
        while (!(Test-Path -LiteralPath $result)) {
            if ([DateTime]::UtcNow -gt $deadline) { throw 'Build status timed out. Inspect the Unity process and obj/unity-build.log before retrying.' }
            Start-Sleep -Milliseconds 500
        }
        $state = Get-Content -LiteralPath $result -Raw | ConvertFrom-Json
        if (!$state.success) { throw $state.error }
        if ($Preview) { Write-Output 'Rendered obj/unity-stage-preview.png without changing the authored scene.' }
        else { BuildWinForms; Write-Output "Built bin/$Configuration using the existing Unity Hub package license." }
        exit 0
    }
}
$scene = Join-Path $repo 'UnitySearch/Assets/Search.unity'
$before = (Get-FileHash -LiteralPath $scene).Hash
$method = if ($Preview) { 'SetupSearchScene.Preview' } else { 'SetupSearchScene.BuildWindows' }
$process = Start-Process -FilePath $Unity -ArgumentList '-batchmode', '-quit', '-projectPath', "`"$repo/UnitySearch`"", '-executeMethod', $method, '-logFile', "`"$repo/obj/unity-build.log`"" -WindowStyle Hidden -PassThru
$process.WaitForExit()
if ($process.ExitCode -ne 0) { throw "Unity build failed; inspect obj/unity-build.log." }
if ((Get-FileHash -LiteralPath $scene).Hash -ne $before) { throw 'Build changed the authored scene.' }
if ($ResultFile) { @{ success = $true } | ConvertTo-Json | Set-Content -LiteralPath $ResultFile; exit 0 }
if ($Preview) { Write-Output 'Rendered obj/unity-stage-preview.png without changing the authored scene.' }
else { BuildWinForms; Write-Output "Built bin/$Configuration including the UnityPlayer directory." }
