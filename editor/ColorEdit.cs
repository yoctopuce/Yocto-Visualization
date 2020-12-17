using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;


namespace YColors
{




  public partial class ColorEdit : UserControl
  {
    ColorCursor C1RGB;
    ColorCursor C2RGB;
    ColorCursor C3RGB;
    ColorCursor C4RGB;

    ColorCursor C1HSL;
    ColorCursor C2HSL;
    ColorCursor C3HSL;
    ColorCursor C4HSL;


    public event ColorChanged OnColorChanged;

    private static List<System.Reflection.PropertyInfo> predefinedColorProperties = (typeof(Color)
      .GetProperties(BindingFlags.Public | BindingFlags.Static)
      .Where(p => p.PropertyType == typeof(Color))).ToList();


     int historyTileSizeX = 32;
     int historyTileSizeY = 32;
     int historyTilesPerRow =5;
     const int historyTileSpacing = 8;
     const int predefinedTilesPerRow = 16;


    int predefinedTileSizeX = 14;
    int predefinedTileSizeY = 14;
   

    const int predefinedTileSpacing = 5;
    const int predefinedColorOffsetX = 4;
    const int predefinedColorOffsetY = 2;





    static List<YColor> _colorHistory = new List<YColor>();
    List<Color> predefinedColors = new List<Color>();
    static int maxHistoryLength =15;


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

   public void	Init(YColor initialValue)
	{ _ok = false;
	  _value = initialValue;
	  autoSelectTab();


    computeSpacing();

    DrawHistory();
	  PreviewColor();
	  DrawPredefinedColors();
	}

    public ColorEdit() : this(YColor.fromColor(Color.Red)) { }
    

    


