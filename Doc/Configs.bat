@echo off & setlocal EnableDelayedExpansion

cd ..\\Tool\\

::excel表目录 上级目录 ..\\..\\test\\
set excelPath="..\\Doc\\"
::导出目录
set exportPath="..\\Doc\\"

set file=%~n0%.xlsx
::删除旧文件
set existFile=%exportPath%%file%
if exist %existFile% rd /s/q %existFile%

if exist %excelPath%%file% python excel2Json.py %excelPath%%file% %exportPath%

pause