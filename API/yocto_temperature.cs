/*********************************************************************
 *
 *  $Id: yocto_temperature.cs 38899 2019-12-20 17:21:03Z mvuilleu $
 *
 *  Implements yFindTemperature(), the high-level API for Temperature functions
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
    //--- (YTemperature return codes)
    //--- (end of YTemperature return codes)
//--- (YTemperature dlldef)
//--- (end of YTemperature dlldef)
//--- (YTemperature yapiwrapper)
//--- (end of YTemperature yapiwrapper)
//--- (YTemperature class start)
/**
 * <summary>
 *   The <c>YTemperature</c> class allows you to read and configure Yoctopuce temperature sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 *   This class adds the ability to configure some specific parameters
 *   for some sensors (connection type, temperature mapping table).
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YTemperature : YSensor
{
//--- (end of YTemperature class start)
    //--- (YTemperature definitions)
    public new delegate void ValueCallback(YTemperature func, string value);
    public new delegate void TimedReportCallback(YTemperature func, YMeasure measure);

    public const int SENSORTYPE_DIGITAL = 0;
    public const int SENSORTYPE_TYPE_K = 1;
    public const int SENSORTYPE_TYPE_E = 2;
    public const int SENSORTYPE_TYPE_J = 3;
    public const int SENSORTYPE_TYPE_N = 4;
    public const int SENSORTYPE_TYPE_R = 5;
    public const int SENSORTYPE_TYPE_S = 6;
    public const int SENSORTYPE_TYPE_T = 7;
    public const int SENSORTYPE_PT100_4WIRES = 8;
    public const int SENSORTYPE_PT100_3WIRES = 9;
    public const int SENSORTYPE_PT100_2WIRES = 10;
    public const int SENSORTYPE_RES_OHM = 11;
    public const int SENSORTYPE_RES_NTC = 12;
    public const int SENSORTYPE_RES_LINEAR = 13;
    public const int SENSORTYPE_RES_INTERNAL = 14;
    public const int SENSORTYPE_IR = 15;
    public const int SENSORTYPE_RES_PT1000 = 16;
    public const int SENSORTYPE_INVALID = -1;
    public const double SIGNALVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const string SIGNALUNIT_INVALID = YAPI.INVALID_STRING;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _sensorType = SENSORTYPE_INVALID;
    protected double _signalValue = SIGNALVALUE_INVALID;
    protected string _signalUnit = SIGNALUNIT_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackTemperature = null;
    protected TimedReportCallback _timedReportCallbackTemperature = null;
    //--- (end of YTemperature definitions)

    public YTemperature(string func)
        : base(func)
    {
        _className = "Temperature";
        //--- (YTemperature attributes initialization)
        //--- (end of YTemperature attributes initialization)
    }

    //--- (YTemperature implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("sensorType"))
        {
            _sensorType = json_val.getInt("sensorType");
        }
        if (json_val.has("signalValue"))
        {
            _signalValue = Math.Round(json_val.getDouble("signalValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("signalUnit"))
        {
            _signalUnit = json_val.getString("signalUnit");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the measuring unit for the measured temperature.
     * <para>
     *   That unit is a string.
     *   If that strings end with the letter F all temperatures values will returned in
     *   Fahrenheit degrees. If that String ends with the letter K all values will be
     *   returned in Kelvin degrees. If that string ends with the letter C all values will be
     *   returned in Celsius degrees.  If the string ends with any other character the
     *   change will be ignored. Remember to call the
     *   <c>saveToFlash()</c> method of the module if the modification must be kept.
     *   WARNING: if a specific calibration is defined for the temperature function, a
     *   unit system change will probably break it.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the measuring unit for the measured temperature
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
     *   Returns the temperature sensor type.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YTemperature.SENSORTYPE_DIGITAL</c>, <c>YTemperature.SENSORTYPE_TYPE_K</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_E</c>, <c>YTemperature.SENSORTYPE_TYPE_J</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_N</c>, <c>YTemperature.SENSORTYPE_TYPE_R</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_S</c>, <c>YTemperature.SENSORTYPE_TYPE_T</c>,
     *   <c>YTemperature.SENSORTYPE_PT100_4WIRES</c>, <c>YTemperature.SENSORTYPE_PT100_3WIRES</c>,
     *   <c>YTemperature.SENSORTYPE_PT100_2WIRES</c>, <c>YTemperature.SENSORTYPE_RES_OHM</c>,
     *   <c>YTemperature.SENSORTYPE_RES_NTC</c>, <c>YTemperature.SENSORTYPE_RES_LINEAR</c>,
     *   <c>YTemperature.SENSORTYPE_RES_INTERNAL</c>, <c>YTemperature.SENSORTYPE_IR</c> and
     *   <c>YTemperature.SENSORTYPE_RES_PT1000</c> corresponding to the temperature sensor type
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YTemperature.SENSORTYPE_INVALID</c>.
     * </para>
     */
    public int get_sensorType()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SENSORTYPE_INVALID;
                }
            }
            res = this._sensorType;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the temperature sensor type.
     * <para>
     *   This function is used
     *   to define the type of thermocouple (K,E...) used with the device.
     *   It has no effect if module is using a digital sensor or a thermistor.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YTemperature.SENSORTYPE_DIGITAL</c>, <c>YTemperature.SENSORTYPE_TYPE_K</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_E</c>, <c>YTemperature.SENSORTYPE_TYPE_J</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_N</c>, <c>YTemperature.SENSORTYPE_TYPE_R</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_S</c>, <c>YTemperature.SENSORTYPE_TYPE_T</c>,
     *   <c>YTemperature.SENSORTYPE_PT100_4WIRES</c>, <c>YTemperature.SENSORTYPE_PT100_3WIRES</c>,
     *   <c>YTemperature.SENSORTYPE_PT100_2WIRES</c>, <c>YTemperature.SENSORTYPE_RES_OHM</c>,
     *   <c>YTemperature.SENSORTYPE_RES_NTC</c>, <c>YTemperature.SENSORTYPE_RES_LINEAR</c>,
     *   <c>YTemperature.SENSORTYPE_RES_INTERNAL</c>, <c>YTemperature.SENSORTYPE_IR</c> and
     *   <c>YTemperature.SENSORTYPE_RES_PT1000</c> corresponding to the temperature sensor type
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
    public int set_sensorType(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("sensorType", rest_val);
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
     *   On failure, throws an exception or returns <c>YTemperature.SIGNALVALUE_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YTemperature.SIGNALUNIT_INVALID</c>.
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
     *   Retrieves a temperature sensor for a given identifier.
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
     *   This function does not require that the temperature sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YTemperature.isOnline()</c> to test if the temperature sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a temperature sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the temperature sensor, for instance
     *   <c>METEOMK2.temperature</c>.
     * </param>
     * <returns>
     *   a <c>YTemperature</c> object allowing you to drive the temperature sensor.
     * </returns>
     */
    public static YTemperature FindTemperature(string func)
    {
        YTemperature obj;
        lock (YAPI.globalLock) {
            obj = (YTemperature) YFunction._FindFromCache("Temperature", func);
            if (obj == null) {
                obj = new YTemperature(func);
                YFunction._AddToCache("Temperature", func, obj);
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
        this._valueCallbackTemperature = callback;
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
        if (this._valueCallbackTemperature != null) {
            this._valueCallbackTemperature(this, value);
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
        this._timedReportCallbackTemperature = callback;
        return 0;
    }


    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackTemperature != null) {
            this._timedReportCallbackTemperature(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Configures NTC thermistor parameters in order to properly compute the temperature from
     *   the measured resistance.
     * <para>
     *   For increased precision, you can enter a complete mapping
     *   table using set_thermistorResponseTable. This function can only be used with a
     *   temperature sensor based on thermistors.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="res25">
     *   thermistor resistance at 25 degrees Celsius
     * </param>
     * <param name="beta">
     *   Beta value
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_ntcParameters(double res25, double beta)
    {
        double t0;
        double t1;
        double res100;
        List<double> tempValues = new List<double>();
        List<double> resValues = new List<double>();
        t0 = 25.0+275.15;
        t1 = 100.0+275.15;
        res100 = res25 * Math.Exp(beta*(1.0/t1 - 1.0/t0));
        tempValues.Clear();
        resValues.Clear();
        tempValues.Add(25.0);
        resValues.Add(res25);
        tempValues.Add(100.0);
        resValues.Add(res100);
        return this.set_thermistorResponseTable(tempValues, resValues);
    }


    /**
     * <summary>
     *   Records a thermistor response table, in order to interpolate the temperature from
     *   the measured resistance.
     * <para>
     *   This function can only be used with a temperature
     *   sensor based on thermistors.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tempValues">
     *   array of floating point numbers, corresponding to all
     *   temperatures (in degrees Celsius) for which the resistance of the
     *   thermistor is specified.
     * </param>
     * <param name="resValues">
     *   array of floating point numbers, corresponding to the resistance
     *   values (in Ohms) for each of the temperature included in the first
     *   argument, index by index.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_thermistorResponseTable(List<double> tempValues, List<double> resValues)
    {
        int siz;
        int res;
        int idx;
        int found;
        double prev;
        double curr;
        double currTemp;
        double idxres;
        siz = tempValues.Count;
        if (!(siz >= 2)) { this._throw( YAPI.INVALID_ARGUMENT, "thermistor response table must have at least two points"); return YAPI.INVALID_ARGUMENT; }
        if (!(siz == resValues.Count)) { this._throw( YAPI.INVALID_ARGUMENT, "table sizes mismatch"); return YAPI.INVALID_ARGUMENT; }

        res = this.set_command("Z");
        if (!(res==YAPI.SUCCESS)) { this._throw( YAPI.IO_ERROR, "unable to reset thermistor parameters"); return YAPI.IO_ERROR; }
        // add records in growing resistance value
        found = 1;
        prev = 0.0;
        while (found > 0) {
            found = 0;
            curr = 99999999.0;
            currTemp = -999999.0;
            idx = 0;
            while (idx < siz) {
                idxres = resValues[idx];
                if ((idxres > prev) && (idxres < curr)) {
                    curr = idxres;
                    currTemp = tempValues[idx];
                    found = 1;
                }
                idx = idx + 1;
            }
            if (found > 0) {
                res = this.set_command("m"+Convert.ToString( (int) Math.Round(1000*curr))+":"+Convert.ToString((int) Math.Round(1000*currTemp)));
                if (!(res==YAPI.SUCCESS)) { this._throw( YAPI.IO_ERROR, "unable to reset thermistor parameters"); return YAPI.IO_ERROR; }
                prev = curr;
            }
        }
        return YAPI.SUCCESS;
    }


    /**
     * <summary>
     *   Retrieves the thermistor response table previously configured using the
     *   <c>set_thermistorResponseTable</c> function.
     * <para>
     *   This function can only be used with a
     *   temperature sensor based on thermistors.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tempValues">
     *   array of floating point numbers, that is filled by the function
     *   with all temperatures (in degrees Celsius) for which the resistance
     *   of the thermistor is specified.
     * </param>
     * <param name="resValues">
     *   array of floating point numbers, that is filled by the function
     *   with the value (in Ohms) for each of the temperature included in the
     *   first argument, index by index.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int loadThermistorResponseTable(List<double> tempValues, List<double> resValues)
    {
        string id;
        byte[] bin_json;
        List<string> paramlist = new List<string>();
        List<double> templist = new List<double>();
        int siz;
        int idx;
        double temp;
        int found;
        double prev;
        double curr;
        double currRes;
        tempValues.Clear();
        resValues.Clear();

        id = this.get_functionId();
        id = (id).Substring( 11, (id).Length - 11);
        if (id == "") {
            id = "1";
        }
        bin_json = this._download("extra.json?page="+id);
        paramlist = this._json_get_array(bin_json);
        // first convert all temperatures to float
        siz = ((paramlist.Count) >> (1));
        templist.Clear();
        idx = 0;
        while (idx < siz) {
            temp = Double.Parse(paramlist[2*idx+1])/1000.0;
            templist.Add(temp);
            idx = idx + 1;
        }
        // then add records in growing temperature value
        tempValues.Clear();
        resValues.Clear();
        found = 1;
        prev = -999999.0;
        while (found > 0) {
            found = 0;
            curr = 999999.0;
            currRes = -999999.0;
            idx = 0;
            while (idx < siz) {
                temp = templist[idx];
                if ((temp > prev) && (temp < curr)) {
                    curr = temp;
                    currRes = Double.Parse(paramlist[2*idx])/1000.0;
                    found = 1;
                }
                idx = idx + 1;
            }
            if (found > 0) {
                tempValues.Add(curr);
                resValues.Add(currRes);
                prev = curr;
            }
        }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Continues the enumeration of temperature sensors started using <c>yFirstTemperature()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned temperature sensors order.
     *   If you want to find a specific a temperature sensor, use <c>Temperature.findTemperature()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YTemperature</c> object, corresponding to
     *   a temperature sensor currently online, or a <c>null</c> pointer
     *   if there are no more temperature sensors to enumerate.
     * </returns>
     */
    public YTemperature nextTemperature()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindTemperature(hwid);
    }

    //--- (end of YTemperature implementation)

    //--- (YTemperature functions)

    /**
     * <summary>
     *   Starts the enumeration of temperature sensors currently accessible.
     * <para>
     *   Use the method <c>YTemperature.nextTemperature()</c> to iterate on
     *   next temperature sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YTemperature</c> object, corresponding to
     *   the first temperature sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YTemperature FirstTemperature()
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
        err = YAPI.apiGetFunctionsByClass("Temperature", 0, p, size, ref neededsize, ref errmsg);
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
        return FindTemperature(serial + "." + funcId);
    }



    //--- (end of YTemperature functions)
}
#pragma warning restore 1591
