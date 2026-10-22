$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot)
New-Item -ItemType Directory -Path obj -Force | Out-Null
$compiler = "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe"
$sources = @(Get-ChildItem -File *.cs | Where-Object Name -ne 'Program.cs' | ForEach-Object FullName)
$sources += @(Get-ChildItem -File UnitySearch/Assets/Shared/*.cs | ForEach-Object FullName)
$sources += Join-Path $PWD 'tests/SearchCases.cs'
& $compiler /nologo /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll /out:obj\SearchChecks.exe $sources tests\SearchChecks.cs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& .\obj\SearchChecks.exe
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& $compiler /nologo /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll /out:obj\PlaybackChecks.exe $sources tests\PlaybackChecks.cs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& .\obj\PlaybackChecks.exe
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& $compiler /nologo /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll /out:obj\ExportSearchSamples.exe $sources tests\ExportSearchSamples.cs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& .\obj\ExportSearchSamples.exe
exit $LASTEXITCODE
