/*********************************************************************
 *
 *  $Id: yocto_proximity.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindProximity(), the high-level API for Proximity functions
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
    //--- (YProximity return codes)
    //--- (end of YProximity return codes)
//--- (YProximity dlldef)
//--- (end of YProximity dlldef)
//--- (YProximity yapiwrapper)
//--- (end of YProximity yapiwrapper)
//--- (YProximity class start)
/**
 * <summary>
 *   The Yoctopuce class YProximity allows you to use and configure Yoctopuce proximity
 *   sensors.
 * <para>
 *   It inherits from the YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 *   This class adds the ability to easily perform a one-point linear calibration
 *   to compensate the effect of a glass or filter placed in front of the sensor.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YProximity : YSensor
{
//--- (end of YProximity class start)
    //--- (YProximity definitions)
    public new delegate void ValueCallback(YProximity func, string value);
    public new delegate void TimedReportCallback(YProximity func, YMeasure measure);

    public const double SIGNALVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const int DETECTIONTHRESHOLD_INVALID = YAPI.INVALID_UINT;
    public const int DETECTIONHYSTERESIS_INVALID = YAPI.INVALID_UINT;
    public const int PRESENCEMINTIME_INVALID = YAPI.INVALID_UINT;
    public const int REMOVALMINTIME_INVALID = YAPI.INVALID_UINT;
    public const int ISPRESENT_FALSE = 0;
    public const int ISPRESENT_TRUE = 1;
    public const int ISPRESENT_INVALID = -1;
    public const long LASTTIMEAPPROACHED_INVALID = YAPI.INVALID_LONG;
    public const long LASTTIMEREMOVED_INVALID = YAPI.INVALID_LONG;
    public const long PULSECOUNTER_INVALID = YAPI.INVALID_LONG;
    public const long PULSETIMER_INVALID = YAPI.INVALID_LONG;
    public const int PROXIMITYREPORTMODE_NUMERIC = 0;
    public const int PROXIMITYREPORTMODE_PRESENCE = 1;
    public const int PROXIMITYREPORTMODE_PULSECOUNT = 2;
    public const int PROXIMITYREPORTMODE_INVALID = -1;
    protected double _signalValue = SIGNALVALUE_INVALID;
    protected int _detectionThreshold = DETECTIONTHRESHOLD_INVALID;
    protected int _detectionHysteresis = DETECTIONHYSTERESIS_INVALID;
    protected int _presenceMinTime = PRESENCEMINTIME_INVALID;
    protected int _removalMinTime = REMOVALMINTIME_INVALID;
    protected int _isPresent = ISPRESENT_INVALID;
    protected long _lastTimeApproached = LASTTIMEAPPROACHED_INVALID;
    protected long _lastTimeRemoved = LASTTIMEREMOVED_INVALID;
    protected long _pulseCounter = PULSECOUNTER_INVALID;
    protected long _pulseTimer = PULSETIMER_INVALID;
    protected int _proximityReportMode = PROXIMITYREPORTMODE_INVALID;
    protected ValueCallback _valueCallbackProximity = null;
    protected TimedReportCallback _timedReportCallbackProximity = null;
    //--- (end of YProximity definitions)

    public YProximity(string func)
        : base(func)
    {
        _className = "Proximity";
        //--- (YProximity attributes initialization)
        //--- (end of YProximity attributes initialization)
    }

    //--- (YProximity implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("signalValue"))
        {
            _signalValue = Math.Round(json_val.getDouble("signalValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("detectionThreshold"))
        {
            _detectionThreshold = json_val.getInt("detectionThreshold");
        }
        if (json_val.has("detectionHysteresis"))
        {
            _detectionHysteresis = json_val.getInt("detectionHysteresis");
        }
        if (json_val.has("presenceMinTime"))
        {
            _presenceMinTime = json_val.getInt("presenceMinTime");
        }
        if (json_val.has("removalMinTime"))
        {
            _removalMinTime = json_val.getInt("removalMinTime");
        }
        if (json_val.has("isPresent"))
        {
            _isPresent = json_val.getInt("isPresent") > 0 ? 1 : 0;
        }
        if (json_val.has("lastTimeApproached"))
        {
            _lastTimeApproached = json_val.getLong("lastTimeApproached");
        }
        if (json_val.has("lastTimeRemoved"))
        {
            _lastTimeRemoved = json_val.getLong("lastTimeRemoved");
        }
        if (json_val.has("pulseCounter"))
        {
            _pulseCounter = json_val.getLong("pulseCounter");
        }
        if (json_val.has("pulseTimer"))
        {
            _pulseTimer = json_val.getLong("pulseTimer");
        }
        if (json_val.has("proximityReportMode"))
        {
            _proximityReportMode = json_val.getInt("proximityReportMode");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the current value of signal measured by the proximity sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current value of signal measured by the proximity sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.SIGNALVALUE_INVALID</c>.
     * </para>
     */
    public double get_signalValue()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SIGNALVALUE_INVALID;
                }
            }
            res = Math.Round(this._signalValue * 1000) / 1000;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the threshold used to determine the logical state of the proximity sensor, when considered
     *   as a binary input (on/off).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the threshold used to determine the logical state of the proximity
     *   sensor, when considered
     *   as a binary input (on/off)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.DETECTIONTHRESHOLD_INVALID</c>.
     * </para>
     */
    public int get_detectionThreshold()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DETECTIONTHRESHOLD_INVALID;
                }
            }
            res = this._detectionThreshold;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the threshold used to determine the logical state of the proximity sensor, when considered
     *   as a binary input (on/off).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the threshold used to determine the logical state of the proximity
     *   sensor, when considered
     *   as a binary input (on/off)
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
    public int set_detectionThreshold(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("detectionThreshold", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the hysteresis used to determine the logical state of the proximity sensor, when considered
     *   as a binary input (on/off).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the hysteresis used to determine the logical state of the proximity
     *   sensor, when considered
     *   as a binary input (on/off)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.DETECTIONHYSTERESIS_INVALID</c>.
     * </para>
     */
    public int get_detectionHysteresis()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DETECTIONHYSTERESIS_INVALID;
                }
            }
            res = this._detectionHysteresis;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the hysteresis used to determine the logical state of the proximity sensor, when considered
     *   as a binary input (on/off).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the hysteresis used to determine the logical state of the proximity
     *   sensor, when considered
     *   as a binary input (on/off)
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
    public int set_detectionHysteresis(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("detectionHysteresis", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the minimal detection duration before signalling a presence event.
     * <para>
     *   Any shorter detection is
     *   considered as noise or bounce (false positive) and filtered out.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the minimal detection duration before signalling a presence event
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.PRESENCEMINTIME_INVALID</c>.
     * </para>
     */
    public int get_presenceMinTime()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PRESENCEMINTIME_INVALID;
                }
            }
            res = this._presenceMinTime;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the minimal detection duration before signalling a presence event.
     * <para>
     *   Any shorter detection is
     *   considered as noise or bounce (false positive) and filtered out.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the minimal detection duration before signalling a presence event
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
    public int set_presenceMinTime(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("presenceMinTime", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the minimal detection duration before signalling a removal event.
     * <para>
     *   Any shorter detection is
     *   considered as noise or bounce (false positive) and filtered out.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the minimal detection duration before signalling a removal event
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.REMOVALMINTIME_INVALID</c>.
     * </para>
     */
    public int get_removalMinTime()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return REMOVALMINTIME_INVALID;
                }
            }
            res = this._removalMinTime;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the minimal detection duration before signalling a removal event.
     * <para>
     *   Any shorter detection is
     *   considered as noise or bounce (false positive) and filtered out.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the minimal detection duration before signalling a removal event
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
    public int set_removalMinTime(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("removalMinTime", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns true if the input (considered as binary) is active (detection value is smaller than the specified <c>threshold</c>), and false otherwise.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YProximity.ISPRESENT_FALSE</c> or <c>YProximity.ISPRESENT_TRUE</c>, according to true if
     *   the input (considered as binary) is active (detection value is smaller than the specified
     *   <c>threshold</c>), and false otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.ISPRESENT_INVALID</c>.
     * </para>
     */
    public int get_isPresent()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ISPRESENT_INVALID;
                }
            }
            res = this._isPresent;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the number of elapsed milliseconds between the module power on and the last observed
     *   detection (the input contact transitioned from absent to present).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last observed
     *   detection (the input contact transitioned from absent to present)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.LASTTIMEAPPROACHED_INVALID</c>.
     * </para>
     */
    public long get_lastTimeApproached()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LASTTIMEAPPROACHED_INVALID;
                }
            }
            res = this._lastTimeApproached;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the number of elapsed milliseconds between the module power on and the last observed
     *   detection (the input contact transitioned from present to absent).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last observed
     *   detection (the input contact transitioned from present to absent)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.LASTTIMEREMOVED_INVALID</c>.
     * </para>
     */
    public long get_lastTimeRemoved()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LASTTIMEREMOVED_INVALID;
                }
            }
            res = this._lastTimeRemoved;
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
     *   On failure, throws an exception or returns <c>YProximity.PULSECOUNTER_INVALID</c>.
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
     *   Returns the timer of the pulse counter (ms).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the timer of the pulse counter (ms)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.PULSETIMER_INVALID</c>.
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
     *   Returns the parameter (sensor value, presence or pulse count) returned by the get_currentValue function and callbacks.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YProximity.PROXIMITYREPORTMODE_NUMERIC</c>,
     *   <c>YProximity.PROXIMITYREPORTMODE_PRESENCE</c> and <c>YProximity.PROXIMITYREPORTMODE_PULSECOUNT</c>
     *   corresponding to the parameter (sensor value, presence or pulse count) returned by the
     *   get_currentValue function and callbacks
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YProximity.PROXIMITYREPORTMODE_INVALID</c>.
     * </para>
     */
    public int get_proximityReportMode()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PROXIMITYREPORTMODE_INVALID;
                }
            }
            res = this._proximityReportMode;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the  parameter  type (sensor value, presence or pulse count) returned by the get_currentValue function and callbacks.
     * <para>
     *   The edge count value is limited to the 6 lowest digits. For values greater than one million, use
     *   get_pulseCounter().
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YProximity.PROXIMITYREPORTMODE_NUMERIC</c>,
     *   <c>YProximity.PROXIMITYREPORTMODE_PRESENCE</c> and <c>YProximity.PROXIMITYREPORTMODE_PULSECOUNT</c>
     *   corresponding to the  parameter  type (sensor value, presence or pulse count) returned by the
     *   get_currentValue function and callbacks
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
    public int set_proximityReportMode(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("proximityReportMode", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a proximity sensor for a given identifier.
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
     *   This function does not require that the proximity sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YProximity.isOnline()</c> to test if the proximity sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a proximity sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the proximity sensor
     * </param>
     * <returns>
     *   a <c>YProximity</c> object allowing you to drive the proximity sensor.
     * </returns>
     */
    public static YProximity FindProximity(string func)
    {
        YProximity obj;
        lock (YAPI.globalLock) {
            obj = (YProximity) YFunction._FindFromCache("Proximity", func);
            if (obj == null) {
                obj = new YProximity(func);
                YFunction._AddToCache("Proximity", func, obj);
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
        this._valueCallbackProximity = callback;
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
        if (this._valueCallbackProximity != null) {
            this._valueCallbackProximity(this, value);
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
        this._timedReportCallbackProximity = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackProximity != null) {
            this._timedReportCallbackProximity(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Resets the pulse counter value as well as its timer.
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
     *   Continues the enumeration of proximity sensors started using <c>yFirstProximity()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned proximity sensors order.
     *   If you want to find a specific a proximity sensor, use <c>Proximity.findProximity()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YProximity</c> object, corresponding to
     *   a proximity sensor currently online, or a <c>null</c> pointer
     *   if there are no more proximity sensors to enumerate.
     * </returns>
     */
    public YProximity nextProximity()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindProximity(hwid);
    }

    //--- (end of YProximity implementation)

    //--- (YProximity functions)

    /**
     * <summary>
     *   Starts the enumeration of proximity sensors currently accessible.
     * <para>
     *   Use the method <c>YProximity.nextProximity()</c> to iterate on
     *   next proximity sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YProximity</c> object, corresponding to
     *   the first proximity sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YProximity FirstProximity()
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
        err = YAPI.apiGetFunctionsByClass("Proximity", 0, p, size, ref neededsize, ref errmsg);
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
        return FindProximity(serial + "." + funcId);
    }



    //--- (end of YProximity functions)
}
#pragma warning restore 1591
