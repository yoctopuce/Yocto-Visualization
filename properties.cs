/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  all widgets customizable properties are described here 
 *  feel free to remove/modify/add more.
 * 
 * 
 *  Prefix describe the widget
 *  Name must match the widget properties
 *  example:
 *   GaugeFormProperties contains
 *     SolidGauge_Uses360Mode which matches the  SolidGauge's Uses360Mode properties
 *    
 *  Some properties might have an  index
 *  example
 *    for graphs
 *    Graphs_series0  is the first series
 *    ....
 *    Graphs_series3  is the fourth series
 *    if you need a fifth series, just create a Graphs_series4 properties
 *    
 *  
 *   - - - - - - - - - License information: - - - - - - - - -
 *
 *  Copyright (C) 2017 and beyond by Yoctopuce Sarl, Switzerland.
 *
 *  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
 *  non-exclusive license to use, modify, copy and integrate this
 *  file into your software for the sole purpose of interfacing
 *  with Yoctopuce products.
 *
 *  You may reproduce and distribute copies of this file in
 *  source or object form, as long as the sole purpose of this
 *  code is to interface with Yoctopuce products. You must retain
 *  this notice in the distributed source file.
 *
 *  You should refer to Yoctopuce General Terms and Conditions
 *  for additional information regarding your rights and
 *  obligations.
 *
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT
 *  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
 *  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
 *  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
 *  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
 *  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 *  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR
 *  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT
 *  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
 *  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
 *  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
 *  WARRANTY, OR OTHERWISE.
 */


using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using YDataRendering;
using YColors;
using System.Drawing.Design;

namespace YoctoVisualisation
{

  //****************************
  //  Solid gauge
  //****************************

  public static class GenericHints
  {
    public const string ColorMsg = " You can use the color chooser dropdown or type direcly the color as HSL:xxx or HSL:xxx where xxx is a 8 digit hex number (Alpha transparence is supported). You can also type a color litteral name (Red, Yellow etc...).";
    public const string BoolMsg = " Value can be toggled with the dropdown menu, or a double click.";
    public const string DevConfAffected = " Changing this value will affect the device configuration.";


    
  }

  public class GaugeFormProperties : GenericProperties
  {
    public GaugeFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { PropagateDataSourceChange(); }

