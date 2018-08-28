using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace YoctoVisualization
{
  public static class XMLConfigTranslator
  {
    public static string TranslateFromV1(XmlDocument doc)
    {
      string XmlConfigFile = "<?xml version=\"1.0\" ?>\n<ROOT version='2'>\n";
      foreach (XmlNode node in doc.DocumentElement.ChildNodes)
      { switch (node.Name)
        {
          case "Config": 
          case "PropertiesForm": 
          case "Sensors": XmlConfigFile += ConfigTranslate(node); break;
          case "angularGaugeForm": XmlConfigFile += AngularGaugeTranslate(node); break;
          case "GaugeForm": XmlConfigFile += GaugeTranslate(node); break;
          case "digitalDisplayForm": XmlConfigFile += DigitalDisplayTranslate(node); break;
          case "GraphForm": XmlConfigFile += GraphTranslate(node); break;


        }
      }


      XmlConfigFile += "</ROOT>\n";

     // System.IO.File.WriteAllText("c:\\tmp\\config.txt", XmlConfigFile);

      

      return XmlConfigFile;


    }

    static string ConfigTranslate(XmlNode node)
    {


      return DumpNode(node, 1);

    }


    static string GraphTranslate(XmlNode node)
    {
      string res = "  <GraphForm>\n";
      foreach (XmlNode n in node.ChildNodes)

      {
        string prefix = n.Name.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });

        if (prefix == "Graphs_Series") res += SeriesTranslate(n, 2);
        else  if (prefix == "Graph_AxisY") res += yAxisTranslate(n, 2);

        else
          switch (n.Name)
          {
            case "location":
            case "size":
            case "Form_Text":
            case "Graph_showRecordedData":
            case "Form_BackColor": res += DumpNode(n, 2); break;
            case "Graph_AxisX0": res += xAxisTranslate( n,  2);break;
            case "Graph_showNavigator": res +=navigatorTranslate(n, 2); break;
            case "Graph_BackColor": res += renameNode(n, "Graph_bgColor1", 2);
                                    res += renameNode(n, "Graph_bgColor2", 2); break;
             

          }
      }
      return res + "  </GraphForm>\n";
    }

    
    static string navigatorTranslate(XmlNode node, int indent)
    {
      string indentation = new String(' ', 2 * indent);
      string res = indentation + "<Graph_navigator>\n";
      res+= indentation + "  <enabled value = \""+node.Attributes["value"].Value+ "\" />\n";
      return  res + indentation + "</Graph_navigator>\n";
    }

    static string SeriesTranslate(XmlNode node, int indent)
    {
      string indentation = new String(' ', 2 * indent);
      string prefix = node.Name.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
      string res = indentation + "<Graph_series" + node.Name.Substring(prefix.Length) + ">\n";

      foreach (XmlNode n in node.ChildNodes)
        switch (n.Name)
        { case "DataSource_source":
          case"DataSource_datatype":
                 res += DumpNode(n, indent + 1); break;
          case "StrokeThickness": res += renameNode(n, "thickness", indent + 1); break;
          case "Title": res += renameNode(n, "legend", indent + 1); break;
          case "Stroke": res += renameNode(n, "color", indent + 1); break;
          case "ScalesYAt": res += renameNode(n, "yAxisIndex", indent + 1); break;
        }
     
      return res + indentation + "</Graph_series" + node.Name.Substring(prefix.Length) + ">\n";
    }

    static string xAxisTranslate(XmlNode node, int indent)
    {
      string indentation = new String(' ', 2 * indent);
    
      string res = indentation + "<Graph_xAxis>\n";

      String fontname = "Arial";
      String fontsize = "10";
      String color = "Black";
      String GridEnabled   = "FALSE";
      string GridColor     = "LightGray";
      string GridThickness = "1";
      string Title = "";

      foreach (XmlNode n in node.ChildNodes)
      {  switch (n.Name)
          {
            case "initialZoom": res += DumpNode(n, 2); break;

            case "Foreground": color = n.Attributes["value"].Value;break;
             case "FontSize": fontsize = n.Attributes["value"].Value; break;
             case "FontFamily": fontname = n.Attributes["value"].Value; break;
             case "Title": Title = n.Attributes["value"].Value; break;



            case "Separator" : GridEnabled = n.Attributes["value"].Value == "Disabled" ?"FALSE":"TRUE";
                               GridColor= n.Attributes["Color"].Value;
                               GridThickness = n.Attributes["Thickness"].Value;
                               break;

       
        }
      }

 
      res = res + indentation + "  <Legend>\n"
                + indentation + "      <title value=\""+ Title + "\"/>\n"
                + indentation + "  </Legend>\n"
                + indentation + "  <font>\n"
                + indentation + "      <name value=\"" + fontname + "\"/>\n"
                + indentation + "      <size value=\"" + (Double.Parse(fontsize) * 0.75).ToString()+ "\"/>\n"
                + indentation + "      <color value=\"" + color + "\"/>\n"
                + indentation + "  </font>\n"
                + indentation + "  <showGrid value=\"" + GridEnabled + "\"/>\n"
                + indentation + "  <gridColor value=\"" + GridColor + "\"/>\n"
                + indentation + "  <gridThickness value=\"" + GridThickness + "\"/>\n"
                + indentation + "  <color value=\"" + color + "\"/>\n";

      return res + indentation + "</Graph_xAxis>\n";
    }


    static string yAxisSectionTranslate(XmlNode node, int indent)
    {
      string indentation = new String(' ', 2 * indent);
      string prefix = node.Name.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
      string res = indentation + "<zones" + node.Name.Substring(prefix.Length) + ">\n";
      double Value = 0;
      double SectionWidth = 0;
      foreach (XmlNode n in node.ChildNodes)
        switch (n.Name)
        {
          case "Value": Value = Double.Parse(n.Attributes["value"].Value); break;
          case "SectionWidth": SectionWidth = Double.Parse(n.Attributes["value"].Value); break;
          case "Fill": res += renameNode(n, "color", indent + 1); break;
        }
      res += indentation + "  <visible value =\"" + ((SectionWidth > 0) ? "TRUE" : "FALSE") + "\"/>\n";
      res += indentation + "  <min value=\"" + Value.ToString() + "\"/>\n";
      res += indentation + "  <max value=\"" + (Value + SectionWidth).ToString() + "\"/>\n";
      return res + indentation + "</zones" + node.Name.Substring(prefix.Length) + ">\n";
    }


    static string yAxisTranslate(XmlNode node, int indent)
    {
      string indentation = new String(' ', 2 * indent);

      string prefix = node.Name.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
      string res = indentation + "<Graph_yAxes" + node.Name.Substring(prefix.Length) + ">\n";

      String fontname = "Arial";
      String fontsize = "10";
      String color = "Black";
      String GridEnabled = "FALSE";
      string GridColor = "LightGray";
      string GridThickness = "1";
      string Title = "";

      foreach (XmlNode n in node.ChildNodes)
      {
        string prefix2 = n.Name.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
        if (prefix2 == "Sections") res += yAxisSectionTranslate(n, 2);
        else
        switch (n.Name)
        {
          case "Foreground": color = n.Attributes["value"].Value; break;
          case "FontSize": fontsize = n.Attributes["value"].Value; break;
          case "FontFamily": fontname = n.Attributes["value"].Value; break;
          case "Title": Title = n.Attributes["value"].Value; break;
          case "Separator":
            GridEnabled = n.Attributes["value"].Value == "Disabled" ? "FALSE" : "TRUE";
            GridColor = n.Attributes["Color"].Value;
            GridThickness = n.Attributes["Thickness"].Value;
            break;


        }
      }

      foreach (XmlNode n in node.ChildNodes)

      {

        switch (n.Name)
        {
         
          case "MinValue": res += renameNode(n, "min", 2); break;
          case "MaxValue": res += renameNode(n, "max", 2); break;
          case "ShowLabels":res += renameNode(n, "visible", 2); break;

        }
      }

      res = res + indentation + "<Legend>\n"
                + indentation + "    <title value=\"" + Title + "\"/>\n"
                + indentation + "</Legend>\n"
                + indentation + "<font>\n"
                + indentation + "    <name value=\"" + fontname + "\"/>\n"
                + indentation + "    <size value=\"" + (Double.Parse(fontsize) * 0.75).ToString() + "\"/>\n"
                + indentation + "    <color value=\"" + color + "\"/>\n"
                + indentation + "</font>\n"
                + indentation + "<showGrid value=\"" + GridEnabled + "\"/>\n"
                + indentation + "<gridColor value=\"" + GridColor + "\"/>\n"
                + indentation + "<gridThickness value=\"" + GridThickness + "\"/>\n"
                + indentation + "<color value=\"" + color + "\"/>\n";

      return res + indentation + "</Graph_yAxes" + node.Name.Substring(prefix.Length) + ">\n";
    }


    static string DigitalDisplayTranslate(XmlNode node)
    {
      string res = "  <digitalDisplayForm>\n";

      string color = "RGB:FF000000";
      foreach (XmlNode n in node.ChildNodes)
        if (n.Name == "Label_ForeColor") color = n.Attributes["value"].Value;
        
      foreach (XmlNode n in node.ChildNodes)

      {
       
          switch (n.Name)
          {
            case "location":
            case "size":
            case "Form_Text":
            case "Form_BackColor":
            case "DataSource_precision":
            case "DataSource_source": res += DumpNode(n, 2); break;
            case "Label_Font":        res += translateFont(n, "display_font", color, 2); break;
            case "Label_MinValue":    res += renameNode(n, "display_outOfRangeMin", 2); break;
            case "Label_MaxValue":    res += renameNode(n, "display_outOfRangeMax", 2); break;
            case "Label_OORColor":    res += renameNode(n, "display_outOfRangeColorColor", 2); break;
          }
      }
      res += "    <display_backgroundColor1 value = \"RGB:00000000\" />\n"
           + "    <display_backgroundColor2 value = \"RGB:00000000\" />\n";

      return res + "  </digitalDisplayForm>\n";
    }


    static  string GaugeTranslate(XmlNode node)
    { string res = "  <GaugeForm>\n";
      foreach (XmlNode n in node.ChildNodes)

      {
       
          switch (n.Name)
          {
            case "location":
            case "size":
            case "Form_Text":
            case "Form_BackColor":
            case "DataSource_precision": 
            case "DataSource_source": res += DumpNode(n, 2); break;
            case "SolidGauge_From": res += renameNode(n, "SolidGauge_min", 2); break;
            case "SolidGauge_To": res += renameNode(n, "SolidGauge_max", 2); break;
            case "SolidGauge_FromColor": res += renameNode(n, "SolidGauge_color1", 2); break;
            case "SolidGauge_ToColor": res += renameNode(n, "SolidGauge_color2", 2); break;
            case "SolidGauge_Uses360Mode": res += "    <SolidGauge_displayMode value=\"" + (n.Attributes["value"].Value == "TRUE" ? "DISPLAY360" : "DISPLAY180") + "\"/>\n";break;
            case "SolidGauge_Font": res += translateFont(n, 2); break;
            case "SolidGauge_Stroke": res += renameNode(n, "SolidGauge_borderColor", 2); break;
            case "SolidGauge_StrokeThickness": res += renameNode(n, "SolidGauge_borderThickness", 2); break;
            case "SolidGauge_GaugeBackground": res += renameNode(n, "SolidGauge_backgroundColor1", 2);
                                               res += renameNode(n, "SolidGauge_backgroundColor2", 2);
                                               break;

            

          }
        }

        res += "    <SolidGauge_thickness value = \"40\" />\n";
        return res + "  </GaugeForm>\n";
    }


    static string AngularGaugeSectionTranslate(XmlNode node, int indent)
    {
      string indentation = new String(' ', 2 * indent);
      string prefix = node.Name.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
      string res = indentation + "<AngularGauge_zones" + node.Name.Substring(prefix.Length) + ">\n";
      double start = 0;
      double to = 0;
      foreach (XmlNode n in node.ChildNodes)
        switch (n.Name)
        {
          case "FromValue": start = Double.Parse(n.Attributes["value"].Value); res += renameNode(n, "min", indent + 1); break;
          case "ToValue": to = Double.Parse(n.Attributes["value"].Value); res += renameNode(n, "max", indent + 1); break;
          case "Fill": res += renameNode(n, "color", indent + 1); break;
        }
      res += indentation + "  <visible value=\"" + ((start != to) ? "TRUE" : "FALSE") + "\"/>\n";
      return res + indentation + "</AngularGauge_zones" + node.Name.Substring(prefix.Length) + ">\n";
    }


    static string  AngularGaugeTranslate(XmlNode node)
    {
      string res = "  <angularGaugeForm>\n";
      foreach (XmlNode n in node.ChildNodes)

      {
        string prefix = n.Name.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
        
        if (prefix == "AngularGauge_Sections") res += AngularGaugeSectionTranslate(n, 2);
        else
          switch (n.Name)
          {
            case "location":
            case "size":
            case "Form_Text":
            case "Form_BackColor":
            case "DataSource_source": res += DumpNode(n, 2); break;
            case "SolidGauge_FromValue": res += renameNode(n, "AngularGauge_min", 2); break;
            case "AngularGauge_ToValue": res += renameNode(n, "AngularGauge_max", 2); break;
            case "AngularGauge_LabelsStep": res += renameNode(n, "AngularGauge_graduation", 2); break;
            case "AngularGauge_TicksStrokeThickness": res += renameNode(n, "AngularGauge_graduationThickness", 2);
                                                       res += renameNode(n, "AngularGauge_subgraduationThickness", 2);
                                                     break;
            case "AngularGauge_TicksForeground": res += renameNode(n, "AngularGauge_graduationColor", 2);
                                                  res += renameNode(n, "AngularGauge_subgraduationColor", 2);
                                                  break;
            case "AngularGauge_NeedleFill": res += renameNode(n, "AngularGauge_needleColor", 2); break;

          }
      }

      res += "    <AngularGauge_graduationFont >\n"
          + "       <name value = \"Arial\" />\n"
          + "        <size value = \"10\" />\n"
          + "        <color value = \"Black\" />\n"
          + "        <italic value = \"FALSE\" />\n"
          + "        <bold value = \"TRUE\" />\n"
          + "     </AngularGauge_graduationFont>\n"
          + "   <AngularGauge_needleWidth value = \"10\"/>\n"
          + "   <AngularGauge_borderThickness value = \"1\"/>\n"
          + "   <AngularGauge_borderThickness value = \"1\"/>\n"
          + "   <AngularGauge_backgroundColor1 value = \"RGB:00000000\"/>\n"
          + "   <AngularGauge_backgroundColor2 value = \"RGB:00000000\"/>\n"
          + "   <AngularGauge_borderColor value = \"DarkGray\"/>\n"
          + "   <AngularGauge_needleContourThickness value = \"0\"/>\n";
      return res + "  </angularGaugeForm>\n";
    }

    static string translateFont(XmlNode n,  int indent)
    { return translateFont(n, n.Name, "", indent); }

    static string translateFont (XmlNode n, string newname, string color, int indent)
    {
       string indentation = new String(' ', 2 * indent);
       string res = indentation + "<" + newname + ">\n";
       res += indentation + "  <name value=\"" + n.Attributes["value"].Value + "\"/>\n";
       res += indentation + "  <size value=\"" + n.Attributes["size"].Value + "\"/>\n";
       if (color!="") res += indentation + "  <color value=\"" + color + "\"/>\n";
      res += indentation + "  <italic value=\"" + (n.Attributes["style"].Value.Contains("Italic")?"TRUE":"FALSE") + "\"/>\n";
       res += indentation + "  <bold value=\"" + (n.Attributes["size"].Value.Contains("Italic") ? "TRUE" : "FALSE")+ "\"/>\n";

       return res + indentation+ "</" + newname + ">\n"; ;

    }

    static string renameNode(XmlNode n, string newname, int indent)
    {
       return new String(' ', 2 * indent)+ "<"+newname+" value=\""+ System.Security.SecurityElement.Escape(n.Attributes["value"].Value) + "\"/>\n";
    }

    static string DumpNode(XmlNode n, int indent)
    { return DumpNode( n, n.Name, indent); }

    static string DumpNode(XmlNode n, string newName, int indent)
    {  string indentation = new String(' ', 2*indent);
       string res = indentation + "<"+ newName;
       foreach (XmlAttribute a in n.Attributes)
        { res += ' ' + a.Name + "=\"" + System.Security.SecurityElement.Escape(a.Value) + "\"";
        }
      if (n.ChildNodes.Count == 0) res += "/>\n";
      else
      {
        res += ">\n";
        foreach (XmlNode s in n.ChildNodes) res += DumpNode(s, indent + 1);
        res += indentation + "</" + newName + ">\n";
      }


       

      return res;
    }



  }
}
