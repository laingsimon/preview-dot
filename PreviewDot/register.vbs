installPath=Session.Property("CustomActionData")
Set WShShell = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
frameworkPath="c:\WINDOWS\Microsoft.NET\Framework64\v4.0.30319"

if fso.FolderExists(frameworkPath) = False then
    frameworkPath="c:\WINDOWS\Microsoft.NET\Framework\v4.0.30319"
end if

strCommand = """" & frameworkPath & "\RegAsm.exe """ & installPath & " /nologo /codebase"
WshShell.Run strCommand, True, 1
