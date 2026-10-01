param([string]$AssemblyPath = "$PSScriptRoot/../bin/Debug/CpE2_DSA_Olivar_26271Sem.exe")
Add-Type -AssemblyName System.Windows.Forms
$assembly = [Reflection.Assembly]::LoadFrom((Resolve-Path $AssemblyPath))
function Field($form, $name) { ,$form.GetType().GetField($name, [Reflection.BindingFlags]'Instance,NonPublic').GetValue($form) }
function Click($form, $name) { $form.GetType().GetMethod($name + '_Click', [Reflection.BindingFlags]'Instance,NonPublic').Invoke($form, @($null, [EventArgs]::Empty)) | Out-Null }
function Assert($condition, $message) { if (!$condition) { throw $message } }
foreach ($kind in @('Stack', 'Queue')) {
    $type = $assembly.GetType('CpE2_DSA_Olivar_26271Sem.' + $kind)
    Assert ($null -ne $type) "Missing $kind"
    $form = [Activator]::CreateInstance($type)
    try {
        $inputBox = Field $form ('txt' + $kind)
        $items = Field $form ('my' + $kind)
        $add = if ($kind -eq 'Stack') { 'btnpush' } else { 'btnEnqueue' }
        $remove = if ($kind -eq 'Stack') { 'btnpop' } else { 'btnDequeue' }
        Click $form $remove
        Click $form 'btnPeek'
        Assert ($items.Count -eq 0) 'Empty operations must be safe'
        $inputBox.Text = '   '
        Click $form $add
        Assert ($items.Count -eq 0) 'Blank input must be rejected'
        foreach ($value in @('first', 'second', 'first')) { $inputBox.Text = $value; Click $form $add }
        Assert ($items.Count -eq 3) 'Insert and duplicate handling failed'
        Click $form $remove
        $expected = if ($kind -eq 'Stack') { 'second' } else { 'second' }
        Assert ($items.Peek() -eq $expected) 'Removal order failed'
        Click $form 'btnPeek'
        Assert ($items.Count -eq 2) 'Peek must not remove'
        $inputBox.Text = 'first'
        Click $form 'btnContains'
        Assert ((Field $form 'lblStatus').Text -match 'True') 'Contains failed'
        Click $form 'btnCount'
        Assert ((Field $form 'lblStatus').Text -match '2') 'Count failed'
        Assert ((Field $form 'lstDisplay').Items.Count -eq 2) 'Display failed'
        Assert ((Field $form 'lstDisplay').Items[0] -eq 'second') 'Display order failed'
        Click $form 'btnClear'
        Assert ($items.Count -eq 0) 'Clear failed'
        Assert ((Field $form 'lstDisplay').Items.Count -eq 0) 'Clear display failed'
    } finally { $form.Dispose() }
}

Write-Output 'PASS: Stack and Queue button operations, blank input, and empty collections.'
