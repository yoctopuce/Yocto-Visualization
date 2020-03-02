
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *   properties management, mainly converter
 *   classes for the propertyGrid editor
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using YColors;
using YDataRendering;

using System.Drawing.Design;

namespace YoctoVisualisation
{
  public delegate bool PropFilter(string propNname);



[AttributeUsage(AttributeTargets.All)]
 public class ParamExtraDescriptionAttribute : System.Attribute
 {
  public Type allowedValueClass =null;

  public ParamExtraDescriptionAttribute(Type AllowedValueClass)
  {
    this.allowedValueClass = AllowedValueClass;
  }


}




[AttributeUsage(AttributeTargets.All)]
  public class ParamCategorySummaryAttribute : System.Attribute
  {
    public string callback = null;

    public ParamCategorySummaryAttribute(string summaryPropertyName)
    {
      this.callback = summaryPropertyName;
    }


  }




  [AttributeUsage(AttributeTargets.All)]
  public class OnlyAvailableonAttribute : System.Attribute
  {
    public const int Windows = 1;
    public const int Linux = 2;
    public const int MacOS = 4;


    


   private int  _targetOs = Windows | Linux | MacOS;

   public OnlyAvailableonAttribute(int targetOs )
    {
      this._targetOs = targetOs;
    }

    public bool Available
    {
      get {
        if (((_targetOs & MacOS) != 0) && constants.OSX_Running) return true;
        if (((_targetOs & Linux) != 0) && constants.MonoRunning) return true;
        if ((_targetOs & Windows) != 0 && !constants.MonoRunning && !constants.OSX_Running) return true;
        return false;
      }
    }

  }

  [AttributeUsage(AttributeTargets.All)]
  public class PreExpandedCategoryAttribute : System.Attribute
  {
    public readonly bool preExpandedCategory;

    public PreExpandedCategoryAttribute(bool value)
    {
      this.preExpandedCategory = value;
    }
  }


  [AttributeUsage(AttributeTargets.All)]
  public class PreExpandedAttribute : System.Attribute
  {
    public readonly bool preExpanded;

    public PreExpandedAttribute(bool value)
    { 
      this.preExpanded = value;
    }
  }

  [AttributeUsage(AttributeTargets.All)]
  public class ChangeCausesParentRefreshAttribute : System.Attribute
  {
    public readonly bool changeCausesParentRefresh;

    public ChangeCausesParentRefreshAttribute(bool value)
    {
      this.changeCausesParentRefresh = value;
    }
  }



  [AttributeUsage(AttributeTargets.All)]
  public class NotSavedInXMLAttribute : System.Attribute
  {
    public readonly bool mustSave;

    public NotSavedInXMLAttribute(bool value)
    {
      this.mustSave = value;
    }
  }


  [AttributeUsage(AttributeTargets.All)]
  public class IsReadonlyCallAttribute : System.Attribute
  {
    public string IsReadonlyCall;

    public IsReadonlyCallAttribute(string value)
    {
      this.IsReadonlyCall = value;
    }
  }

  public static class colorConverter
   {  public static Color colorFromHex(string hex)
      {
        return Color.FromArgb( Convert.ToInt32(hex, 16));
     
      }

