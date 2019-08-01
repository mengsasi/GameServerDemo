echo off

if not exist "./cs" mkdir "./cs"

cd protos

for /f "delims=" %%i in ('dir /a-d /b *.proto') do (  
    ::echo %%i
	protoc.exe ./%%i --csharp_out=./../cs/
)

::pause
