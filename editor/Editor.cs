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





    public PropertiesForm2()
    {   
      InitializeComponent();

      // Don't use SizableTool on OSX, it cannot hide properly
      if (constants.OSX_Running && this.FormBorderStyle == FormBorderStyle.SizableToolWindow)
      {
        this.FormBorderStyle = FormBorderStyle.Sizable;
      }

      panel1.AutoSize = false;
      TopMost = true;
    
      if (constants.OSX_Running)
      { // awfull clipping problems in the Mac version
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

    public void showWindow(Form hostWindow,  GenericProperties structData, SetValueCallBack2 valueCallBack, bool ForceToTshow)
    {

      _changeCallback = valueCallBack;
     

        Text = "Properties of "+ hostWindow.Text;
  

      if (ForceToTshow)
      {
        Show();
        if (this.WindowState == FormWindowState.Minimized)
        {
          this.WindowState = FormWindowState.Normal;
        }
      }
      EditObject(structData, valueCallBack);
    }

    public void EditObject(object structData, SetValueCallBack2 valueCallBack)
    {
      if (!Visible) return;
      label1.Text = "Please wait....";
      label1.Refresh();

      //Console.WriteLine(" -> edit");

      if (EditedDataSourceList.ContainsKey(structData))
        if (EditedDataSourceList[structData] == currentEditedDataSource) return;


      if (currentEditedDataSource != null) currentEditedDataSource.root.stopEdit();

      UIElement it;

      if (!EditedDataSourceList.ContainsKey(structData))
      {
        UIElementBaseParams rootparam = new UIElementBaseParams(panel1, label1, null, null, "root", "Root", "root node");
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

      currentEditedDataSource.root.startEdit(valueCallBack);
      currentEditedDataSource.root.resizeAll();
      label1.Text = "Change any parameter you want. All changes are applied in real time.";
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
