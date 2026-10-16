$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot)
$resources = @('idle', 'search', 'found' | ForEach-Object { "/resource:Assets\Furina\$_.png,CpE2_DSA_Olivar_26271Sem.Assets.Furina.$_.png" })
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /out:obj\AnimationChecks.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll $resources SearchAnimationPanel.cs tests\AnimationChecks.cs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& .\obj\AnimationChecks.exe
exit $LASTEXITCODE