    private void PropagateDataSourceChange()
    {
      ((gaugeForm)ownerForm).SourceChanged(_DataSource_source);
      foreach (var p in this.GetType().GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("DataSource_AlarmSection"))
        {
          // setBrowsableProperty(p.Name, IsDataSourceAssigned()); does not work :-(
          ((AlarmSection)(p.GetValue(this, null))).setDataSource(_DataSource_source);
        }
      }
    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [TypeConverter(typeof(SensorConverter)),
      DisplayName("Sensor"),
      CategoryAttribute("Data source"),
      DescriptionAttribute("Yoctopuce sensor feeding the gauge.")]


    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        PropagateDataSourceChange();


      }
    }
    [DisplayName("Sensor freq"),
    TypeConverter(typeof(FrequencyConverter)),
       CategoryAttribute("Data source"),
    NotSavedInXMLAttribute(true),
    DescriptionAttribute("Sensor data acquisition frequency."+ GenericHints.DevConfAffected)]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }


    private string _DataSource_precision = "0.1";
    [TypeConverter(typeof(FormatterPrecisionConverter)),
      DisplayName("Precision"),
     CategoryAttribute("Data source"),
     DescriptionAttribute("How many digits shown after the decimal point")]
    public string DataSource_precision
    {
      get { return _DataSource_precision; }
      set { _DataSource_precision = value; }
    }

    private AlarmSection _DataSource_AlarmSection0 = new AlarmSection(0);
    [TypeConverter(typeof(AlarmSectionParamConverter)),
     DisplayName("Sensor value alarm 1"),
     NotSavedInXMLAttribute(true),
     ReadOnlyAttribute(true),
     CategoryAttribute("Data source"),

     DescriptionAttribute("Alarm 1 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection0
    {
      get { return _DataSource_AlarmSection0; }
      set { _DataSource_AlarmSection0 = value; }
    }



    private AlarmSection _DataSource_AlarmSection1 = new AlarmSection(1);
    [TypeConverter(typeof(AlarmSectionParamConverter)),
     DisplayName("Sensor value alarm 2"),
     CategoryAttribute("Data source"),
     ReadOnlyAttribute(true),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm 2 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection1
    {
      get { return _DataSource_AlarmSection1; }
      set { _DataSource_AlarmSection1 = value; }
    }




    private double _SolidGauge_min = 0;
    [DisplayName("Minimium value"),
     CategoryAttribute("Values range"),
     DescriptionAttribute("Minimum value displayable by the gauge.")]
    public double SolidGauge_min
    {
      get { return _SolidGauge_min; }
      set { _SolidGauge_min = value; }
    }

    private double _SolidGauge_max = 100;
    [DisplayName("Maximum value"),
     CategoryAttribute("Values range"),
     DescriptionAttribute("Maximum value displayable by the gauge.")]
    public double SolidGauge_max
    {
      get { return _SolidGauge_max; }
      set { _SolidGauge_max = value; }
    }

    private bool _SolidGauge_showMinMax = true;
    [DisplayName("show  min/max"),
     CategoryAttribute("Values range"),
     TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Show the min / max values."+ GenericHints.BoolMsg)]
    public bool SolidGauge_showMinMax
    {
      get { return _SolidGauge_showMinMax; }
      set { _SolidGauge_showMinMax = value; }
    }


    private YColor _SolidGauge_color1 = YColor.fromColor(Color.LightGreen);
    [DisplayName("Min Color"),
     CategoryAttribute("Values range"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Color for minimum value."+ GenericHints.ColorMsg)]
    public YColor SolidGauge_color1
    {
      get { return _SolidGauge_color1; }
      set { _SolidGauge_color1 = value; }
    }

    private YColor _SolidGauge_color2 = YColor.fromColor( Color.Red);
    [DisplayName("Max color"),
     CategoryAttribute("Values range"),
     Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
      DescriptionAttribute("Color for maximum value." + GenericHints.ColorMsg)]
    public YColor SolidGauge_color2
    {
      get { return _SolidGauge_color2; }
      set { _SolidGauge_color2 = value; }
    }


    private FontDescription _SolidGauge_font = new FontDescription("Arial", 20, YColor.fromColor(Color.Black), false, true);
    [DisplayName("Unit  Font"),
     CategoryAttribute("Fonts"),
      ReadOnlyAttribute(true),
     TypeConverter(typeof(YFontDescriptionConverter)),
     DescriptionAttribute("Font for displaying the value/status indicator")]
    public FontDescription SolidGauge_font
    {
      get { return _SolidGauge_font; }
      set { _SolidGauge_font = value; }
    }

    private FontDescription _SolidGauge_minMaxFont = new FontDescription("Arial", 10, YColor.fromColor(Color.Black), false, true);
    [DisplayName("Min Max  Font"),
     CategoryAttribute("Fonts"),
      ReadOnlyAttribute(true),
     TypeConverter(typeof(YFontDescriptionConverter)),
     DescriptionAttribute("Font for displaying min/max values")]
    public FontDescription SolidGauge_minMaxFont
    {
      get { return _SolidGauge_minMaxFont; }
      set { _SolidGauge_minMaxFont = value; }
    }



    // ***************************************************************


    private YSolidGauge.DisplayMode _SolidGauge_displayMode = YSolidGauge.DisplayMode.DISPLAY90;
    [DisplayName("Display mode"),
    CategoryAttribute("Dial"),
     TypeConverter(typeof(SmartEnumConverter)),
    DescriptionAttribute("Dial general shape")]
    public YSolidGauge.DisplayMode SolidGauge_displayMode
    {
      get { return _SolidGauge_displayMode; }
      set { _SolidGauge_displayMode = value; }
    }





    private YColor _SolidGauge_borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color"),
     CategoryAttribute("Dial"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Dial border color." + GenericHints.ColorMsg)]
    public YColor SolidGauge_borderColor
    {
      get { return _SolidGauge_borderColor; }
      set { _SolidGauge_borderColor = value; }
    }

    private double _SolidGauge_borderThickness = 2;
    [DisplayName("Border thickness "),
     CategoryAttribute("Dial"),
     DescriptionAttribute("Thickness of the dial border")]
    public double SolidGauge_borderThickness
    {
      get { return _SolidGauge_borderThickness; }
      set { _SolidGauge_borderThickness = value; }
    }


    private YColor _SolidGauge_backgroundColor1 = YColor.FromArgb(255, 240, 240, 240);
    [DisplayName("Background color 1"),
     CategoryAttribute("Dial"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Dial background gradiant color 1." + GenericHints.ColorMsg)]
    public YColor SolidGauge_backgroundColor1
    {
      get { return _SolidGauge_backgroundColor1; }
      set { _SolidGauge_backgroundColor1 = value; }
    }

    private YColor _SolidGauge_backgroundColor2 = YColor.FromArgb(255, 200, 200, 200);
    [DisplayName("Background color 2"),
     CategoryAttribute("Dial"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Dial background gradiant color 2." + GenericHints.ColorMsg)]
    public YColor SolidGauge_backgroundColor2
    {
      get { return _SolidGauge_backgroundColor2; }
      set { _SolidGauge_backgroundColor2 = value; }
    }


    private double _SolidGauge_thickness = 25;
    [DisplayName("Dial thickness (%) "),
     CategoryAttribute("Dial"),
     DescriptionAttribute("Thickness of the dial, in percentage relative to radius")]
    public double SolidGauge_thickness
    {
      get { return _SolidGauge_thickness; }
      set { _SolidGauge_thickness = value; }
    }

    private double _SolidGauge_maxSpeed = 1;
    [DisplayName("Max speed (%) "),
     CategoryAttribute("Dial"),
     DescriptionAttribute("Maximum speed of the dial in percentage relative to Max-Min. This is meant to limit \"teleporting\" effects.")]
    public double SolidGauge_maxSpeed
    {
      get { return _SolidGauge_maxSpeed; }
      set { _SolidGauge_maxSpeed = value; }
    }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }

 



  }

  //****************************
  //  angular gauge
  //****************************

  public class AngularGaugeFormProperties : GenericProperties
  {
    public AngularGaugeFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { PropagateDataSourceChange();
    }

    public  void PropagateDataSourceChange()
    {
      ((angularGaugeForm)ownerForm).SourceChanged(_DataSource_source);
      foreach (var p in this.GetType().GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("DataSource_AlarmSection"))
        {
          // setBrowsableProperty(p.Name, IsDataSourceAssigned()); does not work :-(
          ((AlarmSection)(p.GetValue(this, null))).setDataSource(_DataSource_source);
        }
      }
    }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [TypeConverter(typeof(SensorConverter)),
      DisplayName("Sensor"),
     CategoryAttribute("Data source"),
     DescriptionAttribute("Yoctopuce sensor feeding the gauge.")]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        PropagateDataSourceChange();

      }
    }

    [DisplayName("Sensor freq"),
    TypeConverter(typeof(FrequencyConverter)),
    CategoryAttribute("Data source"),
    NotSavedInXMLAttribute(true),
    DescriptionAttribute("Sensor data acquisition frequency." + GenericHints.DevConfAffected)]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }


    private AlarmSection _DataSource_AlarmSection0 = new AlarmSection(0);
    [TypeConverter(typeof(AlarmSectionParamConverter)),
     DisplayName("Sensor value alarm 1"),
     NotSavedInXMLAttribute(true),
      ReadOnlyAttribute(true),
     CategoryAttribute("Data source"),

     DescriptionAttribute("Alarm 1 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection0
    {
      get { return _DataSource_AlarmSection0; }
      set { _DataSource_AlarmSection0 = value; }
    }



    private AlarmSection _DataSource_AlarmSection1 = new AlarmSection(1);
    [TypeConverter(typeof(AlarmSectionParamConverter)),
     DisplayName("Sensor value alarm 2"),
     CategoryAttribute("Data source"),
     NotSavedInXMLAttribute(true),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Alarm 2 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection1
    {
      get { return _DataSource_AlarmSection1; }
      set { _DataSource_AlarmSection1 = value; }
    }


    private double _AngularGauge_min = 0;
    [DisplayName("Minimium value"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Minimum value displayable by the gauge.")]
    public double AngularGauge_min
    {
      get { return _AngularGauge_min; }
      set { _AngularGauge_min = value; }
    }

    private double _AngularGauge_max = 100;
    [DisplayName("Maximum value"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Maximum value displayable by the gauge.")]
    public double AngularGauge_max
    {
      get { return _AngularGauge_max; }
      set { _AngularGauge_max = value; }
    }


    private double _AngularGauge_unitFactor = 1;
    [DisplayName("Unit factor"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Data  will be divided by this value before being displayed, this allows simpler graduation marks.")]
     public double AngularGauge_unitFactor
    {
        get { return _AngularGauge_unitFactor; }
        set { _AngularGauge_unitFactor = value; }
      }

    private double _AngularGauge_graduation = 10;
    [DisplayName("Main graduation steps"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Difference between two consecutive main graduation marks")]
    public double AngularGauge_graduation
    {
      get { return _AngularGauge_graduation; }
      set { _AngularGauge_graduation = value; }
    }

    private FontDescription _AngularGauge_graduationFont = new FontDescription("Arial", 20, YColor.fromColor(Color.Black), false, true);
    [DisplayName("Main graduation font"),
     CategoryAttribute("Gauge graduations"),
      ReadOnlyAttribute(true),
     TypeConverter(typeof(YFontDescriptionConverter)),
     DescriptionAttribute("Font used for graduation labels")]
    public FontDescription AngularGauge_graduationFont
    {
      get { return _AngularGauge_graduationFont; }
      set { _AngularGauge_graduationFont = value; }
    }

  


    private double _AngularGauge_graduationSize = 10;
    [DisplayName("Main graduation size (%)"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Main graduation marks size in percent, relative to dial radius")]
    public double AngularGauge_graduationSize
    {
      get { return _AngularGauge_graduationSize; }
      set { _AngularGauge_graduationSize = value; }
    }

    private double _AngularGauge_graduationOuterRadius = 98;
    [DisplayName("Main graduation radius (%)"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Main graduation marks outter radius in percent, relative to dial radius")]
    public double AngularGauge_graduationOuterRadius
    {
      get { return _AngularGauge_graduationOuterRadius; }
      set { _AngularGauge_graduationOuterRadius = value; }
    }

  

    private double _AngularGauge_graduationThickness = 2;
    [DisplayName("Main graduation thickness"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Main gradduation marks thickness")]
    public double AngularGauge_graduationThickness
    {
      get { return _AngularGauge_graduationThickness; }
      set { _AngularGauge_graduationThickness = value; }
    }


    private YColor _AngularGauge_graduationColor = YColor.fromColor(Color.Black);
    [DisplayName("Main graduation color"),
     CategoryAttribute("Gauge graduations"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Main gradduation marks color." + GenericHints.ColorMsg)]
    public YColor AngularGauge_graduationColor
    {
      get { return _AngularGauge_graduationColor; }
      set { _AngularGauge_graduationColor = value; }
    }



    private double _AngularGauge_subgraduationCount = 5;
    [DisplayName("Sub-graduation count"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("How many sub-graduation (+1) marks between two consecutive main graduation marks")]
    public double AngularGauge_subgraduationCount
    {
      get { return _AngularGauge_subgraduationCount; }
      set { _AngularGauge_subgraduationCount = value; }
    }

    private double _AngularGauge_subgraduationSize = 5;
    [DisplayName("Sub-graduation size (%)"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Sub-graduation marks size in percent, relative to dial radius")]
    public double AngularGauge_subgraduationSize
    {
      get { return _AngularGauge_subgraduationSize; }
      set { _AngularGauge_subgraduationSize = value; }
    }


    private double _AngularGauge_subgraduationThickness = 1;
    [DisplayName("Sub-graduation thickness"),
     CategoryAttribute("Gauge graduations"),
     DescriptionAttribute("Subgraduation marks thickness")]
    public double AngularGauge_subgraduationThickness
    {
      get { return _AngularGauge_subgraduationThickness; }
      set { _AngularGauge_subgraduationThickness = value; }
    }


    private YColor _AngularGauge_subgraduationColor = YColor.fromColor(Color.Black);
    [DisplayName("Sub-graduation color"),
     CategoryAttribute("Gauge graduations"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Sub-graduation marks color." + GenericHints.ColorMsg)]
    public YColor AngularGauge_subgraduationColor
    {
      get { return _AngularGauge_subgraduationColor; }
      set { _AngularGauge_subgraduationColor = value; }
    }



    private YColor _AngularGauge_needleColor = YColor.fromColor(Color.Red);
    [DisplayName("Needle color"),
     CategoryAttribute("Needle"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Needle filling color." + GenericHints.ColorMsg)]
    public YColor AngularGauge_needleColor
    {
      get { return _AngularGauge_needleColor; }
      set { _AngularGauge_needleColor = value; }
    }

    private YColor _AngularGauge_needleContourColor = YColor.fromColor(Color.DarkRed);
    [DisplayName("Needle contour color"),
     CategoryAttribute("Needle"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Needle contour color." + GenericHints.ColorMsg)]
     public YColor AngularGauge_needleContourColor
     {
       get { return _AngularGauge_needleContourColor; }
       set { _AngularGauge_needleContourColor = value; }
     }

    private double _AngularGauge_needleContourThickness = 1;
    [DisplayName("Neddle contour thickness"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Thickness of the needle contour")]
     public double AngularGauge_needleContourThickness
     {
       get { return _AngularGauge_needleContourThickness; }
       set { _AngularGauge_needleContourThickness = value; }
     }




    private double _AngularGauge_needleLength1 = 90;
    [DisplayName("Neddle main size (%)"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Length of the needle part pointing to graduations, in % relative to radius")]
    public double AngularGauge_needleLength1
    {
      get { return _AngularGauge_needleLength1; }
      set { _AngularGauge_needleLength1 = value; }
    }

    private double _AngularGauge_needleLength2 = 15;
    [DisplayName("Neddle foot size (%)"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Length of the needle part not pointing to graduations, in % relative to radius")]
     public double AngularGauge_needleLength2
      {
        get { return _AngularGauge_needleLength2; }
        set { _AngularGauge_needleLength2 = value; }
     }

    private double _AngularGauge_needleWidth = 5;
    [DisplayName("Needle width (%)"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Width of the needle, in % relative to radius")]
    public double AngularGauge_needleWidth
    {
      get { return _AngularGauge_needleWidth; }
      set { _AngularGauge_needleWidth = value; }
    }

    private double _AngularGauge_needleMaxSpeed = 0.5;
    [DisplayName("Needle max speed (%)"),
     CategoryAttribute("Needle"),
     DescriptionAttribute("Needle Maximum speed, in % relative to (max-min). This is meant to limit \"teleporting\" effects.")]
    public double AngularGauge_needleMaxSpeed
    {
      get { return _AngularGauge_needleMaxSpeed; }
      set { _AngularGauge_needleMaxSpeed = value; }
    }

   


    private FontDescription _AngularGauge_unitFont = new FontDescription("Arial", 24, YColor.fromColor(Color.DarkGray), false, true);
    [DisplayName("Unit Line Font"),
     CategoryAttribute("Text lines"),
      ReadOnlyAttribute(true),
     TypeConverter(typeof(YFontDescriptionConverter)),
     DescriptionAttribute("Font used in the text line discribing unit")]
    public FontDescription AngularGauge_unitFont
    {
      get { return _AngularGauge_unitFont; }
      set { _AngularGauge_unitFont = value; }
    }

   private FontDescription _AngularGauge_statusFont = new FontDescription("Arial", 24, YColor.fromColor(Color.DarkGray), false, true);
    [DisplayName("Status Line Font"),
      ReadOnlyAttribute(true),
     CategoryAttribute("Text lines"),
     TypeConverter(typeof(YFontDescriptionConverter)),
     DescriptionAttribute("Font used in the text line gauge status")]
    public FontDescription AngularGauge_statusFont
    {
      get { return _AngularGauge_statusFont; }
      set { _AngularGauge_statusFont = value; }
    }



    private YColor _AngularGauge_borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color"),
     CategoryAttribute("Dial"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Dial border color." + GenericHints.ColorMsg)]
    public YColor AngularGauge_borderColor
    {
      get { return _AngularGauge_borderColor; }
      set { _AngularGauge_borderColor = value; }
    }

    private double _AngularGauge_borderThickness = 5;
    [DisplayName("Border thickness "),
     CategoryAttribute("Dial"),
     DescriptionAttribute("Thickness of the dial border")]
     public double AngularGauge_borderThickness
     {
       get { return _AngularGauge_borderThickness; }
       set { _AngularGauge_borderThickness = value; }
     }


    private YColor _AngularGauge_backgroundColor1 = YColor.FromArgb(255, 240, 240, 240);
    [DisplayName("Background color 1"),
     CategoryAttribute("Dial"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Dial background gradiant color 1." + GenericHints.ColorMsg)]
     public YColor AngularGauge_backgroundColor1
     {
       get { return _AngularGauge_backgroundColor1; }
       set { _AngularGauge_backgroundColor1 = value; }
    }

    private YColor _AngularGauge_backgroundColor2 = YColor.FromArgb(255, 200, 200, 200);
    [DisplayName("Background color 2"),
     CategoryAttribute("Dial"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Dial background gradiant color 2." + GenericHints.ColorMsg)]
     public YColor AngularGauge_backgroundColor2
     {
       get { return _AngularGauge_backgroundColor2; }
       set { _AngularGauge_backgroundColor2 = value; }
     }


    private AngularZoneDescription _AngularGauge_zones0 = new AngularZoneDescription(0, 50, YColor.fromColor(Color.LightGreen));
    [DisplayName("Zone 1"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    TypeConverter(typeof(YAngularZoneConverter)),
    DescriptionAttribute("Zone 1 parameters")]
    public AngularZoneDescription AngularGauge_zones0
    {
      get { return _AngularGauge_zones0; }
      set { _AngularGauge_zones0 = value; }
    }

    private AngularZoneDescription _AngularGauge_zones1 = new AngularZoneDescription(50, 80, YColor.fromColor(Color.Yellow));
    [DisplayName("Zone 2"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    TypeConverter(typeof(YAngularZoneConverter)),
    DescriptionAttribute("Zone 2 parameters")]
    public AngularZoneDescription AngularGauge_zones1
    {
      get { return _AngularGauge_zones1; }
      set { _AngularGauge_zones1 = value; }
    }

    private AngularZoneDescription _AngularGauge_zones2 = new AngularZoneDescription(80, 100, YColor.fromColor(Color.Red));
    [DisplayName("Zone 3"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    TypeConverter(typeof(YAngularZoneConverter)),
    DescriptionAttribute("Zone 3 parameters")]
    public AngularZoneDescription AngularGauge_zones2
    {
      get { return _AngularGauge_zones2; }
      set { _AngularGauge_zones2 = value; }
    }


  }

  public class ZoneDescription
  {

   
    double _min;
    double _max;
    bool _visible = false;
    YColor _color;

    public ZoneDescription(double min, double max, YColor color)
    {
      _min = min;
      _max = max;
      _color = color;

    }


    [DisplayName("Visible"),
     CategoryAttribute("Zone"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Zone visibility." + GenericHints.BoolMsg)]
    public bool visible { get { return _visible; } set { _visible = value; } }


    [DisplayName("Minimum value"),
     CategoryAttribute("Zone"),
     DescriptionAttribute("Zone minimum value")]
    public double min { get { return _min; } set { _min = value; } }

    [DisplayName("Maximum value"),
     CategoryAttribute("Zone"),
     DescriptionAttribute("Zone maximum value")]
    public double max { get { return _max; } set { _max = value; } }


    [DisplayName("Color"),
    CategoryAttribute("Zone"),
    Editor(typeof(YColorEditor), typeof(UITypeEditor)),
    DescriptionAttribute("Zone color")]
    public YColor color { get { return _color; } set { _color = value; } }
  }

  public class AngularZoneDescription
  {

    double _width = 5;
    double _outerRadius =98;
    double _min;
    double _max;
    bool _visible = true;
    YColor _color;

    public AngularZoneDescription(double min, double max, YColor color)
    {
      _min = min;
      _max = max;
      _color = color;

    }


    [DisplayName("Visible"),
     CategoryAttribute("Zone"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Zone visibility." + GenericHints.BoolMsg)]
     public bool visible { get { return _visible; } set { _visible = value; } }


    [DisplayName("Minimum value"),
     CategoryAttribute("Zone"),
     DescriptionAttribute("Zone minimum value")]
     public double min { get { return _min; } set { _min = value; } }

    [DisplayName("Maximum value"),
     CategoryAttribute("Zone"),
     DescriptionAttribute("Zone maximum value")]
     public double max { get { return _max; } set { _max = value; } }


    [DisplayName("Color"),
    CategoryAttribute("Zone"),
       Editor(typeof(YColorEditor), typeof(UITypeEditor)),
    DescriptionAttribute("Zone color")]
    public YColor color { get { return _color; } set { _color = value; } }

    [DisplayName("Outer radius (%)"),
     CategoryAttribute("Zone"),
     DescriptionAttribute("Zone outer radius, in percentage relative to dial radius ")]
     public double outerRadius { get { return _outerRadius; } set { _outerRadius = value; } }

    [DisplayName("Width (%)"),
     CategoryAttribute("Zone"),
     DescriptionAttribute("Zone  width, in percentage relative to dial radius ")]
     public double width { get { return _width; } set { _width = value; } }
  }


  //****************************
  //  Fonts
  //****************************



  public class FontDescription
  {
    public FontDescription(string name, double size, YColor color, bool italic, bool bold)
    {
      _name = name;
      _size = size;
      _color = color;
      _italic = italic;
      _bold = bold;

    }

    public override string ToString()
    {
      return _name + " " + _size.ToString();
    }

    private string _name;
    [TypeConverter(typeof(FontNameConverter)),
     DisplayName("Font name"),
     CategoryAttribute("Font"),
     DescriptionAttribute("Name of the font")]
    public string name { get { return _name; } set { _name = value; } }

    private double _size;
    [DisplayName("Font size"),
     CategoryAttribute("Font"),
     DescriptionAttribute("Size of the font")]
    public double size { get { return _size; } set { _size = value; } }

    private YColor _color;
    [DisplayName("Font color"),
     CategoryAttribute("Font"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Color of the font." + GenericHints.ColorMsg)]
    public YColor color { get { return _color; } set { _color = value; } }

    private bool _italic;
    [DisplayName("Italic"),
     CategoryAttribute("Font"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Is the font style italic?." + GenericHints.BoolMsg)]
    public bool italic { get { return _italic; } set { _italic = value; } }


    private bool _bold;
    [DisplayName("Bold"),
     CategoryAttribute("Font"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Is the font style bold?." + GenericHints.BoolMsg)]
    public bool bold { get { return _bold; } set { _bold = value; } }


  }

  //****************************
  //  Digital display
  //****************************




  public class digitalDisplayFormProperties : GenericProperties
  {
    public digitalDisplayFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    {
      PropagateDataSourceChange();

    }

    private void PropagateDataSourceChange()
    {
      ((digitalDisplayForm)ownerForm).SourceChanged(_DataSource_source);
      foreach (var p in this.GetType().GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("DataSource_AlarmSection"))
        { 
          // setBrowsableProperty(p.Name, IsDataSourceAssigned()); does not work :-(
          ((AlarmSection)(p.GetValue(this, null))).setDataSource(_DataSource_source);
        }
      }
    }

    public override bool IsDataSourceAssigned()
    {
      return !(_DataSource_source is NullYSensor);
    }

    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [TypeConverter(typeof(SensorConverter)),
      DisplayName("Sensor"),
      CategoryAttribute("Data source"),
      
      DescriptionAttribute("Yoctopuce sensor feeding the display.")]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        PropagateDataSourceChange();

      }
    }

    [DisplayName("Sensor freq"),
     TypeConverter(typeof(FrequencyConverter)),
     CategoryAttribute("Data source"),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Sensor data acquisition frequency." + GenericHints.DevConfAffected)]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }

    private string _DataSource_precision = "0.1";
    [TypeConverter(typeof(FormatterPrecisionConverter)),
      DisplayName("Precision"),
     CategoryAttribute("Data source"),
     DescriptionAttribute("How many digits shown after the decimal point.")]
    public string DataSource_precision
    {
      get { return _DataSource_precision; }
      set { _DataSource_precision = value; }
    }
    
    private AlarmSection _DataSource_AlarmSection0 = new AlarmSection(0);
    [TypeConverter(typeof(AlarmSectionParamConverter)),
     DisplayName("Sensor value alarm 1"),
     NotSavedInXMLAttribute(true),
      ReadOnlyAttribute(true),
     CategoryAttribute("Data source"),
    
     DescriptionAttribute("Alarm 1 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection0
    {
      get { return _DataSource_AlarmSection0; }
      set { _DataSource_AlarmSection0 = value; }
    }

   

    private AlarmSection _DataSource_AlarmSection1 = new AlarmSection(1);
    [TypeConverter(typeof(AlarmSectionParamConverter)),
     DisplayName("Sensor value alarm 2"),
     CategoryAttribute("Data source"),
      ReadOnlyAttribute(true),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm 2 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection1
    {
      get { return _DataSource_AlarmSection1; }
      set { _DataSource_AlarmSection1 = value; }
    }



    private FontDescription _font = new FontDescription("Arial", 48, YColor.fromColor(Color.LightGreen),false,true);

    [DisplayName("Font"),
     CategoryAttribute("Display"),
    TypeConverter(typeof(YFontDescriptionConverter)),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Display font")]
    public FontDescription display_font
    {
      get { return _font; }
      set { _font = value; }
    }

    private YColor _backgroundColor1 = YColor.fromColor(Color.Black);
    [DisplayName("Background color 1"),
     CategoryAttribute("Display"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Display background gradiant color1." + GenericHints.ColorMsg)]
     public YColor display_backgroundColor1
    {
       get { return _backgroundColor1; }
       set { _backgroundColor1 = value; }
     }

    private YColor _backgroundColor2 = YColor.fromColor(Color.Black);
    [DisplayName("Background color 2"),
     CategoryAttribute("Display"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Display background gradiant color 2." + GenericHints.ColorMsg)]
    public YColor display_backgroundColor2
    {
      get { return _backgroundColor2; }
      set { _backgroundColor2 = value; }
    }



    private YDigitalDisplay.HrzAlignment _hrzAlignment = YDigitalDisplay.HrzAlignment.RIGHT;
    [DisplayName("Hrz alignment method"),
    CategoryAttribute("Display"),
    TypeConverter(typeof(SmartEnumConverter)),
     
    DescriptionAttribute("Horizontal alignment method")]
    public YDigitalDisplay.HrzAlignment display_hrzAlignment
    {
      get { return _hrzAlignment; }
      set { _hrzAlignment = value; }
    }

    private Double _hrzAlignmentOfset = 5.0;
    [DisplayName("Hrz alignment offset"),
    CategoryAttribute("Display"),
    DescriptionAttribute("Horizontal alignment offset in percentage. No effect when chosen horizontal alignment is CENTER")]
    public double display_hrzAlignmentOfset
    {
      get { return _hrzAlignmentOfset; }
      set { _hrzAlignmentOfset = value; }
    }


    private doubleNan _outOfRangeMin = new doubleNan(Double.NaN);
    [DisplayName("Minimum value"),
     CategoryAttribute("Range Control"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("Regular range minimum value. if value goes  outside regular  range, color will turn to \"Out of range Color\".Leave blank if you don't want to define such a range. ")]
    public doubleNan display_outOfRangeMin
    {
      get { return _outOfRangeMin; }
      set { _outOfRangeMin = value; }
    }

    private doubleNan _outOfRangeMax = new doubleNan(Double.NaN);
    [DisplayName("Maximum value"),
     CategoryAttribute("Range Control"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("Regular range maximxum value. if value is outside regular range, color will turn to \"Out of range Color\". Leave blank if you don't want to define such a range. ")]
    public doubleNan display_outOfRangeMax
    {
      get { return _outOfRangeMax; }
      set {_outOfRangeMax = value; }
    }


    private YColor _outOfRangeColor = YColor.fromColor(Color.Red);
    [DisplayName("Out of range Color"),
     CategoryAttribute("Range Control"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Digits color when value is out of range." + GenericHints.ColorMsg)]
    public YColor display_outOfRangeColorColor
    {
      get { return _outOfRangeColor; }
      set { _outOfRangeColor = value; }
    }



  }






  /****************************************
   * Alarm section
   */


  public class AlarmSection
  {

    
    CustomYSensor _sensor = sensorsManager.getNullSensor();
    int _index = 0;

    public AlarmSection(int index)
    {
      _index = index;
    }

    [DisplayName("Data source type"),
    TypeConverter(typeof(AlamSourceTypeConverter)),
    NotSavedInXMLAttribute(true),
    DescriptionAttribute("Alarm sensor data source (Average, minimum or maximimum value during last interval)")]
    public int source
    {
      get
      {
        return _sensor.getAlarmSource(_index);
      }
      set
      {
        _sensor.setAlarmSource(_index, value);
      }
    }


    [DisplayName("Test Condition"),
     TypeConverter(typeof(AlamTriggerTypeConverter)),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm trigger condition")]
    public int condition
    {
      get {
        return _sensor.getAlarmCondition(_index); 
      }
      set {
        _sensor.setAlarmCondition(_index,value); }
    }

    public void setDataSource(CustomYSensor sensor)
    {
      _sensor = sensor;
    }

   
    [DisplayName("Test Value"),
      NotSavedInXMLAttribute(true),
     DescriptionAttribute("Value for the Alarm trigger")]
    public double value
    {
      get {
        return _sensor.getAlarmValue(_index);
      }
      set {
        _sensor.setAlarmValue(_index, value); }
    }

   
    [DisplayName("Trigger action"),
      NotSavedInXMLAttribute(true),
     DescriptionAttribute("System command line executed each time the alarm is triggered, you can use the following variables: $SENSORVALUE$, $HWDID$, $NAME$, $CONDITION$, $TRIGGER$, $DATATYPE$, $NOW$. You can check logs to find out if your alarm command line works.")]
    public string commandLine
    {
      get {
         return _sensor.getAlarmCommandline(_index); 
      }
      set { _sensor.setAlarmCommandline(_index, value); }
    }

   
    [DisplayName("Trigger delay"),
      NotSavedInXMLAttribute(true),
     DescriptionAttribute("Minimum delay, in seconds, between two alarms. Think carefully and make sure you won't create alarm storms.")]
    public int delay
    {
      get {
         return _sensor.getAlarmDelay(_index); 
      }
      set { _sensor.setAlarmDelay(_index,value); }
    }
  }

  

  

 
 

  /*********************************
   * SERIES
   */

  public class ChartSerie
  {

    private Form ownerForm = null;
    private int index = -1;

    public ChartSerie(YColor defaultColor)
    {
      _color = defaultColor;
     
    }

    public void Init(Form owner, int serieIndex)
    {
      ownerForm = owner;
      index = serieIndex;
      PropagateDataSourceChange(_DataSource_source);
    }

    private void PropagateDataSourceChange(CustomYSensor value)
    {
      ((GraphForm)ownerForm).SourceChanged(index, value);
      foreach (var p in this.GetType().GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("DataSource_AlarmSection"))
        {
          // setBrowsableProperty(p.Name, IsDataSourceAssigned()); does not work :-(
          ((AlarmSection)(p.GetValue(this, null))).setDataSource(_DataSource_source);
        }
      }
    }



    private CustomYSensor _DataSource_source = sensorsManager.getNullSensor();
    [TypeConverter(typeof(SensorConverter)),
      DisplayName("Sensor"),
      DescriptionAttribute("Yoctopuce sensor feeding the graph")]
    public CustomYSensor DataSource_source
    {
      get { return _DataSource_source; }
      set
      {
        _DataSource_source = value;
        if (ownerForm != null) PropagateDataSourceChange(_DataSource_source);
          

      }
    }


    [DisplayName("Sensor frequency"),
     TypeConverter(typeof(FrequencyConverter)),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Sensor data acquisition frequency." + GenericHints.DevConfAffected)]

    public string DataSource_freq
    {
      get { return _DataSource_source.get_frequency(); }
      set { _DataSource_source.set_frequency(value); }

    }

    private int _DataType = 0;
    [DisplayName("Sensor data"),
     TypeConverter(typeof(AvgMinMaxConverter)),
     DescriptionAttribute("Which data for sensor are displayed on the graph. Min and Max are available only for frequencies <1Hz")]
    public int DataSource_datatype
    {
      get { return _DataType; }
      set
      {
        _DataType = value;
        if (ownerForm != null)
          ((GraphForm)ownerForm).SourceChanged(index, _DataSource_source);
      }
    }

    
    private AlarmSection _DataSource_AlarmSection0 = new AlarmSection(0);
    [TypeConverter(typeof(AlarmSectionParamConverter)),
     DisplayName("Sensor value alarm 1"),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm 1 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection0
    {
      get { return _DataSource_AlarmSection0; }
      set { _DataSource_AlarmSection0 = value; }
    }



    private AlarmSection _DataSource_AlarmSection1 = new AlarmSection(1);
    [TypeConverter(typeof(AlarmSectionParamConverter)),
     DisplayName("Sensor value alarm 2"),   
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Alarm 2 for this data source, expand for more.")]
    public AlarmSection DataSource_AlarmSection1
    {
      get { return _DataSource_AlarmSection1; }
      set { _DataSource_AlarmSection1 = value; }
    }
    
    

    [DisplayName("Sensor recording"),
     TypeConverter(typeof(YesNoConverter)),
     NotSavedInXMLAttribute(true),
     DescriptionAttribute("Enable/ disable sensor data recording in the device onboard datalogger."+  GenericHints.DevConfAffected + GenericHints.BoolMsg)]

    public bool DataSource_recording
    {
      get { return _DataSource_source.get_recording(); }
      set { _DataSource_source.set_recording(value); }

    }

    private double _thickness = 2.0;
    [DisplayName("Thickness"),
     DescriptionAttribute("Line thikhness.")]
    public double thickness
    {
      get { return _thickness; }
      set { _thickness = value; }
    }

    private string _legend = "";
    [DisplayName("Legend"),
     DescriptionAttribute("Short description of the serie.")]
    public string legend
    {
      get { return _legend; }
      set { _legend = value; }
    }

    private YColor _color = YColor.fromColor(Color.Red);

    [DisplayName("Color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Line color." + GenericHints.ColorMsg)]
    public YColor color
    {
      get { return _color; }
      set { _color = value; }
    }

    private int _yAxisIndex = 0;
    [TypeConverter(typeof(YAxisChooserConverter)),

      DisplayName("Y axis"),
      DescriptionAttribute("Choose which Y axis the data with be scaled to.")]
    public int yAxisIndex
    {
      get { return _yAxisIndex; }
      set { _yAxisIndex = value; }
    }

  

  }

  //****************************
  //  Legend panel
  //  
  //****************************

  public class LegendPanelDescription
  {
    private bool _enabled = false;
    [DisplayName("Enabled"),
     TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Should the legend panel be shown or not"+ GenericHints.BoolMsg)]
    public bool enabled { get { return _enabled; } set { _enabled = value; } }

    private bool _overlap = false;
    [DisplayName("Overlap"),
     TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Can the panel ovelap the graph data, or should we explicitely make space for it?" + GenericHints.BoolMsg)]
    public bool overlap { get { return _overlap; } set { _overlap = value; } }

    private LegendPanel.Position _position = LegendPanel.Position.RIGHT;
    [DisplayName("Position"),
       TypeConverter(typeof(SmartEnumConverter)),
     DescriptionAttribute("Position of the legend panel")]
     public LegendPanel.Position position { get { return _position; } set { _position = value; } }

    private FontDescription _font = new FontDescription("Arial", 7, YColor.FromArgb(255, 32, 32, 32), false, false);
    [DisplayName("Font"),
     TypeConverter(typeof(YFontDescriptionConverter)),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Legend panel contents fonts")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }



    private YColor _bgColor = YColor.FromArgb(200, 255, 255, 255);
    [DisplayName("Background color "),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
    DescriptionAttribute("Legend panel backgound color." + GenericHints.ColorMsg)]
    public YColor bgColor { get { return _bgColor; } set { _bgColor = value;  } }

    private YColor _borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color "),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
    DescriptionAttribute("Legend panel border color." + GenericHints.ColorMsg)]
    public YColor borderColor { get { return _borderColor; } set { _borderColor = value; } }

    private double _borderthickness = 1.0;
    [DisplayName("Border thickness "),
     DescriptionAttribute("Legend panel border thickness")]
    public double borderthickness { get { return _borderthickness; } set { _borderthickness = value; } }

    private double _padding = 10;
    [DisplayName("Padding "),
     DescriptionAttribute("Distance between the panel border and the panel contents")]
    public double padding { get { return _padding; } set { _padding = value; } }

    private double _verticalMargin = 10;
    [DisplayName("Vertical margin "),
     DescriptionAttribute("Vertical distance between the panel border and its surroundings")]
    public double verticalMargin { get { return _verticalMargin; } set { _verticalMargin = value; } }

    private double _horizontalMargin = 10;
    [DisplayName("Horizontal margin "),
    DescriptionAttribute("Dispance distance between the panel border and its surroundings")]
    public double horizontalMargin { get { return _horizontalMargin; } set { _horizontalMargin = value; } }

 

  }



  public class DataTrackerDescription
  {
   

   

    private bool _enabled = false;
    [DisplayName("Enabled"),
     TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Should the data tracker be shown or not." + GenericHints.BoolMsg)]
    public bool enabled { get { return _enabled; } set { _enabled = value; } }

    private double _diameter = 5;
    [DisplayName("Point Diameter"),
    DescriptionAttribute("Data point diameter, in pixels")]
    public double diameter { get { return _diameter; } set { _diameter = value; } }

    private double _handleLength = 25;
    [DisplayName("Handle size"),
    DescriptionAttribute("size of the handle between the data point and the value panel")]
    public double handleLength { get { return _handleLength; } set { _handleLength = value; } }

    private FontDescription _font = new FontDescription("Arial", 7, YColor.FromArgb(255, 32, 32, 32), false, false);
    [DisplayName("Font"),
     TypeConverter(typeof(YFontDescriptionConverter)),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Data tracker label fonts")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }



    private YColor _bgColor = YColor.FromArgb(200, 255, 255, 255);
    [DisplayName("Background color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Value panel ground color." + GenericHints.ColorMsg)]
    public YColor bgColor { get { return _bgColor; } set { _bgColor = value;  } }

    private YColor _borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
    DescriptionAttribute("Value panel border and handle  color." + GenericHints.ColorMsg)]
    public YColor borderColor { get { return _borderColor; } set { _borderColor = value;  } }

    private double _borderthickness = 1.0;
    [DisplayName("Border thickness "),
    DescriptionAttribute("Value panel border and handle  thickness")]
    public double borderthickness { get { return _borderthickness; } set { _borderthickness = value; } }

    private double _padding = 5;
    [DisplayName("Padding"),
     DescriptionAttribute("Distance between the panel border and its contents ")]
    public double padding { get { return _padding; } set { _padding = value;  } }

    private int _detectionDistance = 50;
    [DisplayName("Detection distance"),
     DescriptionAttribute("Maximum distance, in pixels, between the mouse and a data point for data tracker to show. Use zero for infinite distance")]
    public int detectionDistance { get { return _detectionDistance; } set { _detectionDistance = value; } }




  }


  //****************************
  //  graph navigator
  //  
  //****************************

  public class NavigatorDescription
  {



    public NavigatorDescription() {}

    private bool _enabled = false;
    [DisplayName("Enabled"),
     TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Should the navigator be shown or not." + GenericHints.BoolMsg)]
    public bool enabled { get { return _enabled; } set { _enabled = value; } }


    private YColor _bgColor1 = YColor.FromArgb(255, 225, 225, 225);
    [DisplayName("Background color 1"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Navigator background gradiant color 1." + GenericHints.ColorMsg)]
    public YColor bgColor1
    {
      get { return _bgColor1; }
      set { _bgColor1 = value; }
    }


    private YColor _bgColor2 = YColor.FromArgb(255, 225, 225, 225);
    [DisplayName("Background color 2"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Navigator background gradiant color 2." + GenericHints.ColorMsg)]
    public YColor bgColor2
    {
      get { return _bgColor2; }
      set { _bgColor2 = value; }
    }

    private YColor _cursorColor = YColor.FromArgb(25, 0, 255, 0);
    [DisplayName("Navigator cursor fill color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Navigator")]
    public YColor cursorColor
    {
      get { return _cursorColor; }
      set { _cursorColor = value; }
    }

    private YColor _cursorBorderColor = YColor.FromArgb(255, 96, 96, 96);
    [DisplayName("Navigator cursor left/right border color." + GenericHints.ColorMsg),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Navigator")]
    public YColor cursorBorderColor
    {
      get { return _cursorBorderColor; }
      set { _cursorBorderColor = value; }
    }


    private Double _xAxisThickness = 1.0;
    [DisplayName("X axis thickness"),
     DescriptionAttribute("Navigator")]
     public Double xAxisThickness
    {
      get { return _xAxisThickness; }
      set { _xAxisThickness = value; }
    }

    private YColor _xAxisColor = YColor.fromColor(Color.Black);
    [DisplayName("X axis color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Navigator X axis color." + GenericHints.ColorMsg)]
     public YColor xAxisColor
    {
      get { return _xAxisColor; }
      set { _xAxisColor = value; }
    }

    private Navigator.YAxisHandling _yAxisHandling = Navigator.YAxisHandling.AUTO;
    [DisplayName("Y axis range"),
     TypeConverter(typeof(SmartEnumConverter)),
     DescriptionAttribute("Is navigator Y axis zoom automatic or inherited from main view settings?")]
    public Navigator.YAxisHandling yAxisHandling
    {
      get { return _yAxisHandling; }
      set { _yAxisHandling = value; }
    }

   

    private FontDescription _font = new FontDescription("Arial",7, YColor.FromArgb(255,32,32,32), false, false);
    [DisplayName("X-Axis Font"),
     CategoryAttribute("Fonts"),
      ReadOnlyAttribute(true),
     TypeConverter(typeof(YFontDescriptionConverter)),
     DescriptionAttribute("Navigator X axis font")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }

    private Double _borderThickness = 1.0;
    [DisplayName("Border thickness"),
     DescriptionAttribute("Navigator")]
    public Double borderThickness
    {
      get { return _borderThickness; }
      set { _borderThickness = value; }
    }

    private YColor _borderColor = YColor.fromColor(Color.Black);
    [DisplayName("Border color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Navigator Border color." + GenericHints.ColorMsg)]
    public YColor borderColor
    {
      get { return _borderColor; }
      set { _borderColor = value; }
    }
  }

  //****************************
  //  graph Legend
  //  
  //****************************

  public class LegendDescription
  {

    private String _title = "";
    [DisplayName("Text"),
     CategoryAttribute("Legend"),
     DescriptionAttribute("Legend text")]
    public String title
    {
      get { return _title; }
      set { _title = value; }
    }


    private FontDescription _font = new FontDescription("Arial", 12, YColor.fromColor(Color.Black), false, true);
    [DisplayName("Font"),
     CategoryAttribute("Legend"),
     TypeConverter(typeof(YFontDescriptionConverter)),
      ReadOnlyAttribute(true),
     DescriptionAttribute("Legend font")]
     public FontDescription font
     {
       get { return _font; }
       set { _font = value; }
     }

  }




    //****************************
    //  graph Yaxis
    //  
    //****************************

    public class YaxisDescription

  {

    public YaxisDescription(int index , bool shown)
    {
      _visible = shown;
      _showGrid = index == 0;
      _position = index == 0 ? YAxis.HrzPosition.LEFT : YAxis.HrzPosition.RIGHT;
    }

    private bool _visible = false;
    [DisplayName("Visible"),
      TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Should that YAxis be shown?." + GenericHints.BoolMsg)]
    public bool visible
    {
      get { return _visible; }
      set { _visible = value; }
    }

    private YAxis.HrzPosition _position = YAxis.HrzPosition.LEFT;
    [DisplayName("Position"),
       TypeConverter(typeof(SmartEnumConverter)),
     DescriptionAttribute("Y Axis position (left / right)")]
    public YAxis.HrzPosition position
    {
      get { return _position; }
      set { _position = value; }
    }

    private doubleNan _min = new doubleNan(Double.NaN);
    [DisplayName("Minimum value"),
     CategoryAttribute("Y axis"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("YAxis minimum value, leave blank for automatic behavior.")]
    public doubleNan min
    {
      get { return _min; }
      set { _min = value; }
    }

    private doubleNan _max = new doubleNan(Double.NaN);
    [DisplayName("Maximim value"),
     CategoryAttribute("Y axis"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("YAxis maximum value, leave blank for automatic behavior.")]
    public doubleNan max
    {
      get { return _max; }
      set { _max = value; }
    }

    private doubleNan _step = new doubleNan(Double.NaN);
    [DisplayName("Steps"),
     CategoryAttribute("Y axis"),
     TypeConverter(typeof(doubleNanConverter)),
     DescriptionAttribute("YAxis step size, leave blank for automatic behavior.")]
    public doubleNan step
    {
      get { return _step; }
      set { _step = value; }
    }

    private YColor _color = YColor.FromArgb(255,127, 127, 127);
    [DisplayName("Color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     CategoryAttribute("Y axis"),
     DescriptionAttribute("Y Axis Color." + GenericHints.ColorMsg)]
    public YColor color
    {
      get { return _color; }
      set { _color = value; }
    }

    private Double _thickness = 1.0;
    [DisplayName("Thickness"),
     CategoryAttribute("Y axis"),
     DescriptionAttribute("Axis thickness")]
    public Double thickness
    {
      get { return _thickness; }
      set { _thickness = value; }
    }

    private bool _showGrid = false;
    [DisplayName("Show Grid"),
     CategoryAttribute("Y axis"),
     TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Show grid horizontal lines, or not." + GenericHints.BoolMsg)]
    public bool showGrid { get { return _showGrid; } set { _showGrid = value; } }

    private YColor _gridColor = YColor.FromArgb(255,210, 210, 210);
    [DisplayName("Grid Color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     CategoryAttribute("Y axis"),
     DescriptionAttribute("Grid horizontal lines color." + GenericHints.ColorMsg)]
    public YColor gridColor
    {
      get { return _gridColor; }
      set { _gridColor = value; }
    }

    private Double _gridThickness = 1.0;
    [DisplayName("Grid thickness"),
     CategoryAttribute("Y axis"),
     DescriptionAttribute("Grid horizontal lines thickness")]
    public Double gridThickness
    {
      get { return _gridThickness; }
      set { _gridThickness = value; }
    }

    private FontDescription _font = new FontDescription("Arial", 10, YColor.fromColor(Color.Black), false, false);
    [DisplayName("Font"),
     CategoryAttribute("Y axis"),
      ReadOnlyAttribute(true),
     TypeConverter(typeof(YFontDescriptionConverter)),
     DescriptionAttribute("Axis font")]
     public FontDescription font
     {
       get { return _font; }
       set { _font = value; }
     }

    private LegendDescription _legend = new LegendDescription();
    [DisplayName("Legend"),
     CategoryAttribute("Y axis"),
     TypeConverter(typeof(YLegendDescriptionConverter)),
     DescriptionAttribute("Axis legend")]
     public LegendDescription legend
      {  get { return _legend; }
        set { _legend = value; }
      }

    private ZoneDescription _zones0 = new ZoneDescription(0, 50, YColor.fromColor(Color.LightGreen));
    [DisplayName("Zone 1"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    TypeConverter(typeof(YZoneConverter)),
    DescriptionAttribute("Zone 1 parameters")]
    public ZoneDescription zones0
    {
      get { return _zones0; }
      set { _zones0 = value; }
    }

    private ZoneDescription _zones1 = new ZoneDescription(50, 80, YColor.fromColor(Color.Yellow));
    [DisplayName("Zone 2"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    TypeConverter(typeof(YZoneConverter)),
    DescriptionAttribute("Zone 2 parameters")]
    public ZoneDescription zones1
    {
      get { return _zones1; }
      set { _zones1 = value; }
    }

    private ZoneDescription _zones2 = new ZoneDescription(80, 100, YColor.fromColor(Color.Red));
    [DisplayName("Zone 3"),
    CategoryAttribute("Zones"),
      ReadOnlyAttribute(true),
    TypeConverter(typeof(YZoneConverter)),
    DescriptionAttribute("Zone 3 parameters")]
    public ZoneDescription zones2
    {
      get { return _zones2; }
      set { _zones2 = value; }
    }


  }

  //****************************
  //  graph Xaxis
  //  
  //****************************

  public class XaxisDescription

  {
    private double _initialZoom = 60;
    [TypeConverter(typeof(XAxisZoomConverter)),
     DisplayName("Initial Zoom"),
     DescriptionAttribute("Zoom level at application startup.")]
    public double initialZoom
    {
      get { return _initialZoom; }
      set { _initialZoom = value; }
    }



    private XAxis.VrtPosition _position = XAxis.VrtPosition.BOTTOM;
    [DisplayName("Position"),
       TypeConverter(typeof(SmartEnumConverter)),
     DescriptionAttribute("X Axis position (top / bottom)")]
    public XAxis.VrtPosition position
    {
      get { return _position; }
      set { _position = value; }
    }

    private TimeConverter.TimeReference _timeReference = TimeConverter.TimeReference.ABSOLUTE;
    [DisplayName("Time reference"),
       TypeConverter(typeof(SmartEnumConverter)),
    DescriptionAttribute("Are gradutation timestamps absolute or relative to experiment start time? ")]
    public TimeConverter.TimeReference timeReference
    {
      get { return _timeReference; }
      set { _timeReference = value; }
    }


    private XAxis.OverflowHandling _overflowHandling = XAxis.OverflowHandling.SCROLL;
    [DisplayName("Overflow Handling"),
     TypeConverter(typeof(SmartEnumConverter)),
     DescriptionAttribute("What to do when new data are about to reach the graph right border")]
    public XAxis.OverflowHandling overflowHandling
    {
      get { return _overflowHandling; }
      set { _overflowHandling = value; }
    }


    private YColor _color = YColor.FromArgb(255, 127, 127, 127);
    [DisplayName("Color"),
Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("X Axis Color." + GenericHints.ColorMsg)]
    public YColor color
    {
      get { return _color; }
      set { _color = value; }
    }

    private Double _thickness = 1.0;
    [DisplayName("Thickness"),

     DescriptionAttribute("Axis thickness")]
    public Double thickness
    {
      get { return _thickness; }
      set { _thickness = value; }
    }

    private bool _showGrid = false;
    [DisplayName("Show Grid"),
     TypeConverter(typeof(YesNoConverter)),
     DescriptionAttribute("Show grid vertical lines, or not." + GenericHints.BoolMsg)]
    public bool showGrid { get { return _showGrid; } set { _showGrid = value; } }

    private YColor _gridColor = YColor.FromArgb(50, 0, 0, 0);
    [DisplayName("Grid Color"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Grid vertical lines color." + GenericHints.ColorMsg)]
    public YColor gridColor
    {
      get { return _gridColor; }
      set { _gridColor = value; }
    }

    private Double _gridThickness = 1.0;
    [DisplayName("Grid thickness"),
     DescriptionAttribute("Grid vertical lines thickness")]
    public Double gridThickness
    {
      get { return _gridThickness; }
      set { _gridThickness = value; }
    }

    private FontDescription _font = new FontDescription("Arial", 10, YColor.fromColor(Color.Black), false, false);
    [DisplayName("Font"),
      ReadOnlyAttribute(true),
     TypeConverter(typeof(YFontDescriptionConverter)),
     DescriptionAttribute("Axis font")]
    public FontDescription font
    {
      get { return _font; }
      set { _font = value; }
    }

    private LegendDescription _legend = new LegendDescription();
    [DisplayName("Legend"),
      ReadOnlyAttribute(true),
     TypeConverter(typeof(YLegendDescriptionConverter)),
     DescriptionAttribute("X Axis legend")]
    public LegendDescription legend
    {
      get { return _legend; }
      set { _legend = value; }
    }

  }


  //****************************
  //  graph display
  //  
  //****************************


  public class GraphFormProperties : GenericProperties
  {


    public GraphFormProperties(XmlNode initData, Form owner) : base(initData, owner)
    { }

    public override bool IsDataSourceAssigned()
    {
      foreach (var p in typeof(GraphFormProperties).GetProperties())
      {
        string name = p.Name;
        if (name.StartsWith("Graphs_series"))
        {
          ChartSerie s = (ChartSerie)p.GetValue(this, null);
          if (!(s.DataSource_source is NullYSensor)) return true;

        }
      }
      return false;

    }



    ChartSerie _Graph_series0 = new ChartSerie(YColor.fromColor(Color.Tomato));
    [TypeConverter(typeof(ChartSerieConverter)),
      DisplayName("Series 1"),
      CategoryAttribute("Data Sources"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("First data series, expand for more.")]
    public ChartSerie Graph_series0
    {
      get { return _Graph_series0; }
      set { _Graph_series0 = value; }
    }

    ChartSerie _Graph_series1 = new ChartSerie(YColor.fromColor(Color.DodgerBlue));
    [TypeConverter(typeof(ChartSerieConverter)),
      DisplayName("Series 2"),
      CategoryAttribute("Data Sources"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Second data series, expand for more.")]
    public ChartSerie Graph_series1
    {
      get { return _Graph_series1; }
      set { _Graph_series1 = value; }
    }

    ChartSerie _Graph_series2 = new ChartSerie(YColor.fromColor(Color.SeaGreen));
    [TypeConverter(typeof(ChartSerieConverter)),
      DisplayName("Series 3"),
      CategoryAttribute("Data Sources"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Third series, expand for more.")]
    public ChartSerie Graph_series2
    {
      get { return _Graph_series2; }
      set { _Graph_series2 = value; }
    }


    ChartSerie _Graph_series3 = new ChartSerie(YColor.fromColor(Color.Gold));
    [TypeConverter(typeof(ChartSerieConverter)),
      DisplayName("Series 4"),
      CategoryAttribute("Data Sources"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Fourth series, expand for more.")]
    public ChartSerie Graph_series3
    {
      get { return _Graph_series3; }
      set { _Graph_series3 = value; }
    }



    private bool _Graph_showRecordedData = false;
    [
     DisplayName("Use datalogger data"),
     TypeConverter(typeof(YesNoConverter)),
     CategoryAttribute("Graph"),
     DescriptionAttribute("Makes the graph show the datalogger contents." + GenericHints.BoolMsg)]
    public bool Graph_showRecordedData
    {
      get { return _Graph_showRecordedData; }
      set
      {
        if (_Graph_showRecordedData != value)
        {
          _Graph_showRecordedData = value;
          if (ownerForm != null)
            ((GraphForm)ownerForm).showRecordedDatachanged();
        }
      }
    }

    private YColor _Graph_borderColor = YColor.fromColor(Color.LightGray);
    [DisplayName("Border color"),
     CategoryAttribute("Graph"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Canvas borders color." + GenericHints.ColorMsg)]
    public YColor Graph_borderColor
    {
      get { return _Graph_borderColor; }
      set { _Graph_borderColor = value; }
    }

    private Double _Graph_borderThickness = 1.0;
    [DisplayName("Border thickness"),
     CategoryAttribute("Graph"),
     DescriptionAttribute("Canvas borders thickness.")]
    public Double Graph_borderThickness
    {
      get { return _Graph_borderThickness; }
      set { _Graph_borderThickness = value; }
    }

    private YColor _Graph_bgColor1 = YColor.FromArgb(255, 220, 220, 220);
    [DisplayName("Background color 1"),
     CategoryAttribute("Graph"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Background gradiant color 1." + GenericHints.ColorMsg)]
    public YColor Graph_bgColor1
    {
      get { return _Graph_bgColor1; }
      set { _Graph_bgColor1 = value; }
    }

    private YColor _Graph_bgColor2 = YColor.FromArgb(255, 240, 240, 240);
    [DisplayName("Background color 2"),
     CategoryAttribute("Graph"),
      Editor(typeof(YColorEditor), typeof(UITypeEditor)),
       TypeConverter(typeof(YColorConverter)),
     DescriptionAttribute("Background gradiant color 2." + GenericHints.ColorMsg)]
    public YColor Graph_bgColor2
    {
      get { return _Graph_bgColor2; }
      set { _Graph_bgColor2 = value; }
    }

    private Proportional.ResizeRule _Graph_resizeRule = Proportional.ResizeRule.FIXED;
    [DisplayName("Font sizes"),
     CategoryAttribute("Graph"),
     TypeConverter(typeof(SmartEnumConverter)),
     
     DescriptionAttribute("Are font sizes fixed or do they change when window is resized?")]
    public Proportional.ResizeRule Graph_resizeRule
    {
      get { return _Graph_resizeRule; }
      set { _Graph_resizeRule = value; }
    }

    private XaxisDescription _Graph_xAxis = new XaxisDescription();
    [DisplayName("X Axis"),
     TypeConverter(typeof(XaxisDescriptionConverter)),
     CategoryAttribute("X/Y Axes"),
     ReadOnlyAttribute(true),
     DescriptionAttribute("X-Axis, expand for more")]
    public XaxisDescription Graph_xAxis
    { get { return _Graph_xAxis; }
      set { _Graph_xAxis = value; }
    }


    private YaxisDescription _Graph_yAxes0 = new YaxisDescription(0, true);
    [TypeConverter(typeof(YaxisDescriptionConverter)),
      DisplayName("YAxis 1"),
      ReadOnlyAttribute(true),
      CategoryAttribute("X/Y Axes"),
      DescriptionAttribute("First Y Axis, expand for more.")]
    public YaxisDescription Graph_yAxes0
    {
      get { return _Graph_yAxes0; }
      set { _Graph_yAxes0 = value; }
    }


    private YaxisDescription _Graph_yAxes1 = new YaxisDescription(1, false);
    [TypeConverter(typeof(YaxisDescriptionConverter)),
      DisplayName("YAxis 2"),
      ReadOnlyAttribute(true),
     CategoryAttribute("X/Y Axes"),
      DescriptionAttribute("Second Y Axis, expand for more.")]
    public YaxisDescription Graph_yAxes1
    {
      get { return _Graph_yAxes1; }
      set { _Graph_yAxes1 = value; }
    }

    private YaxisDescription _Graph_yAxes2 = new YaxisDescription(2, false);
    [TypeConverter(typeof(YaxisDescriptionConverter)),
      DisplayName("YAxis 3"),
      ReadOnlyAttribute(true),
      CategoryAttribute("X/Y Axes"),
      DescriptionAttribute("Third Y Axis, expand for more.")]
    public YaxisDescription Graph_yAxes2
    {
      get { return _Graph_yAxes2; }
      set { _Graph_yAxes2 = value; }
    }

    private LegendPanelDescription _Graph_legendPanel = new LegendPanelDescription();
    [TypeConverter(typeof(LegendPanelDescriptionConverter)),
      DisplayName("Legend Panel"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Graph"),
      DescriptionAttribute("Panel containing a description of all data series. Expand for more.")]
    public LegendPanelDescription Graph_legendPanel
    {
      get { return _Graph_legendPanel; }
      set { _Graph_legendPanel = value; }


    }



    private DataTrackerDescription _Graph_dataTracker = new DataTrackerDescription();
    [TypeConverter(typeof(DataTrackerDescriptionConverter)),
      DisplayName("Data tracker"),
      ReadOnlyAttribute(true),
      CategoryAttribute("Graph"),
      DescriptionAttribute("Show dynamic value labels while the mouse is moving over the graph. Expand for more.")]
    public DataTrackerDescription Graph_dataTracker
    {
      get { return _Graph_dataTracker; }
      set { _Graph_dataTracker = value; }
    }


    private NavigatorDescription _Graph_navigator = new NavigatorDescription();
    [TypeConverter(typeof(NavigatorDescriptionConverter)),
      DisplayName("Navigator"),
      CategoryAttribute("Graph"),
      ReadOnlyAttribute(true),
      DescriptionAttribute("Small additionnal graph showing the whole data set and allowing to quickly navigate among data. Expand for more.")]
    public NavigatorDescription Graph_navigator
    {
      get { return _Graph_navigator; }
     set { _Graph_navigator = value; }
    }

  }

}
