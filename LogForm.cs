/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 * logs windows
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
using System.Windows.Forms;

namespace YoctoVisualisation
{
  public partial class LogForm : Form
  {
    public LogForm()
    {
      InitializeComponent();
      // Don't use SizableTool on OSX, it cannot hide properly
      if (constants.OSX_Running && this.FormBorderStyle == FormBorderStyle.SizableToolWindow)
      {
        this.FormBorderStyle = FormBorderStyle.Sizable;
      }
    }

    public void Log(string line)
    {
      LogNoTS(DateTime.Now.ToString("yyyy/MM/dd h:mm:ss.ff") + ' ' + line);

    }
    public void LogNoTS(string line)
    {
      try
      {
        textBox1.AppendText(line + "\r\n");
      }
      catch (Exception)
      {
        try
        {
          textBox1.Invoke((MethodInvoker)delegate
          {
            textBox1.AppendText(line + "\r\n");
          });
        }
        catch (Exception) { };
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        e.Cancel = true;
        Hide();
      }
    }
  }

  static class LogManager
  {
    static LogForm w = new LogForm();

    static LogManager()
    {
      w = new LogForm();

    }

    static public void APIlog(string line)
    {
      w.Log("(API)" + line);

    }

    static public void Log(string line)
    {
      w.Log(line);

    }
    static public void LogNoTS(string line)
    {
      w.LogNoTS(line);
    }
    static public void Show()
    {
      w.Show();

    }





  }

}
