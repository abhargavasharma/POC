 param (
[string]$environment = "",
[string]$connectionstring = ""
)

$params = @( )
$params += "-t 300"

if($environment -ne "")
{
$params += "-e"
$params += $environment
}

if($connectionString -ne "")
{
$params += "-c"
$params += $connectionString
}

& .\TAL.QuoteAndApply.DbUp.exe $params | Write-Host
