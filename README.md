> The MRTK for ROS package is currently in preview: Feature requests and bug reports are very much welcome at the [issues page](https://github.com/ms-iot/ros_msft_mrtk/issues).
>
> We want to hear what features you want in order to accomplish mixed reality Robotics scenarios!
>
> During the preview, this repository will be available only as sources. As it exits preview, a Nuget package will be provided.

# Mixed Reality Toolkit for ROS Overview
The Robot Operating system - also called ROS [^1] - is an open source middleware for creating Robots. This repository implements glue which connects your Mixed Reality Robotics application with Robots and infrastructure running ROS1 and/or ROS2. It works with [RosSharp](https://github.com/siemens/ros-sharp) and/or [ROS2.net](https://github.com/ros2-dotnet/ros2_dotnet) within the Unity Game Engine.

The Mixed Reality Toolkit is an Open Source framework for building Virtual Reality and Augmented Reality applications, which target numerous platforms - including Hololens.

The MRTK for ROS Unity Extension is being developed by the ROS2.net maintainers, the Microsoft Edge Robotics team and the Mixed Reality Robotics team.

# Getting started - ROS1
This project depends on your installation of ROS1 melodic for Windows.

> NOTE: Because of the dependency chain, Windows is required. MRTK for ROS development will not be supported on other platforms.
> However, once deployed the MRTK ROS application will support visualizing robots running ROS2 on other platforms.

## Installing ROS1

Please follow the [instructions for installing ROS1 on Windows](https://wiki.ros.org/Installation/Windows).


# Getting started - ROS2
This project depends on your installation of ROS2 eloquent and ROS2.net for eloquent for Windows.

> NOTE: Because of the dependency chain, Windows is required. MRTK for ROS development will not be supported on other platforms.
> However, once deployed the MRTK ROS application will support visualizing robots running ROS2 on other platforms.

## Installing ROS2

Please follow the [Microsoft ROS 2 Getting Started instructions](http://aka.ms/ros/setup_ros2).

> If you are targetting Hololens 2, please add the ARM64 workloads. NOTE: Due to cross compile and toolchain support, Hololens 2 support in progress and available later in 2020.

To help bootstrap your application, we also recommend setting up a [Nav2 simulation environment](https://ms-iot.github.io/ROSOnWindows/ros2/nav2.html), which can be used in VR mode.

## Before building your MRTK ROS application
MRTK ROS applications require binaries from ROS2 to be built on your computer.
Please follow the instructions for [ROS2 on UWP](https://github.com/ooeygui/ros2_uwp)

This project requires an interesting environment setup. Supporting UWP requires that the ROS2 binaries have no environmental dependency.

### Environment 1: Build environment.

For this environment, I have a workspace created in c:\ws\r2_dotnet_ws, which borrows some environment from a full ROS2 install (like colcon), but not dependencies like rosdep or vcpkg.

Create `c:\ws\r2_dotnet_ws\env.bat`:

``` batch
set "ChocolateyInstall=c:\opt\chocolatey"
set "PATH=c:\opt\chocolatey\bin;C:\opt\python37amd64\;C:\opt\python37amd64\Scripts;%PATH%"
set PATH=C:\opt\python37amd64\DLLs;%PATH%
set CMAKE_PREFIX_PATH=C:\opt\rosdeps\x64\include;%CMAKE_PREFIX_PATH%
```

``` json
            {
                "commandline" : "C:\\Windows\\System32\\cmd.exe /k \"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\Common7\\Tools\\VsDevCmd.bat\" -arch=amd64 -host_arch=amd64 && set ChocolateyInstall=c:\\opt\\chocolatey&& call c:\\ws\\r2_dotnet_ws\\env.bat",
                "guid" : "{6ad9f06b-1b12-4d58-8657-3335b10f3d4c}",
                "name" : "ROS 2 - dotnet",
                "startingDirectory" : "c:\\ws\\r2_dotnet_ws",
                "tabTitle" : "R2 .net"
            },
```

#### Running in the Unity editor
When running in the Unity editor, do not build with the WindowsStore build prompt.

``` batch
cd c:\ws\r2_dotnet_ws\tools
colcon build --merge-install --cmake-args -DBUILD_TESTING=OFF
install\local_setup.bat

cd c:\ws\r2_dotnet_ws\target
colcon build --merge-install --packages-ignore rmw_fastrtps_dynamic_cpp rcl_logging_log4cxx rclcpp_components ros2trace tracetools_launch tracetools_read tracetools_test tracetools_trace --cmake-args -DRMW_IMPLEMENTATION=rmw_fastrtps_cpp -DTHIRDPARTY=ON -DINSTALL_EXAMPLES=OFF -DBUILD_TESTING=OFF

```

If you need to debug the middleware, add `-DCMAKE_BUILD_TYPE=RelWithDebInfo` to the end of the colcon command line.

### Environment 2: Full ROS2 Environment
In this environment, you'll use your regular ROS2 build environment - which has rosdeps, vcpkg and the full runtime environment.

This is used for launching Robot or talker apps.

Here I have a turtlebot workspace created using [Microsoft's Nav2 Documentation](https://ms-iot.github.io/ROSOnWindows/ros2/nav2.html)

``` json
            {
                "commandline" : "C:\\Windows\\System32\\cmd.exe /k \"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\Common7\\Tools\\VsDevCmd.bat\" -arch=amd64 -host_arch=amd64 && set ChocolateyInstall=c:\\opt\\chocolatey&& call c:\\ws\\r2_turtle3_ws\\bootstrap.bat",
                "guid" : "{25ee96fe-a3c5-41a3-b913-205c45dfc064}",
                "name" : "ROS 2 - Turtlebot",
                "startingDirectory" : "c:\\ws\\r2_turtle3_ws",
                "tabTitle" : "R2 TBot"
            },
```

To test the ros2.net talker application, in the Full ROS2 environment, you can cd into `c:\\ws\\r2_dotnet_ws` and use its setup:

``` batch
cd c:\ws\r2_dotnet_ws\target
install\setup.bat
ros2 run rcldotnet_examples rcldotnet_talker.exe
```

# Installing the fiducial marker (Apriltag) package

The fiducial system is for processing camera images to locate the robot in space.

``` batch
cd c:\opt\vcpkg
git remote set-url origin https://github.com/ooeygui/vcpkg
git pull
bootstrap-vcpkg.bat
vcpkg install apriltag:x64-windows
vcpkg install opencv-c-wrapper:x64-windows
```


# Bootstrapping your Mixed Reality Toolkit Application

  * Clone this repository to your computer.
  * Open the Unity Hub
  * Select add a project
  * Open the SampleProject from this repository.
  * Ensure that your Unity project is using the `.NET 4.x` Api Compatibility Level (Edit > Project Settings > Player > Other Settings > Api Compatibility Level)
  * ### For manipulating ROS1 robots within Unity:
    * Open the asset store and navigate to the free 'ros#' package
    * Download the 'ros#' package
    * Copy the contents of the 'Ros1Module' directory from the root of this repository into the SampleProject/Assets folder
    * Open up `SampleProject/Assets/csc.rsp` in a text editor and uncomment the `#-define:ROS1_MODULE_LIDAR` line (remove the '#' character at the start of the line)
  * ### For manipulating ROS2 robots within Unity:

    * From the ROS command prompt, change the directory to where you cloned the MRTK extension, and run `copy_assets.cmd c:\ws\r2_dotnet_ws` (make sure your Unity client is closed for this step!)

    * Copy the contents of the 'Ros2Module' directory from the root of this repository into the SampleProject/Assets folder
    * Open up `SampleProject/Assets/csc.rsp` in a text editor and uncomment the `#-define:ROS2_MODULE_LIDAR` line (remove the '#' character at the start of the line)
  * Create a Blank GameObject
  * Add the LidarVisualizer.cs script and select which version of ROS you are using
    * There may be more configuration options to set, depending on your setup
  * Click the play button in the editor.

If configured properly, lidar data should be getting fed in from ROS and displayed in the scene.


# Contributions
Contributions are welcome. Refer to the [Contribution Process](CONTRIBUTING.md) for more details.


# Troubleshooting
During development, Unity's cache can become corrupted. If you encounter build or runtime errors in Unity, try these.

  * Shutdown Unity
  * Delete the Plugins folder
  * Recopy plugins

[^1]: ROS is a trademark of Open Robotics.
