using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YoctoVisualisation
{

 

  public partial class PropertiesForm2 : Form
  {

    Dictionary<object, EditedDataSource> EditedDataSourceList = new Dictionary<object, EditedDataSource>();
    EditedDataSource currentEditedDataSource = null;
    SetValueCallBack2 _changeCallback = null;
    GetValueCallBack2 _getvalueCallback = null;

    class EditedDataSource
    {
      object _source;
      UIElement _root;

      public EditedDataSource(object source, UIElement root)
      {
        _source = source;
        _root = root;
      }

      public UIElement root { get { return _root; }   }

      public void refresh()
      {  if (_root != null) _root.refresh();
      }

    }

    // a way to keep the properties editor on top of
    // others windows only when it matters
    public void setTopmost(bool state)
    { if (state)
      {
        if (!Visible) return;
        if (currentEditedDataSource == null) return;
      }
      if (state!= TopMost) TopMost = state;
    }


    public PropertiesForm2()
    {   
      InitializeComponent();

      // Don't use SizableTool on OSX, it cannot hide properly
      if (constants.OSX_Running && this.FormBorderStyle == FormBorderStyle.SizableToolWindow)
      {
        this.FormBorderStyle = FormBorderStyle.Sizable;
      }

      panel1.AutoSize = false;
     // TopMost = true;
    
      if (constants.OSX_Running)
      { // awful clipping problems in the Mac version
        outterpanel.Top += panel2.Height + 4;
        panel2.Top = 2;
        panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      }
    }

    public void refresh()
      {  if (!Visible) return;
         if (currentEditedDataSource == null) return;
         currentEditedDataSource.refresh();
      }

    public void showWindow(Form hostWindow,  GenericProperties structData, 
                           SetValueCallBack2 setvalueCallBack,
                           GetValueCallBack2 getvalueCallBack, bool ForceToTshow)
    {

      _changeCallback = setvalueCallBack;

      _getvalueCallback = getvalueCallBack;

        Text = "Properties of "+ hostWindow.Text;
  

      if (ForceToTshow)
      {
        Show();
        if (this.WindowState == FormWindowState.Minimized)
        {
          this.WindowState = FormWindowState.Normal;
        }
      }
      EditObject(structData, setvalueCallBack, getvalueCallBack);
    }

    public void EditObject(object structData, SetValueCallBack2 setvalueCallBack, GetValueCallBack2 getvalueCallBack)
    {
      string helpYourselfMsg = "This is the property editor. Change any parameter you want. All changes are applied in real time.";
      if (!Visible) return;
      HelpZone.Text = "Please wait....";
      HelpZone.Refresh();

      //Console.WriteLine(" -> edit");

      if (EditedDataSourceList.ContainsKey(structData))
        if (EditedDataSourceList[structData] == currentEditedDataSource)
        {
          HelpZone.Text = helpYourselfMsg;
          return;
        }

      if (currentEditedDataSource != null) currentEditedDataSource.root.stopEdit();

      UIElement it;

      if (!EditedDataSourceList.ContainsKey(structData))
      {
        UIElementBaseParams rootparam = new UIElementBaseParams(panel1, HelpZone, null, null, "root", "Root", "root node","");
        it = new UIElement(rootparam);
        currentEditedDataSource = new EditedDataSource(structData, it);
        EditedDataSourceList[structData] = currentEditedDataSource;
        it.showLabel = false;
        it.expandable = true;
        it.ProcessNewType(structData);
        it.expanded = true;
        it.tabReOrder(0);
      }
      else
      {
        currentEditedDataSource = EditedDataSourceList[structData];
        currentEditedDataSource.refresh();
      }

      currentEditedDataSource.root.startEdit(setvalueCallBack, getvalueCallBack);
      currentEditedDataSource.root.resizeAll();
      HelpZone.Text = helpYourselfMsg;
      outterpanel.Refresh();
    }

    public string getConfigData()
    {
      return "<PropertiesForm>\n"
            + "<location x='" + this.Location.X.ToString() + "' y='" + this.Location.Y.ToString() + "'/>\n"
            + "<size     w='" + this.Size.Width.ToString() + "' h='" + this.Size.Height.ToString() + "'/>\n"
            + "</PropertiesForm>\n";
    }


    private void Form1_Resize(object sender, EventArgs e)
    {
   
    }

    private void Form1_SizeChanged(object sender, EventArgs e)
    {
      if (currentEditedDataSource == null)  return;
      if (currentEditedDataSource.root != null) currentEditedDataSource.root.resizeAll();
    }

    private void  valuechange( UIElement src)
    { string OriginalPropName = ""; 
      string fullpropname = "";
      string propType = "";

      MessageBox.Show("valuechanged " + src.ExtractPropPath(ref  OriginalPropName, ref  fullpropname, ref  propType).ToString() );
      _changeCallback(src);
    }


 

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void PropertiesForm2_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        e.Cancel = true;
        Hide();
      }
    }
  }
   
  
}
