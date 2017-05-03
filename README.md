# Yocto-Visualisation

This C#.Net application will allow you to visualize data from any [Yoctopuce](https://www.yoctopuce.com) sensor.
Both USB and networked sensors are supported. Sensors datalogger are supported
as well. The UI is based on a widgets concept. Each widget aspect is fully 
configurable by the user. Available widgets are:
- Charts
- Digital display
- Solid gauge
- Angular gauge

![Screenshot example](https://www.yoctopuce.com/pubarchive/2017-04/widgets_1.png)

You will find more information about this application on [Yoctopuce website](https://www.yoctopuce.com).
If you are not much into programming and are only interested in installing Yocto-visualisation,  here is the [windows installer](https://www.yoctopuce.com/FR/downloads/YoctoVisualization.zip) .   


## Source code installation
Unzip the files wherever you want and open the project with at least VisualStudio 
2015.  The *Livecharts* library  used by the project should install automatically.
It might work with previous versions of VisualStudio, but not without pain.
 

## What you need to know
- The application is based on the YSensor generic class. Any past, present and future Yoctopuce sensor compatible with this class will work with this application.
- The editing principle is [C# reflection](https://msdn.microsoft.com/en-us/library/mt656691.aspx) associated with a *propertyGrid* component 
- The most interesting file is *properties.cs*, basically it's the list of all customizable properties in the widgets. This is the place for adding/removing 
  customizable properties. Default values a defined there as well.
 

## IMPORTANT
This work is based on the [LiveCharts](https://lvcharts.net) library, specifically
*LiveCharts* and  *LiveCharts.Geared*. *LiveCharts* is free, but
*LiveCharts.Geared* is not. If you open this project with VisualStudio, it will start your *LiveCharts.Geared*
trial period. After a while, you might have to buy a licence, which is reasonably
cheap by the way. More info here: https://lvcharts.net/licensing/pricing
   


