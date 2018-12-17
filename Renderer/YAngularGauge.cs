/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *   Yocto 2D data renderer, angular gauge class
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

using System.Windows.Forms;



namespace YDataRendering
{
  

  public class YAngularZone : Zone
  {

    PointF[] _path = null;


    public void setPathSize(int count) { _path = new PointF[count]; }
    public void setPathPoint(int index, PointF p) { _path[index] = p; }
    public void resetPath() { _path = null; }

    public PointF[] path { get{ return _path; } }

    protected override void resetCache() { resetPath(); }

    double _width = 10;
    public double width { get { return _width; }
      set
      { if (value <= 0)  throw new ArgumentException("Width must be a positive value");
        _width = value;  _path = null; _parentRenderer.redraw();
      } }


    double _outerRadius = 98;
    public double outerRadius { get { return _outerRadius; } set { _outerRadius = Math.Max(0,Math.Min(100,value)); _path = null; _parentRenderer.redraw(); } }


    public YAngularZone(YDataRenderer parent, Object directParent) : base(parent,directParent)
    {}

  }


  public class YAngularGauge : YDataRenderer
  {

   

    protected Double _min = 0;
    public Double min { get { return _min; }
      set {

        
          if ((value >= _max) && (!minMaxCheckDisabled))
           throw new ArgumentException("Min cannot be greater than max (" + _max.ToString() + ")");


        _min = value;
        for (int i = 0; i < _zones.Count; i++) zones[i].resetPath();
          if (_needleValue < _min) { _needleValue = _min; }
        redraw(); }
    }

    protected Double _max = 100;
    public Double max
    {
      get { return _max; }
      set
      {
        if ((value <= _min) && (!minMaxCheckDisabled))
           throw new ArgumentException("Max cannot be less than min (" + _min.ToString() + ")");
        _max = value;
        for (int i = 0; i < _zones.Count; i++) zones[i].resetPath();
        if (_needleValue > _max) { _needleValue = _max; }
        redraw();
      }
    }


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
    public double borderThickness { get { return _borderThickness; } set { _borderThickness = value; _borderpen = null; redraw(); } }

    private ValueFormater _valueFormater = null;
    public ValueFormater valueFormater
    {
      set { _valueFormater = value; redraw(); }
      get { return _valueFormater; }
    }

    private ValueFormater _minmaxFormater = null;
    public ValueFormater minmaxFormater
    {
      set { _minmaxFormater = value; redraw(); }
      get { return _minmaxFormater; }
    }


    private double _thickness = 20;
    public double thickness { get { return _thickness; }
      set {
        if (value <= 0) throw new ArgumentException("Thickness must be a positive value");
        _thickness = Math.Max(Math.Min(value, 80), 1); redraw();
      } }

    private double _value = 0;
    private double _needleValue = 0;
    public double value { get { return _value; } set { _value = value; redraw(); } }

    private Color _color1 = Color.Green;
    public Color color1 { get { return _color1; } set { _color1 = value; redraw(); } }

    private Color _color2 = Color.Red;
    public Color color2 { get { return _color2; } set { _color2 = value; redraw(); } }

    Pen _graduationPen = null;

    private Color _graduationColor = Color.Black;
    public Color graduationColor { get { return _graduationColor; } set { _graduationColor = value; _graduationPen = null; redraw(); } }

    private double _graduationThickness = 2;
    public double graduationThickness
    {
      get { return _graduationThickness; }
      set { if (value <= 0) throw new ArgumentException("Thickness must be a positive value");
           _graduationThickness = value; _graduationPen = null; redraw(); }
    }

    private double _graduationSize = 10;
    public double graduationSize
    {
      get { return _graduationSize; }
      set { if (value <= 0) throw new ArgumentException("Gradation size must be a positive value");
          _graduationSize = value; redraw(); }

    }

    private double _graduation = 10;
    public double graduation
    {
      get { return _graduation; }
      set { _graduation = value; redraw(); }

    }

