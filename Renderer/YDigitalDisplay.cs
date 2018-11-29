/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *   Yocto 2D data renderer digital display class
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using System.Windows.Forms;


namespace YDataRendering
{


  

  public class YDigitalDisplay : YDataRenderer
  {
    

    public enum HrzAlignment {[Description("Left")]LEFT, [Description("Center")]CENTER, [Description("Decimal point")]DECIMAL, [Description("Right")]RIGHT };
    

    private Brush _bgBrush = null;

    private Color _backgroundColor1 = Color.Black;
    public Color backgroundColor1 { get { return _backgroundColor1; } set { _backgroundColor1 = value; _bgBrush = null; redraw(); } }

    private Color _backgroundColor2 = Color.FromArgb(255,48, 48, 48);
    public Color backgroundColor2 { get { return _backgroundColor2; } set { _backgroundColor2 = value; _bgBrush = null; redraw(); } }

    private String _alternateValue = null;
    public String alternateValue  { get { return _alternateValue; } set { _alternateValue = value; redraw(); } }

    private ValueFormater _valueFormater = null;
    public ValueFormater valueFormater
    {
      set { _valueFormater = value;redraw(); }
      get { return _valueFormater; }
    }


    double _hrzAlignmentOfset = 5.0;
    public double hrzAlignmentOfset { get { return _hrzAlignmentOfset; } set { _hrzAlignmentOfset = value; redraw(); } }

    HrzAlignment _hrzAlignment = HrzAlignment.DECIMAL;
    public HrzAlignment hrzAlignment { get { return _hrzAlignment; } set { _hrzAlignment = value; redraw(); } }


    private double _outOfRangeMin = Double.NaN;
    public double outOfRangeMin { get { return _outOfRangeMin; }
      set {
          if (!Double.IsNaN(value) && !Double.IsNaN(_outOfRangeMax) && !minMaxCheckDisabled)
          if (value >= _outOfRangeMax) throw new ArgumentException("Min cannot be greater than max (" + _outOfRangeMax.ToString() + ")");
          _outOfRangeMin = value; redraw();
       } }

    private double _outOfRangeMax = Double.NaN;
    public double outOfRangeMax { get { return _outOfRangeMax; }

      set {
        if (!Double.IsNaN(value) && !Double.IsNaN(_outOfRangeMin) && !minMaxCheckDisabled)
        if (value <= _outOfRangeMin) throw new ArgumentException("Max cannot be less than min (" + _outOfRangeMin.ToString() + ")");
        _outOfRangeMax = value; redraw(); } }

    private Color _outOfRangeColor = Color.Red;
    public Color outOfRangeColor { get { return _outOfRangeColor; } set { _outOfRangeColor = value;  redraw(); } }



    private double _value = 0;
    public double value { get { return _value; } set { _value = value; redraw(); } }


    YFontParams _font;
    public YFontParams font { get { return _font; } }




    public YDigitalDisplay(PictureBox ChartContainer, logFct logFunction) : base(ChartContainer, logFunction)
    {
     
      this._font       = new YFontParams(this,this, Math.Min(ChartContainer.Width/5, ChartContainer.Height/2), null);
      this._font.color = Color.LightGreen;
     
      AllowRedraw();
      Draw();

    }

    protected override void clearCachedObjects( )
    { font.ResetFont (null);
      _bgBrush = null;



    }




    protected override int Render(Graphics g, int w, int h)
    {
     
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

      StringFormat stringFormat = new StringFormat(StringFormatFlags.NoClip);
      stringFormat.Alignment = StringAlignment.Center;


      if (_bgBrush==null)
        _bgBrush = new LinearGradientBrush(new Point(0,0),
                                           new Point(0,h), _backgroundColor1, _backgroundColor2);

      g.FillRectangle(_bgBrush, 0, 0, w, h);


      // draw unit
      string svalue;
      if (_alternateValue == null)
        {
           svalue = _valueFormater == null ? value.ToString("0.0") : _valueFormater(this, value);
           if ((!Double.IsNaN(_outOfRangeMin)) && (value < _outOfRangeMin)) font.alternateColor = _outOfRangeColor;
           else if ((!Double.IsNaN(_outOfRangeMax)) && (value > _outOfRangeMax)) font.alternateColor = _outOfRangeColor;
           else font.alternateColor = null;

       }
      else
      {
        _font.alternateColor = null;
        svalue = _alternateValue;
      }

      SizeF size = g.MeasureString(svalue, font.fontObject, 10000, stringFormat);
      Rectangle pos;

      HrzAlignment align = _hrzAlignment;
      if ((_alternateValue != null) && (align == HrzAlignment.DECIMAL)) align = HrzAlignment.RIGHT;

      switch  (align)
      {
        case HrzAlignment.LEFT:
           pos = new Rectangle((int)(w* hrzAlignmentOfset /100 ), (int)((h- size.Height) / 2), (int)(size.Width + 1), (int)(size.Height + 1));
           g.DrawString(svalue, font.fontObject, font.brushObject, pos, stringFormat);
           break;
        case HrzAlignment.CENTER:
           pos = new Rectangle((int)((w - size.Width)/2), (int)((h - size.Height) / 2), (int)(size.Width + 1), (int)(size.Height + 1));
           g.DrawString(svalue, font.fontObject, font.brushObject, pos, stringFormat);
           break;
        case HrzAlignment.DECIMAL:
          
            string left = "";

            int p = svalue.LastIndexOf(',');
            if (p < 0) p = svalue.LastIndexOf('.');
            if (p >= 0)
            {
              left = svalue.Substring(0, p + 1);

            }
            else
            {
              p = 0;
              while ((p < svalue.Length) && ((svalue[p] >= '0' && svalue[p] <= '9') || (svalue[p] == '-') || (svalue[p] == '\'') || (svalue[p] == ' '))) p++;
              left = svalue.Substring(0, p);

            }

            SizeF lsize = g.MeasureString(left, font.fontObject, 10000, stringFormat);
            pos = new Rectangle((int)(w - lsize.Width - w * hrzAlignmentOfset / 100), (int)((h - size.Height) / 2), (int)(size.Width + 1), (int)(size.Height + 1));
            g.DrawString(svalue, font.fontObject, font.brushObject, pos, stringFormat);
            break;
         
        case HrzAlignment.RIGHT:
           pos = new Rectangle((int)(w- size.Width - w * hrzAlignmentOfset / 100), (int)((h - size.Height) / 2), (int)(size.Width + 1), (int)(size.Height + 1));
           g.DrawString(svalue, font.fontObject, font.brushObject, pos, stringFormat);
           break;

      }
      drawMessagePanels(g, w, h);

      return 0;
    } 

  }
}