@REM should be run from the installed directory as an administrator

@set netframework32=c:\Windows\Microsoft.NET\Framework64\v4.0.30319\
@set netframework64=c:\Windows\Microsoft.NET\Framework\v4.0.30319\
@set path=%netframework32%;%netframework64%;%path%
@set install_path=%~dp0

@regasm.exe "%install_path%PreviewDot.dll" /unregister

@cd ..
@rmdir "%install_path%" /s /q
@pause