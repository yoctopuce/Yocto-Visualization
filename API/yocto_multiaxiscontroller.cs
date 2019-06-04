/*********************************************************************
 *
 *  $Id: yocto_multiaxiscontroller.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindMultiAxisController(), the high-level API for MultiAxisController functions
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
    //--- (YMultiAxisController return codes)
    //--- (end of YMultiAxisController return codes)
//--- (YMultiAxisController dlldef)
//--- (end of YMultiAxisController dlldef)
//--- (YMultiAxisController yapiwrapper)
//--- (end of YMultiAxisController yapiwrapper)
//--- (YMultiAxisController class start)
/**
 * <summary>
 *   The Yoctopuce application programming interface allows you to drive a stepper motor.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YMultiAxisController : YFunction
{
//--- (end of YMultiAxisController class start)
    //--- (YMultiAxisController definitions)
    public new delegate void ValueCallback(YMultiAxisController func, string value);
    public new delegate void TimedReportCallback(YMultiAxisController func, YMeasure measure);

    public const int NAXIS_INVALID = YAPI.INVALID_UINT;
    public const int GLOBALSTATE_ABSENT = 0;
    public const int GLOBALSTATE_ALERT = 1;
    public const int GLOBALSTATE_HI_Z = 2;
    public const int GLOBALSTATE_STOP = 3;
    public const int GLOBALSTATE_RUN = 4;
    public const int GLOBALSTATE_BATCH = 5;
    public const int GLOBALSTATE_INVALID = -1;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _nAxis = NAXIS_INVALID;
    protected int _globalState = GLOBALSTATE_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackMultiAxisController = null;
    //--- (end of YMultiAxisController definitions)

    public YMultiAxisController(string func)
        : base(func)
    {
        _className = "MultiAxisController";
        //--- (YMultiAxisController attributes initialization)
        //--- (end of YMultiAxisController attributes initialization)
    }

    //--- (YMultiAxisController implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("nAxis"))
        {
            _nAxis = json_val.getInt("nAxis");
        }
        if (json_val.has("globalState"))
        {
            _globalState = json_val.getInt("globalState");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the number of synchronized controllers.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of synchronized controllers
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiAxisController.NAXIS_INVALID</c>.
     * </para>
     */
    public int get_nAxis()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return NAXIS_INVALID;
                }
            }
            res = this._nAxis;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the number of synchronized controllers.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the number of synchronized controllers
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
    public int set_nAxis(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("nAxis", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the stepper motor set overall state.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YMultiAxisController.GLOBALSTATE_ABSENT</c>,
     *   <c>YMultiAxisController.GLOBALSTATE_ALERT</c>, <c>YMultiAxisController.GLOBALSTATE_HI_Z</c>,
     *   <c>YMultiAxisController.GLOBALSTATE_STOP</c>, <c>YMultiAxisController.GLOBALSTATE_RUN</c> and
     *   <c>YMultiAxisController.GLOBALSTATE_BATCH</c> corresponding to the stepper motor set overall state
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMultiAxisController.GLOBALSTATE_INVALID</c>.
     * </para>
     */
    public int get_globalState()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return GLOBALSTATE_INVALID;
                }
            }
            res = this._globalState;
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
     *   Retrieves a multi-axis controller for a given identifier.
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
     *   This function does not require that the multi-axis controller is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YMultiAxisController.isOnline()</c> to test if the multi-axis controller is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a multi-axis controller by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the multi-axis controller
     * </param>
     * <returns>
     *   a <c>YMultiAxisController</c> object allowing you to drive the multi-axis controller.
     * </returns>
     */
    public static YMultiAxisController FindMultiAxisController(string func)
    {
        YMultiAxisController obj;
        lock (YAPI.globalLock) {
            obj = (YMultiAxisController) YFunction._FindFromCache("MultiAxisController", func);
            if (obj == null) {
                obj = new YMultiAxisController(func);
                YFunction._AddToCache("MultiAxisController", func, obj);
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
        this._valueCallbackMultiAxisController = callback;
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
        if (this._valueCallbackMultiAxisController != null) {
            this._valueCallbackMultiAxisController(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    public virtual int sendCommand(string command)
    {
        string url;
        byte[] retBin;
        int res;
        url = "cmd.txt?X="+command;
        //may throw an exception
        retBin = this._download(url);
        res = retBin[0];
        if (res == 49) {
            if (!(res == 48)) { this._throw( YAPI.DEVICE_BUSY, "Motor command pipeline is full, try again later"); return YAPI.DEVICE_BUSY; }
        } else {
            if (!(res == 48)) { this._throw( YAPI.IO_ERROR, "Motor command failed permanently"); return YAPI.IO_ERROR; }
        }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Reinitialize all controllers and clear all alert flags.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int reset()
    {
        return this.set_command("Z");
    }

    /**
     * <summary>
     *   Starts all motors backward at the specified speeds, to search for the motor home position.
     * <para>
     * </para>
     * </summary>
     * <param name="speed">
     *   desired speed for all axis, in steps per second.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int findHomePosition(List<double> speed)
    {
        string cmd;
        int i;
        int ndim;
        ndim = speed.Count;
        cmd = "H"+Convert.ToString((int) Math.Round(1000*speed[0]));
        i = 1;
        while (i < ndim) {
            cmd = ""+ cmd+","+Convert.ToString((int) Math.Round(1000*speed[i]));
            i = i + 1;
        }
        return this.sendCommand(cmd);
    }

    /**
     * <summary>
     *   Starts all motors synchronously to reach a given absolute position.
     * <para>
     *   The time needed to reach the requested position will depend on the lowest
     *   acceleration and max speed parameters configured for all motors.
     *   The final position will be reached on all axis at the same time.
     * </para>
     * </summary>
     * <param name="absPos">
     *   absolute position, measured in steps from each origin.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int moveTo(List<double> absPos)
    {
        string cmd;
        int i;
        int ndim;
        ndim = absPos.Count;
        cmd = "M"+Convert.ToString((int) Math.Round(16*absPos[0]));
        i = 1;
        while (i < ndim) {
            cmd = ""+ cmd+","+Convert.ToString((int) Math.Round(16*absPos[i]));
            i = i + 1;
        }
        return this.sendCommand(cmd);
    }

    /**
     * <summary>
     *   Starts all motors synchronously to reach a given relative position.
     * <para>
     *   The time needed to reach the requested position will depend on the lowest
     *   acceleration and max speed parameters configured for all motors.
     *   The final position will be reached on all axis at the same time.
     * </para>
     * </summary>
     * <param name="relPos">
     *   relative position, measured in steps from the current position.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int moveRel(List<double> relPos)
    {
        string cmd;
        int i;
        int ndim;
        ndim = relPos.Count;
        cmd = "m"+Convert.ToString((int) Math.Round(16*relPos[0]));
        i = 1;
        while (i < ndim) {
            cmd = ""+ cmd+","+Convert.ToString((int) Math.Round(16*relPos[i]));
            i = i + 1;
        }
        return this.sendCommand(cmd);
    }

    /**
     * <summary>
     *   Keep the motor in the same state for the specified amount of time, before processing next command.
     * <para>
     * </para>
     * </summary>
     * <param name="waitMs">
     *   wait time, specified in milliseconds.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int pause(int waitMs)
    {
        return this.sendCommand("_"+Convert.ToString(waitMs));
    }

    /**
     * <summary>
     *   Stops the motor with an emergency alert, without taking any additional precaution.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int emergencyStop()
    {
        return this.set_command("!");
    }

    /**
     * <summary>
     *   Stops the motor smoothly as soon as possible, without waiting for ongoing move completion.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int abortAndBrake()
    {
        return this.set_command("B");
    }

    /**
     * <summary>
     *   Turn the controller into Hi-Z mode immediately, without waiting for ongoing move completion.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int abortAndHiZ()
    {
        return this.set_command("z");
    }

    /**
     * <summary>
     *   Continues the enumeration of multi-axis controllers started using <c>yFirstMultiAxisController()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned multi-axis controllers order.
     *   If you want to find a specific a multi-axis controller, use <c>MultiAxisController.findMultiAxisController()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YMultiAxisController</c> object, corresponding to
     *   a multi-axis controller currently online, or a <c>null</c> pointer
     *   if there are no more multi-axis controllers to enumerate.
     * </returns>
     */
    public YMultiAxisController nextMultiAxisController()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindMultiAxisController(hwid);
    }

    //--- (end of YMultiAxisController implementation)

    //--- (YMultiAxisController functions)

    /**
     * <summary>
     *   Starts the enumeration of multi-axis controllers currently accessible.
     * <para>
     *   Use the method <c>YMultiAxisController.nextMultiAxisController()</c> to iterate on
     *   next multi-axis controllers.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YMultiAxisController</c> object, corresponding to
     *   the first multi-axis controller currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YMultiAxisController FirstMultiAxisController()
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
        err = YAPI.apiGetFunctionsByClass("MultiAxisController", 0, p, size, ref neededsize, ref errmsg);
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
        return FindMultiAxisController(serial + "." + funcId);
    }



    //--- (end of YMultiAxisController functions)
}
#pragma warning restore 1591
