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
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace YoctoVisualisation
{
  public partial class LogForm : Form
  {
    LogHandler h;

    public LogForm()
    {
     InitializeComponent();
      // Don't use SizableTool on OSX, it cannot hide properly
      if (constants.OSX_Running && this.FormBorderStyle == FormBorderStyle.SizableToolWindow)
      {
        this.FormBorderStyle = FormBorderStyle.Sizable;
      }
       h = new LogHandler(textBox1);
    }

    public void Log(string line)
    {
      LogNoTS(DateTime.Now.ToString("yyyy/MM/dd h:mm:ss.ff") + ' ' + line);

    }
    public void LogNoTS(string line)
    {
      h.log(line);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        e.Cancel = true;
        Hide();
      }
    }

    private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {

    }

    private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Clipboard.Clear();          
      Clipboard.SetText(textBox1.Text); 
    }
  }



  public class LogHandler
  {
    List<String> buffer;
    TextBox _logWindow;
    public Mutex Lock = new Mutex();
    System.Windows.Forms.Timer t;

    public LogHandler(TextBox logWindow)
    {
      _logWindow = logWindow;
      buffer = new List<String>();
      t = new System.Windows.Forms.Timer();
      t.Interval = 200;
      t.Tick += T_Tick;
      t.Enabled = true;


    }

    private void T_Tick(object sender, EventArgs e)
    {
      refresh();
    }

    public void log(string msg)
    {
     
      Lock.WaitOne();
      buffer.Add(msg+"\r\n");
      Lock.ReleaseMutex();
    }


    private void refresh()
    {
      if (buffer.Count <= 0) return;
      Lock.WaitOne();
      while (buffer.Count > 0)
      {
        try { _logWindow.AppendText(buffer[0]); }  catch (Exception) { }
        buffer.RemoveAt(0);
      }
      Lock.ReleaseMutex();
      if (_logWindow.Lines.Length >1000)
      {
        List<string> lines = new List<string>(_logWindow.Lines);
        lines.RemoveRange(0, 100);
        this._logWindow.Lines = lines.ToArray();
      }
    }

  }




static class LogManager
  {
    static LogForm w = null; 

    static LogManager()
    { w = new LogForm();

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
