/*********************************************************************
 *
 *  $Id: yocto_quadraturedecoder.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindQuadratureDecoder(), the high-level API for QuadratureDecoder functions
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
    //--- (YQuadratureDecoder return codes)
    //--- (end of YQuadratureDecoder return codes)
//--- (YQuadratureDecoder dlldef)
//--- (end of YQuadratureDecoder dlldef)
//--- (YQuadratureDecoder yapiwrapper)
//--- (end of YQuadratureDecoder yapiwrapper)
//--- (YQuadratureDecoder class start)
/**
 * <summary>
 *   The class YQuadratureDecoder allows you to decode a two-wire signal produced by a
 *   quadrature encoder.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YQuadratureDecoder : YSensor
{
//--- (end of YQuadratureDecoder class start)
    //--- (YQuadratureDecoder definitions)
    public new delegate void ValueCallback(YQuadratureDecoder func, string value);
    public new delegate void TimedReportCallback(YQuadratureDecoder func, YMeasure measure);

    public const double SPEED_INVALID = YAPI.INVALID_DOUBLE;
    public const int DECODING_OFF = 0;
    public const int DECODING_ON = 1;
    public const int DECODING_INVALID = -1;
    protected double _speed = SPEED_INVALID;
    protected int _decoding = DECODING_INVALID;
    protected ValueCallback _valueCallbackQuadratureDecoder = null;
    protected TimedReportCallback _timedReportCallbackQuadratureDecoder = null;
    //--- (end of YQuadratureDecoder definitions)

    public YQuadratureDecoder(string func)
        : base(func)
    {
        _className = "QuadratureDecoder";
        //--- (YQuadratureDecoder attributes initialization)
        //--- (end of YQuadratureDecoder attributes initialization)
    }

    //--- (YQuadratureDecoder implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("speed"))
        {
            _speed = Math.Round(json_val.getDouble("speed") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("decoding"))
        {
            _decoding = json_val.getInt("decoding") > 0 ? 1 : 0;
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the current expected position of the quadrature decoder.
     * <para>
     *   Invoking this function implicitly activates the quadrature decoder.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the current expected position of the quadrature decoder
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
     *   Returns the increments frequency, in Hz.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the increments frequency, in Hz
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YQuadratureDecoder.SPEED_INVALID</c>.
     * </para>
     */
    public double get_speed()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SPEED_INVALID;
                }
            }
            res = this._speed;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the current activation state of the quadrature decoder.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YQuadratureDecoder.DECODING_OFF</c> or <c>YQuadratureDecoder.DECODING_ON</c>, according
     *   to the current activation state of the quadrature decoder
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YQuadratureDecoder.DECODING_INVALID</c>.
     * </para>
     */
    public int get_decoding()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DECODING_INVALID;
                }
            }
            res = this._decoding;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the activation state of the quadrature decoder.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YQuadratureDecoder.DECODING_OFF</c> or <c>YQuadratureDecoder.DECODING_ON</c>, according
     *   to the activation state of the quadrature decoder
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
    public int set_decoding(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("decoding", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a quadrature decoder for a given identifier.
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
     *   This function does not require that the quadrature decoder is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YQuadratureDecoder.isOnline()</c> to test if the quadrature decoder is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a quadrature decoder by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the quadrature decoder
     * </param>
     * <returns>
     *   a <c>YQuadratureDecoder</c> object allowing you to drive the quadrature decoder.
     * </returns>
     */
    public static YQuadratureDecoder FindQuadratureDecoder(string func)
    {
        YQuadratureDecoder obj;
        lock (YAPI.globalLock) {
            obj = (YQuadratureDecoder) YFunction._FindFromCache("QuadratureDecoder", func);
            if (obj == null) {
                obj = new YQuadratureDecoder(func);
                YFunction._AddToCache("QuadratureDecoder", func, obj);
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
        this._valueCallbackQuadratureDecoder = callback;
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
        if (this._valueCallbackQuadratureDecoder != null) {
            this._valueCallbackQuadratureDecoder(this, value);
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
        this._timedReportCallbackQuadratureDecoder = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackQuadratureDecoder != null) {
            this._timedReportCallbackQuadratureDecoder(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of quadrature decoders started using <c>yFirstQuadratureDecoder()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned quadrature decoders order.
     *   If you want to find a specific a quadrature decoder, use <c>QuadratureDecoder.findQuadratureDecoder()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YQuadratureDecoder</c> object, corresponding to
     *   a quadrature decoder currently online, or a <c>null</c> pointer
     *   if there are no more quadrature decoders to enumerate.
     * </returns>
     */
    public YQuadratureDecoder nextQuadratureDecoder()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindQuadratureDecoder(hwid);
    }

    //--- (end of YQuadratureDecoder implementation)

    //--- (YQuadratureDecoder functions)

    /**
     * <summary>
     *   Starts the enumeration of quadrature decoders currently accessible.
     * <para>
     *   Use the method <c>YQuadratureDecoder.nextQuadratureDecoder()</c> to iterate on
     *   next quadrature decoders.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YQuadratureDecoder</c> object, corresponding to
     *   the first quadrature decoder currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YQuadratureDecoder FirstQuadratureDecoder()
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
        err = YAPI.apiGetFunctionsByClass("QuadratureDecoder", 0, p, size, ref neededsize, ref errmsg);
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
        return FindQuadratureDecoder(serial + "." + funcId);
    }



    //--- (end of YQuadratureDecoder functions)
}
#pragma warning restore 1591
