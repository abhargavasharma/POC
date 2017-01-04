$TokenisationCertThumbPrint = "fe82ba39a95c174d06b18316231617515efc2acb"
    $TokenisationCertLocation = "PCI\TalTokenisationCertificate_dev.pfx"
	Write-Host $TokenisationCertLocation
    $TokenisationCertPassword = "d4DcBGv2Ghw74tB"

    if ( (Test-Path "Cert:\LocalMachine\My\$TokenisationCertThumbPrint") -eq $false ) {
        Write-Host "Installing cert $TokenisationCertThumbPrint"
        & "certutil" "-f" "-p" "$($TokenisationCertPassword)" "-importpfx" "$($TokenisationCertLocation)"    
    }

