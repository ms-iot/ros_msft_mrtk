@echo off

set ROS2DOTNET_Workspace=%1
if "%ROS2DOTNET_Workspace%" == "" goto :help

set ROS2DOTNET_install=%ROS2DOTNET_Workspace%\target\install
set UnityProjectName=SampleProject
set AssetsFolder=%~dp0%UnityProjectName%\Assets\Plugins

echo Copying from %ROS2DOTNET_install% to %AssetsFolder%

if not exist %AssetsFolder% (mkdir %AssetsFolder%)


xcopy /y /c %ROS2DOTNET_install%\bin\*.dll %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\bin\*.pdb %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\builtin_interfaces\dotnet\builtin_interfaces_assemblies.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\builtin_interfaces\dotnet\rcldotnet_common.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\rcldotnet\dotnet\rcldotnet_assemblies.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\rclcppdotnet\dotnet\rclcppdotnet_assemblies.* %AssetsFolder%

for /R "%ROS2DOTNET_install%\lib\" %%f in (*_msgs_assemblies.*) do  xcopy  /y /c %%f %AssetsFolder%

goto :eof

:help
    echo Copy Assets moves binaries from a ROS2.net workspace into Unity for inclusion
    echo into a Hololens or VR application
    echo.
    echo.
    echo    copy_assets.cmd "path to ros2 workspace"

goto :eof
