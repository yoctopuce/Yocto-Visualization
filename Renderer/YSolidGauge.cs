/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *   Yocto 2D data renderer solif gauge classes
 *   
 * 
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace YDataRendering
{
  

  public class YSolidGauge : YDataRenderer
  {
    public enum DisplayMode {
      [Description("90°")]
      DISPLAY90,
      [Description("180°")]
      DISPLAY180,
      [Description("270°")]
      DISPLAY270,
      [Description("360°")]
      DISPLAY360, };
    double _shownValue = 0;


    protected Double _min =0;
    public Double min { get { return _min; } set { _min = value; if (_shownValue < _min) _shownValue = _min; redraw(); } }

    protected Double _max = 100;
    public Double max { get { return _max; } set { _max = value; if (_shownValue > _max) _shownValue = _max;  redraw(); } }

    const int SegmentMaxLength = 8;  

    private Pen _borderpen = null;
    public Pen borderpen
    {
      get
      {
        if (_borderpen == null)
        {
          _borderpen = new Pen(_borderColor, (float)_borderThickness);
          //   _borderpen.StartCap = LineCap.Round;
          //   _borderpen.EndCap = LineCap.Round;

          _borderpen.StartCap = LineCap.Square;

          _borderpen.EndCap = LineCap.Square;

        }
          return _borderpen;
      }
    }


    private Color _borderColor = Color.Black;
    public Color borderColor { get { return _borderColor; } set { _borderColor = value; _borderpen = null; redraw(); } }

  

    private Brush _bgBrush = null;
    private Color _backgroundColor1 = Color.FromArgb(255, 240, 240, 240);
    public Color backgroundColor1 { get { return _backgroundColor1; } set { _backgroundColor1 = value; _bgBrush = null; redraw(); } }
    private Color _backgroundColor2 = Color.FromArgb(255, 200, 200, 200);
    public Color backgroundColor2 { get { return _backgroundColor2; } set { _backgroundColor2 = value; _bgBrush = null; redraw(); } }


    private double _borderThickness = 5;
    public double borderThickness { get { return _borderThickness; } set { _borderThickness = value; _borderpen = null; _path = null; redraw(); } }

    private ValueFormater _valueFormater = null;
    public ValueFormater valueFormater
    {
      set { _valueFormater = value;redraw(); }
      get { return _valueFormater; }
    }

    private ValueFormater _minmaxFormater = null;
    public ValueFormater minmaxFormater
    {
      set { _minmaxFormater = value; redraw(); }
      get { return _minmaxFormater; }
    }


    private double _thickness = 25;
    public double thickness { get { return _thickness; }
      set { _thickness = Math.Max(Math.Min(value,80),1); _path = null; redraw(); } }


    private double _maxSpeed = 0.1;
    public double maxSpeed
    {
      get { return _maxSpeed; }
      set { if (value>0) _maxSpeed = value ;  }
    }

    private double _value = 0;
    public double value { get { return _value; } set { _value = value; redraw(); } }

    private Color _color1 = Color.Green;
    public Color color1 { get { return _color1; } set { _color1 = value; redraw(); } }

    private Color _color2 = Color.Red;
    public Color color2 { get { return _color2; } set { _color2 = value; redraw(); } }



    YFontParams _font = null;
    public YFontParams font { get { return _font; } }

   
    YFontParams _minMaxFont = null;
    public YFontParams minMaxFont { get { return _minMaxFont; } }


    private bool _showMinMax = true;
    public bool showMinMax { get { return _showMinMax; } set { _showMinMax = value; redraw(); } }


    PointF[] _path = null;

    DrawPrameters lastDrawParameters;

    private DisplayMode _displayMode = DisplayMode.DISPLAY90;
    public DisplayMode displayMode { get { return _displayMode; } set { _displayMode = value; _path = null; _bgBrush = null; redraw(); } }

    private struct DrawPrameters
    {
      public double outerRadius;
      public double innerRadius;
      public double angleStart;
      public double angleEnd;
      public double ycenter;
      public double xcenter;
      public double heightTop;
      public double heightBottom;



      public Rectangle valueRectangle;
      public StringFormat valueFormat;
      public Rectangle minValueRectangle;
      public StringFormat minValueFormat;
      public Rectangle maxValueRectangle;
      public StringFormat maxValueFormat;
      public string minValue;
      public string maxValue;
      public string value;
    }

    public void FontsizeChange(YFontParams source)
    {
      _path = null;
    }
    public YSolidGauge(PictureBox ChartContainer, DisplayMode mode, logFct logFunction) : base(ChartContainer, logFunction)
    {
      this._minMaxFont = new YFontParams(this, Math.Min(ChartContainer.Width, ChartContainer.Height)/15, FontsizeChange);
      _displayMode = mode;
      this._font = new YFontParams(this, Math.Min(ChartContainer.Width, ChartContainer.Height) / 5,null);
      resizeRule = Proportional.ResizeRule.RELATIVETOBOTH;

      Graphics g = ChartContainer.CreateGraphics();
      DrawPrameters p = ComputeDrawParameters(g, UIContainer.Size.Width, UIContainer.Size.Height);
      g.Dispose();
      
      AllowRedraw();
      Draw();

    }

    protected override void clearCachedObjects()
    {
      _bgBrush = null;
       _path = null;
     

    }

    


    private DrawPrameters ComputeDrawParameters(Graphics g, int UIw, int UIh)
    {
     
      double w = UIw - 5 - _borderThickness;
      double h = UIh - 5 - _borderThickness;
      double xcenter = UIw / 2;
      double outerRadius = 0;
      double angleStart =0 ;
      double angleEnd = 0;
      double ycenter = 0;
      Rectangle ValueRectangle = new Rectangle(0, 0, 0, 0);
      ;
      StringFormat valueFormat = new StringFormat();
      double innerRadius = 0;
      double minMaxHeight = 0;
      SizeF s1 = new SizeF();
      SizeF s2 = new SizeF();

      lastDrawParameters.value = _valueFormater == null ? _value.ToString("0") : _valueFormater(this, _value);

      if (_showMinMax)
      {
        lastDrawParameters.minValue = _minmaxFormater == null ? _min.ToString("0") : _minmaxFormater(this, _min);
        lastDrawParameters.maxValue = _minmaxFormater == null ? _max.ToString("0") : _minmaxFormater(this, _max);
        s1 = g.MeasureString(lastDrawParameters.minValue, _minMaxFont.fontObject, 100000);
        s2 = g.MeasureString(lastDrawParameters.maxValue, _minMaxFont.fontObject, 100000);

        lastDrawParameters.minValueFormat = new StringFormat();
        lastDrawParameters.maxValueFormat = new StringFormat();

        minMaxHeight = s1.Height;
        if (s2.Height> minMaxHeight) minMaxHeight = s2.Height;
       

      }



      switch (_displayMode)
      {
        case DisplayMode.DISPLAY90:
       
            h = h - minMaxHeight;
            w = w - minMaxHeight;
          

          outerRadius = w;
          if (outerRadius > h - _borderThickness) outerRadius = h - _borderThickness;
          if (outerRadius > w - borderThickness) outerRadius = w - borderThickness;
          angleStart = Math.PI / 2;
          angleEnd = Math.PI;
          lastDrawParameters.heightTop = outerRadius;
          lastDrawParameters.heightBottom =0;
          ycenter = h;
          xcenter = UIw / 2 + outerRadius / 2 - minMaxHeight;

          innerRadius = outerRadius * (100 - _thickness) / 100;
          ValueRectangle = new Rectangle((int)(xcenter - innerRadius), (int)(ycenter - innerRadius), (int)innerRadius, (int)innerRadius);
          valueFormat.Alignment = StringAlignment.Far;
          valueFormat.LineAlignment = StringAlignment.Far;
          if (_showMinMax)
          {

            lastDrawParameters.minValueRectangle = new Rectangle((int)(xcenter - ((outerRadius + innerRadius + s1.Width) / 2)), (int)(ycenter + _borderThickness), (int)(s1.Width + 1), (int)(minMaxHeight + 1));
            lastDrawParameters.minValueFormat.Alignment = StringAlignment.Near;
            lastDrawParameters.minValueFormat.LineAlignment = StringAlignment.Near;
            lastDrawParameters.maxValueRectangle = new Rectangle((int)(xcenter  + _borderThickness),(int)(ycenter - outerRadius + (outerRadius -innerRadius - s2.Width) /2),  (int)(minMaxHeight + 1),(int)(s2.Width + 1));
            lastDrawParameters.maxValueFormat.Alignment = StringAlignment.Near;
            lastDrawParameters.maxValueFormat.LineAlignment = StringAlignment.Near;
            lastDrawParameters.maxValueFormat.FormatFlags = StringFormatFlags.DirectionVertical;
          }
          break;
        case DisplayMode.DISPLAY180:

         
          h = h - minMaxHeight;
          SizeF s0 = new SizeF();
       



          s0 = g.MeasureString(lastDrawParameters.value, _font.fontObject, 100000);
          outerRadius = (w / 2) - borderThickness;
          if (outerRadius > h -_borderThickness) outerRadius = h - _borderThickness;
          if (outerRadius > w- borderThickness) outerRadius = w  - borderThickness;
          angleStart = 0;
          angleEnd = Math.PI;
          ycenter = outerRadius + _borderThickness/2;
          innerRadius = outerRadius * (100 - _thickness) / 100;
          lastDrawParameters.heightTop = outerRadius;
          lastDrawParameters.heightBottom = 0;
          ValueRectangle = new Rectangle((int)(xcenter - innerRadius), (int)(ycenter + _borderThickness + minMaxHeight - s0.Height), (int)(2 * innerRadius), (int)(s0.Height +1));
          valueFormat.Alignment = StringAlignment.Center;
          valueFormat.LineAlignment = StringAlignment.Far;
          if (_showMinMax)
          {
            
            lastDrawParameters.minValueRectangle = new Rectangle((int)(xcenter - ((outerRadius+ innerRadius + s1.Width) / 2) ), (int)(ycenter + _borderThickness), (int)(s1.Width + 1), (int)(minMaxHeight + 1));
            lastDrawParameters.minValueFormat.Alignment = StringAlignment.Near;
            lastDrawParameters.minValueFormat.LineAlignment = StringAlignment.Near;
            lastDrawParameters.maxValueRectangle = new Rectangle((int)(xcenter + ((outerRadius + innerRadius - s2.Width) / 2)), (int)(ycenter + _borderThickness), (int)(s2.Width + 1), (int)(minMaxHeight + 1));
            lastDrawParameters.maxValueFormat.Alignment = StringAlignment.Near;
            lastDrawParameters.maxValueFormat.LineAlignment = StringAlignment.Near;
          }
          break;
        case DisplayMode.DISPLAY270:

          outerRadius = w;
          if (outerRadius > h / 2) outerRadius = h / 2;
          if (outerRadius > w / 2) outerRadius = w / 2;
          lastDrawParameters.heightTop = outerRadius;
          lastDrawParameters.heightBottom = outerRadius ;
          angleStart = 0;
          angleEnd = 3 * Math.PI / 2;
          ycenter = UIh/ 2;
          innerRadius = outerRadius * (100 - _thickness) / 100;


          ValueRectangle = new Rectangle((int)(xcenter - innerRadius), (int)(ycenter - innerRadius), (int)(2 * innerRadius), (int)(2 * innerRadius));
          valueFormat.Alignment = StringAlignment.Center;
          valueFormat.LineAlignment = StringAlignment.Center;

          if (_showMinMax)
          {

            lastDrawParameters.minValueRectangle = new Rectangle((int)(xcenter + _borderThickness), (int)(ycenter + (innerRadius+innerRadius) /2), (int)(s1.Width + 1), (int)(s1.Height + 1));
            lastDrawParameters.minValueFormat.Alignment = StringAlignment.Near;
            lastDrawParameters.minValueFormat.LineAlignment = StringAlignment.Near;
            lastDrawParameters.maxValueRectangle = new Rectangle((int)(xcenter +(innerRadius + innerRadius) / 2) , (int)(ycenter + _borderThickness), (int)(s2.Height + 1), (int)(s2.Width + 1));
            lastDrawParameters.maxValueFormat.Alignment = StringAlignment.Near;
            lastDrawParameters.maxValueFormat.LineAlignment = StringAlignment.Near;
            lastDrawParameters.maxValueFormat.FormatFlags = StringFormatFlags.DirectionVertical;
          }



          break;
        case DisplayMode.DISPLAY360:

          outerRadius = w;
          if (outerRadius > (h / .85) / 2) outerRadius = (h / .85) / 2;
          if (outerRadius > w / 2) outerRadius = w / 2;
          lastDrawParameters.heightTop = outerRadius;
          lastDrawParameters.heightBottom = outerRadius * 0.7;
          ycenter = outerRadius + _borderThickness / 2;
          angleStart = -Math.PI / 4;
          angleEnd = 5 * Math.PI / 4;
          innerRadius = outerRadius * (100 - _thickness) / 100;
          ValueRectangle = new Rectangle((int)(xcenter - innerRadius), (int)(ycenter - innerRadius), (int)(2 * innerRadius), (int)(2 * innerRadius));
          valueFormat.Alignment = StringAlignment.Center;
          valueFormat.LineAlignment = StringAlignment.Center;

          if (_showMinMax)
          {
            double dx = Math.Abs(innerRadius * Math.Cos(angleStart));
            double dy = innerRadius * Math.Abs(Math.Sin(angleStart))  + 2*Math.Abs((outerRadius - innerRadius) * Math.Sin(angleStart) /3);


            lastDrawParameters.minValueRectangle = new Rectangle((int)(xcenter - dx), (int)(ycenter + dy - minMaxHeight/2), (int)(s1.Width + 1), (int)(minMaxHeight + 1));
            lastDrawParameters.minValueFormat.Alignment = StringAlignment.Near;
            lastDrawParameters.minValueFormat.LineAlignment = StringAlignment.Center;
            lastDrawParameters.maxValueRectangle = new Rectangle((int)(xcenter + dx - s2.Width), (int)(ycenter + dy - minMaxHeight / 2), (int)(s2.Width + 1), (int)(minMaxHeight + 1));
            lastDrawParameters.maxValueFormat.Alignment = StringAlignment.Near;
            lastDrawParameters.maxValueFormat.LineAlignment = StringAlignment.Center;
          }




          break;

      }

      lastDrawParameters.outerRadius = outerRadius;
      lastDrawParameters.innerRadius= innerRadius;
      lastDrawParameters.angleStart = angleStart;
      lastDrawParameters.angleEnd= angleEnd;
      lastDrawParameters.ycenter= ycenter;
      lastDrawParameters.xcenter= xcenter;
      lastDrawParameters.valueRectangle= ValueRectangle;
      lastDrawParameters.valueFormat= valueFormat;

      return lastDrawParameters;

    }

    protected override int Render(Graphics g, int w, int h)
    {
      
     
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;


      DrawPrameters p = ComputeDrawParameters(g,w,h);


      if (_path == null)
      {
        double outterlength = (2 * p.outerRadius * Math.PI) * (p.angleEnd - p.angleStart) / (2 * Math.PI);
        int stepCount = (int)(outterlength / SegmentMaxLength);
        double stepsize = (p.angleEnd - p.angleStart) / stepCount;
        _path = new PointF[2 * (stepCount + 1)];
        int n = 0;
        for (int i = 0; i <= stepCount; i++)
        { double a = p.angleStart + i * stepsize;
          _path[n++] = new PointF((Single)(p.xcenter + p.outerRadius * Math.Cos(a)),(Single)(p.ycenter - p.outerRadius * Math.Sin(a)));
        }
        for (int i = stepCount; i >= 0; i--)
        {
          double a = p.angleStart + i * stepsize;
          _path[n++] = new PointF((Single)(p.xcenter + p.innerRadius * Math.Cos(a)), (Single)(p.ycenter - p.innerRadius * Math.Sin(a)));
        }
      }

      if (_bgBrush == null)
           _bgBrush = new LinearGradientBrush(new Point(0, (int)(p.ycenter - p.heightTop)),
                                         new Point(0, (int)(p.ycenter + p.heightBottom)), _backgroundColor1, _backgroundColor2);



        
      if (_borderpen == null)
      {
        _borderpen = new Pen(_borderColor, (float)_borderThickness);
        _borderpen.LineJoin = LineJoin.Round;
      }

      g.FillPolygon(_bgBrush, _path);


      if (_shownValue != _value)
      {
        double step =  _maxSpeed * (_max - _min) / 100;
        if (Math.Abs(_value - _shownValue) < step) _shownValue = _value;
        else if (_shownValue < _value) _shownValue += step;
        else _shownValue -= step;


      }



      double v = _shownValue;
      if (v>=_min)
      {  if (v>_max) v=_max;
        double valueFactor = (v - _min) / (_max - min);
         double angleValue = p.angleStart + (p.angleEnd - p.angleStart) * valueFactor; 
         double outterlength = (2 * p.outerRadius * Math.PI) * (angleValue - p.angleStart) / (2 * Math.PI);
         int stepCount = (int)(outterlength / SegmentMaxLength);
         double stepsize = (angleValue - p.angleStart) / stepCount;
         PointF[] pt = new PointF[2 * (stepCount + 1)];
         int n = 0;
          for (int i = 0; i <= stepCount; i++)
          {
            double a = p.angleEnd - i * stepsize;
            pt[n++] = new PointF((Single)(p.xcenter + p.outerRadius * Math.Cos(a)), (Single)(p.ycenter - p.outerRadius * Math.Sin(a)));
          }
         for (int i = stepCount; i >= 0; i--)
          {
            double a = p.angleEnd - i * stepsize;
            pt[n++] = new PointF((Single)(p.xcenter + p.innerRadius * Math.Cos(a)), (Single)(p.ycenter - p.innerRadius * Math.Sin(a)));
          }
        Brush b;
        if (_color1 == _color2)
          b = new SolidBrush(_color1);
        else
        {
          int    A1 = (_color1.ToArgb() >> 24) & 0xFF;
          double H1 = _color1.GetHue();        
          double S1 = _color1.GetSaturation();
          double L1 = _color1.GetBrightness();
          int    A2 = (_color2.ToArgb() >> 24) & 0xFF;
          double H2 = _color2.GetHue();
          double S2 = _color2.GetSaturation();
          double L2 = _color2.GetBrightness();
          int A = ((int)Math.Round(A1 + (double)(A2 - A1) * valueFactor)) & 0xff;

          double H;
          if (Math.Abs(H2-H1)<=180) H = H1 + (double)(H2 - H1) * valueFactor;
                       else
          {
            H=  H1 +360 + (double)(H2 - H1+360) * valueFactor;
            if (H > 360) H -= 360;
          }
          double S = S1 + (double)(S2 - S1) * valueFactor;
          double L = L1 + (double)(L2 - L1) * valueFactor;        
          b = new SolidBrush(Color.FromArgb(A1, Ycolor.hsl2rgb((int) ((255*H)/360), (int)(255*S),(int)(255*L))  ));    
        }
        g.FillPolygon(b, pt);
      }

      if (_borderThickness>0) g.DrawPolygon(_borderpen, _path);
     
      g.DrawString(lastDrawParameters.value, _font.fontObject, _font.brushObject, p.valueRectangle, p.valueFormat);

      if (_showMinMax)
      {
        //Pen pn = new Pen(Color.Red);
        //g.DrawRectangle(pn,lastDrawParameters.minValueRectangle);
        //g.DrawRectangle(pn, lastDrawParameters.maxValueRectangle);
        
        g.DrawString(lastDrawParameters.minValue, _minMaxFont.fontObject, _minMaxFont.brushObject, lastDrawParameters.minValueRectangle, lastDrawParameters.minValueFormat);
        g.DrawString(lastDrawParameters.maxValue, _minMaxFont.fontObject, _minMaxFont.brushObject, lastDrawParameters.maxValueRectangle, lastDrawParameters.maxValueFormat);

      }
      drawMessagePanels(g, w, h);
    
     
     
      return 0;

    }
    protected override void renderingPostProcessing()
    {
      if (_shownValue != _value) redraw();
    }
  }
}