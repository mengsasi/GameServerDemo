echo off

if not exist "./cs" mkdir "./cs"

cd cs

for /f "delims=" %%i in ('dir /a-d /b *.cs') do (  
    ::echo %%i
	copy .\%%i .\..\..\Game_Server\Assets\Scripts\Protos /y
)

for /f "delims=" %%i in ('dir /a-d /b *.cs') do (  
    ::echo %%i
	copy .\%%i .\..\..\Client\Assets\Scripts\Game\Protos /y
)

::pause