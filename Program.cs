
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Application entry point
 * 
 *  - - - - - - - - - License information: - - - - - - - - -
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
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;

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




      if (constants.CheckMonoVersion(out errmsg))
      {
        int res = yAPI.FILE_NOT_FOUND;

        res = YAPI.InitAPI(0, ref errmsg);
        YAPI.RegisterLogFunction(LogManager.APIlog);

        if (res == YAPI.SUCCESS)
        {
          YAPI.SetDeviceListValidity(600);
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);
          try  { Application.Run(new StartForm());}
          catch (OutOfMemoryException)
          {
            string msg = ("Yocto-Visualization ran out of memory");
            Console.Write(msg);
            MessageBox.Show(msg);
          }
          catch (Exception e)
          {
            string msg = "Yocto-Visualization raised an exception:\r\n" + e.Message + "\r\n" + e.StackTrace.ToString();
            Console.Write(msg);
            MessageBox.Show(msg);
          }
        }
        else MessageBox.Show("Init error: " + errmsg);
      }
      else MessageBox.Show(errmsg);
    }
  }
}
