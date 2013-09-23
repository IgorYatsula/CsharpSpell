
if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (GOTO 64BIT) ELSE (GOTO 32BIT)

:64BIT
Dependencies\NUnit\bin\nunit.exe CsharpSpell.Tests\bin\Debug\CsharpSpell.Tests.dll /run
GOTO END

:32BIT
Dependencies\NUnit\bin\nunit-x86.exe CsharpSpell.Tests\bin\Debug\CsharpSpell.Tests.dll /run
GOTO END

:END