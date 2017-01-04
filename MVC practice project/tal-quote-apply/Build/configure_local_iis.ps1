param(    
    [Parameter(Position=1)][string]$basePath = 'C:\ilsource\tal-quote-apply',
    [Parameter(Position=2)][string]$salesPortalWebsite = 'TALQuoteAndApply.SalesPortal',
    [Parameter(Position=3)][string]$salesPortalWebsiteDist = 'TALQuoteAndApplyDist.SalesPortal',
	[Parameter(Position=4)][string]$salesPortalAppPool = 'TALQuoteAndApply.SalesPortal',
    [Parameter(Position=5)][string]$salesPortalBinding = 'local-TalSalesPortal.delivery.lan',
    [Parameter(Position=6)][string]$salesPortalBindingDist = 'local-TalSalesPortal-dist.delivery.lan',

	[Parameter(Position=7)][string]$customerWebsite = 'TALQuoteAndApply.Customer',
    [Parameter(Position=8)][string]$customerWebsiteDist = 'TALQuoteAndApplyDist.Customer',
	[Parameter(Position=9)][string]$customerAppPool = 'TALQuoteAndApply.Customer',
    [Parameter(Position=10)][string]$customerBinding = 'local-talcustomer.delivery.lan',
    [Parameter(Position=11)][string]$customerBindingDist = 'local-talcustomer-dist.delivery.lan',
	[Parameter(Position=5)][string]$thumbprint = 'fe82ba39a95c174d06b18316231617515efc2acb',
    [Parameter(Position=6)][string]$certuser = 'NetworkService',
    [Parameter(Position=6)][string]$certificate_file_name = 'PCI\TokenisationCertificate_DEV-QA-UAT.pfx',
    [Parameter(Position=6)][string]$certificate_password = 'Manitoba'
)

## Comment ##

Import-Module WebAdministration
$ErrorActionPreference = "Stop"

$salesPortalPath = "$basePath\TAL.QuoteAndApply.SalesPortal.Web"
$salesPortalDistPath = "$basePath\TAL.QuoteAndApply.SalesPortal.Web\dist"

$customerPath = "$basePath\TAL.QuoteAndApply.Customer.Web"
$customerDistPath = "$basePath\TAL.QuoteAndApply.Customer.Web\dist"


if(!(Test-Path($salesPortalPath))){
    Write-Error "The website source doesn't exist @ $salesPortalPath"
}
if(!(Test-Path($salesPortalDistPath))){
    Write-Error "The dist website source doesn't exist @ $salesPortalDistPath"
}

if(!(Test-Path($customerPath))){
    Write-Error "The website source doesn't exist @ $customerPath"
}
if(!(Test-Path($customerDistPath))){
    Write-Error "The dist website source doesn't exist @ $customerDistPath"
}

$salesPortalWebsitePath = "IIS:\Sites\$salesPortalWebsite"
$salesPortalWebsiteDistPath = "IIS:\Sites\$salesPortalWebsiteDist"
$salesPortalAppPoolPath = "IIS:\AppPools\$salesPortalAppPool"

$customerWebsitePath = "IIS:\Sites\$customerWebsite"
$customerWebsiteDistPath = "IIS:\Sites\$customerWebsiteDist"
$customerAppPoolPath = "IIS:\AppPools\$customerAppPool"

############# Remove the site from IIS if one exists

if(Test-Path($salesPortalWebsitePath)){    
    Write-Host "Stopping web site $salesPortalWebsite" -foregroundcolor "magenta"
    Stop-Website $salesPortalWebsite
    
    Write-Host "Removing web site $salesPortalWebsite" -foregroundcolor "magenta"
    Remove-WebSite $salesPortalWebsite
}

if(Test-Path($salesPortalWebsiteDistPath)){    
    Write-Host "Stopping dist web site $salesPortalWebsiteDist" -foregroundcolor "magenta"
    Stop-Website $salesPortalWebsiteDist
    
    Write-Host "Removing web site $salesPortalWebsiteDist" -foregroundcolor "magenta"
    Remove-WebSite $salesPortalWebsiteDist
}


if(Test-Path($customerWebsitePath)){    
    Write-Host "Stopping web site $customerWebsite" -foregroundcolor "magenta"
    Stop-Website $customerWebsite
    
    Write-Host "Removing web site $customerWebsite" -foregroundcolor "magenta"
    Remove-WebSite $customerWebsite
}

