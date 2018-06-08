using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Reflection;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;

namespace YColors
{



  public partial class ColorEditor : UserControl
  {
    ColorCursor C1RGB;
    ColorCursor C2RGB;
    ColorCursor C3RGB;
    ColorCursor C4RGB;

    ColorCursor C1HSL;
    ColorCursor C2HSL;
    ColorCursor C3HSL;
    ColorCursor C4HSL;

    //    private static IEnumerable<System.Reflection.PropertyInfo> predefinedColorProperties = typeof(Color)
    //       .GetProperties(BindingFlags.Public | BindingFlags.Static)
    //       .Where(p => p.PropertyType == typeof(Color));

    private static List<System.Reflection.PropertyInfo> predefinedColorProperties = (typeof(Color)
      .GetProperties(BindingFlags.Public | BindingFlags.Static)
      .Where(p => p.PropertyType == typeof(Color))).ToList();


    const int historyTileSize = 32;
    const int historyTileSpacing = 8;
    const int predefinedTileSize = 14;
    const int predefinedTileSpacing = 5;
    const int predefinedColorOffsetX = 4;
    const int predefinedColorOffsetY = 2;





    static List<YColor> _colorHistory = new List<YColor>();
    List<Color> predefinedColors = new List<Color>();
    static int maxHistoryLength =32;


    static public List<YColor> colorHistory { get { return _colorHistory; } }

    static public void  AddColorToHistory(YColor c)
    {
       _colorHistory.Insert(0,c);
      while  (_colorHistory.Count> maxHistoryLength) _colorHistory.RemoveAt(_colorHistory.Count-1);

    }

    class ColorComparer : IComparer<Color>
    {
      public int Compare(Color c1 ,Color c2)
      {  if (c1 == c2) return 0;
        if (c1.GetHue() < c2.GetHue()) return -1;
        if (c1.GetHue() > c2.GetHue()) return 1;

       

  
        if (c1.GetSaturation() < c2.GetSaturation()) return -1;
        if (c1.GetSaturation() > c2.GetSaturation()) return 1;


        if (c1.GetBrightness() < c2.GetBrightness()) return -1;
        if (c1.GetBrightness() > c2.GetBrightness()) return 1;

        return 0;

      }
    }

    

