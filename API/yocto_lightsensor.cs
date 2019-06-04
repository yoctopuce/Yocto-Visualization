/*********************************************************************
 *
 *  $Id: yocto_lightsensor.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindLightSensor(), the high-level API for LightSensor functions
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
    //--- (YLightSensor return codes)
    //--- (end of YLightSensor return codes)
//--- (YLightSensor dlldef)
//--- (end of YLightSensor dlldef)
//--- (YLightSensor yapiwrapper)
//--- (end of YLightSensor yapiwrapper)
//--- (YLightSensor class start)
/**
 * <summary>
 *   The Yoctopuce class YLightSensor allows you to read and configure Yoctopuce light
 *   sensors.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 *   This class adds the ability to easily perform a one-point linear calibration
 *   to compensate the effect of a glass or filter placed in front of the sensor.
 *   For some light sensors with several working modes, this class can select the
 *   desired working mode.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YLightSensor : YSensor
{
//--- (end of YLightSensor class start)
    //--- (YLightSensor definitions)
    public new delegate void ValueCallback(YLightSensor func, string value);
    public new delegate void TimedReportCallback(YLightSensor func, YMeasure measure);

    public const int MEASURETYPE_HUMAN_EYE = 0;
    public const int MEASURETYPE_WIDE_SPECTRUM = 1;
    public const int MEASURETYPE_INFRARED = 2;
    public const int MEASURETYPE_HIGH_RATE = 3;
    public const int MEASURETYPE_HIGH_ENERGY = 4;
    public const int MEASURETYPE_INVALID = -1;
    protected int _measureType = MEASURETYPE_INVALID;
    protected ValueCallback _valueCallbackLightSensor = null;
    protected TimedReportCallback _timedReportCallbackLightSensor = null;
    //--- (end of YLightSensor definitions)

    public YLightSensor(string func)
        : base(func)
    {
        _className = "LightSensor";
        //--- (YLightSensor attributes initialization)
        //--- (end of YLightSensor attributes initialization)
    }

    //--- (YLightSensor implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("measureType"))
        {
            _measureType = json_val.getInt("measureType");
        }
        base._parseAttr(json_val);
    }

    public int set_currentValue(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("currentValue", rest_val);
        }
    }

    /**
     * <summary>
     *   Changes the sensor-specific calibration parameter so that the current value
     *   matches a desired target (linear scaling).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="calibratedVal">
     *   the desired target value.
     * </param>
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int calibrate(double calibratedVal)
    {
        string rest_val;
        rest_val = Math.Round(calibratedVal * 65536.0).ToString();
        return _setAttr("currentValue", rest_val);
    }

    /**
     * <summary>
     *   Returns the type of light measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YLightSensor.MEASURETYPE_HUMAN_EYE</c>, <c>YLightSensor.MEASURETYPE_WIDE_SPECTRUM</c>,
     *   <c>YLightSensor.MEASURETYPE_INFRARED</c>, <c>YLightSensor.MEASURETYPE_HIGH_RATE</c> and
     *   <c>YLightSensor.MEASURETYPE_HIGH_ENERGY</c> corresponding to the type of light measure
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YLightSensor.MEASURETYPE_INVALID</c>.
     * </para>
     */
    public int get_measureType()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MEASURETYPE_INVALID;
                }
            }
            res = this._measureType;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the light sensor type used in the device.
     * <para>
     *   The measure can either
     *   approximate the response of the human eye, focus on a specific light
     *   spectrum, depending on the capabilities of the light-sensitive cell.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YLightSensor.MEASURETYPE_HUMAN_EYE</c>, <c>YLightSensor.MEASURETYPE_WIDE_SPECTRUM</c>,
     *   <c>YLightSensor.MEASURETYPE_INFRARED</c>, <c>YLightSensor.MEASURETYPE_HIGH_RATE</c> and
     *   <c>YLightSensor.MEASURETYPE_HIGH_ENERGY</c> corresponding to the light sensor type used in the device
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
    public int set_measureType(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("measureType", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a light sensor for a given identifier.
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
     *   This function does not require that the light sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YLightSensor.isOnline()</c> to test if the light sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a light sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the light sensor
     * </param>
     * <returns>
     *   a <c>YLightSensor</c> object allowing you to drive the light sensor.
     * </returns>
     */
    public static YLightSensor FindLightSensor(string func)
    {
        YLightSensor obj;
        lock (YAPI.globalLock) {
            obj = (YLightSensor) YFunction._FindFromCache("LightSensor", func);
            if (obj == null) {
                obj = new YLightSensor(func);
                YFunction._AddToCache("LightSensor", func, obj);
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
        this._valueCallbackLightSensor = callback;
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
        if (this._valueCallbackLightSensor != null) {
            this._valueCallbackLightSensor(this, value);
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
        this._timedReportCallbackLightSensor = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackLightSensor != null) {
            this._timedReportCallbackLightSensor(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of light sensors started using <c>yFirstLightSensor()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned light sensors order.
     *   If you want to find a specific a light sensor, use <c>LightSensor.findLightSensor()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YLightSensor</c> object, corresponding to
     *   a light sensor currently online, or a <c>null</c> pointer
     *   if there are no more light sensors to enumerate.
     * </returns>
     */
    public YLightSensor nextLightSensor()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindLightSensor(hwid);
    }

    //--- (end of YLightSensor implementation)

    //--- (YLightSensor functions)

    /**
     * <summary>
     *   Starts the enumeration of light sensors currently accessible.
     * <para>
     *   Use the method <c>YLightSensor.nextLightSensor()</c> to iterate on
     *   next light sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YLightSensor</c> object, corresponding to
     *   the first light sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YLightSensor FirstLightSensor()
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
        err = YAPI.apiGetFunctionsByClass("LightSensor", 0, p, size, ref neededsize, ref errmsg);
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
        return FindLightSensor(serial + "." + funcId);
    }



    //--- (end of YLightSensor functions)
}
#pragma warning restore 1591
