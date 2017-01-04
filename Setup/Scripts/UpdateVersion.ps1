$source = (Get-Content -Path "UpdateVersion.cs");
$root   = (Get-Item -Path "..\..\" -Verbose).FullName;
(Add-Type -TypeDefinition "$source");
[Accord.Setup.Scripts.UpdateVersion]::Replace($root);

