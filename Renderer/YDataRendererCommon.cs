using System;
using System.Collections.Generic;
using System.Drawing;

using System.Windows.Forms;
using System.Runtime.CompilerServices;

using System.Drawing.Imaging;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text;
using System.Drawing.Text;

namespace YDataRendering
{

  static class Ycolor
  {



    static int hsl2rgbInt(int temp1, int temp2, int temp3)
    {
      if (temp3 >= 170) return (int)((temp1 + 127) / 255);
      if (temp3 > 42)
      {
        if (temp3 <= 127) return (int)((temp2 + 127) / 255);
        temp3 = 170 - temp3;
      }
      return (int)((temp1 * 255 + (int)(temp2 - temp1) * (6 * temp3) + 32512) / 65025);
    }

    public static Color hsl2rgb(int H, int S, int L)
    {
      int R;
      int G;
      int B;
      int temp1;
      int temp2;
      int temp3;
      int r, g, b;

      if (S == 0)
      {
        r = L;
        g = L;
        b = L;
        return Color.FromArgb(((L & 0xff) << 16) | ((L & 0xff) << 8) | (L & 0xff));
      }
      if (L <= 127) temp2 = L * (int)(255 + S);
      else temp2 = (L + S) * (int)(255) - L * S;

      temp1 = (510) * L - temp2;

      // R
      temp3 = (H + 85);
      if (temp3 > 255) temp3 = temp3 - 255;
      R = hsl2rgbInt(temp1, temp2, temp3);

      // G
      temp3 = H;
      if (temp3 > 255) temp3 = temp3 - 255;
      G = hsl2rgbInt(temp1, temp2, temp3);

      // B
      if (H >= 85) temp3 = H - 85;
      else temp3 = H + 170;
      B = hsl2rgbInt(temp1, temp2, temp3);

      if (R > 255) R = 255;  // just in case
      if (G > 255) G = 255;
      if (B > 255) B = 255;

      return Color.FromArgb((R << 16) | (G << 8) | B);

    }

  }


  public delegate void logFct(string msg);

  public delegate void ResetCallBack(Proportional source);

  public delegate String ValueFormater(YDataRenderer source, double value);


  public delegate void getCaptureParamaters(YDataRenderer source,
                                              out YDataRenderer.CaptureType capturetype,
                                              out YDataRenderer.CaptureTargets captureTarget,
                                              out string captureFolder,
                                              out YDataRenderer.CaptureFormats captureSizePolicy,
                                              out int captureDPI,
                                              out int captureWidth,
                                              out int captureHeight

    );


  public abstract class GenericPanel
  {
    public enum HorizontalAlignPos {[Description("Left")] LEFT, [Description("Center")] CENTER, [Description("Right")] RIGHT };
    public enum VerticalAlignPos {[Description("Top")] TOP, [Description("Center")] CENTER, [Description("Bottom")] BOTTOM };


    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    protected YDataRenderer _parentRenderer = null;
 
    public enum TextAlign {[Description("Left")]LEFT, [Description("Center")]CENTER, [Description("Right")]RIGHT };

    private Object _directParent = null;
    public Object directParent { get { return _directParent; } }

    public GenericPanel(YDataRenderer parent, Object directParent)
    {
      this._directParent = directParent;
      this._parentRenderer = parent;
      this._font = new YFontParams(parent, this, 8, null);

    }

    protected bool _enabled = false;
    public bool enabled { get { return _enabled; } set { if (_enabled != value) { _enabled = value; _parentRenderer.clearCachedObjects(); _parentRenderer.redraw(); } } }

    

    private TextAlign _panelTextAlign = TextAlign.LEFT;
    public TextAlign panelTextAlign { get { return _panelTextAlign; } set { _panelTextAlign = value; if (_enabled) _parentRenderer.redraw(); } }

    private string _text = "";
    public string text { get { return _text; } set { _text = value; _parentRenderer.clearCachedObjects(); if (_enabled) _parentRenderer.redraw(); } }

    private Color _bgColor = Color.FromArgb(255, 255, 255, 192);
    public Color bgColor { get { return _bgColor; } set { _bgColor = value; _bgBrush = null; if (_enabled) _parentRenderer.redraw(); } }

    private Color _borderColor = Color.Black;
    public Color borderColor { get { return _borderColor; } set { _borderColor = value; _pen = null; if (_enabled) _parentRenderer.redraw(); } }

    private double _borderthickness = 1.0;
    public double borderthickness { get { return _borderthickness; } set { _borderthickness = value; _parentRenderer.clearCachedObjects(); _pen = null; if (_enabled) _parentRenderer.redraw(); } }

    private double _padding = 10;
    public double padding { get { return _padding; } set { _padding = value; _parentRenderer.clearCachedObjects(); if (_enabled) _parentRenderer.redraw(); } }

    private double _verticalMargin = 10;
    public double verticalMargin { get { return _verticalMargin; } set { _verticalMargin = value; if (_enabled) _parentRenderer.redraw(); } }

    private double _horizontalMargin = 10;
    public double horizontalMargin { get { return _horizontalMargin; } set { _horizontalMargin = value; if (_enabled) _parentRenderer.redraw(); } }


    private Brush _bgBrush = null;
    public Brush bgBrush
    {
      get
      {
        if (_bgBrush == null)
          _bgBrush = new SolidBrush(_bgColor);
        return _bgBrush;
      }
    }

    private Pen _pen = null;
    public Pen pen
    {
      get
      {
        _pen = new Pen(_borderColor, (float)_borderthickness);
        return _pen;
      }
    }

    YFontParams _font = null;
    public YFontParams font { get { return _font; } }

  }

   public class MessagePanel: GenericPanel
    {
      
     
        public MessagePanel(YDataRenderer parent, Object directParent): base(parent, directParent)
        { }
     
        private HorizontalAlignPos _panelHrzAlign = HorizontalAlignPos.CENTER;
        public HorizontalAlignPos panelHrzAlign { get { return _panelHrzAlign; } set { _panelHrzAlign = value; if (_enabled) _parentRenderer.redraw(); } }

        private VerticalAlignPos _panelVrtAlign = VerticalAlignPos.CENTER;
        public VerticalAlignPos panelVrtAlign { get { return _panelVrtAlign; } set { _panelVrtAlign = value; if (_enabled) _parentRenderer.redraw(); } }

      
    }

  public class AnnotationPanel : GenericPanel
  {

   

    public AnnotationPanel(YDataRenderer parent, Object directParent) : base(parent, directParent)
    { }


    private bool _overlap = false;
    public bool overlap { get { return _overlap; }
      set {
        if ((!value) && (_panelHrzAlign == HorizontalAlignPos.CENTER) && (_panelVrtAlign == VerticalAlignPos.CENTER))
        {
          panelVrtAlign = VerticalAlignPos.TOP;

        }
        _overlap = value;
        _parentRenderer.clearCachedObjects(); _parentRenderer.redraw(); } }


    private double _positionOffsetX = 50;
    public double positionOffsetX { get { return _positionOffsetX; } set { _positionOffsetX = value; _parentRenderer.clearCachedObjects(); if (_enabled) _parentRenderer.redraw(); } }
    private double _positionOffsetY = 50;
    public double positionOffsetY { get { return _positionOffsetY; } set { _positionOffsetY = value; _parentRenderer.clearCachedObjects(); if (_enabled) _parentRenderer.redraw(); } }

    private HorizontalAlignPos _panelHrzAlign = HorizontalAlignPos.CENTER;
    public HorizontalAlignPos panelHrzAlign
    { get { return _panelHrzAlign; }
      set
      { if ((!overlap) && (value == HorizontalAlignPos.CENTER) && (_panelVrtAlign == VerticalAlignPos.CENTER))
          _panelVrtAlign = VerticalAlignPos.TOP;
        _panelHrzAlign = value; _parentRenderer.clearCachedObjects();
        if (_enabled)  _parentRenderer.redraw();
      } }

    private VerticalAlignPos _panelVrtAlign = VerticalAlignPos.TOP;
    public VerticalAlignPos panelVrtAlign
      {
        get { return _panelVrtAlign; }
       set {
        if ((!overlap) && (value == VerticalAlignPos.CENTER) && (_panelHrzAlign == HorizontalAlignPos.CENTER))
          _panelHrzAlign = HorizontalAlignPos.RIGHT;
        _panelVrtAlign = value; _parentRenderer.clearCachedObjects();
         if (_enabled) _parentRenderer.redraw();
       } }


  }




  public class Zone
    {
        protected YDataRenderer _parentRenderer = null;
        private Object _directParent = null;
        public Object directParent { get { return _directParent; } }

        private Object _userData = null;
        public Object userData { get { return _userData; } set { _userData = value; } }

        protected virtual void resetCache() { }


        public Zone(YDataRenderer parentRenderer, Object directParent)
        {
            this._directParent = directParent;
            this._parentRenderer = parentRenderer;

        }

        private Brush _zoneBrush = null;
        public Brush zoneBrush
        {
            get
            {
                if (_zoneBrush == null) _zoneBrush = new SolidBrush(_color);
                return _zoneBrush;
            }
        }


