param([bool]$Clean=$false, [bool]$Build=$false, [bool]$Test=$false, [bool]$SQL=$false, [bool]$Lucene=$false)

Import-Module SQLPS
Import-Module WebAdministration -ErrorAction SilentlyContinue # Sometimes this complains when it's already added, so just be quiet!

function Main($Clean, $Build, $Test, $Lucene){
    
	if($Clean -eq $true){
		# Clean the unit tests folder manually because the Solution Cleaning before build doesn't clean custom output folders
		Clean-Path (Join-Path -Path $currentPath -ChildPath "..\lib") -RemoveParent $true
    }

    # Build and Deploy sitecore website
	if ($Build -eq -$true) {
		Build-Solution -PathToSolution (Join-Path -Path $currentPath -ChildPath "..\TAL.QuoteAndApply.sln") -Configuration "Debug"
    }

	# Upgrade SQL !
	if ($SQL -eq $true){
		Run-SQL-Migrations
	}
	
	if ($Lucene -eq $true){
		Clean-Lucene-Indexes
	}

	Set-Location $currentPath	
}

Function Clean-Directory($DirectoryToClean, [bool]$FilesOnly){

    if (Test-Path $DirectoryToClean ) {

        Write-Log "Cleaning out $($DirectoryToClean)"

        # First we try to unblock all the files in use by w3wp.exe process because we are deploying on top of a running application
        # Get-ChildItem -Path $DirectoryToClean -Recurse | Unblock-File -ErrorAction Continue | Out-Null

        # Try and just remove the child items at least!
        if ($FilesOnly -eq $true){
            Get-ChildItem -Path $DirectoryToClean -Recurse | where { $_.PSIsContainer -eq $false } | Remove-Item -Force -Recurse -ErrorAction Continue | Out-Null
        }
        else {
            Get-ChildItem -Path $DirectoryToClean -Recurse | Remove-Item -Force -Recurse -ErrorAction Continue | Out-Null
        }        
    }
}

function Build-Solution($PathToSolution, $Configuration) {
    
	# $dotNetVersion = "4.0"
    # $regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
    # $regProperty = "MSBuildToolsPath"
    # $msbuild = Join-Path -Path (Get-ItemProperty $regKey).$regProperty -ChildPath "msbuild.exe"

	#Assumes everyone has VS2015 installed, old registry way doesn't support the new language features
	$msbuild = Join-Path -Path "C:\Program Files (x86)\MSBuild\14.0\Bin" -ChildPath "msbuild.exe"
	

	Write-Log "Building solution $($PathToSolution)"

    & $msbuild "$($PathToSolution)" "/p:Configuration=$($Configuration)" "/t:Clean,Build"
}

function Clean-Path($Path, $RemoveParent = $false){
    if(Test-Path $Path){
        Write-Log "Cleaning $($Path)"
        Get-ChildItem -Path $Path -Recurse | where { $_.PSIsContainer -eq $false } | Remove-Item -Force -Recurse -ErrorAction Continue | Out-Null

        if($RemoveParent -eq $true){
            Remove-Item $Path -Recurse -Force
        }
    }
}

function Run-SQL-Migrations() {
    
	 # RESTORE SQL DATABASE
    Write-Log "RESTORE SQL DATABASE"
	invoke-sqlcmd -inputfile "restore.sql" -serverinstance "localhost\sql2012" -verbose -QueryTimeout 600

    $migrations = Join-Path $currentPath -ChildPath "..\lib\DbUp\TAL.QuoteAndApply.DbUp.exe"

    # RUN DB UP
    Write-Log "RUN DB UP"
	$params = @( )
	$params += "-t 90"
	$params += "-e QA"
	& $migrations $params | Write-Host
}

function Clean-Lucene-Indexes() {
	$appPool = "TALQuoteAndApply.SalesPortal"

	Write-Log "Stopping web application pool $appPool"
	Stop-WebAppPool $appPool

	# Wait for the apppool to shut down.
	do
	{
		Write-Log (Get-WebAppPoolState $appPool).Value
		Start-Sleep -Seconds 2
	}
	until ( (Get-WebAppPoolState -Name $appPool).Value -eq "Stopped" )

	$LuceneSalesIndexRootPath = "C:\ILSource\lucene\tal-sales"
	Write-Log "Clean Lucene Indexes Path $LuceneSalesIndexRootPath"
	Clean-Directory -DirectoryToClean $LuceneSalesIndexRootPath -FilesOnly $true

    $LuceneCustomerIndexRootPath = "C:\ilsource\lucene\tal-customer"
	Write-Log "Clean Lucene Indexes Path $LuceneCustomerIndexRootPath"
	Clean-Directory -DirectoryToClean $LuceneCustomerIndexRootPath -FilesOnly $true


	Write-Log "Starting web application pool $appPool"
	Start-WebAppPool $appPool

	# Wait for the apppool to shut down.
	do
	{
		Write-Log (Get-WebAppPoolState $appPool).Value
		Start-Sleep -Seconds 2
	}
	until ( (Get-WebAppPoolState -Name $appPool).Value -eq "Started" )
}

function Write-Log($Message){
    Write-Host "***** $($Message) *****"
}


$currentPath = ( Split-Path $MyInvocation.MyCommand.Path )

# If all parameters are false, the user didn't specify anything explicit to run
# Otherwise, we just forward on the parameters
if ($Clean -eq $false -and $Build -eq $false -and $SQL -eq $false -and $Lucene -eq $false){

	$Clean = $true
	$Build = $true
	$SQL = $true
	$Lucene = $true
}

Main -Clean $Clean -Build $Build -SQL $SQL -Lucene $Lucene