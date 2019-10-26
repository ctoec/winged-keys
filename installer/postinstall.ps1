New-Item c:/cfn/postinstall-start.txt
Invoke-Expression -Command:"icacls C:/inetpub/AspNetCoreWebApps/winged-keys-app /grant 'IIS AppPool\DefaultAppPool:(OI)(CI)M'"
Invoke-Expression -Command:"icacls C:/inetpub/AspNetCoreWebApps/winged-keys-app"
New-Item c:/cfn/postinstall-end.txt