/*********************************************************************
 *
 *  $Id: yocto_longitude.cs 39658 2020-03-12 15:36:29Z seb $
 *
 *  Implements yFindLongitude(), the high-level API for Longitude functions
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
    //--- (YLongitude return codes)
    //--- (end of YLongitude return codes)
//--- (YLongitude dlldef)
//--- (end of YLongitude dlldef)
//--- (YLongitude yapiwrapper)
//--- (end of YLongitude yapiwrapper)
//--- (YLongitude class start)
/**
 * <summary>
 *   The <c>YLongitude</c> class allows you to read and configure Yoctopuce longitude sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YLongitude : YSensor
{
//--- (end of YLongitude class start)
    //--- (YLongitude definitions)
    public new delegate void ValueCallback(YLongitude func, string value);
    public new delegate void TimedReportCallback(YLongitude func, YMeasure measure);

    protected ValueCallback _valueCallbackLongitude = null;
    protected TimedReportCallback _timedReportCallbackLongitude = null;
    //--- (end of YLongitude definitions)

    public YLongitude(string func)
        : base(func)
    {
        _className = "Longitude";
        //--- (YLongitude attributes initialization)
        //--- (end of YLongitude attributes initialization)
    }

    //--- (YLongitude implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        base._parseAttr(json_val);
    }


    /**
     * <summary>
     *   Retrieves a longitude sensor for a given identifier.
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
     *   This function does not require that the longitude sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YLongitude.isOnline()</c> to test if the longitude sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a longitude sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the longitude sensor, for instance
     *   <c>YGNSSMK2.longitude</c>.
     * </param>
     * <returns>
     *   a <c>YLongitude</c> object allowing you to drive the longitude sensor.
     * </returns>
     */
    public static YLongitude FindLongitude(string func)
    {
        YLongitude obj;
        lock (YAPI.globalLock) {
            obj = (YLongitude) YFunction._FindFromCache("Longitude", func);
            if (obj == null) {
                obj = new YLongitude(func);
                YFunction._AddToCache("Longitude", func, obj);
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
        this._valueCallbackLongitude = callback;
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
        if (this._valueCallbackLongitude != null) {
            this._valueCallbackLongitude(this, value);
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
     *   arguments: the function object of which the value has changed, and an <c>YMeasure</c> object describing
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
        this._timedReportCallbackLongitude = callback;
        return 0;
    }


    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackLongitude != null) {
            this._timedReportCallbackLongitude(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of longitude sensors started using <c>yFirstLongitude()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned longitude sensors order.
     *   If you want to find a specific a longitude sensor, use <c>Longitude.findLongitude()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YLongitude</c> object, corresponding to
     *   a longitude sensor currently online, or a <c>null</c> pointer
     *   if there are no more longitude sensors to enumerate.
     * </returns>
     */
    public YLongitude nextLongitude()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindLongitude(hwid);
    }

    //--- (end of YLongitude implementation)

    //--- (YLongitude functions)

    /**
     * <summary>
     *   Starts the enumeration of longitude sensors currently accessible.
     * <para>
     *   Use the method <c>YLongitude.nextLongitude()</c> to iterate on
     *   next longitude sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YLongitude</c> object, corresponding to
     *   the first longitude sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YLongitude FirstLongitude()
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
        err = YAPI.apiGetFunctionsByClass("Longitude", 0, p, size, ref neededsize, ref errmsg);
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
        return FindLongitude(serial + "." + funcId);
    }



    //--- (end of YLongitude functions)
}
#pragma warning restore 1591
