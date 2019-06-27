/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 *
 *   Yocto 2D data renderer solid graph class
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
using System.Runtime.CompilerServices;

using System.ComponentModel;
using System.Globalization;

namespace YDataRendering
{



  public struct pointXY
  {
    public double x;
    public double y;
  }






  public static class TimeConverter
  {
    private static DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public enum TimeReference {[Description("Absolute")] ABSOLUTE, [Description("Relative to first data")] RELATIVE };
    public static Double ToUnixTime(DateTime datetime)
    {
      return (double)(datetime - sTime).TotalSeconds;
    }

    public static DateTime FromUnixTime(double unixtime)
    {
      return sTime.AddSeconds(unixtime);
    }

    public static TimeResolution BestTimeformat(double dataDeltaTime, double viewportDeltaTime, TimeReference tref)
    {
      TimeResolution res;
      bool ShowSecondsTenth = true;
      bool ShowSecondsHundredth = true;
      bool ShowSeconds = true;
      bool ShowMinutes = false;
      bool ShowHours = false;
      bool ShowDays = false;
      bool ShowMonths = false;
      bool ShowYears = false;

      if (viewportDeltaTime <= 0.10) { res.step = 0.01; }
      else if (viewportDeltaTime <= 1) { res.step = 0.1; }
      else if (viewportDeltaTime <= 2) { res.step = 0.2; }
      else if (viewportDeltaTime <= 5) { res.step = 0.5; }
      else if (viewportDeltaTime <= 10) { res.step = 1; }
      else if (viewportDeltaTime <= 20) { res.step = 2; }
      else if (viewportDeltaTime <= 30) { res.step = 3; }
      else if (viewportDeltaTime <= 40) { res.step = 4; }
      else if (viewportDeltaTime <= 60) { res.step = 5; }
      else if (viewportDeltaTime <= 120) { res.step = 10; }
      else if (viewportDeltaTime <= 300) { res.step = 30; }
      else if (viewportDeltaTime <= 900) { res.step = 60; }
      else if (viewportDeltaTime <= 1800) { res.step = 180; }
      else if (viewportDeltaTime <= 3600) { res.step = 300; }
      else if (viewportDeltaTime <= 7200) { res.step = 600; }
      else if (viewportDeltaTime <= 14000) { res.step = 900; }
      else if (viewportDeltaTime <= 21600) { res.step = 1800; }
      else if (viewportDeltaTime <= 43200) { res.step = 3600; }
      else if (viewportDeltaTime <= 86400) { res.step = 7200; }
      else if (viewportDeltaTime <= 2 * 86400) { res.step = 2 * 7200; }
      else if (viewportDeltaTime <= 4 * 86400) { res.step = 4 * 7200; }
      else if (viewportDeltaTime <= 7 * 86400) { res.step = 86400; }
      else if (viewportDeltaTime <= 14 * 86400) { res.step = 2 * 86400; }
      else if (viewportDeltaTime <= 28 * 86400) { res.step = 4 * 86400; }
      else if (viewportDeltaTime <= 56 * 86400) { res.step = 7 * 86400; }
      else if (viewportDeltaTime <= 112 * 86400) { res.step = 14 * 86400; }
      else if (viewportDeltaTime <= 224 * 86400) { res.step = 31 * 86400; }
      else if (viewportDeltaTime <= 448 * 86400) { res.step = 62 * 86400; }
      else if (viewportDeltaTime <= 896 * 86400) { res.step = 93 * 86400; }
      else { res.step = 365 * 86400; }


      if (tref == TimeReference.ABSOLUTE)
      {
        ShowSecondsHundredth = true;
        ShowSecondsTenth = true;
        ShowMinutes = true;
        ShowHours = true;
        ShowDays = dataDeltaTime > 86400;
        ShowMonths = dataDeltaTime > 86400;
        ShowYears = dataDeltaTime > 28 * 6 * 86400;

        if (res.step >= .1) ShowSecondsHundredth = false;
        if (res.step >= 1) ShowSecondsTenth = false;
        if (res.step >= 60) ShowSeconds = false;
        if (res.step >= 3600) ShowMinutes = false;
        if (res.step >= 86400) ShowHours = false;
        if (res.step >= 31 * 86400) ShowDays = false;
        if (res.step >= 365 * 86400) ShowMonths = false;

        res.format = "";
        if (ShowSecondsHundredth) res.format = ".ff";
        if (ShowSecondsTenth) res.format = ".f";
        if (ShowSeconds) res.format = "ss" + res.format;
        if (ShowMinutes) res.format = "mm" + (res.format == "" ? "" : ":") + res.format;
        if (ShowHours) res.format = "HH" + (res.format == "" ? "\\H" : ":") + res.format;
        if ((res.format != "") && (ShowDays || ShowMonths)) res.format += "\n";
        if (ShowDays) res.format = res.format + "d";
        if (ShowMonths) res.format = res.format + " MMM";
        if (ShowYears) res.format = res.format + " yyyy";

      }
      else

      {

        if (dataDeltaTime <= 0.10) { ShowSecondsHundredth = true; }
        if (dataDeltaTime <= 1) { ShowSecondsTenth = true; }
        if ((dataDeltaTime >= 60) || (viewportDeltaTime >= 60)) { ShowMinutes = true; }
        if ((dataDeltaTime >= 3600) || (viewportDeltaTime >= 3600)) { ShowHours = true; }
        if ((dataDeltaTime >= 86400) || (viewportDeltaTime >= 86400)) { ShowDays = true; }

        if (res.step >= .1) ShowSecondsHundredth = false;
        if (res.step >= 1) ShowSecondsTenth = false;
        if (res.step >= 60) ShowSeconds = false;
        if (res.step >= 3600) ShowMinutes = false;
        if (res.step >= 86400) ShowHours = false;

        res.format = "";
        if (ShowSecondsHundredth) res.format = "\\.ff";
        if (ShowSecondsTenth) res.format = "\\.f";
        if (ShowSeconds) res.format = "ss" + res.format + "\\s";
        if (ShowMinutes) res.format = "mm\\m" + res.format;
        if (ShowHours) res.format = "hh\\h" + res.format;
        if (ShowDays) res.format = "d\\d" + res.format;
      }

      return res;
    }
  }

  static public class MinMaxHandler
  {
    public struct MinMax
    {
      public double Min;
      public double Max;


    }


