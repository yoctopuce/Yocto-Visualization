/*********************************************************************
 *
 *  $Id: yocto_dualpower.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements yFindDualPower(), the high-level API for DualPower functions
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
    //--- (YDualPower return codes)
    //--- (end of YDualPower return codes)
//--- (YDualPower dlldef)
//--- (end of YDualPower dlldef)
//--- (YDualPower yapiwrapper)
//--- (end of YDualPower yapiwrapper)
//--- (YDualPower class start)
/**
 * <summary>
 *   The <c>YDualPower</c> class allows you to control
 *   the power source to use for module functions that require high current.
 * <para>
 *   The module can also automatically disconnect the external power
 *   when a voltage drop is observed on the external power source
 *   (external battery running out of power).
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YDualPower : YFunction
{
//--- (end of YDualPower class start)
    //--- (YDualPower definitions)
    public new delegate void ValueCallback(YDualPower func, string value);
    public new delegate void TimedReportCallback(YDualPower func, YMeasure measure);

    public const int POWERSTATE_OFF = 0;
    public const int POWERSTATE_FROM_USB = 1;
    public const int POWERSTATE_FROM_EXT = 2;
    public const int POWERSTATE_INVALID = -1;
    public const int POWERCONTROL_AUTO = 0;
    public const int POWERCONTROL_FROM_USB = 1;
    public const int POWERCONTROL_FROM_EXT = 2;
    public const int POWERCONTROL_OFF = 3;
    public const int POWERCONTROL_INVALID = -1;
    public const int EXTVOLTAGE_INVALID = YAPI.INVALID_UINT;
    protected int _powerState = POWERSTATE_INVALID;
    protected int _powerControl = POWERCONTROL_INVALID;
    protected int _extVoltage = EXTVOLTAGE_INVALID;
    protected ValueCallback _valueCallbackDualPower = null;
    //--- (end of YDualPower definitions)

    public YDualPower(string func)
        : base(func)
    {
        _className = "DualPower";
        //--- (YDualPower attributes initialization)
        //--- (end of YDualPower attributes initialization)
    }

    //--- (YDualPower implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("powerState"))
        {
            _powerState = json_val.getInt("powerState");
        }
        if (json_val.has("powerControl"))
        {
            _powerControl = json_val.getInt("powerControl");
        }
        if (json_val.has("extVoltage"))
        {
            _extVoltage = json_val.getInt("extVoltage");
        }
        base._parseAttr(json_val);
    }


    /**
     * <summary>
     *   Returns the current power source for module functions that require lots of current.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YDualPower.POWERSTATE_OFF</c>, <c>YDualPower.POWERSTATE_FROM_USB</c> and
     *   <c>YDualPower.POWERSTATE_FROM_EXT</c> corresponding to the current power source for module
     *   functions that require lots of current
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDualPower.POWERSTATE_INVALID</c>.
     * </para>
     */
    public int get_powerState()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return POWERSTATE_INVALID;
                }
            }
            res = this._powerState;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the selected power source for module functions that require lots of current.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YDualPower.POWERCONTROL_AUTO</c>, <c>YDualPower.POWERCONTROL_FROM_USB</c>,
     *   <c>YDualPower.POWERCONTROL_FROM_EXT</c> and <c>YDualPower.POWERCONTROL_OFF</c> corresponding to the
     *   selected power source for module functions that require lots of current
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDualPower.POWERCONTROL_INVALID</c>.
     * </para>
     */
    public int get_powerControl()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return POWERCONTROL_INVALID;
                }
            }
            res = this._powerControl;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the selected power source for module functions that require lots of current.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YDualPower.POWERCONTROL_AUTO</c>, <c>YDualPower.POWERCONTROL_FROM_USB</c>,
     *   <c>YDualPower.POWERCONTROL_FROM_EXT</c> and <c>YDualPower.POWERCONTROL_OFF</c> corresponding to the
     *   selected power source for module functions that require lots of current
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
    public int set_powerControl(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("powerControl", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the measured voltage on the external power source, in millivolts.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the measured voltage on the external power source, in millivolts
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDualPower.EXTVOLTAGE_INVALID</c>.
     * </para>
     */
    public int get_extVoltage()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return EXTVOLTAGE_INVALID;
                }
            }
            res = this._extVoltage;
        }
        return res;
    }


    /**
     * <summary>
     *   Retrieves a dual power switch for a given identifier.
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
     *   This function does not require that the dual power switch is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YDualPower.isOnline()</c> to test if the dual power switch is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a dual power switch by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the dual power switch, for instance
     *   <c>SERVORC1.dualPower</c>.
     * </param>
     * <returns>
     *   a <c>YDualPower</c> object allowing you to drive the dual power switch.
     * </returns>
     */
    public static YDualPower FindDualPower(string func)
    {
        YDualPower obj;
        lock (YAPI.globalLock) {
            obj = (YDualPower) YFunction._FindFromCache("DualPower", func);
            if (obj == null) {
                obj = new YDualPower(func);
                YFunction._AddToCache("DualPower", func, obj);
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
        this._valueCallbackDualPower = callback;
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
        if (this._valueCallbackDualPower != null) {
            this._valueCallbackDualPower(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of dual power switches started using <c>yFirstDualPower()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned dual power switches order.
     *   If you want to find a specific a dual power switch, use <c>DualPower.findDualPower()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YDualPower</c> object, corresponding to
     *   a dual power switch currently online, or a <c>null</c> pointer
     *   if there are no more dual power switches to enumerate.
     * </returns>
     */
    public YDualPower nextDualPower()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindDualPower(hwid);
    }

    //--- (end of YDualPower implementation)

    //--- (YDualPower functions)

    /**
     * <summary>
     *   Starts the enumeration of dual power switches currently accessible.
     * <para>
     *   Use the method <c>YDualPower.nextDualPower()</c> to iterate on
     *   next dual power switches.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YDualPower</c> object, corresponding to
     *   the first dual power switch currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YDualPower FirstDualPower()
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
        err = YAPI.apiGetFunctionsByClass("DualPower", 0, p, size, ref neededsize, ref errmsg);
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
        return FindDualPower(serial + "." + funcId);
    }



    //--- (end of YDualPower functions)
}
#pragma warning restore 1591
