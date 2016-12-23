@rem Module name
SET MODULE=%1
@rem Path to AssemblyInfo.cs for this project
SET ASMINFOPATH="%2Properties\AssemblyInfo.cs"
@rem Solution directory
SET SLNDIR=%3
@rem Get number of commits made to Git repo as the revision number
cd /d %3
for /f %%i in ('git rev-list --count HEAD') do set REVISION=%%i
@rem Assign other numbers on your own
SET BUILD=0
SET MINOR=0
SET MAJOR=1

@rem Append AssmeblyInfo.cs strings
echo. > %ASMINFOPATH%
echo using System.Reflection; >> %ASMINFOPATH%
echo using System.Runtime.CompilerServices; >> %ASMINFOPATH%
echo using System.Runtime.InteropServices; >> %ASMINFOPATH%
echo. >> %ASMINFOPATH%
echo [assembly: AssemblyTitle("%MODULE%")] >> %ASMINFOPATH%
echo [assembly: AssemblyDescription("")] >> %ASMINFOPATH%
echo [assembly: AssemblyConfiguration("")] >> %ASMINFOPATH%
echo [assembly: AssemblyCompany("Alex Leung")] >> %ASMINFOPATH%
echo [assembly: AssemblyProduct("Open Bus Driving Simulator")] >> %ASMINFOPATH%
echo [assembly: AssemblyCopyright("")] >> %ASMINFOPATH%
echo [assembly: AssemblyTrademark("")] >> %ASMINFOPATH%
echo [assembly: AssemblyCulture("")] >> %ASMINFOPATH%
echo [assembly: ComVisible(false)] >> %ASMINFOPATH%
echo [assembly: AssemblyVersion("%MAJOR%.%MINOR%.%BUILD%.%REVISION%")] >> %ASMINFOPATH%
echo [assembly: AssemblyFileVersion("%MAJOR%.%MINOR%.%BUILD%.%REVISION%")] >> %ASMINFOPATH%