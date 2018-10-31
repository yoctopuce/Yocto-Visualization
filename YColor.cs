using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Globalization;

namespace YColors
{




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


}