    private double _unitFactor = 1;
    public double unitFactor
    {
      get { return _unitFactor; }
      set
      {
        if (value == 0) throw new ArgumentException("Factor cannot be zero.");
         _unitFactor = value; redraw();
      }

    }

    private string _unit = "";
    public string unit
    {
      get { return _unit; }
      set { _unit = value; redraw(); }

    }

    YFontParams _unitFont;
    public YFontParams unitFont { get { return _unitFont; } }





    Pen _subgraduationPen = null;

    private Color _subgraduationColor = Color.Black;
    public Color subgraduationColor { get { return _subgraduationColor; }
      set {
        _subgraduationColor = value;
        _subgraduationPen = null; redraw(); } }

    private double _subgraduationThickness = 1;
    public double subgraduationThickness
    {
      get { return _subgraduationThickness; }
      set { if (value <= 0) throw new ArgumentException("Thickness must be a positive value");
           _subgraduationThickness = value; _subgraduationPen = null; redraw(); }
    }

    private double _subgraduationSize = 5;
    public double subgraduationSize
    {
      get { return _subgraduationSize; }
      set { if (value <= 0) throw new ArgumentException("Size must be a positive value");
           _subgraduationSize = value; redraw(); }

    }

    private double _graduationOuterRadiusSize = 98;
    public double graduationOuterRadius
    {
      get { return _graduationOuterRadiusSize; }
      set { _graduationOuterRadiusSize = Math.Max(0, Math.Min(100, value)); redraw(); }

    }


    private double _subgraduationCount = 5;
    public double subgraduationCount
    {
      get { return _subgraduationCount; }
      set { if (value < 0) throw new ArgumentException("Count must be a positive value");
            _subgraduationCount = value; redraw(); }

    }


    private Color _statusColor = Color.Gray;
    public Color statusColor { get { return _statusColor; } set { _statusColor = value; redraw(); } }


    private YFontParams _statusFont;
    public YFontParams statusFont { get { return _unitFont; } }

    private string _statusLine = "";
    public string statusLine { get { return _statusLine; } set { _statusLine = value; redraw(); } }


    private bool _showNeedle = true;
    public bool showNeedle { get { return _showNeedle; } set { _showNeedle = value; redraw(); } }


    private Brush _needleBrush = null;
    private Color _needleColor = Color.Red;
    public Color needleColor { get { return _needleColor; } set { _needleColor = value; _needleBrush = null; redraw(); } }


    private double _needleMaxSpeed = 5;
    public double needleMaxSpeedh
    {

      get { return _needleMaxSpeed; }
      set
      {
        if (value <= 0) throw new ArgumentException("Speed must be a positive value");
        _needleMaxSpeed = value; redraw();
      }
    }


    private double _needleLength1 = 90;
    public double needleLength1 { get { return _needleLength1; } set { _needleLength1 = value; redraw(); } }
    private double _needleLength2 = 15;
    public double needleLength2 { get { return _needleLength2; } set { _needleLength2 = value; redraw(); } }

    private double _needleWidth = 5;
    public double needleWidth { get { return _needleWidth; }
      set { if (value <= 0) throw new ArgumentException("Width must be a positive value");
        _needleWidth = value; redraw();
      } }

    private Pen _needleContourPen = null;
    private Color _needleContourColor = Color.DarkRed;
    public Color needleContourColor { get { return _needleContourColor; } set { _needleContourColor = value; _needleContourPen = null; redraw(); } }

    private double _needleContourThickness = 1;
    public double needleContourThickness { get { return _needleContourThickness; }
      set { if (value <= 0) throw new ArgumentException("Thickness must be a positive value");
            _needleContourThickness = value; _needleContourPen = null; redraw();
      } }

    // public FontParams font;
    YFontParams _graduationFont;
    public YFontParams graduationFont { get { return _graduationFont; } }


    private bool _showMinMax = true;
    public bool showMinMax { get { return _showMinMax; } set { _showMinMax = value; redraw(); } }


    PointF[] _path = null;



