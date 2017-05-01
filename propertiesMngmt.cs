
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *   properties management, mainly converter
 *   classes for the propertyGrid editor
 * 
 * 
 */

using LiveCharts.WinForms;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace YoctoVisualisation
{
  public delegate bool PropFilter(string propNname);

  [AttributeUsage(AttributeTargets.All)]
  public class NotSavedInXMLAttribute : System.Attribute
  {
    public readonly bool mustSave;

    public NotSavedInXMLAttribute(bool value)
    {
      this.mustSave = value;
    }


  }



  public class YAxisParamConverter : ExpandableObjectConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(YAxisParam)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                object value, System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is YAxisParam)
      {
        return "..";
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }

  public class XAxisParamConverter : ExpandableObjectConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(XAxisBasicParam)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                object value, System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is XAxisBasicParam)
      {
        return "..";
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }


  public class YAngularSectionParamConverter : ExpandableObjectConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(YAngularSection)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                object value, System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is YAngularSection)
      {
        return "..";
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }

  public class YAxisSectionParamConverter : ExpandableObjectConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(YAxisSection)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                object value, System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is YAxisSection)
      {
        return "..";
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }



  public class ChartSerieConverter : ExpandableObjectConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(ChartSerie)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                object value, System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is ChartSerie)
      {
        return "..";
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }



  public class YesNoConverter : StringConverter
  {

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    { return true; }

    public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
    { return new StandardValuesCollection(new string[] { "Yes", "No" }); }


    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(bool)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                CultureInfo culture,
                                object value,
                                System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is bool)
      {

        return (bool)value ? "Yes" : "No";
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context,
                              System.Type sourceType)
    {
      if (sourceType == typeof(string)) return true;

      return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context,
                              CultureInfo culture, object value)
    {
      if (value is String)
      {
        return (((string)value).ToUpper() == "YES");
      }

      return base.ConvertFrom(context, culture, value);
    }
  }


  public class AvgMinMaxConverter : StringConverter
  {

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    { return true; }

    public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
    { return new StandardValuesCollection(new string[] { "Avg values", "Min values", "Max values" }); }


    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(int)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                CultureInfo culture,
                                object value,
                                System.Type destinationType)
    {
      if ((destinationType == typeof(System.String)) && (value is int))
      {
        switch ((int)value)
        {
          case 1: return "Min values";
          case 2: return "Max values";
          default: return "Avg values";
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context,
                              System.Type sourceType)
    {
      if (sourceType == typeof(string)) return true;

      return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context,
                              CultureInfo culture, object value)
    {
      if (value is String)
      {
        if (((string)value).ToUpper().IndexOf("MIN") >= 0) return 1;
        if (((string)value).ToUpper().IndexOf("MAX") >= 0) return 2;
        return 0;
      }

      return base.ConvertFrom(context, culture, value);
    }
  }









  public class FormatterPrecisionConverter : StringConverter
  {
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    { return true; }

    public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
    { return new StandardValuesCollection(new string[] { "0", "0.1", "0.12", "0.123" }); }
    public override bool GetStandardValuesExclusive(
                           ITypeDescriptorContext context)
    {
      return true;
    }
  }

  public class FrequencyConverter : StringConverter
  {
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    { return true; }

    public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
    {
      return new StandardValuesCollection(new string[] { "25/s", "10/s", "5/s", "4/s","3/s","1/s",
                                                         "30/m","12/m","6/m","4/m","3/m","2/m","1/m",
                                                         "30/h","12/h","6/h","4/h","3/h","2/h","1/h"
    });
    }

    public override bool GetStandardValuesExclusive(
                           ITypeDescriptorContext context)
    {
      return true;
    }


  }


  public class AxisSeparatorConverter : ExpandableObjectConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context,
                                 System.Type destinationType)
    {
      if (destinationType == typeof(AxisSeparator)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                               CultureInfo culture,
                               object value,
                               System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is AxisSeparator)
        return ((AxisSeparator)value).IsEnabled ? "Enabled" : "Disabled";

      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool GetStandardValuesExclusive(
                           ITypeDescriptorContext context)
    {
      return true;
    }

  }









  public class FontConverter : StringConverter
  {
    FontFamily[] ffArray = FontFamily.Families;
    string[] fontnames;

    public FontConverter()
    {
      fontnames = new string[ffArray.Length];
      for (int i = 0; i < ffArray.Length; i++)
        fontnames[i] = ffArray[i].Name;

    }


    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    { return true; }

    public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
    {
      return new StandardValuesCollection(fontnames);
    }

    public override bool GetStandardValuesExclusive(
                           ITypeDescriptorContext context)
    {
      return true;
    }

  }



  public class XAxisZoomConverter : StringConverter
  {

    string[] avalues = new string[] { "15 secondes", "30 secondes ",
                                       "1 minute", "5 minutes", "15 minutes", "30 minutes",
                                       "1 hour", "6 hour", "12 hours",
                                       "1 day", "1 week", "1 month"};

    double[] nvalues = new double[] { 15,30,
                                      60,300,15*60,30*60,
                                      3600, 6*3600,12*3600,
                                      86400, 7*86400, 30*86400};


    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    { return true; }

    public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
    { return new StandardValuesCollection(avalues); }


    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(double)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override bool GetStandardValuesExclusive(
                           ITypeDescriptorContext context)
    {
      return true;
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                CultureInfo culture,
                                object value,
                                System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is double)
      {
        for (int i = 0; i < nvalues.Length; i++)
          if ((double)value == nvalues[i]) return avalues[i];
        return ((double)value).ToString();
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context,
                              System.Type sourceType)
    {
      if (sourceType == typeof(string)) return true;

      return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context,
                              CultureInfo culture, object value)
    {
      if (value is String)
      {
        for (int i = 0; i < avalues.Length; i++)
          if ((string)value == avalues[i]) return nvalues[i];
        return float.Parse((string)value);
      }

      return base.ConvertFrom(context, culture, value);
    }
  }



  public class YAxisPositionConverter : StringConverter
  {

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    { return true; }

    public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
    { return new StandardValuesCollection(new string[] { "Left", "Right" }); }


    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(LiveCharts.AxisPosition)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                CultureInfo culture,
                                object value,
                                System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is LiveCharts.AxisPosition)
      {
        if ((LiveCharts.AxisPosition)value == LiveCharts.AxisPosition.RightTop) return "Right";
        return "Left";
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context,
                              System.Type sourceType)
    {
      if (sourceType == typeof(string)) return true;

      return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context,
                              CultureInfo culture, object value)
    {
      if (value is String)
      {
        if ((string)value == "Right") return LiveCharts.AxisPosition.RightTop;
        return LiveCharts.AxisPosition.LeftBottom;
      }

      return base.ConvertFrom(context, culture, value);
    }
  }


  public class YAxisChooserConverter : StringConverter
  {

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    { return true; }

    public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
    {
      List<string> AxisList = new List<string>();
      for (int i = 0; i < GraphForm.YAxisCount; i++)
        switch (i)
        {
          case 0: AxisList.Add("1rst Y Axis"); break;
          case 1: AxisList.Add("2nd Y Axis"); break;
          case 2: AxisList.Add("3rnd Y Axis"); break;
          default: AxisList.Add(i.ToString() + "th Y axis"); break;
        }

      return new StandardValuesCollection(AxisList);
    }


    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(int)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                CultureInfo culture,
                                object value,
                                System.Type destinationType)
    {
      if (destinationType == typeof(string) && (value is int))
      {
        int v = (int)value;
        switch (v)
        {
          case 0: return "1rst Y Axis";
          case 1: return "2nd Y Axis";
          case 2: return "3rnd Y Axis";
          default: return v.ToString() + "th Y axis";
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context,
                              System.Type sourceType)
    {
      if (sourceType == typeof(string)) return true;

      return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context,
                              CultureInfo culture, object value)
    {
      if (value is String)
      {
        return Convert.ToInt32(Regex.Replace((string)value, "[^0-9]", "")) - 1;
      }
      return base.ConvertFrom(context, culture, value);
    }
  }


  public class doubleNanConverter : StringConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(doubleNan)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                CultureInfo culture,
                                object value,
                                System.Type destinationType)
    {
      if (destinationType == typeof(System.String) && value is doubleNan)
      {
        if (double.IsNaN(((doubleNan)value).value))
          return "";
        return ((doubleNan)value).ToString();
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context,
                              System.Type sourceType)
    {
      if (sourceType == typeof(string))
        return true;

      return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context,
                              CultureInfo culture, object value)
    {
      if (value is String)
      {
        return new doubleNan((string)value);
      }
      return base.ConvertFrom(context, culture, value);
    }
  }

  public class doubleNan
  {
    double _value = Double.NaN;
    public doubleNan(double v)
    {
      _value = v;

    }

    public doubleNan()
    {


    }
    public doubleNan(string v)
    {

      if (!Double.TryParse(v, out _value)) _value = Double.NaN;
    }

    public override string ToString()

    {
      if (Double.IsNaN(_value)) return "";
      return _value.ToString();
    }

    public double value
    {
      get { return this._value; }
      set { this._value = value; }

    }
  }


  public abstract class GenericProperties
  {
    public Form ownerForm = null;

    static public bool NoFilter(string propNname) { return true; }

    public abstract bool IsDataSourceAssigned();

    public GenericProperties(XmlNode initData, Form Owner)
    {
      ownerForm = Owner;
      _Form_Text = StartForm.newWindowName();
      if (initData != null) loadProperties(initData, Owner, this);



    }

    public void loadProperties(XmlNode initData, Form Owner, Object o)
    {
      string value;
      ownerForm = Owner;
      if (initData != null)
        foreach (var p in o.GetType().GetProperties())
        {
          foreach (XmlNode node in initData.ChildNodes)
          {
            if (p.Name == node.Name)
            {
              Object target = p.GetValue(o, null);
              object[] attrs = p.GetCustomAttributes(true);
              bool Mustload = true;
              foreach (object a in attrs)
                if (a is NotSavedInXMLAttribute)
                  Mustload = !((NotSavedInXMLAttribute)a).mustSave;


              if (Mustload)
              {
                if (IsStructured(target))
                {
                  loadProperties(node, Owner, target);
                }
                else
                  switch (p.PropertyType.ToString())
                  {
                    case "System.Boolean":
                      p.SetValue(o, (node.Attributes["value"].InnerText.ToUpper() == "TRUE")); break;
                    case "System.Double":
                      p.SetValue(o, Convert.ToDouble(node.Attributes["value"].InnerText));
                      break;
                    case "System.Int32":
                      p.SetValue(o, Int32.Parse(node.Attributes["value"].InnerText));
                      break;
                    case "System.String":
                      p.SetValue(o, node.Attributes["value"].InnerText);
                      break;
                    case "YoctoVisualisation.AxisSeparator":
                      AxisSeparator sep = new AxisSeparator
                                                (node.Attributes["value"].InnerText.ToUpper() == "ENABLED",
                                                 float.Parse(node.Attributes["Thickness"].InnerText),
                                                 (Color)ColorTranslator.FromHtml(node.Attributes["Color"].InnerText));


                      p.SetValue(o, sep);
                      break;
                    case "System.Drawing.Font":
                      System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
                      var values = Enum.GetValues(typeof(System.Drawing.FontStyle));
                      string attr = node.Attributes["style"].InnerText;
                      foreach (var v in values)
                        if (attr.IndexOf(v.ToString()) >= 0) style |= (System.Drawing.FontStyle)v;
                      p.SetValue(o, new Font(node.Attributes["value"].InnerText, float.Parse(node.Attributes["size"].InnerText), style));
                      break;
                    case "System.Drawing.Color":
                      value = node.Attributes["value"].InnerText;
                      p.SetValue(o, (Color)ColorTranslator.FromHtml(value));
                      break;
                    case "YoctoVisualisation.doubleNan":
                      value = node.Attributes["value"].InnerText;
                      p.SetValue(o, new doubleNan(value));
                      break;

                    case "System.Windows.Media.FontFamily":
                      value = node.Attributes["value"].InnerText;
                      p.SetValue(o, new System.Windows.Media.FontFamily(value));
                      break;
                    case "LiveCharts.AxisPosition":
                      value = node.Attributes["value"].InnerText;
                      p.SetValue(o, value == "RightTop" ? LiveCharts.AxisPosition.RightTop : LiveCharts.AxisPosition.LeftBottom);
                      break;
                    case "YoctoVisualisation.CustomYSensor":
                      value = node.Attributes["value"].InnerText;
                      if ((value.ToUpper() != "NULL") && (value != ""))

                      {
                        CustomYSensor s = sensorsManager.AddNewSensor(value);
                        p.SetValue(o, s);

                      }
                      else p.SetValue(o, sensorsManager.getNullSensor());

                      break;
                    default:
                      throw new ArgumentException("Init from XML file: Unhandled data type: (" + p.PropertyType.ToString() + ")");
                  }
              }
            }
          }
        }
    }

    private string _Form_Text = "New window";
    [DisplayName("Window title"),
     CategoryAttribute("Window"),
     DescriptionAttribute("Text displayed in the window top banner")]
    public string Form_Text
    {
      get { return _Form_Text; }
      set { _Form_Text = value; }
    }

    private Color _Form_BackColor = Color.FromArgb(0xff, 0xf0, 0xf0, 0xf0);
    [DisplayName("Background color"),
     CategoryAttribute("Window"),
     DescriptionAttribute("Window background color")]
    public Color Form_BackColor
    {
      get { return _Form_BackColor; }
      set { _Form_BackColor = value; }
    }

    public string getXml()
    {
      return getXml(this);
    }

    public string getXml(Object o)
    {
      string res = "";

      string value = "";
      foreach (System.Reflection.PropertyInfo p in o.GetType().GetProperties())
      {
        var child = p.GetValue(o, null);
        if (IsStructured(child))
        {
          res = res + "<" + p.Name + ">\r\n"
                      + getXml(child)
                      + "</" + p.Name + ">\r\n";
        }
        else
        {
          string type = p.PropertyType.ToString();

          bool Mustsave = true;
          object[] attrs = p.GetCustomAttributes(true);
          foreach (object a in attrs)
            if (a is NotSavedInXMLAttribute)
              Mustsave = !((NotSavedInXMLAttribute)a).mustSave;


          if (Mustsave)
          {
            switch (type)
            {
              case "System.Drawing.Font":
                Font f = (Font)p.GetValue(o, null);
                value = f.Name + "\" size=\"" + f.Size.ToString() + "\" style=\"" + f.Style.ToString();
                break;

              case "YoctoVisualisation.AxisSeparator":
                AxisSeparator sep = (AxisSeparator)p.GetValue(o, null);
                value = (sep.IsEnabled ? "Enabled" : "Disabled") + "\" Thickness=\"" + sep.StrokeThickness.ToString() + "\" Color=\"" + ColorTranslator.ToHtml(sep.Stroke);
                break;
              case "System.Boolean": value = (bool)p.GetValue(o, null) ? "TRUE" : "FALSE"; break;
              case "System.Double":
              case "System.Int32":
              case "System.Windows.Media.FontFamily":
              case "System.String": value = System.Security.SecurityElement.Escape(p.GetValue(o, null).ToString()); break;
              case "System.Drawing.Color": value = ColorTranslator.ToHtml(((Color)p.GetValue(o, null))); break;
              case "YoctoVisualisation.doubleNan": value = p.GetValue(o, null).ToString(); break;
              case "LiveCharts.AxisPosition": value = p.GetValue(o, null).ToString(); break;
              case "YoctoVisualisation.CustomYSensor":
                CustomYSensor s = (CustomYSensor)p.GetValue(o, null);
                value = ((s == null) || (s is NullYSensor)) ? "NULL" : s.get_hardwareId();
                break;
              default:
                throw new ArgumentException("XML generation : ubhandled type (" + type + ")");
            }
            res = res + "<" + p.Name + " value=\"" + value + "\"/>\n";
          }
        }
      }
      return res;
    }

    static public bool IsStructured(Object o)
    {
      if (o == null) return false;
      if (o is String) return false;
      if (o is System.Drawing.Color) return false;
      if (o is System.Windows.Media.FontFamily) return false;
      if (o is Font) return false;
      if (o is bool) return false;
      if (o is AxisSeparator) return false;
      if (o is CustomYSensor) return false;
      if (o is doubleNan) return false;
      foreach (var p in o.GetType().GetProperties())
      { return true; }
      return false;
    }

    static public List<string> ExtractPropPath(GridItem p, ref string OriginalPropName, ref string fullpropname, ref string propType)
    {
      List<string> path = new List<string>();
      int index = -1;
      OriginalPropName = p.PropertyDescriptor.Name;
      do
      {
        fullpropname = p.PropertyDescriptor.Name;
        index = fullpropname.IndexOf("_");
        if (index < 0)
        {
          path.Insert(0, fullpropname);
          p = p.Parent;
          if (p == null) throw new System.ArgumentException("invalid Property name");
        }

      } while (index < 0);

      propType = fullpropname.Substring(0, index);
      string propname = fullpropname.Substring(index + 1);

      path.Insert(0, propname);
      return path;
    }

    static public void newSetProperty(object rootTarget, object source, string propertySourceName, List<string> path)
    {
      newSetProperty(rootTarget, source, propertySourceName, path, NoFilter);
    }


    static public void newSetProperty(object rootTarget, object source, string propertySourceName, List<string> path, PropFilter filterAllow)
    {
      System.Reflection.PropertyInfo tinfo = null;
      string ttype = "";

      object FinalTarget = rootTarget;
      for (int i = 0; i < path.Count - 1; i++)
      {
        string name = path[i];
        string index = "";
        while (char.IsDigit(name, name.Length - 1))
        {
          index = name[name.Length - 1] + index;
          name = name.Substring(0, name.Length - 1);
        }
        tinfo = FinalTarget.GetType().GetProperty(name);
        if (index == "")
        {

          FinalTarget = tinfo.GetValue(FinalTarget);
        }
        else
        {
          object targetArray = tinfo.GetValue(FinalTarget, null);
          string targetTypename = targetArray.GetType().FullName;
          if (targetArray is List<AngularSection>)
            FinalTarget = ((List<AngularSection>)targetArray)[int.Parse(index)];
          else
         if (targetArray is AxesCollection)
            FinalTarget = ((AxesCollection)targetArray)[int.Parse(index)];
          else
         if (targetArray is SectionsCollection)
            FinalTarget = ((SectionsCollection)targetArray)[int.Parse(index)];
          else

         if (targetArray is LiveCharts.SeriesCollection)
            FinalTarget = ((LiveCharts.SeriesCollection)targetArray)[int.Parse(index)];
          else
            FinalTarget = ((List<object>)targetArray)[int.Parse(index)];
        }
      }

      object TerminalSource = source;
      System.Reflection.PropertyInfo sinfo = null;

      sinfo = TerminalSource.GetType().GetProperty(propertySourceName);
      TerminalSource = sinfo.GetValue(TerminalSource);

      for (int i = 1; i < path.Count; i++)
      {
        sinfo = TerminalSource.GetType().GetProperty(path[i]);
        TerminalSource = sinfo.GetValue(TerminalSource);


      }
      string stype = sinfo.PropertyType.FullName;
      if (path.Count > 0)
      {
        string targetname = path[path.Count - 1];
        tinfo = FinalTarget.GetType().GetProperty(targetname);
      }
      else
      {
        string targetname = propertySourceName;
        int n = targetname.IndexOf("_");
        targetname = targetname.Substring(n + 1);
        tinfo = rootTarget.GetType().GetProperty(targetname);

      }
      if (tinfo == null) return; // the propertie does not exists in the widget
      ttype = tinfo.PropertyType.FullName;




      if ((stype == "System.Drawing.Color") && (ttype == "System.Windows.Media.Color"))
      {
        Color c = (Color)TerminalSource;
        System.Windows.Media.Color C2 = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        tinfo.SetValue(FinalTarget, C2);
      }
      else

      if ((stype == "YoctoVisualisation.doubleNan") && (ttype == "System.Double"))
      {
        doubleNan v = (doubleNan)TerminalSource;
        tinfo.SetValue(FinalTarget, v.value);
      }
      else
      if ((stype == "YoctoVisualisation.doubleNan") && (ttype.StartsWith("System.Nullable")))
      {
        if (Double.IsNaN(((doubleNan)TerminalSource).value))
          tinfo.SetValue(FinalTarget, null, null);
        else
          tinfo.SetValue(FinalTarget, ((doubleNan)TerminalSource).value, null);
      }


      else
      if ((stype == "System.String") && (ttype == "System.Windows.Media.FontFamily"))
      {
        string v = (string)TerminalSource;
        tinfo.SetValue(FinalTarget, new System.Windows.Media.FontFamily(v));
      }


      else
        if ((stype == "System.Drawing.Color") && (ttype == "System.Windows.Media.Brush"))
      {
        Color c = (Color)TerminalSource;
        byte a = c.A;
        if (propertySourceName == "Graph2_ScrollBarFill") a = 80; // dirty hack to keep the navigator's cursor semi-transparent 
        System.Windows.Media.Color C2 = System.Windows.Media.Color.FromArgb(a, c.R, c.G, c.B);
        System.Windows.Media.SolidColorBrush b = new System.Windows.Media.SolidColorBrush(C2);
        tinfo.SetValue(FinalTarget, b);
      }
      else

       if ((stype == "YoctoVisualisation.AxisSeparator") && (ttype == "LiveCharts.Wpf.Separator"))
      {

        AxisSeparator it = (AxisSeparator)TerminalSource;
        System.Windows.Media.SolidColorBrush b = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(it.Stroke.R, it.Stroke.G, it.Stroke.B));

        Axis a = (Axis)FinalTarget;
        Separator sep = ((Separator)tinfo.GetValue(a, null));

        sep.Stroke = b;
        sep.StrokeThickness = it.StrokeThickness;
        sep.IsEnabled = it.IsEnabled;


      }
      else
      {
        tinfo.SetValue(FinalTarget, TerminalSource, null);
      }
    }



    public static string ExtractIndex(string value, ref int arrayIndex)
    {
      int n = 0;
      arrayIndex = -1;
      while ((value[value.Length - 1 - n] >= '0') &&
             (value[value.Length - 1 - n] <= '9')) n++;

      if (n > 0)
      {
        arrayIndex = int.Parse(value.Substring(value.Length - n));
        value = value.Substring(0, value.Length - n);

      }
      return value;

    }

    public void ApplyAllProperties(object target)
    { ApplyAllProperties(target, NoFilter); }


    public void ApplyProperties(object rootSource, object rootTarget, string fullpropname, object sourceValue, List<String> path)
    {
      if (!IsStructured(sourceValue))
      {
        newSetProperty(rootTarget, this, fullpropname, path);
      }
      else
      {
        System.Reflection.PropertyInfo[] props = sourceValue.GetType().GetProperties();
        for (int i = 0; i < props.Length; i++)
        {
          path.Add(props[i].Name);
          ApplyProperties(rootSource, rootTarget, fullpropname, props[i].GetValue(sourceValue, null), path);
          path.RemoveAt(path.Count - 1);
        }

      }

    }

    public void ApplyAllProperties(object target, PropFilter filter)
    {
      System.Reflection.PropertyInfo[] props = this.GetType().GetProperties();
      foreach (var p in props)
      {
        string fullpropname = p.Name;
        int index = fullpropname.IndexOf("_");
        if (index < 0) throw new System.ArgumentException("invalid Propertiy name");
        string propType = fullpropname.Substring(0, index);
        string propname = fullpropname.Substring(index + 1);
        // int arrayIndex = -1;
        //  propname = ExtractIndex(propname, ref arrayIndex);
        List<String> path = new List<String>();
        path.Add(propname);


        if ((target is Form) && (propType == "Form"))
          ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);


        if ((target is SolidGauge) && (propType == "SolidGauge"))
          ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        if ((target is LiveCharts.WinForms.AngularGauge) && (propType == "AngularGauge"))
          ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        if ((target is LiveCharts.WinForms.CartesianChart) && (propType == "Graph"))
          if ((string)((LiveCharts.WinForms.CartesianChart)target).Tag == "main")
            ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        if ((target is LiveCharts.WinForms.CartesianChart) && (propType == "Graph2"))

          if ((string)((LiveCharts.WinForms.CartesianChart)target).Tag == "secondary")

            ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        if ((target is LiveCharts.WinForms.CartesianChart) && (propType == "Graphs"))
        {
          if ((string)((LiveCharts.WinForms.CartesianChart)target).Tag == "main")
            ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);
          else
           if (GraphForm.AppliesToSecondaryGraph(propname))
            ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        }




      }
    }
  }

}
