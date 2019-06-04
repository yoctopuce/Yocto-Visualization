/*********************************************************************
 *
 *  $Id: yocto_voc.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindVoc(), the high-level API for Voc functions
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
    //--- (YVoc return codes)
    //--- (end of YVoc return codes)
//--- (YVoc dlldef)
//--- (end of YVoc dlldef)
//--- (YVoc yapiwrapper)
//--- (end of YVoc yapiwrapper)
//--- (YVoc class start)
/**
 * <summary>
 *   The Yoctopuce class YVoc allows you to read and configure Yoctopuce Volatile Organic
 *   Compound sensors.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YVoc : YSensor
{
//--- (end of YVoc class start)
    //--- (YVoc definitions)
    public new delegate void ValueCallback(YVoc func, string value);
    public new delegate void TimedReportCallback(YVoc func, YMeasure measure);

    protected ValueCallback _valueCallbackVoc = null;
    protected TimedReportCallback _timedReportCallbackVoc = null;
    //--- (end of YVoc definitions)

    public YVoc(string func)
        : base(func)
    {
        _className = "Voc";
        //--- (YVoc attributes initialization)
        //--- (end of YVoc attributes initialization)
    }

    //--- (YVoc implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Retrieves a Volatile Organic Compound sensor for a given identifier.
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
     *   This function does not require that the Volatile Organic Compound sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YVoc.isOnline()</c> to test if the Volatile Organic Compound sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a Volatile Organic Compound sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the Volatile Organic Compound sensor
     * </param>
     * <returns>
     *   a <c>YVoc</c> object allowing you to drive the Volatile Organic Compound sensor.
     * </returns>
     */
    public static YVoc FindVoc(string func)
    {
        YVoc obj;
        lock (YAPI.globalLock) {
            obj = (YVoc) YFunction._FindFromCache("Voc", func);
            if (obj == null) {
                obj = new YVoc(func);
                YFunction._AddToCache("Voc", func, obj);
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
        this._valueCallbackVoc = callback;
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
        if (this._valueCallbackVoc != null) {
            this._valueCallbackVoc(this, value);
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
        this._timedReportCallbackVoc = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackVoc != null) {
            this._timedReportCallbackVoc(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of Volatile Organic Compound sensors started using <c>yFirstVoc()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned Volatile Organic Compound sensors order.
     *   If you want to find a specific a Volatile Organic Compound sensor, use <c>Voc.findVoc()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YVoc</c> object, corresponding to
     *   a Volatile Organic Compound sensor currently online, or a <c>null</c> pointer
     *   if there are no more Volatile Organic Compound sensors to enumerate.
     * </returns>
     */
    public YVoc nextVoc()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindVoc(hwid);
    }

    //--- (end of YVoc implementation)

    //--- (YVoc functions)

    /**
     * <summary>
     *   Starts the enumeration of Volatile Organic Compound sensors currently accessible.
     * <para>
     *   Use the method <c>YVoc.nextVoc()</c> to iterate on
     *   next Volatile Organic Compound sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YVoc</c> object, corresponding to
     *   the first Volatile Organic Compound sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YVoc FirstVoc()
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
        err = YAPI.apiGetFunctionsByClass("Voc", 0, p, size, ref neededsize, ref errmsg);
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
        return FindVoc(serial + "." + funcId);
    }



    //--- (end of YVoc functions)
}
#pragma warning restore 1591
