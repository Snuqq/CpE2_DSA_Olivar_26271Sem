$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot)
$sources = @(Get-ChildItem -File *.cs | Where-Object Name -ne 'Program.cs' | ForEach-Object FullName)
$sources += Join-Path $PWD 'tests\FormChecks.cs'
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /out:obj\FormChecks.exe /main:FormChecks /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll $sources
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& .\obj\FormChecks.exe
exit $LASTEXITCODE
