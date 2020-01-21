/*********************************************************************
 *
 *  $Id: yocto_currentloopoutput.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements yFindCurrentLoopOutput(), the high-level API for CurrentLoopOutput functions
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
    //--- (YCurrentLoopOutput return codes)
    //--- (end of YCurrentLoopOutput return codes)
//--- (YCurrentLoopOutput dlldef)
//--- (end of YCurrentLoopOutput dlldef)
//--- (YCurrentLoopOutput yapiwrapper)
//--- (end of YCurrentLoopOutput yapiwrapper)
//--- (YCurrentLoopOutput class start)
/**
 * <summary>
 *   The <c>YCurrentLoopOutput</c> class allows you to drive a 4-20mA output
 *   by regulating the current flowing through the current loop.
 * <para>
 *   It can also provide information about the power state of the current loop.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YCurrentLoopOutput : YFunction
{
//--- (end of YCurrentLoopOutput class start)
    //--- (YCurrentLoopOutput definitions)
    public new delegate void ValueCallback(YCurrentLoopOutput func, string value);
    public new delegate void TimedReportCallback(YCurrentLoopOutput func, YMeasure measure);

    public const double CURRENT_INVALID = YAPI.INVALID_DOUBLE;
    public const string CURRENTTRANSITION_INVALID = YAPI.INVALID_STRING;
    public const double CURRENTATSTARTUP_INVALID = YAPI.INVALID_DOUBLE;
    public const int LOOPPOWER_NOPWR = 0;
    public const int LOOPPOWER_LOWPWR = 1;
    public const int LOOPPOWER_POWEROK = 2;
    public const int LOOPPOWER_INVALID = -1;
    protected double _current = CURRENT_INVALID;
    protected string _currentTransition = CURRENTTRANSITION_INVALID;
    protected double _currentAtStartUp = CURRENTATSTARTUP_INVALID;
    protected int _loopPower = LOOPPOWER_INVALID;
    protected ValueCallback _valueCallbackCurrentLoopOutput = null;
    //--- (end of YCurrentLoopOutput definitions)

    public YCurrentLoopOutput(string func)
        : base(func)
    {
        _className = "CurrentLoopOutput";
        //--- (YCurrentLoopOutput attributes initialization)
        //--- (end of YCurrentLoopOutput attributes initialization)
    }

    //--- (YCurrentLoopOutput implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("current"))
        {
            _current = Math.Round(json_val.getDouble("current") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("currentTransition"))
        {
            _currentTransition = json_val.getString("currentTransition");
        }
        if (json_val.has("currentAtStartUp"))
        {
            _currentAtStartUp = Math.Round(json_val.getDouble("currentAtStartUp") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("loopPower"))
        {
            _loopPower = json_val.getInt("loopPower");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the current loop, the valid range is from 3 to 21mA.
     * <para>
     *   If the loop is
     *   not properly powered, the  target current is not reached and
     *   loopPower is set to LOWPWR.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the current loop, the valid range is from 3 to 21mA
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
    public int set_current(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("current", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the loop current set point in mA.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the loop current set point in mA
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCurrentLoopOutput.CURRENT_INVALID</c>.
     * </para>
     */
    public double get_current()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENT_INVALID;
                }
            }
            res = this._current;
        }
        return res;
    }


    public string get_currentTransition()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENTTRANSITION_INVALID;
                }
            }
            res = this._currentTransition;
        }
        return res;
    }

    public int set_currentTransition(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("currentTransition", rest_val);
        }
    }

    /**
     * <summary>
     *   Changes the loop current at device start up.
     * <para>
     *   Remember to call the matching
     *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the loop current at device start up
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
    public int set_currentAtStartUp(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("currentAtStartUp", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the current in the loop at device startup, in mA.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current in the loop at device startup, in mA
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCurrentLoopOutput.CURRENTATSTARTUP_INVALID</c>.
     * </para>
     */
    public double get_currentAtStartUp()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENTATSTARTUP_INVALID;
                }
            }
            res = this._currentAtStartUp;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the loop powerstate.
     * <para>
     *   POWEROK: the loop
     *   is powered. NOPWR: the loop in not powered. LOWPWR: the loop is not
     *   powered enough to maintain the current required (insufficient voltage).
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YCurrentLoopOutput.LOOPPOWER_NOPWR</c>, <c>YCurrentLoopOutput.LOOPPOWER_LOWPWR</c>
     *   and <c>YCurrentLoopOutput.LOOPPOWER_POWEROK</c> corresponding to the loop powerstate
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCurrentLoopOutput.LOOPPOWER_INVALID</c>.
     * </para>
     */
    public int get_loopPower()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LOOPPOWER_INVALID;
                }
            }
            res = this._loopPower;
        }
        return res;
    }


    /**
     * <summary>
     *   Retrieves a 4-20mA output for a given identifier.
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
     *   This function does not require that the 4-20mA output is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YCurrentLoopOutput.isOnline()</c> to test if the 4-20mA output is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a 4-20mA output by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the 4-20mA output, for instance
     *   <c>TX420MA1.currentLoopOutput</c>.
     * </param>
     * <returns>
     *   a <c>YCurrentLoopOutput</c> object allowing you to drive the 4-20mA output.
     * </returns>
     */
    public static YCurrentLoopOutput FindCurrentLoopOutput(string func)
    {
        YCurrentLoopOutput obj;
        lock (YAPI.globalLock) {
            obj = (YCurrentLoopOutput) YFunction._FindFromCache("CurrentLoopOutput", func);
            if (obj == null) {
                obj = new YCurrentLoopOutput(func);
                YFunction._AddToCache("CurrentLoopOutput", func, obj);
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
        this._valueCallbackCurrentLoopOutput = callback;
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
        if (this._valueCallbackCurrentLoopOutput != null) {
            this._valueCallbackCurrentLoopOutput(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Performs a smooth transition of current flowing in the loop.
     * <para>
     *   Any current explicit
     *   change cancels any ongoing transition process.
     * </para>
     * </summary>
     * <param name="mA_target">
     *   new current value at the end of the transition
     *   (floating-point number, representing the end current in mA)
     * </param>
     * <param name="ms_duration">
     *   total duration of the transition, in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     */
    public virtual int currentMove(double mA_target, int ms_duration)
    {
        string newval;
        if (mA_target < 3.0) {
            mA_target  = 3.0;
        }
        if (mA_target > 21.0) {
            mA_target = 21.0;
        }
        newval = ""+Convert.ToString( (int) Math.Round(mA_target*65536))+":"+Convert.ToString(ms_duration);

        return this.set_currentTransition(newval);
    }

    /**
     * <summary>
     *   Continues the enumeration of 4-20mA outputs started using <c>yFirstCurrentLoopOutput()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned 4-20mA outputs order.
     *   If you want to find a specific a 4-20mA output, use <c>CurrentLoopOutput.findCurrentLoopOutput()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCurrentLoopOutput</c> object, corresponding to
     *   a 4-20mA output currently online, or a <c>null</c> pointer
     *   if there are no more 4-20mA outputs to enumerate.
     * </returns>
     */
    public YCurrentLoopOutput nextCurrentLoopOutput()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindCurrentLoopOutput(hwid);
    }

    //--- (end of YCurrentLoopOutput implementation)

    //--- (YCurrentLoopOutput functions)

    /**
     * <summary>
     *   Starts the enumeration of 4-20mA outputs currently accessible.
     * <para>
     *   Use the method <c>YCurrentLoopOutput.nextCurrentLoopOutput()</c> to iterate on
     *   next 4-20mA outputs.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCurrentLoopOutput</c> object, corresponding to
     *   the first 4-20mA output currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YCurrentLoopOutput FirstCurrentLoopOutput()
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
        err = YAPI.apiGetFunctionsByClass("CurrentLoopOutput", 0, p, size, ref neededsize, ref errmsg);
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
        return FindCurrentLoopOutput(serial + "." + funcId);
    }



    //--- (end of YCurrentLoopOutput functions)
}
#pragma warning restore 1591
