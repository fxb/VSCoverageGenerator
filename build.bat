@echo off
call "%VS120COMNTOOLS%vsvars32.bat"
csc /out:VSCoverageGenerator.exe /platform:x86 /recurse:src\*.cs /reference:Microsoft.VisualStudio.Coverage.Analysis.dll
