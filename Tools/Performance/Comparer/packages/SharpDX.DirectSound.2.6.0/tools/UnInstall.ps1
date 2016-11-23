param($installPath, $toolsPath, $package, $project)

"Uninstalling [{0}] from project [{1}]" -f $package.Id, $project.FullName | Write-Host

# Retrieve the reference to the package
$sharpdx_reference = $project.Object.References.Item($package.Id)
if ($sharpdx_reference)
{
	# Remove the reference
	$sharpdx_reference.Remove()
	# Save the project
	$project.Save()
}
