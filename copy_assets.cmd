:: @echo off
echo Copy Assets moves binaries from a ROS2.net workspace into Unity for inclusion
echo into a Hololens or VR application

set ROS2DOTNET_Workspace=c:\ws\r2_dotnet_ws
set ROS2DOTNET_install=%ROS2DOTNET_Workspace%\install
set ROS2DOTNET_build=%ROS2DOTNET_Workspace%\build
set UnityProjectName=SampleProject
set ROS2_OPT=c:\opt\ros\%ROS_DISTRO%\%VSCMD_ARG_HOST_ARCH%
set ROS2_DEPS=c:\opt\rosdeps\%VSCMD_ARG_HOST_ARCH%
set AssetsFolder=%~dp0%UnityProjectName%\Assets\Plugins

echo Copying from %ROS2DOTNET_Workspace% to %AssetsFolder%

if not exist %AssetsFolder% (mkdir %AssetsFolder%)


xcopy  /y /c %ROS2_DEPS%\bin\tinyxml2.dll %AssetsFolder%
xcopy  /y /c %ROS2_DEPS%\bin\libcrypto-1_1-x64.dll %AssetsFolder%
xcopy  /y /c %ROS2_DEPS%\bin\libssl-1_1-x64.dll %AssetsFolder%
xcopy  /y /c %ROS2_DEPS%\bin\PocoFoundation.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\fastcdr-1.0.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\fastrtps-1.9.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\yaml.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rcutils.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rcl_yaml_param_parser.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rosidl_generator_c.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rmw.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rosidl_typesupport_introspection_c.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rosidl_typesupport_fastrtps_cpp.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rmw_fastrtps_shared_cpp.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rosidl_typesupport_fastrtps_c.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rmw_fastrtps_cpp.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rmw_implementation.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rosidl_typesupport_c.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rcl_logging_spdlog.dll %AssetsFolder%
xcopy  /y /c %ROS2_OPT%\bin\rcl.dll %AssetsFolder%

xcopy /y /c %ROS2DOTNET_install%\bin\*.dll %AssetsFolder%
xcopy /y /c %ROS2DOTNET_install%\bin\*.pdb %AssetsFolder%
xcopy /y /c %ROS2DOTNET_build%\rcldotnet\RelWithDebInfo\*.pdb %AssetsFolder%
xcopy /y /c %ROS2DOTNET_build%\rmw_implementation\RelWithDebInfo\*.pdb %AssetsFolder%
xcopy  /y /c %ROS2DOTNET_install%\lib\rcldotnet\dotnet\rcldotnet_assemblies.* %AssetsFolder%
xcopy  /y /c %ROS2DOTNET_install%\lib\rcldotnet\dotnet\rcldotnet_common.* %AssetsFolder%
xcopy  /y /c %ROS2DOTNET_install%\lib\rcldotnet\dotnet\*_msgs_assemblies.* %AssetsFolder%
xcopy  /y /c %ROS2DOTNET_install%\lib\rcldotnet\dotnet\builtin_interfaces_assemblies.* %AssetsFolder%
xcopy  /y /c %ROS2DOTNET_install%\lib\rcldotnet\dotnet\builtin_interfaces_assemblies.* %AssetsFolder%