if(Test-Path($customerWebsiteDistPath)){    
    Write-Host "Stopping dist web site $customerWebsiteDist" -foregroundcolor "magenta"
    Stop-Website $customerWebsiteDist
    
    Write-Host "Removing web site $customerWebsiteDist" -foregroundcolor "magenta"
    Remove-WebSite $customerWebsiteDist
}

############# Remove the app pool

if(Test-Path($salesPortalAppPoolPath)){
    Write-Host "Removing app pool $salesPortalWebsite" -foregroundcolor "magenta"
    Remove-WebAppPool $salesPortalAppPool
}

if(Test-Path($customerAppPoolPath)){
    Write-Host "Removing app pool $customerWebsite" -foregroundcolor "magenta"
    Remove-WebAppPool $customerAppPool
}

############# Create the app pool

Write-Host "Creating app pool $salesPortalAppPool" -foregroundcolor "magenta"
$pool = New-WebAppPool $salesPortalAppPool -Force
Set-ItemProperty $salesPortalAppPoolPath -name managedRuntimeVersion -value "v4.0"
Set-ItemProperty $salesPortalAppPoolPath -name enable32BitAppOnWin64 -value True
Set-ItemProperty $salesPortalAppPoolPath -Name ProcessModel.IdentityType -value 2 ## Network Service


Write-Host "Creating app pool $customerAppPool" -foregroundcolor "magenta"
$pool = New-WebAppPool $customerAppPool -Force
Set-ItemProperty $customerAppPoolPath -name managedRuntimeVersion -value "v4.0"
Set-ItemProperty $customerAppPoolPath -name enable32BitAppOnWin64 -value True
Set-ItemProperty $customerAppPoolPath -Name ProcessModel.IdentityType -value 2 ## Network Service

############# Create the site

Write-Host "Creating site for $salesPortalWebsite" -foregroundcolor "magenta"
New-Item $salesPortalWebsitePath -bindings @{protocol="http";bindingInformation=":80:$salesPortalBinding"} -physicalPath $salesPortalPath

Write-Host "Creating site for $salesPortalWebsiteDist" -foregroundcolor "magenta"
New-Item $salesPortalWebsiteDistPath -bindings @{protocol="http";bindingInformation=":80:$salesPortalBindingDist"} -physicalPath $salesPortalDistPath

Write-Host "Creating site for $customerWebsite" -foregroundcolor "magenta"
New-Item $customerWebsitePath -bindings @{protocol="http";bindingInformation=":80:$customerBinding"} -physicalPath $customerPath

Write-Host "Creating site for $customerWebsiteDist" -foregroundcolor "magenta"
New-Item $customerWebsiteDistPath -bindings @{protocol="http";bindingInformation=":80:$customerBindingDist"} -physicalPath $customerDistPath

############# Set the application pool for site

Write-Host "Setting app pool for $salesPortalWebsite" -foregroundcolor "magenta"
Set-ItemProperty $salesPortalWebsitePath -name applicationPool -value $salesPortalAppPool

Write-Host "Setting app pool for $salesPortalWebsiteDist" -foregroundcolor "magenta"
Set-ItemProperty $salesPortalWebsiteDistPath -name applicationPool -value $salesPortalAppPool

Write-Host "Setting app pool for $customerWebsite" -foregroundcolor "magenta"
Set-ItemProperty $customerWebsitePath -name applicationPool -value $customerAppPool

Write-Host "Setting app pool for $customerWebsiteDist" -foregroundcolor "magenta"
Set-ItemProperty $customerWebsiteDistPath -name applicationPool -value $customerAppPool

############# Update Authentication
Write-Host "Setting authentication for $salesPortalWebsite" -foregroundcolor "magenta"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/AnonymousAuthentication -name enabled -value false -location "$salesPortalWebsite"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/windowsAuthentication -name enabled -value true -location "$salesPortalWebsite"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/basicAuthentication -name enabled -value false -location "$salesPortalWebsite"

