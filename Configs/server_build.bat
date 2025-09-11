set WORKSPACE=.\
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t server ^
    -c my-protobuf3 ^
    -d protobuf3-bin ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=.\Test\Cpp\CodeGen\config ^
    -x outputDataDir=.\Test\Cpp\Data

pause