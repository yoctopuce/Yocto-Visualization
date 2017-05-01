
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  some global constant and command line arguments management
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace YoctoVisualisation
{
  class constants
  {
    public static string configfile = Path.Combine(Application.UserAppDataPath, "config.xml");
    public static int MAXRAWDATAROWS = 2000;
    public static bool OpenLogWindowAtStartUp = false;

    public static void init(String[] args)
    {
      for (int i = 0; i < args.Length; i++)
      {
        if ((args[i] == "-config") && (i < args.Length - 1))
        { configfile = args[i + 1]; i++; }

        if ((args[i] == "-log")) OpenLogWindowAtStartUp = true;

      }

      string path = Path.GetDirectoryName(configfile);
      if (!Directory.Exists(path))
        MessageBox.Show("Warning: Path " + path + " doesn't exist\nChanges won't be saved.");


    }

    // ...
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)  // thanks stackoverflow
    {
      // Unix timestamp is seconds past epoch
      System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
      dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
      return dtDateTime;
    }

  }
}
