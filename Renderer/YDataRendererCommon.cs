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
                                              out YDataRenderer.CaptureTargets captureTarget,
                                              out string captureFolder,
                                              out YDataRenderer.CaptureFormats captureSizePolicy,
                                              out int captureDPI,
                                              out int captureWidth,
                                              out int captureHeight

    );


  public class MessagePanel
  {
    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    private YDataRenderer _parentRenderer = null;

    public enum HorizontalAlign
    {
      [Description("Left")]
      LEFT,
      [Description("Center")]
      CENTER,
      [Description("Right")]
      RIGHT
    };
    public enum VerticalAlign {[Description("Top")] TOP, [Description("Center")]CENTER, [Description("Bottom")] BOTTOM };
    public enum TextAlign {[Description("Left")]LEFT, [Description("Center")]CENTER, [Description("Right")]RIGHT };

    private Object _directParent = null;
    public Object directParent { get { return _directParent; } }


    public MessagePanel(YDataRenderer parent, Object directParent)
    {
      this._directParent = directParent;
      this._parentRenderer = parent;
      this._font = new YFontParams(parent, this, 8, null);

    }

    private bool _enabled = false;
    public bool enabled { get { return _enabled; } set { if (_enabled != value) { _enabled = value; _parentRenderer.redraw(); } } }






    private HorizontalAlign _panelHrzAlign = HorizontalAlign.CENTER;
    public HorizontalAlign panelHrzAlign { get { return _panelHrzAlign; } set { _panelHrzAlign = value; if (_enabled) _parentRenderer.redraw(); } }

    private VerticalAlign _panelVrtAlign = VerticalAlign.CENTER;
    public VerticalAlign panelVrtAlign { get { return _panelVrtAlign; } set { _panelVrtAlign = value; if (_enabled) _parentRenderer.redraw(); } }

    private TextAlign _panelTextAlign = TextAlign.LEFT;
    public TextAlign panelTextAlign { get { return _panelTextAlign; } set { _panelTextAlign = value; if (_enabled) _parentRenderer.redraw(); } }



    private string _text = "";
    public string text { get { return _text; } set { _text = value; if (_enabled) _parentRenderer.redraw(); } }

    private Color _bgColor = Color.FromArgb(255, 255, 255, 192);
    public Color bgColor { get { return _bgColor; } set { _bgColor = value; _bgBrush = null; if (_enabled) _parentRenderer.redraw(); } }

    private Color _borderColor = Color.Black;
    public Color borderColor { get { return _borderColor; } set { _borderColor = value; _pen = null; if (_enabled) _parentRenderer.redraw(); } }

    private double _borderthickness = 1.0;
    public double borderthickness { get { return _borderthickness; } set { _borderthickness = value; _pen = null; if (_enabled) _parentRenderer.redraw(); } }

    private double _padding = 10;
    public double padding { get { return _padding; } set { _padding = value; if (_enabled) _parentRenderer.redraw(); } }

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


    public void containerResized(double newWidth, double newHeight)
    {

      switch (_resizeRule)
      {
        case ResizeRule.FIXED: return;
        case ResizeRule.RELATIVETOWIDTH:

          _value = _refValue * newWidth / _refWidth;
          break;
        case ResizeRule.RELATIVETOHEIGHT:

          _value = _refValue * newHeight / _refHeight;
          break;
        case ResizeRule.RELATIVETOBOTH:

          _value = _refValue * Math.Min(newHeight / _refHeight, newWidth / _refWidth);
          break;

      }

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

  public abstract class YDataRenderer
  {
    private RegisterHotKeyClass _RegisKey = new RegisterHotKeyClass();
    protected int _redrawAllowed = 1;
    

    public delegate void RendererDblClickCallBack(YDataRenderer source, MouseEventArgs eventArg);
    public delegate void RendererRightClickCallBack(YDataRenderer source, MouseEventArgs eventArg);    

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

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public enum CaptureTargets { ToClipBoard, ToPng };

    static private bool _disableMinMaxCheck = false;
    static public bool minMaxCheckDisabled { get { return _disableMinMaxCheck; } set { _disableMinMaxCheck = value; }  }
    


    private getCaptureParamaters _getCaptureParameters = null;
    public getCaptureParamaters getCaptureParameters { get { return _getCaptureParameters; } set { _getCaptureParameters = value; } }

    public event RendererDblClickCallBack OnDblClick;
    public event RendererRightClickCallBack OnRightClick;

    protected List<MessagePanel> _messagePanels;
    public List<MessagePanel> messagePanels { get { return _messagePanels; } }
#if !NET35
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void AllowRedraw()
    {
      _redrawAllowed--;
      if (_redrawAllowed < 0) throw new InvalidOperationException("Too many AllowRedraw calls");
      if (_redrawAllowed == 0) redraw();

    }

#if !NET35
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void AllowRedrawNoRefresh()
    {
      _redrawAllowed--;
      if (_redrawAllowed < 0) throw new InvalidOperationException("Too many AllowRedraw calls");


    }



#if !NET35
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
      Graphics g;

      watch.Reset();
      watch.Start();
      g = Graphics.FromImage(DrawArea);

      try {
        Render(g, w, h);
      } catch (Exception e) { log("Rendering error: " + e.Message); }

      g.Dispose();
      long timing = watch.ElapsedMilliseconds;
    //  log(" refresh done in " + timing.ToString() + "ms");
      AllowRedrawNoRefresh();
      renderingPostProcessing();
      return (int)timing;

    }


    protected virtual void renderingPostProcessing() { }

    protected abstract int Render(Graphics g, int w, int h);





    protected abstract void clearCachedObjects();

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

      string error = "";

      YDataRenderer.CaptureFormats captureSizePolicy = YDataRenderer.CaptureFormats.Keep;
      YDataRenderer.CaptureTargets captureTarget = YDataRenderer.CaptureTargets.ToClipBoard;
      string captureFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
      int captureWidth = 1024;
      int captureHeight = 1024;
      int captureDPI = 70;
      if (_getCaptureParameters != null) _getCaptureParameters(this, out captureTarget, out captureFolder, out captureSizePolicy,
                                                  out captureDPI, out captureWidth, out captureHeight);

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
      DrawArea.SetResolution(captureDPI, captureDPI);

      Graphics g;


      g = Graphics.FromImage(DrawArea);
      if (captureTarget == YDataRenderer.CaptureTargets.ToClipBoard)
        g.FillRectangle(new SolidBrush(parentForm.BackColor), 0, 0, w, h);

      resetProportionalSizeObjectsCache(w, h);  // reset all size related cached objects
      bool renderok = false;
      _snapshotPanel.enabled = false;
      try
      {

        int t = Render(g, w, h);
        renderok = true;

      }
      catch (Exception e) { error = e.Message; log("Render error: " + error); }

      resetProportionalSizeObjectsCache(UIContainer.Size.Width, UIContainer.Size.Height); // reset all size related cached objects, again
      g.Dispose();




      if (renderok)
      {

        if (captureTarget == YDataRenderer.CaptureTargets.ToClipBoard)
          Clipboard.SetImage(DrawArea);
        else
        {
          renderok = false;
          string[] f = new String[0];

          if (Directory.Exists(captureFolder)) f = Directory.GetFiles(captureFolder);

          List<string> files = new List<string>();
          for (int i = 0; i < f.Length; i++) files.Add(f[i]);
          string filename;
          int n = 0;
          do
          {
            n++;
            filename = Path.Combine(captureFolder, "YoctoVisualisationCapture-" + n.ToString("D4") + ".png");

          } while (files.Contains(filename));

          if (Directory.Exists(captureFolder))
            try
            {

              DrawArea.Save(filename, ImageFormat.Png);
              renderok = true;
            }
            catch (Exception e) { error = e.Message; log("PNG file save error: " + error); }
          else { error = "PNG file save error:\nFolder \"" + captureFolder + "\"does not exists."; }

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
      this._messagePanels = new List<MessagePanel>();

      DisableRedraw();
      _snapshotPanel = addMessagePanel();
      _snapshotPanel.panelTextAlign = MessagePanel.TextAlign.CENTER;
      _snapshotPanel.text = "Captured to clipboard";
      _snapshotPanel.panelHrzAlign = MessagePanel.HorizontalAlign.CENTER;
      _snapshotPanel.panelVrtAlign = MessagePanel.VerticalAlign.CENTER;
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
    }

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

    public void drawMessagePanels(Graphics g, int viewPortWidth, int viewPortHeight)
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
            case MessagePanel.HorizontalAlign.LEFT: x = p.horizontalMargin; break;
            case MessagePanel.HorizontalAlign.RIGHT: x = viewPortWidth - panelWidth - p.horizontalMargin; break;
            default: x = (viewPortWidth - panelWidth) / 2; break;
          }

          double y = 0;
          switch (p.panelVrtAlign)
          {
            case MessagePanel.VerticalAlign.TOP: y = p.verticalMargin; break;
            case MessagePanel.VerticalAlign.BOTTOM: y = viewPortHeight - panelHeight - p.verticalMargin; break;
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


}