        private Color _color = Color.FromArgb(128, 255, 0, 0);
        public Color color { get { return _color; } set { _color = value; _zoneBrush = null; if (visible) _parentRenderer.redraw(); } }

        private bool _visible = false;
        public bool visible { get { return _visible; } set { _visible = value; _parentRenderer.redraw(); } }

        public void set_minMax(double min, double max)
        {
            if (min > max) throw new ArgumentException("Min cannot be greater than max ");
            _min = min; _max = max; resetCache(); if (visible) _parentRenderer.redraw();

        }


    private double _min = 0;
    public double min { get { return _min; }

      set {
        if ((value >= _max)  && !YDataRenderer.minMaxCheckDisabled)
          throw new ArgumentException("Min cannot be greater than max (" + _max.ToString() + ")");
        _min = value; resetCache(); if (visible) _parentRenderer.redraw();
      } }

    private double _max = 100;
    public double max { get { return _max; }
      set {
        if ((value <= _min)  && !YDataRenderer.minMaxCheckDisabled)
          throw new ArgumentException("Max cannot be greater than min (" + _min.ToString() + ")");
        _max = value; resetCache(); if (visible) _parentRenderer.redraw();
      } }

  }


  public class Proportional
  {
    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    ResetCallBack _reset = null;
    YDataRenderer _parentRenderer = null;
    Object _directParent = null;
    double _refWidth = 1;
    double _refHeight = 1;
    double _refValue;
    List<double> valueStack = new List<double>();


    public Object directParent { get { return _directParent; } }

    public enum ResizeRule {[Description("Fixed")]FIXED, [Description("Relative to Width")]RELATIVETOWIDTH, [Description("Relatif to height")]RELATIVETOHEIGHT, [Description("Relative to Width and Height")] RELATIVETOBOTH };

    ResizeRule _resizeRule = ResizeRule.FIXED;

    private void set_refPoint()
    {
      _refWidth = Math.Max(1, _parentRenderer.usableUiWidth());
      _refHeight = Math.Max(1, _parentRenderer.usableUiHeight());
      _refValue = _value;
    }

    public ResizeRule resizeRule
    {
      get { return _resizeRule; }
      set
      {
        set_refPoint();
        _resizeRule = value;

      }

    }
    public void containerResizedPushNewCoef(double coef)
    {
      valueStack.Add(_value);
      _value = _refValue * coef;
      if (_reset != null) _reset(this);
    }
   

    public void containerResizedPop()
    {  if (valueStack.Count <= 0) throw new InvalidOperationException("Can't pop, empty stack.");
      _value = valueStack[valueStack.Count - 1];
      valueStack.RemoveAt(valueStack.Count - 1);
      if (_reset != null) _reset(this);
    }

    public static  double resizeCoef(ResizeRule rule, double refWidth, double refHeight,  double newWidth, double newHeight)
    { 
      
      switch (rule)
      {
        case ResizeRule.RELATIVETOWIDTH:  return   newWidth / refWidth;      
        case ResizeRule.RELATIVETOHEIGHT: return   newHeight / refHeight;
        case ResizeRule.RELATIVETOBOTH:   return Math.Min(newHeight / refHeight, newWidth / refWidth); // original
      }
      return 1.0;
    }



    public void containerResized(double newWidth, double newHeight)
    { _value = _refValue* resizeCoef(_resizeRule, _refWidth, _refHeight, newWidth, newHeight);
      if (_reset != null) _reset(this);
    }

    public void forceChangeCallback()
    {
      if (_reset != null) _reset(this);

    }



    public Proportional(double value, ResizeRule resizeRule, YDataRenderer parentRenderer, Object directParent, ResetCallBack resetCallBack)
    {
      _reset = resetCallBack;
      _parentRenderer = parentRenderer;
      _value = value;
      _resizeRule = resizeRule;
      _directParent = directParent;
      set_refPoint();
      parentRenderer.AddNewProportionalToSizeValue(this);


    }

    double _value;
    public double value
    {
      get { return _value; }
      set
      {
        _value = value;
        set_refPoint();
        if (_reset != null) _reset(this);
      }

    }







  }



  public delegate void fontChangeResetCallBack(YFontParams source);

  public class YFontParams
  {
    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    private YDataRenderer _parentRenderer = null;

    private fontChangeResetCallBack _fontChangeCallback = null;


    private Object _directParent = null;
    public Object directParent { get { return _directParent; } }


    public YFontParams(YDataRenderer parentRenderer, object directParent, int size, fontChangeResetCallBack fontChangeCallback)
    {
      this._parentRenderer = parentRenderer;
      _directParent = directParent;
      _fontChangeCallback = fontChangeCallback;
      this._size = new Proportional(size, Proportional.ResizeRule.FIXED, parentRenderer, this, ResetFont);

    }


    public YFontParams(YDataRenderer parent, Object directParent)
    {
      _directParent = directParent;

      this._parentRenderer = parent;
      this._size = new Proportional(10, Proportional.ResizeRule.FIXED, parent, this, ResetFont);
    }

    public void ResetFont(Proportional source)
    {
      _font = null;
      if (source != null) _parentRenderer.ProportionnalValueChanged(source);
    }


    private String _name = "Arial";
    public string name { get { return _name; } set { _name = value; ResetFont(null); _parentRenderer.redraw(); } }

    private Proportional _size;
    public Double size
    {
      get { return _size.value; }

      set
      {
        if (value <= 0) throw new ArgumentException("Size must be a positive value");
        _size.value = value;
        ResetFont(null);
        if (_fontChangeCallback != null) _fontChangeCallback(this);
        _parentRenderer.redraw();
      }
    }

    private bool _italic = false;
    public bool italic
    {
      get { return _italic; }
      set
      {
        if (_italic != value) { _italic = value; ResetFont(null); _parentRenderer.redraw(); }
      }
    }

    private bool _bold = false;
    public bool bold { get { return _bold; } set { if (_bold != value) { _bold = value; ResetFont(null); _parentRenderer.redraw(); } } }

    private Color _color = Color.Black;
    public Color color
    {
      get { return _color; }
      set { if (_color != value) { _color = value; _brush = null; _parentRenderer.redraw(); } }
    }

    private Nullable<Color> _alternateColor = null;
    public Nullable<Color> alternateColor
    {
      get { return _alternateColor; }
      set { if (_alternateColor != value) { _alternateColor = value; _brush = null; _parentRenderer.redraw(); } }
    }


    private Font _font = null;
    public Font fontObject
    {
      get
      {
        if (_font == null)
        {
          FontFamily f = new FontFamily(_name);
          bool i = _italic;
          bool b = _bold;
          if (!f.IsStyleAvailable(FontStyle.Italic)) i = false;
          if (!f.IsStyleAvailable(FontStyle.Bold)) b = false;
          float s = _size.value > 0 ? (float)_size.value : 1;

          

          try
          {
            _font = new Font(f, s, (b ? FontStyle.Bold : 0) | (i ? FontStyle.Italic : 0));

          }
          catch (Exception e)
          {
            _parentRenderer.log("can't create instanciate " + _name + " font (" + e.Message + "), falling back to Arial.");
            _font = new Font(new FontFamily("Arial"), (float)_size.value, (b ? FontStyle.Bold : 0) | (i ? FontStyle.Italic : 0));
          }

        }
        return _font;
      }
    }

    private Brush _brush = null;
    public Brush brushObject
    {
      get
      {
        if (_brush == null)
        {
          _brush = new SolidBrush(_alternateColor == null ? _color : (Color)_alternateColor);
        }
        return _brush;
      }
    }
  }
 
  public delegate void ProportionnalValueChangeCallback(Proportional source);

  public delegate string PatchAnnotationCallback(string  text);

  public abstract class YDataRenderer
  {
    private RegisterHotKeyClass _RegisKey = new RegisterHotKeyClass();
    protected int _redrawAllowed = 1;
    private int _refWidth = 1;
    private int _refHeight = 1;

    static public readonly string[] FloatToStrformats = { "0", "0", "0", "0.0", "0.00", "0.000", "0.0000" };

    protected PatchAnnotationCallback _PatchAnnotationCallback =null;

    public delegate void RendererDblClickCallBack(YDataRenderer source, MouseEventArgs eventArg);
    public delegate void RendererRightClickCallBack(YDataRenderer source, MouseEventArgs eventArg);

    protected List<AnnotationPanel> _annotationPanels;
    public List<AnnotationPanel> annotationPanels { get { return _annotationPanels; } }

    public enum CaptureFormats
    {
      [Description("Keep OriginalSize")]
      Keep,
      [Description("Fixed size")]
      Fixed,
      [Description("Fixed width, keep ration aspect")]
      FixedWidth,
      [Description("Fixed height, keep ration aspect")]
      FixedHeight
    };

   

    public enum CaptureType
    {
      [Description("Bitmap (PNG)")]
      PNG,
      [Description("Vector (SVG)")]
      SVG,

    };


    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public enum CaptureTargets { ToClipBoard, ToFile };

