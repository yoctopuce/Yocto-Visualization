/*********************************************************************
 *
 *  $Id: yocto_rangefinder.cs 37149 2019-09-12 21:24:53Z mvuilleu $
 *
 *  Implements yFindRangeFinder(), the high-level API for RangeFinder functions
 *
 *  - - - - - - - - - License information: - - - - - - - - -
 *
 *  Copyright (C) 2011 and beyond by Yoctopuce Sarl, Switzerland.
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
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED 'AS IS' WITHOUT
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
 *
 *********************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using YDEV_DESCR = System.Int32;
using YFUN_DESCR = System.Int32;

 #pragma warning disable 1591
    //--- (YRangeFinder return codes)
    //--- (end of YRangeFinder return codes)
//--- (YRangeFinder dlldef)
//--- (end of YRangeFinder dlldef)
//--- (YRangeFinder yapiwrapper)
//--- (end of YRangeFinder yapiwrapper)
//--- (YRangeFinder class start)
/**
 * <summary>
 *   The Yoctopuce class YRangeFinder allows you to use and configure Yoctopuce range finder
 *   sensors.
 * <para>
 *   It inherits from the YSensor class the core functions to read measurements,
 *   register callback functions, access the autonomous datalogger.
 *   This class adds the ability to easily perform a one-point linear calibration
 *   to compensate the effect of a glass or filter placed in front of the sensor.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YRangeFinder : YSensor
{
//--- (end of YRangeFinder class start)
    //--- (YRangeFinder definitions)
    public new delegate void ValueCallback(YRangeFinder func, string value);
    public new delegate void TimedReportCallback(YRangeFinder func, YMeasure measure);

    public const int RANGEFINDERMODE_DEFAULT = 0;
    public const int RANGEFINDERMODE_LONG_RANGE = 1;
    public const int RANGEFINDERMODE_HIGH_ACCURACY = 2;
    public const int RANGEFINDERMODE_HIGH_SPEED = 3;
    public const int RANGEFINDERMODE_INVALID = -1;
    public const long TIMEFRAME_INVALID = YAPI.INVALID_LONG;
    public const int QUALITY_INVALID = YAPI.INVALID_UINT;
    public const string HARDWARECALIBRATION_INVALID = YAPI.INVALID_STRING;
    public const double CURRENTTEMPERATURE_INVALID = YAPI.INVALID_DOUBLE;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _rangeFinderMode = RANGEFINDERMODE_INVALID;
    protected long _timeFrame = TIMEFRAME_INVALID;
    protected int _quality = QUALITY_INVALID;
    protected string _hardwareCalibration = HARDWARECALIBRATION_INVALID;
    protected double _currentTemperature = CURRENTTEMPERATURE_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackRangeFinder = null;
    protected TimedReportCallback _timedReportCallbackRangeFinder = null;
    //--- (end of YRangeFinder definitions)

    public YRangeFinder(string func)
        : base(func)
    {
        _className = "RangeFinder";
        //--- (YRangeFinder attributes initialization)
        //--- (end of YRangeFinder attributes initialization)
    }

    //--- (YRangeFinder implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("rangeFinderMode"))
        {
            _rangeFinderMode = json_val.getInt("rangeFinderMode");
        }
        if (json_val.has("timeFrame"))
        {
            _timeFrame = json_val.getLong("timeFrame");
        }
        if (json_val.has("quality"))
        {
            _quality = json_val.getInt("quality");
        }
        if (json_val.has("hardwareCalibration"))
        {
            _hardwareCalibration = json_val.getString("hardwareCalibration");
        }
        if (json_val.has("currentTemperature"))
        {
            _currentTemperature = Math.Round(json_val.getDouble("currentTemperature") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the measuring unit for the measured range.
     * <para>
     *   That unit is a string.
     *   String value can be <c>"</c> or <c>mm</c>. Any other value is ignored.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     *   WARNING: if a specific calibration is defined for the rangeFinder function, a
     *   unit system change will probably break it.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the measuring unit for the measured range
     * </param>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int set_unit(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("unit", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the range finder running mode.
     * <para>
     *   The rangefinder running mode
     *   allows you to put priority on precision, speed or maximum range.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YRangeFinder.RANGEFINDERMODE_DEFAULT</c>, <c>YRangeFinder.RANGEFINDERMODE_LONG_RANGE</c>,
     *   <c>YRangeFinder.RANGEFINDERMODE_HIGH_ACCURACY</c> and <c>YRangeFinder.RANGEFINDERMODE_HIGH_SPEED</c>
     *   corresponding to the range finder running mode
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRangeFinder.RANGEFINDERMODE_INVALID</c>.
     * </para>
     */
    public int get_rangeFinderMode()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RANGEFINDERMODE_INVALID;
                }
            }
            res = this._rangeFinderMode;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the rangefinder running mode, allowing you to put priority on
     *   precision, speed or maximum range.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YRangeFinder.RANGEFINDERMODE_DEFAULT</c>, <c>YRangeFinder.RANGEFINDERMODE_LONG_RANGE</c>,
     *   <c>YRangeFinder.RANGEFINDERMODE_HIGH_ACCURACY</c> and <c>YRangeFinder.RANGEFINDERMODE_HIGH_SPEED</c>
     *   corresponding to the rangefinder running mode, allowing you to put priority on
     *   precision, speed or maximum range
     * </param>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int set_rangeFinderMode(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("rangeFinderMode", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the time frame used to measure the distance and estimate the measure
     *   reliability.
     * <para>
     *   The time frame is expressed in milliseconds.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the time frame used to measure the distance and estimate the measure
     *   reliability
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRangeFinder.TIMEFRAME_INVALID</c>.
     * </para>
     */
    public long get_timeFrame()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return TIMEFRAME_INVALID;
                }
            }
            res = this._timeFrame;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the time frame used to measure the distance and estimate the measure
     *   reliability.
     * <para>
     *   The time frame is expressed in milliseconds. A larger timeframe
     *   improves stability and reliability, at the cost of higher latency, but prevents
     *   the detection of events shorter than the time frame.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the time frame used to measure the distance and estimate the measure
     *   reliability
     * </param>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int set_timeFrame(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("timeFrame", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns a measure quality estimate, based on measured dispersion.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to a measure quality estimate, based on measured dispersion
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRangeFinder.QUALITY_INVALID</c>.
     * </para>
     */
    public int get_quality()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return QUALITY_INVALID;
                }
            }
            res = this._quality;
        }
        return res;
    }

    public string get_hardwareCalibration()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return HARDWARECALIBRATION_INVALID;
                }
            }
            res = this._hardwareCalibration;
        }
        return res;
    }

    public int set_hardwareCalibration(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("hardwareCalibration", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the current sensor temperature, as a floating point number.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current sensor temperature, as a floating point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRangeFinder.CURRENTTEMPERATURE_INVALID</c>.
     * </para>
     */
    public double get_currentTemperature()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENTTEMPERATURE_INVALID;
                }
            }
            res = this._currentTemperature;
        }
        return res;
    }

    public string get_command()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return COMMAND_INVALID;
                }
            }
            res = this._command;
        }
        return res;
    }

    public int set_command(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("command", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a range finder for a given identifier.
     * <para>
     *   The identifier can be specified using several formats:
     * </para>
     * <para>
     * </para>
     * <para>
     *   - FunctionLogicalName
     * </para>
     * <para>
     *   - ModuleSerialNumber.FunctionIdentifier
     * </para>
     * <para>
     *   - ModuleSerialNumber.FunctionLogicalName
     * </para>
     * <para>
     *   - ModuleLogicalName.FunctionIdentifier
     * </para>
     * <para>
     *   - ModuleLogicalName.FunctionLogicalName
     * </para>
     * <para>
     * </para>
     * <para>
     *   This function does not require that the range finder is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YRangeFinder.isOnline()</c> to test if the range finder is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a range finder by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * <para>
     *   If a call to this object's is_online() method returns FALSE although
     *   you are certain that the matching device is plugged, make sure that you did
     *   call registerHub() at application initialization time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the range finder
     * </param>
     * <returns>
     *   a <c>YRangeFinder</c> object allowing you to drive the range finder.
     * </returns>
     */
    public static YRangeFinder FindRangeFinder(string func)
    {
        YRangeFinder obj;
        lock (YAPI.globalLock) {
            obj = (YRangeFinder) YFunction._FindFromCache("RangeFinder", func);
            if (obj == null) {
                obj = new YRangeFinder(func);
                YFunction._AddToCache("RangeFinder", func, obj);
            }
        }
        return obj;
    }

    /**
     * <summary>
     *   Registers the callback function that is invoked on every change of advertised value.
     * <para>
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
     *   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the function object of which the value has changed, and the character string describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerValueCallback(ValueCallback callback)
    {
        string val;
        if (callback != null) {
            YFunction._UpdateValueCallbackList(this, true);
        } else {
            YFunction._UpdateValueCallbackList(this, false);
        }
        this._valueCallbackRangeFinder = callback;
        // Immediately invoke value callback with current value
        if (callback != null && this.isOnline()) {
            val = this._advertisedValue;
            if (!(val == "")) {
                this._invokeValueCallback(val);
            }
        }
        return 0;
    }

    public override int _invokeValueCallback(string value)
    {
        if (this._valueCallbackRangeFinder != null) {
            this._valueCallbackRangeFinder(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Registers the callback function that is invoked on every periodic timed notification.
     * <para>
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
     *   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the function object of which the value has changed, and an YMeasure object describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerTimedReportCallback(TimedReportCallback callback)
    {
        YSensor sensor;
        sensor = this;
        if (callback != null) {
            YFunction._UpdateTimedReportCallbackList(sensor, true);
        } else {
            YFunction._UpdateTimedReportCallbackList(sensor, false);
        }
        this._timedReportCallbackRangeFinder = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackRangeFinder != null) {
            this._timedReportCallbackRangeFinder(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Returns the temperature at the time when the latest calibration was performed.
     * <para>
     *   This function can be used to determine if a new calibration for ambient temperature
     *   is required.
     * </para>
     * </summary>
     * <returns>
     *   a temperature, as a floating point number.
     *   On failure, throws an exception or return YAPI.INVALID_DOUBLE.
     * </returns>
     */
    public virtual double get_hardwareCalibrationTemperature()
    {
        string hwcal;
        hwcal = this.get_hardwareCalibration();
        if (!((hwcal).Substring(0, 1) == "@")) {
            return YAPI.INVALID_DOUBLE;
        }
        return YAPI._atoi((hwcal).Substring(1, (hwcal).Length));
    }

    /**
     * <summary>
     *   Triggers a sensor calibration according to the current ambient temperature.
     * <para>
     *   That
     *   calibration process needs no physical interaction with the sensor. It is performed
     *   automatically at device startup, but it is recommended to start it again when the
     *   temperature delta since the latest calibration exceeds 8°C.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int triggerTemperatureCalibration()
    {
        return this.set_command("T");
    }

    /**
     * <summary>
     *   Triggers the photon detector hardware calibration.
     * <para>
     *   This function is part of the calibration procedure to compensate for the the effect
     *   of a cover glass. Make sure to read the chapter about hardware calibration for details
     *   on the calibration procedure for proper results.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int triggerSpadCalibration()
    {
        return this.set_command("S");
    }

    /**
     * <summary>
     *   Triggers the hardware offset calibration of the distance sensor.
     * <para>
     *   This function is part of the calibration procedure to compensate for the the effect
     *   of a cover glass. Make sure to read the chapter about hardware calibration for details
     *   on the calibration procedure for proper results.
     * </para>
     * </summary>
     * <param name="targetDist">
     *   true distance of the calibration target, in mm or inches, depending
     *   on the unit selected in the device
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int triggerOffsetCalibration(double targetDist)
    {
        int distmm;
        if (this.get_unit() == "\"") {
            distmm = (int) Math.Round(targetDist * 25.4);
        } else {
            distmm = (int) Math.Round(targetDist);
        }
        return this.set_command("O"+Convert.ToString(distmm));
    }

    /**
     * <summary>
     *   Triggers the hardware cross-talk calibration of the distance sensor.
     * <para>
     *   This function is part of the calibration procedure to compensate for the the effect
     *   of a cover glass. Make sure to read the chapter about hardware calibration for details
     *   on the calibration procedure for proper results.
     * </para>
     * </summary>
     * <param name="targetDist">
     *   true distance of the calibration target, in mm or inches, depending
     *   on the unit selected in the device
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int triggerXTalkCalibration(double targetDist)
    {
        int distmm;
        if (this.get_unit() == "\"") {
            distmm = (int) Math.Round(targetDist * 25.4);
        } else {
            distmm = (int) Math.Round(targetDist);
        }
        return this.set_command("X"+Convert.ToString(distmm));
    }

    /**
     * <summary>
     *   Cancels the effect of previous hardware calibration procedures to compensate
     *   for cover glass, and restores factory settings.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int cancelCoverGlassCalibrations()
    {
        return this.set_hardwareCalibration("");
    }

    /**
     * <summary>
     *   Continues the enumeration of range finders started using <c>yFirstRangeFinder()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned range finders order.
     *   If you want to find a specific a range finder, use <c>RangeFinder.findRangeFinder()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRangeFinder</c> object, corresponding to
     *   a range finder currently online, or a <c>null</c> pointer
     *   if there are no more range finders to enumerate.
     * </returns>
     */
    public YRangeFinder nextRangeFinder()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindRangeFinder(hwid);
    }

    //--- (end of YRangeFinder implementation)

    //--- (YRangeFinder functions)

    /**
     * <summary>
     *   Starts the enumeration of range finders currently accessible.
     * <para>
     *   Use the method <c>YRangeFinder.nextRangeFinder()</c> to iterate on
     *   next range finders.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRangeFinder</c> object, corresponding to
     *   the first range finder currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YRangeFinder FirstRangeFinder()
    {
        YFUN_DESCR[] v_fundescr = new YFUN_DESCR[1];
        YDEV_DESCR dev = default(YDEV_DESCR);
        int neededsize = 0;
        int err = 0;
        string serial = null;
        string funcId = null;
        string funcName = null;
        string funcVal = null;
        string errmsg = "";
        int size = Marshal.SizeOf(v_fundescr[0]);
        IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr[0]));
        err = YAPI.apiGetFunctionsByClass("RangeFinder", 0, p, size, ref neededsize, ref errmsg);
        Marshal.Copy(p, v_fundescr, 0, 1);
        Marshal.FreeHGlobal(p);
        if ((YAPI.YISERR(err) | (neededsize == 0)))
            return null;
        serial = "";
        funcId = "";
        funcName = "";
        funcVal = "";
        errmsg = "";
        if ((YAPI.YISERR(YAPI.yapiGetFunctionInfo(v_fundescr[0], ref dev, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg))))
            return null;
        return FindRangeFinder(serial + "." + funcId);
    }



    //--- (end of YRangeFinder functions)
}
#pragma warning restore 1591
