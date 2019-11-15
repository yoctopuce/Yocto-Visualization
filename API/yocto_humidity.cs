/*********************************************************************
 *
 *  $Id: yocto_humidity.cs 37827 2019-10-25 13:07:48Z mvuilleu $
 *
 *  Implements yFindHumidity(), the high-level API for Humidity functions
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
    //--- (YHumidity return codes)
    //--- (end of YHumidity return codes)
//--- (YHumidity dlldef)
//--- (end of YHumidity dlldef)
//--- (YHumidity yapiwrapper)
//--- (end of YHumidity yapiwrapper)
//--- (YHumidity class start)
/**
 * <summary>
 *   The YHumidity class allows you to read and configure Yoctopuce humidity
 *   sensors, for instance using a Yocto-Meteo-V2, a Yocto-VOC-V3 or a Yocto-CO2-V2.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YHumidity : YSensor
{
//--- (end of YHumidity class start)
    //--- (YHumidity definitions)
    public new delegate void ValueCallback(YHumidity func, string value);
    public new delegate void TimedReportCallback(YHumidity func, YMeasure measure);

    public const double RELHUM_INVALID = YAPI.INVALID_DOUBLE;
    public const double ABSHUM_INVALID = YAPI.INVALID_DOUBLE;
    protected double _relHum = RELHUM_INVALID;
    protected double _absHum = ABSHUM_INVALID;
    protected ValueCallback _valueCallbackHumidity = null;
    protected TimedReportCallback _timedReportCallbackHumidity = null;
    //--- (end of YHumidity definitions)

    public YHumidity(string func)
        : base(func)
    {
        _className = "Humidity";
        //--- (YHumidity attributes initialization)
        //--- (end of YHumidity attributes initialization)
    }

    //--- (YHumidity implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("relHum"))
        {
            _relHum = Math.Round(json_val.getDouble("relHum") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("absHum"))
        {
            _absHum = Math.Round(json_val.getDouble("absHum") * 1000.0 / 65536.0) / 1000.0;
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the primary unit for measuring humidity.
     * <para>
     *   That unit is a string.
     *   If that strings starts with the letter 'g', the primary measured value is the absolute
     *   humidity, in g/m3. Otherwise, the primary measured value will be the relative humidity
     *   (RH), in per cents.
     * </para>
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification
     *   must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the primary unit for measuring humidity
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
     *   Returns the current relative humidity, in per cents.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current relative humidity, in per cents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YHumidity.RELHUM_INVALID</c>.
     * </para>
     */
    public double get_relHum()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RELHUM_INVALID;
                }
            }
            res = this._relHum;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the current absolute humidity, in grams per cubic meter of air.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current absolute humidity, in grams per cubic meter of air
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YHumidity.ABSHUM_INVALID</c>.
     * </para>
     */
    public double get_absHum()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ABSHUM_INVALID;
                }
            }
            res = this._absHum;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves a humidity sensor for a given identifier.
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
     *   This function does not require that the humidity sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YHumidity.isOnline()</c> to test if the humidity sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a humidity sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the humidity sensor, for instance
     *   <c>METEOMK2.humidity</c>.
     * </param>
     * <returns>
     *   a <c>YHumidity</c> object allowing you to drive the humidity sensor.
     * </returns>
     */
    public static YHumidity FindHumidity(string func)
    {
        YHumidity obj;
        lock (YAPI.globalLock) {
            obj = (YHumidity) YFunction._FindFromCache("Humidity", func);
            if (obj == null) {
                obj = new YHumidity(func);
                YFunction._AddToCache("Humidity", func, obj);
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
        this._valueCallbackHumidity = callback;
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
        if (this._valueCallbackHumidity != null) {
            this._valueCallbackHumidity(this, value);
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
        this._timedReportCallbackHumidity = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackHumidity != null) {
            this._timedReportCallbackHumidity(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of humidity sensors started using <c>yFirstHumidity()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned humidity sensors order.
     *   If you want to find a specific a humidity sensor, use <c>Humidity.findHumidity()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YHumidity</c> object, corresponding to
     *   a humidity sensor currently online, or a <c>null</c> pointer
     *   if there are no more humidity sensors to enumerate.
     * </returns>
     */
    public YHumidity nextHumidity()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindHumidity(hwid);
    }

    //--- (end of YHumidity implementation)

    //--- (YHumidity functions)

    /**
     * <summary>
     *   Starts the enumeration of humidity sensors currently accessible.
     * <para>
     *   Use the method <c>YHumidity.nextHumidity()</c> to iterate on
     *   next humidity sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YHumidity</c> object, corresponding to
     *   the first humidity sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YHumidity FirstHumidity()
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
        err = YAPI.apiGetFunctionsByClass("Humidity", 0, p, size, ref neededsize, ref errmsg);
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
        return FindHumidity(serial + "." + funcId);
    }



    //--- (end of YHumidity functions)
}
#pragma warning restore 1591