Write-Host "Setting authentication for $salesPortalWebsiteDist" -foregroundcolor "magenta"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/AnonymousAuthentication -name enabled -value false -location "$salesPortalWebsiteDist"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/windowsAuthentication -name enabled -value true -location "$salesPortalWebsiteDist"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/basicAuthentication -name enabled -value false -location "$salesPortalWebsiteDist"

Write-Host "Setting authentication for $customerWebsite" -foregroundcolor "magenta"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/AnonymousAuthentication -name enabled -value true -location "$customerWebsite"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/windowsAuthentication -name enabled -value false -location "$customerWebsite"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/basicAuthentication -name enabled -value false -location "$customerWebsite"

Write-Host "Setting authentication for $customerWebsiteDist" -foregroundcolor "magenta"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/AnonymousAuthentication -name enabled -value true -location "$customerWebsiteDist"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/windowsAuthentication -name enabled -value false -location "$customerWebsiteDist"
Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/basicAuthentication -name enabled -value false -location "$customerWebsiteDist"

############# Enable localhost BackConnectionHostNames windows authentication ###############

Write-Host "Enabling localhost BackConnectionHostNames windows authentication" -foregroundcolor "magenta"
regedit /s $basePath\Build\enable_local_windows_authentication.reg

############ restart iis ##############
Write-Host "restarting IIS" -foregroundcolor "magenta"
invoke-command -scriptblock {iisreset}

############# Host entry
. (Join-Path 'Carbon' 'Import-Carbon.ps1')

Set-HostsEntry -IPAddress 127.0.0.1 -HostName "$salesPortalBinding"
Set-HostsEntry -IPAddress 127.0.0.1 -HostName "$salesPortalBindingDist"

Set-HostsEntry -IPAddress 127.0.0.1 -HostName "$customerBinding"
Set-HostsEntry -IPAddress 127.0.0.1 -HostName "$customerBindingDist"

############ Install PCI certificate ################

Write-Host "Adding/updating certificate in store" -foregroundcolor "magenta"

## $certFullPath = $basePath + "\Build\" + $certificate_file_name
## $certBytes = [System.IO.File]::ReadAllBytes($certFullPath)
## $cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certBytes, $certificate_password, "Exportable,MachineKeySet,PersistKeySet")

## if ($cert.PrivateKey -ne $null) {
## ##     Write-Host $cert.PrivateKey.CspKeyContainerInfo.UniqueKeyContainerName
## }
## $store = New-Object System.Security.Cryptography.X509Certificates.X509Store("My", "LocalMachine")
## $store.Open("ReadWrite")
## $store.Add($cert)
## $store.Close()

   
	Write-Host $certificate_file_name
    if ( (Test-Path "Cert:\LocalMachine\My\$thumbprint") -eq $false ) {
        Write-Host "Installing cert $thumbprint"
        & "certutil" "-f" "-p" "$($certificate_password)" "-importpfx" "$($certificate_file_name)"    
    }



############ Grant AppPool User Access to PCI certificate ################

Write-Host "Granting AppPool identity '$certuser' access to certificate" -foregroundcolor "magenta"

$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("My", "LocalMachine")
$store.Open("ReadWrite")

$col = $store.Certificates.Find([System.Security.Cryptography.X509Certificates.X509FindType]"FindByThumbprint", $thumbprint, $false)

Foreach ($certificate in $col) {

    Write-Host $certificate

    if ($certificate.PrivateKey -eq $null) {
        Write-Error ("Certificate doesn't have Private Key") -ErrorAction:Stop
    }

    $privateKeysPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::CommonApplicationData) + "\Microsoft\Crypto\RSA\MachineKeys"
    
    Write-Host $privateKeysPath
    
    $file = New-Object System.IO.FileInfo($privateKeysPath + "\" + $certificate.PrivateKey.CspKeyContainerInfo.UniqueKeyContainerName)

    Write-Host $file

    $fs = $file.GetAccessControl()

    $account = New-Object System.Security.Principal.NTAccount($certuser)
    $fileSystemRights = New-Object System.Security.AccessControl.FileSystemAccessRule($account, [System.Security.AccessControl.FileSystemRights]"FullControl", [System.Security.AccessControl.AccessControlType]"Allow")

    $fs.AddAccessRule($fileSystemRights)
    $file.SetAccessControl($fs)
}

$store.Close()


############ Done #####################
Write-Host "Done" -foregroundcolor "green"