param([switch]$Fit, [switch]$Smoke, [switch]$Motion, [switch]$Animation, [switch]$Record, [switch]$RevisionRecord, [switch]$PolishRecord, [switch]$Lifecycle, [switch]$Labels, [switch]$Performance, [string]$AppDirectory = 'bin/Debug')
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot)
$env:DSA_TEST_OUTPUT = Join-Path $PWD 'obj'
& "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe" /nologo "/out:$AppDirectory/EmbeddedChecks.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll "/r:$AppDirectory/CpE2_DSA_Olivar_26271Sem.exe" tests\EmbeddedChecks.cs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
if ($Fit) { & "$AppDirectory/EmbeddedChecks.exe" fit } elseif ($Animation) { & "$AppDirectory/EmbeddedChecks.exe" animation } elseif ($PolishRecord) { & "$AppDirectory/EmbeddedChecks.exe" polish-record } elseif ($Performance) { & "$AppDirectory/EmbeddedChecks.exe" performance } elseif ($RevisionRecord) { & "$AppDirectory/EmbeddedChecks.exe" revision-record } elseif ($Labels) { & "$AppDirectory/EmbeddedChecks.exe" labels } elseif ($Lifecycle) { & "$AppDirectory/EmbeddedChecks.exe" lifecycle } elseif ($Record) { & "$AppDirectory/EmbeddedChecks.exe" record } elseif ($Motion) { & "$AppDirectory/EmbeddedChecks.exe" motion } elseif ($Smoke) { & "$AppDirectory/EmbeddedChecks.exe" smoke } else { & "$AppDirectory/EmbeddedChecks.exe" }
exit $LASTEXITCODE

