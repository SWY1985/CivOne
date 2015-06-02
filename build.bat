@echo off

ECHO.
ECHO %1
ECHO %2
ECHO %3
ECHO.

IF(%2)==(build) GOTO Build:
IF(%2)==(run) GOTO Run:

GOTO Error:

:Build
@echo on
%1 CivOne.csproj /t:Rebuild /p:Configuration=%3
@echo off
GOTO End:

:Run
bin\CivOne.exe
GOTO End:

:Error
echo.
echo Invalid build parameters.
echo Usage: build.bat [msbuild^|xbuild] [build^|run]
echo.

:End