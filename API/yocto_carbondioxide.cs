/*********************************************************************
 *
 *  $Id: yocto_carbondioxide.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindCarbonDioxide(), the high-level API for CarbonDioxide functions
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
    //--- (YCarbonDioxide return codes)
    //--- (end of YCarbonDioxide return codes)
//--- (YCarbonDioxide dlldef)
//--- (end of YCarbonDioxide dlldef)
//--- (YCarbonDioxide yapiwrapper)
//--- (end of YCarbonDioxide yapiwrapper)
//--- (YCarbonDioxide class start)
/**
 * <summary>
 *   The Yoctopuce class YCarbonDioxide allows you to read and configure Yoctopuce CO2
 *   sensors.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions,  to access the autonomous datalogger.
 *   This class adds the ability to perform manual calibration if required.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YCarbonDioxide : YSensor
{
//--- (end of YCarbonDioxide class start)
    //--- (YCarbonDioxide definitions)
    public new delegate void ValueCallback(YCarbonDioxide func, string value);
    public new delegate void TimedReportCallback(YCarbonDioxide func, YMeasure measure);

    public const int ABCPERIOD_INVALID = YAPI.INVALID_INT;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _abcPeriod = ABCPERIOD_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackCarbonDioxide = null;
    protected TimedReportCallback _timedReportCallbackCarbonDioxide = null;
    //--- (end of YCarbonDioxide definitions)

    public YCarbonDioxide(string func)
        : base(func)
    {
        _className = "CarbonDioxide";
        //--- (YCarbonDioxide attributes initialization)
        //--- (end of YCarbonDioxide attributes initialization)
    }

    //--- (YCarbonDioxide implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("abcPeriod"))
        {
            _abcPeriod = json_val.getInt("abcPeriod");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the Automatic Baseline Calibration period, in hours.
     * <para>
     *   A negative value
     *   means that automatic baseline calibration is disabled.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the Automatic Baseline Calibration period, in hours
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCarbonDioxide.ABCPERIOD_INVALID</c>.
     * </para>
     */
    public int get_abcPeriod()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ABCPERIOD_INVALID;
                }
            }
            res = this._abcPeriod;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes Automatic Baseline Calibration period, in hours.
     * <para>
     *   If you need
     *   to disable automatic baseline calibration (for instance when using the
     *   sensor in an environment that is constantly above 400 ppm CO2), set the
     *   period to -1. Remember to call the <c>saveToFlash()</c> method of the
     *   module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to Automatic Baseline Calibration period, in hours
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
    public int set_abcPeriod(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("abcPeriod", rest_val);
        }
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
     *   Retrieves a CO2 sensor for a given identifier.
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
     *   This function does not require that the CO2 sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YCarbonDioxide.isOnline()</c> to test if the CO2 sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a CO2 sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the CO2 sensor
     * </param>
     * <returns>
     *   a <c>YCarbonDioxide</c> object allowing you to drive the CO2 sensor.
     * </returns>
     */
    public static YCarbonDioxide FindCarbonDioxide(string func)
    {
        YCarbonDioxide obj;
        lock (YAPI.globalLock) {
            obj = (YCarbonDioxide) YFunction._FindFromCache("CarbonDioxide", func);
            if (obj == null) {
                obj = new YCarbonDioxide(func);
                YFunction._AddToCache("CarbonDioxide", func, obj);
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
        this._valueCallbackCarbonDioxide = callback;
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
        if (this._valueCallbackCarbonDioxide != null) {
            this._valueCallbackCarbonDioxide(this, value);
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
        this._timedReportCallbackCarbonDioxide = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackCarbonDioxide != null) {
            this._timedReportCallbackCarbonDioxide(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Triggers a baseline calibration at standard CO2 ambiant level (400ppm).
     * <para>
     *   It is normally not necessary to manually calibrate the sensor, because
     *   the built-in automatic baseline calibration procedure will automatically
     *   fix any long-term drift based on the lowest level of CO2 observed over the
     *   automatic calibration period. However, if you disable automatic baseline
     *   calibration, you may want to manually trigger a calibration from time to
     *   time. Before starting a baseline calibration, make sure to put the sensor
     *   in a standard environment (e.g. outside in fresh air) at around 400 ppm.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int triggerBaselineCalibration()
    {
        return this.set_command("BC");
    }

    public virtual int triggetBaselineCalibration()
    {
        return this.triggerBaselineCalibration();
    }

    /**
     * <summary>
     *   Triggers a zero calibration of the sensor on carbon dioxide-free air.
     * <para>
     *   It is normally not necessary to manually calibrate the sensor, because
     *   the built-in automatic baseline calibration procedure will automatically
     *   fix any long-term drift based on the lowest level of CO2 observed over the
     *   automatic calibration period. However, if you disable automatic baseline
     *   calibration, you may want to manually trigger a calibration from time to
     *   time. Before starting a zero calibration, you should circulate carbon
     *   dioxide-free air within the sensor for a minute or two, using a small pipe
     *   connected to the sensor. Please contact support@yoctopuce.com for more details
     *   on the zero calibration procedure.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int triggerZeroCalibration()
    {
        return this.set_command("ZC");
    }

    public virtual int triggetZeroCalibration()
    {
        return this.triggerZeroCalibration();
    }

    /**
     * <summary>
     *   Continues the enumeration of CO2 sensors started using <c>yFirstCarbonDioxide()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned CO2 sensors order.
     *   If you want to find a specific a CO2 sensor, use <c>CarbonDioxide.findCarbonDioxide()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCarbonDioxide</c> object, corresponding to
     *   a CO2 sensor currently online, or a <c>null</c> pointer
     *   if there are no more CO2 sensors to enumerate.
     * </returns>
     */
    public YCarbonDioxide nextCarbonDioxide()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindCarbonDioxide(hwid);
    }

    //--- (end of YCarbonDioxide implementation)

    //--- (YCarbonDioxide functions)

    /**
     * <summary>
     *   Starts the enumeration of CO2 sensors currently accessible.
     * <para>
     *   Use the method <c>YCarbonDioxide.nextCarbonDioxide()</c> to iterate on
     *   next CO2 sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCarbonDioxide</c> object, corresponding to
     *   the first CO2 sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YCarbonDioxide FirstCarbonDioxide()
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
        err = YAPI.apiGetFunctionsByClass("CarbonDioxide", 0, p, size, ref neededsize, ref errmsg);
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
        return FindCarbonDioxide(serial + "." + funcId);
    }



    //--- (end of YCarbonDioxide functions)
}
#pragma warning restore 1591
