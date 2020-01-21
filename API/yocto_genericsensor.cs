/*********************************************************************
 *
 *  $Id: yocto_genericsensor.cs 38899 2019-12-20 17:21:03Z mvuilleu $
 *
 *  Implements yFindGenericSensor(), the high-level API for GenericSensor functions
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
    //--- (YGenericSensor return codes)
    //--- (end of YGenericSensor return codes)
//--- (YGenericSensor dlldef)
//--- (end of YGenericSensor dlldef)
//--- (YGenericSensor yapiwrapper)
//--- (end of YGenericSensor yapiwrapper)
//--- (YGenericSensor class start)
/**
 * <summary>
 *   The <c>YGenericSensor</c> class allows you to read and configure Yoctopuce signal
 *   transducers.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 *   This class adds the ability to configure the automatic conversion between the
 *   measured signal and the corresponding engineering unit.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YGenericSensor : YSensor
{
//--- (end of YGenericSensor class start)
    //--- (YGenericSensor definitions)
    public new delegate void ValueCallback(YGenericSensor func, string value);
    public new delegate void TimedReportCallback(YGenericSensor func, YMeasure measure);

    public const double SIGNALVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const string SIGNALUNIT_INVALID = YAPI.INVALID_STRING;
    public const string SIGNALRANGE_INVALID = YAPI.INVALID_STRING;
    public const string VALUERANGE_INVALID = YAPI.INVALID_STRING;
    public const double SIGNALBIAS_INVALID = YAPI.INVALID_DOUBLE;
    public const int SIGNALSAMPLING_HIGH_RATE = 0;
    public const int SIGNALSAMPLING_HIGH_RATE_FILTERED = 1;
    public const int SIGNALSAMPLING_LOW_NOISE = 2;
    public const int SIGNALSAMPLING_LOW_NOISE_FILTERED = 3;
    public const int SIGNALSAMPLING_HIGHEST_RATE = 4;
    public const int SIGNALSAMPLING_INVALID = -1;
    public const int ENABLED_FALSE = 0;
    public const int ENABLED_TRUE = 1;
    public const int ENABLED_INVALID = -1;
    protected double _signalValue = SIGNALVALUE_INVALID;
    protected string _signalUnit = SIGNALUNIT_INVALID;
    protected string _signalRange = SIGNALRANGE_INVALID;
    protected string _valueRange = VALUERANGE_INVALID;
    protected double _signalBias = SIGNALBIAS_INVALID;
    protected int _signalSampling = SIGNALSAMPLING_INVALID;
    protected int _enabled = ENABLED_INVALID;
    protected ValueCallback _valueCallbackGenericSensor = null;
    protected TimedReportCallback _timedReportCallbackGenericSensor = null;
    //--- (end of YGenericSensor definitions)

    public YGenericSensor(string func)
        : base(func)
    {
        _className = "GenericSensor";
        //--- (YGenericSensor attributes initialization)
        //--- (end of YGenericSensor attributes initialization)
    }

    //--- (YGenericSensor implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("signalValue"))
        {
            _signalValue = Math.Round(json_val.getDouble("signalValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("signalUnit"))
        {
            _signalUnit = json_val.getString("signalUnit");
        }
        if (json_val.has("signalRange"))
        {
            _signalRange = json_val.getString("signalRange");
        }
        if (json_val.has("valueRange"))
        {
            _valueRange = json_val.getString("valueRange");
        }
        if (json_val.has("signalBias"))
        {
            _signalBias = Math.Round(json_val.getDouble("signalBias") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("signalSampling"))
        {
            _signalSampling = json_val.getInt("signalSampling");
        }
        if (json_val.has("enabled"))
        {
            _enabled = json_val.getInt("enabled") > 0 ? 1 : 0;
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the measuring unit for the measured value.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the measuring unit for the measured value
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
     *   Returns the current value of the electrical signal measured by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current value of the electrical signal measured by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALVALUE_INVALID</c>.
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
     *   Returns the measuring unit of the electrical signal used by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the measuring unit of the electrical signal used by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALUNIT_INVALID</c>.
     * </para>
     */
    public string get_signalUnit()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SIGNALUNIT_INVALID;
                }
            }
            res = this._signalUnit;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the input signal range used by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the input signal range used by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALRANGE_INVALID</c>.
     * </para>
     */
    public string get_signalRange()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SIGNALRANGE_INVALID;
                }
            }
            res = this._signalRange;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the input signal range used by the sensor.
     * <para>
     *   When the input signal gets out of the planned range, the output value
     *   will be set to an arbitrary large value, whose sign indicates the direction
     *   of the range overrun.
     * </para>
     * <para>
     *   For a 4-20mA sensor, the default input signal range is "4...20".
     *   For a 0-10V sensor, the default input signal range is "0.1...10".
     *   For numeric communication interfaces, the default input signal range is
     *   "-999999.999...999999.999".
     * </para>
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the input signal range used by the sensor
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
    public int set_signalRange(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("signalRange", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the physical value range measured by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the physical value range measured by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.VALUERANGE_INVALID</c>.
     * </para>
     */
    public string get_valueRange()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VALUERANGE_INVALID;
                }
            }
            res = this._valueRange;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the output value range, corresponding to the physical value measured
     *   by the sensor.
     * <para>
     *   The default output value range is the same as the input signal
     *   range (1:1 mapping), but you can change it so that the function automatically
     *   computes the physical value encoded by the input signal. Be aware that, as a
     *   side effect, the range modification may automatically modify the display resolution.
     * </para>
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the output value range, corresponding to the physical value measured
     *   by the sensor
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
    public int set_valueRange(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("valueRange", rest_val);
        }
    }

    /**
     * <summary>
     *   Changes the electric signal bias for zero shift adjustment.
     * <para>
     *   If your electric signal reads positive when it should be zero, setup
     *   a positive signalBias of the same value to fix the zero shift.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the electric signal bias for zero shift adjustment
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
    public int set_signalBias(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("signalBias", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the electric signal bias for zero shift adjustment.
     * <para>
     *   A positive bias means that the signal is over-reporting the measure,
     *   while a negative bias means that the signal is under-reporting the measure.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the electric signal bias for zero shift adjustment
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALBIAS_INVALID</c>.
     * </para>
     */
    public double get_signalBias()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SIGNALBIAS_INVALID;
                }
            }
            res = this._signalBias;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the electric signal sampling method to use.
     * <para>
     *   The <c>HIGH_RATE</c> method uses the highest sampling frequency, without any filtering.
     *   The <c>HIGH_RATE_FILTERED</c> method adds a windowed 7-sample median filter.
     *   The <c>LOW_NOISE</c> method uses a reduced acquisition frequency to reduce noise.
     *   The <c>LOW_NOISE_FILTERED</c> method combines a reduced frequency with the median filter
     *   to get measures as stable as possible when working on a noisy signal.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YGenericSensor.SIGNALSAMPLING_HIGH_RATE</c>,
     *   <c>YGenericSensor.SIGNALSAMPLING_HIGH_RATE_FILTERED</c>, <c>YGenericSensor.SIGNALSAMPLING_LOW_NOISE</c>,
     *   <c>YGenericSensor.SIGNALSAMPLING_LOW_NOISE_FILTERED</c> and <c>YGenericSensor.SIGNALSAMPLING_HIGHEST_RATE</c>
     *   corresponding to the electric signal sampling method to use
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALSAMPLING_INVALID</c>.
     * </para>
     */
    public int get_signalSampling()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SIGNALSAMPLING_INVALID;
                }
            }
            res = this._signalSampling;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the electric signal sampling method to use.
     * <para>
     *   The <c>HIGH_RATE</c> method uses the highest sampling frequency, without any filtering.
     *   The <c>HIGH_RATE_FILTERED</c> method adds a windowed 7-sample median filter.
     *   The <c>LOW_NOISE</c> method uses a reduced acquisition frequency to reduce noise.
     *   The <c>LOW_NOISE_FILTERED</c> method combines a reduced frequency with the median filter
     *   to get measures as stable as possible when working on a noisy signal.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YGenericSensor.SIGNALSAMPLING_HIGH_RATE</c>,
     *   <c>YGenericSensor.SIGNALSAMPLING_HIGH_RATE_FILTERED</c>, <c>YGenericSensor.SIGNALSAMPLING_LOW_NOISE</c>,
     *   <c>YGenericSensor.SIGNALSAMPLING_LOW_NOISE_FILTERED</c> and <c>YGenericSensor.SIGNALSAMPLING_HIGHEST_RATE</c>
     *   corresponding to the electric signal sampling method to use
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
    public int set_signalSampling(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("signalSampling", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the activation state of this input.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YGenericSensor.ENABLED_FALSE</c> or <c>YGenericSensor.ENABLED_TRUE</c>, according to the
     *   activation state of this input
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.ENABLED_INVALID</c>.
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
     *   Changes the activation state of this input.
     * <para>
     *   When an input is disabled,
     *   its value is no more updated. On some devices, disabling an input can
     *   improve the refresh rate of the other active inputs.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YGenericSensor.ENABLED_FALSE</c> or <c>YGenericSensor.ENABLED_TRUE</c>, according to the
     *   activation state of this input
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
     *   Retrieves a generic sensor for a given identifier.
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
     *   This function does not require that the generic sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YGenericSensor.isOnline()</c> to test if the generic sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a generic sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the generic sensor, for instance
     *   <c>RX010V01.genericSensor1</c>.
     * </param>
     * <returns>
     *   a <c>YGenericSensor</c> object allowing you to drive the generic sensor.
     * </returns>
     */
    public static YGenericSensor FindGenericSensor(string func)
    {
        YGenericSensor obj;
        lock (YAPI.globalLock) {
            obj = (YGenericSensor) YFunction._FindFromCache("GenericSensor", func);
            if (obj == null) {
                obj = new YGenericSensor(func);
                YFunction._AddToCache("GenericSensor", func, obj);
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
        this._valueCallbackGenericSensor = callback;
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
        if (this._valueCallbackGenericSensor != null) {
            this._valueCallbackGenericSensor(this, value);
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
     *   arguments: the function object of which the value has changed, and an <c>YMeasure</c> object describing
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
        this._timedReportCallbackGenericSensor = callback;
        return 0;
    }


    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackGenericSensor != null) {
            this._timedReportCallbackGenericSensor(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Adjusts the signal bias so that the current signal value is need
     *   precisely as zero.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int zeroAdjust()
    {
        double currSignal;
        double currBias;
        currSignal = this.get_signalValue();
        currBias = this.get_signalBias();
        return this.set_signalBias(currSignal + currBias);
    }

    /**
     * <summary>
     *   Continues the enumeration of generic sensors started using <c>yFirstGenericSensor()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned generic sensors order.
     *   If you want to find a specific a generic sensor, use <c>GenericSensor.findGenericSensor()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YGenericSensor</c> object, corresponding to
     *   a generic sensor currently online, or a <c>null</c> pointer
     *   if there are no more generic sensors to enumerate.
     * </returns>
     */
    public YGenericSensor nextGenericSensor()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindGenericSensor(hwid);
    }

    //--- (end of YGenericSensor implementation)

    //--- (YGenericSensor functions)

    /**
     * <summary>
     *   Starts the enumeration of generic sensors currently accessible.
     * <para>
     *   Use the method <c>YGenericSensor.nextGenericSensor()</c> to iterate on
     *   next generic sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YGenericSensor</c> object, corresponding to
     *   the first generic sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YGenericSensor FirstGenericSensor()
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
        err = YAPI.apiGetFunctionsByClass("GenericSensor", 0, p, size, ref neededsize, ref errmsg);
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
        return FindGenericSensor(serial + "." + funcId);
    }



    //--- (end of YGenericSensor functions)
}
#pragma warning restore 1591
