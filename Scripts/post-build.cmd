@rem Solution directory
SET SLNDIR=%1
@rem Project directory
SET PROJDIR=%2
@rem Output directory of the binaries
SET OUTDIR=%3
@rem Platform (x64, x86 or AnyCPU)
SET PLATFORM=%4
@rem Configuration (Debug or Release)
SET CONFIG=%5

@rem Remove the beginning and ending quotes of the three directories
SET SLNDIR=%SLNDIR:~1,-1%
SET OUTDIR=%OUTDIR:~1,-1%
SET PROJDIR=%PROJDIR:~1,-1%

@rem Copy the corresponding depending binaries
copy "%SLNDIR%Referenced Libraries\SDL2-CS.dll" "%PROJDIR%%OUTDIR%"
if "%PLATFORM%" == "x64" (
    copy "%SLNDIR%Referenced Libraries\x64\SDL2.dll" "%PROJDIR%%OUTDIR%"
) else (
    copy "%SLNDIR%Referenced Libraries\x86\SDL2.dll" "%PROJDIR%%OUTDIR%"
)
@rem Copy the application icon
copy "%PROJDIR%Images\favicon.ico" "%PROJDIR%%OUTDIR%"
@rem Delete the un-necessary files for Release mode
if "%CONFIG%" == "Release" (
    del "%PROJDIR%%OUTDIR%*.pdb"
    del "%PROJDIR%%OUTDIR%*vshost*"
)
@rem Delete the OpenTK xml file
del "%PROJDIR%%OUTDIR%OpenTK.xml"