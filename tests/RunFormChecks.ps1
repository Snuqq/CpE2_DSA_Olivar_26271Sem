$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot)
$sources = @(Get-ChildItem -File *.cs | Where-Object Name -ne 'Program.cs' | ForEach-Object FullName)
$sources += Join-Path $PWD 'tests\FormChecks.cs'
$resources = @('idle', 'search', 'found' | ForEach-Object { "/resource:Assets\Furina\$_.png,CpE2_DSA_Olivar_26271Sem.Assets.Furina.$_.png" })
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /out:obj\FormChecks.exe /main:FormChecks /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll $resources $sources
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& .\obj\FormChecks.exe
exit $LASTEXITCODE
