if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (GOTO 64BIT) ELSE (GOTO 32BIT)

:64BIT
%windir%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe CsharpSpell.sln /p:Configuration=Release
GOTO END

:32BIT
%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe CsharpSpell.sln /p:Configuration=Release
GOTO END

:END

pause