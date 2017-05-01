/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 * logs windows
 * 
 * 
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
        textBox1.Invoke((MethodInvoker)delegate
        {
          textBox1.AppendText(line + "\r\n");
        });
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
