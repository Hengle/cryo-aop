@echo off
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_2_0_Debug;NoWarn="0649;1685" /verbosity:normal
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_2_0_Release;NoWarn="0649;1685" /verbosity:normal
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_3_5_Debug;NoWarn=0649 /verbosity:normal
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_3_5_Release;NoWarn=0649 /verbosity:normal
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_4_0_Debug;NoWarn=0649 /verbosity:normal
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_4_0_Release;NoWarn=0649 /verbosity:normal
pause