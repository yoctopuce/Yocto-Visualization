@ECHO OFF
echo.

IF "%1" == "" goto release
IF %1 == release goto release
IF %1 == clean goto clean
echo INVALID TARGET
goto end


:clean
msbuild /t:Clean
del /Q /F bin\Debug\*
rmdir /S /Q obj
goto end

:release
nuget.exe restore
msbuild /property:Configuration=Release
goto end


:error
echo msbuild failled
exit /b 1
:end