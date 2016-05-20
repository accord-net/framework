param($installPath, $toolsPath, $package, $project)

"Installing [{0}] to project [{1}]" -f $package.Id, $project.FullName | Write-Host

# Load MSBuild assembly if it’s not loaded yet.
Add-Type -AssemblyName "Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"

# Check that SharpDX.targets was correctly imported
$buildProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1
$importsToRemove = $buildProject.Xml.Imports | Where-Object { $_.Project.Endswith('SharpDX.targets') }
if (!$importsToRemove)
{
	throw ("SharpDX.targets import not found in project [{0}]" -f $project.FullName)
}
$sharpdx_package_bin_dir = $buildProject.GetProperty("SharpDXPackageBinDir").EvaluatedValue
$sharpdx_assembly_path = "{0}\{1}.dll" -f $sharpdx_package_bin_dir, $package.Id

# Add the assembly through the project in order for VS to update correctly the references in the IDE
$project.Object.References.Add($sharpdx_assembly_path)

# Find the references we just added
$sharpdx_reference = $buildProject.GetItems("Reference") | Where-Object { $_.EvaluatedInclude -eq $package.Id }
if (!$sharpdx_reference)
{
	$sharpdx_reference = $buildProject.GetItems("Reference") | Where-Object { $_.EvaluatedInclude.StartsWith("{0}," -f $package.Id) }
}
if (!$sharpdx_reference)
{
	throw ("Unable to find reference in project for assembly [{0}]" -f $package.Id)
}

# Replace the HintPath using the $(SharpDXPackageBinDir) variable provided by the SharpDX.targets
$sharpdx_reference.SetMetadataValue("HintPath", '$(SharpDXPackageBinDir)\{0}.dll' -f $package.Id)

# Save the project
$project.Save()