    static private bool _disableMinMaxCheck = false;
    static public bool minMaxCheckDisabled { get { return _disableMinMaxCheck; } set { _disableMinMaxCheck = value; } }

    public virtual void resetlegendPens() { }

    private getCaptureParamaters _getCaptureParameters = null;
    public getCaptureParamaters getCaptureParameters { get { return _getCaptureParameters; } set { _getCaptureParameters = value; } }

    public event RendererDblClickCallBack OnDblClick;
    public event RendererRightClickCallBack OnRightClick;

    protected List<MessagePanel> _messagePanels;
    public List<MessagePanel> messagePanels { get { return _messagePanels; } }


    public AnnotationPanel addAnnotationPanel()
    {
      AnnotationPanel p = new AnnotationPanel(this, this);
      _annotationPanels.Add(p);
      redraw();
      return p;
    }

    /*
     *  .NET 3.5 compatibility
     *
     *  To add NET3.5 compatibilty, just copy/paste the following lines
     *  at the end of the .csproj file, right before the </Project>
     *  closing tag.
     *  
     * <PropertyGroup>
     *   <!-- Allows the project to be compiled with .NET 3.5 (Windows XP  compatibity) -->
     *   <DefineConstants Condition="'$(TargetFrameworkVersion)'=='v3.5'">NET35</DefineConstants>
     * </PropertyGroup> 
     */

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void AllowRedraw()
    {
      _redrawAllowed--;
      if (_redrawAllowed < 0) throw new InvalidOperationException("Too many AllowRedraw calls");
      if (_redrawAllowed == 0) redraw();

    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void AllowRedrawNoRefresh()
    {
      _redrawAllowed--;
      if (_redrawAllowed < 0) throw new InvalidOperationException("Too many AllowRedraw calls");


    }



#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void DisableRedraw()
    {
      _redrawAllowed++;

    }

    List<Proportional> ProportionalToSizeValues = new List<Proportional>();

    public void AddNewProportionalToSizeValue(Proportional v)
    {
      if (!ProportionalToSizeValues.Contains(v)) ProportionalToSizeValues.Add(v);

    }

    protected bool canRedraw()
    {
      return (_redrawAllowed == 0);

    }

    protected PictureBox UIContainer;
    protected Form parentForm;
    protected logFct _logFunction;
    Timer redrawTimer = null;
    private Stopwatch watch = Stopwatch.StartNew();
    private int LastDrawTiming = 0;

    public void setPatchAnnotationCallback(PatchAnnotationCallback callback)
    { _PatchAnnotationCallback = callback;   } 

    public string patchAnnotation(string text)
    {
      text = text.Replace("\\n", "\n");
      if (text.IndexOf('$') < 0) return text;
      DateTime now = DateTime.Now;
      text = text.Replace("$DAY$", now.ToString("dd"));
      text = text.Replace("$MONTH$", now.ToString("MM"));
      text = text.Replace("$YEAR$", now.ToString("yy"));
      text = text.Replace("$HOUR$", now.ToString("hh"));
      text = text.Replace("$MINUTE$", now.ToString("mm"));
      text = text.Replace("$SECOND$", now.ToString("ss"));   
      if (_PatchAnnotationCallback!=null) text = _PatchAnnotationCallback(text);
       return text;

    }

    protected Nullable<Point> mousePosition()
    {
      Point p = parentForm.PointToClient(System.Windows.Forms.Cursor.Position);
      if ((p.X > UIContainer.Location.X) &&
         (p.Y > UIContainer.Location.Y) &&
         (p.X < UIContainer.Location.X + UIContainer.Width) &&
         (p.Y < UIContainer.Location.Y + UIContainer.Height)) return new Point { X = p.X - UIContainer.Location.X, Y = p.Y - UIContainer.Location.Y };
      return null;

    }

    private ProportionnalValueChangeCallback _proportionnalValueChangeCallback = null;
    public ProportionnalValueChangeCallback proportionnalValueChangeCallback
    {
      set
      {
        _proportionnalValueChangeCallback = value;
        //if (value != null)
        //foreach (Proportional p in ProportionalToSizeValues)
        //  p.forceChangeCallback();
        //
      }

    }


    public void ProportionnalValueChanged(Proportional source)
    {
      if (_proportionnalValueChangeCallback != null) _proportionnalValueChangeCallback(source);



    }



    public int Draw()
    {

      if (!canRedraw()) return 0;

      int w = UIContainer.Size.Width;
      int h = UIContainer.Size.Height;

      if ((w <= 5) || (h <= 5)) return 0;

      DisableRedraw();

      Bitmap DrawArea = new Bitmap(w, h);
      UIContainer.Image = DrawArea;
      YGraphics g;

      watch.Reset();
      watch.Start();
      g = new YGraphics(DrawArea);

      try {
        Render(g, w, h);
      } catch (Exception e) { log("Rendering error: " + e.Message); }

      g.Dispose();
      long timing = watch.ElapsedMilliseconds;
     // log(" refresh done in " + timing.ToString() + "ms");
      AllowRedrawNoRefresh();
      renderingPostProcessing();
      return (int)timing;

    }


    protected virtual void renderingPostProcessing() { }

    protected abstract int Render(YGraphics g, int w, int h);





    public abstract void clearCachedObjects();

    Proportional.ResizeRule _resizeRule = Proportional.ResizeRule.FIXED;
    public Proportional.ResizeRule resizeRule
    {
      get { return _resizeRule; }
      set
      {

        if (value != _resizeRule)
        {
          DisableRedraw();
          _resizeRule = value;
          for (int i = 0; i < ProportionalToSizeValues.Count; i++)
            ProportionalToSizeValues[i].resizeRule = _resizeRule;
          AllowRedraw();
          redraw();

        }

      }
    }


    public void redraw()
    {
      if (parentForm.WindowState == FormWindowState.Minimized) return;
      if (_redrawAllowed > 0) return;
      if (UIContainer.Height < 2) return;
      if (UIContainer.Width < 2) return;
      if (redrawTimer.Enabled) return;

      int elapsed = (int)watch.ElapsedMilliseconds;
      // log("elapsed" + elapsed.ToString());

      redrawTimer.Enabled = false;

      int timelimit = 40; // no need to refresh  more then 25 times per seconds
      if (timelimit < LastDrawTiming) timelimit = (15 * LastDrawTiming) / 10; // refresh starting to get very slow, don't try to go faster than the music
      if (timelimit > 1000) timelimit = 1000; // if last redraw took more then 1 sec, you are in deep sh*t anayway.



      if (elapsed < timelimit)
      {
        redrawTimer.Interval = timelimit - elapsed;
        redrawTimer.Enabled = true;
        //  log("postponed" + elapsed.ToString() + "<" + timelimit.ToString());
        return;
      }

      //log("drawing");
      LastDrawTiming = Draw();



    }




    private void TimerTick(object sender, EventArgs e)
    {
      redrawTimer.Enabled = false;
      redraw();

    }

    public virtual int usableUiWidth() { return UIContainer.Width; }
    public virtual int usableUiHeight() { return UIContainer.Height; }

    public void resetProportionalSizeObjectsCachePush(double newcoef)
    {
      clearCachedObjects();
      if (_resizeRule != Proportional.ResizeRule.FIXED)
        for (int i = 0; i < ProportionalToSizeValues.Count; i++)
          ProportionalToSizeValues[i].containerResizedPushNewCoef(newcoef);
    }






    public void resetProportionalSizeObjectsCachePop()
    {
      clearCachedObjects();
      if (_resizeRule != Proportional.ResizeRule.FIXED)
        for (int i = 0; i < ProportionalToSizeValues.Count; i++)
          ProportionalToSizeValues[i].containerResizedPop();
    }



    protected void resetProportionalSizeObjectsCache(double w, double h)
    {
      clearCachedObjects();
      if (_resizeRule != Proportional.ResizeRule.FIXED)
        for (int i = 0; i < ProportionalToSizeValues.Count; i++)
          ProportionalToSizeValues[i].containerResized(w, h);
    }

    public void containerResized()
    {
      containerResize(null, null);
    }



    private void containerResize(object sender, EventArgs e)
    {
      if (((Form)sender).WindowState == FormWindowState.Minimized) return;

      DisableRedraw();
      // log("resize " + ((Control)sender).Width.ToString() + "/" + ((Control)sender).Height.ToString());

      resetProportionalSizeObjectsCache(usableUiWidth(), usableUiHeight());
      AllowRedraw();
      redraw();

    }

    public void proportionnalsizeReset()
    {
      resetProportionalSizeObjectsCache(usableUiWidth(), usableUiHeight());
    }

    private void _snapshotTimer_Tick(object sender, EventArgs e)
    {

      _snapshotTimer.Enabled = false;
      _snapshotPanel.enabled = false;
    }
    /* can't get the clipboard transparency to work 
    static void WriteIntToByteArray(Byte[] target, int ofset, int size, bool bidon, UInt32 data)
    {
      while(size>0)
      {
        target[ofset++] = (byte)( data & 0xff);
        data >>= 8;
        size--;

      }

    }

    static Byte[] GetImageData(Bitmap scaledBitmap)
    {
      Rectangle rect = new Rectangle(0, 0, scaledBitmap.Width, scaledBitmap.Height);
      var bitmapData = scaledBitmap.LockBits(rect, ImageLockMode.ReadWrite,  scaledBitmap.PixelFormat);
      var length = bitmapData.Stride * bitmapData.Height;

      Byte[] bytes = new byte[length];

      // Copy bitmap to byte[]
      Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
      scaledBitmap.UnlockBits(bitmapData);
      return bytes;
    }

    /// <summary>
    /// Converts the image to Device Independent Bitmap format of type BITFIELDS.
    /// This is (wrongly) accepted by many applications as containing transparency,
    /// so I'm abusing it for that.
    /// </summary>
    /// <param name="image">Image to convert to DIB</param>
    /// <returns>The image converted to DIB, in bytes.</returns>
    public static Byte[] ConvertToDib(Image image)
    {
      Byte[] bm32bData;
      Int32 width = image.Width;
      Int32 height = image.Height;
      // Ensure image is 32bppARGB by painting it on a new 32bppARGB image.
      using (Bitmap bm32b = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb))
      {
        using (Graphics gr = Graphics.FromImage(bm32b))
          gr.DrawImage(image, new Rectangle(0, 0, bm32b.Width, bm32b.Height));
        // Bitmap format has its lines reversed.
        bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);

        bm32bData = GetImageData(bm32b);
        for (int i = 0; i < bm32bData.Length / 4; i++) bm32bData[i*4+3] = 127;
      }
      // BITMAPINFOHEADER struct for DIB.
      Int32 hdrSize = 0x28;
      Byte[] fullImage = new Byte[hdrSize + 12 + bm32bData.Length];
      //Int32 biSize;
     WriteIntToByteArray(fullImage, 0x00, 4, true, (UInt32)hdrSize);
      //Int32 biWidth;
      WriteIntToByteArray(fullImage, 0x04, 4, true, (UInt32)width);
      //Int32 biHeight; 
     WriteIntToByteArray(fullImage, 0x08, 4, true, (UInt32)height);
      //Int16 biPlanes;
      WriteIntToByteArray(fullImage, 0x0C, 2, true, 1);
      //Int16 biBitCount;
      WriteIntToByteArray(fullImage, 0x0E, 2, true, 32);
      //BITMAPCOMPRESSION biCompression = BITMAPCOMPRESSION.BITFIELDS;
      WriteIntToByteArray(fullImage, 0x10, 4, true, 3);
      //Int32 biSizeImage;
     WriteIntToByteArray(fullImage, 0x14, 4, true, (UInt32)bm32bData.Length);
      // These are all 0. Since .net clears new arrays, don't bother writing them.
      //Int32 biXPelsPerMeter = 0;
      //Int32 biYPelsPerMeter = 0;
      //Int32 biClrUsed = 0;
      //Int32 biClrImportant = 0;

      // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
      WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
      WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
      WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
      Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
      return fullImage;
    }

    /// <summary>
    /// Copies the given image to the clipboard as PNG, DIB and standard Bitmap format.
    /// </summary>
    /// <param name="image">Image to put on the clipboard.</param>
    /// <param name="imageNoTr">Optional specifically nontransparent version of the image to put on the clipboard.</param>
    /// <param name="data">Clipboard data object to put the image into. Might already contain other stuff. Leave null to create a new one.</param>
    public static void SetClipboardImage(Bitmap image, Bitmap imageNoTr, DataObject data)
    {


      Clipboard.Clear();
      if (data == null)
        data = new DataObject();
      if (imageNoTr == null)
        imageNoTr = image;
      using (MemoryStream pngMemStream = new MemoryStream())
      using (MemoryStream dibMemStream = new MemoryStream())
      {
        // // As standard bitmap, without transparency support
        //  data.SetData(DataFormats.Bitmap, true, imageNoTr);
        // As PNG. Gimp will prefer this over the other two.

        image.Save(pngMemStream, ImageFormat.Png);
        data.SetData("PNG", false, pngMemStream);

        // As DIB. This is (wrongly) accepted as ARGB by many applications.
        Byte[] dibData = ConvertToDib(image);
        dibMemStream.Write(dibData, 0, dibData.Length);
        data.SetData(DataFormats.Dib, false, dibMemStream);

        // The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
        Clipboard.SetDataObject(data, true);
      }
    }
    */

    public void capture() { _Regis_HotKey(); }

    void _Regis_HotKey()
    {


      YDataRenderer.CaptureType captureType = YDataRenderer.CaptureType.SVG;
      YDataRenderer.CaptureFormats captureSizePolicy = YDataRenderer.CaptureFormats.Keep;
      YDataRenderer.CaptureTargets captureTarget = YDataRenderer.CaptureTargets.ToClipBoard;
      string captureFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
      int captureWidth = 1024;
      int captureHeight = 1024;
      int captureDPI = 96;
      if (_getCaptureParameters != null) _getCaptureParameters(this, out captureType, out captureTarget,
                                                                     out captureFolder, out captureSizePolicy,
                                                                     out captureDPI, out captureWidth, out captureHeight);

      capture(captureType, captureTarget, captureFolder, captureSizePolicy, captureDPI, captureWidth, captureHeight);

    }
    public void capture(CaptureType captureType, CaptureTargets captureTarget,
                                               string captureFolder,
                                               YDataRenderer.CaptureFormats captureSizePolicy,
                                                int captureDPI,
                                               int captureWidth,
                                               int captureHeight)

    {
      string error = "";
      int w = 0;
      int h = 0;
      switch (captureSizePolicy)
      {
        case CaptureFormats.Fixed: w = captureWidth; h = captureHeight; break;
        case CaptureFormats.Keep: w = UIContainer.Size.Width; h = UIContainer.Size.Height; break;
        case CaptureFormats.FixedWidth: w = captureWidth; h = (int)(w * UIContainer.Size.Height / UIContainer.Size.Width); break;
        case CaptureFormats.FixedHeight: h = captureHeight; w = (int)(h * UIContainer.Size.Width / UIContainer.Size.Height); break;

      }



      if ((w <= 5) || (h <= 5)) return;

      DisableRedraw();

      Bitmap DrawArea = new Bitmap(w, h);


      YGraphics g;
      switch (captureType)
      { case CaptureType.PNG: g = new YGraphics(Graphics.FromImage(DrawArea), w, h, captureDPI); break;
        case CaptureType.SVG: g = new YGraphicsSVG(Graphics.FromImage(DrawArea), w, h, captureDPI); break;
        default: throw new InvalidOperationException("capture :unknown type");
      }


      if ((captureTarget == YDataRenderer.CaptureTargets.ToClipBoard) && (captureType == CaptureType.PNG))
        g.FillRectangle(new SolidBrush(parentForm.BackColor), 0, 0, w, h);



      double newCoef = Proportional.resizeCoef(Proportional.ResizeRule.RELATIVETOBOTH, refWidth, refHeight, w, h);

      log("start capture");
      resetProportionalSizeObjectsCachePush(newCoef);  // reset all size related cached objects
      bool renderok = false;
      _snapshotPanel.enabled = false;
      try
      {

        int t = Render(g, w, h);
        renderok = true;

      }
      catch (Exception e) { error = e.Message; log("Render error: " + error); }
      log("capture completed");
      resetProportionalSizeObjectsCachePop(); // reset all size related cached objects, again
      g.Dispose();


      DrawArea.SetResolution(captureDPI, captureDPI);  // note : this affects the behavior of graphics.MeasureString

      if (renderok)
      {

        if (captureTarget == YDataRenderer.CaptureTargets.ToClipBoard)
        {
          Clipboard.Clear();
          if (captureType == CaptureType.PNG) Clipboard.SetImage(DrawArea);
          if (captureType == CaptureType.SVG)
          {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(((YGraphicsSVG)g).get_svgContents());

            MemoryStream stream = new MemoryStream(bytes);

            Clipboard.SetData("image/svg+xml", stream);
          }


        }
        else
        {
          renderok = false;
          string[] f = new String[0];
          string filenamePNG = "";
          string filenameSVG = "";

          FileAttributes attr = 0;
          bool pathCheckOk = false;

          // get the file attributes for file or directory
          try
          {
            attr = File.GetAttributes(captureFolder);
            pathCheckOk = true;
          }
          catch (Exception) { };

          //detect whether its a directory or file
          if ((pathCheckOk) && (attr & FileAttributes.Directory) == FileAttributes.Directory)
          {
            if (Directory.Exists(captureFolder)) f = Directory.GetFiles(captureFolder);

            List<string> files = new List<string>();
            for (int i = 0; i < f.Length; i++) files.Add(f[i]);

            int n = 0;
            do
            {
              n++;
              filenamePNG = Path.Combine(captureFolder, "YoctoVisualisationCapture-" + n.ToString("D4") + ".png");
              filenameSVG = Path.Combine(captureFolder, "YoctoVisualisationCapture-" + n.ToString("D4") + ".svg");

            } while (files.Contains(filenamePNG) || files.Contains(filenameSVG));
          }
          else
          {
            filenamePNG = captureFolder;
            filenameSVG = captureFolder;

            captureFolder = Path.GetDirectoryName(captureFolder);

          }

          if (Directory.Exists(captureFolder))
            try
            {

              if (captureType == CaptureType.PNG) DrawArea.Save(filenamePNG, ImageFormat.Png);

              if (captureType == CaptureType.SVG) ((YGraphicsSVG)g).save(filenameSVG);


              renderok = true;
            }
            catch (Exception e) { error = e.Message; log("File save error: " + error); }
          else { error = "File save error:\nFolder \"" + captureFolder + "\"does not exists."; }

        }
      }



      if (renderok)
      {
        _snapshotPanel.text = (captureTarget == YDataRenderer.CaptureTargets.ToClipBoard) ? "Saved to clipboard" : "Saved to " + captureFolder;
        _snapshotPanel.bgColor = Color.FromArgb(200, 127, 255, 127);
      }
      else
      {
        _snapshotPanel.text = "Capture error, " + error;
        _snapshotPanel.bgColor = Color.FromArgb(200, 255, 127, 127);
      }
      AllowRedrawNoRefresh();

      _snapshotPanel.enabled = true;
      _snapshotTimer.Interval = 2000;
      _snapshotTimer.Enabled = true;
      redraw();
    }

    private void getFocus(object sender, EventArgs e)
    {
      if (((Form)sender).WindowState == FormWindowState.Minimized) return;

      try
      {

        if (_AllowPrintScreenCapture) _RegisKey.StarHotKey();

      }
      catch (Exception err)
      {
        _AllowPrintScreenCapture = false;
        log("Can't register PrintScrn key capture (" + err.Message + ")");
      }
      
    }

    public void gainFocus()
    {
      if (parentForm.Focused) return;
      parentForm.Focus();
    }

    private void lostFocus(object sender, EventArgs e)
    {
      if (_AllowPrintScreenCapture) _RegisKey.StopHotKey();
    }

    private MessagePanel _snapshotPanel = null;
    private Timer _snapshotTimer = null;

    private bool _AllowPrintScreenCapture = false;
    public bool AllowPrintScreenCapture { get { return _AllowPrintScreenCapture; } set { _AllowPrintScreenCapture = value; } }
    System.Diagnostics.Stopwatch DblClickWatch = null;

    public YDataRenderer(PictureBox UIContainer, logFct logFunction)
    {
      this.UIContainer = UIContainer;

      this.UIContainer.SizeMode = PictureBoxSizeMode.Normal;
      _logFunction = logFunction;
      parentForm = (Form)UIContainer.Parent;
      this._annotationPanels = new List<AnnotationPanel>();
      this._messagePanels = new List<MessagePanel>();

      DisableRedraw();
      _snapshotPanel = addMessagePanel();
      _snapshotPanel.panelTextAlign = MessagePanel.TextAlign.CENTER;
      _snapshotPanel.text = "Captured to clipboard";
      _snapshotPanel.panelHrzAlign = MessagePanel.HorizontalAlignPos.CENTER;
      _snapshotPanel.panelVrtAlign = MessagePanel.VerticalAlignPos.CENTER;
      _snapshotPanel.bgColor = Color.FromArgb(200, 0xcc, 0xf7, 0xa1);
      _snapshotPanel.font.size = 16;
      _snapshotTimer = new Timer();
      _snapshotTimer.Enabled = false;
      _snapshotTimer.Tick += _snapshotTimer_Tick;
      AllowRedrawNoRefresh();

      redrawTimer = new Timer();
      redrawTimer.Enabled = false;
      redrawTimer.Tick += TimerTick;
      // ProportionalToSizeValues = new List<Proportional>();
      this.parentForm.Resize += containerResize;





      _RegisKey.Keys = Keys.PrintScreen;
      _RegisKey.ModKey = 0;
      _RegisKey.WindowHandle = this.parentForm.Handle;
      _RegisKey.HotKey += new RegisterHotKeyClass.HotKeyPass(_Regis_HotKey);
      this.parentForm.Activated += getFocus;
      this.parentForm.Deactivate += lostFocus;


      UIContainer.Click += RendererCanvas_Click;
      UIContainer.DoubleClick += RendererCanvas_DoubleClick;

      resetRefrenceSize();

    }
    public void resetRefrenceSize()
    {
      _refWidth = UIContainer.Width;
      _refHeight = UIContainer.Height;
    }
    public int refWidth  { get { return _refWidth; } }
    public int refHeight { get { return _refHeight; } }

    private void RendererCanvas_Click(object sender, EventArgs e)
    {
      MouseEventArgs m = (MouseEventArgs)e;
      if(OnRightClick != null && m.Button == MouseButtons.Right) {
        OnRightClick(this, m);
        return;
      }
                
      // emulates a a double click, which not does seem to exist on touch-screens
      if (DblClickWatch == null)
      {
        DblClickWatch = System.Diagnostics.Stopwatch.StartNew();
        return;
      }

      DblClickWatch.Stop();
      var elapsedMs = DblClickWatch.ElapsedMilliseconds;
      DblClickWatch = System.Diagnostics.Stopwatch.StartNew();

      if (elapsedMs < 330) RendererCanvas_DoubleClick(sender, e);
    }


    private void RendererCanvas_DoubleClick(object sender, EventArgs e)
    {    
      MouseEventArgs m = (MouseEventArgs)e;
      if (OnDblClick != null) OnDblClick(this, m);
    }

    public MessagePanel addMessagePanel()
    {
      MessagePanel p = new MessagePanel(this, this);
      _messagePanels.Add(p);
      return p;

    }

    public void DrawMessagePanels(YGraphics g,  int viewPortWidth, int viewPortHeight)
    {
     

      g.SetClip(new Rectangle(0, 0, viewPortWidth, viewPortHeight));

      for (int i = 0; i < _messagePanels.Count; i++)
        if (_messagePanels[i].enabled)
        {
          MessagePanel p = _messagePanels[i];

          double AvailableWidth = viewPortWidth - 2 * p.padding - p.borderthickness;
          if (AvailableWidth < 100) AvailableWidth = 100;


          SizeF ssize = g.MeasureString(p.text, p.font.fontObject, (int)AvailableWidth);
          double panelWidth = ssize.Width + 2 * p.padding + p.borderthickness;
          double panelHeight = ssize.Height + 2 * p.padding + p.borderthickness;

          double x = 0;
          switch (p.panelHrzAlign)
          {
            case MessagePanel.HorizontalAlignPos.LEFT: x = p.horizontalMargin; break;
            case MessagePanel.HorizontalAlignPos.RIGHT: x = viewPortWidth - panelWidth - p.horizontalMargin; break;
            default: x = (viewPortWidth - panelWidth) / 2; break;
          }

          double y = 0;
          switch (p.panelVrtAlign)
          {
            case MessagePanel.VerticalAlignPos.TOP: y = p.verticalMargin; break;
            case MessagePanel.VerticalAlignPos.BOTTOM: y = viewPortHeight - panelHeight - p.verticalMargin; break;
            default: y = (viewPortHeight - panelHeight) / 2; break;
          }

          g.FillRectangle(p.bgBrush, (int)x, (int)y, (int)panelWidth, (int)panelHeight);
          if (p.borderthickness > 0) g.DrawRectangle(p.pen, (int)x, (int)y, (int)panelWidth, (int)panelHeight);

          StringFormat sf = new StringFormat();
          switch (p.panelTextAlign)
          {
            case MessagePanel.TextAlign.LEFT:
              sf.LineAlignment = StringAlignment.Near;
              sf.Alignment = StringAlignment.Near;
              break;
            case MessagePanel.TextAlign.RIGHT:
              sf.LineAlignment = StringAlignment.Far;
              sf.Alignment = StringAlignment.Far;
              break;
            default:
              sf.LineAlignment = StringAlignment.Center;
              sf.Alignment = StringAlignment.Center;
              break;
          }
          Rectangle r = new Rectangle((int)(x + p.padding + p.borderthickness / 2),

            (int)(y + p.padding + p.borderthickness / 2),
                  (int)ssize.Width + 1, (int)ssize.Height + 1);
          g.DrawString(p.text, p.font.fontObject, p.font.brushObject, r, sf);


        }
    }

    public void drawAnnotationPanels(YGraphics g, List<AnnotationPanel>  annotationPanels , int viewPortWidth, int viewPortHeight, bool overlap, ref ViewPortSettings mainViewPort)
    {

      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
      bool active = false;
      for (int i = 0; i < annotationPanels.Count; i++)
        if (annotationPanels[i].enabled) active = true;
      if (!active) return;



      g.SetClip(new Rectangle(0, 0, viewPortWidth, viewPortHeight));

      for (int i = 0; i < annotationPanels.Count; i++)
        if ((annotationPanels[i].enabled) && (annotationPanels[i].overlap == overlap))
        {
          AnnotationPanel p = annotationPanels[i];
          double AvailableWidth = viewPortWidth - 2 * p.padding - p.borderthickness;
          if (AvailableWidth < 100) AvailableWidth = 100;

          string textToDisplay = p.text.Replace("\\n", "\n");

          if (textToDisplay.IndexOf('$') >= 0)
          {
            textToDisplay = textToDisplay.Replace("\\n", "\n");
            textToDisplay = patchAnnotation(textToDisplay);

          }

          SizeF ssize = g.MeasureString(textToDisplay, p.font.fontObject, (int)AvailableWidth);
          double panelWidth = ssize.Width + 2 * p.padding + p.borderthickness;
          double panelHeight = ssize.Height + 2 * p.padding + p.borderthickness;



          double x = 0;
          switch (p.panelHrzAlign)
          {
            case MessagePanel.HorizontalAlignPos.LEFT:
              x = p.horizontalMargin;
              if (!annotationPanels[i].overlap && (mainViewPort.Lmargin < panelWidth + 10))
                mainViewPort.Lmargin = (int)panelWidth + 10;
              break;
            case MessagePanel.HorizontalAlignPos.RIGHT:
              x = viewPortWidth - panelWidth - p.horizontalMargin;
              if (!annotationPanels[i].overlap && (mainViewPort.Rmargin < panelWidth + 20))
                mainViewPort.Rmargin = (int)panelWidth + 20;
              break;

            default: x = (viewPortWidth - panelWidth) / 2; break;
          }


          double y = 0;
          switch (p.panelVrtAlign)
          {
            case MessagePanel.VerticalAlignPos.TOP:
              y = p.verticalMargin;
              if (!annotationPanels[i].overlap && (mainViewPort.Tmargin < panelHeight + 20))
                mainViewPort.Tmargin = (int)panelHeight + 20;
              break;
            case MessagePanel.VerticalAlignPos.BOTTOM:
              y = viewPortHeight - panelHeight - p.verticalMargin;
              if (!annotationPanels[i].overlap && (mainViewPort.Bmargin < panelHeight + 20))
                mainViewPort.Bmargin = (int)panelHeight + 20;
              break;
            default: y = (viewPortHeight - panelHeight) / 2; break;
          }

          if (annotationPanels[i].overlap)
          {
            x += (annotationPanels[i].positionOffsetX / 100) * (viewPortWidth - panelWidth);
            y += (annotationPanels[i].positionOffsetY / 100) * (viewPortHeight - panelHeight);

            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x > viewPortWidth - panelWidth) x = viewPortWidth - panelWidth;
            if (y > viewPortHeight - panelHeight) y = viewPortHeight - panelHeight;

          }


          g.FillRectangle(p.bgBrush, (int)x, (int)y, (int)panelWidth, (int)panelHeight);
          if (p.borderthickness > 0) g.DrawRectangle(p.pen, (int)x, (int)y, (int)panelWidth, (int)panelHeight);
          g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
         
          StringFormat sf = new StringFormat();
          switch (p.panelTextAlign)
          {
            case MessagePanel.TextAlign.LEFT:
              sf.LineAlignment = StringAlignment.Near;
              sf.Alignment = StringAlignment.Near;
              break;
            case MessagePanel.TextAlign.RIGHT:
              sf.LineAlignment = StringAlignment.Far;
              sf.Alignment = StringAlignment.Far;
              break;
            default:
              sf.LineAlignment = StringAlignment.Center;
              sf.Alignment = StringAlignment.Center;
              break;
          }
          Rectangle r = new Rectangle((int)(x + p.padding + p.borderthickness / 2),

            (int)(y + p.padding + p.borderthickness / 2),
                  (int)ssize.Width + 1, (int)ssize.Height + 1);
          g.DrawString(textToDisplay, p.font.fontObject, p.font.brushObject, r, sf);


        }


    }

