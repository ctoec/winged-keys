#
# Touch token file for post deploy start (for debugging purposes only)
#
fc > c:/cfn/postinstall-start.txt

#
# Give IIS AppPool\DefaultAppPool full control on webapp root
#
Invoke-Expression -Command:"icacls C:/inetpub/AspNetCoreWebApps/winged-keys-app /grant 'IIS AppPool\DefaultAppPool:(OI)(CI)F'"

#
# Display permissions for webapp root
#
Invoke-Expression -Command:"icacls C:/inetpub/AspNetCoreWebApps/winged-keys-app"

#
# Copy entity framework dll to web root
#
Copy-Item -force C:/inetpub/AspNetCoreWebApps/winged-keys-app/installer/lib/ef.dll C:/inetpub/AspNetCoreWebApps/winged-keys-app/ef.dll

#
# Touch token file for post deploy end (for debugging purposes only)
#
fc > c:/cfn/postinstall-end.txt