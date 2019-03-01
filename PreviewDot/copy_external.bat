@set working_dir=%~1
@set output_dir=%~2
@set package_dir=packages\Graphviz.2.38.0.2

@echo %working_dir%%package_dir% to %output_dir%external
@xcopy /S /C /I /H /R /Y "%working_dir%%package_dir%" "%output_dir%external"

@REM remove some files we dont want to package.
@echo Removing superfluous files...
del "%output_dir%external\gd.zip"
del "%output_dir%external\Graphviz.2.38.0.2.nupkg"
del "%output_dir%external\New Text Document.txt"
del "%output_dir%external\Temp.rar"

@echo External files copied.