    public ColorEditor(YColor initialValue)
    {


     


      // WTF???   System.Drawing.Color predefinded values contains duplicates
      for(int i = 0;i < predefinedColorProperties.Count;i++)
      {
        Color c =  (Color)predefinedColorProperties[i].GetValue(null, null);
        if ((c.A == 255) && (c.Name!= "Transparent"))
        {
          bool found = false;
          for (int j = 0; j < predefinedColors.Count; j++)
            if ((c.R == predefinedColors[j].R) && (c.G == predefinedColors[j].G) && (c.B == predefinedColors[j].B)) found = true;


          if (!found) predefinedColors.Add(c);
        }

        predefinedColors.Sort(new ColorComparer());

     }




      if (_colorHistory.Count<=0)
      {
        _colorHistory.Add(YColor.FromAhsl(255, 0, 0, 0));
        _colorHistory.Add(YColor.FromAhsl(255, 0, 0, 51));
        _colorHistory.Add(YColor.FromAhsl(255, 0, 0, 102));
        _colorHistory.Add(YColor.FromAhsl(255, 0, 0, 153));
        _colorHistory.Add(YColor.FromAhsl(255, 0, 0, 204));
        _colorHistory.Add(YColor.FromAhsl(255, 0, 0, 255));

       
        _colorHistory.Add(YColor.FromAhsl(255, 0, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 42, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 85, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 96, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 127, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 170, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 192, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 212, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 226, 217, 214));


      }

      _value = initialValue;
      InitializeComponent();
      maxHistoryLength = (int)((pictureBoxHistory.Size.Width+ historyTileSpacing) / (historyTileSize + historyTileSpacing)) * (int)((pictureBoxHistory.Size.Height+ historyTileSpacing) / (historyTileSize + historyTileSpacing));

      BackColor= SystemColors.Control;
      ToolTip tt = new ToolTip();
      tt.SetToolTip(pictureBoxHistory, "Color history, click to choose, dbl-click to choose and validate");

      tt = new ToolTip();
      tt.SetToolTip(PredefinedPanel, "Predefined colors palette, click to choose, dbl-click to choose and validate");

      tt = new ToolTip();
      tt.SetToolTip(pictureBoxPreview, "Color Preview");

      tt = new ToolTip();
      tt.SetToolTip(textBoxColorCode, "Color code");

      textBoxColorCode.TextChanged += ColorCodeChanged;


      C1RGB = new ColorCursor( ColorCursor.ColorFunction.COLOR_R,  C1RGB_pictureBox, C1RGB_textBox, ValueChanged, _value);
      C2RGB = new ColorCursor( ColorCursor.ColorFunction.COLOR_G,  C2RGB_pictureBox, C2RGB_textBox, ValueChanged, _value);
      C3RGB = new ColorCursor( ColorCursor.ColorFunction.COLOR_B,  C3RGB_pictureBox, C3RGB_textBox, ValueChanged, _value);
      C4RGB = new ColorCursor( ColorCursor.ColorFunction.COLOR_A , C4RGB_pictureBox, C4RGB_textBox, ValueChanged, _value);

      C1HSL = new ColorCursor(ColorCursor.ColorFunction.COLOR_H,  C1HSL_pictureBox, C1HSL_textBox, ValueChanged, _value);
      C2HSL = new ColorCursor(ColorCursor.ColorFunction.COLOR_S,  C2HSL_pictureBox, C2HSL_textBox, ValueChanged, _value);
      C3HSL = new ColorCursor(ColorCursor.ColorFunction.COLOR_L,  C3HSL_pictureBox, C3HSL_textBox, ValueChanged, _value);
      C4HSL = new ColorCursor(ColorCursor.ColorFunction.COLOR_A,  C4HSL_pictureBox, C4HSL_textBox, ValueChanged, _value);


      autoSelectTab();



      pictureBoxHistory.MouseMove += HistoryMouseMove;
      pictureBoxHistory.MouseUp += HistoryMouseUp;
      pictureBoxHistory.MouseDoubleClick += HistoryMouseDoubleClick;

      PredefinedPanel.MouseMove += PredefinedMouseMove;
      PredefinedPanel.MouseUp += PredefinedMouseUp;
      PredefinedPanel.MouseDoubleClick += PredefinedMouseDoubleClick;


      DrawHistory();
      PreviewColor();
      DrawPredefinedColors();
    }

    private void autoSelectTab()
    {
      if (_value.isAPredefinedColor()) ColorTypeTab.SelectedIndex = 2;
      else ColorTypeTab.SelectedIndex = _value.isRGB ? 0 : 1;
    }

    private void updateTextBox()
    {
      textBoxColorCode.TextChanged -= ColorCodeChanged;
      textBoxColorCode.Text = _value.ToString();
      if (textBoxColorCode.Text.Length > 0)
      {
        textBoxColorCode.SelectionStart = textBoxColorCode.Text.Length ;
         textBoxColorCode.SelectionLength = 0;
      }
       textBoxColorCode.TextChanged += ColorCodeChanged;
    }


    private void ColorCodeChanged(object sender, EventArgs e)
    {
      bool ok;

      YColor c  = YColor.fromString(textBoxColorCode.Text,out ok);
      if (ok) { _value = c; PreviewColor(); }
    }

    private int findHistoryIndex(int x, int y)
    {  
      int spacing = historyTileSize + historyTileSpacing;
      if ((x % spacing > historyTileSize) || (y % spacing > historyTileSize)) return -1;
      int count = pictureBoxHistory.Width / spacing;
      int n = (int)(y / spacing) * count + x / spacing;
      if (n >= _colorHistory.Count) return -1;
      return n;

    }

    private void HistoryMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (findHistoryIndex(e.X, e.Y) >= 0) pictureBoxHistory.Cursor = Cursors.Hand;
      else pictureBoxHistory.Cursor = Cursors.Default;
   
    }