    public static String colorToHex(Color c)
    {
      return c.ToArgb().ToString("X8");

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





  
    /// <summary>
    /// Set the Browsable property.
    /// NOTE: Be sure to decorate the property with [Browsable(true)]
    /// </summary>
    /// <param name="PropertyName">Name of the variable</param>
    /// <param name="bIsBrowsable">Browsable Value</param>
    public  void setBrowsableProperty(string strPropertyName, bool bIsBrowsable)
    {
      // Get the Descriptor's Properties
      PropertyDescriptor theDescriptor = TypeDescriptor.GetProperties(this.GetType())[strPropertyName];

      // Get the Descriptor's "Browsable" Attribute
      BrowsableAttribute theDescriptorBrowsableAttribute = (BrowsableAttribute)theDescriptor.Attributes[typeof(BrowsableAttribute)];
      FieldInfo isBrowsable = theDescriptorBrowsableAttribute.GetType().GetField("Browsable", BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance);

      // Set the Descriptor's "Browsable" Attribute
      isBrowsable.SetValue(theDescriptorBrowsableAttribute, bIsBrowsable);
    }



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
          if (p.CanWrite)
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
                   if (p.PropertyType.IsEnum)
                  {
                      p.SetValue(o,  Enum.Parse(p.PropertyType,  node.Attributes["value"].InnerText,true),null); break;
                 }
                else
                  switch (p.PropertyType.ToString())
                  {
                    case "System.Boolean":
                      p.SetValue(o, (node.Attributes["value"].InnerText.ToUpper() == "TRUE"),null); break;
                    case "System.Double":
                        p.SetValue(o, Convert.ToDouble(node.Attributes["value"].InnerText), null);
                      break;
                    case "System.Int32":
                      p.SetValue(o, Int32.Parse(node.Attributes["value"].InnerText), null);
                      break;
                    case "System.String":
                      p.SetValue(o, node.Attributes["value"].InnerText, null);
                      break;

                    case "System.Drawing.Font":
                      System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
                      var values = Enum.GetValues(typeof(System.Drawing.FontStyle));
                      string attr = node.Attributes["style"].InnerText;
                      foreach (var v in values)
                        if (attr.IndexOf(v.ToString()) >= 0) style |= (System.Drawing.FontStyle)v;
                      p.SetValue(o, new Font(node.Attributes["value"].InnerText, float.Parse(node.Attributes["size"].InnerText), style), null);
                      break;
                    case "System.Drawing.Color":
                      value = node.Attributes["value"].InnerText;
                      p.SetValue(o, colorConverter.colorFromHex(value), null);
                      break;
                    case "YoctoVisualisation.doubleNan":
                      value = node.Attributes["value"].InnerText;
                      p.SetValue(o, new doubleNan(value), null);
                      break;

                    case "System.Windows.Media.FontFamily":
                      value = node.Attributes["value"].InnerText;
                      p.SetValue(o, new FontFamily(value),null);
                      break;

                    case "YColors.YColor":
                      value = node.Attributes["value"].InnerText;
                      p.SetValue(o, YColor.fromString(value), null);
                      break;


                    case "YoctoVisualisation.CustomYSensor":
                      value = node.Attributes["value"].InnerText;
                      if ((value.ToUpper() != "NULL") && (value != ""))

                      {
                        CustomYSensor s = sensorsManager.AddNewSensor(value);
                        p.SetValue(o, s, null);

                      }
                      else p.SetValue(o, sensorsManager.getNullSensor(), null);

                      break;
                    default:
                      throw new ArgumentException("Initialization from XML file: unhandled data type: (" + p.PropertyType.ToString() + ")");
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

    public enum BordersMode
    {
      [Description("Normal")]
      Sizable = System.Windows.Forms.FormBorderStyle.Sizable,
      [Description("Minimal")]
      SizableToolWindow = System.Windows.Forms.FormBorderStyle.SizableToolWindow,
      [Description("No border")]
      None = System.Windows.Forms.FormBorderStyle.None,
     
    };
   

    public static FormBorderStyle BordersModeFromString(string value)
    { value = value.ToUpper();
      FormBorderStyle res = FormBorderStyle.Sizable;
      foreach (BordersMode m in Enum.GetValues(typeof(BordersMode)))
      {  if (m.ToString().ToUpper() == value) res = (FormBorderStyle)(int)m;

      }
      return res;

    }


    private BordersMode _Form_FormBorderStyle = BordersMode.Sizable;
    [DisplayName("Borders"),
     CategoryAttribute("Window"),
     OnlyAvailableonAttribute(OnlyAvailableonAttribute.Windows),
     DescriptionAttribute("Window borders style. If you use \"none\" on a maximized window, you will get a kiosk mode.")]
    public BordersMode Form_FormBorderStyle
    {
      get { return _Form_FormBorderStyle; }
      set { _Form_FormBorderStyle = value; }
    }

    private YColor _Form_BackColor = YColor.FromArgb(0xff, 0xf0, 0xf0, 0xf0);
    [DisplayName("Background color"),
     CategoryAttribute("Window"),
    
     DescriptionAttribute("Window background color." + GenericHints.ColorMsg)]
    public YColor Form_BackColor
    {
      get { return _Form_BackColor; }
      set { _Form_BackColor = value; }
    }

    public string getXml(int deep)
    {
      return getXml(deep,this);
    }

    public string getXml(int deep, Object o)
    {
      string res = "";

      string value = "";
      foreach (System.Reflection.PropertyInfo p in o.GetType().GetProperties())
        if (p.CanWrite)
      {

        bool Mustsave = true;
        object[] attrs = p.GetCustomAttributes(true);
        foreach (object a in attrs)
          if (a is NotSavedInXMLAttribute)
            Mustsave = !((NotSavedInXMLAttribute)a).mustSave;

        var child = p.GetValue(o, null);


        if (IsStructured(child))
        { if (Mustsave)
          {
            res = res + new String(' ', 2 * deep) + "<" + p.Name + ">\r\n"
                     + getXml(deep + 1, child)
                     + new String(' ', 2 * deep) + "</" + p.Name + ">\r\n";
          }
         }
        else
        {
          string type = p.PropertyType.ToString();

        


          if (Mustsave)
          {
            if (p.PropertyType.IsEnum)
            {
               value = p.GetValue(o, null).ToString();

            } 
            else 
            switch (type)
            {
             

             
              case "System.Boolean": value = (bool)p.GetValue(o, null) ? "TRUE" : "FALSE"; break;
              case "System.Double":
              case "System.Int32":
              case "System.Windows.Media.FontFamily":
              case "System.String": value = System.Security.SecurityElement.Escape(p.GetValue(o, null).ToString()); break;
              case "System.Drawing.Color": value = colorConverter.colorToHex(((Color)p.GetValue(o, null))); break;
              case "YColors.YColor":
              case "YoctoVisualisation.doubleNan":
                  value = p.GetValue(o, null).ToString(); break;
                  
              case "YoctoVisualisation.CustomYSensor":
                CustomYSensor s = (CustomYSensor)p.GetValue(o, null);
                value = ((s == null) || (s is NullYSensor)) ? "NULL" : s.get_hardwareId();
                break;
              default:
                throw new ArgumentException("XML generation : unhandled type (" + type + ")");
            }
            res = res + new String(' ', 2 * deep) + "<" + p.Name + " value=\"" + value + "\"/>\n";
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
      if (o is FontFamily) return false;
      if (o is Font) return false;
      if (o is bool) return false;
     
      if (o is CustomYSensor) return false;
      if (o is YColor) return false;
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

    static public object getObjectFromPath(object rootTarget, List<string> path)
    {
      System.Reflection.PropertyInfo tinfo = null;
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

          FinalTarget = tinfo.GetValue(FinalTarget, null);
        }
        else
        {
          object targetArray = tinfo.GetValue(FinalTarget, null);
          string targetTypename = targetArray.GetType().FullName;
          if (targetArray is List<YAngularZone>)
            FinalTarget = ((List<YAngularZone>)targetArray)[int.Parse(index)];
          else
             if (targetArray is List<Zone>)
            FinalTarget = ((List<Zone>)targetArray)[int.Parse(index)];
          else
          if (targetArray is List<XAxis>)
            FinalTarget = ((List<XAxis>)targetArray)[int.Parse(index)];
          else
          if (targetArray is List<YAxis>)
            FinalTarget = ((List<YAxis>)targetArray)[int.Parse(index)];
          else
          if (targetArray is List<DataSerie>)
            FinalTarget = ((List<DataSerie>)targetArray)[int.Parse(index)];
          else
          if (targetArray is List<AnnotationPanel>)
            FinalTarget = ((List<AnnotationPanel>)targetArray)[int.Parse(index)];

          else
            FinalTarget = ((List<object>)targetArray)[int.Parse(index)];
        }
      }
      return FinalTarget;
    }
       

    static public void newSetProperty(object rootTarget, object source, string propertySourceName, List<string> path, PropFilter filterAllow)
    {
      System.Reflection.PropertyInfo tinfo = null;
      string ttype = "";


      object FinalTarget = getObjectFromPath(rootTarget, path);


      object TerminalSource = source;
      System.Reflection.PropertyInfo sinfo = null;

      sinfo = TerminalSource.GetType().GetProperty(propertySourceName);
      TerminalSource = sinfo.GetValue(TerminalSource,null);

      for (int i = 1; i < path.Count; i++)
      {
        sinfo = TerminalSource.GetType().GetProperty(path[i]);
        TerminalSource = sinfo.GetValue(TerminalSource,null);


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
      if (tinfo == null) return; // the property does not exists in the widget
      ttype = tinfo.PropertyType.FullName;




      if ((stype == "System.Drawing.Color") && (ttype == "System.Windows.Media.Color"))
      {
        Color c = (Color)TerminalSource;
        Color C2 = Color.FromArgb(c.A, c.R, c.G, c.B);
        tinfo.SetValue(FinalTarget, C2,null);
      }
      else

      if ((stype == "YoctoVisualisation.doubleNan") && (ttype == "System.Double"))
      {
        doubleNan v = (doubleNan)TerminalSource;
        tinfo.SetValue(FinalTarget, v.value, null);
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
        tinfo.SetValue(FinalTarget, new FontFamily(v),null);
      }


      else
        if ((stype == "System.Drawing.Color") && (ttype == "System.Windows.Media.Brush"))
      {
        Color c = (Color)TerminalSource;
        byte a = c.A;
        if (propertySourceName == "Graph2_ScrollBarFill") a = 80; // dirty hack to keep the navigator's cursor semi-transparent 
        Color C2 = Color.FromArgb(a, c.R, c.G, c.B);
       SolidBrush b = new SolidBrush(C2);
        tinfo.SetValue(FinalTarget, b,null);
      }
      else
      if ((stype == "YColors.YColor") && (ttype == "System.Drawing.Color"))
      {
          tinfo.SetValue(FinalTarget, ((YColor)TerminalSource).toColor(), null);
      }
      
      else
        if ((stype == "System.Drawing.Color") && (ttype == "System.Windows.Media.Brush"))
      {
        Color c = (Color)TerminalSource;
        byte a = c.A;
       // if (propertySourceName == "Graph2_ScrollBarFill") a = 80; // dirty hack to keep the navigator's cursor semi-transparent 
        Color C2 = Color.FromArgb(a, c.R, c.G, c.B);
        SolidBrush b = new SolidBrush(C2);
        tinfo.SetValue(FinalTarget, b,null);
      }
      else
    if ((stype == "YoctoVisualisation.DataTrackerDescription+DataPrecision") && (ttype == "YDataRendering.DataTracker+DataPrecision"))
      {  // Mono doesn't like enums implicit conversions
        YoctoVisualisation.DataTrackerDescription.DataPrecision svalue = (YoctoVisualisation.DataTrackerDescription.DataPrecision)TerminalSource;
        int temp = (int)svalue;
        YDataRendering.DataTracker.DataPrecision tvalue = (YDataRendering.DataTracker.DataPrecision)(temp);
        tinfo.SetValue(FinalTarget, tvalue, null);
        //int a = 1;
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
     if (sourceValue is AlarmSection)  return;  // dirty hack: alarms are no handled through reflexion

      if ((sourceValue is GenericProperties.BordersMode) && constants.MonoRunning)
        return;  // dirty hack: borders not supported in mono


      if (!IsStructured(sourceValue))
      {
        newSetProperty(rootTarget, this, fullpropname, path);
      }
      else
      {
        // map all target's user data to its mirrored source 
        List<String> path2 = new List<String>(path);
        path2.Add("");
        object target = getObjectFromPath(rootTarget, path2);
        PropertyInfo info = target.GetType().GetProperty("userData");
        if (info != null)
          info.SetValue(target, sourceValue, null);

       

        foreach (PropertyInfo p  in sourceValue.GetType().GetProperties())
         if (p.CanWrite)
          {
            path.Add(p.Name);
            ApplyProperties(rootSource, rootTarget, fullpropname, p.GetValue(sourceValue, null), path);
            path.RemoveAt(path.Count - 1);
          }

      }

    }

    public void ApplyAllProperties(object target, PropFilter filter)
    {
      System.Reflection.PropertyInfo[] props = this.GetType().GetProperties();
      foreach (var p in props)
        if (p.CanWrite)
      {
        string fullpropname = p.Name;
        int index = fullpropname.IndexOf("_");
        if (index < 0) throw new System.ArgumentException("invalid Property name");
        string propType = fullpropname.Substring(0, index);
        string propname = fullpropname.Substring(index + 1);
        // int arrayIndex = -1;
        //  propname = ExtractIndex(propname, ref arrayIndex);
        List<String> path = new List<String>();
        path.Add(propname);


        if ((target is Form) && (propType == "Form"))
          ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        if ((target is YDigitalDisplay) && (propType == "display"))
          ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);


        if ((target is YAngularGauge) && (propType == "AngularGauge"))
          ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        if ((target is YSolidGauge) && (propType == "SolidGauge"))
          ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        if ((target is YGraph) && (propType == "Graph"))
          ApplyProperties(this, target, fullpropname, p.GetValue(this, null), path);

        }
    }
  }

}
