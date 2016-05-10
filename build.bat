@echo off

IF(%2)==(build) GOTO Build:
IF(%2)==(run) GOTO Run:
IF(%2)==(setup) GOTO Setup:

GOTO Error:

:Build
%1 CivOne.csproj /t:Rebuild /p:Configuration=%3
GOTO End:

:Run
bin\CivOne.exe
GOTO End:

:Setup
bin\CivOne.exe setup
GOTO End:

:Error
echo.
echo Invalid build parameters.
echo Usage: build.bat [msbuild^|xbuild] [build^|run]
echo.

:End