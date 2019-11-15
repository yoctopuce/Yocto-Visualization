/*********************************************************************
 *
 *  $Id: yocto_arithmeticsensor.cs 37827 2019-10-25 13:07:48Z mvuilleu $
 *
 *  Implements yFindArithmeticSensor(), the high-level API for ArithmeticSensor functions
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
    //--- (YArithmeticSensor return codes)
    //--- (end of YArithmeticSensor return codes)
//--- (YArithmeticSensor dlldef)
//--- (end of YArithmeticSensor dlldef)
//--- (YArithmeticSensor yapiwrapper)
//--- (end of YArithmeticSensor yapiwrapper)
//--- (YArithmeticSensor class start)
/**
 * <summary>
 *   The YArithmeticSensor class allows some Yoctopuce devices to compute in real-time
 *   values based on an arithmetic formula involving one or more measured signals as
 *   well as the temperature.
 * <para>
 *   This functionality is only available on specific
 *   Yoctopuce devices, for instance using a Yocto-MaxiMicroVolt-Rx.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YArithmeticSensor : YSensor
{
//--- (end of YArithmeticSensor class start)
    //--- (YArithmeticSensor definitions)
    public new delegate void ValueCallback(YArithmeticSensor func, string value);
    public new delegate void TimedReportCallback(YArithmeticSensor func, YMeasure measure);

    public const string DESCRIPTION_INVALID = YAPI.INVALID_STRING;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected string _description = DESCRIPTION_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackArithmeticSensor = null;
    protected TimedReportCallback _timedReportCallbackArithmeticSensor = null;
    //--- (end of YArithmeticSensor definitions)

    public YArithmeticSensor(string func)
        : base(func)
    {
        _className = "ArithmeticSensor";
        //--- (YArithmeticSensor attributes initialization)
        //--- (end of YArithmeticSensor attributes initialization)
    }

    //--- (YArithmeticSensor implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("description"))
        {
            _description = json_val.getString("description");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the measuring unit for the arithmetic sensor.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the measuring unit for the arithmetic sensor
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
     *   Returns a short informative description of the formula.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to a short informative description of the formula
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YArithmeticSensor.DESCRIPTION_INVALID</c>.
     * </para>
     */
    public string get_description()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DESCRIPTION_INVALID;
                }
            }
            res = this._description;
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
     *   Retrieves an arithmetic sensor for a given identifier.
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
     *   This function does not require that the arithmetic sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YArithmeticSensor.isOnline()</c> to test if the arithmetic sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   an arithmetic sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the arithmetic sensor, for instance
     *   <c>RXUVOLT1.arithmeticSensor1</c>.
     * </param>
     * <returns>
     *   a <c>YArithmeticSensor</c> object allowing you to drive the arithmetic sensor.
     * </returns>
     */
    public static YArithmeticSensor FindArithmeticSensor(string func)
    {
        YArithmeticSensor obj;
        lock (YAPI.globalLock) {
            obj = (YArithmeticSensor) YFunction._FindFromCache("ArithmeticSensor", func);
            if (obj == null) {
                obj = new YArithmeticSensor(func);
                YFunction._AddToCache("ArithmeticSensor", func, obj);
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
        this._valueCallbackArithmeticSensor = callback;
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
        if (this._valueCallbackArithmeticSensor != null) {
            this._valueCallbackArithmeticSensor(this, value);
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
        this._timedReportCallbackArithmeticSensor = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackArithmeticSensor != null) {
            this._timedReportCallbackArithmeticSensor(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Defines the arithmetic function by means of an algebraic expression.
     * <para>
     *   The expression
     *   may include references to device sensors, by their physical or logical name, to
     *   usual math functions and to auxiliary functions defined separately.
     * </para>
     * </summary>
     * <param name="expr">
     *   the algebraic expression defining the function.
     * </param>
     * <param name="descr">
     *   short informative description of the expression.
     * </param>
     * <returns>
     *   the current expression value if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YAPI.INVALID_DOUBLE.
     * </para>
     */
    public virtual double defineExpression(string expr, string descr)
    {
        string id;
        string fname;
        string content;
        byte[] data;
        string diags;
        double resval;
        id = this.get_functionId();
        id = (id).Substring( 16, (id).Length - 16);
        fname = "arithmExpr"+id+".txt";

        content = "// "+ descr+"\n"+expr;
        data = this._uploadEx(fname, YAPI.DefaultEncoding.GetBytes(content));
        diags = YAPI.DefaultEncoding.GetString(data);
        if (!((diags).Substring(0, 8) == "Result: ")) { this._throw( YAPI.INVALID_ARGUMENT, diags); return YAPI.INVALID_DOUBLE; }
        resval = Double.Parse((diags).Substring( 8, (diags).Length-8));
        return resval;
    }

    /**
     * <summary>
     *   Retrieves the algebraic expression defining the arithmetic function, as previously
     *   configured using the <c>defineExpression</c> function.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string containing the mathematical expression.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual string loadExpression()
    {
        string id;
        string fname;
        string content;
        int idx;
        id = this.get_functionId();
        id = (id).Substring( 16, (id).Length - 16);
        fname = "arithmExpr"+id+".txt";

        content = YAPI.DefaultEncoding.GetString(this._download(fname));
        idx = (content).IndexOf("\n");
        if (idx > 0) {
            content = (content).Substring( idx+1, (content).Length-(idx+1));
        }
        return content;
    }

    /**
     * <summary>
     *   Defines a auxiliary function by means of a table of reference points.
     * <para>
     *   Intermediate values
     *   will be interpolated between specified reference points. The reference points are given
     *   as pairs of floating point numbers.
     *   The auxiliary function will be available for use by all ArithmeticSensor objects of the
     *   device. Up to nine auxiliary function can be defined in a device, each containing up to
     *   96 reference points.
     * </para>
     * </summary>
     * <param name="name">
     *   auxiliary function name, up to 16 characters.
     * </param>
     * <param name="inputValues">
     *   array of floating point numbers, corresponding to the function input value.
     * </param>
     * <param name="outputValues">
     *   array of floating point numbers, corresponding to the output value
     *   desired for each of the input value, index by index.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int defineAuxiliaryFunction(string name, List<double> inputValues, List<double> outputValues)
    {
        int siz;
        string defstr;
        int idx;
        double inputVal;
        double outputVal;
        string fname;
        siz = inputValues.Count;
        if (!(siz > 1)) { this._throw( YAPI.INVALID_ARGUMENT, "auxiliary function must be defined by at least two points"); return YAPI.INVALID_ARGUMENT; }
        if (!(siz == outputValues.Count)) { this._throw( YAPI.INVALID_ARGUMENT, "table sizes mismatch"); return YAPI.INVALID_ARGUMENT; }
        defstr = "";
        idx = 0;
        while (idx < siz) {
            inputVal = inputValues[idx];
            outputVal = outputValues[idx];
            defstr = ""+ defstr+""+YAPI._floatToStr( inputVal)+":"+YAPI._floatToStr(outputVal)+"\n";
            idx = idx + 1;
        }
        fname = "userMap"+name+".txt";

        return this._upload(fname, YAPI.DefaultEncoding.GetBytes(defstr));
    }

    /**
     * <summary>
     *   Retrieves the reference points table defining an auxiliary function previously
     *   configured using the <c>defineAuxiliaryFunction</c> function.
     * <para>
     * </para>
     * </summary>
     * <param name="name">
     *   auxiliary function name, up to 16 characters.
     * </param>
     * <param name="inputValues">
     *   array of floating point numbers, that is filled by the function
     *   with all the function reference input value.
     * </param>
     * <param name="outputValues">
     *   array of floating point numbers, that is filled by the function
     *   output value for each of the input value, index by index.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int loadAuxiliaryFunction(string name, List<double> inputValues, List<double> outputValues)
    {
        string fname;
        byte[] defbin;
        int siz;

        fname = "userMap"+name+".txt";
        defbin = this._download(fname);
        siz = (defbin).Length;
        if (!(siz > 0)) { this._throw( YAPI.INVALID_ARGUMENT, "auxiliary function does not exist"); return YAPI.INVALID_ARGUMENT; }
        inputValues.Clear();
        outputValues.Clear();
        // FIXME: decode line by line
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Continues the enumeration of arithmetic sensors started using <c>yFirstArithmeticSensor()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned arithmetic sensors order.
     *   If you want to find a specific an arithmetic sensor, use <c>ArithmeticSensor.findArithmeticSensor()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YArithmeticSensor</c> object, corresponding to
     *   an arithmetic sensor currently online, or a <c>null</c> pointer
     *   if there are no more arithmetic sensors to enumerate.
     * </returns>
     */
    public YArithmeticSensor nextArithmeticSensor()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindArithmeticSensor(hwid);
    }

    //--- (end of YArithmeticSensor implementation)

    //--- (YArithmeticSensor functions)

    /**
     * <summary>
     *   Starts the enumeration of arithmetic sensors currently accessible.
     * <para>
     *   Use the method <c>YArithmeticSensor.nextArithmeticSensor()</c> to iterate on
     *   next arithmetic sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YArithmeticSensor</c> object, corresponding to
     *   the first arithmetic sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YArithmeticSensor FirstArithmeticSensor()
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
        err = YAPI.apiGetFunctionsByClass("ArithmeticSensor", 0, p, size, ref neededsize, ref errmsg);
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
        return FindArithmeticSensor(serial + "." + funcId);
    }



    //--- (end of YArithmeticSensor functions)
}
#pragma warning restore 1591
