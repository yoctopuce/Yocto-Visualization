using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using YoctoVisualisation;

namespace YoctoVisualization
{
  public partial class HubEdit : Form
  {
    bool ok ;

    public HubEdit()
    {
      InitializeComponent();
    }

    public bool newHub(out Hub h)
    {
      Text = "New hub connection";
      ok = false;
      protocolChooser.SelectedIndex = 0; protocolChooser.BackColor = Color.White;
      addressChooser.Text = ""; addressChooser.BackColor = Color.White;
      portChooser.Text = ""; portChooser.BackColor = Color.White;
      pathChooser.Text = ""; pathChooser.BackColor = Color.White;
      usernameChooser.Text = ""; usernameChooser.BackColor = Color.White;
      passwordChooser.Text = ""; passwordChooser.BackColor = Color.White;

      new InputFieldManager(addressChooser, InputFieldManager.dataType.DATA_STRING, false, Double.NaN, Double.NaN, null);
      new InputFieldManager(portChooser, InputFieldManager.dataType.DATA_STRICT_POSITIVE_INT, true, Double.NaN, Double.NaN, null);


      ShowDialog();
      if (ok)
       {
        h = new Hub(Hub.HubType.REMOTEHUB,
                    protocolChooser.SelectedIndex == 0 ? "ws" : "http",
                    usernameChooser.Text, passwordChooser.Text,
                    addressChooser.Text, portChooser.Text,pathChooser.Text);
       } else h = null;




      return ok;
    }

    public bool editHub(ref Hub h)
    {
      Text = "Edit hub connection";
      protocolChooser.SelectedIndex = h.protocol == "ws" ?0:1; protocolChooser.BackColor = Color.White;
      addressChooser.Text =h.addr; addressChooser.BackColor = Color.White;
      portChooser.Text = h.port; portChooser.BackColor = Color.White;
      pathChooser.Text = h.path; pathChooser.BackColor = Color.White;
      usernameChooser.Text = h.user; usernameChooser.BackColor = Color.White;
      passwordChooser.Text = h.clearPassword; passwordChooser.BackColor = Color.White;
      ShowDialog();
      if (ok)
      {
        


        h = new Hub(Hub.HubType.REMOTEHUB,
                   protocolChooser.SelectedIndex == 0 ? "ws" : "http",
                   usernameChooser.Text, passwordChooser.Text,
                   addressChooser.Text, portChooser.Text, pathChooser.Text);

      }
      return ok;
    }


    private void button1_Click(object sender, EventArgs e)
    {
      if (addressChooser.Text.Trim() =="")
      {
        MessageBox.Show("Address cannot be empty.");
        return;
      }


      if ((addressChooser.Text.Trim() == "127.0.0.1")
           && (usernameChooser.Text.Trim() == "")
           && (passwordChooser.Text.Trim() == "")

           && (portChooser.Text.Trim() == "")
           && (pathChooser.Text.Trim() == ""))
      {
        MessageBox.Show("Use the check-box above the connection list to use a local VirtualHub with default parameters.");
        return;
      }


      ok = true; Close();

    }

    private void button2_Click(object sender, EventArgs e)
    {
      ok = false; Close();
    }
  }
}
