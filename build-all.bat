set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_2_0_Debug
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_2_0_Release
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_3_5_Debug
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_3_5_Release
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_4_0_Debug
msbuild .\CryoAOP.sln /t:Rebuild /p:Configuration=net_4_0_Release
pause