    private void HistoryMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left) return;
      int n = findHistoryIndex(e.X, e.Y);
      if (n < 0) return;
      _value = _colorHistory[n];
      ValueChanged(null, 0);
      PreviewColor();
      autoSelectTab();

    }

    void HistoryMouseDoubleClick(object sender, MouseEventArgs e)
    {
      HistoryMouseUp(sender, e);
      button1_Click(null, e);

    }

    private int findPredefinedIndex(int x, int y)
    { if (x < predefinedColorOffsetX) return -1;
      if (y < predefinedColorOffsetY) return -1;

      x -= predefinedColorOffsetX;
      y -= predefinedColorOffsetY;
      int spacing = predefinedTileSize + predefinedTileSpacing;
      if ((x % spacing > predefinedTileSize) || (y % spacing > predefinedTileSize)) return -1;
      int count = PredefinedPanel.Width / spacing;
      int n = (int)(y / spacing) * count + x / spacing;
      if (n >= predefinedColors.Count) return -1;
      return n;

    }

    private void PredefinedMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (findPredefinedIndex(e.X, e.Y) >= 0) PredefinedPanel.Cursor = Cursors.Hand;
      else PredefinedPanel.Cursor = Cursors.Default;

    }

    private void PredefinedMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left) return;
      int n = findPredefinedIndex(e.X, e.Y);
      if (n < 0) return;


      _value = YColor.fromColor(predefinedColors[n]);
      DrawPredefinedColors();
      ValueChanged(null, 0);
      PreviewColor();
     

    }

    void PredefinedMouseDoubleClick(object sender, MouseEventArgs e)
    {
      PredefinedMouseUp(sender, e);
      button1_Click(null, e);

    }




    void updateCursors()
    {
      if (_value.isRGB)
      {
        C1RGB.setNewColor(_value);
        C2RGB.setNewColor(_value);
        C3RGB.setNewColor(_value);
        C4RGB.setNewColor(_value);
      }
      else
      {
        C1HSL.setNewColor(_value);
        C2HSL.setNewColor(_value);
        C3HSL.setNewColor(_value);
        C4HSL.setNewColor(_value);
      }
    }

    void ValueChanged(ColorCursor source, byte value)
    {
      if ((source == C1HSL) )  _value = YColor.FromAhsl(C4HSL.value, value, C2HSL.value, C3HSL.value);
      if ((source == C1RGB) )  _value = YColor.FromArgb(C4RGB.value, value, C2RGB.value, C3RGB.value);
      if ((source == C2HSL) )  _value = YColor.FromAhsl(C4HSL.value, C1HSL.value, value, C3HSL.value);
      if ((source == C2RGB) )  _value = YColor.FromArgb(C4RGB.value, C1RGB.value, value, C3RGB.value);
      if ((source == C3HSL) )  _value = YColor.FromAhsl(C4HSL.value, C1HSL.value, C2HSL.value, value);
      if ((source == C3RGB) )  _value = YColor.FromArgb(C4RGB.value, C1RGB.value, C2RGB.value, value);
      if ((source == C4HSL) )  _value = YColor.FromAhsl(value, C1HSL.value, C2HSL.value, C3HSL.value);
      if ((source == C4RGB))   _value = YColor.FromArgb(value, C1RGB.value, C2RGB.value, C3RGB.value);


      updateCursors();
      PreviewColor();
    


    }


    private void DrawPredefinedColors()
    {

      PredefinedPanel.Height = (predefinedTileSize + predefinedTileSpacing) *(int)(1 + (predefinedColors.Count ) / (predefinedTileSize + predefinedTileSpacing));
      int w = PredefinedPanel.Size.Width;
      int h = PredefinedPanel.Size.Height;
      Bitmap DrawArea = new Bitmap(w, h);
      PredefinedPanel.Image = DrawArea;
      Graphics g = Graphics.FromImage(DrawArea);

      int x = predefinedColorOffsetX;
      int y = predefinedColorOffsetY;

      for (int i =0;i< predefinedColors.Count;i++)
      
      {
        Color colorPropertyValue = predefinedColors[i];
        Brush Brush = new SolidBrush(colorPropertyValue);

       // YColor.FromArgb(255, colorPropertyValue.R, colorPropertyValue.G, colorPropertyValue.B);
        g.FillRectangle(new SolidBrush(colorPropertyValue), x, y, predefinedTileSize, predefinedTileSize);
        Pen p = Pens.Black;
        if ((_value.isRGB) && (_value.A == colorPropertyValue.A) && (_value.R == colorPropertyValue.R) && (_value.G == colorPropertyValue.G) && (_value.B == colorPropertyValue.B)) p = new Pen(Color.Black, 3);


        g.DrawRectangle(p, x, y, predefinedTileSize - 1, predefinedTileSize - 1);
        x += predefinedTileSize + predefinedTileSpacing;
        if (x > w - predefinedTileSize) { x = predefinedColorOffsetX; y += predefinedTileSize + predefinedTileSpacing;  }
      }

      g.Dispose();


    }


    private void DrawHistory()
    {

     
      int w = pictureBoxHistory.Size.Width;
      int h = pictureBoxHistory.Size.Height;
      Bitmap DrawArea = new Bitmap(w, h);
      pictureBoxHistory.Image = DrawArea;
      Graphics g = Graphics.FromImage(DrawArea);
      int Xcount = w / (historyTileSize + historyTileSpacing);

      int n= 0;
      while ((n< maxHistoryLength) && n< (_colorHistory.Count))
      {
        int x = (n % Xcount) * (historyTileSize + historyTileSpacing);
        int y = (n / Xcount) * (historyTileSize + historyTileSpacing);
        g.FillRectangle(Brushes.White, x, y, historyTileSize , historyTileSize);
        g.FillRectangle(Brushes.Black, x, y, historyTileSize >> 1, historyTileSize >> 1);
        g.FillRectangle(Brushes.Black, x + (historyTileSize >> 1),y + (historyTileSize >> 1), historyTileSize >> 1, historyTileSize >> 1);
        g.FillRectangle(new SolidBrush(_colorHistory[n].toColor()), x,y,historyTileSize,historyTileSize);
        g.DrawRectangle(Pens.Black, x,y,historyTileSize-1,historyTileSize-1);
        n++;
      }

      g.Dispose();


    }

    private void PreviewColor()
    {
      int w = pictureBoxPreview.Size.Width;
      int h = pictureBoxPreview.Size.Height;


      Bitmap DrawArea = new Bitmap(w, h);
      pictureBoxPreview.Image = DrawArea;
      Graphics g = Graphics.FromImage(DrawArea);
      g.FillRectangle(Brushes.White, 0, 0, w, h);
      g.FillRectangle(Brushes.Black, 0, 0, w >>1, h>>1);
      g.FillRectangle(Brushes.Black, w >> 1, h >> 1, w >> 1, h >> 1);
      g.FillRectangle(new SolidBrush(_value.toColor()), 0, 0, w-1, h-1);
      g.DrawRectangle(Pens.Black, 0, 0, w-1, h-1);

      updateTextBox();
    }

    private bool _ok = false;
    public bool ok { get { return _ok; } }


    private void button1_Click(object sender, EventArgs e)
    {
      _ok = true;
      int found = -1;
      for (int i = 0; i < _colorHistory.Count; i++) if (_value == _colorHistory[i]) found = i;
      if (found>=0) _colorHistory.RemoveAt(found);
      _colorHistory.Insert(0, _value);
      while (_colorHistory.Count > maxHistoryLength) _colorHistory.RemoveAt(_colorHistory.Count - 1);
      

      this.ParentForm.Close();   

    }

    private YColor _value;

    public YColor value
    {
      get { return _value; }
      set { _value = value; }
    }

  

    private void ColorTypeTab_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (ColorTypeTab.SelectedIndex)
      { case 0  :  _value.setModelToRGB(); updateCursors();  break;
        case 1  :  _value.setModelToHSL(); updateCursors();  break;
        case 2  :  DrawPredefinedColors(); updateCursors();  break;

      }
    }

    private void textBoxColorCode_KeyDown(object sender, KeyEventArgs e)
    {

    

    }

    private void textBoxColorCode_Validating(object sender, CancelEventArgs e)
    {
      
    }

    private void ColorEditor_KeyPress(object sender, KeyPressEventArgs e)
    {
    
    }


    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if ((this.ActiveControl == textBoxColorCode) && (keyData == Keys.Return))
      {
        button1_Click(null, null);
        return true;
      }
      else
      {
        return base.ProcessCmdKey(ref msg, keyData);
      }
    }
  }

  delegate void ColorCursorChangeCallBack(ColorCursor source,byte value);

  class ColorCursor

  {
    Bitmap HueBackground;

    public enum ColorFunction { COLOR_R , COLOR_G, COLOR_B, COLOR_H, COLOR_S, COLOR_L, COLOR_A  };

    Byte _value;
    public Byte value
    { get { return _value; }
      set { _value = value; redraw(); }
    }

    
    PictureBox _bitmap;
    TextBox _textbox;
    ColorCursorChangeCallBack _callback;
    YColor _color;
    ColorFunction _colorFunction;
  //  ToolTip _tt; looks like tooltips messes with mouse events.
  
    
    public ColorCursor(ColorFunction colorFunction,    PictureBox bitmap, TextBox textbox, ColorCursorChangeCallBack callback, YColor initialColor)
    {
      
      _bitmap = bitmap;
      _textbox = textbox;
      _colorFunction = colorFunction;
      _callback = callback;
      //_tt = new ToolTip();
      
      HueBackground = new Bitmap(255, 10);
      Graphics g = Graphics.FromImage(HueBackground);

      for (int i = 0; i < 256; i++)
      {
        YColor c = YColor.FromAhsl(255, (byte)i, 255, 127);
        Pen p = new Pen(c.toColor(), 1);
        g.DrawLine(p, i, 0, i, 11);
      }
      g.Dispose();

    
      

      _bitmap.MouseDown += MouseAction;
      _bitmap.MouseMove += MouseAction;
      _textbox.Text = _value.ToString();
      _textbox.TextChanged += textBox_TextChanged;
      _textbox.MaxLength = 3;
      setNewColor(initialColor);
    }

    private void updateTextBox()
    { _textbox.TextChanged -= textBox_TextChanged;
      _textbox.Text = _value.ToString();
      _textbox.TextChanged += textBox_TextChanged;
    }


    public void setNewColor(YColor color)
    {
      _color = color;

      if ((_colorFunction == ColorFunction.COLOR_H) || (_colorFunction == ColorFunction.COLOR_R))
        _colorFunction = _color.isHSL ? ColorFunction.COLOR_H : ColorFunction.COLOR_R;

      if ((_colorFunction == ColorFunction.COLOR_S) || (_colorFunction == ColorFunction.COLOR_G))
        _colorFunction = _color.isHSL ? ColorFunction.COLOR_S : ColorFunction.COLOR_G;

      if ((_colorFunction == ColorFunction.COLOR_L) || (_colorFunction == ColorFunction.COLOR_B))
        _colorFunction = _color.isHSL ? ColorFunction.COLOR_L : ColorFunction.COLOR_B;

      switch (_colorFunction)
      {
        case ColorFunction.COLOR_H:  /*_tt.SetToolTip(_bitmap, "Hue");*/ _value = _color.H; updateTextBox(); break;
        case ColorFunction.COLOR_R:  /*_tt.SetToolTip(_bitmap, "Red"); */_value = _color.R; updateTextBox(); break;
        case ColorFunction.COLOR_G:  /*_tt.SetToolTip(_bitmap, "Green");*/ _value = _color.G; updateTextBox(); break;
        case ColorFunction.COLOR_B:  /*_tt.SetToolTip(_bitmap, "Blue"); */_value = _color.B; updateTextBox(); break;
        case ColorFunction.COLOR_S:  /*_tt.SetToolTip(_bitmap, "Saturation");*/ _value = _color.S; updateTextBox(); break;
        case ColorFunction.COLOR_L:  /*_tt.SetToolTip(_bitmap, "Luminosity"); */_value = _color.L; updateTextBox(); break;
        case ColorFunction.COLOR_A:  /*_tt.SetToolTip(_bitmap, "Alpha-transparency"); */_value = _color.A; updateTextBox(); break;
      }
      redraw();
    }


    private void textBox_TextChanged(object sender, EventArgs e)
    { byte v;
      if (byte.TryParse(_textbox.Text, out v))
      {
        _value = v;
        _callback(this, value);
        redraw();

      }

    }

    private void MouseAction(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left) return;
      double p = Math.Round((255 * ((double)e.X - 1) / _bitmap.Width));
      if (p < 0) p = 0; if (p > 255) p = 255;
      _value = (byte)p;
      _callback(this, value);
      //redraw();
      updateTextBox();

    }



    private void redraw()
    {
      int w = _bitmap.Size.Width;
      int h = _bitmap.Size.Height;


      Bitmap DrawArea = new Bitmap(w, h);
      _bitmap.Image = DrawArea;
     

      Graphics g = Graphics.FromImage(DrawArea);

      Rectangle contentsRectangle = new Rectangle( 1,  1, _bitmap.Width - 2, _bitmap.Height-2);
      Rectangle outsideRectangle = new Rectangle(0, 0, _bitmap.Width-1 , _bitmap.Height-1);

      switch (_colorFunction)
      {
        case ColorFunction.COLOR_H: g.DrawImage(HueBackground, contentsRectangle); break;
        case ColorFunction.COLOR_S:
          for (int i=0;i< _bitmap.Width-2;i++)
          { byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(  YColor.FromAhsl(255, _color.H, b , _color.L).toColor());
            g.DrawLine(p,i+1, 0, i+1, _bitmap.Height);
          }
          break;
        case ColorFunction.COLOR_L:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromAhsl(255, _color.H, _color.S, b).toColor());
            g.DrawLine(p, i+1, 0, i+1, _bitmap.Height);
          }
          break;


        case ColorFunction.COLOR_R:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromArgb(255, b, _color.G, _color.B).toColor());
            g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
          }
          break;

        case ColorFunction.COLOR_G:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromArgb(255, _color.R, b, _color.B).toColor());
            g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
          }
          break;

        case ColorFunction.COLOR_B:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromArgb(255, _color.R, _color.G, b).toColor());
            g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
          }
          break;


        case ColorFunction.COLOR_A:
          g.FillRectangle(Brushes.White, 0, 0, _bitmap.Width, _bitmap.Height);
          for (int i = 0; i < (_bitmap.Width-2) / 16; i++)
              g.FillRectangle(Brushes.Black, i*16,  i%2==0?0: (_bitmap.Height - 2) / 2,1+ (_bitmap.Width-2)/16,1+ (_bitmap.Height-2)/2);

          if (_color.isHSL)
            for (int i = 0; i < _bitmap.Width - 2; i++)
            {
              byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
              Pen p = new Pen(YColor.FromAhsl(b, _color.H, _color.S, _color.L).toColor());
              g.DrawLine(p, i+1, 0, i+1, _bitmap.Height);
            }
          else
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromArgb(b, _color.R, _color.G, _color.B).toColor());
            g.DrawLine(p, i+1, 0, i+1, _bitmap.Height);
          }


          break;





      }

      g.DrawRectangle(Pens.Black, outsideRectangle);
    

      int x = (int)Math.Round((_bitmap.Width - 2) * (double)this._value / 255.0);

      Point[] TopPolygon = new Point[3];
      TopPolygon[0] = new Point { X = 1 + x - 5, Y = 0 };
      TopPolygon[1] = new Point { X =  1 + x, Y =  5 };
      TopPolygon[2] = new Point { X =  1+x + 5, Y = 0 };

      Point[] BottomPolygon = new Point[3];
      BottomPolygon[0] = new Point { X =  + 1 + x - 5, Y = _bitmap.Height };
      BottomPolygon[1] = new Point { X =  + 1 + x, Y = _bitmap.Height - 5 };
      BottomPolygon[2] = new Point { X = 1 + x + 5, Y = _bitmap.Height };

      g.FillPolygon(Brushes.Black, TopPolygon);
      g.DrawPolygon(Pens.White, TopPolygon);

      g.FillPolygon(Brushes.Black, BottomPolygon);
      g.DrawPolygon(Pens.White, BottomPolygon);

      g.Dispose();

    }

  }

  // an improvement over regular .Net Color Classe:
  // this one remember if color was defined from  RGB
  // of HSL color space and can implicitly convert
  // colors literal names.  
  public struct YColor
  {



    private byte _A, _R, _G, _B, _H, _S, _L;
    private bool _fromHSL;


    public byte A { get { return _A; } }
    public byte R { get { return _R; } }
    public byte G { get { return _G; } }
    public byte B { get { return _B; } }
    public byte H { get { return _H; } }
    public byte S { get { return _S; } }
    public byte L { get { return _L; } }
    public bool isHSL { get { return _fromHSL; } }
    public bool isRGB { get { return !_fromHSL; } }

    public void setModelToHSL() { _fromHSL = true; }
    public void setModelToRGB() { _fromHSL = false; }


  //  private static IEnumerable<System.Reflection.PropertyInfo> knownColorProperties = typeof(Color)
  //     .GetProperties(BindingFlags.Public | BindingFlags.Static)
  //     .Where(p => p.PropertyType == typeof(Color));

    private static List<System.Reflection.PropertyInfo> knownColorProperties = (typeof(Color)
      .GetProperties(BindingFlags.Public | BindingFlags.Static)
      .Where(p => p.PropertyType == typeof(Color))).ToList();

    public static YColor FromArgb(byte A, byte R, byte G, byte B)
    {
      YColor it = new YColor();

      it._fromHSL = false;
      it._A = A; it._R = R; it._G = G; it._B = B;

      float _fR = (R / 255f);
      float _fG = (G / 255f);
      float _fB = (B / 255f);

      float _Min = Math.Min(Math.Min(_fR, _fG), _fB);
      float _Max = Math.Max(Math.Max(_fR, _fG), _fB);
      float _Delta = _Max - _Min;

      float fH = 0;
      float fS = 0;
      float fL = (float)((_Max + _Min) / 2.0f);

      if (_Delta != 0)
      {
        if (fL < 0.5f)
        {
          fS = (float)(_Delta / (_Max + _Min));
        }
        else
        {
          fS = (float)(_Delta / (2.0f - _Max - _Min));
        }


        if (_fR == _Max)
        {
          fH = (_fG - _fB) / _Delta;
        }
        else if (_fG == _Max)
        {
          fH = 2f + (_fB - _fR) / _Delta;
        }
        else if (_fB == _Max)
        {
          fH = 4f + (_fR - _fG) / _Delta;
        }
      }

      it._H = (byte)(255 * fH / 6);
      it._S = (byte)(255 * fS);
      it._L = (byte)(255 * fL);
      return it;

    }

    private static double ColorCalc(double c, double t1, double t2)
    {

      if (c < 0) c += 1d;
      if (c > 1) c -= 1d;
      if (6.0d * c < 1.0d) return t1 + (t2 - t1) * 6.0d * c;
      if (2.0d * c < 1.0d) return t2;
      if (3.0d * c < 2.0d) return t1 + (t2 - t1) * (2.0d / 3.0d - c) * 6.0d;
      return t1;
    }

    public static YColor FromAhsl(byte A, byte H, byte S, byte L)
    {
      YColor it = new YColor();
      it._fromHSL = true;
      it._A = A;
      it._H = H;
      it._S = S;
      it._L = L;

      if (S == 0)
      {
        it._R = L;
        it._G = L;
        it._B = L;
      }
      else
      {
        double Hue = (6 * (double)H) / 255;
        double Saturation = ((double)S / 255);
        double Luminosity = ((double)L / 255);



        double t1, t2;
        double th = Hue / 6.0d;

        if (Luminosity < 0.5d)
        {
          t2 = Luminosity * (1d + Saturation);
        }
        else
        {
          t2 = (Luminosity + Saturation) - (Luminosity * Saturation);
        }
        t1 = 2d * Luminosity - t2;

        double tr, tg, tb;
        tr = th + (1.0d / 3.0d);
        tg = th;
        tb = th - (1.0d / 3.0d);

        tr = ColorCalc(tr, t1, t2);
        tg = ColorCalc(tg, t1, t2);
        tb = ColorCalc(tb, t1, t2);
        it._R = (byte)Math.Round(tr * 255d);
        it._G = (byte)Math.Round(tg * 255d);
        it._B = (byte)Math.Round(tb * 255d);
      }
      return it;
    }

    public bool isAPredefinedColor()
    {
      if (_fromHSL) return false;
        if (_A != 255)  return false;
      foreach (var colorProperty in knownColorProperties)
      {
        var colorPropertyValue = (Color)colorProperty.GetValue(null, null);
        if (colorPropertyValue.R == _R
               && colorPropertyValue.G == _G
               && colorPropertyValue.B == _B)
        {
          return true;
        }
      }
      return false;

    }

    public override String ToString()
    {
      if (_fromHSL) return "HSL:" + _A.ToString("X2") + _H.ToString("X2") + _S.ToString("X2") + _L.ToString("X2");

      if ((_A == 0) && (_R == 0) && (_G == 0) && (_B == 0)) return "Transparent";

      if (_A == 255)
      {
        if ((_R==255) && (_G == 255) && (_B == 255)) return "White";  // there is a prefined Color matching white but named "transparent"; 

        foreach (var colorProperty in knownColorProperties)
        {
          var colorPropertyValue = (Color)colorProperty.GetValue(null, null);
          if (colorPropertyValue.R == _R
                 && colorPropertyValue.G == _G
                 && colorPropertyValue.B == _B)
          {
            return colorPropertyValue.Name;
          }
        }



      }

      return "RGB:" + _A.ToString("X2") + _R.ToString("X2") + _G.ToString("X2") + _B.ToString("X2");
    }

    public Color toColor()
    {
      return Color.FromArgb(_A, _R, _G, _B);
    }


    public static YColor fromColor(Color c)
    {
      return FromArgb(c.A, c.R, c.G, c.B);
    }
     


    public static YColor fromString(string s, out bool matchFound)
    {
      matchFound = false;
      s = s.Replace(" ", "");

      if (s.StartsWith("#"))  s = "RGB:" + s.Substring(1);
      if (s.ToUpper() == "LIGHTGREY") s = "RGB:FFD3D3D3";  // sometimes it's LightGrey, sometimes it's LightGray,
      if (s.ToUpper() == "TRANSPARENT") s = "RGB:00000000";  // 

      foreach (var colorProperty in knownColorProperties)
      {
        var colorPropertyValue = (Color)colorProperty.GetValue(null, null);
        if (String.Equals(colorProperty.Name, s, StringComparison.OrdinalIgnoreCase))
        {
          matchFound = true;
          return YColor.FromArgb(255, colorPropertyValue.R, colorPropertyValue.G, colorPropertyValue.B);
        }


      }



      bool fromHSL = false;
      if ((s.Length > 4))
      {
        if (s.Substring(0, 4).ToUpper() == "HSL:") { fromHSL = true; s = s.Substring(4); }
        else if (s.Substring(0, 4).ToUpper() == "RGB:") { s = s.Substring(4); }
      }

      long value;
      if (( s.Length>=6) && (long.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out value)))
      {
        byte a;
        if (s.Length < 7) a = 255; else a = (byte)(value >> 24);
        if (fromHSL) { matchFound = true; return YColor.FromAhsl(a, (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)((value) & 0xFF)); }
        else  { matchFound = true; return  FromArgb(a, (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)((value) & 0xFF)); }

      }

     
      return YColor.FromArgb(255, 0, 0, 0);



    }

    public static YColor fromString(string s)
    {
      bool ok;
      return fromString( s, out ok);

    }


    public static bool operator !=(YColor c1, YColor c2)

    {
      if ((c1._A == c2._A) && (c1._R == c2._R) && (c1._G == c2._G) && (c1._B == c2._B)) return false;
      if ((c1._A == c2._A) && (c1._H == c2._H) && (c1._S == c2._S) && (c1._L == c2._L)) return false;
      return true;

    }

    public static bool operator ==(YColor c1, YColor c2)

    {
      if ((c1._A == c2._A) && (c1._R == c2._R) && (c1._G == c2._G) && (c1._B == c2._B)) return true;
      if ((c1._A == c2._A) && (c1._H == c2._H) && (c1._S == c2._S) && (c1._L == c2._L)) return true;
      return false;

    }

    public override  bool Equals(Object o)
    {
      if ( o is YColor)
      {
        return this == (YColor)o;
      }
      return false;
    }

    public override int GetHashCode()
    {
      return ((int)_A << 24) | ((int)_R << 16) | ((int)_G << 8) |  (int)_B;
    }

  }


  public class YColorConverter : StringConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
    {
      if (destinationType == typeof(YColor)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(String)) return true;
      return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is String) return YColor.fromString((string)value);
      return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                object value, System.Type destinationType)
    {
      if (destinationType == typeof(YColor) && value is String)
      {
        YColor.fromString((string)value);
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }

  class YColorEditor : UITypeEditor
  {

    Brush White;
    Brush Black;

    public YColorEditor()
    {
      White = new SolidBrush(Color.White);
      Black = new SolidBrush(Color.Black);


    }




    public override bool GetPaintValueSupported(
    ITypeDescriptorContext context)
    {
      return true;
    }

    public override void PaintValue(PaintValueEventArgs e)
    {
      e.Graphics.FillRectangle(White, e.Bounds);
      e.Graphics.FillRectangle(Black, new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width >> 1, e.Bounds.Height >> 1));
      e.Graphics.FillRectangle(Black, new Rectangle(e.Bounds.Left + (e.Bounds.Width >> 1), e.Bounds.Top + (e.Bounds.Height >> 1), e.Bounds.Width >> 1, e.Bounds.Height >> 1));
      Brush b = new SolidBrush(((YColor)e.Value).toColor());
      e.Graphics.FillRectangle(b, e.Bounds);




    }



    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }
    public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
    {
      IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;


      ColorEditor control = new ColorEditor((YColor)value);



      svc.DropDownControl(control);
      if (control.ok) return control.value;
      return value;




    }
  }



}
