
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  some global constant and command line arguments management
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using YDataRendering;

namespace YoctoVisualisation
{
  class constants
  {
    private static string _configfile = Path.Combine(Application.UserAppDataPath, "config.xml");
    private static bool _configfileOveridden = false;
    public static int MAXRAWDATAROWS = 2000;
    public static bool OpenLogWindowAtStartUp = false;
   
  
    public static YDataRenderer.CaptureFormats captureSizePolicy = YDataRenderer.CaptureFormats.Keep;
    public static YDataRenderer.CaptureTargets captureTarget = YDataRenderer.CaptureTargets.ToClipBoard;
    public static string captureFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    public static int captureWidth  = 1024;
    public static int captureHeight = 1024;
    public static int captureDPI = 70;


     public static  void getCaptureParametersCallback(YDataRenderer source,
                                              out YDataRenderer.CaptureTargets pcaptureTarget,
                                              out string pcaptureFolder,
                                              out YDataRenderer.CaptureFormats pcaptureSizePolicy,
                                              out int pcaptureDPI,
                                              out int pcaptureWidth,
                                              out int pcaptureHeight)
      {
      pcaptureTarget = captureTarget;
      pcaptureFolder = captureFolder;
      pcaptureSizePolicy = captureSizePolicy;
      pcaptureDPI = captureDPI;
      pcaptureWidth = captureWidth;
      pcaptureHeight = captureHeight;

    }





    public static string configfile
    { get { return _configfile; }  set { _configfile = value; _configfileOveridden = true; } }

    public static bool configfileOveridden
    { get { return _configfileOveridden; } } 

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
