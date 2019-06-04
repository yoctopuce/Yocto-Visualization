/*********************************************************************
 *
 *  $Id: yocto_anbutton.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindAnButton(), the high-level API for AnButton functions
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
    //--- (YAnButton return codes)
    //--- (end of YAnButton return codes)
//--- (YAnButton dlldef)
//--- (end of YAnButton dlldef)
//--- (YAnButton yapiwrapper)
//--- (end of YAnButton yapiwrapper)
//--- (YAnButton class start)
/**
 * <summary>
 *   Yoctopuce application programming interface allows you to measure the state
 *   of a simple button as well as to read an analog potentiometer (variable resistance).
 * <para>
 *   This can be use for instance with a continuous rotating knob, a throttle grip
 *   or a joystick. The module is capable to calibrate itself on min and max values,
 *   in order to compute a calibrated value that varies proportionally with the
 *   potentiometer position, regardless of its total resistance.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YAnButton : YFunction
{
//--- (end of YAnButton class start)
    //--- (YAnButton definitions)
    public new delegate void ValueCallback(YAnButton func, string value);
    public new delegate void TimedReportCallback(YAnButton func, YMeasure measure);

    public const int CALIBRATEDVALUE_INVALID = YAPI.INVALID_UINT;
    public const int RAWVALUE_INVALID = YAPI.INVALID_UINT;
    public const int ANALOGCALIBRATION_OFF = 0;
    public const int ANALOGCALIBRATION_ON = 1;
    public const int ANALOGCALIBRATION_INVALID = -1;
    public const int CALIBRATIONMAX_INVALID = YAPI.INVALID_UINT;
    public const int CALIBRATIONMIN_INVALID = YAPI.INVALID_UINT;
    public const int SENSITIVITY_INVALID = YAPI.INVALID_UINT;
    public const int ISPRESSED_FALSE = 0;
    public const int ISPRESSED_TRUE = 1;
    public const int ISPRESSED_INVALID = -1;
    public const long LASTTIMEPRESSED_INVALID = YAPI.INVALID_LONG;
    public const long LASTTIMERELEASED_INVALID = YAPI.INVALID_LONG;
    public const long PULSECOUNTER_INVALID = YAPI.INVALID_LONG;
    public const long PULSETIMER_INVALID = YAPI.INVALID_LONG;
    protected int _calibratedValue = CALIBRATEDVALUE_INVALID;
    protected int _rawValue = RAWVALUE_INVALID;
    protected int _analogCalibration = ANALOGCALIBRATION_INVALID;
    protected int _calibrationMax = CALIBRATIONMAX_INVALID;
    protected int _calibrationMin = CALIBRATIONMIN_INVALID;
    protected int _sensitivity = SENSITIVITY_INVALID;
    protected int _isPressed = ISPRESSED_INVALID;
    protected long _lastTimePressed = LASTTIMEPRESSED_INVALID;
    protected long _lastTimeReleased = LASTTIMERELEASED_INVALID;
    protected long _pulseCounter = PULSECOUNTER_INVALID;
    protected long _pulseTimer = PULSETIMER_INVALID;
    protected ValueCallback _valueCallbackAnButton = null;
    //--- (end of YAnButton definitions)

    public YAnButton(string func)
        : base(func)
    {
        _className = "AnButton";
        //--- (YAnButton attributes initialization)
        //--- (end of YAnButton attributes initialization)
    }

    //--- (YAnButton implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("calibratedValue"))
        {
            _calibratedValue = json_val.getInt("calibratedValue");
        }
        if (json_val.has("rawValue"))
        {
            _rawValue = json_val.getInt("rawValue");
        }
        if (json_val.has("analogCalibration"))
        {
            _analogCalibration = json_val.getInt("analogCalibration") > 0 ? 1 : 0;
        }
        if (json_val.has("calibrationMax"))
        {
            _calibrationMax = json_val.getInt("calibrationMax");
        }
        if (json_val.has("calibrationMin"))
        {
            _calibrationMin = json_val.getInt("calibrationMin");
        }
        if (json_val.has("sensitivity"))
        {
            _sensitivity = json_val.getInt("sensitivity");
        }
        if (json_val.has("isPressed"))
        {
            _isPressed = json_val.getInt("isPressed") > 0 ? 1 : 0;
        }
        if (json_val.has("lastTimePressed"))
        {
            _lastTimePressed = json_val.getLong("lastTimePressed");
        }
        if (json_val.has("lastTimeReleased"))
        {
            _lastTimeReleased = json_val.getLong("lastTimeReleased");
        }
        if (json_val.has("pulseCounter"))
        {
            _pulseCounter = json_val.getLong("pulseCounter");
        }
        if (json_val.has("pulseTimer"))
        {
            _pulseTimer = json_val.getLong("pulseTimer");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the current calibrated input value (between 0 and 1000, included).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current calibrated input value (between 0 and 1000, included)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.CALIBRATEDVALUE_INVALID</c>.
     * </para>
     */
    public int get_calibratedValue()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CALIBRATEDVALUE_INVALID;
                }
            }
            res = this._calibratedValue;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the current measured input value as-is (between 0 and 4095, included).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current measured input value as-is (between 0 and 4095, included)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.RAWVALUE_INVALID</c>.
     * </para>
     */
    public int get_rawValue()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RAWVALUE_INVALID;
                }
            }
            res = this._rawValue;
        }
        return res;
    }

    /**
     * <summary>
     *   Tells if a calibration process is currently ongoing.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YAnButton.ANALOGCALIBRATION_OFF</c> or <c>YAnButton.ANALOGCALIBRATION_ON</c>
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.ANALOGCALIBRATION_INVALID</c>.
     * </para>
     */
    public int get_analogCalibration()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ANALOGCALIBRATION_INVALID;
                }
            }
            res = this._analogCalibration;
        }
        return res;
    }

    /**
     * <summary>
     *   Starts or stops the calibration process.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module at the end of the calibration if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YAnButton.ANALOGCALIBRATION_OFF</c> or <c>YAnButton.ANALOGCALIBRATION_ON</c>
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
    public int set_analogCalibration(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("analogCalibration", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the maximal value measured during the calibration (between 0 and 4095, included).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximal value measured during the calibration (between 0 and 4095, included)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.CALIBRATIONMAX_INVALID</c>.
     * </para>
     */
    public int get_calibrationMax()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CALIBRATIONMAX_INVALID;
                }
            }
            res = this._calibrationMax;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the maximal calibration value for the input (between 0 and 4095, included), without actually
     *   starting the automated calibration.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the maximal calibration value for the input (between 0 and 4095,
     *   included), without actually
     *   starting the automated calibration
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
    public int set_calibrationMax(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("calibrationMax", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the minimal value measured during the calibration (between 0 and 4095, included).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the minimal value measured during the calibration (between 0 and 4095, included)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.CALIBRATIONMIN_INVALID</c>.
     * </para>
     */
    public int get_calibrationMin()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CALIBRATIONMIN_INVALID;
                }
            }
            res = this._calibrationMin;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the minimal calibration value for the input (between 0 and 4095, included), without actually
     *   starting the automated calibration.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the minimal calibration value for the input (between 0 and 4095,
     *   included), without actually
     *   starting the automated calibration
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
    public int set_calibrationMin(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("calibrationMin", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the sensibility for the input (between 1 and 1000) for triggering user callbacks.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the sensibility for the input (between 1 and 1000) for triggering user callbacks
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.SENSITIVITY_INVALID</c>.
     * </para>
     */
    public int get_sensitivity()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SENSITIVITY_INVALID;
                }
            }
            res = this._sensitivity;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the sensibility for the input (between 1 and 1000) for triggering user callbacks.
     * <para>
     *   The sensibility is used to filter variations around a fixed value, but does not preclude the
     *   transmission of events when the input value evolves constantly in the same direction.
     *   Special case: when the value 1000 is used, the callback will only be thrown when the logical state
     *   of the input switches from pressed to released and back.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the sensibility for the input (between 1 and 1000) for triggering user callbacks
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
    public int set_sensitivity(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("sensitivity", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns true if the input (considered as binary) is active (closed contact), and false otherwise.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YAnButton.ISPRESSED_FALSE</c> or <c>YAnButton.ISPRESSED_TRUE</c>, according to true if
     *   the input (considered as binary) is active (closed contact), and false otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.ISPRESSED_INVALID</c>.
     * </para>
     */
    public int get_isPressed()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ISPRESSED_INVALID;
                }
            }
            res = this._isPressed;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the number of elapsed milliseconds between the module power on and the last time
     *   the input button was pressed (the input contact transitioned from open to closed).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last time
     *   the input button was pressed (the input contact transitioned from open to closed)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.LASTTIMEPRESSED_INVALID</c>.
     * </para>
     */
    public long get_lastTimePressed()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LASTTIMEPRESSED_INVALID;
                }
            }
            res = this._lastTimePressed;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the number of elapsed milliseconds between the module power on and the last time
     *   the input button was released (the input contact transitioned from closed to open).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last time
     *   the input button was released (the input contact transitioned from closed to open)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.LASTTIMERELEASED_INVALID</c>.
     * </para>
     */
    public long get_lastTimeReleased()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LASTTIMERELEASED_INVALID;
                }
            }
            res = this._lastTimeReleased;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the pulse counter value.
     * <para>
     *   The value is a 32 bit integer. In case
     *   of overflow (>=2^32), the counter will wrap. To reset the counter, just
     *   call the resetCounter() method.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the pulse counter value
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.PULSECOUNTER_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YAnButton.PULSETIMER_INVALID</c>.
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
     *   Retrieves an analog input for a given identifier.
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
     *   This function does not require that the analog input is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YAnButton.isOnline()</c> to test if the analog input is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   an analog input by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the analog input
     * </param>
     * <returns>
     *   a <c>YAnButton</c> object allowing you to drive the analog input.
     * </returns>
     */
    public static YAnButton FindAnButton(string func)
    {
        YAnButton obj;
        lock (YAPI.globalLock) {
            obj = (YAnButton) YFunction._FindFromCache("AnButton", func);
            if (obj == null) {
                obj = new YAnButton(func);
                YFunction._AddToCache("AnButton", func, obj);
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
        this._valueCallbackAnButton = callback;
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
        if (this._valueCallbackAnButton != null) {
            this._valueCallbackAnButton(this, value);
        } else {
            base._invokeValueCallback(value);
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
     *   Continues the enumeration of analog inputs started using <c>yFirstAnButton()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned analog inputs order.
     *   If you want to find a specific an analog input, use <c>AnButton.findAnButton()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YAnButton</c> object, corresponding to
     *   an analog input currently online, or a <c>null</c> pointer
     *   if there are no more analog inputs to enumerate.
     * </returns>
     */
    public YAnButton nextAnButton()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindAnButton(hwid);
    }

    //--- (end of YAnButton implementation)

    //--- (YAnButton functions)

    /**
     * <summary>
     *   Starts the enumeration of analog inputs currently accessible.
     * <para>
     *   Use the method <c>YAnButton.nextAnButton()</c> to iterate on
     *   next analog inputs.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YAnButton</c> object, corresponding to
     *   the first analog input currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YAnButton FirstAnButton()
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
        err = YAPI.apiGetFunctionsByClass("AnButton", 0, p, size, ref neededsize, ref errmsg);
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
        return FindAnButton(serial + "." + funcId);
    }



    //--- (end of YAnButton functions)
}
#pragma warning restore 1591
