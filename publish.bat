dotnet publish /p:PublishProfile=FolderProfile
mkdir build
mkdir "build/RampancyLightBridge/Halo CE/RampancyHalo1"
xcopy /y "./Rampancy.LightBridge\bin\Release\net8.0-windows7.0\win-x64\publish\win-x64\Rampancy.LightBridge.exe" "./build"
xcopy /s/e/y "./Rampancy.LightBridge\Assets\Trenchbroom\Halo1\RampancyHalo1" "./build/RampancyLightBridge/Halo CE/RampancyHalo1"
powershell Compress-Archive -Force ./build/* Rampancy.LightBridge.zip