/*********************************************************************
 *
 *  $Id: yocto_groundspeed.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindGroundSpeed(), the high-level API for GroundSpeed functions
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
    //--- (YGroundSpeed return codes)
    //--- (end of YGroundSpeed return codes)
//--- (YGroundSpeed dlldef)
//--- (end of YGroundSpeed dlldef)
//--- (YGroundSpeed yapiwrapper)
//--- (end of YGroundSpeed yapiwrapper)
//--- (YGroundSpeed class start)
/**
 * <summary>
 *   The Yoctopuce class YGroundSpeed allows you to read the ground speed from Yoctopuce
 *   geolocation sensors.
 * <para>
 *   It inherits from the YSensor class the core functions to
 *   read measurements, register callback functions, access the autonomous
 *   datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YGroundSpeed : YSensor
{
//--- (end of YGroundSpeed class start)
    //--- (YGroundSpeed definitions)
    public new delegate void ValueCallback(YGroundSpeed func, string value);
    public new delegate void TimedReportCallback(YGroundSpeed func, YMeasure measure);

    protected ValueCallback _valueCallbackGroundSpeed = null;
    protected TimedReportCallback _timedReportCallbackGroundSpeed = null;
    //--- (end of YGroundSpeed definitions)

    public YGroundSpeed(string func)
        : base(func)
    {
        _className = "GroundSpeed";
        //--- (YGroundSpeed attributes initialization)
        //--- (end of YGroundSpeed attributes initialization)
    }

    //--- (YGroundSpeed implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Retrieves a ground speed sensor for a given identifier.
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
     *   This function does not require that the ground speed sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YGroundSpeed.isOnline()</c> to test if the ground speed sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a ground speed sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the ground speed sensor
     * </param>
     * <returns>
     *   a <c>YGroundSpeed</c> object allowing you to drive the ground speed sensor.
     * </returns>
     */
    public static YGroundSpeed FindGroundSpeed(string func)
    {
        YGroundSpeed obj;
        lock (YAPI.globalLock) {
            obj = (YGroundSpeed) YFunction._FindFromCache("GroundSpeed", func);
            if (obj == null) {
                obj = new YGroundSpeed(func);
                YFunction._AddToCache("GroundSpeed", func, obj);
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
        this._valueCallbackGroundSpeed = callback;
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
        if (this._valueCallbackGroundSpeed != null) {
            this._valueCallbackGroundSpeed(this, value);
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
        this._timedReportCallbackGroundSpeed = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackGroundSpeed != null) {
            this._timedReportCallbackGroundSpeed(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of ground speed sensors started using <c>yFirstGroundSpeed()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned ground speed sensors order.
     *   If you want to find a specific a ground speed sensor, use <c>GroundSpeed.findGroundSpeed()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YGroundSpeed</c> object, corresponding to
     *   a ground speed sensor currently online, or a <c>null</c> pointer
     *   if there are no more ground speed sensors to enumerate.
     * </returns>
     */
    public YGroundSpeed nextGroundSpeed()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindGroundSpeed(hwid);
    }

    //--- (end of YGroundSpeed implementation)

    //--- (YGroundSpeed functions)

    /**
     * <summary>
     *   Starts the enumeration of ground speed sensors currently accessible.
     * <para>
     *   Use the method <c>YGroundSpeed.nextGroundSpeed()</c> to iterate on
     *   next ground speed sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YGroundSpeed</c> object, corresponding to
     *   the first ground speed sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YGroundSpeed FirstGroundSpeed()
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
        err = YAPI.apiGetFunctionsByClass("GroundSpeed", 0, p, size, ref neededsize, ref errmsg);
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
        return FindGroundSpeed(serial + "." + funcId);
    }



    //--- (end of YGroundSpeed functions)
}
#pragma warning restore 1591
