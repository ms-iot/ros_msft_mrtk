# Mixed Reality Toolkit for ROS2 Overview
> The MRTK for ROS is being developed in the open: Feature requests are very much welcome at the [issues page](https://github.com/ms-iot/ros_msft_mrtk/issues).

The Robot Operating system - also called ROS [^1] - is an open source middleware for creating Robots. This repository implements glue which connects your Mixed Reality Robotics application with Robots and infrastructure running using ROS2. It works with [ROS2.net](https://github.com/ros2-dotnet/ros2_dotnet) within the Unity Game engine.

The Mixed Reality Toolkit is an Open Source framework for building Virtual Reality and Augmented Reality applications, which target numerous platforms - including Hololens.

The MRTK for ROS Unity Extension is being developed by the ROS2.net maintainers, the Microsoft Edge Robotics team and the Mixed Reality Robotics team.

## Getting started with ROS2 on Hololens.
This depends on [ROS2 Foxy UPM supplied by Microsoft](http://aka.ms/ros/mrtk_native). Windows is required for Hololens and Windows Mixed Reality development. MRTK for ROS development will not be supported on other platforms.

### Prerequsitites
* [Unity 2020.3 or later](https://unity.com/)
* [Microsoft Mixed Reality Toolkit](https://docs.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/?view=mrtkunity-2021-05)
* [Nuget for Unity 3.02 or later](https://github.com/GlitchEnzo/NuGetForUnity/releases)
* [Microsoft Mixed Reality QR Nuget](https://nuget.org/Packages/Microsoft.MixedReality.QR)

### Create your application

* Create a 3D Unity application, and set up for Windows Mixed Reality using the instructions provided by the [Microsoft Mixed Reality Toolkit](https://docs.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/?view=mrtkunity-2021-05) documentation.
* Add Nuget for unity to your application by double clicking the the `NuGetForUnity.3.0.2.unitypackage`. Restart Unity for this to take effect.
* In the Menu for Unity, select `NuGet`, then `Manage Packages`. 
* In the Search edit box, enter `Microsoft.MixedReality.QR`, then click search. Install 0.5.3 or later.
* In the `Window` Menu, select `Package Manager`. 
* Select `+`, then `Add Package from Git Url`. enter `http://aka.ms/ros/mrtk_native`
* Select `+`, then `Add Package from Git Url`. enter `http://aka.ms/ros/mrtk`

You can now add scenes from the ROS MRTK extension or use individual components in your application.

# Contributions
Contributions are welcome. Refer to the [Contribution Process](CONTRIBUTING.md) for more details.


[^1]: ROS is a trademark of Open Robotics.
