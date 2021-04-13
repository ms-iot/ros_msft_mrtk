# Mixed Reality Toolkit for ROS Overview
> The MRTK for ROS is being developed in the open: Feature requests are very much welcome at the [issues page](https://github.com/ms-iot/ros_msft_mrtk/issues).

The Robot Operating system - also called ROS [^1] - is an open source middleware for creating Robots. This repository implements glue which connects your Mixed Reality Robotics application with Robots and infrastructure running using ROS2. It works with [ROS2.net](https://github.com/ros2-dotnet/ros2_dotnet) within the Unity Game Engine.

The Mixed Reality Toolkit is an Open Source framework for building Virtual Reality and Augmented Reality applications, which target numerous platforms - including Hololens.

The MRTK for ROS Unity Extension is being developed by the ROS2.net maintainers, the Microsoft Edge Robotics team and the Mixed Reality Robotics team.

# Getting started with ROS2
This project depends on your installation of ROS2 Foxy and [ROS2.net](https://github.com/ros2-dotnet/ros2_dotnet) for Foxy for Windows. Because of the dependency chain, Windows is required. MRTK for ROS development will not be supported on other platforms.

## Installing ROS2

Please follow the [Microsoft ROS 2 Getting Started instructions](http://aka.ms/ros/setup_ros2).

> If you are targetting Hololens 2, please add the ARM64 workloads.

To help bootstrap your application, we also recommend setting up a [Nav2 simulation environment](https://ms-iot.github.io/ROSOnWindows/ros2/nav2.html), which can be used in VR mode.

## Running in the Unity editor
***coming soon***

# Bootstrapping your Mixed Reality Toolkit Application

  * Clone this repository to your computer.
  * Open the Unity Hub
  * Select add a project
  * Open the SampleProject from this repository.
  * Ensure that your Unity project is using the `.NET 4.x` Api Compatibility Level (Edit > Project Settings > Player > Other Settings > Api Compatibility Level)
  * ### For manipulating ROS2 robots within Unity:

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
