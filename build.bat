@echo off

IF(%2)==(build) GOTO Build:
IF(%2)==(run) GOTO Run:

GOTO Error:

:Build
%1 CivOne.csproj
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