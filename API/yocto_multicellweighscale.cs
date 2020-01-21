/*********************************************************************
 *
 *  $Id: yocto_multicellweighscale.cs 38899 2019-12-20 17:21:03Z mvuilleu $
 *
 *  Implements yFindMultiCellWeighScale(), the high-level API for MultiCellWeighScale functions
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
    //--- (YMultiCellWeighScale return codes)
    //--- (end of YMultiCellWeighScale return codes)
//--- (YMultiCellWeighScale dlldef)
//--- (end of YMultiCellWeighScale dlldef)
//--- (YMultiCellWeighScale yapiwrapper)
//--- (end of YMultiCellWeighScale yapiwrapper)
//--- (YMultiCellWeighScale class start)
/**
 * <summary>
 *   The <c>YMultiCellWeighScale</c> class provides a weight measurement from a set of ratiometric
 *   sensors.
 * <para>
 *   It can be used to control the bridge excitation parameters, in order to avoid
 *   measure shifts caused by temperature variation in the electronics, and can also
 *   automatically apply an additional correction factor based on temperature to
 *   compensate for offsets in the load cells themselves.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YMultiCellWeighScale : YSensor
{
//--- (end of YMultiCellWeighScale class start)
    //--- (YMultiCellWeighScale definitions)
    public new delegate void ValueCallback(YMultiCellWeighScale func, string value);
    public new delegate void TimedReportCallback(YMultiCellWeighScale func, YMeasure measure);

    public const int CELLCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int EXCITATION_OFF = 0;
    public const int EXCITATION_DC = 1;
    public const int EXCITATION_AC = 2;
    public const int EXCITATION_INVALID = -1;
    public const double TEMPAVGADAPTRATIO_INVALID = YAPI.INVALID_DOUBLE;
    public const double TEMPCHGADAPTRATIO_INVALID = YAPI.INVALID_DOUBLE;
    public const double COMPTEMPAVG_INVALID = YAPI.INVALID_DOUBLE;
    public const double COMPTEMPCHG_INVALID = YAPI.INVALID_DOUBLE;
    public const double COMPENSATION_INVALID = YAPI.INVALID_DOUBLE;
    public const double ZEROTRACKING_INVALID = YAPI.INVALID_DOUBLE;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _cellCount = CELLCOUNT_INVALID;
    protected int _excitation = EXCITATION_INVALID;
    protected double _tempAvgAdaptRatio = TEMPAVGADAPTRATIO_INVALID;
    protected double _tempChgAdaptRatio = TEMPCHGADAPTRATIO_INVALID;
    protected double _compTempAvg = COMPTEMPAVG_INVALID;
    protected double _compTempChg = COMPTEMPCHG_INVALID;
    protected double _compensation = COMPENSATION_INVALID;
    protected double _zeroTracking = ZEROTRACKING_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackMultiCellWeighScale = null;
    protected TimedReportCallback _timedReportCallbackMultiCellWeighScale = null;
    //--- (end of YMultiCellWeighScale definitions)

    public YMultiCellWeighScale(string func)
        : base(func)
    {
        _className = "MultiCellWeighScale";
        //--- (YMultiCellWeighScale attributes initialization)
        //--- (end of YMultiCellWeighScale attributes initialization)
    }

    //--- (YMultiCellWeighScale implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("cellCount"))
        {
            _cellCount = json_val.getInt("cellCount");
        }
        if (json_val.has("excitation"))
        {
            _excitation = json_val.getInt("excitation");
        }
        if (json_val.has("tempAvgAdaptRatio"))
        {
            _tempAvgAdaptRatio = Math.Round(json_val.getDouble("tempAvgAdaptRatio") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("tempChgAdaptRatio"))
        {
            _tempChgAdaptRatio = Math.Round(json_val.getDouble("tempChgAdaptRatio") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("compTempAvg"))
        {
            _compTempAvg = Math.Round(json_val.getDouble("compTempAvg") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("compTempChg"))
        {
            _compTempChg = Math.Round(json_val.getDouble("compTempChg") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("compensation"))
        {
            _compensation = Math.Round(json_val.getDouble("compensation") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("zeroTracking"))
        {
            _zeroTracking = Math.Round(json_val.getDouble("zeroTracking") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the measuring unit for the weight.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the measuring unit for the weight
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
     *   Returns the number of load cells in use.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of load cells in use
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiCellWeighScale.CELLCOUNT_INVALID</c>.
     * </para>
     */
    public int get_cellCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CELLCOUNT_INVALID;
                }
            }
            res = this._cellCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the number of load cells in use.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the number of load cells in use
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
    public int set_cellCount(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("cellCount", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the current load cell bridge excitation method.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YMultiCellWeighScale.EXCITATION_OFF</c>, <c>YMultiCellWeighScale.EXCITATION_DC</c>
     *   and <c>YMultiCellWeighScale.EXCITATION_AC</c> corresponding to the current load cell bridge excitation method
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiCellWeighScale.EXCITATION_INVALID</c>.
     * </para>
     */
    public int get_excitation()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return EXCITATION_INVALID;
                }
            }
            res = this._excitation;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the current load cell bridge excitation method.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YMultiCellWeighScale.EXCITATION_OFF</c>, <c>YMultiCellWeighScale.EXCITATION_DC</c>
     *   and <c>YMultiCellWeighScale.EXCITATION_AC</c> corresponding to the current load cell bridge excitation method
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
    public int set_excitation(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("excitation", rest_val);
        }
    }

    /**
     * <summary>
     *   Changes the averaged temperature update rate, in per mille.
     * <para>
     *   The purpose of this adaptation ratio is to model the thermal inertia of the load cell.
     *   The averaged temperature is updated every 10 seconds, by applying this adaptation rate
     *   to the difference between the measures ambient temperature and the current compensation
     *   temperature. The standard rate is 0.2 per mille, and the maximal rate is 65 per mille.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the averaged temperature update rate, in per mille
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
    public int set_tempAvgAdaptRatio(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("tempAvgAdaptRatio", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the averaged temperature update rate, in per mille.
     * <para>
     *   The purpose of this adaptation ratio is to model the thermal inertia of the load cell.
     *   The averaged temperature is updated every 10 seconds, by applying this adaptation rate
     *   to the difference between the measures ambient temperature and the current compensation
     *   temperature. The standard rate is 0.2 per mille, and the maximal rate is 65 per mille.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the averaged temperature update rate, in per mille
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiCellWeighScale.TEMPAVGADAPTRATIO_INVALID</c>.
     * </para>
     */
    public double get_tempAvgAdaptRatio()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return TEMPAVGADAPTRATIO_INVALID;
                }
            }
            res = this._tempAvgAdaptRatio;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the temperature change update rate, in per mille.
     * <para>
     *   The temperature change is updated every 10 seconds, by applying this adaptation rate
     *   to the difference between the measures ambient temperature and the current temperature used for
     *   change compensation. The standard rate is 0.6 per mille, and the maximal rate is 65 per mille.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the temperature change update rate, in per mille
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
    public int set_tempChgAdaptRatio(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("tempChgAdaptRatio", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the temperature change update rate, in per mille.
     * <para>
     *   The temperature change is updated every 10 seconds, by applying this adaptation rate
     *   to the difference between the measures ambient temperature and the current temperature used for
     *   change compensation. The standard rate is 0.6 per mille, and the maximal rate is 65 per mille.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the temperature change update rate, in per mille
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiCellWeighScale.TEMPCHGADAPTRATIO_INVALID</c>.
     * </para>
     */
    public double get_tempChgAdaptRatio()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return TEMPCHGADAPTRATIO_INVALID;
                }
            }
            res = this._tempChgAdaptRatio;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the current averaged temperature, used for thermal compensation.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current averaged temperature, used for thermal compensation
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiCellWeighScale.COMPTEMPAVG_INVALID</c>.
     * </para>
     */
    public double get_compTempAvg()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return COMPTEMPAVG_INVALID;
                }
            }
            res = this._compTempAvg;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the current temperature variation, used for thermal compensation.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current temperature variation, used for thermal compensation
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiCellWeighScale.COMPTEMPCHG_INVALID</c>.
     * </para>
     */
    public double get_compTempChg()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return COMPTEMPCHG_INVALID;
                }
            }
            res = this._compTempChg;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the current current thermal compensation value.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current current thermal compensation value
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiCellWeighScale.COMPENSATION_INVALID</c>.
     * </para>
     */
    public double get_compensation()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return COMPENSATION_INVALID;
                }
            }
            res = this._compensation;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the zero tracking threshold value.
     * <para>
     *   When this threshold is larger than
     *   zero, any measure under the threshold will automatically be ignored and the
     *   zero compensation will be updated.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the zero tracking threshold value
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
    public int set_zeroTracking(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("zeroTracking", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the zero tracking threshold value.
     * <para>
     *   When this threshold is larger than
     *   zero, any measure under the threshold will automatically be ignored and the
     *   zero compensation will be updated.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the zero tracking threshold value
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiCellWeighScale.ZEROTRACKING_INVALID</c>.
     * </para>
     */
    public double get_zeroTracking()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ZEROTRACKING_INVALID;
                }
            }
            res = this._zeroTracking;
        }
        return res;
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
     *   Retrieves a multi-cell weighing scale sensor for a given identifier.
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
     *   This function does not require that the multi-cell weighing scale sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YMultiCellWeighScale.isOnline()</c> to test if the multi-cell weighing scale sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a multi-cell weighing scale sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the multi-cell weighing scale sensor, for instance
     *   <c>YWMBRDG1.multiCellWeighScale</c>.
     * </param>
     * <returns>
     *   a <c>YMultiCellWeighScale</c> object allowing you to drive the multi-cell weighing scale sensor.
     * </returns>
     */
    public static YMultiCellWeighScale FindMultiCellWeighScale(string func)
    {
        YMultiCellWeighScale obj;
        lock (YAPI.globalLock) {
            obj = (YMultiCellWeighScale) YFunction._FindFromCache("MultiCellWeighScale", func);
            if (obj == null) {
                obj = new YMultiCellWeighScale(func);
                YFunction._AddToCache("MultiCellWeighScale", func, obj);
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
        this._valueCallbackMultiCellWeighScale = callback;
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
        if (this._valueCallbackMultiCellWeighScale != null) {
            this._valueCallbackMultiCellWeighScale(this, value);
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
        this._timedReportCallbackMultiCellWeighScale = callback;
        return 0;
    }


    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackMultiCellWeighScale != null) {
            this._timedReportCallbackMultiCellWeighScale(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Adapts the load cell signal bias (stored in the corresponding genericSensor)
     *   so that the current signal corresponds to a zero weight.
     * <para>
     * </para>
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
    public virtual int tare()
    {
        return this.set_command("T");
    }


    /**
     * <summary>
     *   Configures the load cells span parameters (stored in the corresponding genericSensors)
     *   so that the current signal corresponds to the specified reference weight.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="currWeight">
     *   reference weight presently on the load cell.
     * </param>
     * <param name="maxWeight">
     *   maximum weight to be expected on the load cell.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int setupSpan(double currWeight, double maxWeight)
    {
        return this.set_command("S"+Convert.ToString( (int) Math.Round(1000*currWeight))+":"+Convert.ToString((int) Math.Round(1000*maxWeight)));
    }

    /**
     * <summary>
     *   Continues the enumeration of multi-cell weighing scale sensors started using <c>yFirstMultiCellWeighScale()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned multi-cell weighing scale sensors order.
     *   If you want to find a specific a multi-cell weighing scale sensor, use
     *   <c>MultiCellWeighScale.findMultiCellWeighScale()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YMultiCellWeighScale</c> object, corresponding to
     *   a multi-cell weighing scale sensor currently online, or a <c>null</c> pointer
     *   if there are no more multi-cell weighing scale sensors to enumerate.
     * </returns>
     */
    public YMultiCellWeighScale nextMultiCellWeighScale()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindMultiCellWeighScale(hwid);
    }

    //--- (end of YMultiCellWeighScale implementation)

    //--- (YMultiCellWeighScale functions)

    /**
     * <summary>
     *   Starts the enumeration of multi-cell weighing scale sensors currently accessible.
     * <para>
     *   Use the method <c>YMultiCellWeighScale.nextMultiCellWeighScale()</c> to iterate on
     *   next multi-cell weighing scale sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YMultiCellWeighScale</c> object, corresponding to
     *   the first multi-cell weighing scale sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YMultiCellWeighScale FirstMultiCellWeighScale()
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
        err = YAPI.apiGetFunctionsByClass("MultiCellWeighScale", 0, p, size, ref neededsize, ref errmsg);
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
        return FindMultiCellWeighScale(serial + "." + funcId);
    }



    //--- (end of YMultiCellWeighScale functions)
}
#pragma warning restore 1591
