/*********************************************************************
 *
 *  $Id: yocto_motor.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindMotor(), the high-level API for Motor functions
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
    //--- (YMotor return codes)
    //--- (end of YMotor return codes)
//--- (YMotor dlldef)
//--- (end of YMotor dlldef)
//--- (YMotor yapiwrapper)
//--- (end of YMotor yapiwrapper)
//--- (YMotor class start)
/**
 * <summary>
 *   Yoctopuce application programming interface allows you to drive the
 *   power sent to the motor to make it turn both ways, but also to drive accelerations
 *   and decelerations.
 * <para>
 *   The motor will then accelerate automatically: you will not
 *   have to monitor it. The API also allows to slow down the motor by shortening
 *   its terminals: the motor will then act as an electromagnetic brake.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YMotor : YFunction
{
//--- (end of YMotor class start)
    //--- (YMotor definitions)
    public new delegate void ValueCallback(YMotor func, string value);
    public new delegate void TimedReportCallback(YMotor func, YMeasure measure);

    public const int MOTORSTATUS_IDLE = 0;
    public const int MOTORSTATUS_BRAKE = 1;
    public const int MOTORSTATUS_FORWD = 2;
    public const int MOTORSTATUS_BACKWD = 3;
    public const int MOTORSTATUS_LOVOLT = 4;
    public const int MOTORSTATUS_HICURR = 5;
    public const int MOTORSTATUS_HIHEAT = 6;
    public const int MOTORSTATUS_FAILSF = 7;
    public const int MOTORSTATUS_INVALID = -1;
    public const double DRIVINGFORCE_INVALID = YAPI.INVALID_DOUBLE;
    public const double BRAKINGFORCE_INVALID = YAPI.INVALID_DOUBLE;
    public const double CUTOFFVOLTAGE_INVALID = YAPI.INVALID_DOUBLE;
    public const int OVERCURRENTLIMIT_INVALID = YAPI.INVALID_INT;
    public const double FREQUENCY_INVALID = YAPI.INVALID_DOUBLE;
    public const int STARTERTIME_INVALID = YAPI.INVALID_INT;
    public const int FAILSAFETIMEOUT_INVALID = YAPI.INVALID_UINT;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _motorStatus = MOTORSTATUS_INVALID;
    protected double _drivingForce = DRIVINGFORCE_INVALID;
    protected double _brakingForce = BRAKINGFORCE_INVALID;
    protected double _cutOffVoltage = CUTOFFVOLTAGE_INVALID;
    protected int _overCurrentLimit = OVERCURRENTLIMIT_INVALID;
    protected double _frequency = FREQUENCY_INVALID;
    protected int _starterTime = STARTERTIME_INVALID;
    protected int _failSafeTimeout = FAILSAFETIMEOUT_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackMotor = null;
    //--- (end of YMotor definitions)

    public YMotor(string func)
        : base(func)
    {
        _className = "Motor";
        //--- (YMotor attributes initialization)
        //--- (end of YMotor attributes initialization)
    }

    //--- (YMotor implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("motorStatus"))
        {
            _motorStatus = json_val.getInt("motorStatus");
        }
        if (json_val.has("drivingForce"))
        {
            _drivingForce = Math.Round(json_val.getDouble("drivingForce") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("brakingForce"))
        {
            _brakingForce = Math.Round(json_val.getDouble("brakingForce") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("cutOffVoltage"))
        {
            _cutOffVoltage = Math.Round(json_val.getDouble("cutOffVoltage") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("overCurrentLimit"))
        {
            _overCurrentLimit = json_val.getInt("overCurrentLimit");
        }
        if (json_val.has("frequency"))
        {
            _frequency = Math.Round(json_val.getDouble("frequency") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("starterTime"))
        {
            _starterTime = json_val.getInt("starterTime");
        }
        if (json_val.has("failSafeTimeout"))
        {
            _failSafeTimeout = json_val.getInt("failSafeTimeout");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Return the controller state.
     * <para>
     *   Possible states are:
     *   IDLE   when the motor is stopped/in free wheel, ready to start;
     *   FORWD  when the controller is driving the motor forward;
     *   BACKWD when the controller is driving the motor backward;
     *   BRAKE  when the controller is braking;
     *   LOVOLT when the controller has detected a low voltage condition;
     *   HICURR when the controller has detected an over current condition;
     *   HIHEAT when the controller has detected an overheat condition;
     *   FAILSF when the controller switched on the failsafe security.
     * </para>
     * <para>
     *   When an error condition occurred (LOVOLT, HICURR, HIHEAT, FAILSF), the controller
     *   status must be explicitly reset using the <c>resetStatus</c> function.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YMotor.MOTORSTATUS_IDLE</c>, <c>YMotor.MOTORSTATUS_BRAKE</c>,
     *   <c>YMotor.MOTORSTATUS_FORWD</c>, <c>YMotor.MOTORSTATUS_BACKWD</c>,
     *   <c>YMotor.MOTORSTATUS_LOVOLT</c>, <c>YMotor.MOTORSTATUS_HICURR</c>,
     *   <c>YMotor.MOTORSTATUS_HIHEAT</c> and <c>YMotor.MOTORSTATUS_FAILSF</c>
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMotor.MOTORSTATUS_INVALID</c>.
     * </para>
     */
    public int get_motorStatus()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MOTORSTATUS_INVALID;
                }
            }
            res = this._motorStatus;
        }
        return res;
    }

    public int set_motorStatus(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("motorStatus", rest_val);
        }
    }

    /**
     * <summary>
     *   Changes immediately the power sent to the motor.
     * <para>
     *   The value is a percentage between -100%
     *   to 100%. If you want go easy on your mechanics and avoid excessive current consumption,
     *   try to avoid brutal power changes. For example, immediate transition from forward full power
     *   to reverse full power is a very bad idea. Each time the driving power is modified, the
     *   braking power is set to zero.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to immediately the power sent to the motor
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
    public int set_drivingForce(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("drivingForce", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the power sent to the motor, as a percentage between -100% and +100%.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the power sent to the motor, as a percentage between -100% and +100%
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMotor.DRIVINGFORCE_INVALID</c>.
     * </para>
     */
    public double get_drivingForce()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DRIVINGFORCE_INVALID;
                }
            }
            res = this._drivingForce;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes immediately the braking force applied to the motor (in percents).
     * <para>
     *   The value 0 corresponds to no braking (free wheel). When the braking force
     *   is changed, the driving power is set to zero. The value is a percentage.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to immediately the braking force applied to the motor (in percents)
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
    public int set_brakingForce(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("brakingForce", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the braking force applied to the motor, as a percentage.
     * <para>
     *   The value 0 corresponds to no braking (free wheel).
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the braking force applied to the motor, as a percentage
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMotor.BRAKINGFORCE_INVALID</c>.
     * </para>
     */
    public double get_brakingForce()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BRAKINGFORCE_INVALID;
                }
            }
            res = this._brakingForce;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the threshold voltage under which the controller automatically switches to error state
     *   and prevents further current draw.
     * <para>
     *   This setting prevent damage to a battery that can
     *   occur when drawing current from an "empty" battery.
     *   Note that whatever the cutoff threshold, the controller switches to undervoltage
     *   error state if the power supply goes under 3V, even for a very brief time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the threshold voltage under which the controller
     *   automatically switches to error state
     *   and prevents further current draw
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
    public int set_cutOffVoltage(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("cutOffVoltage", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the threshold voltage under which the controller automatically switches to error state
     *   and prevents further current draw.
     * <para>
     *   This setting prevents damage to a battery that can
     *   occur when drawing current from an "empty" battery.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the threshold voltage under which the controller
     *   automatically switches to error state
     *   and prevents further current draw
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMotor.CUTOFFVOLTAGE_INVALID</c>.
     * </para>
     */
    public double get_cutOffVoltage()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CUTOFFVOLTAGE_INVALID;
                }
            }
            res = this._cutOffVoltage;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the current threshold (in mA) above which the controller automatically
     *   switches to error state.
     * <para>
     *   A zero value means that there is no limit.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current threshold (in mA) above which the controller automatically
     *   switches to error state
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMotor.OVERCURRENTLIMIT_INVALID</c>.
     * </para>
     */
    public int get_overCurrentLimit()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return OVERCURRENTLIMIT_INVALID;
                }
            }
            res = this._overCurrentLimit;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the current threshold (in mA) above which the controller automatically
     *   switches to error state.
     * <para>
     *   A zero value means that there is no limit. Note that whatever the
     *   current limit is, the controller switches to OVERCURRENT status if the current
     *   goes above 32A, even for a very brief time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the current threshold (in mA) above which the controller automatically
     *   switches to error state
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
    public int set_overCurrentLimit(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("overCurrentLimit", rest_val);
        }
    }

    /**
     * <summary>
     *   Changes the PWM frequency used to control the motor.
     * <para>
     *   Low frequency is usually
     *   more efficient and may help the motor to start, but an audible noise might be
     *   generated. A higher frequency reduces the noise, but more energy is converted
     *   into heat.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the PWM frequency used to control the motor
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
    public int set_frequency(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("frequency", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the PWM frequency used to control the motor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWM frequency used to control the motor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMotor.FREQUENCY_INVALID</c>.
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
     *   Returns the duration (in ms) during which the motor is driven at low frequency to help
     *   it start up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the duration (in ms) during which the motor is driven at low frequency to help
     *   it start up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMotor.STARTERTIME_INVALID</c>.
     * </para>
     */
    public int get_starterTime()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return STARTERTIME_INVALID;
                }
            }
            res = this._starterTime;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the duration (in ms) during which the motor is driven at low frequency to help
     *   it start up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the duration (in ms) during which the motor is driven at low frequency to help
     *   it start up
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
    public int set_starterTime(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("starterTime", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the delay in milliseconds allowed for the controller to run autonomously without
     *   receiving any instruction from the control process.
     * <para>
     *   When this delay has elapsed,
     *   the controller automatically stops the motor and switches to FAILSAFE error.
     *   Failsafe security is disabled when the value is zero.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the delay in milliseconds allowed for the controller to run autonomously without
     *   receiving any instruction from the control process
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMotor.FAILSAFETIMEOUT_INVALID</c>.
     * </para>
     */
    public int get_failSafeTimeout()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return FAILSAFETIMEOUT_INVALID;
                }
            }
            res = this._failSafeTimeout;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the delay in milliseconds allowed for the controller to run autonomously without
     *   receiving any instruction from the control process.
     * <para>
     *   When this delay has elapsed,
     *   the controller automatically stops the motor and switches to FAILSAFE error.
     *   Failsafe security is disabled when the value is zero.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the delay in milliseconds allowed for the controller to run autonomously without
     *   receiving any instruction from the control process
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
    public int set_failSafeTimeout(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("failSafeTimeout", rest_val);
        }
    }

    public string get_command()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return COMMAND_INVALID;
                }
            }
            res = this._command;
        }
        return res;
    }

    public int set_command(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("command", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a motor for a given identifier.
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
     *   This function does not require that the motor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YMotor.isOnline()</c> to test if the motor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a motor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the motor
     * </param>
     * <returns>
     *   a <c>YMotor</c> object allowing you to drive the motor.
     * </returns>
     */
    public static YMotor FindMotor(string func)
    {
        YMotor obj;
        lock (YAPI.globalLock) {
            obj = (YMotor) YFunction._FindFromCache("Motor", func);
            if (obj == null) {
                obj = new YMotor(func);
                YFunction._AddToCache("Motor", func, obj);
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
        this._valueCallbackMotor = callback;
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
        if (this._valueCallbackMotor != null) {
            this._valueCallbackMotor(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Rearms the controller failsafe timer.
     * <para>
     *   When the motor is running and the failsafe feature
     *   is active, this function should be called periodically to prove that the control process
     *   is running properly. Otherwise, the motor is automatically stopped after the specified
     *   timeout. Calling a motor <i>set</i> function implicitly rearms the failsafe timer.
     * </para>
     * </summary>
     */
    public virtual int keepALive()
    {
        return this.set_command("K");
    }

    /**
     * <summary>
     *   Reset the controller state to IDLE.
     * <para>
     *   This function must be invoked explicitly
     *   after any error condition is signaled.
     * </para>
     * </summary>
     */
    public virtual int resetStatus()
    {
        return this.set_motorStatus(MOTORSTATUS_IDLE);
    }

    /**
     * <summary>
     *   Changes progressively the power sent to the motor for a specific duration.
     * <para>
     * </para>
     * </summary>
     * <param name="targetPower">
     *   desired motor power, in percents (between -100% and +100%)
     * </param>
     * <param name="delay">
     *   duration (in ms) of the transition
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int drivingForceMove(double targetPower, int delay)
    {
        return this.set_command("P"+Convert.ToString((int) Math.Round(targetPower*10))+","+Convert.ToString(delay));
    }

    /**
     * <summary>
     *   Changes progressively the braking force applied to the motor for a specific duration.
     * <para>
     * </para>
     * </summary>
     * <param name="targetPower">
     *   desired braking force, in percents
     * </param>
     * <param name="delay">
     *   duration (in ms) of the transition
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int brakingForceMove(double targetPower, int delay)
    {
        return this.set_command("B"+Convert.ToString((int) Math.Round(targetPower*10))+","+Convert.ToString(delay));
    }

    /**
     * <summary>
     *   Continues the enumeration of motors started using <c>yFirstMotor()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned motors order.
     *   If you want to find a specific a motor, use <c>Motor.findMotor()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YMotor</c> object, corresponding to
     *   a motor currently online, or a <c>null</c> pointer
     *   if there are no more motors to enumerate.
     * </returns>
     */
    public YMotor nextMotor()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindMotor(hwid);
    }

    //--- (end of YMotor implementation)

    //--- (YMotor functions)

    /**
     * <summary>
     *   Starts the enumeration of motors currently accessible.
     * <para>
     *   Use the method <c>YMotor.nextMotor()</c> to iterate on
     *   next motors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YMotor</c> object, corresponding to
     *   the first motor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YMotor FirstMotor()
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
        err = YAPI.apiGetFunctionsByClass("Motor", 0, p, size, ref neededsize, ref errmsg);
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
        return FindMotor(serial + "." + funcId);
    }



    //--- (end of YMotor functions)
}
#pragma warning restore 1591
