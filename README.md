# Yocto-Visualization V2

Yocto-Visualization is a C# .Net application to visualize data from any [Yoctopuce](https://www.yoctopuce.com) sensor. This application runs on Windows, Linux and macOS. Both USB and networked sensors are supported. Data can be retrieved from the sensor's datalogger as well. The UI is based on a widgets concept. Each widget aspect is fully configurable by the user. Available widgets are:

- Charts
- Digital display
- Solid gauge
- Angular gauge

![Screenshot example](http://www.yoctopuce.com/pubarchive/2018-06/YoctoVisualizationV2-Renderer_1.png)

You will find more information about this application on [Yoctopuce website](https://www.yoctopuce.com/EN/article/version-2-for-the-yocto-visualization). If you are not much into programming and are only interested in installing Yocto-Visualization V2, here is a page linking to [Windows, Linux and macOS binaries](https://www.yoctopuce.com/EN/tools.php) .

## Customizing widgets shown at startup
You do not even need to recompile the software if your intent is simply to pre-configure the widgets shown at startup: the widgets configuration is saved in a xml configuration file, and you can force to use a given configuration file using the command-line option *-config*

## Source code installation
Extract the project files wherever you want.

### On Windows 7+
Open the *.csprog* project with at least Visual-Studio 2015, that's it. Adaptation for  previous version of Visual-Studio should be possible at the cost of some minor rework.  

### On Windows XP
This project normaly requires .Net 4.5 which can't be installed on Windows XP, but there is a workaround: just open the project with at least visual studio 2015, search for *Application/Target Framework* parameter in the *Project properties*, set it to *.Net framework 3.5* and recompile, the resulting executable will run on XP systems as long as .Net Framework 3.5 is installed but some optimizations will be lost on the way.

### On Linux
Make sure that Mono is installed (min version 4) as well as Mono-Develop (min version 5) and open the  *.csprog* project with Mono-Develop. Avoid the flatpak based Mono-Develop version as it is sand-boxed and can't access to the libusb. More info on [this page](https://www.yoctopuce.com/EN/article/yocto-visualization-v2-on-linux)

### On macOS 
Install Mono for macOS (Visual Studio channel) and Visual Studio for macOS and open the *.csprog* project with Visual Studio. More info on [this page](https://www.yoctopuce.com/EN/article/yocto-visualization-v2-on-macos)

## What you need to know
- The application is based on the Yoctopuce YSensor generic class. Any past, present and future Yoctopuce sensor compatible with this class will work with this application.
- The editing principle is [C# reflection](https://msdn.microsoft.com/en-us/library/mt656691.aspx) associated with a *propertyGrid* component 
- The most interesting file to change is *properties.cs*. It contains the list of all customizable properties in the widgets. This is the place for adding / removing / customize properties. Default values are defined there as well.

## Note
Unlike the previous version, Yocto-Visualization V2 uses its own rendering code and does not rely on any third-party library.