    public ColorEdit(YColor initialValue)
    {

      TabStop = true;
     


      // WTF???   System.Drawing.Color predefined values contains duplicates
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
        _colorHistory.Add(YColor.FromAhsl(100, 0, 0, 0));
        _colorHistory.Add(YColor.FromAhsl(100, 0, 0, 51));
        _colorHistory.Add(YColor.FromAhsl(100, 0, 0, 102));
        _colorHistory.Add(YColor.FromAhsl(100, 0, 0, 153));
        _colorHistory.Add(YColor.FromAhsl(100, 0, 0, 204));
        _colorHistory.Add(YColor.FromAhsl(100, 0, 0, 255));

       
        _colorHistory.Add(YColor.FromAhsl(100, 0, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(100, 42, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(100, 85, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(100, 96, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(100, 127, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(100, 170, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(100, 192, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(100, 212, 255, 127));
        _colorHistory.Add(YColor.FromAhsl(255, 226, 217, 214));


      }

      _value = initialValue;
      this.InitializeComponent();
  
      BackColor= SystemColors.Control;
      ToolTip tt = new ToolTip();
      tt.SetToolTip(pictureBoxHistory, "Color history");

      tt = new ToolTip();
      tt.SetToolTip(PredefinedPanel, "Predefined colors palette");

      tt = new ToolTip();
      tt.SetToolTip(pictureBoxPreview, "Color Preview");

    

   

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

    

      computeSpacing();

      DrawHistory();
      PreviewColor();
      DrawPredefinedColors();
    }

    private void autoSelectTab()
    {
      if (_value.isAPredefinedColor()) ColorTypeTab.SelectedIndex = 2;
      else ColorTypeTab.SelectedIndex = _value.isRGB ? 0 : 1;
    }

  
    public int tabReOrder(int index)
    {
      TabIndex = index++;
      ColorTypeTab.TabIndex = index++;
      C1RGB_textBox.TabIndex = index++;
      C2RGB_textBox.TabIndex = index++;
      C3RGB_textBox.TabIndex = index++;
      C4RGB_textBox.TabIndex = index++;
      C1HSL_textBox.TabIndex = index++;
      C2HSL_textBox.TabIndex = index++;
      C3HSL_textBox.TabIndex = index++;
      C4HSL_textBox.TabIndex = index++;

      return index;
    }

  

    private int findHistoryIndex(int x, int y)
    {  
      int spacingX = historyTileSizeX + historyTileSpacing;
      int spacingY = historyTileSizeY + historyTileSpacing;

      if ((x % spacingX > historyTileSizeX) || (y % spacingY > historyTileSizeY)) return -1;
    
      int n = (int)(y / spacingY) * historyTilesPerRow + x / spacingX;
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
      if (OnColorChanged != null) OnColorChanged(this, new ColorChangeEventArgs(_value));

    }

    void HistoryMouseDoubleClick(object sender, MouseEventArgs e)
    {
      HistoryMouseUp(sender, e);
    

    }

    private int findPredefinedIndex(int x, int y)
    { if (x < predefinedColorOffsetX) return -1;
      if (y < predefinedColorOffsetY) return -1;

      x -= predefinedColorOffsetX;
      y -= predefinedColorOffsetY;
      int spacingX = predefinedTileSizeX + predefinedTileSpacing;
      int spacingY = predefinedTileSizeY + predefinedTileSpacing;

      if ((x % spacingX > predefinedTileSizeX) || (y % spacingY > predefinedTileSizeY)) return -1;
     
      int n = (int)(y / spacingY) * predefinedTilesPerRow + x / spacingX;
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
      if (OnColorChanged != null) OnColorChanged(this,new ColorChangeEventArgs(_value));



    }

    void PredefinedMouseDoubleClick(object sender, MouseEventArgs e)
    {
      PredefinedMouseUp(sender, e);
     

    }
    public void refresh()
    {
      updateCursors();
    }



    void updateCursors()
    { if (C1RGB == null) return;  // Mono protection
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
      if (OnColorChanged != null) OnColorChanged(this, new ColorChangeEventArgs(_value));


    }


    private void DrawPredefinedColors()
    {

    
      int w = PredefinedPanel.Size.Width;
      int h = PredefinedPanel.Size.Height;
      Bitmap DrawArea = new Bitmap(w, h);
      Image previous = PredefinedPanel.Image;
      PredefinedPanel.Image = DrawArea;
      if (previous != null) previous.Dispose();
      Graphics g = Graphics.FromImage(DrawArea);

      int x = predefinedColorOffsetX;
      int y = predefinedColorOffsetY;

      for (int i =0;i< predefinedColors.Count;i++)
      
      {
        Color colorPropertyValue = predefinedColors[i];
        Brush Brush = new SolidBrush(colorPropertyValue);

       // YColor.FromArgb(255, colorPropertyValue.R, colorPropertyValue.G, colorPropertyValue.B);
        g.FillRectangle(new SolidBrush(colorPropertyValue), x, y, predefinedTileSizeX, predefinedTileSizeY);
        Pen p = Pens.Black;
        if ((_value.isRGB) && (_value.A == colorPropertyValue.A) && (_value.R == colorPropertyValue.R) && (_value.G == colorPropertyValue.G) && (_value.B == colorPropertyValue.B)) p = new Pen(Color.Black, 3);


        g.DrawRectangle(p, x, y, predefinedTileSizeX - 1, predefinedTileSizeY - 1);
        x += predefinedTileSizeX + predefinedTileSpacing;
        if (x > w - predefinedTileSizeX) { x = predefinedColorOffsetX; y += predefinedTileSizeY + predefinedTileSpacing;  }
      }

      g.Dispose();


    }


    private void DrawHistory()
    {

     
      int w = pictureBoxHistory.Size.Width;
      int h = pictureBoxHistory.Size.Height;
      Bitmap DrawArea = new Bitmap(w, h);
      Image previous = pictureBoxHistory.Image;
      pictureBoxHistory.Image = DrawArea;
      if (previous!=null) previous.Dispose();
      Graphics g = Graphics.FromImage(DrawArea);
      

      int n= 0;
      while ((n< maxHistoryLength) && n< (_colorHistory.Count))
      {
        int x = (n % historyTilesPerRow) * (historyTileSizeX + historyTileSpacing);
        int y = (n / historyTilesPerRow) * (historyTileSizeY + historyTileSpacing);
        g.FillRectangle(Brushes.White, x, y, historyTileSizeX , historyTileSizeY);
        g.FillRectangle(Brushes.Black, x, y, historyTileSizeX >> 1, historyTileSizeY >> 1);
        g.FillRectangle(Brushes.Black, x + (historyTileSizeX >> 1),y + (historyTileSizeY >> 1), historyTileSizeX >> 1, historyTileSizeY >> 1);
        g.FillRectangle(new SolidBrush(_colorHistory[n].toColor()), x,y,historyTileSizeX,historyTileSizeY);
        g.DrawRectangle(Pens.Black, x,y,historyTileSizeX-1,historyTileSizeY-1);
        n++;
      }

      g.Dispose();


    }

    private void PreviewColor()
    {
      int w = pictureBoxPreview.Size.Width;
      int h = pictureBoxPreview.Size.Height;


      Bitmap DrawArea = new Bitmap(w, h);
      Image previous = pictureBoxPreview.Image;
      pictureBoxPreview.Image = DrawArea;
      if (previous != null)  previous.Dispose();
      Graphics g = Graphics.FromImage(DrawArea);
      g.FillRectangle(Brushes.White, 0, 0, w, h);
      g.FillRectangle(Brushes.Black, 0, 0, w >>1, h>>1);
      g.FillRectangle(Brushes.Black, w >> 1, h >> 1, w >> 1, h >> 1);
      g.FillRectangle(new SolidBrush(_value.toColor()), 0, 0, w-1, h-1);
      g.DrawRectangle(Pens.Black, 0, 0, w-1, h-1);

    
    }

    private bool _ok = false;
    public bool ok { get { return _ok; } }


 

    private YColor _value;

    public YColor value
    {
      get { return _value; }
      set
      {
        if (_value != value)
        {
          Init(value);



        }
      }
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


  

 

    private void ColorEditor_Load(object sender, EventArgs e)
    {

    }

  
    private void computeSpacing()
    { 
      int w = ColorTypeTab.ClientSize.Width;
      PredefinedPanel.Width = w -8;
      int rowCount = 1 + (int)((predefinedColors.Count - 1) / predefinedTilesPerRow);
      predefinedTileSizeX = (int)(PredefinedPanel.Width / predefinedTilesPerRow) - predefinedTileSpacing;
      predefinedTileSizeY = (int)(PredefinedPanel.Height / rowCount) - predefinedTileSpacing;
      


      rowCount = 1 + (int)((maxHistoryLength - 1) / historyTilesPerRow);  
      historyTileSizeX = (int)((pictureBoxHistory.Width - predefinedTileSpacing -2) / historyTilesPerRow) - predefinedTileSpacing;
      historyTileSizeY = (int)((pictureBoxHistory.Height - 1 )/ rowCount) - historyTileSpacing;



      pictureBoxPreview.Height = rowCount * historyTileSizeY + (rowCount - 1) * historyTileSpacing;

    }


    private void ColorEditor_Resize(object sender, EventArgs e)
    {
      

      computeSpacing();



      updateCursors();
      DrawHistory();
      PreviewColor();
      DrawPredefinedColors();

    }

   
  }

  public delegate void ColorChanged(object source, ColorChangeEventArgs e);

  public class ColorChangeEventArgs : EventArgs
  {
    private YColor _color;
    public ColorChangeEventArgs(YColor color)
    {
      _color = color;
    }
    public YColor color { get { return _color; } }

  }

  delegate void ColorCursorChangeCallBack(ColorCursor source, byte value);

  class ColorCursor

  {
    Bitmap HueBackground;

    public enum ColorFunction { COLOR_R, COLOR_G, COLOR_B, COLOR_H, COLOR_S, COLOR_L, COLOR_A };

    Byte _value;
    public Byte value
    {
      get { return _value; }
      set { _value = value; redraw(); }
    }


    PictureBox _bitmap;
    TextBox _textbox;
    ColorCursorChangeCallBack _callback;
    YColor _color;
    ColorFunction _colorFunction;
    //  ToolTip _tt; looks like tool-tips messes with mouse events.


    public ColorCursor(ColorFunction colorFunction, PictureBox bitmap, TextBox textbox, ColorCursorChangeCallBack callback, YColor initialColor)
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
        p.Dispose();
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
    {
      _textbox.TextChanged -= textBox_TextChanged;
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
    {
      byte v;
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

      Rectangle contentsRectangle = new Rectangle(1, 1, _bitmap.Width - 2, _bitmap.Height - 2);
      Rectangle outsideRectangle = new Rectangle(0, 0, _bitmap.Width - 1, _bitmap.Height - 1);

      switch (_colorFunction)
      {
        case ColorFunction.COLOR_H: g.DrawImage(HueBackground, contentsRectangle); break;
        case ColorFunction.COLOR_S:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromAhsl(255, _color.H, b, _color.L).toColor());
            g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
            p.Dispose();
          }
          break;
        case ColorFunction.COLOR_L:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromAhsl(255, _color.H, _color.S, b).toColor());
            g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
            p.Dispose();
          }
          break;


        case ColorFunction.COLOR_R:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromArgb(255, b, _color.G, _color.B).toColor());
            g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
            p.Dispose();
          }
          break;

        case ColorFunction.COLOR_G:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromArgb(255, _color.R, b, _color.B).toColor());
            g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
            p.Dispose();
          }
          break;

        case ColorFunction.COLOR_B:
          for (int i = 0; i < _bitmap.Width - 2; i++)
          {
            byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
            Pen p = new Pen(YColor.FromArgb(255, _color.R, _color.G, b).toColor());
            g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
            p.Dispose();
          }
          break;


        case ColorFunction.COLOR_A:
          g.FillRectangle(Brushes.White, 0, 0, _bitmap.Width, _bitmap.Height);
          for (int i = 0; i < 16; i++)
            g.FillRectangle(Brushes.Black, i * _bitmap.Width / 16, i % 2 == 0 ? 0 : (_bitmap.Height - 2) / 2, 1 + (_bitmap.Width - 2) / 16, 1 + (_bitmap.Height - 2) / 2);

          if (_color.isHSL)
            for (int i = 0; i < _bitmap.Width - 2; i++)
            {
              byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
              Pen p = new Pen(YColor.FromAhsl(b, _color.H, _color.S, _color.L).toColor());
              g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
              p.Dispose();
            }
          else
            for (int i = 0; i < _bitmap.Width - 2; i++)
            {
              byte b = (byte)Math.Round(255 * (double)i / (_bitmap.Width - 2));
              Pen p = new Pen(YColor.FromArgb(b, _color.R, _color.G, _color.B).toColor());
              g.DrawLine(p, i + 1, 0, i + 1, _bitmap.Height);
              p.Dispose();
            }


          break;





      }

      g.DrawRectangle(Pens.Black, outsideRectangle);


      int x = (int)Math.Round((_bitmap.Width - 2) * (double)this._value / 255.0);

      Point[] TopPolygon = new Point[3];
      TopPolygon[0] = new Point { X = 1 + x - 5, Y = 0 };
      TopPolygon[1] = new Point { X = 1 + x, Y = 5 };
      TopPolygon[2] = new Point { X = 1 + x + 5, Y = 0 };

      Point[] BottomPolygon = new Point[3];
      BottomPolygon[0] = new Point { X = +1 + x - 5, Y = _bitmap.Height };
      BottomPolygon[1] = new Point { X = +1 + x, Y = _bitmap.Height - 5 };
      BottomPolygon[2] = new Point { X = 1 + x + 5, Y = _bitmap.Height };

      g.FillPolygon(Brushes.Black, TopPolygon);
      g.DrawPolygon(Pens.White, TopPolygon);

      g.FillPolygon(Brushes.Black, BottomPolygon);
      g.DrawPolygon(Pens.White, BottomPolygon);

      g.Dispose();

    }

  }



}
