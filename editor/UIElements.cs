using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Windows.Forms;
using YColors;
using YDataRendering;

namespace YoctoVisualisation
{

  public class FlatCombo : ComboBox  // thanks Reza Aghaei @ stackoverflow
  {
    private const int WM_PAINT = 0xF;
    private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
    protected override void WndProc(ref Message m)
    {
      base.WndProc(ref m);
      if (m.Msg == WM_PAINT)
      {
        using (var g = Graphics.FromHwnd(Handle))
        {
          using (var p = new Pen(Color.White))
          {
            g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
            g.DrawLine(p, Width - buttonWidth, 0, Width - buttonWidth, Height);
            p.Dispose();
          }
          
        }
      }
    }
  }



  public class CustomFlatButtom : Button
  {

    public enum ButtonLocation { LEFT, RIGHT };
    public enum ActionType { ACTION_EXPAND, ACTION_PUSH };

    public ButtonLocation _position = ButtonLocation.LEFT;
    public ActionType _type = ActionType.ACTION_EXPAND;
    public ActionType type { get { return _type; } }

    private Pen _pen = new Pen(Color.Black);
    public ButtonLocation side { get { return _position; } set { _position = value; } }
    UIElement _parent = null;
    EventHandler pushEvent = null;
    EventHandler clickEvent =null;

    private bool _pushed = false;
    public bool pushed
      { get { return _pushed; }
        set { _pushed = value; this.Refresh(); }
      }


   
    public void setType(ActionType t, EventHandler e)
    {
     

      _type = t;
      if (_type == ActionType.ACTION_EXPAND)
        this.clickEvent = e;
      else
      {
        this.clickEvent = pushClick;
        this.pushEvent  = e;
      }


    }

    public void click(object sender, EventArgs e)
    {
      if (clickEvent!=null) clickEvent(sender, e);
    }

    public void pushClick(object sender, EventArgs e)
    {
      pushed = !pushed;
      this.Refresh();
      this.pushEvent(this, e);

    }

    public CustomFlatButtom(UIElement parent): base()
    {
      _parent = parent;
      FlatStyle = FlatStyle.Flat;
      Paint += ExpandButton_Paint;
      GotFocus += ExpandButtonFocus;
      this.Click += click;

    }

    private void ExpandButtonFocus(object sender, EventArgs e)
    {
      if (_parent != null) _parent.control_GotFocus(sender, e);
    }

    private bool _expanded = false;
    public bool expanded
    {
      get { return _expanded; }
      set {
        _expanded = value;
        Refresh();
      }
    }

    private void ExpandButton_Paint(object sender, PaintEventArgs e)
    {
      int w1 = (Width >> 1) - 1;
      int w2 = (Width >> 1) + 1;
      int h1 = (Height >> 1) - 1;
      int h2 = (Height >> 1) + 1;
      if (_pushed)
        e.Graphics.FillRectangle(Brushes.Lime, new Rectangle(0, 0, Width, Height));
      else
        e.Graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, Width, Height));
      

