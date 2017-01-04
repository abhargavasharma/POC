# This is our fancy custom ReplaceInFile method that does transformation of simple text
Function ReplaceInFile($TargetFile, [HashTable] $Values){
    
    if ( (Test-Path $TargetFile ) -eq $false){
        throw "The target file '$($TargetFile)' does not exist."
    }

    Write-Host " --- Starting custom transformation for $($TargetFile)"

    $fileContent = Get-Content $TargetFile
    $Values.GetEnumerator() | ForEach-Object { 
        Write-Host "Replacing [$($_.Key)] with [$($_.Value)]"
        $fileContent = $fileContent -replace $_.Key, $_.Value
    }

    [IO.File]::WriteAllText($TargetFile, ($fileContent -join "`r`n"))
}

$CurrentPsPath = ( split-path $MyInvocation.MyCommand.Path )

# Find and Replace for Web.Config
$WebConfigFile = ( Join-Path $CurrentPsPath -ChildPath "Web.Config"	)
ReplaceInFile -TargetFile $WebConfigFile -Values @{
	'add name="ServedBy" value="1"' = "add name=""ServedBy"" value=""$($ServedByHeader)"""
}	
ReplaceInFile -TargetFile $WebConfigFile -Values @{
	'add name="Access-Control-Allow-Origin" value="tal-qa1.delivery.lan"' = "add name=""Access-Control-Allow-Origin"" value=""$($CrosDomain)"""
}