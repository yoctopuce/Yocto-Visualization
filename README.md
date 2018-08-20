# Yocto-Visualization V2

This C#.Net application will allow you to visualize data from any [Yoctopuce](https://www.yoctopuce.com) sensor. This application runs on Windows as well as Linux. Both USB and networked sensors are supported. Sensors datalogger are supported too. The UI is based on a widgets concept. Each widget aspect is fully configurable by the user. Available widgets are:
- Charts
- Digital display
- Solid gauge
- Angular gauge

![Screenshot example](http://www.yoctopuce.com/pubarchive/2018-06/YoctoVisualizationV2-Renderer_1.png)

You will find more information about this application on [Yoctopuce website](https://www.yoctopuce.com/EN/article/version-2-for-the-yocto-visualization). If you are not much into programming and are only interested in installing Yocto-Visualization V2,  here is a page linking to both [Windows and Linux installers](https://www.yoctopuce.com/EN/tools.php) .   


## Source code installation
Unzip the project files wherever you want.
### On Windows
Open the *.csprog* project with at least Visual-Studio 2015, that's it.  This project requires .NET 4, adaptation for  previous version of .Net and/or Visual-Studio should be possible at the cost of some minor rework. 
#### On Linux
Make sure that Mono is installed (min version 4) as well as Mono-Develop (min version 5) and open the  *.csprog* project with Mono-Develop. Avoid the flatpak based Mono-Develop version as it is sand-boxed and can't access to the libusb. More info on [this page](https://www.yoctopuce.com/EN/article/yocto-visualization-v2-on-linux)

## What you need to know
- The application is based on the Yoctopuce YSensor generic class. Any past, present and future Yoctopuce sensor compatible with this class will work with this application.
- The editing principle is [C# reflection](https://msdn.microsoft.com/en-us/library/mt656691.aspx) associated with a *propertyGrid* component 
- The most interesting file is *properties.cs*, basically it's the list of all customizable properties in the widgets. This is the place for adding / removing / customize properties. Default values are defined there as well.
 

## Note
Unlike the previous version, Yocto-Visualization V2 uses its own rendering code and does not rely on any third-party library.




