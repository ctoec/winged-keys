New-Item c:/cfn/postinstall.txt
Invoke-Expression -Command:"icacls C:/inetpub/AspNetCoreWebApps/winged-keys-app /grant 'IIS AppPool\DefaultAppPool:(OI)(CI)M'"