/*********************************************************************
 *
 *  $Id: yocto_powersupply.cs 37827 2019-10-25 13:07:48Z mvuilleu $
 *
 *  Implements yFindPowerSupply(), the high-level API for PowerSupply functions
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
    //--- (YPowerSupply return codes)
    //--- (end of YPowerSupply return codes)
//--- (YPowerSupply dlldef)
//--- (end of YPowerSupply dlldef)
//--- (YPowerSupply yapiwrapper)
//--- (end of YPowerSupply yapiwrapper)
//--- (YPowerSupply class start)
/**
 * <summary>
 *   The YPowerSupply class allows you to drive a Yoctopuce power supply$DEV_ENà.
 * <para>
 *   It can be use to change the voltage set point,
 *   the current limit and the enable/disable the output.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YPowerSupply : YFunction
{
//--- (end of YPowerSupply class start)
    //--- (YPowerSupply definitions)
    public new delegate void ValueCallback(YPowerSupply func, string value);
    public new delegate void TimedReportCallback(YPowerSupply func, YMeasure measure);

    public const double VOLTAGESETPOINT_INVALID = YAPI.INVALID_DOUBLE;
    public const double CURRENTLIMIT_INVALID = YAPI.INVALID_DOUBLE;
    public const int POWEROUTPUT_OFF = 0;
    public const int POWEROUTPUT_ON = 1;
    public const int POWEROUTPUT_INVALID = -1;
    public const int VOLTAGESENSE_INT = 0;
    public const int VOLTAGESENSE_EXT = 1;
    public const int VOLTAGESENSE_INVALID = -1;
    public const double MEASUREDVOLTAGE_INVALID = YAPI.INVALID_DOUBLE;
    public const double MEASUREDCURRENT_INVALID = YAPI.INVALID_DOUBLE;
    public const double INPUTVOLTAGE_INVALID = YAPI.INVALID_DOUBLE;
    public const double VINT_INVALID = YAPI.INVALID_DOUBLE;
    public const double LDOTEMPERATURE_INVALID = YAPI.INVALID_DOUBLE;
    public const string VOLTAGETRANSITION_INVALID = YAPI.INVALID_STRING;
    public const double VOLTAGEATSTARTUP_INVALID = YAPI.INVALID_DOUBLE;
    public const double CURRENTATSTARTUP_INVALID = YAPI.INVALID_DOUBLE;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected double _voltageSetPoint = VOLTAGESETPOINT_INVALID;
    protected double _currentLimit = CURRENTLIMIT_INVALID;
    protected int _powerOutput = POWEROUTPUT_INVALID;
    protected int _voltageSense = VOLTAGESENSE_INVALID;
    protected double _measuredVoltage = MEASUREDVOLTAGE_INVALID;
    protected double _measuredCurrent = MEASUREDCURRENT_INVALID;
    protected double _inputVoltage = INPUTVOLTAGE_INVALID;
    protected double _vInt = VINT_INVALID;
    protected double _ldoTemperature = LDOTEMPERATURE_INVALID;
    protected string _voltageTransition = VOLTAGETRANSITION_INVALID;
    protected double _voltageAtStartUp = VOLTAGEATSTARTUP_INVALID;
    protected double _currentAtStartUp = CURRENTATSTARTUP_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackPowerSupply = null;
    //--- (end of YPowerSupply definitions)

    public YPowerSupply(string func)
        : base(func)
    {
        _className = "PowerSupply";
        //--- (YPowerSupply attributes initialization)
        //--- (end of YPowerSupply attributes initialization)
    }

    //--- (YPowerSupply implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("voltageSetPoint"))
        {
            _voltageSetPoint = Math.Round(json_val.getDouble("voltageSetPoint") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("currentLimit"))
        {
            _currentLimit = Math.Round(json_val.getDouble("currentLimit") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("powerOutput"))
        {
            _powerOutput = json_val.getInt("powerOutput") > 0 ? 1 : 0;
        }
        if (json_val.has("voltageSense"))
        {
            _voltageSense = json_val.getInt("voltageSense");
        }
        if (json_val.has("measuredVoltage"))
        {
            _measuredVoltage = Math.Round(json_val.getDouble("measuredVoltage") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("measuredCurrent"))
        {
            _measuredCurrent = Math.Round(json_val.getDouble("measuredCurrent") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("inputVoltage"))
        {
            _inputVoltage = Math.Round(json_val.getDouble("inputVoltage") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("vInt"))
        {
            _vInt = Math.Round(json_val.getDouble("vInt") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("ldoTemperature"))
        {
            _ldoTemperature = Math.Round(json_val.getDouble("ldoTemperature") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("voltageTransition"))
        {
            _voltageTransition = json_val.getString("voltageTransition");
        }
        if (json_val.has("voltageAtStartUp"))
        {
            _voltageAtStartUp = Math.Round(json_val.getDouble("voltageAtStartUp") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("currentAtStartUp"))
        {
            _currentAtStartUp = Math.Round(json_val.getDouble("currentAtStartUp") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the voltage set point, in V.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the voltage set point, in V
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
    public int set_voltageSetPoint(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("voltageSetPoint", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the voltage set point, in V.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the voltage set point, in V
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.VOLTAGESETPOINT_INVALID</c>.
     * </para>
     */
    public double get_voltageSetPoint()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VOLTAGESETPOINT_INVALID;
                }
            }
            res = this._voltageSetPoint;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the current limit, in mA.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the current limit, in mA
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
    public int set_currentLimit(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("currentLimit", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the current limit, in mA.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current limit, in mA
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.CURRENTLIMIT_INVALID</c>.
     * </para>
     */
    public double get_currentLimit()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENTLIMIT_INVALID;
                }
            }
            res = this._currentLimit;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the power supply output switch state.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YPowerSupply.POWEROUTPUT_OFF</c> or <c>YPowerSupply.POWEROUTPUT_ON</c>, according to the
     *   power supply output switch state
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.POWEROUTPUT_INVALID</c>.
     * </para>
     */
    public int get_powerOutput()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return POWEROUTPUT_INVALID;
                }
            }
            res = this._powerOutput;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the power supply output switch state.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YPowerSupply.POWEROUTPUT_OFF</c> or <c>YPowerSupply.POWEROUTPUT_ON</c>, according to the
     *   power supply output switch state
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
    public int set_powerOutput(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("powerOutput", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the output voltage control point.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YPowerSupply.VOLTAGESENSE_INT</c> or <c>YPowerSupply.VOLTAGESENSE_EXT</c>, according to
     *   the output voltage control point
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.VOLTAGESENSE_INVALID</c>.
     * </para>
     */
    public int get_voltageSense()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VOLTAGESENSE_INVALID;
                }
            }
            res = this._voltageSense;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the voltage control point.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YPowerSupply.VOLTAGESENSE_INT</c> or <c>YPowerSupply.VOLTAGESENSE_EXT</c>, according to
     *   the voltage control point
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
    public int set_voltageSense(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("voltageSense", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the measured output voltage, in V.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the measured output voltage, in V
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.MEASUREDVOLTAGE_INVALID</c>.
     * </para>
     */
    public double get_measuredVoltage()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MEASUREDVOLTAGE_INVALID;
                }
            }
            res = this._measuredVoltage;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the measured output current, in mA.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the measured output current, in mA
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.MEASUREDCURRENT_INVALID</c>.
     * </para>
     */
    public double get_measuredCurrent()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MEASUREDCURRENT_INVALID;
                }
            }
            res = this._measuredCurrent;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the measured input voltage, in V.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the measured input voltage, in V
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.INPUTVOLTAGE_INVALID</c>.
     * </para>
     */
    public double get_inputVoltage()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return INPUTVOLTAGE_INVALID;
                }
            }
            res = this._inputVoltage;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the internal voltage, in V.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the internal voltage, in V
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.VINT_INVALID</c>.
     * </para>
     */
    public double get_vInt()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VINT_INVALID;
                }
            }
            res = this._vInt;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the LDO temperature, in Celsius.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the LDO temperature, in Celsius
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.LDOTEMPERATURE_INVALID</c>.
     * </para>
     */
    public double get_ldoTemperature()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LDOTEMPERATURE_INVALID;
                }
            }
            res = this._ldoTemperature;
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
     *   Changes the voltage set point at device start up.
     * <para>
     *   Remember to call the matching
     *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the voltage set point at device start up
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
     *   Returns the selected voltage set point at device startup, in V.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the selected voltage set point at device startup, in V
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.VOLTAGEATSTARTUP_INVALID</c>.
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
     *   Changes the current limit at device start up.
     * <para>
     *   Remember to call the matching
     *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the current limit at device start up
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
     *   Returns the selected current limit at device startup, in mA.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the selected current limit at device startup, in mA
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerSupply.CURRENTATSTARTUP_INVALID</c>.
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
     *   Retrieves a regulated power supply for a given identifier.
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
     *   This function does not require that the regulated power supply is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YPowerSupply.isOnline()</c> to test if the regulated power supply is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a regulated power supply by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the regulated power supply, for instance
     *   <c>MyDevice.powerSupply</c>.
     * </param>
     * <returns>
     *   a <c>YPowerSupply</c> object allowing you to drive the regulated power supply.
     * </returns>
     */
    public static YPowerSupply FindPowerSupply(string func)
    {
        YPowerSupply obj;
        lock (YAPI.globalLock) {
            obj = (YPowerSupply) YFunction._FindFromCache("PowerSupply", func);
            if (obj == null) {
                obj = new YPowerSupply(func);
                YFunction._AddToCache("PowerSupply", func, obj);
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
        this._valueCallbackPowerSupply = callback;
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
        if (this._valueCallbackPowerSupply != null) {
            this._valueCallbackPowerSupply(this, value);
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
        newval = ""+Convert.ToString( (int) Math.Round(V_target*65536))+":"+Convert.ToString(ms_duration);

        return this.set_voltageTransition(newval);
    }

    /**
     * <summary>
     *   Continues the enumeration of regulated power supplies started using <c>yFirstPowerSupply()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned regulated power supplies order.
     *   If you want to find a specific a regulated power supply, use <c>PowerSupply.findPowerSupply()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YPowerSupply</c> object, corresponding to
     *   a regulated power supply currently online, or a <c>null</c> pointer
     *   if there are no more regulated power supplies to enumerate.
     * </returns>
     */
    public YPowerSupply nextPowerSupply()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindPowerSupply(hwid);
    }

    //--- (end of YPowerSupply implementation)

    //--- (YPowerSupply functions)

    /**
     * <summary>
     *   Starts the enumeration of regulated power supplies currently accessible.
     * <para>
     *   Use the method <c>YPowerSupply.nextPowerSupply()</c> to iterate on
     *   next regulated power supplies.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YPowerSupply</c> object, corresponding to
     *   the first regulated power supply currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YPowerSupply FirstPowerSupply()
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
        err = YAPI.apiGetFunctionsByClass("PowerSupply", 0, p, size, ref neededsize, ref errmsg);
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
        return FindPowerSupply(serial + "." + funcId);
    }



    //--- (end of YPowerSupply functions)
}
#pragma warning restore 1591
