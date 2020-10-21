@echo off

set ROS2DOTNET_Workspace=%1
if "%ROS2DOTNET_Workspace%" == "" goto :help

set ROS2DOTNET_install=%ROS2DOTNET_Workspace%\target\x64
set UnityProjectName=SampleProject
set AssetsFolder=%~dp0%UnityProjectName%\Assets\Plugins\x86_64

echo Copying from %ROS2DOTNET_install% to %AssetsFolder%

if not exist %AssetsFolder% (mkdir %AssetsFolder%)

rem Cleanup
del %AssetsFolder%\*.dll
del %AssetsFolder%\*.pdb
del %AssetsFolder%\*.deps.json

rem ROS2.NET
xcopy /y /c %ROS2DOTNET_install%\bin\*.dll %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\bin\*.pdb %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\builtin_interfaces\dotnet\builtin_interfaces_assemblies.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\builtin_interfaces\dotnet\rcldotnet_common.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\rcldotnet\dotnet\rcldotnet_assemblies.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\rclcppdotnet\dotnet\rclcppdotnet_assemblies.* %AssetsFolder%

rem Apriltags
xcopy /y /c C:\opt\vcpkg\buildtrees\apriltag\x64-windows-rel\*.dll %AssetsFolder%
xcopy /y /c C:\opt\vcpkg\buildtrees\apriltag\x64-windows-rel\*.pdb %AssetsFolder%

xcopy /y /c %ROS2DOTNET_Workspace%\src\build\x64\Release\*.dll %AssetsFolder%
xcopy /y /c %ROS2DOTNET_Workspace%\src\build\x64\Release\*.pdb %AssetsFolder%

xcopy /y /c c:\opt\vcpkg\installed\x64-windows\bin\*.dll %AssetsFolder%
xcopy /y /c c:\opt\vcpkg\installed\x64-windows\bin\*.pdb %AssetsFolder%

for /R "%ROS2DOTNET_install%\lib\" %%f in (*_msgs_assemblies.*) do  xcopy  /y /c %%f %AssetsFolder%



set ROS2DOTNET_install=%ROS2DOTNET_Workspace%\target\arm64
set AssetsFolder=%~dp0%UnityProjectName%\Assets\Plugins\WSA\arm64
if not exist %AssetsFolder% (mkdir %AssetsFolder%)

rem ROS2.NET
xcopy /y /c %ROS2DOTNET_install%\bin\*.dll %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\bin\*.pdb %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\builtin_interfaces\dotnet\builtin_interfaces_assemblies.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\builtin_interfaces\dotnet\rcldotnet_common.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\rcldotnet\dotnet\rcldotnet_assemblies.* %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\Lib\rclcppdotnet\dotnet\rclcppdotnet_assemblies.* %AssetsFolder%

rem Apriltags
xcopy /y /c C:\opt\vcpkg\buildtrees\apriltag\arm64-uwp-rel\Release\*.dll %AssetsFolder%
xcopy /y /c C:\opt\vcpkg\buildtrees\apriltag\arm64-uwp-rel\Release\*.pdb %AssetsFolder%

xcopy /y /c %ROS2DOTNET_Workspace%\src\build\arm64\Release\*.dll %AssetsFolder%
xcopy /y /c %ROS2DOTNET_Workspace%\src\build\arm64\Release\*.pdb %AssetsFolder%

xcopy /y /c c:\opt\vcpkg\installed\arm64-uwp\bin\*.dll %AssetsFolder%
xcopy /y /c c:\opt\vcpkg\installed\arm64-uwp\bin\*.pdb %AssetsFolder%

for /R "%ROS2DOTNET_install%\lib\" %%f in (*_msgs_assemblies.*) do  xcopy  /y /c %%f %AssetsFolder%

goto :eof

:help
    echo Copy Assets moves binaries from a ROS2.net workspace into Unity for inclusion
    echo into a Hololens or VR application.
    echo.
    echo.
    echo    copy_assets.cmd "path to ros2 workspace"

goto :eof
