set WORKSPACE=.\
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t client ^
    -c gameframework-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x namingConvention.enumItem=pascal ^
    -x outputDataDir=..\Assets\GameRes\Common\Config ^
    -x outputCodeDir=..\Assets\GameModules\Proto\Runtime\Config

pause