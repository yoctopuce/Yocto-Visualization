/*********************************************************************
 *
 *  $Id: yocto_hubport.cs 38899 2019-12-20 17:21:03Z mvuilleu $
 *
 *  Implements yFindHubPort(), the high-level API for HubPort functions
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
    //--- (YHubPort return codes)
    //--- (end of YHubPort return codes)
//--- (YHubPort dlldef)
//--- (end of YHubPort dlldef)
//--- (YHubPort yapiwrapper)
//--- (end of YHubPort yapiwrapper)
//--- (YHubPort class start)
/**
 * <summary>
 *   The <c>YHubPort</c> class provides control over the power supply for slave ports
 *   on a YoctoHub.
 * <para>
 *   It provide information about the device connected to it.
 *   The logical name of a YHubPort is always automatically set to the
 *   unique serial number of the Yoctopuce device connected to it.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YHubPort : YFunction
{
//--- (end of YHubPort class start)
    //--- (YHubPort definitions)
    public new delegate void ValueCallback(YHubPort func, string value);
    public new delegate void TimedReportCallback(YHubPort func, YMeasure measure);

    public const int ENABLED_FALSE = 0;
    public const int ENABLED_TRUE = 1;
    public const int ENABLED_INVALID = -1;
    public const int PORTSTATE_OFF = 0;
    public const int PORTSTATE_OVRLD = 1;
    public const int PORTSTATE_ON = 2;
    public const int PORTSTATE_RUN = 3;
    public const int PORTSTATE_PROG = 4;
    public const int PORTSTATE_INVALID = -1;
    public const int BAUDRATE_INVALID = YAPI.INVALID_UINT;
    protected int _enabled = ENABLED_INVALID;
    protected int _portState = PORTSTATE_INVALID;
    protected int _baudRate = BAUDRATE_INVALID;
    protected ValueCallback _valueCallbackHubPort = null;
    //--- (end of YHubPort definitions)

    public YHubPort(string func)
        : base(func)
    {
        _className = "HubPort";
        //--- (YHubPort attributes initialization)
        //--- (end of YHubPort attributes initialization)
    }

    //--- (YHubPort implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("enabled"))
        {
            _enabled = json_val.getInt("enabled") > 0 ? 1 : 0;
        }
        if (json_val.has("portState"))
        {
            _portState = json_val.getInt("portState");
        }
        if (json_val.has("baudRate"))
        {
            _baudRate = json_val.getInt("baudRate");
        }
        base._parseAttr(json_val);
    }


    /**
     * <summary>
     *   Returns true if the YoctoHub port is powered, false otherwise.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YHubPort.ENABLED_FALSE</c> or <c>YHubPort.ENABLED_TRUE</c>, according to true if the
     *   YoctoHub port is powered, false otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YHubPort.ENABLED_INVALID</c>.
     * </para>
     */
    public int get_enabled()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ENABLED_INVALID;
                }
            }
            res = this._enabled;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the activation of the YoctoHub port.
     * <para>
     *   If the port is enabled, the
     *   connected module is powered. Otherwise, port power is shut down.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YHubPort.ENABLED_FALSE</c> or <c>YHubPort.ENABLED_TRUE</c>, according to the activation
     *   of the YoctoHub port
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
    public int set_enabled(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("enabled", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the current state of the YoctoHub port.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YHubPort.PORTSTATE_OFF</c>, <c>YHubPort.PORTSTATE_OVRLD</c>,
     *   <c>YHubPort.PORTSTATE_ON</c>, <c>YHubPort.PORTSTATE_RUN</c> and <c>YHubPort.PORTSTATE_PROG</c>
     *   corresponding to the current state of the YoctoHub port
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YHubPort.PORTSTATE_INVALID</c>.
     * </para>
     */
    public int get_portState()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PORTSTATE_INVALID;
                }
            }
            res = this._portState;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the current baud rate used by this YoctoHub port, in kbps.
     * <para>
     *   The default value is 1000 kbps, but a slower rate may be used if communication
     *   problems are encountered.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current baud rate used by this YoctoHub port, in kbps
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YHubPort.BAUDRATE_INVALID</c>.
     * </para>
     */
    public int get_baudRate()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BAUDRATE_INVALID;
                }
            }
            res = this._baudRate;
        }
        return res;
    }


    /**
     * <summary>
     *   Retrieves a YoctoHub slave port for a given identifier.
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
     *   This function does not require that the YoctoHub slave port is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YHubPort.isOnline()</c> to test if the YoctoHub slave port is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a YoctoHub slave port by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the YoctoHub slave port, for instance
     *   <c>YHUBETH1.hubPort1</c>.
     * </param>
     * <returns>
     *   a <c>YHubPort</c> object allowing you to drive the YoctoHub slave port.
     * </returns>
     */
    public static YHubPort FindHubPort(string func)
    {
        YHubPort obj;
        lock (YAPI.globalLock) {
            obj = (YHubPort) YFunction._FindFromCache("HubPort", func);
            if (obj == null) {
                obj = new YHubPort(func);
                YFunction._AddToCache("HubPort", func, obj);
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
        this._valueCallbackHubPort = callback;
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
        if (this._valueCallbackHubPort != null) {
            this._valueCallbackHubPort(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of YoctoHub slave ports started using <c>yFirstHubPort()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned YoctoHub slave ports order.
     *   If you want to find a specific a YoctoHub slave port, use <c>HubPort.findHubPort()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YHubPort</c> object, corresponding to
     *   a YoctoHub slave port currently online, or a <c>null</c> pointer
     *   if there are no more YoctoHub slave ports to enumerate.
     * </returns>
     */
    public YHubPort nextHubPort()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindHubPort(hwid);
    }

    //--- (end of YHubPort implementation)

    //--- (YHubPort functions)

    /**
     * <summary>
     *   Starts the enumeration of YoctoHub slave ports currently accessible.
     * <para>
     *   Use the method <c>YHubPort.nextHubPort()</c> to iterate on
     *   next YoctoHub slave ports.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YHubPort</c> object, corresponding to
     *   the first YoctoHub slave port currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YHubPort FirstHubPort()
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
        err = YAPI.apiGetFunctionsByClass("HubPort", 0, p, size, ref neededsize, ref errmsg);
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
        return FindHubPort(serial + "." + funcId);
    }



    //--- (end of YHubPort functions)
}
#pragma warning restore 1591
