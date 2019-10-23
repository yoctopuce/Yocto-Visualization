/*********************************************************************
 *
 *  $Id: yocto_pwminput.cs 37149 2019-09-12 21:24:53Z mvuilleu $
 *
 *  Implements yFindPwmInput(), the high-level API for PwmInput functions
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
    //--- (YPwmInput return codes)
    //--- (end of YPwmInput return codes)
//--- (YPwmInput dlldef)
//--- (end of YPwmInput dlldef)
//--- (YPwmInput yapiwrapper)
//--- (end of YPwmInput yapiwrapper)
//--- (YPwmInput class start)
/**
 * <summary>
 *   The Yoctopuce class YPwmInput allows you to read and configure Yoctopuce PWM
 *   sensors.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 *   This class adds the ability to configure the signal parameter used to transmit
 *   information: the duty cycle, the frequency or the pulse width.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YPwmInput : YSensor
{
//--- (end of YPwmInput class start)
    //--- (YPwmInput definitions)
    public new delegate void ValueCallback(YPwmInput func, string value);
    public new delegate void TimedReportCallback(YPwmInput func, YMeasure measure);

    public const double DUTYCYCLE_INVALID = YAPI.INVALID_DOUBLE;
    public const double PULSEDURATION_INVALID = YAPI.INVALID_DOUBLE;
    public const double FREQUENCY_INVALID = YAPI.INVALID_DOUBLE;
    public const double PERIOD_INVALID = YAPI.INVALID_DOUBLE;
    public const long PULSECOUNTER_INVALID = YAPI.INVALID_LONG;
    public const long PULSETIMER_INVALID = YAPI.INVALID_LONG;
    public const int PWMREPORTMODE_PWM_DUTYCYCLE = 0;
    public const int PWMREPORTMODE_PWM_FREQUENCY = 1;
    public const int PWMREPORTMODE_PWM_PULSEDURATION = 2;
    public const int PWMREPORTMODE_PWM_EDGECOUNT = 3;
    public const int PWMREPORTMODE_PWM_PULSECOUNT = 4;
    public const int PWMREPORTMODE_PWM_CPS = 5;
    public const int PWMREPORTMODE_PWM_CPM = 6;
    public const int PWMREPORTMODE_PWM_STATE = 7;
    public const int PWMREPORTMODE_PWM_FREQ_CPS = 8;
    public const int PWMREPORTMODE_PWM_FREQ_CPM = 9;
    public const int PWMREPORTMODE_INVALID = -1;
    public const int DEBOUNCEPERIOD_INVALID = YAPI.INVALID_UINT;
    protected double _dutyCycle = DUTYCYCLE_INVALID;
    protected double _pulseDuration = PULSEDURATION_INVALID;
    protected double _frequency = FREQUENCY_INVALID;
    protected double _period = PERIOD_INVALID;
    protected long _pulseCounter = PULSECOUNTER_INVALID;
    protected long _pulseTimer = PULSETIMER_INVALID;
    protected int _pwmReportMode = PWMREPORTMODE_INVALID;
    protected int _debouncePeriod = DEBOUNCEPERIOD_INVALID;
    protected ValueCallback _valueCallbackPwmInput = null;
    protected TimedReportCallback _timedReportCallbackPwmInput = null;
    //--- (end of YPwmInput definitions)

    public YPwmInput(string func)
        : base(func)
    {
        _className = "PwmInput";
        //--- (YPwmInput attributes initialization)
        //--- (end of YPwmInput attributes initialization)
    }

    //--- (YPwmInput implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("dutyCycle"))
        {
            _dutyCycle = Math.Round(json_val.getDouble("dutyCycle") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("pulseDuration"))
        {
            _pulseDuration = Math.Round(json_val.getDouble("pulseDuration") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("frequency"))
        {
            _frequency = Math.Round(json_val.getDouble("frequency") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("period"))
        {
            _period = Math.Round(json_val.getDouble("period") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("pulseCounter"))
        {
            _pulseCounter = json_val.getLong("pulseCounter");
        }
        if (json_val.has("pulseTimer"))
        {
            _pulseTimer = json_val.getLong("pulseTimer");
        }
        if (json_val.has("pwmReportMode"))
        {
            _pwmReportMode = json_val.getInt("pwmReportMode");
        }
        if (json_val.has("debouncePeriod"))
        {
            _debouncePeriod = json_val.getInt("debouncePeriod");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the measuring unit for the measured quantity.
     * <para>
     *   That unit
     *   is just a string which is automatically initialized each time
     *   the measurement mode is changed. But is can be set to an
     *   arbitrary value.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the measuring unit for the measured quantity
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
     *   Returns the PWM duty cycle, in per cents.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWM duty cycle, in per cents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmInput.DUTYCYCLE_INVALID</c>.
     * </para>
     */
    public double get_dutyCycle()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DUTYCYCLE_INVALID;
                }
            }
            res = this._dutyCycle;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the PWM pulse length in milliseconds, as a floating point number.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWM pulse length in milliseconds, as a floating point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmInput.PULSEDURATION_INVALID</c>.
     * </para>
     */
    public double get_pulseDuration()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PULSEDURATION_INVALID;
                }
            }
            res = this._pulseDuration;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the PWM frequency in Hz.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWM frequency in Hz
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmInput.FREQUENCY_INVALID</c>.
     * </para>
     */
    public double get_frequency()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return FREQUENCY_INVALID;
                }
            }
            res = this._frequency;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the PWM period in milliseconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWM period in milliseconds
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmInput.PERIOD_INVALID</c>.
     * </para>
     */
    public double get_period()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PERIOD_INVALID;
                }
            }
            res = this._period;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the pulse counter value.
     * <para>
     *   Actually that
     *   counter is incremented twice per period. That counter is
     *   limited  to 1 billion.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the pulse counter value
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmInput.PULSECOUNTER_INVALID</c>.
     * </para>
     */
    public long get_pulseCounter()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PULSECOUNTER_INVALID;
                }
            }
            res = this._pulseCounter;
        }
        return res;
    }

    public int set_pulseCounter(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("pulseCounter", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the timer of the pulses counter (ms).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the timer of the pulses counter (ms)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmInput.PULSETIMER_INVALID</c>.
     * </para>
     */
    public long get_pulseTimer()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PULSETIMER_INVALID;
                }
            }
            res = this._pulseTimer;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the parameter (frequency/duty cycle, pulse width, edges count) returned by the get_currentValue function and callbacks.
     * <para>
     *   Attention
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YPwmInput.PWMREPORTMODE_PWM_DUTYCYCLE</c>, <c>YPwmInput.PWMREPORTMODE_PWM_FREQUENCY</c>,
     *   <c>YPwmInput.PWMREPORTMODE_PWM_PULSEDURATION</c>, <c>YPwmInput.PWMREPORTMODE_PWM_EDGECOUNT</c>,
     *   <c>YPwmInput.PWMREPORTMODE_PWM_PULSECOUNT</c>, <c>YPwmInput.PWMREPORTMODE_PWM_CPS</c>,
     *   <c>YPwmInput.PWMREPORTMODE_PWM_CPM</c>, <c>YPwmInput.PWMREPORTMODE_PWM_STATE</c>,
     *   <c>YPwmInput.PWMREPORTMODE_PWM_FREQ_CPS</c> and <c>YPwmInput.PWMREPORTMODE_PWM_FREQ_CPM</c>
     *   corresponding to the parameter (frequency/duty cycle, pulse width, edges count) returned by the
     *   get_currentValue function and callbacks
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmInput.PWMREPORTMODE_INVALID</c>.
     * </para>
     */
    public int get_pwmReportMode()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PWMREPORTMODE_INVALID;
                }
            }
            res = this._pwmReportMode;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the  parameter  type (frequency/duty cycle, pulse width, or edge count) returned by the get_currentValue function and callbacks.
     * <para>
     *   The edge count value is limited to the 6 lowest digits. For values greater than one million, use
     *   get_pulseCounter().
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YPwmInput.PWMREPORTMODE_PWM_DUTYCYCLE</c>, <c>YPwmInput.PWMREPORTMODE_PWM_FREQUENCY</c>,
     *   <c>YPwmInput.PWMREPORTMODE_PWM_PULSEDURATION</c>, <c>YPwmInput.PWMREPORTMODE_PWM_EDGECOUNT</c>,
     *   <c>YPwmInput.PWMREPORTMODE_PWM_PULSECOUNT</c>, <c>YPwmInput.PWMREPORTMODE_PWM_CPS</c>,
     *   <c>YPwmInput.PWMREPORTMODE_PWM_CPM</c>, <c>YPwmInput.PWMREPORTMODE_PWM_STATE</c>,
     *   <c>YPwmInput.PWMREPORTMODE_PWM_FREQ_CPS</c> and <c>YPwmInput.PWMREPORTMODE_PWM_FREQ_CPM</c>
     *   corresponding to the  parameter  type (frequency/duty cycle, pulse width, or edge count) returned
     *   by the get_currentValue function and callbacks
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
    public int set_pwmReportMode(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("pwmReportMode", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the shortest expected pulse duration, in ms.
     * <para>
     *   Any shorter pulse will be automatically ignored (debounce).
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the shortest expected pulse duration, in ms
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmInput.DEBOUNCEPERIOD_INVALID</c>.
     * </para>
     */
    public int get_debouncePeriod()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DEBOUNCEPERIOD_INVALID;
                }
            }
            res = this._debouncePeriod;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the shortest expected pulse duration, in ms.
     * <para>
     *   Any shorter pulse will be automatically ignored (debounce).
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the shortest expected pulse duration, in ms
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
    public int set_debouncePeriod(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("debouncePeriod", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a PWM input for a given identifier.
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
     *   This function does not require that the PWM input is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YPwmInput.isOnline()</c> to test if the PWM input is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a PWM input by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the PWM input
     * </param>
     * <returns>
     *   a <c>YPwmInput</c> object allowing you to drive the PWM input.
     * </returns>
     */
    public static YPwmInput FindPwmInput(string func)
    {
        YPwmInput obj;
        lock (YAPI.globalLock) {
            obj = (YPwmInput) YFunction._FindFromCache("PwmInput", func);
            if (obj == null) {
                obj = new YPwmInput(func);
                YFunction._AddToCache("PwmInput", func, obj);
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
        this._valueCallbackPwmInput = callback;
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
        if (this._valueCallbackPwmInput != null) {
            this._valueCallbackPwmInput(this, value);
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
        this._timedReportCallbackPwmInput = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackPwmInput != null) {
            this._timedReportCallbackPwmInput(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Returns the pulse counter value as well as its timer.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int resetCounter()
    {
        return this.set_pulseCounter(0);
    }

    /**
     * <summary>
     *   Continues the enumeration of PWM inputs started using <c>yFirstPwmInput()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned PWM inputs order.
     *   If you want to find a specific a PWM input, use <c>PwmInput.findPwmInput()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YPwmInput</c> object, corresponding to
     *   a PWM input currently online, or a <c>null</c> pointer
     *   if there are no more PWM inputs to enumerate.
     * </returns>
     */
    public YPwmInput nextPwmInput()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindPwmInput(hwid);
    }

    //--- (end of YPwmInput implementation)

    //--- (YPwmInput functions)

    /**
     * <summary>
     *   Starts the enumeration of PWM inputs currently accessible.
     * <para>
     *   Use the method <c>YPwmInput.nextPwmInput()</c> to iterate on
     *   next PWM inputs.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YPwmInput</c> object, corresponding to
     *   the first PWM input currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YPwmInput FirstPwmInput()
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
        err = YAPI.apiGetFunctionsByClass("PwmInput", 0, p, size, ref neededsize, ref errmsg);
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
        return FindPwmInput(serial + "." + funcId);
    }



    //--- (end of YPwmInput functions)
}
#pragma warning restore 1591
