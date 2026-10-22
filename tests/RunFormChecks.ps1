$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot)
$sources = @(Get-ChildItem -File *.cs | Where-Object Name -ne 'Program.cs' | ForEach-Object FullName)
$sources += Join-Path $PWD 'tests\FormChecks.cs'
$sources += Join-Path $PWD 'UnitySearch\Assets\Shared\SearchStep.cs'
$sources += Join-Path $PWD 'UnitySearch\Assets\Shared\SearchPlan.cs'
$sources += Join-Path $PWD 'UnitySearch\Assets\Shared\SearchWire.cs'
$env:DSA_DISABLE_UNITY = '1'
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /out:obj\FormChecks.exe /main:FormChecks /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll $sources
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& .\obj\FormChecks.exe
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /out:obj\SearchLayoutChecks.exe /main:SearchLayoutChecks /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll $sources tests\SearchLayoutChecks.cs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& .\obj\SearchLayoutChecks.exe
exit $LASTEXITCODE
