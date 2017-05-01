
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Application entry point
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoctoVisualisation
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(String[] args)
    {
      string errmsg = "";
      constants.init(args);
      YAPI.RegisterLogFunction(LogManager.APIlog);

      if (YAPI.InitAPI(0, ref errmsg) == YAPI.SUCCESS)
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new StartForm());
      }
      else MessageBox.Show("Init error: " + errmsg);
    }
  }
}