    public YAngularGauge(PictureBox ChartContainer, logFct logFunction) : base(ChartContainer, logFunction)
    {
      this._graduationFont = new YFontParams(this, this, Math.Min(ChartContainer.Width, ChartContainer.Height) / 15, null);
      this._unitFont = new YFontParams(this, this, Math.Min(ChartContainer.Width, ChartContainer.Height) / 20, null);
      this._statusFont = new YFontParams(this, this, Math.Min(ChartContainer.Width, ChartContainer.Height) / 15, null);
      this.unitFont.color = Color.DarkGray;
      this.statusFont.color = Color.DarkGray;
      _zones = new List<YAngularZone>();
    
      AllowRedraw();
      Draw();

    }

    protected override void clearCachedObjects()
    { 
      if (_zones!=null) for (int i = 0; i < _zones.Count; i++) _zones[i].resetPath();
      _path = null;
      _bgBrush = null;

    }




    private List<YAngularZone> _zones;
    public List<YAngularZone> zones { get { return _zones; } }

    public YAngularZone AddZone()
    {

      YAngularZone z = new YAngularZone(this,this);
      _zones.Add(z);
      return z;
    }


 

    protected override int Render(YGraphics g, int w, int h)
    {

      g.SmoothingMode = SmoothingMode.HighQuality;
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      StringFormat stringFormat4Sizing = new StringFormat(StringFormatFlags.NoClip);
      StringFormat stringFormat = new StringFormat(StringFormatFlags.NoClip);
      stringFormat.Alignment = StringAlignment.Center;
      stringFormat.LineAlignment = StringAlignment.Center;

      double xcenter = w / 2;
      double ycenter = h / 2;

      double radius = (Math.Min(w, h) / 2) - borderThickness;
      int circonference = (int)(2 * radius * 3.14);
      double AngleAperture = 4 * 2 * Math.PI / 5;



      if (_path == null)
      {
        double outterlength = (2 * radius * Math.PI);
        int stepCount = (int)(outterlength / SegmentMaxLength);
        double stepsize = (2 * Math.PI) / stepCount;

        _path = new PointF[stepCount];
        int n = 0;
        for (int i = 0; i < stepCount; i++)
        {
          double a = (2 * i * Math.PI) / stepCount;
          _path[n++] = new PointF((Single)(xcenter + radius * Math.Cos(a)), (Single)(ycenter - radius * Math.Sin(a)));
        }
      }

      if (_bgBrush == null) _bgBrush = new LinearGradientBrush(new Point(0, (int)(ycenter - radius)),
                                       new Point(0, (int)(ycenter + radius)), _backgroundColor1, _backgroundColor2);


      if (_borderpen == null)
      {
        _borderpen = new Pen(_borderColor, (float)_borderThickness);
        _borderpen.LineJoin = LineJoin.Round;
      }

      if (_path.Length > 3) g.FillPolygon(_bgBrush, _path);


      if (_graduationPen == null) _graduationPen = new Pen(_graduationColor, (float)_graduationThickness);
      if (_subgraduationPen == null) _subgraduationPen = new Pen(_subgraduationColor, (float)_subgraduationThickness);

      // draw unit
      string unitDesc = ((_unitFactor != 1) ? "x" + _unitFactor.ToString() + " " : "") + _unit;
      SizeF size = g.MeasureString(unitDesc.ToString(), _unitFont.fontObject, 10000, stringFormat4Sizing);
      Rectangle unitPos = new Rectangle((int)(xcenter - size.Width / 2), (int)(ycenter + radius / 2 - size.Height / 2), (int)(size.Width + 1), (int)(size.Height + 1));
      g.DrawString(unitDesc, _unitFont.fontObject, _unitFont.brushObject, unitPos, stringFormat);

      // draw status line
      if (_statusLine != "")
      {
        size = g.MeasureString(_statusLine, _statusFont.fontObject, 10000, stringFormat4Sizing);
        Rectangle statusPos = new Rectangle((int)(xcenter - size.Width / 2), (int)(ycenter - radius / 3 - size.Height / 2), (int)(size.Width + 1), (int)(size.Height + 1));
        g.DrawString(_statusLine, _statusFont.fontObject, _statusFont.brushObject, statusPos, stringFormat);
      }

      double firstGraduation;
      int gratuationCount;
      double Angle, C, S, R1, R2;

      double outerCoef = _graduationOuterRadiusSize / 100;

      // draw zones

      for (int i = 0; i < _zones.Count; i++)
        if (_zones[i].visible)
        {
          if (_zones[i].path == null)
          {
            double zmin = Math.Max(_min, Math.Min(_max, _zones[i].min));
            double zmax = Math.Max(_min, Math.Min(_max, _zones[i].max));

            if (zmax > zmin)
            {

              double zOuterCoef = _zones[i].outerRadius / 100;
              double Angle1 = ((Math.PI - AngleAperture) / 2) + AngleAperture * (zmin - _min) / (_max - _min);
              double Angle2 = ((Math.PI - AngleAperture) / 2) + AngleAperture * (zmax - _min) / (_max - _min);
              double outterlength = (Angle2 - Angle1) * radius;
              int stepCount = (int)(outterlength / SegmentMaxLength);
              if (stepCount < 2) stepCount = 2;
              _zones[i].setPathSize(2 * stepCount + 2);

              PointF[] Path = new PointF[2 * stepCount + 2];
              for (int j = 0; j <= stepCount; j++)
              {
                double A = Angle1 + ((Angle2 - Angle1) * j) / stepCount;
                _zones[i].setPathPoint(j, new PointF((float)(xcenter - radius * zOuterCoef * Math.Cos(A)), (float)(ycenter - radius * zOuterCoef * Math.Sin(A))));

              }
              double innerRadiusCoef = zOuterCoef - (_zones[i].width / 100);

              for (int j = stepCount; j >= 0; j--)
              {
                double A = Angle1 + ((Angle2 - Angle1) * j) / stepCount;
                _zones[i].setPathPoint(2 * stepCount + 1 - j, new PointF((float)(xcenter - radius * innerRadiusCoef * Math.Cos(A)), (float)(ycenter - radius * innerRadiusCoef * Math.Sin(A))));

              }
            }
          }
            if (_zones[i].path!=null)   g.FillPolygon(_zones[i].zoneBrush, _zones[i].path);
          
           

        }

      firstGraduation = _graduation * (int)(_min / _graduation);
      if (_min < 0) firstGraduation -= _graduation;

      while (firstGraduation < _min) firstGraduation += _graduation;
      gratuationCount = (int)((_max - _min) / _graduation) + 1;



      // draw sub graduations

      if ((_subgraduationCount > 0)  && ((_subgraduationCount* gratuationCount)< circonference))
      {
        double subgraduation = _graduation / _subgraduationCount;
        firstGraduation = subgraduation * (int)(_min / subgraduation);
        if (_min < 0) firstGraduation -= subgraduation;
        while (firstGraduation < _min) firstGraduation += subgraduation;


        gratuationCount = (int)((_max - _min) / subgraduation) + 1;


        for (int i = 0; i < gratuationCount; i++)
        {
          double value = firstGraduation + i * subgraduation;
          if (value <= _max)
          {
            Angle = ((Math.PI - AngleAperture) / 2) + AngleAperture * (value - _min) / (_max - _min);
            C = Math.Cos(Angle);
            S = Math.Sin(Angle);
            R1 = (outerCoef * (radius - _borderThickness / 2));
            R2 = (100 - _subgraduationSize) * (outerCoef * (radius - _borderThickness / 2)) / 100;
            g.DrawLine(_subgraduationPen, (float)(xcenter - R1 * C), (float)(ycenter - R1 * S),
                                       (float)(xcenter - R2 * C), (float)(ycenter - R2 * S));
          }
        }
      }



      // draw Main graduations

     

      if (gratuationCount<circonference)  // stop drawing graduation if too many
      for (int i = 0; i < gratuationCount; i++)
      {
        double gvalue = firstGraduation + i * _graduation;
        if (gvalue <= _max)
        {
          Angle = ((Math.PI - AngleAperture) / 2) + AngleAperture * (gvalue - _min) / (_max - _min);
          C = Math.Cos(Angle);
          S = Math.Sin(Angle);
          R1 = (outerCoef * (radius - _borderThickness / 2));
          R2 = (100 - _graduationSize) * (outerCoef * (radius - _borderThickness / 2)) / 100;

          g.DrawLine(_graduationPen, (float)(xcenter - R1 * C), (float)(ycenter - R1 * S),
                                     (float)(xcenter - R2 * C), (float)(ycenter - R2 * S));

          size = g.MeasureString(gvalue.ToString().Trim(), _graduationFont.fontObject, 1000, stringFormat4Sizing);

          double HalfDiagonal = 0.4 * Math.Sqrt(size.Width * size.Width + size.Height * size.Height);
          Rectangle position = new Rectangle((int)(xcenter - (R2 - HalfDiagonal) * C - (size.Width / 2)),
                                             (int)(ycenter - (R2 - HalfDiagonal) * S - (size.Height / 2)),
                                             (int)size.Width + 1, (int)size.Height);

         //  g.DrawRectangle(new Pen(Color.Red, 1), position);


          g.DrawString(gvalue.ToString(), _graduationFont.fontObject, _graduationFont.brushObject, position, stringFormat);
        }
      }

      // draw Border

      if ((_borderThickness > 0) && (_path.Length > 3)) g.DrawPolygon(_borderpen, _path);

      // draw Needle
      if (_showNeedle)
      {
        if (_needleValue != _value)
        {
          double step = _unitFactor * _needleMaxSpeed * (_max - _min) / 100;
          if (Math.Abs(_value - _needleValue) < step) _needleValue = _value;
          else if (_needleValue < value) _needleValue += step;
          else _needleValue -= step;


        }


        double needlevalue = _needleValue / _unitFactor;
        double allowedOverflow = (_max - min) * 0.05;
        if (needlevalue < _min - allowedOverflow) needlevalue = _min - allowedOverflow;
        if (needlevalue > _max + allowedOverflow) needlevalue = _max + allowedOverflow;

        Angle = ((Math.PI - AngleAperture) / 2) + AngleAperture * (needlevalue - _min) / (_max - _min);
        C = Math.Cos(Angle);
        S = Math.Sin(Angle);
        R1 = (radius * _needleLength1) / 100;
        R2 = (radius * _needleLength2) / 100;
        double R3 = (radius * _needleWidth) / 200;

        PointF[] needlepath = new PointF[4];
        needlepath[0] = new PointF((float)(xcenter - R1 * C), (float)(ycenter - R1 * S));
        needlepath[1] = new PointF((float)(xcenter + R3 * S), (float)(ycenter - R3 * C));
        needlepath[2] = new PointF((float)(xcenter + R2 * C), (float)(ycenter + R2 * S));
        needlepath[3] = new PointF((float)(xcenter - R3 * S), (float)(ycenter + R3 * C));

        if (_needleBrush == null) _needleBrush = new SolidBrush(_needleColor);
        g.FillPolygon(_needleBrush, needlepath);

        if (_needleContourThickness > 0)
        {
          if (_needleContourPen == null)
          {
            _needleContourPen = new Pen(_needleContourColor, (float)_needleContourThickness);
            _needleContourPen.StartCap = LineCap.Round;

            _needleContourPen.EndCap = LineCap.Round;
            _needleContourPen.LineJoin = LineJoin.Round;
          }

          PointF[] needlepath2 = new PointF[5];
          needlepath2[0] = needlepath[0];
          needlepath2[1] = needlepath[1];
          needlepath2[2] = needlepath[2];
          needlepath2[3] = needlepath[3];
          needlepath2[4] = needlepath[0];

          g.DrawLines(_needleContourPen, needlepath2);

        }

      }
      drawMessagePanels(g, w, h);
      return 0;

    }

    protected override void renderingPostProcessing()
    { if (_needleValue != _value) redraw();
    }

  }
}