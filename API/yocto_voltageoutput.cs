/*********************************************************************
 *
 *  $Id: yocto_voltageoutput.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindVoltageOutput(), the high-level API for VoltageOutput functions
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
    //--- (YVoltageOutput return codes)
    //--- (end of YVoltageOutput return codes)
//--- (YVoltageOutput dlldef)
//--- (end of YVoltageOutput dlldef)
//--- (YVoltageOutput yapiwrapper)
//--- (end of YVoltageOutput yapiwrapper)
//--- (YVoltageOutput class start)
/**
 * <summary>
 *   The Yoctopuce application programming interface allows you to change the value of the voltage output.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YVoltageOutput : YFunction
{
//--- (end of YVoltageOutput class start)
    //--- (YVoltageOutput definitions)
    public new delegate void ValueCallback(YVoltageOutput func, string value);
    public new delegate void TimedReportCallback(YVoltageOutput func, YMeasure measure);

    public const double CURRENTVOLTAGE_INVALID = YAPI.INVALID_DOUBLE;
    public const string VOLTAGETRANSITION_INVALID = YAPI.INVALID_STRING;
    public const double VOLTAGEATSTARTUP_INVALID = YAPI.INVALID_DOUBLE;
    protected double _currentVoltage = CURRENTVOLTAGE_INVALID;
    protected string _voltageTransition = VOLTAGETRANSITION_INVALID;
    protected double _voltageAtStartUp = VOLTAGEATSTARTUP_INVALID;
    protected ValueCallback _valueCallbackVoltageOutput = null;
    //--- (end of YVoltageOutput definitions)

    public YVoltageOutput(string func)
        : base(func)
    {
        _className = "VoltageOutput";
        //--- (YVoltageOutput attributes initialization)
        //--- (end of YVoltageOutput attributes initialization)
    }

    //--- (YVoltageOutput implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("currentVoltage"))
        {
            _currentVoltage = Math.Round(json_val.getDouble("currentVoltage") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("voltageTransition"))
        {
            _voltageTransition = json_val.getString("voltageTransition");
        }
        if (json_val.has("voltageAtStartUp"))
        {
            _voltageAtStartUp = Math.Round(json_val.getDouble("voltageAtStartUp") * 1000.0 / 65536.0) / 1000.0;
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the output voltage, in V.
     * <para>
     *   Valid range is from 0 to 10V.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the output voltage, in V
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
    public int set_currentVoltage(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("currentVoltage", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the output voltage set point, in V.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the output voltage set point, in V
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YVoltageOutput.CURRENTVOLTAGE_INVALID</c>.
     * </para>
     */
    public double get_currentVoltage()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENTVOLTAGE_INVALID;
                }
            }
            res = this._currentVoltage;
        }
        return res;
    }

    public string get_voltageTransition()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VOLTAGETRANSITION_INVALID;
                }
            }
            res = this._voltageTransition;
        }
        return res;
    }

    public int set_voltageTransition(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("voltageTransition", rest_val);
        }
    }

    /**
     * <summary>
     *   Changes the output voltage at device start up.
     * <para>
     *   Remember to call the matching
     *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the output voltage at device start up
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
    public int set_voltageAtStartUp(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("voltageAtStartUp", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the selected voltage output at device startup, in V.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the selected voltage output at device startup, in V
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YVoltageOutput.VOLTAGEATSTARTUP_INVALID</c>.
     * </para>
     */
    public double get_voltageAtStartUp()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VOLTAGEATSTARTUP_INVALID;
                }
            }
            res = this._voltageAtStartUp;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves a voltage output for a given identifier.
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
     *   This function does not require that the voltage output is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YVoltageOutput.isOnline()</c> to test if the voltage output is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a voltage output by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the voltage output
     * </param>
     * <returns>
     *   a <c>YVoltageOutput</c> object allowing you to drive the voltage output.
     * </returns>
     */
    public static YVoltageOutput FindVoltageOutput(string func)
    {
        YVoltageOutput obj;
        lock (YAPI.globalLock) {
            obj = (YVoltageOutput) YFunction._FindFromCache("VoltageOutput", func);
            if (obj == null) {
                obj = new YVoltageOutput(func);
                YFunction._AddToCache("VoltageOutput", func, obj);
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
        this._valueCallbackVoltageOutput = callback;
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
        if (this._valueCallbackVoltageOutput != null) {
            this._valueCallbackVoltageOutput(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Performs a smooth transition of output voltage.
     * <para>
     *   Any explicit voltage
     *   change cancels any ongoing transition process.
     * </para>
     * </summary>
     * <param name="V_target">
     *   new output voltage value at the end of the transition
     *   (floating-point number, representing the end voltage in V)
     * </param>
     * <param name="ms_duration">
     *   total duration of the transition, in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     */
    public virtual int voltageMove(double V_target, int ms_duration)
    {
        string newval;
        if (V_target < 0.0) {
            V_target  = 0.0;
        }
        if (V_target > 10.0) {
            V_target = 10.0;
        }
        newval = ""+Convert.ToString( (int) Math.Round(V_target*65536))+":"+Convert.ToString(ms_duration);

        return this.set_voltageTransition(newval);
    }

    /**
     * <summary>
     *   Continues the enumeration of voltage outputs started using <c>yFirstVoltageOutput()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned voltage outputs order.
     *   If you want to find a specific a voltage output, use <c>VoltageOutput.findVoltageOutput()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YVoltageOutput</c> object, corresponding to
     *   a voltage output currently online, or a <c>null</c> pointer
     *   if there are no more voltage outputs to enumerate.
     * </returns>
     */
    public YVoltageOutput nextVoltageOutput()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindVoltageOutput(hwid);
    }

    //--- (end of YVoltageOutput implementation)

    //--- (YVoltageOutput functions)

    /**
     * <summary>
     *   Starts the enumeration of voltage outputs currently accessible.
     * <para>
     *   Use the method <c>YVoltageOutput.nextVoltageOutput()</c> to iterate on
     *   next voltage outputs.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YVoltageOutput</c> object, corresponding to
     *   the first voltage output currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YVoltageOutput FirstVoltageOutput()
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
        err = YAPI.apiGetFunctionsByClass("VoltageOutput", 0, p, size, ref neededsize, ref errmsg);
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
        return FindVoltageOutput(serial + "." + funcId);
    }



    //--- (end of YVoltageOutput functions)
}
#pragma warning restore 1591
