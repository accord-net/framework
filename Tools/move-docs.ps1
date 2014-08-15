# This script can be used to replace the older documentation
# at the Google Code repository with links to the actual, more
# up-to-date documentation at GitHub.

function Main
{
	$docs  = "..\Docs\"
	$site  = "http://accord-framework.net/docs/"
	$index = "http://accord-framework.net/docs/Index.html"
    $template = @"
	<html>
	  <meta http-equiv="refresh" content="0;url=%PATH%">
	</html>
"@

	"Full path to the docs directory is"
	$docs_full = [System.IO.Path]::GetFullPath((Join-Path (pwd) $docs))
	$docs_full
	

	foreach ($item in Get-ChildItem -Path $docs *.htm* -Recurse)
	{
		$itemPath = $item.FullName
		$relative = $item | Resolve-Path -Relative
		
		$url = $relative.Replace($docs, $site).Replace("\", "/")
		
		$code = check-website($url)
		
		$current = $url
		
		if ($code -ne "200") { 
		    Write-Host -NoNewline "*"
		    $url = $index
		} 
		
		$current
		
		$contents = $template.Replace("%PATH%", $url)
		
		# $contents > $itemPath
	} 
}

function check-website($url)
{
<#
	$xHTTP = new-object -com msxml2.xmlhttp;
	$xHTTP.open("HEAD", $url, $false);
	$xHTTP.send();
	
	return $xHTTP.status 
#>

#<#
	  $request = [System.Net.WebRequest]::Create($url)
		
	  try { 
		  $response = $request.GetResponse() 
	  } catch [System.Net.WebException] { 
		  $response = $_.Exception.Response 
	  }
	  
	  $code = [int]$response.StatusCode;
	  
	  $response.Close();
	  return $code
#>
}


Main