      if (_type == ActionType.ACTION_EXPAND)
      {
        if (_position == ButtonLocation.LEFT)
        {
          if (!_expanded) e.Graphics.FillRectangle(Brushes.Black, new Rectangle(w1, 2, w2 - w1, Height - 4));
          e.Graphics.FillRectangle(Brushes.Black, new Rectangle(2, h1, Width - 4, h2 - h1));
        }
        else
        {
          e.Graphics.FillRectangle(Brushes.Black, new Rectangle(Width >> 2, Height >> 1, 2, 2));
          e.Graphics.FillRectangle(Brushes.Black, new Rectangle(2 * (Width >> 2), Height >> 1, 2, 2));
          e.Graphics.FillRectangle(Brushes.Black, new Rectangle(3 * (Width >> 2), Height >> 1, 2, 2));

        }
      }
      if (_type == ActionType.ACTION_PUSH)
      {
        int mx = Height >> 1;
        int my = Width >> 1;

        e.Graphics.DrawLine(_pen, mx,   2        , mx  ,     my-2  );
        e.Graphics.DrawLine(_pen, mx,   Height- 2, mx  ,     my + 2);
        e.Graphics.DrawLine(_pen, 2,    my,        mx - 2,   my );
        e.Graphics.DrawLine(_pen, mx+2, my,        Width- 2, my);
        // e.Graphics.DrawRectangle(_pen, mx-3, my-3, 6, 6);
        e.Graphics.DrawArc(_pen, mx-3,mx-3, 6,6 ,0, 400);





      }
      if (Focused) e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
    }
  }

  public class UIElementBaseParams
  {
    public Panel parentPanel;
    public Label HelpZoneLabel;
    public UIElement parentNode;
    public UIElement RootNode;
    public string internalname;
    public string label;
    public string description;
    public PropertyInfo prop;
    public bool changeCausesParentRefresh;
    public bool preExpanded;
    public bool expandable = false;
    public string isReadonlyCall = "";
    // public GetSummaryCallBack summaryCallback = null;
    public PropertyInfo summary =null;
    public object structRoot = null;



    public UIElementBaseParams(Panel p_parentPanel, Label p_descriptionLabel, UIElement p_parentNode, UIElement p_RootNode, string p_internalname, string p_label, string p_description, string p_isReadonlyCall)
    {
      parentPanel = p_parentPanel;
      HelpZoneLabel = p_descriptionLabel;
      parentNode = p_parentNode;
      RootNode = p_RootNode;
      internalname = p_internalname;
      label = p_label;
      description = p_description;
      changeCausesParentRefresh = false;
      preExpanded = false;
      isReadonlyCall = p_isReadonlyCall;

    }

  }

  //public delegate void targetValueHaschanged(UIElement src);

  public delegate void SetValueCallBack2(UIElement src);
  public delegate object GetValueCallBack2(UIElement src);
  public delegate string GetSummaryCallBack();


  public class UIElement
  {


    protected const string ARTIFICIALSECTIONNAME = "-ignore-";

    public string name = "NoName";
    protected string _targetFullName = "unknowTargetName";
    protected static int baseheight = 0;
    protected static int indentation = 16;
    protected static int expandableShift = 0;
    protected static int scrollBarWidth = 0;
    protected static int expandableVerticalOffset = 2;

    protected int ExpandableDepth = 0;

    protected bool _expanded = false;
    protected Label _helpZoneLabel = null;
    protected string _description = "";

    protected CustomFlatButtom expandCtrl = null;
    protected Label mainlabel = null;
    protected UIElement _rootNode = null;
    protected UIElement _parentNode = null;
    protected Panel _parentPanel;
    protected bool _changeCausesParentRefresh = false;
    protected int _availableWidth = 0;
    protected PropertyInfo _summaryProperty = null;
    protected string _isReadOnlyCall = "";


    protected bool expandButtonInitDone = false;
    protected bool mainLabelInitDone = false;
    protected List<UIElement> subElements = new List<UIElement>();
    protected Object _structRoot;

    protected SetValueCallBack2 _valueChangeCallback = null;
    protected GetValueCallBack2 _getValueCallback = null; 
    protected bool _shown = true;

    protected UIElement parentNode { get { return _parentNode; } }

    protected string targetFullName { get { return _targetFullName; } }


    protected GetSummaryCallBack _summaryCallback = null;

    public virtual void removeFromEditor()
    {
      if (mainlabel != null) _parentPanel.Controls.Remove(mainlabel);
      if (expandCtrl != null) _parentPanel.Controls.Remove(expandCtrl);
    }


    public virtual string  editorHelpText{ get { return ""; } }

    public virtual void addToEditor()
    {
      if ((mainlabel != null) && (!_parentPanel.Controls.Contains(mainlabel)))
        _parentPanel.Controls.Add(mainlabel);
      if ((expandCtrl != null) && (!_parentPanel.Controls.Contains(expandCtrl)))
        _parentPanel.Controls.Add(expandCtrl);

    }

    public virtual void refresh()
    {
      if (mainlabel != null) mainlabel.Text = name + getSummary();
      foreach (UIElement e in subElements)
        e.refresh();
    }

    public virtual int tabReOrder(int index)
    {

      if ((_expandable) && (expandCtrl != null)) expandCtrl.TabIndex = index++;

      foreach (UIElement e in subElements) index = e.tabReOrder(index);
      return index;
    }

    public void triggerValueChangeCallback()
    {
      if (_valueChangeCallback != null) _valueChangeCallback(this);

    }

    protected virtual void control_LostFocus(object sender, EventArgs e)
    {
      _helpZoneLabel.Text = "";
    }

    private void scrollToMakeVisible()
      { if (mainlabel == null) return;
      if (_parentPanel == null) return;

      int scrollposition = ((Panel)_parentPanel.Parent).VerticalScroll.Value;
      int scrollPage = ((Panel)_parentPanel.Parent).Height;

      if (constants.MonoRunning)
      {
        if (mainlabel.Top < scrollposition)
          ((Panel)_parentPanel.Parent).VerticalScroll.Value = mainlabel.Top;
        else if (mainlabel.Top > scrollposition + scrollPage - mainlabel.Height)
          ((Panel)_parentPanel.Parent).VerticalScroll.Value = mainlabel.Top - scrollPage + mainlabel.Height;
      } else  _parentPanel.ScrollControlIntoView(_helpZoneLabel);   // does not work on Mono

    }

    public virtual void control_GotFocus(object sender, EventArgs e)
    {
       string extraHelp = this.editorHelpText;
      if (extraHelp != "") extraHelp = " " + extraHelp;
      _helpZoneLabel.Text = _description + editorHelpText;
      scrollToMakeVisible();
    }

    protected void preExpandIfNeeded()
    {
      if ((_parentNode != null) && (_parentNode.expanded))
      {
        AllocateControls();
        if (expandable) initExpandButton();
        initMainLabel();
      }
      initExpandButton();

    }

    public UIElement(UIElementBaseParams p)
    {
      name = p.label;
      _targetFullName = p.internalname;
      _helpZoneLabel = p.HelpZoneLabel;
      _description = p.description;
      _changeCausesParentRefresh = p.changeCausesParentRefresh;
      _expandable = p.expandable;
      _preExpanded = p.preExpanded;
      _structRoot = p.structRoot;
      _isReadOnlyCall = p.isReadonlyCall;

     

      _summaryProperty = p.summary;

      if (baseheight == 0)
      {
        Label l = new Label();
        baseheight = (int)(l.Font.Height* (constants.OSX_Running?1.2:1.3));
        expandableShift = baseheight + 2;

        VScrollBar v = new VScrollBar();
        scrollBarWidth = v.Width;

      }

      _availableWidth = p.parentPanel.ClientSize.Width;
      _parentPanel = p.parentPanel;
      _parentNode = p.parentNode;
      _rootNode = p.RootNode == null ? this : p.RootNode;





    }
      
    protected virtual void initMainLabel()
    {
      if (mainlabel == null) return;
      if (mainLabelInitDone) return;
      mainlabel.AutoEllipsis = true;
      mainlabel.Text = name + getSummary();

      mainlabel.Top = 0;
      mainlabel.Left = 0;
      mainlabel.BackColor = System.Drawing.Color.Transparent;
      mainlabel.AutoSize = false;
      mainlabel.Height = baseheight;
      int w = _availableWidth;
      if ((expandCtrl != null) && expandable) w -= expandCtrl.Width;

      mainlabel.Width = w;
      mainlabel.Visible = _showLabel;

      ExpandableDepth = 0;

      UIElement p = this;
      while (p != null)
      {
        if (p.expandable) ExpandableDepth++;
        p = p.parentNode;
      }

      mainlabel.Text = name + getSummary();
      if (expandable)

        switch (ExpandableDepth)
        {
          case 1:
            mainlabel.BackColor = Color.Blue;
            break;
          case 2:
            mainlabel.BackColor = Color.FromArgb(255, 7, 141, 255);
            mainlabel.ForeColor = Color.White;
            break;
          case 3:
            mainlabel.BackColor = Color.FromArgb(255, 120, 196, 255);
            break;
          case 4:
            mainlabel.BackColor = Color.FromArgb(255, 213, 237, 255);
            break;
          case 5:
            mainlabel.BackColor = Color.FromArgb(255, 237, 246, 253);
            break;
          default:
            mainlabel.BackColor = Color.FromArgb(255, 246, 251, 255);
            break;
        } else mainlabel.BackColor = Color.Transparent;
      mainLabelInitDone = true;
    }

    public virtual void AllocateControls()
    {

      if (mainlabel == null)
      {
        mainlabel = new Label();
        initMainLabel();
        _parentPanel.Controls.Add(mainlabel);
      }

      if ((_expandable) && (expandCtrl == null))
      {
        expandCtrl = new CustomFlatButtom(this);
        initExpandButton();
        _parentPanel.Controls.Add(expandCtrl);
      }

    }

    protected void initExpandButton()
    {
      if (expandCtrl == null) return;
      if (expandButtonInitDone) return;

      expandCtrl.Top = 0;
      expandCtrl.Left = 0;
      expandCtrl.Text = "";
      expandCtrl.MinimumSize = new Size(5, 5);
      expandCtrl.Width = baseheight - 2;
      expandCtrl.Height = baseheight - 2;
      expandCtrl.FlatStyle = FlatStyle.Flat;

      expandCtrl.setType(CustomFlatButtom.ActionType.ACTION_EXPAND, ExpandCtrl_Click_expand);


     

      expandCtrl.expanded = _expanded;
      expandCtrl.GotFocus += control_GotFocus;
      expandCtrl.LostFocus += control_LostFocus;
      expandCtrl.Visible = _showLabel;
      expandButtonInitDone = true;
    }

    public virtual int rowHeight { get { return baseheight; } }

    public List<String> ExtractPropPath(ref string OriginalPropName, ref string fullpropname, ref string propType)
    {

      int index = -1;

      List<string> path = new List<string>();
      UIElement p = this;
      OriginalPropName = p.targetFullName;
      do
      {
        fullpropname = p.targetFullName;
        index = fullpropname.IndexOf("_");
        if (index < 0) 
        {
          if  (fullpropname!= ARTIFICIALSECTIONNAME) path.Insert(0, fullpropname); // ignore artificial sections
          p = p.parentNode;
          if (p == null) throw new System.ArgumentException("invalid Property name");
        }

      } while (index < 0)  ;

      propType = fullpropname.Substring(0, index);
      string propname = fullpropname.Substring(index + 1);

      path.Insert(0, propname);
      return path;

    }

    public void startEdit(SetValueCallBack2 setcallback, GetValueCallBack2 getCallback)
    {

      addToEditor();
      _valueChangeCallback = setcallback;
      _getValueCallback = getCallback;
      foreach (UIElement item in subElements) item.startEdit(setcallback, getCallback);

    }

    public void stopEdit()
    {

      removeFromEditor();
      _valueChangeCallback = null;
      _getValueCallback = null;
      foreach (UIElement item in subElements) item.stopEdit();

    }

    protected virtual void expand()
    {
      foreach (UIElement e in subElements)
      {
        e.AllocateControls();
        e.show();
        _rootNode.tabReOrder(0);
      }
      _expanded = true;

    }
    protected virtual void colapse()
    {
      foreach (UIElement e in subElements) e.hide();

    }

    public bool expanded
    {
      get { return _expanded; }
      set
      {
        _expanded = value;
        expandCtrl.expanded = _expanded;
        if (_expanded) expand(); else colapse();
        _rootNode.resizeAll();
        ResumeDrawing(_parentPanel);

      }
    }

    public string getSummary()
    {
      if (_summaryProperty != null)
       return " (" + _summaryProperty.GetValue(_structRoot,null) + ")";
      return "";
    }

    public List<object> sensorList()
    {

      // return sensorsManager.sensorList.ToList<Object>();

      // .NET 3.5 compatibility
      List<Object> res = new List<Object>();
      foreach (CustomYSensor s in sensorsManager.sensorList) res.Add(s);
      return res;

      

    }

    class CustomAttributesExtractor
    {
      public bool available = true;
      public string displayName = "unknown";
      public string category = "";
      public string displayDescription = "Not documented";
      public string[] prefinedValues = null;
      public bool changeCausesParentRefresh = false;
      public bool PreExpandedCategory = false;
      public bool PreExpanded = false;
      public string summaryPropertyName = "";
      public string isReadOnlyCall = "";

      public CustomAttributesExtractor(PropertyInfo ps)
      {
        // .NET 3.5
        object[] list = ps.GetCustomAttributes(false);
        foreach (object o in list)
        {
          if (o is OnlyAvailableonAttribute) available = ((OnlyAvailableonAttribute)o).Available;
          else if (o is DisplayNameAttribute) displayName = ((DisplayNameAttribute)o).DisplayName;
          else if (o is CategoryAttribute) category = ((CategoryAttribute)o).Category;
          else if (o is DescriptionAttribute) displayDescription = ((DescriptionAttribute)o).Description;
          else if (o is ParamExtraDescriptionAttribute) prefinedValues = (string[])((ParamExtraDescriptionAttribute)o).allowedValueClass.GetProperty("AllowedValues").GetValue(((ParamExtraDescriptionAttribute)o).allowedValueClass, null);
          else if (o is ChangeCausesParentRefreshAttribute) changeCausesParentRefresh = ((ChangeCausesParentRefreshAttribute)o).changeCausesParentRefresh;
          else if (o is PreExpandedCategoryAttribute) PreExpandedCategory = ((PreExpandedCategoryAttribute)o).preExpandedCategory;
          else if (o is PreExpandedAttribute) PreExpanded = ((PreExpandedAttribute)o).preExpanded;
          else if (o is ParamCategorySummaryAttribute) summaryPropertyName = ((ParamCategorySummaryAttribute)o).callback;
          else if (o is IsReadonlyCallAttribute)
            isReadOnlyCall = ((IsReadonlyCallAttribute)o).IsReadonlyCall;


        }
        /* .NET 4
        var cust_attribute8 = ps.GetCustomAttribute<OnlyAvailableonAttribute>();
        if (cust_attribute8 != null) available = (bool)cust_attribute8.Available;

        if (available)
        {
          var cust_attribute1 = ps.GetCustomAttribute<DisplayNameAttribute>();
          if (cust_attribute1 != null) displayName = cust_attribute1.DisplayName;

          var cust_attribute2 = ps.GetCustomAttribute<CategoryAttribute>();
          if (cust_attribute2 != null) category = cust_attribute2.Category;

          var cust_attribute3 = ps.GetCustomAttribute<DescriptionAttribute>();
          if (cust_attribute3 != null) displayDescription = cust_attribute3.Description;

          var cust_attribute4 = ps.GetCustomAttribute<ParamExtraDescriptionAttribute>();
          if (cust_attribute4 != null) prefinedValues = (string[])cust_attribute4.allowedValueClass.GetProperty("AllowedValues").GetValue(null);

          var cust_attribute5 = ps.GetCustomAttribute<ChangeCausesParentRefreshAttribute>();
          if (cust_attribute5 != null) changeCausesParentRefresh = (bool)cust_attribute5.changeCausesParentRefresh;

          var cust_attribute6 = ps.GetCustomAttribute<PreExpandedCategoryAttribute>();
          if (cust_attribute6 != null) PreExpandedCategory = (bool)cust_attribute6.preExpandedCategory;

          var cust_attribute7 = ps.GetCustomAttribute<PreExpandedAttribute>();
          if (cust_attribute7 != null) PreExpanded = (bool)cust_attribute7.preExpanded;

          var cust_attribute9 = ps.GetCustomAttribute<ParamCategorySummaryAttribute>();
          if (cust_attribute9 != null) summaryPropertyName = cust_attribute9.callback;

        } */
      }
     }

    public void ProcessNewType(object dataStucture)
    {
      Dictionary<string, UIElement> subsections = new Dictionary<string, UIElement>();

      _structRoot = dataStucture;
      List<UIElement> toExpand = new List<UIElement>();
      PropertyInfo[] props = dataStucture.GetType().GetProperties();
      foreach (PropertyInfo ps in props)
      { if (ps.CanWrite)
        { CustomAttributesExtractor attr = new CustomAttributesExtractor(ps);
          if (attr.available)
          { UIElement section = this;
            if (attr.category != "")
            {
              if (subsections.ContainsKey(attr.category)) section = subsections[attr.category];
              else
              {
                UIElementBaseParams sectionparam = new UIElementBaseParams(_parentPanel, _helpZoneLabel, this, _rootNode, ARTIFICIALSECTIONNAME, attr.category, attr.category + " section, expand for more...","");
                sectionparam.preExpanded = attr.PreExpandedCategory;
                sectionparam.expandable = true;
                sectionparam.structRoot = dataStucture;
                if (attr.summaryPropertyName != "")
                  sectionparam.summary = dataStucture.GetType().GetProperty(attr.summaryPropertyName);
                    
                section = new UIElement(sectionparam);
                if (attr.PreExpandedCategory) toExpand.Add(section);

                section.showLabel = true;
                subsections[attr.category] = section;
                addSubElement(section);
              }
            }

            UIElementBaseParams elParam = new UIElementBaseParams(_parentPanel, _helpZoneLabel, section, _rootNode, ps.Name, attr.displayName, attr.displayDescription, attr.isReadOnlyCall);
            elParam.preExpanded = attr.PreExpanded;
            elParam.changeCausesParentRefresh = attr.changeCausesParentRefresh;

            string targetName = _targetFullName;
            string targetPrefix = "";

            int p = targetName.IndexOf("_");
            if (p >= 0)
            {
              targetPrefix = targetName.Substring(0, p);
              targetName = targetName.Substring(p + 1);
            }

            if (ps.PropertyType.IsEnum)
            {
              UIElementEnum s = new UIElementEnum(elParam, dataStucture, ps);
              section.addSubElement(s);
            }
            else
              switch (ps.PropertyType.Name)
              {

                case "String":
                  {
                    if (attr.prefinedValues == null)
                    {
                      UIElementString s = new UIElementString(elParam, dataStucture, ps);
                      section.addSubElement(s);
                    }
                    else
                    {
                      UIElementList s = new UIElementList(elParam, dataStucture, ps, attr.prefinedValues.ToList<Object>(),null);
                      section.addSubElement(s);
                    }
                    break;
                  }
                case "Int32":
                  {
                    if (attr.prefinedValues == null)
                    {
                      UIElementInteger s = new UIElementInteger(elParam, dataStucture, ps);
                      section.addSubElement(s);
                    }
                    else
                    {
                      UIElementList s = new UIElementList(elParam, dataStucture, ps, attr.prefinedValues.ToList<Object>(),null);
                      section.addSubElement(s);
                    }

                    break;
                  }
                case "Double":
                  {
                    UIElementDouble s = new UIElementDouble(elParam, dataStucture, ps);
                    section.addSubElement(s);
                    break;
                  }
                case "doubleNan":
                  {
                    UIElementDoubleNan s = new UIElementDoubleNan(elParam, dataStucture, ps);
                    section.addSubElement(s);
                    break;
                  }
                case "Boolean":
                  {
                    UIElementBool s = new UIElementBool(elParam, dataStucture, ps);
                    section.addSubElement(s);
                    break;
                  }

                /*
            case "listItem":
                {
                  UIElementList s = new UIElementList(_parentPanel, section, _rootNode,ps.Name, name, dataValue, pd,List.Cast<object>().ToList());
                  section.addSubElement(s);
                  break;
                }*/

                case "xAxisPosition":
                  {
                    UIElementMarkerPos s = new UIElementMarkerPos(elParam, dataStucture, ps);
                    section.addSubElement(s);
                    break;
                  }

                case "YColor":
                  {
                    UIElementColor s = new UIElementColor(elParam, dataStucture, ps);
                    section.addSubElement(s);
                    break;
                  }
                case "CustomYSensor":
                  {
                    UIElementList s = new UIElementList(elParam, dataStucture, ps, sensorList(), sensorList);
                    section.addSubElement(s);
                    break;
                  }

                default:
                  if (ps.PropertyType is Object)
                  {
                    elParam.expandable = true;
                    elParam.preExpanded = attr.PreExpanded;
                    UIElement subobjectsection = new UIElement(elParam);
                    if (attr.PreExpanded) toExpand.Add(subobjectsection);
                    subobjectsection.showLabel = true;
                    section.addSubElement(subobjectsection);
                    subobjectsection.ProcessNewType(ps.GetValue(dataStucture, null));

                  }
                  break;
              }

            //    watch1.Stop(); Console.WriteLine("creating sub node  " + displayName +" took "+ watch1.ElapsedMilliseconds.ToString() + " ms");
          }
          }
        else
        {
          if (ps.Name == "summary")
          { _summaryProperty = ps; }
        }
      }


      if (_summaryProperty != null) refresh();
      foreach (UIElement e in toExpand)
        e.expand();





    }

    public virtual void show()
    {
      _shown = true;
      if (mainlabel != null) mainlabel.Visible = true;
      if (expandCtrl != null) expandCtrl.Visible = _expandable;
      if (expanded)
        foreach (UIElement e in subElements) e.show();

    }

    public virtual void hide()
    {
      _shown = false;
      if (mainlabel != null) mainlabel.Visible = false;
      if (expandCtrl != null) expandCtrl.Visible = false;
      foreach (UIElement e in subElements) e.hide();
    }

    private void ExpandCtrl_Click_expand(object sender, EventArgs e)
    {
      expanded = !_expanded;
    }

    private void ExpandCtrl_Click_push(object sender, EventArgs e)
    {
      
    }


    public void resizeAll()
    {
      bool scrollBarNeeded;
      int parentWidth;
      int h = computeHeight();
      if (_parentPanel.Parent == null) return;  // may happen when application is closing
      scrollBarNeeded = h > _parentPanel.Parent.Size.Height;
      parentWidth = _parentPanel.Parent.Size.Width;
     
      SuspendDrawing(_parentPanel);
      _availableWidth = scrollBarNeeded ? parentWidth - scrollBarWidth - 10 : parentWidth - 5;
      _parentPanel.Size = new Size(_availableWidth, h);
      resize(0, 0, _availableWidth >> 1, _availableWidth - 5);
      ResumeDrawing(_parentPanel);
    }
    protected bool _showLabel = false;
    public bool showLabel
    {
      get { return _showLabel; }
      set
      {
        _showLabel = value;
        if (expandCtrl != null) expandCtrl.Visible = value && _expandable;
      }
    }

    protected bool _expandable = false;
    protected bool _preExpanded = false;

    public bool expandable
    {
      get { return _expandable; }
      set
      {

        _expandable = value;
        if (!value) return;

        if (expandCtrl == null)
        {
          AllocateControls();
          initExpandButton();
        }

        expandCtrl.Visible = value && _showLabel;

      }
    }

    public void addSubElement(UIElement node)
    {
      subElements.Add(node);
    }

    virtual public int computeHeight()
    {
      if (!_shown) return 0;
      int h = rowHeight;
      if ((_expandable) && (expandCtrl != null) && (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT)) h += expandableVerticalOffset;
      if (_expanded)
        foreach (UIElement e in subElements)
          h += e.computeHeight();
      //Console.WriteLine("C:" + this.name + ":" + h.ToString());
      return h;

    }


    virtual public int resize(int top, int left1, int left2, int width)
    {
      if ((_expandable) && (expandCtrl != null) && (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT)) top += expandableVerticalOffset;
      if (expandCtrl != null)
      {
        expandCtrl.Visible = _showLabel;
        expandCtrl.Width = baseheight - 2;
        expandCtrl.Height = baseheight - 2;
      }

      if (mainlabel != null)
      {
        mainlabel.Visible = _showLabel;
        mainlabel.Top = top;
        mainlabel.Height = rowHeight;
       
        mainlabel.Text = this.name + getSummary();
        mainlabel.Left = left1 + (_expandable && (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT) ? expandCtrl.Width + 2 : 0);
        mainlabel.Width = width - left1 - (_expandable ? expandCtrl.Width : 0); ;
      }

      if ((expandCtrl != null) && (mainlabel != null))
      {
        expandCtrl.Visible = _expandable && _showLabel;
        expandCtrl.Top = top + ((mainlabel.Height - expandCtrl.Height) >> 1);
        expandCtrl.Left = (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT) ? left1 : left1 + width - expandCtrl.Width;
      }
      int y = top + (_showLabel ? rowHeight : 0);
      if (_expanded)
        foreach (UIElement e in subElements)
        {
          y = e.resize(y, left1 + (_showLabel ? indentation : 0), left2, width);
          e.show();
        }
      else foreach (UIElement e in subElements) e.hide();
      //Console.WriteLine("D:" + this.name + ":" + y.ToString() + " "+ (_showLabel?"visible":"hidden"));
      return y;
    }

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    private const int WM_SETREDRAW = 0xB;

    public static void SuspendDrawing(Control target)
    {
      return;
      // was a nice trick, turns out that it causes more problems that it solves
      //if (constants.OSX_Running) return;
      //SendMessage(target.Handle, WM_SETREDRAW, 0, 0);
    }

    public static void ResumeDrawing(Control target)
    {
      target.Parent.Refresh();
      return;
      // was a nice trick, turns out that it causes more problems that it solves
      //if (constants.OSX_Running) return;
      //try
      //{ // may crash  when application is closing
      //  SendMessage(target.Handle, WM_SETREDRAW, 1, 0);
      //  target.Parent.Refresh();
      //}
      //catch (Exception) { }
      }

  }

  /***
   ***  Generic 
   ***/

  class UIElementGeneric : UIElement
  {
    protected object _dataContainer;
    protected PropertyInfo _prop;
    protected Control input = null;

    public UIElementGeneric(UIElementBaseParams p, object dataContainer, PropertyInfo prop)
        : base(p)
    {

      _prop = prop;
      _dataContainer = dataContainer;
      _showLabel = true;

      if (_preExpanded) AllocateControls();
      if ((_expandable) && (_preExpanded))
        expand();

    }

    public override void removeFromEditor()
    {
      base.removeFromEditor();
      if (input != null) _parentPanel.Controls.Remove(input);

    }

    public override int tabReOrder(int index)
    {
      if ((_expandable) && (expandCtrl != null) && (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT)) expandCtrl.TabIndex = index++;

      if (input != null) input.TabIndex = index++;
      if ((_expandable) && (expandCtrl != null) && (expandCtrl.side == CustomFlatButtom.ButtonLocation.RIGHT)) expandCtrl.TabIndex = index++;
      foreach (UIElement e in subElements) index = e.tabReOrder(index);
      return index;

    }

    public override void addToEditor()
    {
      base.addToEditor();
      if (input != null) _parentPanel.Controls.Add(input);

    }

    public override void show()
    {

      base.show();
      if (input != null) input.Visible = true;

     

    }

    public void checkForReadOnly()
    {
      if (input == null) return;

     if (_isReadOnlyCall != "")
      {
        bool readOnly = (bool)_dataContainer.GetType().GetProperty(_isReadOnlyCall).GetValue(_dataContainer, null);
        input.Enabled = !readOnly;
      }

    }

    public override void hide()
    {

      if (input != null) input.Visible = false;
      base.hide();
    }

    public override int resize(int top, int left1, int left2, int width)
    {
      if (!_shown) return 0;
      if ((_expandable) && (expandCtrl != null) && (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT)) top += expandableVerticalOffset;
      if (expandCtrl != null)
      {
        expandCtrl.Width = baseheight - 2;
        expandCtrl.Height = baseheight - 2;
      }

      if (mainlabel != null)
      {
        mainlabel.Top = top;
        mainlabel.Left = left1 + (_expandable && (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT) ? expandCtrl.Width + 2 : 0);
        mainlabel.Height = rowHeight;
        mainlabel.Width = left2 - left1 - (_expandable ? expandCtrl.Width : 0); ;
      }
      if ((expandCtrl != null) && (mainlabel != null))
      {
        expandCtrl.Visible = _expandable && _showLabel;
        expandCtrl.Top = top + ((mainlabel.Height - expandCtrl.Height) >> 1);
        expandCtrl.Left = (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT) ? left1 : width - expandCtrl.Width;
      }

      if (input != null)
      {
        input.Top = top;
        input.Left = left2;
        input.Width = width - left2;
      }
      return top + (input != null ? input.Height : 0);
    }

  }

  /***
   ***  String
   ***/
  class UIElementString : UIElementGeneric
  {

    public UIElementString(UIElementBaseParams p, object dataContainer, PropertyInfo prop) :
              base(p, dataContainer, prop)
    {
      preExpandIfNeeded();
    }

   

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {

        input = new TextBox();
        input.AutoSize = false;
        input.Height = baseheight;
        input.Text = (string)_prop.GetValue(_dataContainer, null);
        new InputFieldManager(input, InputFieldManager.dataType.DATA_STRING, true, Double.NaN, Double.NaN, Text_TextChanged);

       
        
        ((TextBox)input).BorderStyle = BorderStyle.None;
        _parentPanel.Controls.Add(input);
        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;
        checkForReadOnly();

      }

    }

    public override void refresh()
    {
      if (input == null) return;
      if (_getValueCallback == null) return;
      string s = (string)_getValueCallback(this);

      if (input != null) if (s != input.Text) input.Text = s;
      checkForReadOnly();
    }

    private void Text_TextChanged(object sender, EventArgs e)
    {
      if (input != null)
      {
        _value = input.Text;
        _prop.SetValue(_dataContainer, _value, null);
        if (_valueChangeCallback != null) _valueChangeCallback(this);
        if (_changeCausesParentRefresh) parentNode.refresh();
      }
    }

    protected string _value = "";
    public string value
    {
      get { return _value; }
      set { _value = value; input.Text = value; }
    }

    public override int resize(int top, int left1, int left2, int width)
    {
      base.resize(top, left1, left2, width);
   
     

        return top + input.Height;
    }

  }

  /***
  ***  Integer
  ***/
  class UIElementInteger : UIElementGeneric
  {

    public UIElementInteger(UIElementBaseParams p, object dataContainer, PropertyInfo prop) :
                  base(p, dataContainer, prop)
    {
      preExpandIfNeeded();
    }

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {
        input = new TextBox();
        input.AutoSize = false;
        input.Height = baseheight;
        ((TextBox)input).TextAlign = HorizontalAlignment.Right;
        ((TextBox)input).BorderStyle = BorderStyle.None;
        input.Text = ((int)_prop.GetValue(_dataContainer, null)).ToString();
        new InputFieldManager(input, InputFieldManager.dataType.DATA_INT, false, Double.NaN, Double.NaN, Text_TextChanged);
        input.TextChanged += Text_TextChanged;
        _parentPanel.Controls.Add(input);
        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;
        checkForReadOnly();
      }
    }

    public override void refresh()
    {
      if (input == null) return;
      if (_getValueCallback == null) return;
      object o = _getValueCallback(this);
      if (o == null) return;
      int propValue = (int)o;
      int inputValue ;
      bool ok = false;
      if (int.TryParse(input.Text, out inputValue))
      {
        ok = (inputValue == propValue);
      }
      if (!ok) input.Text = propValue.ToString();
      checkForReadOnly();
    }

    private void Text_TextChanged(object sender, EventArgs e)
    {
      if (Int32.TryParse(input.Text, out _value))
      {
        _prop.SetValue(_dataContainer, _value, null);

      
        if (_valueChangeCallback != null) _valueChangeCallback(this);
        if (_changeCausesParentRefresh) parentNode.refresh();
    
       }
    }

    protected int _value = 0;
    public int value
    {
      get { return _value; }
      set { _value = value; input.Text = value.ToString(); }
    }

    public override int resize(int top, int left1, int left2, int width)
    {
      base.resize(top, left1, left2, width);

      return top + input.Height;
    }
  }

  /***
  ***  Double
  ***/
  class UIElementDouble : UIElementGeneric
  {
    public UIElementDouble(UIElementBaseParams p, object dataContainer, PropertyInfo prop) :
                  base(p, dataContainer, prop)
    {
      preExpandIfNeeded();
    }

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {
        input = new TextBox();
        input.AutoSize = false;
        input.Height = baseheight;
        ((TextBox)input).TextAlign = HorizontalAlignment.Right;
        ((TextBox)input).BorderStyle = BorderStyle.None;
       
        input.Text = ((double)_prop.GetValue(_dataContainer, null)).ToString();
        new InputFieldManager(input, InputFieldManager.dataType.DATA_FLOAT, false, Double.NaN, Double.NaN, Text_TextChanged);
      
        _parentPanel.Controls.Add(input);
        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;
        checkForReadOnly();
      }
    }

    public override void refresh()
    { if (input == null) return;

      if (_getValueCallback == null) return;
     
      object o = _getValueCallback(this);
      if (o == null) return;
      double propValue = (double)o;
      double inputValue;
      bool  ok = false;
      if  (Double.TryParse(input.Text, out inputValue))
      {
        ok = (inputValue == propValue);
      }      
      if  (!ok) input.Text = propValue.ToString();
      checkForReadOnly();
    }

    protected virtual void Text_TextChanged(object sender, EventArgs e)
    {
      if (Double.TryParse(input.Text, out _value))
      {
        _prop.SetValue(_dataContainer, _value, null);

       
        if (_valueChangeCallback != null) _valueChangeCallback(this);
        if (_changeCausesParentRefresh) parentNode.refresh();
      }
     
    }

    protected double _value = 0;
    public double value
    {
      get { return _value; }
      set { _value = value; input.Text = value.ToString(); }
    }

    public override int resize(int top, int left1, int left2, int width)
    {
      base.resize(top, left1, left2, width);

      return top + input.Height;
    }
  }

  class UIElementDoubleNan : UIElementGeneric
  {
    public UIElementDoubleNan(UIElementBaseParams p, object dataContainer, PropertyInfo prop) :
                  base(p, dataContainer, prop)
    {
      preExpandIfNeeded();
    }

    public override string editorHelpText { get { return "Leave blank for automatic behavior."; } }

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {
        input = new TextBox();
        input.AutoSize = false;
        input.Height = baseheight;
        ((TextBox)input).TextAlign = HorizontalAlignment.Right;
        ((TextBox)input).BorderStyle = BorderStyle.None;
       
        input.Text = ((doubleNan)_prop.GetValue(_dataContainer, null)).ToString();
        new InputFieldManager(input, InputFieldManager.dataType.DATA_FLOAT, true, Double.NaN, Double.NaN, Text_TextChanged);
        _parentPanel.Controls.Add(input);
        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;
        checkForReadOnly();
      }
    }

    public override void refresh()
    {

      if (input == null) return;
      if (_getValueCallback == null) return;

      object o = _getValueCallback(this);
      double propValue = o is doubleNan ? ((doubleNan)o).value : (double)o;
      if (double.IsNaN(propValue))
       {
        if (input.Text != "") input.Text = "";
        return;
      }


      double inputValue;
      bool ok = false;
      if (Double.TryParse(input.Text, out inputValue))
      {
        ok = (inputValue == propValue);
      }
      if (!ok) input.Text = propValue.ToString();

      checkForReadOnly();

    }

    protected virtual void Text_TextChanged(object sender, EventArgs e)
    {
      bool changeOK = false;
      double value;
      if (input.Text == "")
      {
        ((doubleNan)_prop.GetValue(_dataContainer, null)).value = Double.NaN;
        changeOK = true;
      }
      else
      if (Double.TryParse(input.Text, out value))
      {
        ((doubleNan)_prop.GetValue(_dataContainer, null)).value = value;
        changeOK = true;
      }

      if (changeOK)
      {

        if (_valueChangeCallback != null) _valueChangeCallback(this);
       
        if (input != null) if (_changeCausesParentRefresh) parentNode.refresh();
      
        }
    }

    public override int resize(int top, int left1, int left2, int width)
    {
      base.resize(top, left1, left2, width);

      return top + input.Height;
    }
  }

  /***
   ***  Boolean
   ***/
  class UIElementBool : UIElementGeneric
  {

    public UIElementBool(UIElementBaseParams p, object dataContainer, PropertyInfo prop) :
                  base(p, dataContainer, prop)
    {
      preExpandIfNeeded();
    }

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {
        input = new CheckBox();
        input.Height = baseheight;
        ((CheckBox)input).FlatStyle = FlatStyle.Flat;

        _value = (bool)_prop.GetValue(_dataContainer, null);
        ((CheckBox)input).Checked = _value;
        ((CheckBox)input).CheckedChanged += UIElementBool_CheckedChanged;
        ((CheckBox)input).Text = _value ? "Yes" : "No";
        _parentPanel.Controls.Add(input);
        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;
        checkForReadOnly();
      }
    }

    public override void refresh()
    {
      if (input == null) return;
      if (_getValueCallback == null) return;
      
      object o = _getValueCallback(this);

      bool b = false;

      if (o == null)
      {
        b =(bool) _prop.GetValue(_dataContainer, null);
      } else  b = (bool)o;

      if (input != null)
        if (b != ((CheckBox)input).Checked)
        {
          ((CheckBox)input).Checked = b;
          ((CheckBox)input).Text = b ? "Yes" : "No";
        }
      checkForReadOnly();
    }

    private void UIElementBool_CheckedChanged(object sender, EventArgs e)
    {
      _value = ((CheckBox)input).Checked;
      _prop.SetValue(_dataContainer, _value, null);
      ((CheckBox)input).Text = _value ? "Yes" : "No";
      if (_valueChangeCallback != null)
          _valueChangeCallback(this);
      if (_changeCausesParentRefresh)
         parentNode.refresh();

    }

    protected bool _value = false;
    public bool value
    {
      get { return _value; }
      set { _value = value; ((CheckBox)input).Text = _value ? "Yes" : "No"; }
    }

    public override int resize(int top, int left1, int left2, int width)
    {
      base.resize(top, left1, left2, width);
      ((CheckBox)input).Text = _value ? "Yes" : "No";
      if (mainlabel != null) { mainlabel.Top = top + 2; mainlabel.Height = rowHeight - 2; }
      return top + input.Height;
    }
  }

  /***
   ***  enum
   ***/
  class UIElementEnum : UIElementGeneric
  {
    public class elmnt
    {
      int _value = -1;
      string _desc = null;
      public elmnt(string desc, int value)
      {
        _value = value;
        _desc = desc;
      }
      public override string ToString() { return _desc; }
      public int value() { return _value; }
    }

    public UIElementEnum(UIElementBaseParams p, object dataContainer, PropertyInfo prop) : base(p, dataContainer, prop)
    {

      value = (int)_prop.GetValue(_dataContainer, null);
      preExpandIfNeeded();
    }

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {

        input = new FlatCombo();
        ((ComboBox)input).DropDownStyle = ComboBoxStyle.DropDownList;
        ((ComboBox)input).FlatStyle = FlatStyle.Flat;
        ((ComboBox)input).ItemHeight = baseheight;
        input.BackColor = Color.White;
        input.AutoSize = false;
        input.Height = baseheight;

        Type t = _prop.PropertyType;
        var enumName = t.Name;
        foreach (FieldInfo fieldInfo in t.GetFields())
        {
          if (fieldInfo.FieldType.IsEnum)
          {
            string fName = fieldInfo.Name;
            int fValue = (int)fieldInfo.GetRawConstantValue();

            // .NET 4
            //DescriptionAttribute a = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));

            // .NET 3.5
            object[] attr = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute),false);
            DescriptionAttribute a =null;
            foreach (object o in attr) if (o is DescriptionAttribute) a = (DescriptionAttribute)o;

            ((ComboBox)input).Items.Add(new elmnt(((DescriptionAttribute)a).Description, fValue));
          }
        }
        value = (int)_prop.GetValue(_dataContainer, null);
       

        ((ComboBox)input).SelectedValueChanged += UIElementEnum_SelectedValueChanged;
        _parentPanel.Controls.Add(input);
        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;
        checkForReadOnly();
      }
    }

    public override int rowHeight { get { return input.Height; } }

    public override void refresh()
    {
      if (input != null)
      {
        if (_getValueCallback == null) return;
        value = (int)_getValueCallback(this);
        //value = (int)_prop.GetValue(_dataContainer, null);

        elmnt el = ((elmnt)(((ComboBox)input)).SelectedItem);
        if (value != el.value())
          ((ComboBox)input).SelectedIndex = value;



      }
      checkForReadOnly();
    }

    private void UIElementEnum_SelectedValueChanged(object sender, EventArgs e)
    {
      elmnt el = ((elmnt)(((ComboBox)input)).SelectedItem);
      _value = el.value();
      _prop.SetValue(_dataContainer, _value, null);
      if (_valueChangeCallback != null)
        _valueChangeCallback(this);
      if (_changeCausesParentRefresh)
          parentNode.refresh();

    }

    protected int _value = -1;
    public int value
    {
      get { return _value; }
      set
      {
        _value = value;

        if (input != null)
        {
          for (int i = 0; i < ((ComboBox)input).Items.Count; i++)
            if (((elmnt)(((ComboBox)input).Items[i])).value() == value)
            {
              ((ComboBox)input).SelectedItem = ((ComboBox)input).Items[i];
            }
        }
      }
    }

    public override int resize(int top, int left1, int left2, int width)
    {
      base.resize(top, left1, left2, width);
      if (mainlabel != null)    mainlabel.Top = top + 3;
      return top + input.Height;
    }

  }

  /***
  ***  List<Object>
  ***/
  class UIElementList : UIElementGeneric
  {

    public delegate List<Object> refreshUIElementListCallback();

    List<Object> _list = null;
    refreshUIElementListCallback _refreshCallback =null;

    public UIElementList(UIElementBaseParams p, object dataContainer, PropertyInfo prop, List<Object> list, refreshUIElementListCallback refreshContents) :
              base(p, dataContainer, prop)
    {
      _refreshCallback = refreshContents;
       _list = list;
      preExpandIfNeeded();
    }

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {
        input = new FlatCombo();
        ((ComboBox)input).DropDownStyle = ComboBoxStyle.DropDownList;
        ((ComboBox)input).FlatStyle = FlatStyle.Flat;
        input.BackColor = Color.White;
        foreach (object o in _list) ((ComboBox)input).Items.Add(o);
        value = _prop.GetValue(_dataContainer, null);
        if (value is Int32)
          ((ComboBox)input).SelectedIndex = (int)value;
        if (value is String)
          for (int i = 0; i < ((ComboBox)input).Items.Count; i++)
            if (((ComboBox)input).Items[i].ToString() == value.ToString())
              ((ComboBox)input).SelectedIndex = i;

        ((ComboBox)input).SelectedValueChanged += UIElementList_SelectedValueChanged;
        _parentPanel.Controls.Add(input);
        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;
        checkForReadOnly();
      }
    }

    public override int rowHeight { get { return input.Height; } }

    public override void control_GotFocus(object sender, EventArgs e)
    { 
      base.control_GotFocus(sender, e);
      refresh();
    }


    public override void refresh()
    {
      if (input == null) return;




        ((ComboBox)input).SelectedValueChanged -= UIElementList_SelectedValueChanged;
      if (_refreshCallback != null) 
      {
        _list = _refreshCallback();

        ((ComboBox)input).Items.Clear();
        foreach (object o in _list)
              ((ComboBox)input).Items.Add(o);
       

      }

     

     value = _prop.GetValue(_dataContainer, null);
      if (value is Int32)
          ((ComboBox)input).SelectedIndex = (int)value;

      
      if (value is String)
      {
        Boolean found = false;
        for (int i = 0; i < ((ComboBox)input).Items.Count; i++)
          if (((ComboBox)input).Items[i].ToString() == value.ToString())
          {
            found = true;
            if (((ComboBox)input).SelectedIndex != i)
              ((ComboBox)input).SelectedIndex = i;
          }
        if ((!found)  && (value.ToString()!="")) // add unlisted values
        {
          ((ComboBox)input).Items.Add(value.ToString());
          ((ComboBox)input).SelectedIndex = ((ComboBox)input).Items.Count - 1;

        }
      }
      ((ComboBox)input).SelectedValueChanged += UIElementList_SelectedValueChanged;
      checkForReadOnly();
    }

    private void UIElementList_SelectedValueChanged(object sender, EventArgs e)
    {
      _value = ((ComboBox)input).SelectedItem;
      int index = ((ComboBox)input).SelectedIndex;
      if (_prop.PropertyType == typeof(int))   // dirty little hack for backward  compatibily
        _prop.SetValue(_dataContainer, index, null); // with some early defined property
      else
        _prop.SetValue(_dataContainer, _value, null);
      if (_valueChangeCallback != null) _valueChangeCallback(this);
      if (_changeCausesParentRefresh) parentNode.refresh();
    }
    protected object _value = null;
    public object value
    {
      get { return _value; }
      set
      {
        _value = value;
        if (input != null) ((ComboBox)input).SelectedItem = _value;
      }
    }

    public override int resize(int top, int left1, int left2, int width)
    {
      base.resize(top, left1, left2, width);
      if (mainlabel != null) mainlabel.Top = top + 3;
      return top + input.Height;
    }
  }

  /***
  ***  Color
  ***/
  class UIElementColor : UIElementGeneric
  {
    PictureBox previewBox = new PictureBox();
    ColorEdit Editor = null;
    int _availableWidthForEditor = 200;

    private void CreateEditor()
    {
      if (Editor != null) return;
      Editor = new ColorEdit();
      Editor.Visible = _expanded;
      Editor.Height = 300;
      Editor.Width = _availableWidthForEditor;
      YColor c;
      if (PreviewColor(out c)) Editor.value = c;

     


      Editor.OnColorChanged += Editor_OnColorChanged;
      preExpandIfNeeded();
      Editor.refresh();

    }
    public override string editorHelpText { get { return "Just click on the right button to open the color chosser or type directly the color as HSL:xxx or HSL:xxx where xxx is a 8 digit hex number (Alpha transparency is supported). You can also type a color literal name (Red, Yellow etc...).";  } }

    public UIElementColor(UIElementBaseParams p, object dataContainer, PropertyInfo prop) :
              base(p, dataContainer, prop)
    {
      expandable = true;
      expandCtrl.side = CustomFlatButtom.ButtonLocation.RIGHT;
      expandCtrl.Width = 2 * baseheight;

      previewBox.Width = 20;
      previewBox.Height = baseheight;
      if (_expanded) CreateEditor();

      YColor c;
      PreviewColor(out c);

    }

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {
        input = new TextBox();
        ((TextBox)input).BorderStyle = BorderStyle.None;
        input.AutoSize = false;
        input.BackColor = Color.White;
        input.Height = baseheight;
        input.Text = ((YColors.YColor)_prop.GetValue(_dataContainer, null)).ToString();
        new InputFieldManager(input, InputFieldManager.dataType.DATA_COLOR, false, Double.NaN, Double.NaN, Text_TextChanged);
     
        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;

      }
    }

    public override int tabReOrder(int index)
    {

      if (input != null) input.TabIndex = index++;
      if (expandCtrl != null) expandCtrl.TabIndex = index++;
      if (Editor != null) index = Editor.tabReOrder(index);

      return index;
    }

    public override void refresh()
    { // the color in the widget is not an YColor
      // so it's difficult to find the original YColor
      //if (_getValueCallback == null) return;
      //object o = _getValueCallback(this);
      //string s = ((Color)o).ToString();
      //if (s != input.Text) input.Text = s;
      if (Editor != null) Editor.refresh();
    }

    public override void removeFromEditor()
    {
      base.removeFromEditor();
      _parentPanel.Controls.Remove(previewBox);
      if (Editor != null) _parentPanel.Controls.Remove(Editor);
    }

    public override void addToEditor()
    {
      base.addToEditor();
      _parentPanel.Controls.Add(previewBox);
      if (Editor != null) _parentPanel.Controls.Add(Editor);
    }

    private void Editor_OnColorChanged(object source, ColorChangeEventArgs e)
    {
      input.Text = e.color.ToString();
    }

    protected override void expand()
    {
      if (Editor == null)
      {
        CreateEditor();
        _parentPanel.Controls.Add(Editor);
      }
      Editor.Visible = true;
      _rootNode.tabReOrder(0);

    }
    protected override void colapse()
    {
      if (Editor != null) Editor.Visible = false;

    }

    private void Text_TextChanged(object sender, EventArgs e)
    {
      YColor c;

      _value = input.Text;

      if (PreviewColor(out c))
      {
        _prop.SetValue(_dataContainer, c, null);    
        if (Editor != null) Editor.value = c;
        if (_valueChangeCallback != null) _valueChangeCallback(this);
        if (_changeCausesParentRefresh) parentNode.refresh();
      }
    
    }

    protected string _value = "";
    public string value
    {
      get { return _value; }
      set { _value = value; input.Text = value; }
    }

    private bool PreviewColor(out YColor c)
    {

      bool convertionOK;
      c = YColor.fromString(((TextBox)input).Text, out convertionOK);

      if (!_shown) return convertionOK;
      if (convertionOK)
      {
        int w = 20;
        int h = baseheight;
        Bitmap DrawArea = new Bitmap(w, h);
        Image previous = previewBox.Image;
        previewBox.Image = DrawArea;
        if (previous != null) previous.Dispose();
         Graphics g = Graphics.FromImage(DrawArea);
        g.FillRectangle(Brushes.White, 0, 0, w, h);
        g.FillRectangle(Brushes.Black, 0, 0, w >> 1, h >> 1);
        g.FillRectangle(Brushes.Black, w >> 1, h >> 1, w >> 1, h >> 1);
        g.FillRectangle(new SolidBrush(c.toColor()), 0, 0, w - 1, h - 1);
        g.DrawRectangle(Pens.Black, 0, 0, w - 1, h - 1);

      }
      return convertionOK;
    }

    public override int computeHeight()
    {
      if (!_shown) return 0;
      int h = baseheight;
      if (Editor != null) if (_expanded) h += Editor.Height;

      return h;

    }

    public override void show()
    {

      base.show();
      if (Editor != null) Editor.Visible = _expanded;
      previewBox.Visible = true;
    }

    public override void hide()
    {

      if (Editor != null) Editor.Visible = false;
      previewBox.Visible = false;
      base.hide();
    }

    protected override void initMainLabel()
    {
      base.initMainLabel();
      if (mainlabel != null) mainlabel.BackColor = Color.Transparent;
    }

    public override int resize(int top, int left1, int left2, int width)
    {

      base.resize(top, left1, left2, width);

      _availableWidthForEditor = width - left1 + 10;

      if ((_expandable) && (expandCtrl != null) && (expandCtrl != null) && (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT)) top += expandableVerticalOffset;

      if (Editor != null)
      {
        Editor.Top = top + input.Height;
        Editor.Left = left1;
        Editor.Width = _availableWidthForEditor;
        Editor.Visible = _expanded;
      }

      input.Width = width - left2 - 24 - expandCtrl.Width;
      input.Left = left2 + 22;
      if (previewBox != null)
      {
        previewBox.Left = left2;
        previewBox.Top = top;
        previewBox.Width = 20;
        previewBox.Height = baseheight;
      }

      int y = top + input.Height;
      if (Editor != null) y += (_expanded ? Editor.Height : 0);
      return y;
    }

  }

  /***
  ***  Marker Position
  ***/
  class UIElementMarkerPos : UIElementGeneric
  {

    int _availableWidthForEditor = 200;
    xAxisPosition _cachedValue = null;



    public UIElementMarkerPos(UIElementBaseParams p, object dataContainer, PropertyInfo prop) :
              base(p, dataContainer, prop)
    {


      expandable = true;
      expandCtrl.side = CustomFlatButtom.ButtonLocation.RIGHT;
      expandCtrl.Width = 2 * baseheight;
      expandCtrl.setType(CustomFlatButtom.ActionType.ACTION_PUSH, buttonPush);





    }

    const string extra = " You can also use the crosshair button to place the marker directly on the graph. ";

    public override string editorHelpText
    {
      
      get
      {
        string p = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        if (_cachedValue.relative)
          return "Format for relative values is \"00d00h00m00" + p + "00s\". " +
            " Here are some examples: \"" + TimeConverter.secTimeSpanToString(1122312) + "\" , "
                                         + TimeConverter.secTimeSpanToString(42) + "\" , "
                                         + TimeConverter.secTimeSpanToString(4980) + "\" , "
                                         + TimeConverter.secTimeSpanToString(12.3) + "\" , \"1.5h\" etc.."+ extra;


        else
          return "Format for relative values is " + xAxisPosition.DTdisplayformat + ". If not specified,  date will be set as today."+ extra;

            


      }
    
  }


    public void buttonPush(object sender, EventArgs e)
    {

      // Since the editor cahnge change properties, the 
      // marker position capture call is done through
      // the special xAxisPosition capture property
      xAxisPosition value = (xAxisPosition)_prop.GetValue(_dataContainer, null);
      value.capture = true;
      _prop.SetValue(_dataContainer, value, null);
      _valueChangeCallback(this);
    }

    public override void AllocateControls()
    {
      base.AllocateControls();
      if (input == null)
      {
        input = new TextBox();
        ((TextBox)input).BorderStyle = BorderStyle.None;
        ((TextBox)input).TextAlign  = HorizontalAlignment.Right;
        input.AutoSize = false;
        input.BackColor = Color.White;
        input.Height = baseheight;
        _cachedValue = ((xAxisPosition)_prop.GetValue(_dataContainer, null));
        input.Text = _cachedValue.ToString(); 
        new InputFieldManager(input, InputFieldManager.dataType.DATA_XAXISPOS, false, Double.NaN, Double.NaN, Text_TextChanged);

        input.LostFocus += control_LostFocus;
        input.GotFocus += control_GotFocus;

      }
    }

    public override int tabReOrder(int index)
    {

      if (input != null) input.TabIndex = index++;
      if (expandCtrl != null) expandCtrl.TabIndex = index++;
     

      return index;
    }

    public override void refresh()
    {
      if (input == null) return;

      if (_getValueCallback == null) return;

      

       _cachedValue = (xAxisPosition)_getValueCallback(this);
      
      bool ok = false;
      double ParsedValue;
      if (_cachedValue.TryParse(input.Text, out ParsedValue))
      {      
         ok = (_cachedValue.value == ParsedValue);
      }
      if (!ok)
      {
         expandCtrl.pushed = false;
         input.Text = _cachedValue.ToString();
      }
      checkForReadOnly();

    }

   

    

   

    private void Text_TextChanged(object sender, EventArgs e)
    {
      

      double ParsedValue;
      if (_cachedValue.TryParse(input.Text, out ParsedValue))
      {
        _cachedValue.value  = ParsedValue;      
        _prop.SetValue(_dataContainer, _cachedValue, null);
        if (_valueChangeCallback != null) _valueChangeCallback(this);
        if (_changeCausesParentRefresh) parentNode.refresh();
        input.BackColor = Color.White;
    
      } else input.BackColor = Color.Pink;

    }

   
    public string value
    {
      get { return _cachedValue.ToString(); }
      set {
        input.Text = value;

        }
    }

  

    public override int computeHeight()
    {
      if (!_shown) return 0;
      int h = baseheight;
    
      return h;

    }

 

    protected override void initMainLabel()
    {
      base.initMainLabel();
      if (mainlabel != null) mainlabel.BackColor = Color.Transparent;
    }

    public override int resize(int top, int left1, int left2, int width)
    {

      base.resize(top, left1, left2, width);

      _availableWidthForEditor = width - left1 + 10;

      if ((_expandable) && (expandCtrl != null) && (expandCtrl != null) && (expandCtrl.side == CustomFlatButtom.ButtonLocation.LEFT)) top += expandableVerticalOffset;

    

      input.Width = width - left2 - 2 - expandCtrl.Width;
      input.Left = left2 ;
    

      int y = top + input.Height;
  
      return y;
    }

  }


  public class InputFieldManager
  {
    public delegate void  InputFieldManagerChange(object sender, EventArgs e);

    public enum dataType { DATA_STRING, DATA_INT, DATA_POSITIVE_INT, DATA_STRICT_POSITIVE_INT , DATA_FLOAT, DATA_POSITIVE_FLOAT, DATA_STRICT_POSITIVE_FLOAT, DATA_COLOR, DATA_PATH, DATA_XAXISPOS  };

    BackgroundWorker pathChecker =null;

    Control _input;
    dataType _type;
    bool _allowEmpty;
    double _min;
    double _max;
    InputFieldManagerChange _callback ;
    ToolTip _tt =null;

    public static void nullCallback(object sender, EventArgs e) {}

    public InputFieldManager(Control input, dataType type, bool allowEmpty,  double min, double max, InputFieldManagerChange callback)
    {
      _input = input;
      _type = type;
      _allowEmpty = allowEmpty;
      _min= min;
      _max = max;
      _callback = callback;
      checkData();
      _input.TextChanged += InputChange;
    }

    void showError(string msg)
    {  if (_tt == null) _tt = new ToolTip();
   //   _tt.IsBalloon = true;
      _tt.InitialDelay = 0;
      _tt.ShowAlways = true;
   
      _tt.Show( msg, _input);

    }

    void clearError()
    {
      if (_tt == null) return;
      _tt.Dispose();
      _tt = null;

    }

    private void pathChecker_doWork(object sender, DoWorkEventArgs e)  

      {
        string path = (string)(e.Argument);
      if (!Directory.Exists(path)) e.Result = "Folder does not exists";
      else
      {
        if (!constants.MonoRunning) // don't know how to fo that on mono :-(
        {
          try
         
          
          {
            DirectorySecurity ds = Directory.GetAccessControl(path);
            e.Result = "";
          }
          catch (Exception) { e.Result = "Folder is readonly"; }
        }
        else e.Result = "";
      } 
        
      }

    private void pathChecker_workDone(object sender, RunWorkerCompletedEventArgs e)
    { if (e.Cancelled) return;
      string res = (string)(e.Result);
      if ( res=="") backGroundColorFeedback("", true);
      else backGroundColorFeedback(res, false);
      pathChecker = null;
    }

    bool backGroundColorFeedback(string ErrorMsg, bool state)
    {
      if (state)  clearError(); else
         showError( ErrorMsg);



      Color c = _input.BackColor;

      if ((state) && (c!= Color.White)) _input.BackColor = Color.White;
      if ((!state) && (c != Color.Pink)) _input.BackColor = Color.Pink;
      return state;

    }

    bool checkData()
    { int intValue;
      double doubleValue;

      if (!_input.Enabled) return true;
       
      string strValue = _input.Text;
      if (strValue == "") return backGroundColorFeedback("This field can't be empty",_allowEmpty);
     
      switch (_type)
      { case dataType.DATA_STRING:
           break;
        case dataType.DATA_INT:
           if (!int.TryParse(strValue, out intValue)) return backGroundColorFeedback("Integer value expected",false); 
           if (!Double.IsNaN(_min) && (intValue<_min)) return backGroundColorFeedback("Integer value greater than" + _min.ToString()+" expected",false);
           if (!Double.IsNaN(_max) && (intValue>_max)) return backGroundColorFeedback("Integer value lower than " + _min.ToString() + " expected",false);
           break;
        case dataType.DATA_POSITIVE_INT:
           if(!int.TryParse(strValue, out intValue)) return backGroundColorFeedback("Positive integer value expected", false);
           if (intValue<0) return backGroundColorFeedback("Positive integer  value expected", false);
           if (!Double.IsNaN(_min) && (intValue < _min)) return backGroundColorFeedback("Integer value greater than" + _min.ToString() + " expected",false);
           if (!Double.IsNaN(_max) && (intValue > _max)) return backGroundColorFeedback("Integer value lower than " + _min.ToString() + " expected", false);
           break;
        case dataType.DATA_STRICT_POSITIVE_INT:
           if (!int.TryParse(strValue, out intValue)) return backGroundColorFeedback("Strictly positive integer  value expected", false);
           if (intValue <= 0) return backGroundColorFeedback("Strictly positive integer  value expected", false);
           if (!Double.IsNaN(_min) && (intValue < _min)) return backGroundColorFeedback("Integer value greater than" + _min.ToString() + " expected", false);
           if (!Double.IsNaN(_max) && (intValue > _max)) return backGroundColorFeedback("Integer value lower than " + _min.ToString() + " expected", false);
           break;
        case dataType.DATA_FLOAT:
           if (!double.TryParse(strValue, out doubleValue)) return backGroundColorFeedback("Floating point value expected", false);
           if (!Double.IsNaN(_min) && (doubleValue < _min)) return backGroundColorFeedback("Floating point value greater than" + _min.ToString() + " expected",false);
           if (!Double.IsNaN(_max) && (doubleValue > _max)) return backGroundColorFeedback("Floating point value lower than " + _min.ToString() + " expected", false);
           break;
        case dataType.DATA_POSITIVE_FLOAT:
          if (!double.TryParse(strValue, out doubleValue)) return backGroundColorFeedback("Positive floating point value expected",(false));
          if (doubleValue < 0) return backGroundColorFeedback("Positive floating point value expected", false);
          if (!Double.IsNaN(_min) && (doubleValue < _min)) return backGroundColorFeedback("Floating point value greater than" + _min.ToString() + " expected", false);
          if (!Double.IsNaN(_max) && (doubleValue > _max)) return backGroundColorFeedback("Floating point value lower than " + _min.ToString() + " expected", false);
          break;
        case dataType.DATA_STRICT_POSITIVE_FLOAT:
          if (!double.TryParse(strValue, out doubleValue)) return backGroundColorFeedback("Strictly positive floating point value expected", false);
          if (doubleValue <= 0) return backGroundColorFeedback("Strictly positive floating point value expected",  false);
          if (!Double.IsNaN(_min) && (doubleValue < _min)) return backGroundColorFeedback("Floating point value greater than" + _min.ToString() + " expected", false);
          if (!Double.IsNaN(_max) && (doubleValue > _max)) return backGroundColorFeedback("Floating point value greater than" + _min.ToString() + " expected", false);
          break;
        case dataType.DATA_COLOR:
          bool convertionOK;
          YColor.fromString(strValue, out convertionOK);
          if (!convertionOK) return backGroundColorFeedback("Value color name/definition expected",false);
          break;
        case dataType.DATA_PATH:
          if (pathChecker != null) pathChecker.CancelAsync();
         
          pathChecker = new BackgroundWorker();
          pathChecker.WorkerSupportsCancellation = true;
          pathChecker.DoWork += pathChecker_doWork;
          pathChecker.RunWorkerCompleted+= pathChecker_workDone;
          pathChecker.RunWorkerAsync(strValue);
          return true;
         


      }
     
      return backGroundColorFeedback("",true);

    }

    void InputChange(object sender, EventArgs e)
    {

      if (checkData())
      {
        try
        {
          if (_callback != null) _callback(sender, e);
        }
        catch (TargetInvocationException  ex)
        {
          backGroundColorFeedback(ex.InnerException.Message, false);

        }
      }
    }


  }

}
