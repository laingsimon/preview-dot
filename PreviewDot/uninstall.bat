@REM should be run from the installed directory

@set netframework32=c:\Windows\Microsoft.NET\Framework64\v4.0.30319\
@set netframework64=c:\Windows\Microsoft.NET\Framework\v4.0.30319\
@set path=%netframework32%;%netframework64%;%path%

@regasm.exe PreviewIo.dll /unregister

@cd ..
@rmdir PreviewIo /s /q