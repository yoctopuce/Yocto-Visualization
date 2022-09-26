
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

using System.Xml;
using System.Windows.Forms;
using System.IO;
using YDataRendering;
using System.Reflection;


namespace YoctoVisualisation 
{
  class constants
  {
    public static string appVersion = "2.0";
    public static string buildVersion = "51050";
    private static string _configfile = Path.Combine(Application.UserAppDataPath, "config.xml");
    private static bool _configfileOveridden = false;
    public static int MAXRAWDATAROWS = 2000;
    public static bool OpenLogWindowAtStartUp = false;

    // note : automatic check for updates is not implemented on the GitHub version.
    private static bool _forceCheckForUpdate = false;
    private static bool _checkforUpdate = true; 
    public static bool checkForUpdate { get {return _checkforUpdate || _forceCheckForUpdate; } set { _checkforUpdate = value; } }   
    private static int _UpdateIgnoreBuild = 0;
    public static int updateIgnoreBuild { get { return _UpdateIgnoreBuild; } set { _UpdateIgnoreBuild = value; } }


    public  static string  loginCypherPassword = "YouShouldReallyChangeThis!";
    public static YDataRenderer.CaptureFormats captureSizePolicy = YDataRenderer.CaptureFormats.Keep;
    public static YDataRenderer.CaptureTargets captureTarget = YDataRenderer.CaptureTargets.ToClipBoard;
    public static YDataRenderer.CaptureType captureType = YDataRenderer.CaptureType.PNG;

    public static string captureFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    public static int captureWidth  = 1024;
    public static int captureHeight = 1024;
    public static int captureDPI = 70;

    // these constants are used to filter out measures with inconsistant timestamps
    // which would ruin Graphs rendering.
    public static bool checkForSuspiciousTimestamps = false;
    public const   int CredibleAppStartYear  = 2000; // if current date at start time is within this 
    public const   int CredibleAppStartEnd   = 2038; // range, then we can activate filter
    public static  int CredibleTimestampStart  ;
    public static  int CredibleTimestampEnd    ;
    private static int CredibleTimestampDelta = 10;  // +/- 5 year around app start date




    public static int maxPointsPerGraphSerie = 0;
    public static int maxPointsPerDataloggerSerie = 0;
    public static int maxDataRecordsPerSensor = 0;
    public static bool dbleClickBringsUpContextMenu = false;

    public static bool _OSX_Running = (Environment.OSVersion.Platform == PlatformID.Unix &&
                                       Directory.Exists("/Applications") && Directory.Exists("/System") &&
                                       Directory.Exists("/Users") && Directory.Exists("/Volumes"));
    public static bool OSX_Running { get { return _OSX_Running; } }
    private static bool  _MonoRunning = (Type.GetType ("Mono.Runtime") != null);
		public static bool MonoRunning {get {return  _MonoRunning;}}
    private static string MonoMinVersion { get { return "4.0"; }} 

    static constants()
    {
      int y = DateTime.Now.Year;
      constants.checkForSuspiciousTimestamps = (y >= CredibleAppStartYear) && (y < CredibleAppStartEnd) ;
      constants.CredibleTimestampStart = y - (CredibleTimestampDelta >>1);
      constants.CredibleTimestampEnd   = y+ (CredibleTimestampDelta >> 1);

  }

    public static string MonoVersion
    {
      get
      {
        Type type = Type.GetType("Mono.Runtime");
        if (type != null)
        {
          MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
          if (displayName != null)
            return displayName.Invoke(null, null).ToString();
          else
            return "unknown";
        }
        return "Not in Mono";
      }
    }