    public void log(string s)
    {
      if (_logFunction == null) return;
      _logFunction(s);

    }



  }




  public class RegisterHotKeyClass  // thanks stackoverflow
  {
    private IntPtr m_WindowHandle = IntPtr.Zero;
    private MODKEY m_ModKey = MODKEY.MOD_CONTROL;
    private Keys m_Keys = Keys.A;
    private int m_WParam = 10000;
    private bool Star = false;
    private HotKeyWndProc m_HotKeyWnd = new HotKeyWndProc();

    public IntPtr WindowHandle
    {
      get { return m_WindowHandle; }
      set { if (Star) return; m_WindowHandle = value; }
    }
    public MODKEY ModKey
    {
      get { return m_ModKey; }
      set { if (Star) return; m_ModKey = value; }
    }
    public Keys Keys
    {
      get { return m_Keys; }
      set { if (Star) return; m_Keys = value; }
    }
    public int WParam
    {
      get { return m_WParam; }
      set { if (Star) return; m_WParam = value; }
    }

    public void StarHotKey()
    {
      if (m_WindowHandle != IntPtr.Zero)
      {
        if (!RegisterHotKey(m_WindowHandle, m_WParam, m_ModKey, m_Keys))
        {
          throw new Exception(GetLastError().ToString());
        }
        try
        {
          m_HotKeyWnd.m_HotKeyPass = new HotKeyPass(KeyPass);
          m_HotKeyWnd.m_WParam = m_WParam;
          m_HotKeyWnd.AssignHandle(m_WindowHandle);
          Star = true;
        }
        catch
        {
          StopHotKey();
        }
      }
    }
    private void KeyPass()
    {
      if (HotKey != null) HotKey();
    }
    public void StopHotKey()
    {
      if (Star)
      {
        if (!UnregisterHotKey(m_WindowHandle, m_WParam))
        {
          throw new Exception(GetLastError().ToString());
        }
        Star = false;
        m_HotKeyWnd.ReleaseHandle();
      }
    }


