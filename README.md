# Mixed Reality Toolkit for ROS2 Overview
> The MRTK for ROS is being developed in the open: Feature requests are very much welcome at the [issues page](https://github.com/ms-iot/ros_msft_mrtk/issues).

The Robot Operating system - also called ROS [^1] - is an open source middleware for creating Robots. This repository implements glue which connects your Mixed Reality Robotics application with Robots and infrastructure using ROS2. ROS2 runs directly on the Hololens without going through ROS Bridge - reducing latency and improving network efficiency. 
The ROS2 MRTK extension works with [ROS2.net](https://github.com/ros2-dotnet/ros2_dotnet)[^2] within the Unity Game engine, allowing you to write ROS2-like code directly in Unity.

The Mixed Reality Toolkit is an Open Source framework for building Virtual Reality and Augmented Reality applications, which target numerous platforms - including Hololens.

## Getting started with ROS2 on Hololens 2.

This depends on [ROS2 Foxy UPM supplied by Microsoft](http://aka.ms/ros/mrtk_native). Windows is required for Hololens and Windows Mixed Reality development. MRTK for ROS2 development will not be supported on other platforms.

### Prerequsitites
* [Unity 2020.3 or later](https://unity.com/)
* [Microsoft Mixed Reality Toolkit](https://docs.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/?view=mrtkunity-2021-05)
* [Nuget for Unity 3.02 or later](https://github.com/GlitchEnzo/NuGetForUnity/releases)
* [Microsoft Mixed Reality QR Nuget](https://nuget.org/Packages/Microsoft.MixedReality.QR)
* Download the Unity release for the ROS2 native from [http://aka.ms/ros/mrtk_native](http://aka.ms/ros/mrtk_native)
* Download the Unity release for the Mixed Reality Toolit for ROS2 from [http://aka.ms/ros/mrtk](http://aka.ms/ros/mrtk)

### Create your application

* Create a 3D Unity application
* Use the [Microsoft Mixed Reality Feature Tool](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool), and select the following features:
  * Azure Mixed Reality Services
    * Azure Spatial Anchors SDK Core
    * Azure Spatial Anchors SDK for iOS
    * Azure Spatial Anchors SDK for Windows
  * Mixed Reality Toolkit
    * Mixed Reality Toolkit Extensions
    * Mixed Reality Toolkit Foundation
    * Mixed Reality Standard Assets
    * Mixed Reality Toolkit Tools
  * Platform Support
    * Mixed Reality OpenXR Plugin
  * Spatial Audio
    * Microsoft Spatializer
  * World Locking Tools
    * WLT Core
* Wait for Unity to deploy these components.
  * If asked about the `new Input System`, select `Yes`. 
* When Unity restarts, you should now see a window titled `MRTK Project Configurator`
  * Select `Unity OpenXR Plugin`
  * Select `Show XR Plug-in Management`
    * In the new window, with the monitor icon on the tab, select `OpenXR`.
    * Switch to the Microsoft icon, ans select `OpenXR`
    * Close the Unity Configuration Window
  * Return to the `MRTK Project Configurator` Window and select `Apply Settings`, then select `Next`
  * Select `Microsoft Spatializer` in the `Audio Spatializer` dropdown, then click `Apply`
* When Unity Restarts (again), Select the `Mixed Reality` Menu item, `Toolkit`, `Add to Scene and Configure`
* Add Nuget for unity to your application by double clicking the the `NuGetForUnity.3.0.2.unitypackage`. Import the assets into your project. 
* In the Menu for Unity, select `NuGet`, then `Manage Packages`. 
* In the Search edit box, enter `Microsoft.MixedReality.QR`, then click search. Install 0.5.3 or later.
* In the `Window` Menu, select `Package Manager`. 
* Select `+`, then `Add Package from tarball`. select the Unity release for ROS2 Native downloaded above.
* Select `+`, then `Add Package from tarball`. select the Unity release for the Mixed Reality Toolkit for ROS2 downloaded above.
* In the `Project` tree view in Unity, expand `ROS2 and ROS2.net Native components for Unity` and locate the `link.xml` file. Drag and copy this file into your asset folder. 
  * This is needed to prevent optimizations which break ROS2.net Messages.

* Configure your project for Hololens:
  * Select `Edit` then `Project Settings`
  * Select `Player`. Select the Windows icon.
  * Under Capabilities, ensure that the following are checked:
    * InternetClient
    * InternetClientServer
    * PrivateNetworkClientServer
    * WebCam
    * Microphone
    * SpatialPerception
  * Select `File` Then `Build Settings`
    * Select `Add Open Scenes` to add your current scenes to the project.
    * Select `Universal Windows Platform` and `Switch Plaform` if needed.
    * Select `Build`
    * In the save dialog, right click on the background and select `New Folder`, name it `App`, ensure `App` is selected, then click `Select Folder`
  * Open the resulting Visual Studio Solution in the `App` folder
  * Configure Visual Studio
    * Select the `Release` build type. (This is important, as the ROS2 binaries are only delivered as release)
    * Select `ARM64`.
    * Build and Deploy



You can now add scenes from the ROS MRTK extension or use individual components in your application.

### Sample Scene to bootstrap your application
One you have configured the application above, you can now add components to your scene to define your application. A Sample scene is provided which demonstrates spatial pinning and Lidar visualization.

* In the Unity Package Manager Window, select `ROS2 and ROS2.net Native components for Unity`.
* Under the Samples dropdown, select `Basic ROS World` and select Import.
* Drag this scene to the Unity Hierarchy Window.
* Remove any existing scenes.


# Contributions
Contributions are welcome. Refer to the [Contribution Process](CONTRIBUTING.md) for more details.

[^1]: ROS is a trademark of Open Robotics.
[^2]: ROS2.net is maintained by the ROS2.net maintainers and community.