    public static string DumpException(Exception e)
    {
      string UserMsg = "";
      string contentsDump = "";
      string InnerMessage = "";

      while (e!=null)
       {
        InnerMessage = e.Message;
        UserMsg = e.Message + "\r\n" + UserMsg;
         contentsDump = "<b>" + e.Message + "</b><br><tt>" + e.StackTrace.ToString().Replace("\n", "<br>\n") + "</tt>\r\n";
         e = e.InnerException;


       }

      string filename = Path.Combine(Application.UserAppDataPath, "YoctoVisualization.last.crash.html");
      UserMsg = "Yocto-Visualization raised an exception: "
                + UserMsg
                + "\r\nDetails about this mishap has been saved in:\r\n\r\n"
                + filename + "\r\n\r\n"
                + "Should this happen again, don't hesitate to contact Yoctopuce support.";


      contentsDump = "<HTML><BODY>"
      + "<h1>YoctoVisualisation Exception</h1>\r\n"
      + "<hr><br><b>" + InnerMessage + "</b><hr><br>\r\n"
      + "Occurred on : " + DateTime.Now.ToString("G") + "<br>\n\n"
      + "Build version : " + buildVersion + "<br>\r\n"
      + "Running on :" + (MonoRunning ? MonoVersion : ".NET") + "<br>\r\n"
      + "<HR/>"
      + "<h2>DUMP (inner first)</h2>\r\n"
      + contentsDump
      + "</BODY></HTML>\r\n";

      File.WriteAllText(filename, contentsDump);
      return(UserMsg);


    }

    public static bool CheckMonoVersion(out string errmsg)
    {
       errmsg = "";
       if (!MonoRunning) return true;
       string[] requirement = MonoMinVersion.Split(new[] { "." }, StringSplitOptions.None);
       string[] value = MonoVersion.Split(new[] { " " }, StringSplitOptions.None)[0].Split(new[] { "." }, StringSplitOptions.None);
       for (int i=0;i< Math.Min(requirement.Length, value.Length); i++)
       {
        if  (Int32.Parse(value[i])< Int32.Parse(requirement[i]))
        {
          errmsg = "Yocto-Visualization requires at least Mono " + MonoMinVersion + ", installed version is " + MonoVersion;
          return false;
        }

       }


       return true;


    }


    public static  void getCaptureParametersCallback(YDataRenderer source,
                                              out YDataRenderer.CaptureType pcapturetype,
                                              out YDataRenderer.CaptureTargets pcaptureTarget,
                                              out string pcaptureFolder,
                                              out YDataRenderer.CaptureFormats pcaptureSizePolicy,
                                              out int pcaptureDPI,
                                              out int pcaptureWidth,
                                              out int pcaptureHeight)
      {
      pcapturetype = captureType;
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
      bool configFileOverride = false;
     
      if (_MonoRunning) captureTarget = YDataRenderer.CaptureTargets.ToFile;

      



      for (int i = 0; i < args.Length; i++)
      { 
        if ((args[i] == "-config") && (i < args.Length - 1))
        { configfile = args[i + 1]; i++;
          configFileOverride = true;
        }

        if ((args[i] == "-log")) OpenLogWindowAtStartUp = true;

        if (args[i] == "-check4updates")
           _forceCheckForUpdate = true;

      }

      string path = Path.GetDirectoryName(configfile);
      if (!Directory.Exists(path))
        MessageBox.Show("Warning: Path " + path + " doesn't exist\nChanges won't be saved.");

      if (!configFileOverride) CheckForPreviousConfig(appVersion);
    }

    private static void CheckForPreviousConfig(string version)
    {
      if (!File.Exists(_configfile))
      {
        //look if there is a previous applications setting in the UserAppDataPath directory
        DirectoryInfo dir = Directory.GetParent(Application.UserAppDataPath);
        DirectoryInfo[] directories = dir.GetDirectories(version + ".*.*");
        int max = 0;
        string previous = version + ".0.0";
        foreach (DirectoryInfo dirinfo in directories)
        {
          if (dirinfo.Name == Application.ProductVersion)
          {
            continue;
          }

          string[] parts = dirinfo.Name.Split(".".ToCharArray());
          try
          {
            int b = Int32.Parse(parts[2]);
            if (b > max)
            {
              max = b;
              previous = dirinfo.Name;
            }
          }
          catch (Exception) { }
        }

        string previousPath = Path.Combine(dir.FullName, previous);
        string previousSettings = Path.Combine(previousPath, "config.xml");
        if (File.Exists(previousSettings))
        {
          File.Copy(previousSettings, _configfile);
        }
      }
    }

    // ...
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)  // thanks Stackoverflow
    {
      // Unix time-stamp is seconds past epoch
      System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
      dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
      return dtDateTime;
    }

  }


  }
