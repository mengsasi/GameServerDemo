echo off

for /f "delims=" %%i in ('dir /a-d /b *.json') do (  
    ::echo %%i
	copy .\%%i .\..\Game_Server\Assets\Resources /y
)

::for /f "delims=" %%i in ('dir /a-d /b *.json') do (  
::    ::echo %%i
::	copy .\%%i .\..\Client\Assets\Resources /y
::)

for /f "delims=" %%i in ('dir /a-d /b *.json') do (  
    ::echo %%i
	copy .\%%i .\..\Database /y
)

::pause