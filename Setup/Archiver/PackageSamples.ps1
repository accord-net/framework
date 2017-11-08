$source = (Get-Content -Path "PackageSamples.cs");
(Add-Type -TypeDefinition "$source" -ReferencedAssemblies System.IO.Compression);
[Accord.Setup.Archiver.PackageSamples]::Main($null);