    public delegate void HotKeyPass();
    public event HotKeyPass HotKey;


    private class HotKeyWndProc : NativeWindow
    {
      public int m_WParam = 10000;
      public HotKeyPass m_HotKeyPass;
      protected override void WndProc(ref Message m)
      {
        if (m.Msg == 0x0312 && m.WParam.ToInt32() == m_WParam)
        {
          if (m_HotKeyPass != null) m_HotKeyPass.Invoke();
        }

        base.WndProc(ref m);
      }
    }

    public enum MODKEY
    {
      MOD_ALT = 0x0001,
      MOD_CONTROL = 0x0002,
      MOD_SHIFT = 0x0004,
      MOD_WIN = 0x0008,
    }

    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr wnd, int id, MODKEY mode, Keys vk);

    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr wnd, int id);

    [DllImport("kernel32.dll")]
    static extern uint GetLastError();
  }

  /*
   *  abstraction layer allowing to render in both bitmap and Vector(SVG) format 
   * 
   */
  public class YGraphics
  {
    protected Graphics _g;
    protected int _width = 0;
    protected int _height = 0;
    protected double _dpi = 0;
    protected Image _image = null;




    public YGraphics(Graphics g, int width, int height, double dpi)
    {
      _g = g;
      _width = width;
      _height = height;
      _dpi = dpi;

    }

    public YGraphics(Image img)
    {
      _image = img;
      _g = Graphics.FromImage(img);
      _width = img.Width;
      _height = img.Height;
      _dpi = img.HorizontalResolution;

    }

    public YGraphics(PictureBox container)
    {
      _image = null;
      _g = container.CreateGraphics();
      _width = container.Width;
      _height = container.Height;
      _dpi = 90;

    }


    public Graphics graphics { get { return _g; } }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawLine(Pen p, float x1, float y1, float x2, float y2) { _g.DrawLine(p, x1, y1, x2, y2); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawLine(Pen p, PointF p1, PointF p2) { _g.DrawLine(p, p1, p2); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void SetClip(Rectangle rect) { _g.SetClip(rect); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void ResetClip() { _g.ResetClip(); }
    
#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    
    public SizeF MeasureString(string text, Font font, int width)
    { return _g.MeasureString(text, font, width); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void FillRectangle(Brush brush, Rectangle rect) { _g.FillRectangle(brush, rect); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void FillRectangle(Brush brush, float x, float y, float width, float height) { _g.FillRectangle(brush, x, y, width, height); }


#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawRectangle(Pen pen, Rectangle rect) { _g.DrawRectangle(pen, rect); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawRectangle(Pen pen, float x, float y, float width, float height) { _g.DrawRectangle(pen, x, y, width, height); }


#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawString(string s, Font font, Brush brush, float x, float y) { _g.DrawString(s, font, brush, x, y); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format) { _g.DrawString(s, font, brush, point, format); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawString(string s, Font font, Brush brush, PointF point) { _g.DrawString(s, font, brush, point); }


#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format) { _g.DrawString(s, font, brush, layoutRectangle, format); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void Transform(float dx, float dy, float angle) { _g.TranslateTransform(dx, dy); _g.RotateTransform(angle); }



#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void ResetTransform() { _g.ResetTransform(); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void FillEllipse(Brush brush, int x, int y, int width, int height) { _g.FillEllipse(brush, x, y, width, height); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawEllipse(Pen pen, int x, int y, int width, int height) { _g.DrawEllipse(pen, x, y, width, height); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void FillPolygon(Brush brush, PointF[] points) { _g.FillPolygon(brush, points); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawPolygon(Pen pen, PointF[] points) { _g.DrawPolygon(pen, points); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawLines(Pen pen, PointF[] points) { _g.DrawLines(pen, points); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawLines(Pen pen, Point[] points) { _g.DrawLines(pen, points); }


#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual SizeF MeasureString(string text, Font font, int width, StringFormat stringFormat) { return _g.MeasureString(text, font, width, stringFormat); }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void Dispose() { _g.Dispose(); }

    public TextRenderingHint TextRenderingHint { get { return _g.TextRenderingHint; } set { _g.TextRenderingHint = value; } }

    public SmoothingMode SmoothingMode { get { return _g.SmoothingMode; } set { _g.SmoothingMode = value; } }

    public int width { get { return _width; } }
    public int height { get { return _height; } }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
    { _g.DrawImage(image, destRect, srcRect, srcUnit); }


#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public virtual void comment(string s) { }

  }

  public class YGraphicsSVG : YGraphics
  {
    StringBuilder SVGdefs = null;
    StringBuilder SVGcontents = null;
    int clipcount = 0;
    int clipSectionsToClose = 0;
    int transformSectionsToClose = 0;
    int gradientCount = 0;
    static int SVGID = 0;
    private string color2svg(Color c) { return "rgb(" + c.R.ToString() + ", " + c.G.ToString() + ", " + c.B.ToString() + ")"; }
    private string alpha2svg(Color c) { return (c.A / 255.0).ToString("0.000"); }

    public YGraphicsSVG(Graphics g, int width, int height, double dpi) : base(g, width, height, dpi)
    {
      SVGID++;
      SVGdefs = new StringBuilder();
      SVGcontents = new StringBuilder();
      SVGdefs.AppendLine("<clipPath id=\"pageClip_" + SVGID.ToString() + "\"><rect x=\"0\" y=\"0\"  width=\"" + width.ToString() + "\" height=\"" + height.ToString() + "\"/></clipPath>");
    }

    public override void DrawLine(Pen p, float x1, float y1, float x2, float y2)
    {
      SVGcontents.AppendLine("<line x1=\"" + x1.ToString() + "\" "
                        + " y1 =\"" + y1.ToString() + "\" "
                        + " x2 =\"" + x2.ToString() + "\" "
                        + " y2 =\"" + y2.ToString() + "\" "
                        + "style = \"stroke:" + color2svg(p.Color) + ";stroke-opacity:" + alpha2svg(p.Color) + "; stroke-width:" + p.Width.ToString() + "\"/>");
    }

    public override void DrawLine(Pen p, PointF p1, PointF p2)
    { DrawLine(p, p1.X, p1.Y, p2.X, p2.Y); }

    public override void SetClip(Rectangle rect)
    {
      ResetClip();

      SVGdefs.AppendLine("<clipPath id=\"clip_" + SVGID.ToString() + "_" + clipcount.ToString() + "\"><rect x=\"" + rect.Left.ToString() + "\" y=\"" + rect.Top.ToString()
                                  + "\"  width=\"" + rect.Width.ToString() + "\" height=\"" + rect.Height.ToString() + "\"/></clipPath>");
      SVGcontents.AppendLine("<g clip-path=\"url(#clip_" + SVGID.ToString() + "_" + clipcount.ToString() + ")\">");
      clipcount++;
      clipSectionsToClose++;
    }

    public override void ResetClip()
    {
      if (clipSectionsToClose > 0)
      {
        SVGcontents.AppendLine("</g>");
        clipSectionsToClose--;
      }
    }

    private String BrushToSVG(Brush brush, bool revert)
    {
      string fillParam = "";
      if (brush is SolidBrush)
        fillParam = "fill = \"" + color2svg(((SolidBrush)brush).Color) + "\" fill-opacity=\"" + alpha2svg(((SolidBrush)brush).Color) + "\" ";
      else if (brush is LinearGradientBrush)
      {
        LinearGradientBrush br = (LinearGradientBrush)brush;


        SVGdefs.AppendLine("<linearGradient id=\"grad_" + SVGID.ToString() + "_" + gradientCount + "\" "
                 + "x1=\"0%\" "                                 // over-simplified gradient transaltion as we only use full size vertical gradients.
                 + (revert ? "y1=\"100%\" " : "y1=\"0%\" ")    // Yes, I know, it's cheap, but i couldn't find any reliable .NET 3.5 way to retreive  
                 + "x2=\"0%\" "                                // LinearGradientBrush's stop points
                 + (revert ? "y2=\"0%\" " : "y2=\"100%\" ") + ">\r\n"
                 + "<stop offset=\"0%\" style =\"stop-color:" + color2svg(br.LinearColors[0]) + ";stop-opacity:" + alpha2svg(br.LinearColors[0]) + "\"/>\r\n"
                 + "<stop offset=\"100%\" style =\"stop-color:" + color2svg(br.LinearColors[1]) + ";stop-opacity:" + alpha2svg(br.LinearColors[1]) + "\"/>\r\n"
                 + "</linearGradient>");

        fillParam = "fill=\"url(#grad_" + SVGID.ToString() + "_" + gradientCount + ")\" ";

        gradientCount++;

      }
      else throw new ArgumentException("unsupported brush type.");
      return fillParam;
    }

    public override void FillRectangle(Brush brush, Rectangle rect)
    {
      SVGcontents.AppendLine("<rect x=\"" + rect.Left.ToString() + "\" "
                      + " y =\"" + rect.Top.ToString() + "\" "
                      + " width =\"" + rect.Width.ToString() + "\" "
                      + " height =\"" + rect.Height.ToString() + "\" "
                      + BrushToSVG(brush, true)
                      + "style=\"stroke-width:0\"/>");

    }

    public override void FillRectangle(Brush brush, float x, float y, float width, float height)
    { FillRectangle(brush, new Rectangle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height))); }


    public override void DrawRectangle(Pen pen, Rectangle rect)
    {
      SVGcontents.AppendLine("<rect x=\"" + rect.Left.ToString() + "\" "
                       + " y =\"" + rect.Top.ToString() + "\" "
                       + " width =\"" + rect.Width.ToString() + "\" "
                       + " height =\"" + rect.Height.ToString() + "\" "
                       + " fill=\"none\" "
                        + "style = \"stroke:" + color2svg(pen.Color) + ";stroke-opacity:" + alpha2svg(pen.Color) + "; stroke-width:" + pen.Width.ToString() + "\"/>");

    }

    public override void DrawRectangle(Pen pen, float x, float y, float width, float height)
    { DrawRectangle(pen, new Rectangle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height))); }


    public override void DrawEllipse(Pen pen, int x, int y, int width, int height)

    {
      SVGcontents.AppendLine("<ellipse  cx=\"" + (x + width / 2.0).ToString() + "\" "
                    + " cy =\"" + (y + height / 2.0).ToString() + "\" "
                    + " rx =\"" + (width / 2).ToString() + "\" "
                    + " ry =\"" + (height / 2).ToString() + "\" "
                    + " fill=\"none\" "
                     + "style = \"stroke:" + color2svg(pen.Color) + ";stroke-opacity:" + alpha2svg(pen.Color) + "; stroke-width:" + pen.Width.ToString() + "\"/>");
    }

    public override void FillEllipse(Brush brush, int x, int y, int width, int height)

    {
      SVGcontents.AppendLine("<ellipse  cx=\"" + (x + width / 2.0).ToString() + "\" "
                    + " cy =\"" + (y + height / 2.0).ToString() + "\" "
                    + " rx =\"" + (width / 2).ToString() + "\" "
                    + " ry =\"" + (height / 2).ToString() + "\" "
                    + BrushToSVG(brush, false)
                    + "style=\"stroke-width:0\"/>");
    }


    public override void DrawString(string s, Font font, Brush brush, float x, float y)
    {
     
      string[] tokens = s.Split('\n');
      for (int i = 0; i < tokens.Length; i++)
      { s = tokens[i];
        SVGcontents.AppendLine("<text x=\"" + x.ToString() + "\" y=\"" + (y + font.Size * 1.25).ToString() + "\" text-anchor=\"start\" "  // dominant-baseline=\"hanging\" " //Not supported in  Inkscape :-(
                    + "font-family=\"" + font.FontFamily.Name.ToString() + "\" "
                    + "font-size=\"" + font.SizeInPoints + "pt\" "
                    + "font-weight=\"" + (((font.Style & FontStyle.Bold) != 0) ? "bold" : "normal") + "\" "
                    + "font-style=\"" + (((font.Style & FontStyle.Italic) != 0) ? "italic" : "normal") + "\" "
                    + BrushToSVG(brush, false)
                    + "style=\"stroke-width:0\">\r\n"
                    + System.Security.SecurityElement.Escape(s)
                    + "\r\n</text>");
        y += (float)(font.Size * 1.5);
      }



    }

    public override void DrawString(string s, Font font, Brush brush, PointF point) { DrawString(s, font, brush, point.X, point.Y); }

    public override void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
    {

      SizeF totalsz = _g.MeasureString(s, font);

      double y = point.Y + font.Size * 1.25;

      switch (format.LineAlignment)
      {
        case StringAlignment.Near: break;
        case StringAlignment.Center: y += -totalsz.Height / 2; break;
        case StringAlignment.Far: y += -totalsz.Height; break;

      }

      string[] tokens =  s.Split('\n');

      for (int i = 0; i < tokens.Length; i++ )
      { s = tokens[i];
        SizeF sz = _g.MeasureString(s, font);
        double x = point.X;
        
        switch (format.Alignment)
        {
          case StringAlignment.Near: break;
          case StringAlignment.Center: x += -sz.Width / 2; break;
          case StringAlignment.Far: x += -sz.Width; break;

        }
     


        SVGcontents.AppendLine("<text x=\"" + x.ToString() + "\" y=\"" + y.ToString() + "\" text-anchor=\"start\" "  // dominant-baseline=\"hanging\" " //Not supported in  Inkscape :-(
                      + "font-family=\"" + font.FontFamily.Name.ToString() + "\" "
                      + "font-size=\"" + font.SizeInPoints + "pt\" "
                      + "font-weight=\"" + (((font.Style & FontStyle.Bold) != 0) ? "bold" : "normal") + "\" "
                      + "font-style=\"" + (((font.Style & FontStyle.Italic) != 0) ? "italic" : "normal") + "\" "
                      + BrushToSVG(brush, false)
                      + "style=\"stroke-width:0\">\r\n"
                      + System.Security.SecurityElement.Escape(s)
                      + "\r\n</text>");

        y +=(float) (font.Size * 1.5); 
      }

    }



    public override void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)

    {
      SizeF totalsz = _g.MeasureString(s, font);

      
     
      double y = layoutRectangle.Y + font.Size *1.25;

      switch (format.LineAlignment)
      {
        case StringAlignment.Near: break;
        case StringAlignment.Center: y += (layoutRectangle.Height - totalsz.Height) / 2; break;
        case StringAlignment.Far: y += (layoutRectangle.Height - totalsz.Height); break;

      }

      string[] tokens = s.Split('\n');

      for (int i = 0; i < tokens.Length; i++)
      { s = tokens[i];
        SizeF sz = _g.MeasureString(s, font);
        double x = layoutRectangle.X;
        switch (format.Alignment)
        {
          case StringAlignment.Near: break;
          case StringAlignment.Center: x += (layoutRectangle.Width - sz.Width) / 2; break;
          case StringAlignment.Far: x += (layoutRectangle.Width - sz.Width); break;

        }
    
      SVGcontents.AppendLine("<text x=\"" + x.ToString() + "\" y=\"" + y.ToString() + "\" text-anchor=\"start\" "  // dominant-baseline=\"hanging\" " //Not supported in  Inkscape :-(
                    + "font-family=\"" + font.FontFamily.Name.ToString() + "\" "
                    + "font-size=\"" + (font.SizeInPoints *1.1).ToString() + "pt\" "
                    + "font-weight=\"" + (((font.Style & FontStyle.Bold) != 0) ? "bold" : "normal") + "\" "
                    + "font-style=\"" + (((font.Style & FontStyle.Italic) != 0) ? "italic" : "normal") + "\" "
                    + BrushToSVG(brush, false)
                    + "style=\"stroke-width:0\">\r\n"
                    + System.Security.SecurityElement.Escape(s)
                    + "\r\n</text>");
        y += (float)(font.Size * 1.5);
      }

    }



    public override void Transform(float dx, float dy, float angle)
    {
      SVGcontents.AppendLine("<g transform=\"translate(" + dx.ToString() + " " + dy.ToString() + ") rotate(" + angle.ToString() + ")\">");
      transformSectionsToClose++;


    }


    public override void ResetTransform()
    {
      if (transformSectionsToClose > 0) { SVGcontents.AppendLine("</g>"); transformSectionsToClose--; }
    }

    public override void DrawPolygon(Pen pen, PointF[] points)
    {
      if (points.Length < 2) return;

      SVGcontents.Append("<path  d=\"M " + points[0].X.ToString() + " " + points[0].Y.ToString());
      for (int i = 1; i < points.Length; i += 1)
        SVGcontents.Append(" L " + points[i].X.ToString() + " " + points[i].Y.ToString());

      SVGcontents.AppendLine(" z\" fill=\"none\" "
                     + "style=\"stroke:" + color2svg(pen.Color) + ";stroke-opacity:" + alpha2svg(pen.Color) + "; stroke-width:" + pen.Width.ToString() + "\"/>");
    }

    public override void DrawLines(Pen pen, PointF[] points)
    {
      if (points.Length < 2) return;

      SVGcontents.Append("<path  d=\"M " + points[0].X.ToString() + " " + points[0].Y.ToString());
      for (int i = 1; i < points.Length; i += 1)
        SVGcontents.Append(" L " + points[i].X.ToString() + " " + points[i].Y.ToString());

      SVGcontents.AppendLine("\" fill=\"none\" "
                     + "style=\"stroke:" + color2svg(pen.Color) + ";stroke-opacity:" + alpha2svg(pen.Color) + "; stroke-linecap:round; stroke-linejoin:round;stroke-width:" + pen.Width.ToString() + "\"/>");
    }

    public override void DrawLines(Pen pen, Point[] points)
    {
      if (points.Length < 2) return;

      SVGcontents.Append("<path  d=\"M " + points[0].X.ToString() + " " + points[0].Y.ToString());
      for (int i = 1; i < points.Length; i += 1)
        SVGcontents.Append(" L " + points[i].X.ToString() + " " + points[i].Y.ToString());

      SVGcontents.AppendLine("\" fill=\"none\" "
                     + "style=\"stroke:" + color2svg(pen.Color) + ";stroke-opacity:" + alpha2svg(pen.Color) + ";stroke-linecap:round; stroke-linejoin:round;stroke-width:" + pen.Width.ToString() + "\"/>");
    }


    public override void FillPolygon(Brush brush, PointF[] points)
    {
      if (points.Length < 2) return;

      SVGcontents.Append("<path  d=\"M " + points[0].X.ToString() + " " + points[0].Y.ToString());
      for (int i = 1; i < points.Length; i += 1)
        SVGcontents.Append(" L " + points[i].X.ToString() + " " + points[i].Y.ToString());
      SVGcontents.AppendLine(" z\" " + BrushToSVG(brush, false)
                  + "style=\"stroke-width:0\"/>\r\n");
    }

    public override void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
    { throw new NotSupportedException("DrawImage not supported, find an other way."); }



    /*

    public string getDefs() { return SVGdefs.ToString(); }

    public string getContents()
    { string close = "";
      for (int i = 0; i < clipSectionsToClose; i++) close += "</g>";
      for (int i = 0; i < transformSectionsToClose; i++) close += "</g>";
      return SVGcontents.ToString() + close;
    }


    public  void DrawGraphics(YGraphicsSVG  src, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
    {
      SVGdefs.Append(src.getDefs());
      SVGcontents.AppendLine("<g transform=\"translate(" + destRect.Top + " " + destRect.Left + ")\">");
      SVGcontents.Append(src.getContents());
      SVGcontents.AppendLine("</g>");

    }*/


    public void save(string filename)
    {
      System.IO.File.WriteAllText(filename, get_svgContents());
    }

    public override void comment(string s) { SVGcontents.AppendLine("<!--" + s + "-->"); }

    public string get_svgContents()
    {
      string physicalWidth = (2.54 * (_width / _dpi)).ToString("0.000");
      string physicalheight = (2.54 * (_height / _dpi)).ToString("0.000");


      while (clipSectionsToClose > 0)
      {
        SVGcontents.AppendLine("</g>");
        clipSectionsToClose--;
      }
      while (transformSectionsToClose > 0)
      {
        SVGcontents.AppendLine("</g>");
        transformSectionsToClose--;
      }



      return "<?xml version = \"1.0\" standalone = \"no\" ?>\r\n"
                + "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\r\n"
                 + "<svg width = \"" + physicalWidth + "cm\" height = \"" + physicalheight + "cm\" viewBox = \"0 0 " + _width.ToString() + " " + _height.ToString() + "\" "
                 + "xmlns = \"http://www.w3.org/2000/svg\" version = \"1.1\" >\r\n"
                 + "<defs>\r\n"
                 + SVGdefs
                 + "</defs>\r\n"
                 + "<g clip-path=\"url(#pageClip_" + SVGID.ToString() + ")\">\r\n"
                 + SVGcontents
                 + "</g>\r\n"
                 + "</svg>\n";

    }
  }




}