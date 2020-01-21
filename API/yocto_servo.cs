/*********************************************************************
 *
 *  $Id: yocto_servo.cs 38899 2019-12-20 17:21:03Z mvuilleu $
 *
 *  Implements yFindServo(), the high-level API for Servo functions
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
    //--- (YServo return codes)
    //--- (end of YServo return codes)
//--- (YServo dlldef)
//--- (end of YServo dlldef)
//--- (YServo yapiwrapper)
//--- (end of YServo yapiwrapper)
//--- (YServo class start)
/**
 * <summary>
 *   The <c>YServo</c> class is designed to drive remote-control servo motors
 *   outputs.
 * <para>
 *   This class allows you not only to move
 *   a servo to a given position, but also to specify the time interval
 *   in which the move should be performed. This makes it possible to
 *   synchronize two servos involved in a same move.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YServo : YFunction
{
//--- (end of YServo class start)
    //--- (YServo definitions)
    public new delegate void ValueCallback(YServo func, string value);
    public new delegate void TimedReportCallback(YServo func, YMeasure measure);

    public /* struct */ class YServoMove
    {
        public int target = YAPI.INVALID_INT;
        public int ms = YAPI.INVALID_INT;
        public int moving = YAPI.INVALID_UINT;
    }

    public const int POSITION_INVALID = YAPI.INVALID_INT;
    public const int ENABLED_FALSE = 0;
    public const int ENABLED_TRUE = 1;
    public const int ENABLED_INVALID = -1;
    public const int RANGE_INVALID = YAPI.INVALID_UINT;
    public const int NEUTRAL_INVALID = YAPI.INVALID_UINT;
    public const int POSITIONATPOWERON_INVALID = YAPI.INVALID_INT;
    public const int ENABLEDATPOWERON_FALSE = 0;
    public const int ENABLEDATPOWERON_TRUE = 1;
    public const int ENABLEDATPOWERON_INVALID = -1;
    public static readonly YServoMove MOVE_INVALID = null;
    protected int _position = POSITION_INVALID;
    protected int _enabled = ENABLED_INVALID;
    protected int _range = RANGE_INVALID;
    protected int _neutral = NEUTRAL_INVALID;
    protected YServoMove _move = new YServoMove();
    protected int _positionAtPowerOn = POSITIONATPOWERON_INVALID;
    protected int _enabledAtPowerOn = ENABLEDATPOWERON_INVALID;
    protected ValueCallback _valueCallbackServo = null;
    //--- (end of YServo definitions)

    public YServo(string func)
        : base(func)
    {
        _className = "Servo";
        //--- (YServo attributes initialization)
        //--- (end of YServo attributes initialization)
    }

    //--- (YServo implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("position"))
        {
            _position = json_val.getInt("position");
        }
        if (json_val.has("enabled"))
        {
            _enabled = json_val.getInt("enabled") > 0 ? 1 : 0;
        }
        if (json_val.has("range"))
        {
            _range = json_val.getInt("range");
        }
        if (json_val.has("neutral"))
        {
            _neutral = json_val.getInt("neutral");
        }
        if (json_val.has("move"))
        {
            YAPI.YJSONObject subjson = json_val.getYJSONObject("move");
            if (subjson.has("moving")) {
                _move.moving = subjson.getInt("moving");
            }
            if (subjson.has("target")) {
                _move.target = subjson.getInt("target");
            }
            if (subjson.has("ms")) {
                _move.ms = subjson.getInt("ms");
            }
        }
        if (json_val.has("positionAtPowerOn"))
        {
            _positionAtPowerOn = json_val.getInt("positionAtPowerOn");
        }
        if (json_val.has("enabledAtPowerOn"))
        {
            _enabledAtPowerOn = json_val.getInt("enabledAtPowerOn") > 0 ? 1 : 0;
        }
        base._parseAttr(json_val);
    }


    /**
     * <summary>
     *   Returns the current servo position.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current servo position
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YServo.POSITION_INVALID</c>.
     * </para>
     */
    public int get_position()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return POSITION_INVALID;
                }
            }
            res = this._position;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes immediately the servo driving position.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to immediately the servo driving position
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
    public int set_position(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("position", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the state of the RC servo motors.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YServo.ENABLED_FALSE</c> or <c>YServo.ENABLED_TRUE</c>, according to the state of the RC servo motors
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YServo.ENABLED_INVALID</c>.
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
     *   Stops or starts the RC servo motor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YServo.ENABLED_FALSE</c> or <c>YServo.ENABLED_TRUE</c>
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
     *   Returns the current range of use of the servo.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current range of use of the servo
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YServo.RANGE_INVALID</c>.
     * </para>
     */
    public int get_range()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RANGE_INVALID;
                }
            }
            res = this._range;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the range of use of the servo, specified in per cents.
     * <para>
     *   A range of 100% corresponds to a standard control signal, that varies
     *   from 1 [ms] to 2 [ms], When using a servo that supports a double range,
     *   from 0.5 [ms] to 2.5 [ms], you can select a range of 200%.
     *   Be aware that using a range higher than what is supported by the servo
     *   is likely to damage the servo. Remember to call the matching module
     *   <c>saveToFlash()</c> method, otherwise this call will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the range of use of the servo, specified in per cents
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
    public int set_range(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("range", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the duration in microseconds of a neutral pulse for the servo.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the duration in microseconds of a neutral pulse for the servo
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YServo.NEUTRAL_INVALID</c>.
     * </para>
     */
    public int get_neutral()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return NEUTRAL_INVALID;
                }
            }
            res = this._neutral;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the duration of the pulse corresponding to the neutral position of the servo.
     * <para>
     *   The duration is specified in microseconds, and the standard value is 1500 [us].
     *   This setting makes it possible to shift the range of use of the servo.
     *   Be aware that using a range higher than what is supported by the servo is
     *   likely to damage the servo. Remember to call the matching module
     *   <c>saveToFlash()</c> method, otherwise this call will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the duration of the pulse corresponding to the neutral position of the servo
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
    public int set_neutral(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("neutral", rest_val);
        }
    }


    public YServoMove get_move()
    {
        YServoMove res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MOVE_INVALID;
                }
            }
            res = this._move;
        }
        return res;
    }

    public int set_move(YServoMove newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval.target).ToString()+":"+(newval.ms).ToString();
            return _setAttr("move", rest_val);
        }
    }

    /**
     * <summary>
     *   Performs a smooth move at constant speed toward a given position.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="target">
     *   new position at the end of the move
     * </param>
     * <param name="ms_duration">
     *   total duration of the move, in milliseconds
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
    public int move(int target,int ms_duration)
    {
        string rest_val;
        rest_val = (target).ToString()+":"+(ms_duration).ToString();
        return _setAttr("move", rest_val);
    }


    /**
     * <summary>
     *   Returns the servo position at device power up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the servo position at device power up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YServo.POSITIONATPOWERON_INVALID</c>.
     * </para>
     */
    public int get_positionAtPowerOn()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return POSITIONATPOWERON_INVALID;
                }
            }
            res = this._positionAtPowerOn;
        }
        return res;
    }

    /**
     * <summary>
     *   Configure the servo position at device power up.
     * <para>
     *   Remember to call the matching
     *   module <c>saveToFlash()</c> method, otherwise this call will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer
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
    public int set_positionAtPowerOn(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("positionAtPowerOn", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the servo signal generator state at power up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YServo.ENABLEDATPOWERON_FALSE</c> or <c>YServo.ENABLEDATPOWERON_TRUE</c>, according to
     *   the servo signal generator state at power up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YServo.ENABLEDATPOWERON_INVALID</c>.
     * </para>
     */
    public int get_enabledAtPowerOn()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ENABLEDATPOWERON_INVALID;
                }
            }
            res = this._enabledAtPowerOn;
        }
        return res;
    }

    /**
     * <summary>
     *   Configure the servo signal generator state at power up.
     * <para>
     *   Remember to call the matching module <c>saveToFlash()</c>
     *   method, otherwise this call will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YServo.ENABLEDATPOWERON_FALSE</c> or <c>YServo.ENABLEDATPOWERON_TRUE</c>
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
    public int set_enabledAtPowerOn(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("enabledAtPowerOn", rest_val);
        }
    }


    /**
     * <summary>
     *   Retrieves a RC servo motor for a given identifier.
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
     *   This function does not require that the RC servo motor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YServo.isOnline()</c> to test if the RC servo motor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a RC servo motor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the RC servo motor, for instance
     *   <c>SERVORC1.servo1</c>.
     * </param>
     * <returns>
     *   a <c>YServo</c> object allowing you to drive the RC servo motor.
     * </returns>
     */
    public static YServo FindServo(string func)
    {
        YServo obj;
        lock (YAPI.globalLock) {
            obj = (YServo) YFunction._FindFromCache("Servo", func);
            if (obj == null) {
                obj = new YServo(func);
                YFunction._AddToCache("Servo", func, obj);
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
        this._valueCallbackServo = callback;
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
        if (this._valueCallbackServo != null) {
            this._valueCallbackServo(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of RC servo motors started using <c>yFirstServo()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned RC servo motors order.
     *   If you want to find a specific a RC servo motor, use <c>Servo.findServo()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YServo</c> object, corresponding to
     *   a RC servo motor currently online, or a <c>null</c> pointer
     *   if there are no more RC servo motors to enumerate.
     * </returns>
     */
    public YServo nextServo()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindServo(hwid);
    }

    //--- (end of YServo implementation)

    //--- (YServo functions)

    /**
     * <summary>
     *   Starts the enumeration of RC servo motors currently accessible.
     * <para>
     *   Use the method <c>YServo.nextServo()</c> to iterate on
     *   next RC servo motors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YServo</c> object, corresponding to
     *   the first RC servo motor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YServo FirstServo()
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
        err = YAPI.apiGetFunctionsByClass("Servo", 0, p, size, ref neededsize, ref errmsg);
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
        return FindServo(serial + "." + funcId);
    }



    //--- (end of YServo functions)
}
#pragma warning restore 1591
