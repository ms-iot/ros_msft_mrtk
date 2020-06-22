> The MRTK for ROS package is currently in preview: Feature requests and bug reports are very much welcome at the [issues page](https://github.com/ms-iot/ros_msft_mrtk/issues).
>
> We want to hear what features you want in order to accomplish mixed reality Robotics scenarios!
>
> During the preview, this repository will be available only as sources. As it exits preview, a Nuget package will be provided.

# Mixed Reality Toolkit for ROS Overview
The Robot Operating system - also called ROS [^1] - is an open source middleware for creating Robots. This repository implements glue which connects your Mixed Reality Robotics application with Robots and infrastructure running ROS2. It works with [ROS2.net](https://github.com/ros2-dotnet/ros2_dotnet)[^2] within the Unity Game Engine.

The Mixed Reality Toolkit is an Open Source framework for building Virtual Reality and Augmented Reality applications, which target numerous platforms - including Hololens.

The MRTK for ROS Unity Extension is being developed by the ROS2.net maintainers, the Microsoft Edge Robotics team and the Mixed Reality Robotics team.


# Getting started
This project depends on your installation of ROS2 eloquent and ROS2.net for eloquent for Windows.

> NOTE: Because of the dependency chain, Windows is required. MRTK for ROS development will not be supported on other platforms.
> However, once deployed the MRTK ROS application will support visualizing robots running ROS2 on other platforms.

## Installing ROS2

Please follow the [Microsoft ROS 2 Getting Started instructions](http://aka.ms/ros/setup_ros2). 

> If you are targetting Hololens 2, please add the ARM64 workloads. NOTE: Due to cross compile and toolchain support, Hololens 2 support in progress and available later in 2020.

To help bootstrap your application, we also recommend setting up a [Nav2 simulation environment](https://ms-iot.github.io/ROSOnWindows/ros2/nav2.html), which can be used in VR mode.

## Before building your MRTK ROS application
MRTK ROS applications require binaries from ROS2 to be built on your computer.
Please follow the instructions for [ROS2 on UWP](https://github.com/theseankelly/ros2_uwp).

## Bootstrapping your Application

  * Clone this repository to your computer.
  * Install [Unity 2019](https://unity.com/).
  * Open the Unity Hub
  * Select add a project
  * Open the SampleProject from this repository.
  * From the ROS command prompt, change the directory to where you cloned the MRTK extension, and run `copy_assets.cmd <path to the ros2_uwp workspace>`


# Contributions
Contributions are welcome. Refer to the [Contribution Process](CONTRIBUTING.md) for more details.


[^1]: ROS is a trademark of Open Robotics.
[^2]: If you are interesting in using ROS1, you can leverage this repository with the [ROS1/ROS2 bridge](https://github.com/ros2/ros1_bridge), or look at [ros-sharp](https://github.com/siemens/ros-sharp). However, please understand this is not directly supported.