    public static MinMax extend(MinMax M, double factor)
    {


      if (Double.IsNaN(M.Min)) return M;
      double delta = M.Max - M.Min;
      return new MinMax { Min = M.Min - (delta * (factor - 1)) / 2, Max = M.Max + (delta * (factor - 1)) / 2 };
    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static MinMax DefaultValue()
    {
      return new MinMax { Min = Double.NaN, Max = Double.NaN };

    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static MinMax DefaultValue(double value)
    {
      return new MinMax { Min = value, Max = value };

    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static MinMax DefaultValue(double value1, double value2)
    {
      if (value2 < value1) throw new InvalidOperationException("MinMax invalid parameters (" + value1.ToString() + ">" + value2.ToString());

      return new MinMax { Min = value1, Max = value2 };

    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static bool isDefined(MinMax v)
    {
      return !Double.IsNaN(v.Min);

    }


#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static MinMax Combine(MinMax M1, MinMax M2)
    {
      if (Double.IsNaN(M1.Min)) return M2;
      if (Double.IsNaN(M2.Min)) return M1;
      if (M1.Min < M2.Min) M2.Min = M1.Min;
      if (M1.Max < M2.Min) M2.Min = M1.Max;
      if (M1.Min > M2.Max) M2.Max = M1.Min;
      if (M1.Max > M2.Max) M2.Max = M1.Max;
      return M2;
    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static MinMax Combine(MinMax M1, double value)
    {
      if (double.IsNaN(M1.Min)) { M1.Min = value; M1.Max = value; return M1; }
      if (value < M1.Min) M1.Min = value;
      if (value > M1.Max) M1.Max = value;
      return M1;
    }


  }

  public class DataSegment
  {
    const int SegmentGranularity = 10000;
    public pointXY[] data;
    public int count;
    //  public double max;
    //  public double min;

    public DataSegment(pointXY p)
    {
      data = new pointXY[SegmentGranularity];
      data[0] = p;
      count = 1;
      //    min = p.y;
      //    max = p.y;

    }


    public DataSegment(pointXY[] p)
    {
      data = new pointXY[p.Length];
      Array.Copy(p, 0, data, 0, p.Length);
      count = p.Length;
      /*    min = data[0].y;
          max = data[0].y;
          for (int i=1;i<count;i++)
          { if (min > data[i].y) min = data[i].y;
            if (max > data[i].y) max = data[i].y;
          }
        */
    }

    public void grow() { Array.Resize(ref data, data.Length + SegmentGranularity); }




  }


  public class DataSerie
  {
    protected YGraph parent = null;
    static int _MaxPointsPerSeries = 0;
    int totalPointCount = 0;
    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public static int MaxPointsPerSeries { get { return _MaxPointsPerSeries; } set { _MaxPointsPerSeries = value; } }


    public DataSerie(YGraph parent)
    {
      if (parent.yAxes.Count <= 0) throw new System.ArgumentException("Define at least one yAxis");
      _timeRange = MinMaxHandler.DefaultValue();
      _valueRange = MinMaxHandler.DefaultValue();

      this.parent = parent;

    }

    private int _yAxisIndex = 0;
    public int yAxisIndex
    {
      get { return _yAxisIndex; }
      set
      {
        if (value >= parent.yAxes.Count) throw new System.ArgumentException("No such yAxis (" + value.ToString() + ")");
        _yAxisIndex = value;
        parent.yAxes[_yAxisIndex].AutoShow();

      }
    }

    private Pen _pen = null;
    private Pen _legendPen = null;


    public Pen pen
    {
      get
      {
        if (_pen == null)
        {
          _pen = new Pen(_color, (float)_thickness);
          _pen.EndCap = LineCap.Round;
          _pen.LineJoin = LineJoin.Round;
        }
        return _pen;
      }
    }

    public Pen legendPen
    {
      get
      {
        if (_legendPen == null)
        {
          _legendPen = new Pen(_color, (float)(_thickness* parent.legendPanel.traceWidthFactor));
          

        }
        return _legendPen;
      }
    }

    public void resetlegendPen() { _legendPen = null; } 

    private Brush _brush = null;


    public Brush brush
    {
      get
      {
        if (_brush == null) _brush = new SolidBrush(_color);
        return _brush;
      }
    }



    private Pen _navigatorpen = null;
    public Pen navigatorpen
    {
      get
      {
        if (_navigatorpen == null) _navigatorpen = new Pen(Color.FromArgb(100, _color), (float)1.0);
        return _navigatorpen;
      }
    }


    private bool _visible = true;  // whet not visible, series is  not shown but still intervene in Axis auto range calculus
    public bool visible { get { return _visible; } set { _visible = value; parent.redraw(); } }


    private bool _disabled = false;  // when series is disabled,rendering acts just like to series does not exists
    public bool disabled { get { return _disabled; } set { _disabled = value; parent.redraw(); } }



    private Color _color = Color.Black;
    public Color color { get { return _color; } set { _color = value; _pen = null; _legendPen = null; _brush = null; _navigatorpen = null; parent.redraw(); } }

    private double _thickness = 1.0;
    public double thickness
    {
      get { return _thickness; }
      set
      {
        if (value <= 0) throw new ArgumentException("Thickness must be a positive value");
        _thickness = value; _pen = null; _legendPen = null;  parent.redraw();
      }
    }

    private string _legend = "";
    public string legend { get { return _legend; } set { _legend = value; parent.redraw(); } }

    private string _unit = "";
    public string unit { get { return _unit; } set { _unit = value; parent.redraw(); } }

    public List<DataSegment> segments = new List<DataSegment>();

    private void AddNewSegment(pointXY p)
    {
      segments.Insert(0, new DataSegment(p));
    }

    private MinMaxHandler.MinMax _timeRange;
    public MinMaxHandler.MinMax timeRange { get { return _timeRange; } }

    private MinMaxHandler.MinMax _valueRange;
    public MinMaxHandler.MinMax valueRange { get { return _valueRange; } }



    public void AddPoint(pointXY p)
    {


      _timeRange = MinMaxHandler.Combine(_timeRange, p.x);
      _valueRange = MinMaxHandler.Combine(_valueRange, p.y);



      if (segments.Count <= 0) { AddNewSegment(p); totalPointCount++; return; }
      else if (segments[0].count > 1)
      {
        double delta1 = segments[0].data[segments[0].count - 1].x - segments[0].data[segments[0].count - 2].x;
        double delta2 = p.x - segments[0].data[segments[0].count - 1].x;
        if ((delta2 > 0.1) && ((delta2 < 0) || (delta2 > 2 * delta1))) { AddNewSegment(p); return; }
        else if (segments[0].count >= segments[0].data.Length) segments[0].grow();

      }

      segments[0].data[segments[0].count] = p;
      segments[0].count++;
      totalPointCount++;
      if ((_MaxPointsPerSeries > 0) && (totalPointCount > _MaxPointsPerSeries)) dataCleanUp();

      parent.adjustGlobalTimeRange(p.x);
      parent.redraw();
    }

    public void dataCleanUp()
    {
      if (segments.Count <= 0) return;
      int newLimit = ((_MaxPointsPerSeries * 90) / 100);
      while (segments[segments.Count - 1].count <= (totalPointCount - newLimit))
      {
        totalPointCount -= segments[segments.Count - 1].count;
        segments.RemoveAt(segments.Count - 1);
      }

      if (totalPointCount > newLimit)
      {
        int delta = totalPointCount - newLimit;
        int newsize = segments[segments.Count - 1].count - delta;
        pointXY[] newdata = new pointXY[newsize];
        Array.Copy(segments[segments.Count - 1].data, delta, newdata, 0, segments[segments.Count - 1].count - delta);
        segments[segments.Count - 1].data = newdata;
        segments[segments.Count - 1].count -= delta;
        totalPointCount -= delta;

      }

      double tmin = segments[0].data[0].x;
      double tmax = segments[0].data[0].x;
      double ymin = segments[0].data[0].y;
      double ymax = segments[0].data[0].y;

      for (int i = 0; i < segments.Count; i++)
      {
        int count = segments[i].count;
        if (tmin > segments[i].data[0].x) tmin = segments[i].data[0].x;
        if (tmax < segments[i].data[count - 1].x) tmax = segments[i].data[count - 1].x;
        for (int j = 0; j < count; j++)
        {
          if (ymin > segments[i].data[j].y) ymin = segments[i].data[j].y;
          if (ymax < segments[i].data[j].y) ymax = segments[i].data[j].y;
        }
      }
      _timeRange.Min = tmin;
      _timeRange.Max = tmax;
      _valueRange.Min = ymin;
      _valueRange.Max = ymax;


    }

    public void InsertPoints(pointXY[] points)
    {
      if (points.Length == 0) return;
      if (points.Length == 1)
      {
        _timeRange = MinMaxHandler.Combine(_timeRange, points[0].x);
        _valueRange = MinMaxHandler.Combine(_valueRange, points[0].y);
        return;
      }

      double FirstStep = points[1].x - points[0].x;
      double LastStep = points[points.Length - 1].x - points[points.Length - 2].x;
      int InsertAtBegining = -1;
      int InsertAtEnd = -1;
      int sz = System.Runtime.InteropServices.Marshal.SizeOf(typeof(pointXY));

      // can we merge with one already existing segment ?
      for (int i = 0; i < segments.Count; i++)
        if (segments[i].count > 1)
        {
          double DeltaInsertAtBegining = segments[i].data[0].x - points[points.Length - 1].x;
          double DeltaInsertAtEnd = points[0].x - segments[i].data[segments[i].count - 1].x;
          if ((DeltaInsertAtBegining > 0) && (DeltaInsertAtBegining < 2 * FirstStep)) InsertAtBegining = i;
          if ((DeltaInsertAtEnd > 0) && (DeltaInsertAtEnd < 2 * LastStep)) InsertAtEnd = i;
        }


      if (InsertAtBegining >= 0)  // insert at the beginning of segments[InsertAtBegining]
      {
        if (segments[InsertAtBegining].count + points.Length >= segments[InsertAtBegining].data.Length) segments[InsertAtBegining].grow();
        Array.Copy(segments[InsertAtBegining].data, 0, segments[InsertAtBegining].data, points.Length, segments[InsertAtBegining].count);
        Array.Copy(points, 0, segments[InsertAtBegining].data, 0, points.Length);
        segments[InsertAtBegining].count += points.Length;
        totalPointCount += points.Length;
      }

      else if (InsertAtEnd >= 0) // insert at the end of segments[InsertAtEnd]
      {

        if (segments[InsertAtEnd].count + points.Length >= segments[InsertAtEnd].data.Length) segments[InsertAtEnd].grow();
        Array.Copy(points, 0, segments[InsertAtEnd].data, segments[InsertAtEnd].count, points.Length);
        segments[InsertAtEnd].count += points.Length;
        totalPointCount += points.Length;
      }
      else // create a whole new segment
      {
        segments.Add(new DataSegment(points));
        totalPointCount += points.Length;
      }

      _timeRange = MinMaxHandler.Combine(_timeRange, points[0].x);
      _timeRange = MinMaxHandler.Combine(_timeRange, points[points.Length - 1].x);

      for (int i = 0; i < points.Length; i++)
        _valueRange = MinMaxHandler.Combine(_valueRange, points[i].y);

      if ((_MaxPointsPerSeries > 0) && (totalPointCount > _MaxPointsPerSeries)) dataCleanUp();


      parent.redraw();

    }

    private static int CompareSegments(DataSegment a, DataSegment b)

    {
      if (a.data[0].x > b.data[0].x) return -1;

      if (a.data[0].x < b.data[0].x) return 1;
      return 0;

    }

    public List<pointXY> getData()
    {
      List<pointXY> res = new List<pointXY>();

      segments.Sort(CompareSegments);
      for (int i = segments.Count - 1; i >= 0; i--)
        for (int j = 0; j < segments[i].count; j++)
          res.Add(segments[i].data[j]);

      return res;

    }


    public Nullable<pointXY> findClosestValue(Double x, bool AllowInterpolation)
    {
      int N1, N2, Pos;

      if (segments.Count <= 0) return null;

      // check for best match inside segments
      for (int i = 0; i < segments.Count; i++)
      {
        if ((x >= segments[i].data[0].x) && (x <= segments[i].data[segments[i].count - 1].x))
        {
          pointXY[] data = segments[i].data;
          N1 = 0; N2 = segments[i].count - 1;
          while (N2 - N1 > 1)
          {
            int N = (N1 + N2) >> 1;
            if (data[N].x > x) N2 = N; else N1 = N;
          }
          Pos = N1 - 1; if (Pos < 0) Pos = 0;
          if (x - data[Pos].x < data[Pos + 1].x - x) return data[Pos]; else return data[Pos + 1];
        }
      }

      // check for best match outside segments
      pointXY match = segments[0].data[0];
      double delta = Math.Abs(segments[0].data[0].x - x);
      for (int i = 0; i < segments.Count; i++)
      {
        double d1 = Math.Abs(segments[i].data[0].x - x);
        double d2 = Math.Abs(segments[i].data[segments[i].count - 1].x - x);
        if (d1 < delta) { match = segments[i].data[0]; delta = d1; }
        if (d2 < delta) { match = segments[i].data[segments[i].count - 1]; delta = d2; }
      }



      return match;
    }


    public void clear()
    {
      segments.Clear();
      _timeRange = MinMaxHandler.DefaultValue();
      _valueRange = MinMaxHandler.DefaultValue();
      parent.clearCachedObjects();
      totalPointCount = 0;
    }

  }




  public struct ViewPortSettings
  {
    public double IRLx;
    public double IRLy;

    public double zoomx;
    public double zoomy;
    public int Lmargin;
    public int Rmargin;
    public int Tmargin;
    public int Bmargin;
    public int Width;
    public int Height;
    public bool Capture;
    public double IRLCaptureStartX;
    public int CaptureStartY;
    public double OriginalXAxisMin;
    public double OriginalXAxisMax;
    public double OriginalIRLx;
    public double OriginalLmargin;
    public double OriginalZoomx;

  }


  public class DataTracker
  {
    private YDataRenderer _parentRenderer = null;
    private Object _directParent = null;
    public Object directParent { get { return _directParent; } }

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public DataTracker(YDataRenderer parent, object directParent)
    {
      this._directParent = directParent;
      this._parentRenderer = parent;
      this._font = new YFontParams(parent, this, 8, null);

    }

    private bool _enabled = false;
    public bool enabled { get { return _enabled; } set { _enabled = value; _parentRenderer.redraw(); } }

    private bool _showSerieName = false;
    public bool showSerieName { get { return _showSerieName; } set { _showSerieName = value; _parentRenderer.redraw(); } }

    private string _dataPrecisionString = "";
    public enum DataPrecision
    {
      [Description("As is")] PRECISION_NOLIMIT,
      [Description("1")] PRECISION_1,
      [Description("0.1")] PRECISION_01,
      [Description("0.01")] PRECISION_001,
      [Description("0.001")] PRECISION_0001,
      [Description("0.0001")] PRECISION_00001,
      [Description("0.00001")] PRECISION_000001,
      [Description("0.000001")] PRECISION_0000001,
      [Description("0.0000001")] PRECISION_00000001,
      [Description("0.00000001")] PRECISION_000000001
    };

    private DataPrecision _dataPrecision = DataPrecision.PRECISION_NOLIMIT;
    public DataPrecision dataPrecision { get { return _dataPrecision; } set { _dataPrecision = value; compute_dataPrecisionString(); _parentRenderer.redraw(); } }


    private void compute_dataPrecisionString()
    {

      if (_dataPrecision == DataPrecision.PRECISION_NOLIMIT) { _dataPrecisionString = ""; return; }
      _dataPrecisionString = "0.";
      for (int i = 1; i < (int)_dataPrecision; i++) _dataPrecisionString += "#";
    }



    public string dataPrecisionString { get { return _dataPrecisionString; } }



    private double _diameter = 5;
    public double diameter { get { return _diameter; } set { _diameter = value; _parentRenderer.redraw(); } }

    private double _handleLength = 25;
    public double handleLength
    {
      get { return _handleLength; }
      set
      {
        if (value <= 0) throw new ArgumentException("Diameter must be a positive value");
        _handleLength = value; _parentRenderer.redraw();
      }
    }

    private int _detectionDistance = 50;
    public int detectionDistance
    {
      get { return _detectionDistance; }
      set
      {
        if (value <= 0) throw new ArgumentException("Distance must be a positive value");
        _detectionDistance = value;
      }
    }


    private Color _bgColor = Color.FromArgb(200, 255, 255, 255);
    public Color bgColor { get { return _bgColor; } set { _bgColor = value; _bgBrush = null; _parentRenderer.redraw(); } }

    private Color _borderColor = Color.Black;
    public Color borderColor { get { return _borderColor; } set { _borderColor = value; _pen = null; _parentRenderer.redraw(); } }

    private double _borderthickness = 1.0;
    public double borderthickness
    {
      get { return _borderthickness; }
      set
      {
        if (value <= 0) throw new ArgumentException("Thickness must be a positive value");
        _borderthickness = value; _pen = null; _parentRenderer.redraw();
      }
    }

    private double _padding = 10;
    public double padding { get { return _padding; } set { _padding = value; _parentRenderer.redraw(); } }

    private double _verticalMargin = 10;
    public double verticalMargin
    {
      get { return _verticalMargin; }
      set
      {
        if (value < 0) throw new ArgumentException("Margin must be a positive value");
        _verticalMargin = value; _parentRenderer.redraw();
      }
    }

    private double _horizontalMargin = 10;
    public double horizontalMargin
    {
      get { return _horizontalMargin; }
      set
      {
        if (value < 0) throw new ArgumentException("Margin must be a positive value");
        _horizontalMargin = value; _parentRenderer.redraw();
      }
    }



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



  public class LegendPanel
  {
    private YDataRenderer _parentRenderer = null;
    private Object _directParent = null;
    public Object directParent { get { return _directParent; } }

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public enum Position {[Description("Left")] LEFT, [Description("Top-Left")] TOPLEFT, [Description("Top")] TOP, [Description("Top-Right")] TOPRIGHT, [Description("Right")] RIGHT, [Description("Bottom-Right")] BOTTOMRIGHT, [Description("Bottom")] BOTTOM, [Description("Bottom-Left")] BOTTOMLEFT };

    protected double _traceWidth = 1.0;
    public double traceWidthFactor
    {
      get { return _traceWidth; }
      set
      {
        if (value > 0)
        {
          _traceWidth = value;
          _parentRenderer.resetlegendPens();
          _parentRenderer.redraw();
        }
        else throw new ArgumentException("This has to be a strictly positive value");
      }
    }

    public LegendPanel(YDataRenderer parent, Object directParent)
    {
      _directParent = directParent;
      _parentRenderer = parent;
      _font = new YFontParams(parent, this, 8, null);

    }

    private bool _enabled = false;
    public bool enabled { get { return _enabled; } set { _enabled = value; _parentRenderer.redraw(); } }

    private Position _position = Position.BOTTOM;
    public Position position { get { return _position; } set { _position = value; _parentRenderer.redraw(); } }

    private bool _overlap = false;
    public bool overlap { get { return _overlap; } set { _overlap = value; _parentRenderer.redraw(); } }



    private Color _bgColor = Color.FromArgb(200, 255, 255, 255);
    public Color bgColor { get { return _bgColor; } set { _bgColor = value; _bgBrush = null; _parentRenderer.redraw(); } }

    private Color _borderColor = Color.Black;
    public Color borderColor { get { return _borderColor; } set { _borderColor = value; _pen = null; _parentRenderer.redraw(); } }

    private double _borderthickness = 1.0;
    public double borderthickness
    {
      get { return _borderthickness; }
      set
      {
        if (value < 0) throw new ArgumentException("Thickness must be a positive value");
        _borderthickness = value; _pen = null; _parentRenderer.redraw();
      }
    }

    private double _padding = 10;
    public double padding
    {
      get { return _padding; }
      set
      {
        if (value < 0) throw new ArgumentException("Padding must be a positive value");
        _padding = value; _parentRenderer.redraw();
      }
    }

    private double _verticalMargin = 10;
    public double verticalMargin
    {
      get { return _verticalMargin; }
      set
      {
        if (value < 0) throw new ArgumentException("Margin must be a positive value");
        _verticalMargin = value; _parentRenderer.redraw();
      }
    }

    private double _horizontalMargin = 10;
    public double horizontalMargin
    {
      get { return _horizontalMargin; }
      set
      {
        if (value < 0) throw new ArgumentException("Margin must be a positive value");
        _horizontalMargin = value; _parentRenderer.redraw();
      }
    }



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


  public class Navigator
  {
    private YGraph _parentRenderer = null;

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    private Object _directParent = null;
    public Object directParent { get { return _directParent; } }

    public enum YAxisHandling {[Description("Automatic")] AUTO, [Description("Inherit from main view")] INHERIT };

    private ViewPortSettings _viewport = new ViewPortSettings
    {
      Lmargin = 0,
      Rmargin = 0,
      Tmargin = 0,
      Bmargin = 0,
      Width = 0,
      Height = 0
    };

    public MinMaxHandler.MinMax Xrange;

    private bool _showXAxisZones = true;

    public bool showXAxisZones { get { return _showXAxisZones; } set { _showXAxisZones = value; } }

    double _relativeheight = 10;
    public double relativeheight
    {
      get { return _relativeheight; }
      set
      {
        if (value < 10) value = 10;
        if (value > 50) value = 50;
        _relativeheight = value;
        _parentRenderer.redraw();
      }
    }



    public Navigator(YGraph parent, Object directParent)
    {
      _directParent = directParent;
      _parentRenderer = parent;
      _font = new YFontParams(parent, this, 8, null);

    }

    private bool _enabled = false;
    public bool enabled { get { return _enabled; } set { _enabled = value; _parentRenderer.redraw(); } }

    private Color _bgColor1 = Color.FromArgb(255, 225, 225, 225);
    public Color bgColor1 { get { return _bgColor1; } set { _bgColor1 = value; _bgBrush = null; _parentRenderer.redraw(); } }


    private Color _cursorBorderColor = Color.FromArgb(255, 40, 40, 40);
    public Color cursorBorderColor { get { return _cursorBorderColor; } set { _cursorBorderColor = value; _cursorBorderPen = null; _parentRenderer.redraw(); } }




    private YAxisHandling _yAxisHandling = YAxisHandling.AUTO;
    public YAxisHandling yAxisHandling { get { return _yAxisHandling; } set { _yAxisHandling = value; _parentRenderer.redraw(); } }

    private Color _bgColor2 = Color.FromArgb(255, 255, 255, 255);
    public Color bgColor2 { get { return _bgColor2; } set { _bgColor2 = value; _bgBrush = null; _parentRenderer.redraw(); } }


    private Color _cursorColor = Color.FromArgb(100, 0, 255, 0);
    public Color cursorColor { get { return _cursorColor; } set { _cursorColor = value; _cursorBrush = null; _parentRenderer.redraw(); } }


    private Brush _cursorBrush = null;
    public Brush cursorBrush
    {
      get
      {
        if (_cursorBrush == null)
          _cursorBrush = new SolidBrush(_cursorColor);
        return _cursorBrush;
      }
    }

    private Pen _pen = null;
    public Pen pen
    {
      get
      {
        _pen = new Pen(_xAxisColor, (float)_xAxisThickness);
        return _pen;
      }
    }

    private Pen _cursorBorderPen = null;
    public Pen cursorBorderPen
    {
      get
      {
        _cursorBorderPen = new Pen(_cursorBorderColor, 1);
        return _cursorBorderPen;
      }
    }





    private Color _xAxisColor = Color.Black;
    public Color xAxisColor { get { return _xAxisColor; } set { _xAxisColor = value; _pen = null; _parentRenderer.redraw(); } }

    private double _xAxisThickness = 1.0;
    public double xAxisThickness
    {
      get { return _xAxisThickness; }
      set
      {
        if (value < 0) throw new ArgumentException("Thickness must be a positive value");
        _xAxisThickness = value; _pen = null; _parentRenderer.redraw();
      }
    }


    private Pen _borderPen = null;
    public Pen borderPen
    {
      get
      {
        _borderPen = new Pen(_borderColor, (float)_borderThickness);
        return _borderPen;
      }
    }

    private Color _borderColor = Color.DarkGray;
    public Color borderColor { get { return _borderColor; } set { _borderColor = value; _borderPen = null; _parentRenderer.redraw(); } }

    private Double _borderThickness = 1.0;
    public Double borderThickness
    {
      get { return _borderThickness; }
      set
      {
        if (value < 0) throw new ArgumentException("Thickness must be a positive value");
        _borderThickness = value; _borderPen = null; _parentRenderer.redraw();
      }
    }


    private LinearGradientBrush _bgBrush = null;
    public void setPosition(int ParentWidth, int ParentHeight, int Lmargin, int Rmargin, int Tmargin, int Bmargin)
    {
      if ((_viewport.Lmargin != Lmargin) || (_viewport.Rmargin != Rmargin) || (_viewport.Tmargin != Tmargin) || (_viewport.Bmargin != Bmargin)) _bgBrush = null;
      _viewport.Lmargin = Lmargin;
      _viewport.Rmargin = Rmargin;
      _viewport.Bmargin = Bmargin;
      _viewport.Tmargin = Tmargin;
      _viewport.Width = ParentWidth;
      _viewport.Height = ParentHeight;
    }

    public void setIRLPosition(double IRLx, double IRLy, double xZoom, double yZoom)
    {
      _viewport.IRLx = IRLx;
      _viewport.IRLy = IRLy;
      _viewport.zoomx = xZoom;
      _viewport.zoomy = yZoom;



    }

    public void startCapture(pointXY IRLStartPoint, double xAxisMin, double xAxisMax)
    {
      _viewport.OriginalXAxisMin = xAxisMin;
      _viewport.OriginalXAxisMax = xAxisMax;

      _viewport.OriginalIRLx = _viewport.IRLx;
      _viewport.OriginalLmargin = _viewport.Lmargin;
      _viewport.OriginalZoomx = _viewport.zoomx;
      _viewport.IRLCaptureStartX = IRLStartPoint.x;

      _viewport.Capture = true;
    }

    public void stopCapture()

    {
      _viewport.Capture = false;

    }

    public ViewPortSettings viewport
    {
      get { return _viewport; }
    }

    public LinearGradientBrush bgBrush
    {
      get
      {
        if (_bgBrush == null)
          _bgBrush = new LinearGradientBrush(new Point(0, _viewport.Height - _viewport.Bmargin), new Point(0, _viewport.Tmargin), _bgColor1, _bgColor2);
        return _bgBrush;
      }
    }

    YFontParams _font = null;
    public YFontParams font { get { return _font; } }
  }




  public class Legend
  {

    protected YGraph _parentRenderer = null;

    private Object _directParent = null;
    public Object directParent { get { return _directParent; } }

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public Legend(YGraph parent, Object directParent)
    {
      _directParent = directParent;
      _parentRenderer = parent;
      _font = new YFontParams(parent, this, 12, null);
    }

    private String _title = "";
    public string title { get { return _title; } set { _title = value; _parentRenderer.redraw(); } }



    YFontParams _font = null;
    public YFontParams font { get { return _font; } }

  }


  abstract public class GenericAxis
  {
    protected YGraph _parentRenderer = null;
    private Object _directParent = null;
    public Object directParent { get { return _directParent; } }

    protected List<Zone> _zones;
    public List<Zone> zones { get { return _zones; } }


    public Zone AddZone()
    {

      Zone z = new Zone(_parentRenderer, this);
      _zones.Add(z);
      return z;
    }

    private Object _userData = null;
    public Object userData { get { return _userData; } set { _userData = value; } }

    public delegate void axisChangedCallBack(GenericAxis source);


    protected axisChangedCallBack _AxisChanged = null;

    public axisChangedCallBack AxisChanged { set { _AxisChanged = value; } get { return _AxisChanged; } }

    public GenericAxis(YGraph parent, Object directParent)
    {
      _zones = new List<Zone>();
      _directParent = directParent;
      _parentRenderer = parent;
      _legend = new Legend(parent, this);
      _font = new YFontParams(parent, this);
    }


    protected Pen _pen = null;
    public Pen pen
    {
      get
      {
        if (_pen == null)
        {
          _pen = new Pen(_color, (float)_thickness);
        }
        return _pen;
      }
    }

    protected Pen _gridPen = null;
    public Pen gridPen
    {
      get
      {
        if (_gridPen == null)
        {
          _gridPen = new Pen(_gridColor, (float)_gridThickness);
        }
        return _gridPen;
      }
    }


    protected bool _visible = true;
    public bool visible { get { return _visible; } set { _visible = value; if (!value) { _AllowAutoShow = false; } _parentRenderer.redraw(); } }


    protected bool _AllowAutoShow = false;
    public bool AllowAutoShow { get { return _AllowAutoShow; } set { _AllowAutoShow = value; } }

    public void AutoShow() { if (_AllowAutoShow) { visible = true; if (_AxisChanged != null) _AxisChanged(this); } }


    public void set_minMax(double value_min, double value_max)
    {
      if (!Double.IsNaN(value_min) && !Double.IsNaN(value_max) && (value_min >= value_max))
        throw new ArgumentException("Min (" + value_min.ToString() + ") cannot be greater than max (" + value_max.ToString() + ")");

      _min = value_min;
      _max = value_max;
      _parentRenderer.redraw();
    }

    protected Double _min = Double.NaN;
    public Double min
    {
      get { return _min; }

      set
      {
        if (!Double.IsNaN(value) && !Double.IsNaN(_max) && !YDataRenderer.minMaxCheckDisabled)
          if (value >= _max)
            throw new ArgumentException("Min cannot be greater than max (" + _max.ToString() + ")");
        _min = value; _parentRenderer.redraw();
      }
    }

    protected Double _max = Double.NaN;
    public Double max
    {
      get { return _max; }
      set
      {
        if (!Double.IsNaN(value) && !Double.IsNaN(_min) && !YDataRenderer.minMaxCheckDisabled)
          if (value <= _min) throw new ArgumentException("Max cannot be less than min (" + _min.ToString() + ")");

        _max = value;
        _parentRenderer.redraw();
      }
    }

    protected Double _step = Double.NaN;
    public Double step { get { return _step; } set { _step = value; _parentRenderer.redraw(); } }

    protected Double _thickness = 1.0;
    public Double thickness
    {
      get { return _thickness; }
      set
      {
        if (value < 0) throw new ArgumentException("Thickness must be a positive value");
        _thickness = value; _pen = null; _parentRenderer.redraw();
      }
    }

    protected Color _color = Color.Black;
    public Color color { get { return _color; } set { _color = value; _pen = null; _parentRenderer.redraw(); } }


    protected bool _showGrid = false;
    public bool showGrid { get { return _showGrid; } set { _showGrid = value; _parentRenderer.redraw(); } }

    protected Color _gridColor = Color.FromArgb(50, 0, 0, 0);
    public Color gridColor { get { return _gridColor; } set { _gridColor = value; _gridPen = null; _parentRenderer.redraw(); } }

    protected Double _gridThickness = 1.0;
    public Double gridThickness
    {
      get { return _gridThickness; }
      set
      {
        if (value <= 0) throw new ArgumentException("Thickness must be a positive value");
        _gridThickness = value; _gridPen = null; _parentRenderer.redraw();
      }
    }


    YFontParams _font = null;
    public YFontParams font { get { return _font; } }


    Legend _legend = null;
    public Legend legend { get { return _legend; } }
  }

  public struct StartStopStep
  {
    public double dataMin;
    public double dataMax;
    public double absMin;
    public double absMax;
    public double step;
    public double start;
    public double stop;

    public int precision;


  }

  public class ReadOnlyIndexedProperty<TValue>
  {
    readonly List<TValue> ContainerList;

    public ReadOnlyIndexedProperty(List<TValue> list)
    {
      this.ContainerList = list;
    }

    public TValue this[int i]
    {
      get
      {
        return ContainerList[i];
      }
    }

    public int Count { get { return ContainerList.Count; } }
  }


  public class YAxis : GenericAxis
  {




    NumberFormatInfo nfi;
    int _index = 0;


    public YAxis(YGraph parent, Object directParent, int index) : base(parent, directParent)
    {
      _index = index; ;

      nfi = new NumberFormatInfo();
      nfi.NumberDecimalSeparator = ".";

    }

    public int index { get { return _index; } }


    public enum HrzPosition {[Description("Left")] LEFT, [Description("Right")] RIGHT };


    public void lockMinMax()
    {
      _min = startStopStep.absMin;
      _max = startStopStep.absMax;
      _parentRenderer.redraw();
    }

    public void unlockMinMax()
    { _min = Double.NaN;
      _max = Double.NaN;
      _parentRenderer.redraw();
    }


    private bool _highlightZero = false;
    public bool highlightZero { get { return _highlightZero; } set { _highlightZero = value; _parentRenderer.redraw(); }  }


    private HrzPosition _position = HrzPosition.LEFT;
    public HrzPosition position { get { return _position; } set { _position = value; _parentRenderer.redraw(); } }

    public int innerWidth = 0;


    public double zoom = 0;
    public double IRLy = 0;


    public StartStopStep startStopStep = new StartStopStep { start = 0, stop = 1, step = .1 };



    public StartStopStep computeStartAndStep(MinMaxHandler.MinMax M)
    {
      StartStopStep res;
      double min = this.min;
      double max = this.max;
      res.step = step;
      res.precision = 0;

      if (!MinMaxHandler.isDefined(M)) M = MinMaxHandler.DefaultValue(0.0, 100.0);
      if (Double.IsNaN(min)) min = M.Min;
      if (Double.IsNaN(max)) max = M.Max;

      res.absMax = max;
      res.absMin = min;



      if (min == max) { min -= 0.5; max += 0.5; }

      if (min != 0) min -= (max - min) * 0.025;
      if (max != 0) max += (max - min) * 0.025;



      res.start = min;
      res.stop = max;
      res.dataMin = min;
      res.dataMax = max;

      Double Delta = max - min;



      if (Double.IsNaN(res.step))
      {
        Double MagnitudePwr = Math.Log10(Delta);
        if ((MagnitudePwr - Math.Floor(MagnitudePwr)) != 0) MagnitudePwr = Math.Floor(MagnitudePwr) + 1;

        res.precision = (int)MagnitudePwr - 1;
        Double Magnitude = Math.Pow(10, res.precision);

        Double C = Delta / Magnitude;

        if (C <= 2) { res.step = (Magnitude / 5); res.precision--; }

        else if (C <= 5) { res.step = (Magnitude / 2); res.precision--; }
        else res.step = (Magnitude);

        if (Double.IsNaN(this.min))
        {
          double c = min / res.step;
          if (c - Math.Floor(c) != 0) c = (c > 0) ? Math.Floor(c) + 1 : Math.Floor(c) - 1;
          res.start = res.step * c;


          //     if ((M.Min < 0) && (M.Min - (int)M.Min != 0)) res.start -= res.step;

        }
      }
      else
      {
        String v = res.step.ToString(nfi);
        int p = v.IndexOf('.');
        if (p >= 0)
        {
          res.precision = -(v.Length - p - 1);
        }
        else res.precision = 0;


      }

      startStopStep = res;
      return res;

    }
  }

  public struct TimeResolution
  {
    public Double step;
    public string format;

  }


  public class XAxis : GenericAxis
  {
    public const string FORMATAUTO = "auto";

    public enum VrtPosition {[Description("Top")] TOP, [Description("Bottom")] BOTTOM };
    public enum OverflowHandling {[Description("Do nothing")] DONOTHING, [Description("Scroll contents")] SCROLL, [Description("Squeeze contents")] CONTRACT };


    private VrtPosition _position = VrtPosition.BOTTOM;
    public VrtPosition position { get { return _position; } set { _position = value; _parentRenderer.redraw(); } }



    private double _initialZoom = 300;
    public double initialZoom
    {
      get { return _initialZoom; }
      set
      {
        if (value <= 0) throw new ArgumentException("Zoom must be a positive value");
        _initialZoom = value; max = min + initialZoom; _parentRenderer.redraw();
      }
    }

    private String _format = FORMATAUTO;
    public string labelFormat
    {
      get { return _format; }

      set
      {
        if (labelFormat != FORMATAUTO)
          try { 1234.ToString(labelFormat); }
          catch (Exception) { throw new ArgumentException("\"format\" is not a valid format."); }


        _format = value;
        _parentRenderer.redraw();
      }
    }

    public XAxis(YGraph parent, Object directParent) : base(parent, directParent)
    {
      min = TimeConverter.ToUnixTime(DateTime.UtcNow);
      max = min + initialZoom;
      step = 30;
    }


    protected TimeConverter.TimeReference _timeReference = TimeConverter.TimeReference.ABSOLUTE;
    public TimeConverter.TimeReference timeReference
    { get { return _timeReference; } set { _timeReference = value; _parentRenderer.redraw(); } }

    public TimeResolution bestFormat(double dataTimedelta, double viewportTimedelta)
    {

      return TimeConverter.BestTimeformat(dataTimedelta, viewportTimedelta, _timeReference);


    }

    public int innerHeight = 0;

    private OverflowHandling _overflowHandling = OverflowHandling.DONOTHING;
    public OverflowHandling overflowHandling { get { return _overflowHandling; } set { _overflowHandling = value; } }

  }


  public class DataPanel : GenericPanel
  {

    public enum HorizontalAlign
    {[Description("Left")] LEFTOF, [Description("Center")] CENTERED, [Description("Right")] RIGHTOF };
    public enum VerticalAlign
    {[Description("Top")] ABOVE, [Description("Center")] CENTERED, [Description("Bottom")] BELOW };

    public enum HorizontalPosition
    {
      [Description("Left border")] LEFTBORDER, [Description("Absolute X position")] ABSOLUTEX, [Description("Right border")] RIGHTBORDER
    };

    public enum VerticalPosition
    {
      [Description("Top border")] TOPBORDER, [Description("Absolute Y position")] ABSOLUTEY, [Description("Bottom border")] BOTTOMBORDER
    };

    public DataPanel(YDataRenderer parent, Object directParent) : base(parent, directParent)
    { }

    private HorizontalAlign _panelHrzAlign = HorizontalAlign.CENTERED;
    public HorizontalAlign panelHrzAlign { get { return _panelHrzAlign; } set { _panelHrzAlign = value; if (_enabled) _parentRenderer.redraw(); } }

    private VerticalAlign _panelVrtAlign = VerticalAlign.CENTERED;
    public VerticalAlign panelVrtAlign { get { return _panelVrtAlign; } set { _panelVrtAlign = value; if (_enabled) _parentRenderer.redraw(); } }

    private HorizontalPosition _horizontalPosition = HorizontalPosition.ABSOLUTEX;
    public HorizontalPosition horizontalPosition { get { return _horizontalPosition; } set { _horizontalPosition = value; if (_enabled) _parentRenderer.redraw(); } }

    private VerticalPosition _verticalPosition = VerticalPosition.ABSOLUTEY;
    public VerticalPosition verticalPosition { get { return _verticalPosition; } set { _verticalPosition = value; if (_enabled) _parentRenderer.redraw(); } }

    private Double _AbsoluteXposition = 0;
    public Double AbsoluteXposition { get { return _AbsoluteXposition; } set { _AbsoluteXposition = value; if (_enabled) _parentRenderer.redraw(); } }

    private Double _AbsoluteYposition = 0;
    public Double AbsoluteYposition { get { return _AbsoluteYposition; } set { _AbsoluteYposition = value; if (_enabled) _parentRenderer.redraw(); } }

    private int _YScaleIndex = 0;
    public int yScaleIndex { get { return _YScaleIndex; } set { _YScaleIndex = value; if (_enabled) _parentRenderer.redraw(); } }



  }



  public class YGraph : YDataRenderer
  {

    XAxis _xAxis;
    List<YAxis> _yAxes;
    List<DataSerie> _series;

    public static bool VerticalDragZoomEnabled = false;

    int lastPointCount = -1;
    int lastTopMargin = -1;
    int lastBottomMargin = -1;

    Bitmap navigatorCache;


    private LegendPanel _legendPanel;
    public LegendPanel legendPanel { get { return _legendPanel; } }

    private DataTracker _dataTracker;
    public DataTracker dataTracker { get { return _dataTracker; } }

    private Pen borderPen = null;

    private Color _borderColor = Color.LightGray;
    public Color borderColor { get { return _borderColor; } set { _borderColor = value; borderPen = null; redraw(); } }

    private Double _borderThickness = 1.0;
    public Double borderThickness { get { return _borderThickness; } set { _borderThickness = value; borderPen = null; redraw(); } }





ViewPortSettings mainViewPort = new ViewPortSettings() { IRLx = 0, IRLy = 0, zoomx = 1.0, zoomy = 1.0, Lmargin = 0, Rmargin = 0, Tmargin = 0, Bmargin = 0, Capture = false };


    protected List<DataPanel> _dataPanels;
    public List<DataPanel> dataPanels { get { return _dataPanels; } }

    public DataPanel addDataPanel()
    {
      DataPanel p = new DataPanel(this, this);
      _dataPanels.Add(p);
      return p;

    }


    public YGraph(PictureBox ChartContainer, logFct logFunction) : base(ChartContainer, logFunction)
    {

      _xAxis = new XAxis(this, this);
      _yAxes = new List<YAxis>();
      _series = new List<DataSerie>();
      _dataPanels = new List<DataPanel>();


      _navigator = new Navigator(this, this);
      _legendPanel = new LegendPanel(this, this);
      _dataTracker = new DataTracker(this, this);

      this.UIContainer.MouseDown += MouseDown;
      this.UIContainer.MouseMove += MouseMove;

      parentForm.MouseWheel += new System.Windows.Forms.MouseEventHandler(mouseWheelEvent);
      parentForm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown);


      _timeRange = MinMaxHandler.DefaultValue();
      int originalContainerWidth = ChartContainer.Width;
      int originalContainerHeight = ChartContainer.Height;
      int originalFormWidth = parentForm.Width;
      int originalFormHeight = parentForm.Height;


      AllowRedraw();
      Draw();

    }

    private MinMaxHandler.MinMax _timeRange;
    public void adjustGlobalTimeRange(double x)
    {
      double max = _timeRange.Max;
      _timeRange = MinMaxHandler.Combine(_timeRange, x);
      if (Double.IsNaN(max)) return;
      double ofset = x - max;
      if (ofset > 0)
      {
        switch (xAxis.overflowHandling)
        {
          case XAxis.OverflowHandling.SCROLL:
            if (max > xAxis.min + ((xAxis.max - xAxis.min) * 0.85) && (max <= xAxis.max))
            {
              DisableRedraw();
              xAxis.set_minMax(xAxis.min + ofset, xAxis.max + ofset);
              AllowRedraw();
            }
            break;
          case XAxis.OverflowHandling.CONTRACT:
            if (max > xAxis.min + ((xAxis.max - xAxis.min) * 0.95) && (max <= xAxis.max))
            {
              DisableRedraw();
              xAxis.max += ofset;
              AllowRedraw();
            }
            break;
        }
      }
    }

    private LinearGradientBrush _bgBrush = null;



    public Navigator _navigator;


    private Color _bgColor1 = Color.FromArgb(255, 200, 200, 200);
    public Color bgColor1 { get { return _bgColor1; } set { _bgColor1 = value; _bgBrush = null; redraw(); } }

    private Color _bgColor2 = Color.FromArgb(255, 255, 255, 255);
    public Color bgColor2 { get { return _bgColor2; } set { _bgColor2 = value; _bgBrush = null; redraw(); } }

    public XAxis xAxis { get { return _xAxis; } }

    public Navigator navigator { get { return _navigator; } }

    public List<YAxis> yAxes { get { return _yAxes; } }

    public List<DataSerie> series { get { return _series; } }

    public YAxis addYAxis()
    {
      YAxis s = new YAxis(this, this, _yAxes.Count);
      _yAxes.Add(s);
      redraw();
      return s;

    }




    public DataSerie addSerie()
    {
      DataSerie s = new DataSerie(this);
      _series.Add(s);
      redraw();
      return s;

    }





    public override void clearCachedObjects()
    {

      _bgBrush = null;
      navigatorCache = null;


    }

    private void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left) return;
      if ((e.X >= mainViewPort.Lmargin)
          && (e.X <= mainViewPort.Width - mainViewPort.Rmargin)
          && (e.Y >= mainViewPort.Lmargin)
          && (e.Y <= mainViewPort.Height - mainViewPort.Bmargin))
      {
        mainViewPort.OriginalXAxisMin = xAxis.min;
        mainViewPort.OriginalXAxisMax = xAxis.max;
        mainViewPort.OriginalIRLx = mainViewPort.IRLx;
        mainViewPort.OriginalLmargin = mainViewPort.Lmargin;
        mainViewPort.OriginalZoomx = mainViewPort.zoomx;
        pointXY p = ViewPortPointToIRL(mainViewPort, new Point(e.X, e.Y));
        mainViewPort.CaptureStartY = e.Y;
        mainViewPort.IRLCaptureStartX = p.x;
        mainViewPort.Capture = true;



      }
      else
       if ((e.X >= _navigator.viewport.Lmargin)
          && (e.X <= _navigator.viewport.Width - _navigator.viewport.Rmargin)
          && (e.Y >= _navigator.viewport.Lmargin)
          && (e.Y <= _navigator.viewport.Height - _navigator.viewport.Bmargin))
      {
        pointXY p = ViewPortPointToIRL(_navigator.viewport, new Point(e.X, e.Y));
        pointXY p2 = ViewPortPointToIRL(mainViewPort, new Point(mainViewPort.Lmargin, 0));
        pointXY p3 = ViewPortPointToIRL(mainViewPort, new Point(mainViewPort.Width - mainViewPort.Rmargin, 0));

        if ((p.x >= p2.x) && (p.x <= p3.x))
        {

          _navigator.startCapture(p, _xAxis.min, _xAxis.max);
          //  log("monitor start monitor capture  at " + _navigator.viewport.IRLCaptureStartX.ToString());
        }
        else
        {
          DisableRedraw();
          double min = p.x - (p3.x - p2.x) / 2;
          double max = min + (p3.x - p2.x);
          _xAxis.set_minMax(min, max);

          AllowRedraw();
          //  log("Jump to " + mainViewPort.IRLx.ToString());
          Draw();
        }
      }
    }

    private void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
      {
        mainViewPort.Capture = false;
        _navigator.stopCapture();

        if (_dataTracker.enabled) redraw();

        return;
      }


      if (mainViewPort.Capture)
      {
        double x1 = mainViewPort.OriginalIRLx + (double)(e.X - mainViewPort.OriginalLmargin) / mainViewPort.OriginalZoomx;

        double deltaX = (x1 - mainViewPort.IRLCaptureStartX);
        double deltaY = (e.Y - mainViewPort.CaptureStartY);
        DisableRedraw();
        double halfAxisDelta = (mainViewPort.OriginalXAxisMax - mainViewPort.OriginalXAxisMin) / 2;
        double Axismiddle = (mainViewPort.OriginalXAxisMax + mainViewPort.OriginalXAxisMin) / 2;
        double deltaCoef = (YGraph.VerticalDragZoomEnabled && (Math.Abs(deltaY) > 10)) ? Math.Pow(1.01, deltaY) : 1;


        _xAxis.set_minMax(Axismiddle - halfAxisDelta * deltaCoef - deltaX,
                           Axismiddle + halfAxisDelta * deltaCoef - deltaX);
        AllowRedraw();
        redraw();
        return;
      }
      if (_navigator.viewport.Capture)
      {
        double x1 = _navigator.viewport.OriginalIRLx + (double)(e.X - _navigator.viewport.OriginalLmargin) / _navigator.viewport.OriginalZoomx;
        double delta = (x1 - _navigator.viewport.IRLCaptureStartX);
        DisableRedraw();
        _xAxis.set_minMax(_navigator.viewport.OriginalXAxisMin + delta,
                          _navigator.viewport.OriginalXAxisMax + delta);
        AllowRedraw();
        redraw();
        return;

      }






    }

    public void cross(Point p)
    {
      Graphics g = Graphics.FromImage(UIContainer.Image);
      Pen mypen = new Pen(Brushes.Red);
      g.DrawLine(mypen, p.X - 10, p.Y, p.X + 10, p.Y);
      g.DrawLine(mypen, p.X, p.Y - 10, p.X, p.Y + 10);
      g.Dispose();
      UIContainer.Invalidate();
    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private Point IRLPointToViewPort(ViewPortSettings viewport, pointXY p)
    {
      int xx = viewport.Lmargin + (int)Math.Round((p.x - viewport.IRLx) * viewport.zoomx);
      int yy = viewport.Height - viewport.Bmargin - (int)Math.Round((p.y - viewport.IRLy) * viewport.zoomy);
      return new Point() { X = xx, Y = yy };

    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private Point IRLPointToViewPort(ViewPortSettings viewport, pointXY p, double IRLy, double zoomy)
    {
      int xx = viewport.Lmargin + (int)Math.Round((p.x - viewport.IRLx) * viewport.zoomx);
      int yy = viewport.Height - viewport.Bmargin - (int)Math.Round((p.y - IRLy) * zoomy);
      return new Point() { X = xx, Y = yy };

    }


#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private pointXY ViewPortPointToIRL(ViewPortSettings viewport, Point p)
    {
      return new pointXY()
      {
        x = viewport.IRLx + (double)(p.X - viewport.Lmargin) / viewport.zoomx,
        y = viewport.IRLy + (double)(+viewport.Height - p.Y - viewport.Bmargin) / viewport.zoomy


      };
    }

#if (!NET35 && !NET40)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private pointXY ViewPortPointToIRL(ViewPortSettings viewport, Point p, Double IRLy, Double zoomy)
    {
      return new pointXY()
      {
        x = viewport.IRLx + (double)(p.X - viewport.Lmargin) / viewport.zoomx,
        y = IRLy + (double)(+viewport.Height - p.Y - viewport.Bmargin) / zoomy


      };
    }


    private MinMaxHandler.MinMax FindMinMax(Double start, Double end, pointXY[] data, int count)
    {
      MinMaxHandler.MinMax res = MinMaxHandler.DefaultValue();


      // Do we need to consider that segment?
      if (!(data[0].x < end) && (data[count - 1].x > start)) return res; // completely out of view port full zone, abort.

      int N1, N2;
      // find out the first visible point ;
      int First = 0;
      if (data[0].x < start)
      {
        N1 = 0; N2 = count - 1;
        while (N2 - N1 > 1)
        {
          int N = (N1 + N2) >> 1;
          if (data[N].x > start) N2 = N; else N1 = N;
        }
        First = N1 - 1; if (First < 0) First = 0;
      }
      // data clipping: find out the last visible point;
      int Last = count - 1;
      if (data[Last].x > end)
      {
        N1 = 0; N2 = count - 1;
        while (N2 - N1 > 1)
        {
          int N = (N1 + N2) >> 1;
          if (data[N].x < end) N1 = N; else N2 = N;
        }
        Last = N2 + 1; if (Last > count - 1) Last = count - 1;
      }
      res.Min = data[First].y;
      res.Max = data[First].y;
      for (int i = First + 1; i <= Last; i++)
      {
        if (data[i].y < res.Min) res.Min = data[i].y;
        if (data[i].y > res.Max) res.Max = data[i].y;
      }


      return res;
    }


    public override void resetlegendPens()
    {
      for (int i = 0; i < _series.Count; i++)
        _series[i].resetlegendPen();
    }


    public void drawLegendPanel(YGraphics g, int viewPortWidth, int viewPortHeight, ref ViewPortSettings mainViewPort)
    {
      if (!_legendPanel.enabled) return;
      double[] legendWidths = new double[_series.Count];
      double[] legendHeight = new double[_series.Count];
      double[] ofsetx = new double[_series.Count];
      double[] ofsety = new double[_series.Count];

      string[] legends = new string[_series.Count];
      double totalHeight = 0;
      double totalWidth = 0;
      double maxWidth = 0;
      double maxHeight = 0;

      g.SetClip(new Rectangle(0, 0, viewPortWidth, viewPortHeight));

      for (int i = 0; i < _series.Count; i++)
        if (_series[i].legend != "") legends[i] = _series[i].legend; else legends[i] = "Series " + (i + 1).ToString();


      if ((_legendPanel.position == LegendPanel.Position.TOP) || (_legendPanel.position == LegendPanel.Position.BOTTOM))
      {
        double availableWidth = viewPortWidth - 2 * _legendPanel.padding + _legendPanel.borderthickness;
        if (_legendPanel.overlap) availableWidth = availableWidth - mainViewPort.Lmargin - mainViewPort.Rmargin;

        totalHeight = 0; // 2 * _legendPanel.padding + legendPanel.borderthickness;
        double xx = 0;
        double yy = 0;



        for (int i = 0; i < _series.Count; i++)

          if ((_series[i].segments.Count > 0) && (_series[i].visible) && (!_series[i].disabled))
          {
            SizeF ssize = g.MeasureString(legends[i], _legendPanel.font.fontObject, 100000);
            legendHeight[i] = (ssize.Height) + 1;
            double ww = (ssize.Width + 20);
            if (xx == 0) totalHeight += ssize.Height;
            if (availableWidth - xx < ww)
            {
              if (xx == 0)
              {
                ofsetx[i] = xx;
                ofsety[i] = yy;

                yy += ssize.Height;
                if (maxWidth < ww) maxWidth = ww;
              }
              else
              {
                yy += ssize.Height;
                ofsetx[i] = 00;
                ofsety[i] = yy;
                xx = ww;
                totalHeight += ssize.Height;
                if (maxWidth < xx) maxWidth = xx;
              }
            }
            else
            {
              ofsetx[i] = xx;
              ofsety[i] = yy;
              xx += ww;
              if (maxWidth < xx) maxWidth = xx;
            }
          }
        if (totalWidth > availableWidth) totalWidth = availableWidth;
      }
      else
      {
        double ty = 0;
        for (int i = 0; i < _series.Count; i++)

          if ((_series[i].segments.Count > 0) && (_series[i].visible) && (!_series[i].disabled))
          {
            SizeF ssize = g.MeasureString(legends[i], _legendPanel.font.fontObject, 100000);
            legendWidths[i] = (ssize.Width) + 1; if (maxWidth < legendWidths[i] + 20) maxWidth = legendWidths[i] + 20;
            legendHeight[i] = (ssize.Height) + 1; if (maxHeight < legendHeight[i]) maxHeight = legendHeight[i];
            ofsetx[i] = 0;
            ofsety[i] = ty;
            ty += ssize.Height;
            totalHeight += ssize.Height;
          }

      }


      double w = maxWidth + 2 * _legendPanel.padding + _legendPanel.borderthickness;
      double h = totalHeight + 2 * _legendPanel.padding + _legendPanel.borderthickness;
      double x = 0;
      double y = 0;

      switch (_legendPanel.position)
      {
        case LegendPanel.Position.LEFT:
          x = legendPanel.horizontalMargin;
          if (!_legendPanel.overlap)
          {
            mainViewPort.Lmargin += (int)(w + 2 * legendPanel.horizontalMargin + legendPanel.borderthickness);
            y = (viewPortHeight - h) / 2;
          }
          else
          {
            x += mainViewPort.Lmargin;
            y = mainViewPort.Tmargin + (viewPortHeight - mainViewPort.Tmargin - mainViewPort.Bmargin - h) / 2;
          }
          break;
        case LegendPanel.Position.TOPLEFT:
          x = legendPanel.horizontalMargin;
          y = legendPanel.verticalMargin;
          if (!_legendPanel.overlap) mainViewPort.Lmargin += (int)(w + 2 * legendPanel.horizontalMargin + legendPanel.borderthickness);
          else
          {
            x += mainViewPort.Lmargin;
            y += mainViewPort.Tmargin;
          }
          break;
        case LegendPanel.Position.TOP:
          if (!_legendPanel.overlap)
          {
            x = (viewPortWidth - w) / 2 - legendPanel.horizontalMargin - legendPanel.borderthickness;
            y = legendPanel.verticalMargin + legendPanel.borderthickness;
            mainViewPort.Tmargin += (int)(totalHeight + legendPanel.verticalMargin + 2 * legendPanel.verticalMargin + legendPanel.borderthickness);
          }
          else
          {
            x = mainViewPort.Lmargin + (viewPortWidth - mainViewPort.Lmargin - mainViewPort.Rmargin - w) / 2 - legendPanel.horizontalMargin - legendPanel.borderthickness;
            y = mainViewPort.Tmargin + legendPanel.verticalMargin - legendPanel.borderthickness;
          }
          break;

        case LegendPanel.Position.TOPRIGHT:
          x = viewPortWidth - legendPanel.horizontalMargin - w;
          y = legendPanel.verticalMargin;
          if (!_legendPanel.overlap) mainViewPort.Rmargin += (int)(w + 2 * legendPanel.horizontalMargin + legendPanel.borderthickness);
          else
          {
            x -= mainViewPort.Rmargin;
            y += mainViewPort.Tmargin;
          }

          break;
        case LegendPanel.Position.RIGHT:

          x = viewPortWidth - legendPanel.horizontalMargin - w;
          if (!_legendPanel.overlap)
          {
            mainViewPort.Rmargin += (int)(w + 2 * legendPanel.horizontalMargin + legendPanel.borderthickness);
            y = (viewPortHeight - h) / 2;
          }
          else
          {
            x -= mainViewPort.Rmargin;
            y = mainViewPort.Tmargin + (viewPortHeight - mainViewPort.Tmargin - mainViewPort.Bmargin - h) / 2;
          }


          break;
        case LegendPanel.Position.BOTTOMRIGHT:
          x = viewPortWidth - legendPanel.horizontalMargin - w;

          if (!_legendPanel.overlap)
          {
            mainViewPort.Rmargin += (int)(w + 2 * legendPanel.horizontalMargin + legendPanel.borderthickness);
            y = viewPortHeight - legendPanel.verticalMargin - h;
          }

          else
          {
            x -= mainViewPort.Rmargin;
            y = viewPortHeight - mainViewPort.Bmargin - h - legendPanel.verticalMargin;
          }
          break;
        case LegendPanel.Position.BOTTOM:
          if (!_legendPanel.overlap)
          {
            x = (viewPortWidth - w) / 2 - legendPanel.horizontalMargin - legendPanel.borderthickness;
            y = viewPortHeight - legendPanel.verticalMargin - 2 * legendPanel.padding - legendPanel.borderthickness - totalHeight;
            mainViewPort.Bmargin += (int)(totalHeight + 2 * legendPanel.padding + 2 * legendPanel.verticalMargin + legendPanel.borderthickness);
          }
          else
          {
            x = mainViewPort.Lmargin + (viewPortWidth - mainViewPort.Lmargin - mainViewPort.Rmargin - w) / 2 - legendPanel.horizontalMargin - legendPanel.borderthickness;
            y = viewPortHeight - mainViewPort.Bmargin - totalHeight - 2 * legendPanel.padding - 2 * legendPanel.verticalMargin - legendPanel.borderthickness;
          }
          break;


        case LegendPanel.Position.BOTTOMLEFT:
          x = legendPanel.horizontalMargin;
          y = legendPanel.verticalMargin;
          if (!_legendPanel.overlap)
          {
            mainViewPort.Lmargin += (int)(w + 2 * legendPanel.horizontalMargin + legendPanel.borderthickness);
            y = viewPortHeight - legendPanel.verticalMargin - h;
          }
          else
          {
            x += mainViewPort.Lmargin;
            y = viewPortHeight - mainViewPort.Bmargin - h - legendPanel.verticalMargin;
          }
          break;
      }






      Rectangle r = new Rectangle((int)x, (int)y, (int)w, (int)h);
      g.FillRectangle(_legendPanel.bgBrush, r);
      g.DrawRectangle(_legendPanel.pen, r);
      
      for (int i = 0; i < _series.Count; i++)
        if ((_series[i].segments.Count > 0) && (_series[i].visible) && (!_series[i].disabled))
        { 
          g.DrawString(legends[i], _legendPanel.font.fontObject, _legendPanel.font.brushObject,
             (int)(x + ofsetx[i] + 20 + legendPanel.padding),
             (int)(y + ofsety[i] + legendPanel.padding));
          int px = (int)(x + ofsetx[i] + _legendPanel.borderthickness / 2 + legendPanel.padding + 6);
          int py = (int)(y + ofsety[i] + legendPanel.padding + legendHeight[i] / 2);
          g.DrawLine(_series[i].legendPen , new Point(px, py), new Point(px + 12, py));

        }
    }








    private int DoSegmentRendering(ViewPortSettings w, YGraphics g, Pen p, pointXY[] data, int count)
    {
      bool isSVG = g is YGraphicsSVG;
      pointXY Bottomleft = ViewPortPointToIRL(w, new Point(w.Lmargin, w.Height - w.Bmargin));
      pointXY TopRight = ViewPortPointToIRL(w, new Point(w.Width - w.Rmargin, w.Tmargin));
      int N1, N2;
      // Do we need to draw that segment?
      if (!(data[0].x < TopRight.x) && (data[count - 1].x > Bottomleft.x)) return 0; // completely out of view port display zone, abort.

      // data clipping: find out the first point to draw;
      int First = 0;
      if (data[0].x < Bottomleft.x)
      {
        N1 = 0; N2 = count - 1;
        while (N2 - N1 > 1)
        {
          int N = (N1 + N2) >> 1;
          if (data[N].x > Bottomleft.x) N2 = N; else N1 = N;
        }
        First = N1 - 1; if (First < 0) First = 0;
      }
      // data clipping: find out the last point to draw;
      int Last = count - 1;
      if (data[Last].x > TopRight.x)
      {
        N1 = 0; N2 = count - 1;
        while (N2 - N1 > 1)
        {
          int N = (N1 + N2) >> 1;
          if (data[N].x < TopRight.x) N1 = N; else N2 = N;
        }
        Last = N2 + 1; if (Last > count - 1) Last = count - 1;
      }

      if (Last - First > 2 * w.Width - w.Lmargin - w.Rmargin)  // to many points to Draw, lets do some clean up
      {
        Point[] ToDraw = new Point[3 * (Last - First + 1)];
        Point Current = IRLPointToViewPort(w, data[First]);
        Point New;
        int i = First + 1;
        int n = 0;
        while (i < Last)
        {
          int min = Current.Y;
          int max = Current.Y;
          ToDraw[n++] = Current;
          do
          {
            New = IRLPointToViewPort(w, data[i]);
            if (New.Y > max) max = New.Y;
            if (New.Y < min) min = New.Y;
            i++;
          } while ((i < Last) && (Current.X == New.X));

          ToDraw[n++] = new Point(Current.X, min);
          ToDraw[n++] = new Point(Current.X, max);
          Current = New;
        }

        ToDraw[n++] = IRLPointToViewPort(w, data[Last]);
        Array.Resize(ref ToDraw, n);
        if (n > 1) g.DrawLines(p, ToDraw);
        return n;

      }
      else
      { // in SVG mode, DrawLines linejoins are rendered correctly,
        // in bitmap mode they aren't
        if (isSVG)
        {
          Point[] ToDraw = new Point[Last - First + 1];
          for (int i = First; i <= Last; i++) ToDraw[i - First] = IRLPointToViewPort(w, data[i]);
          g.DrawLines(p, ToDraw);
        }
        else
          for (int i = First; i < Last; i++)
          {
            g.DrawLine(p, IRLPointToViewPort(w, data[i]), IRLPointToViewPort(w, data[i + 1]));
          }
      }
      return Last - First;

    }


    private void DrawYAxisZones(ViewPortSettings w, YGraphics g, YAxis scale)
    {
      if (!scale.visible) return;

      Double Delta = scale.startStopStep.dataMax - scale.startStopStep.dataMin;
      Double YZoom = (Delta) / (w.Height - w.Bmargin - w.Tmargin);
      for (int i = 0; i < scale.zones.Count; i++)
        if (scale.zones[i].visible)
        {
          double max = scale.zones[i].max;
          double min = scale.zones[i].min;
          if (double.IsNaN(max))
            max = scale.startStopStep.dataMax;
          if (double.IsNaN(min)) min = scale.startStopStep.dataMin;


          if (max < min) { double t = max; max = min; min = t; }



          int y0 = w.Height - w.Bmargin - (int)Math.Round((max - scale.startStopStep.dataMin) / YZoom);
          int h = (int)Math.Round((max - min) / YZoom);
          g.FillRectangle(scale.zones[i].zoneBrush, mainViewPort.Lmargin, y0, mainViewPort.Width - mainViewPort.Rmargin - mainViewPort.Lmargin + 1, h);



        }

    }

    private void DrawXAxisZones(ViewPortSettings w, YGraphics g, XAxis scale)
    {
      if (!scale.visible) return;
      double delta = scale.max - scale.min;
      Double XZoom = (delta) / (w.Width - w.Lmargin - w.Rmargin);

      for (int i = 0; i < scale.zones.Count; i++)
        if (scale.zones[i].visible)
        {
          double max = scale.zones[i].max;
          double min = scale.zones[i].min;
          if (double.IsNaN(max)) max = scale.min;
          if (double.IsNaN(min)) min = scale.max;
          if (max < min) { double t = max; max = min; min = t; }
          int x0 = w.Lmargin + (int)Math.Round((min - scale.min) / XZoom);
          g.FillRectangle(scale.zones[i].zoneBrush,
          x0, mainViewPort.Tmargin,
          (int)((max - min) / XZoom),
           mainViewPort.Height - mainViewPort.Tmargin - mainViewPort.Bmargin);
        }
    }


    private int DrawYAxis(ViewPortSettings w, YGraphics g, YAxis axis, int ofset, bool simulation)
    {
      if (!axis.visible)
      {
        axis.innerWidth = 0;
        return axis.innerWidth;




      }

      Double Delta = axis.startStopStep.dataMax - axis.startStopStep.dataMin;

      Double YZoom = (Delta) / (w.Height - w.Bmargin - w.Tmargin);
      bool leftSide = axis.position == YAxis.HrzPosition.LEFT;

      int x = leftSide ? w.Lmargin - ofset : (w.Width - w.Rmargin + ofset);

      if (!simulation) g.DrawLine(axis.pen, x, w.Tmargin, x, w.Height - w.Bmargin);

      double FirstStep = axis.startStopStep.step * (Math.Truncate(axis.startStopStep.start / axis.startStopStep.step));

      if (FirstStep < 0) { FirstStep -= axis.startStopStep.step; }

      int stepCount = (int)((Delta - (FirstStep - axis.startStopStep.dataMin)) / axis.startStopStep.step) + 1;



      if (!simulation) g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      double UnitWidth = 0;

      string labelPrecision = "F0";
      if (axis.startStopStep.precision < 0) labelPrecision = "F" + (-axis.startStopStep.precision).ToString();

      if (stepCount < w.Height) // protection again infinity of graduation
        for (int i = 0; i < stepCount; i++)
        {
          int y = (int)Math.Round((FirstStep + i * axis.startStopStep.step - axis.startStopStep.dataMin) / YZoom);
          if (y >= 0)
          {

            y = w.Height - w.Bmargin - y;
            double v = FirstStep + i * axis.startStopStep.step;
            


            if (!simulation)
            {
              if ((axis.showGrid) && ((i > 0) || (axis.startStopStep.dataMin != 0))) g.DrawLine(axis.gridPen, w.Lmargin, y, w.Width - w.Rmargin, y);

              if ((Math.Abs(v) <1E-6) && axis.highlightZero)
              {
               
                  g.DrawLine(axis.pen, w.Lmargin, y, w.Width - w.Rmargin, y);
                

              }
               g.DrawLine(axis.pen, x + ((leftSide) ? -2 : 2), y, x + ((leftSide) ? 5 : -5), y);
            }


            String label = (v).ToString(labelPrecision);
            SizeF ssize = g.MeasureString(label, axis.font.fontObject, 100000);
            if (ssize.Width > UnitWidth) UnitWidth = ssize.Width;
            if (!simulation) g.DrawString(label, axis.font.fontObject, axis.font.brushObject, new Point(x + ((leftSide) ? -(int)ssize.Width : 2), y - (int)(ssize.Height / 2)));
          }

        }

      if (axis.legend.title != "")
      {
        SizeF size = g.MeasureString(axis.legend.title, axis.legend.font.fontObject, 100000);

        if (!simulation)
        {
          StringFormat format = new StringFormat();
          format.Alignment = StringAlignment.Center;
          format.Trimming = StringTrimming.None;
          int legendX = x + (int)((leftSide) ? -UnitWidth - size.Height : UnitWidth + size.Height + 2);
          int legendY = (int)(w.Tmargin + (w.Height - w.Tmargin - w.Bmargin) / 2);
          g.Transform(legendX, legendY, leftSide ? -90 : 90);

          g.DrawString(axis.legend.title, axis.legend.font.fontObject, axis.legend.font.brushObject, new Point(0, 0), format);
          g.ResetTransform();
        }
        UnitWidth += size.Height;
      }

      axis.innerWidth = (int)UnitWidth + 10;
      return axis.innerWidth;

    }

    private void DrawMonitorXAxis(ViewPortSettings w, YGraphics g, MinMaxHandler.MinMax xRange, string format)
    {


      double delta = xRange.Max - xRange.Min;
      TimeResolution scale = TimeConverter.BestTimeformat(delta, delta, xAxis.timeReference);
      Double XZoom = (delta) / (w.Width - w.Lmargin - w.Rmargin);



      int stepCount = (int)(delta / scale.step) + 2;
      double FirstStep = scale.step * (Math.Truncate(xRange.Min / scale.step));
      if (FirstStep < xRange.Min) FirstStep += scale.step;
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      int y = w.Height - w.Bmargin;

      g.DrawLine(_navigator.pen, w.Lmargin, w.Height - w.Bmargin - 1, w.Width - w.Rmargin, w.Height - w.Bmargin - 1);
      String label;
      double t = FirstStep;
      do
      {

        DateTime d = TimeConverter.FromUnixTime(t);
        if (scale.step > 30 * 86400)  // resynchronize with the beginning of the month.
          t = TimeConverter.ToUnixTime(new DateTime(d.Year, d.Month, 1));
        if (t >= xRange.Min)
        {
          int x = w.Lmargin + (int)Math.Round((t - xRange.Min) / XZoom);
          g.DrawLine(_navigator.pen, x, y, x, y - 4);
          if (format == XAxis.FORMATAUTO)
            label = TimeConverter.FromUnixTime(t).ToLocalTime().ToString(scale.format);
          else label = t.ToString(format);

          SizeF ssize = g.MeasureString(label, _navigator.font.fontObject, 100000);
          g.DrawString(label, _navigator.font.fontObject, _navigator.font.brushObject, new Point((int)(x - ssize.Width / 2), (int)(y - ssize.Height - 2)));
        }
        t += scale.step;

      } while (t < xRange.Max);



    }

    private int DrawXAxis(ViewPortSettings w, YGraphics g, XAxis scale, bool simulation)
    {
      //string lastdate = "";
      StringFormat stringFormat = new StringFormat();
      stringFormat.Alignment = StringAlignment.Center;      // Horizontal Alignment

      bool bottomSide = scale.position == XAxis.VrtPosition.BOTTOM;

      int y = bottomSide ? w.Height - w.Bmargin : w.Tmargin;

      if (!simulation) g.DrawLine(scale.pen, w.Lmargin, y, w.Width - w.Rmargin, y);

      double delta = scale.max - scale.min;
      Double XZoom = (delta) / (w.Width - w.Lmargin - w.Rmargin);
      int stepCount = (int)(delta / scale.step) + 1;
      double FirstStep = 0;
      MinMaxHandler.MinMax timeRange = MinMaxHandler.DefaultValue();

      if (scale.timeReference == TimeConverter.TimeReference.ABSOLUTE)
      {
        FirstStep = scale.step * (Math.Truncate(scale.min / scale.step));
        timeRange.Min = scale.min; timeRange.Max = scale.max;

      }
      else
      {
        for (int i = 0; i < this._series.Count; i++)
          if (!_series[i].disabled)
            timeRange = MinMaxHandler.Combine(timeRange, this._series[i].timeRange);
        if (double.IsNaN(timeRange.Min)) return 0;
        FirstStep = timeRange.Min + scale.step * (Math.Truncate((scale.min - timeRange.Min) / scale.step));
      }
      if (FirstStep < scale.min) FirstStep += scale.step;

      //log("Viewport Size: " + (scale.max - scale.min).ToString() + "Sec (" + ((scale.max - scale.min)/86400).ToString()+" days)");

      TimeResolution scaleFormat = scale.bestFormat(timeRange.Max - timeRange.Min, scale.max - scale.min);

      if (!simulation) g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      double UnitHeight = 0;
      string label = "";

      scale.step = scaleFormat.step;


      double t = FirstStep;




      do
      {
        DateTime d = TimeConverter.FromUnixTime(t);
        if ((scale.step > 30 * 86400) && (scale.timeReference == TimeConverter.TimeReference.ABSOLUTE))  // resynchronize with the begining of the month.
          t = TimeConverter.ToUnixTime(new DateTime(d.Year, d.Month, 1));

        if (t >= scale.min)
        {

          int x = w.Lmargin + (int)Math.Round((t - scale.min) / XZoom);
          if (x <= w.Width - w.Rmargin)
          {
            if (!simulation)
            {
              if (scale.showGrid) g.DrawLine(scale.gridPen, x, w.Tmargin, x, w.Height - w.Bmargin);
              g.DrawLine(scale.pen, x, y + (bottomSide ? 2 : -2), x, y + (bottomSide ? -5 : 5));
            }

            if (scale.timeReference == TimeConverter.TimeReference.ABSOLUTE)
            {
              if (scale.labelFormat == XAxis.FORMATAUTO)
                label = TimeConverter.FromUnixTime(t).ToLocalTime().ToString(scaleFormat.format);
              else label = t.ToString(scale.labelFormat);

            }
            else
            {
              Int64 ticks = (Int64)((double)TimeSpan.TicksPerSecond * (t - timeRange.Min));


#if (!NET35 && !NET40)
              label = (ticks < 0) ? "-" + new TimeSpan(-ticks).ToString(scaleFormat.format) : new TimeSpan(ticks).ToString(scaleFormat.format);
#else
             label = (ticks < 0) ? "-" + new TimeSpan(-ticks).ToString() : new TimeSpan(ticks).ToString();

#endif

            }


            SizeF ssize = g.MeasureString(label, scale.font.fontObject, 100000);
            if (ssize.Height > UnitHeight) UnitHeight = ssize.Height;

            if (!simulation) g.DrawString(label, scale.font.fontObject, scale.font.brushObject, new Point(x /*- (int)ssize.Width / 2*/, y + ((bottomSide) ? +2 : -(int)ssize.Height)), stringFormat);
          }
        }
        t += scale.step;
      } while (t <= scale.max);







      if (scale.legend.title != "")
      {

        SizeF size = g.MeasureString(scale.legend.title, scale.legend.font.fontObject, 100000);
        if (!simulation)
        {
          int legendX = (int)(w.Lmargin + (w.Width - w.Lmargin - w.Rmargin - size.Width) / 2);
          int legendY = (int)(bottomSide ? w.Height - w.Bmargin + UnitHeight + 5 : w.Tmargin - UnitHeight - size.Height);
          g.DrawString(scale.legend.title, scale.legend.font.fontObject, scale.legend.font.brushObject, new Point(legendX, legendY));
        }
        UnitHeight += (int)size.Height;


      }

      scale.innerHeight = (int)UnitHeight + 10;
      return scale.innerHeight;

    }

    private void DrawDataTracker(YGraphics g, int viewPortWidth, int viewPortHeight)
    {
      if (!_dataTracker.enabled) { return; }

      Nullable<Point> p = mousePosition();
      if (p == null) { return; }

      if (p.Value.X <= mainViewPort.Lmargin) { return; }
      if (p.Value.Y <= mainViewPort.Tmargin) { return; }

      if (p.Value.X >= UIContainer.Width - mainViewPort.Rmargin) { return; }
      if (p.Value.Y >= UIContainer.Height - mainViewPort.Bmargin) { return; }

      g.SetClip(new Rectangle(0, 0, viewPortWidth, viewPortHeight));

      pointXY DataPoint = ViewPortPointToIRL(mainViewPort, p.Value);  //DataPoint Y value will be incorrect, but we don't need it.


      double delta = -1;
      int bestindex = -1;
      Nullable<Point>[] bestmatch = new Nullable<Point>[_series.Count];
      Nullable<pointXY>[] IRLmatch = new Nullable<pointXY>[_series.Count];


      for (int i = 0; i < _series.Count; i++)
        if ((_series[i].visible) && (!_series[i].disabled))
        {

          IRLmatch[i] = _series[i].findClosestValue(DataPoint.x, false);
          if (IRLmatch[i] != null)
          {
            bestmatch[i] = IRLPointToViewPort(mainViewPort, IRLmatch[i].Value, yAxes[_series[i].yAxisIndex].IRLy, yAxes[_series[i].yAxisIndex].zoom);
            if (bestindex < 0 || (delta > Math.Abs(bestmatch[i].Value.Y - p.Value.Y)))
            {
              delta = Math.Abs(bestmatch[i].Value.Y - p.Value.Y);

              if ((_dataTracker.detectionDistance == 0) ||
                   ((delta <= _dataTracker.detectionDistance) &&
                    (Math.Abs(bestmatch[i].Value.X - p.Value.X) < _dataTracker.detectionDistance)))
                bestindex = i;
            }
          }
        }

      if (bestindex >= 0)
      {

        int xx = (int)(bestmatch[bestindex].Value.X - _dataTracker.diameter / 2);
        int yy = (int)(bestmatch[bestindex].Value.Y - _dataTracker.diameter / 2);
        int dd = (int)_dataTracker.diameter;

        g.FillEllipse(_series[bestindex].brush, xx, yy, dd, dd);

        g.DrawEllipse(_dataTracker.pen, xx, yy, dd, dd);

        double dx, dy;
        if (p.Value.X > mainViewPort.Lmargin + (viewPortWidth - mainViewPort.Lmargin - mainViewPort.Rmargin) / 2) dx = -1; else dx = 1;
        if (p.Value.Y > mainViewPort.Tmargin + (viewPortHeight - mainViewPort.Tmargin - mainViewPort.Bmargin) / 2) dy = -1; else dy = 1;

        int xx2 = (int)(bestmatch[bestindex].Value.X + dx * (_dataTracker.handleLength * 1.5));
        int yy2 = (int)(bestmatch[bestindex].Value.Y + dy * _dataTracker.handleLength);

        g.DrawLine(_dataTracker.pen, (int)(bestmatch[bestindex].Value.X + dx * 0.707 * _dataTracker.diameter / 2), (int)(bestmatch[bestindex].Value.Y + dy * 0.707 * _dataTracker.diameter / 2),
                                      (int)(bestmatch[bestindex].Value.X + dx * _dataTracker.handleLength), (int)(bestmatch[bestindex].Value.Y + dy * _dataTracker.handleLength));

        g.DrawLine(_dataTracker.pen, (int)(bestmatch[bestindex].Value.X + dx * _dataTracker.handleLength), (int)(bestmatch[bestindex].Value.Y + dy * _dataTracker.handleLength),
                                    xx2, yy2);


        string strValue = "";
        if (_dataTracker.showSerieName) strValue += _series[bestindex].legend + "\r\n";

        if (_dataTracker.dataPrecision == DataTracker.DataPrecision.PRECISION_NOLIMIT)
          strValue += IRLmatch[bestindex].Value.y.ToString() + _series[bestindex].unit;
        else
          strValue += IRLmatch[bestindex].Value.y.ToString(_dataTracker.dataPrecisionString) + _series[bestindex].unit;

        SizeF ssize = g.MeasureString(strValue, _dataTracker.font.fontObject, 10000);

        int labelwidth = (int)(ssize.Width + 2 * _dataTracker.padding + _dataTracker.borderthickness);
        int labelHeight = (int)(ssize.Height + 2 * _dataTracker.padding + _dataTracker.borderthickness);

        if (dx > 0)
        {
          g.FillRectangle(_dataTracker.bgBrush, xx2, yy2 - (labelHeight >> 1), labelwidth, labelHeight);
          g.DrawRectangle(_dataTracker.pen, xx2, yy2 - (labelHeight >> 1), labelwidth, labelHeight);
          g.DrawString(strValue, _dataTracker.font.fontObject, _dataTracker.font.brushObject, (int)(xx2 + _dataTracker.padding), (int)(yy2 - (labelHeight >> 1) + _dataTracker.padding));
        }
        else
        {
          g.FillRectangle(_dataTracker.bgBrush, xx2 - labelwidth, yy2 - (labelHeight >> 1), labelwidth, labelHeight);
          g.DrawRectangle(_dataTracker.pen, xx2 - labelwidth, yy2 - (labelHeight >> 1), labelwidth, labelHeight);
          g.DrawString(strValue, _dataTracker.font.fontObject, _dataTracker.font.brushObject, (int)(xx2 + _dataTracker.padding - labelwidth), (int)(yy2 - (labelHeight >> 1) + _dataTracker.padding));
        }

      }




    }


    /*
     *  XAxis scale)
    {
        if (!scale.visible) return;
        double delta = scale.max - scale.min;
        Double XZoom = (delta) / (w.Width - w.Lmargin - w.Rmargin);

        for (int i = 0; i < scale.zones.Count; i++)
            if (scale.zones[i].visible)
            {
                double max = scale.zones[i].max;
                double min = scale.zones[i].min;
                if (double.IsNaN(max)) max = scale.min;
                if (double.IsNaN(min)) min = scale.max;
                if (max < min) { double t = max; max = min; min = t; }
                int x0 =  w.Lmargin + (int)Math.Round((min - scale.min) / XZoom);
     *
     * */

    public void DrawDataPanels(ViewPortSettings w, YGraphics g, XAxis scaleX, List<YAxis> scalesY, int viewPortWidth, int viewPortHeight)
    {
      if (_dataPanels.Count == 0) return;


      g.SetClip(new Rectangle(w.Lmargin, w.Tmargin, w.Width - w.Rmargin - w.Lmargin, w.Height - w.Bmargin - w.Tmargin));
      for (int i = 0; i < _dataPanels.Count; i++)
        if (_dataPanels[i].enabled)
        {
          DataPanel p = _dataPanels[i];

          if (p.yScaleIndex < scalesY.Count)
          {

            double AvailableWidth = w.Width - 2 * p.padding - p.borderthickness;
            if (AvailableWidth < 100) AvailableWidth = 100;


            SizeF ssize = g.MeasureString(p.text, p.font.fontObject, (int)AvailableWidth);
            double panelWidth = ssize.Width + 2 * p.padding + p.borderthickness;
            double panelHeight = ssize.Height + 2 * p.padding + p.borderthickness;
            double x = 0;
            switch (p.horizontalPosition)
            {
              case DataPanel.HorizontalPosition.LEFTBORDER: x = w.Lmargin; break;
              case DataPanel.HorizontalPosition.RIGHTBORDER: x = w.Width - w.Rmargin; break;
              case DataPanel.HorizontalPosition.ABSOLUTEX:
                double delta = scaleX.max - scaleX.min;
                Double XZoom = (delta) / (w.Width - w.Lmargin - w.Rmargin);
                x = w.Lmargin + (int)Math.Round((p.AbsoluteXposition - scaleX.min) / XZoom);
                break;
            }


            double y = 0;
            switch (p.verticalPosition)
            {
              case DataPanel.VerticalPosition.TOPBORDER: y = w.Tmargin; break;
              case DataPanel.VerticalPosition.BOTTOMBORDER: y = w.Height - w.Bmargin; break;
              case DataPanel.VerticalPosition.ABSOLUTEY:
                y = w.Height - w.Bmargin - (int)Math.Round((p.AbsoluteYposition - scalesY[p.yScaleIndex].IRLy) * scalesY[p.yScaleIndex].zoom);
                break;
            }


            switch (p.panelHrzAlign)
            {
              case DataPanel.HorizontalAlign.LEFTOF: x -= panelWidth + p.horizontalMargin; break;
              case DataPanel.HorizontalAlign.RIGHTOF: x += p.horizontalMargin; break;
              default: x -= (panelWidth) / 2; break;
            }

            switch (p.panelVrtAlign)
            {
              case DataPanel.VerticalAlign.ABOVE: y -= panelHeight + p.verticalMargin; break;
              case DataPanel.VerticalAlign.BELOW: y += p.verticalMargin; break;
              default: y -= (panelHeight) / 2; break;
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
          else throw new InvalidOperationException("Cannot renderer data panel #" + i.ToString() + ", no such Y axis");
        }
    }




    protected override int Render(YGraphics g, int UIw, int UIh)
    {







      mainViewPort.Width = UIw;
      mainViewPort.Height = UIh;
      mainViewPort.Lmargin = 0;
      mainViewPort.Rmargin = 0;


      g.SmoothingMode = SmoothingMode.HighQuality;
      int yMarginOffset = 5;

      /* Step 1, found out margins */
      // top (bottom) margin: make sure the top(/bottom) number
      // on Y scale can be draw completely

      for (int i = 0; i < _yAxes.Count; i++)
        if (_yAxes[i].visible)
        {
          SizeF s = g.MeasureString("8", _yAxes[i].font.fontObject, 100000);
          int o = (int)((s.Height + 1) / 2);
          if (yMarginOffset < o) yMarginOffset = o;
        }


      mainViewPort.Tmargin = (_xAxis.position == XAxis.VrtPosition.TOP) ? 0 : yMarginOffset;
      mainViewPort.Bmargin = (_xAxis.position == XAxis.VrtPosition.BOTTOM) ? 0 : yMarginOffset;


      /* Step 2B-2  Draw Legend if it doesn't overlap the data */
      if (!_legendPanel.overlap) drawLegendPanel(g, UIw, UIh, ref mainViewPort);

      if (mainViewPort.Bmargin == 0) mainViewPort.Bmargin = 5;
      if (mainViewPort.Tmargin == 0) mainViewPort.Tmargin = 5;


      /* Step 1-A  compute margins dues to X axis  */
      int h = DrawXAxis(mainViewPort, g, _xAxis, true);
      if (_xAxis.position == XAxis.VrtPosition.TOP) mainViewPort.Tmargin += h; else mainViewPort.Bmargin += h;


      mainViewPort.IRLx = _xAxis.min;

      /* Step 1-B  Find out all Y axis  start / stop  graduation spacing  */
      MinMaxHandler.MinMax M;
      for (int i = 0; i < _yAxes.Count; i++)
      {
        M = MinMaxHandler.DefaultValue();
        for (int k = 0; k < _series.Count; k++)
          if ((_series[k].yAxisIndex == i) && (!_series[k].disabled))
          {

            for (int j = 0; j < _series[k].segments.Count; j++)
            {
              M = MinMaxHandler.Combine(M, FindMinMax(_xAxis.min, _xAxis.max, _series[k].segments[j].data, _series[k].segments[j].count));
            }

          }
        _yAxes[i].computeStartAndStep(M);
      }


      /* Step 1-B  compute  margins dues to Y axes  */

      if (mainViewPort.Lmargin == 0) mainViewPort.Lmargin = 5;
      if (mainViewPort.Rmargin == 0) mainViewPort.Rmargin = 5;





      for (int i = 0; i < _yAxes.Count; i++)
      {
        int sw = DrawYAxis(mainViewPort, g, _yAxes[i], 0, true);
        mainViewPort.Lmargin += (_yAxes[i].position == YAxis.HrzPosition.LEFT) ? sw : 0;
        mainViewPort.Rmargin += (_yAxes[i].position == YAxis.HrzPosition.RIGHT) ? sw : 0;
      }


      if (_navigator.enabled)
      {
        int nh = (int)(_navigator.relativeheight * UIContainer.Size.Height / 100.0);
        int ofset = xAxis.position == XAxis.VrtPosition.BOTTOM ? h : 0;
        _navigator.setPosition(UIw, UIh, mainViewPort.Lmargin, mainViewPort.Rmargin, mainViewPort.Height - nh - mainViewPort.Bmargin + ofset, mainViewPort.Bmargin - ofset);
        mainViewPort.Bmargin += nh;

      }



      /* step 2A draw background */

      if ((lastTopMargin != mainViewPort.Tmargin) || (lastBottomMargin != mainViewPort.Bmargin))
      {
        _bgBrush = null;
        lastTopMargin = mainViewPort.Tmargin;
        lastBottomMargin = mainViewPort.Bmargin;
      }

      if (_bgBrush == null)

        _bgBrush = new LinearGradientBrush(new Point(0, mainViewPort.Height - mainViewPort.Bmargin),
                                           new Point(0, mainViewPort.Tmargin), _bgColor1, _bgColor2);


      g.FillRectangle(_bgBrush,
                      mainViewPort.Lmargin,
                      mainViewPort.Tmargin,
                      mainViewPort.Width - mainViewPort.Rmargin - mainViewPort.Lmargin,
                      mainViewPort.Height - mainViewPort.Bmargin - mainViewPort.Tmargin
                      );


      if (_borderThickness > 0)
      {
        if (borderPen == null) borderPen = new Pen(_borderColor, (float)_borderThickness);
        g.DrawRectangle(borderPen, mainViewPort.Lmargin, mainViewPort.Tmargin, mainViewPort.Width - mainViewPort.Rmargin - mainViewPort.Lmargin, mainViewPort.Height - mainViewPort.Bmargin - mainViewPort.Tmargin);

      }





      /* Step 2B  Draw Y-axes and X axis zones */
      g.SetClip(new Rectangle(mainViewPort.Lmargin, mainViewPort.Tmargin, mainViewPort.Width - mainViewPort.Rmargin - mainViewPort.Lmargin, mainViewPort.Height - mainViewPort.Bmargin - mainViewPort.Tmargin));
      for (int i = 0; i < _yAxes.Count; i++)
        DrawYAxisZones(mainViewPort, g, _yAxes[i]);

      DrawXAxisZones(mainViewPort, g, xAxis);
      g.ResetClip();




      /* step 3 draw X scale */
      DrawXAxis(mainViewPort, g, _xAxis, false);

      /* step 4 draw Y scale */
      int leftOffset = 0;
      int rightOffset = 0;
      for (int i = 0; i < _yAxes.Count; i++)
      {
        int ww = DrawYAxis(mainViewPort, g, _yAxes[i], (_yAxes[i].position == YAxis.HrzPosition.LEFT) ? leftOffset : rightOffset, false);
        if (_yAxes[i].position == YAxis.HrzPosition.LEFT) leftOffset += ww;
        if (_yAxes[i].position == YAxis.HrzPosition.RIGHT) rightOffset += ww;

      }




      // step 5 step define data zone
      //  Pen mypenb = Pens.Black;
      //g.DrawRectangle(mypenb, ViewPort1.Lmargin, ViewPort1.Tmargin, ViewPort1.Width - ViewPort1.Rmargin - ViewPort1.Lmargin, ViewPort1.Height - ViewPort1.Bmargin - ViewPort1.Tmargin);
      g.SetClip(new Rectangle(mainViewPort.Lmargin, mainViewPort.Tmargin, mainViewPort.Width - mainViewPort.Rmargin - mainViewPort.Lmargin, mainViewPort.Height - mainViewPort.Bmargin - mainViewPort.Tmargin));

      // step 6 series rendering





      mainViewPort.zoomx = (mainViewPort.Width - mainViewPort.Lmargin - mainViewPort.Rmargin) / (_xAxis.max - _xAxis.min);



      Pen mypenb;
      int lineCount = 0;
      int pointCount = 0;
      for (int k = 0; k < _series.Count; k++)
        if ((_series[k].visible) && !(_series[k].disabled))
        {
          int scaleIndex = _series[k].yAxisIndex;
          mypenb = _series[k].pen;
          mainViewPort.IRLy = _yAxes[scaleIndex].startStopStep.dataMin;
          _yAxes[_series[k].yAxisIndex].IRLy = mainViewPort.IRLy;
          double delta = _yAxes[scaleIndex].startStopStep.dataMax - _yAxes[scaleIndex].startStopStep.dataMin;
          if (delta == 0) { delta = 1; mainViewPort.IRLy -= delta / 2; }

          mainViewPort.zoomy = (mainViewPort.Height - mainViewPort.Tmargin - mainViewPort.Bmargin) / (delta);
          _yAxes[_series[k].yAxisIndex].zoom = mainViewPort.zoomy;
          g.comment("** main view-port series " + k.ToString());

          for (int i = 0; i < _series[k].segments.Count; i++)
          {
            lineCount += DoSegmentRendering(mainViewPort, g, mypenb, _series[k].segments[i].data, _series[k].segments[i].count);
            pointCount += _series[k].segments[i].count;
          }
        }






      // step 7  draw  navigator
      if (_navigator.enabled)
      {
        g.comment("** navigator **");
        ViewPortSettings v = _navigator.viewport;

        // step 7A, find out Time Range
        MinMaxHandler.MinMax range = MinMaxHandler.DefaultValue();
        for (int i = 0; i < _series.Count; i++)
          if (!_series[i].disabled) range = MinMaxHandler.Combine(range, _series[i].timeRange);

        _navigator.Xrange = MinMaxHandler.extend(range, 1.05);


        v.zoomx = (v.Width - v.Lmargin - v.Rmargin) / (_navigator.Xrange.Max - _navigator.Xrange.Min);
        if ((lastPointCount != pointCount) || (navigatorCache == null) || (g is YGraphicsSVG))
        {
          g.comment("Redraw navigator");
          if (navigatorCache != null) navigatorCache.Dispose();
          navigatorCache = new Bitmap(v.Width, v.Height, g.graphics);
          lastPointCount = pointCount;

          YGraphics ng;
          if (g is YGraphicsSVG) ng = g; else ng = new YGraphics(navigatorCache);


          //ng.SetClip(new Rectangle(v.Lmargin, v.Tmargin, v.Width - v.Rmargin - v.Lmargin, v.Height - v.Bmargin - v.Tmargin));
          ng.ResetClip();
          ng.FillRectangle(_navigator.bgBrush, v.Lmargin, v.Tmargin, v.Width - v.Rmargin - v.Lmargin, v.Height - v.Bmargin - v.Tmargin);

          if ((xAxis.zones.Count > 0) && _navigator.showXAxisZones)
          {
            double delta = _navigator.Xrange.Max - _navigator.Xrange.Min;
            Double XZoom = (delta) / (v.Width - v.Lmargin - v.Rmargin);
            for (int i = 0; i < xAxis.zones.Count; i++)
              if (xAxis.zones[i].visible)
              {
                double min = xAxis.zones[i].min;
                double max = xAxis.zones[i].max;
                if (Double.IsNaN(min)) min = _navigator.Xrange.Min;
                if (Double.IsNaN(max)) max = _navigator.Xrange.Max;
                ng.FillRectangle(xAxis.zones[i].zoneBrush,
                             v.Lmargin + (int)((min - _navigator.Xrange.Min) / XZoom),
                             v.Tmargin,
                            (int)((max - min) / XZoom),
                             v.Height - v.Bmargin - v.Tmargin);


              }

          }



          if ((MinMaxHandler.isDefined(_navigator.Xrange)) && ((_navigator.Xrange.Max - _navigator.Xrange.Min) > 0))  // if (Xrange<=0) then nothing to draw
          { // step 7B, draw series
            double Min;
            double Max;
            v.IRLx = _navigator.Xrange.Min;
            double dontSticktoBorderZoom = 4.0 / (v.Height - v.Bmargin - v.Tmargin);

            if (_navigator.yAxisHandling == Navigator.YAxisHandling.AUTO)
            { // Automatic yAxis handling
              for (int k = 0; k < _series.Count; k++)
                if (!_series[k].disabled)
                {
                  ng.comment("** navigator series " + k.ToString());
                  v.IRLy = _series[k].valueRange.Min;
                  int yAxisIndex = _series[k].yAxisIndex;
                  mypenb = _series[k].navigatorpen;
                  Min = _series[k].valueRange.Min;
                  Max = _series[k].valueRange.Max;
                  if (Max - Min <= 0)
                  {
                    v.IRLy = Min - 0.5; Max = Min + 0.5;
                  }
                  else
                  {
                    double delta = Max - Min;
                    Min -= delta * dontSticktoBorderZoom; // 0.025;
                    Max += delta * dontSticktoBorderZoom; // 0.025;


                  }

                  v.IRLy = Min;
                  v.zoomy = (v.Height - v.Tmargin - v.Bmargin) / (Max - Min);
                  for (int i = 0; i < _series[k].segments.Count; i++)
                    lineCount += DoSegmentRendering(v, ng, mypenb, _series[k].segments[i].data, _series[k].segments[i].count);
                }
            }
            else
            {  //  yAxis handling inherited from main view-port settings
              for (int i = 0; i < _yAxes.Count; i++)
              { // find out data MinMax
                MinMaxHandler.MinMax Yrange = MinMaxHandler.DefaultValue();
                for (int j = 0; j < _series.Count; j++)
                  if ((_series[j].yAxisIndex == i) && (!_series[j].disabled))
                    Yrange = MinMaxHandler.Combine(Yrange, _series[j].valueRange);
                Yrange = MinMaxHandler.extend(Yrange, 1 + 2 * dontSticktoBorderZoom);
                Min = _yAxes[i].min; if (Double.IsNaN(Min)) Min = Yrange.Min;
                Max = _yAxes[i].max; if (Double.IsNaN(Max)) Max = Yrange.Max;
                if (Double.IsNaN(Min)) { Min = 0.0; Max = 1.0; }
                if (Max - Min <= 0) { Min = Min - 0.5; Max = Min + 0.5; }
                v.IRLy = Min;
                v.zoomy = (v.Height - v.Tmargin - v.Bmargin) / (Max - Min);
                for (int j = 0; j < _series.Count; j++)
                  if ((_series[j].yAxisIndex == i) && (!_series[j].disabled) && (_series[j].visible))
                  {
                    ng.comment("** navigator series " + j.ToString());
                    mypenb = _series[j].navigatorpen;
                    for (int k = 0; k < _series[j].segments.Count; k++)
                      lineCount += DoSegmentRendering(v, ng, mypenb, _series[j].segments[k].data, _series[j].segments[k].count);
                  }
              }





            }


            if (_navigator.borderThickness > 0)
            {
              ng.DrawLine(_navigator.borderPen, v.Lmargin, v.Tmargin, v.Width - v.Rmargin, v.Tmargin);
            }
            // step 7C, draw Scale

            DrawMonitorXAxis(v, ng, _navigator.Xrange, xAxis.labelFormat);
            _navigator.setIRLPosition(v.IRLx, v.IRLy, v.zoomx, v.zoomy);


          }
          if (!(g is YGraphicsSVG))  ng.Dispose();



        }
        // set  7E, copy cache to display
        Rectangle r = new Rectangle(v.Lmargin, v.Tmargin, v.Width - v.Rmargin - v.Lmargin + 1, v.Height - v.Bmargin - v.Tmargin);
        g.SetClip(r);

        if (!(g is YGraphicsSVG)) g.DrawImage(navigatorCache, r, r, GraphicsUnit.Pixel);


        //navigatorCache.Save("C:\\tmp\\t.png", ImageFormat.Png);
        // set  7E, draw Cursor

        if (_navigator.borderThickness > 0)
        {
          g.DrawLine(_navigator.borderPen, v.Lmargin, v.Tmargin, v.Lmargin, v.Height - v.Bmargin);
          g.DrawLine(_navigator.borderPen, v.Width - v.Rmargin, v.Tmargin, v.Width - v.Rmargin, v.Height - v.Bmargin);
        }
        pointXY IRLCursorStart = ViewPortPointToIRL(mainViewPort, new Point(mainViewPort.Lmargin, 0));
        pointXY IRLCursorEnd = ViewPortPointToIRL(mainViewPort, new Point(mainViewPort.Width - mainViewPort.Rmargin, 0));
        Point CursorStart = IRLPointToViewPort(_navigator.viewport, new pointXY { x = IRLCursorStart.x, y = 0 });
        Point CursorEnd = IRLPointToViewPort(_navigator.viewport, new pointXY { x = IRLCursorEnd.x, y = 0 });


        g.FillRectangle(_navigator.cursorBrush, new Rectangle(CursorStart.X - 1, v.Tmargin, CursorEnd.X - CursorStart.X + 2, v.Height - v.Bmargin - v.Tmargin));
        g.DrawLine(_navigator.cursorBorderPen, (int)(CursorStart.X - 1), (int)(v.Tmargin), (int)(CursorStart.X - 1), (int)(v.Height - v.Bmargin));
        g.DrawLine(_navigator.cursorBorderPen, (int)(CursorEnd.X + 1), (int)(v.Tmargin), (int)(CursorEnd.X + 1), (int)(v.Height - v.Bmargin));





      }




      if (_legendPanel.overlap) drawLegendPanel(g, UIw, UIh, ref mainViewPort);



      DrawDataPanels(mainViewPort, g, xAxis, _yAxes, UIw, UIh);

      DrawDataTracker(g, UIw, UIh);

      DrawMessagePanels(g, UIw, UIh);


      return 0;

    }


    private void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Left)
      {
        double delta = 0.2 * (_xAxis.max - _xAxis.min);
        _xAxis.set_minMax(_xAxis.min - delta,
                          _xAxis.max - delta);
        redraw();
      }
      if (e.KeyCode == Keys.Right)
      {
        double delta = 0.2 * (_xAxis.max - _xAxis.min);
        _xAxis.set_minMax(_xAxis.min + delta,
                            _xAxis.max + delta);
        redraw();
      }
      if (e.KeyCode == Keys.Up)
      {
        mouseWheel(new Point(this.UIContainer.Width >> 1, this.UIContainer.Height >> 1), 10);
      }

      if (e.KeyCode == Keys.Down)
      {
        mouseWheel(new Point(this.UIContainer.Width >> 1, this.UIContainer.Height >> 1), -10);
      }
    }




    public void mouseWheel(Point pos, int delta)
    {
      // not the most elegant way to perform the zoom transformation :-(
      double ZoomFactor = Math.Pow(1.25, (double)delta / 120);  // 120 is totally arbitrary
      double NextZoomX = mainViewPort.zoomx * ZoomFactor;
      if ((NextZoomX > mainViewPort.zoomx) && (NextZoomX > 1000)) return;
      mainViewPort.IRLx += ((pos.X - mainViewPort.Lmargin) / mainViewPort.zoomx) - ((pos.X - mainViewPort.Lmargin) / NextZoomX);
      double range = _xAxis.max - _xAxis.min;
      _xAxis.set_minMax(mainViewPort.IRLx,
                          mainViewPort.IRLx + range / ZoomFactor);
      mainViewPort.zoomx = NextZoomX;
      //   log("ZoomX=" + mainViewPort.zoomx.ToString());
      redraw();
    }

    public Control FindControlAtPoint(Control container, Point pos)
    {
      Control child;
      foreach (Control c in container.Controls)
      {
        if (c.Visible && c.Bounds.Contains(pos))
        {
          child = FindControlAtPoint(c, new Point(pos.X - c.Left, pos.Y - c.Top));
          if (child == null) return c;
          else return child;
        }
      }
      return null;
    }




    public void mouseWheelEvent(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      Point mouseLocation = new Point(e.X, e.Y);

      Control c = FindControlAtPoint(parentForm, mouseLocation);
      if (c == UIContainer) this.mouseWheel(new Point(e.X - UIContainer.Left, e.Y - UIContainer.Top), e.Delta);



    }


  }





}
