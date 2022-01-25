/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements yFindInputChain(), the high-level API for InputChain functions
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
    //--- (YInputChain return codes)
    //--- (end of YInputChain return codes)
//--- (YInputChain dlldef)
//--- (end of YInputChain dlldef)
//--- (YInputChain yapiwrapper)
//--- (end of YInputChain yapiwrapper)
//--- (YInputChain class start)
/**
 * <summary>
 *   The <c>YInputChain</c> class provides access to separate
 *   digital inputs connected in a chain.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YInputChain : YFunction
{
//--- (end of YInputChain class start)
    //--- (YInputChain definitions)
    public new delegate void ValueCallback(YInputChain func, string value);
    public new delegate void TimedReportCallback(YInputChain func, YMeasure measure);
    public delegate void YEventCallback(YInputChain inputChain, int timestamp, string eventType, string eventData, string eventChange);

    protected static void yInternalEventCallback(YInputChain inputChain, String value)
    {
        inputChain._internalEventHandler(value);
    }


    public const int EXPECTEDNODES_INVALID = YAPI.INVALID_UINT;
    public const int DETECTEDNODES_INVALID = YAPI.INVALID_UINT;
    public const int REFRESHRATE_INVALID = YAPI.INVALID_UINT;
    public const string BITCHAIN1_INVALID = YAPI.INVALID_STRING;
    public const string BITCHAIN2_INVALID = YAPI.INVALID_STRING;
    public const string BITCHAIN3_INVALID = YAPI.INVALID_STRING;
    public const string BITCHAIN4_INVALID = YAPI.INVALID_STRING;
    public const string BITCHAIN5_INVALID = YAPI.INVALID_STRING;
    public const string BITCHAIN6_INVALID = YAPI.INVALID_STRING;
    public const string BITCHAIN7_INVALID = YAPI.INVALID_STRING;
    public const int WATCHDOGPERIOD_INVALID = YAPI.INVALID_UINT;
    public const int CHAINDIAGS_INVALID = YAPI.INVALID_UINT;
    protected int _expectedNodes = EXPECTEDNODES_INVALID;
    protected int _detectedNodes = DETECTEDNODES_INVALID;
    protected int _refreshRate = REFRESHRATE_INVALID;
    protected string _bitChain1 = BITCHAIN1_INVALID;
    protected string _bitChain2 = BITCHAIN2_INVALID;
    protected string _bitChain3 = BITCHAIN3_INVALID;
    protected string _bitChain4 = BITCHAIN4_INVALID;
    protected string _bitChain5 = BITCHAIN5_INVALID;
    protected string _bitChain6 = BITCHAIN6_INVALID;
    protected string _bitChain7 = BITCHAIN7_INVALID;
    protected int _watchdogPeriod = WATCHDOGPERIOD_INVALID;
    protected int _chainDiags = CHAINDIAGS_INVALID;
    protected ValueCallback _valueCallbackInputChain = null;
    protected YEventCallback _eventCallback;
    protected int _prevPos = 0;
    protected int _eventPos = 0;
    protected int _eventStamp = 0;
    protected List<string> _eventChains = new List<string>();
    //--- (end of YInputChain definitions)

    public YInputChain(string func)
        : base(func)
    {
        _className = "InputChain";
        //--- (YInputChain attributes initialization)
        //--- (end of YInputChain attributes initialization)
    }

    //--- (YInputChain implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("expectedNodes"))
        {
            _expectedNodes = json_val.getInt("expectedNodes");
        }
        if (json_val.has("detectedNodes"))
        {
            _detectedNodes = json_val.getInt("detectedNodes");
        }
        if (json_val.has("refreshRate"))
        {
            _refreshRate = json_val.getInt("refreshRate");
        }
        if (json_val.has("bitChain1"))
        {
            _bitChain1 = json_val.getString("bitChain1");
        }
        if (json_val.has("bitChain2"))
        {
            _bitChain2 = json_val.getString("bitChain2");
        }
        if (json_val.has("bitChain3"))
        {
            _bitChain3 = json_val.getString("bitChain3");
        }
        if (json_val.has("bitChain4"))
        {
            _bitChain4 = json_val.getString("bitChain4");
        }
        if (json_val.has("bitChain5"))
        {
            _bitChain5 = json_val.getString("bitChain5");
        }
        if (json_val.has("bitChain6"))
        {
            _bitChain6 = json_val.getString("bitChain6");
        }
        if (json_val.has("bitChain7"))
        {
            _bitChain7 = json_val.getString("bitChain7");
        }
        if (json_val.has("watchdogPeriod"))
        {
            _watchdogPeriod = json_val.getInt("watchdogPeriod");
        }
        if (json_val.has("chainDiags"))
        {
            _chainDiags = json_val.getInt("chainDiags");
        }
        base._parseAttr(json_val);
    }


    /**
     * <summary>
     *   Returns the number of nodes expected in the chain.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of nodes expected in the chain
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.EXPECTEDNODES_INVALID</c>.
     * </para>
     */
    public int get_expectedNodes()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return EXPECTEDNODES_INVALID;
                }
            }
            res = this._expectedNodes;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the number of nodes expected in the chain.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the number of nodes expected in the chain
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
    public int set_expectedNodes(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("expectedNodes", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the number of nodes detected in the chain.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of nodes detected in the chain
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.DETECTEDNODES_INVALID</c>.
     * </para>
     */
    public int get_detectedNodes()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DETECTEDNODES_INVALID;
                }
            }
            res = this._detectedNodes;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the desired refresh rate, measured in Hz.
     * <para>
     *   The higher the refresh rate is set, the higher the
     *   communication speed on the chain will be.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the desired refresh rate, measured in Hz
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.REFRESHRATE_INVALID</c>.
     * </para>
     */
    public int get_refreshRate()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return REFRESHRATE_INVALID;
                }
            }
            res = this._refreshRate;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the desired refresh rate, measured in Hz.
     * <para>
     *   The higher the refresh rate is set, the higher the
     *   communication speed on the chain will be.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the desired refresh rate, measured in Hz
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
    public int set_refreshRate(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("refreshRate", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the state of input 1 for all nodes of the input chain,
     *   as a hexadecimal string.
     * <para>
     *   The node nearest to the controller
     *   is the lowest bit of the result.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the state of input 1 for all nodes of the input chain,
     *   as a hexadecimal string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN1_INVALID</c>.
     * </para>
     */
    public string get_bitChain1()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BITCHAIN1_INVALID;
                }
            }
            res = this._bitChain1;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the state of input 2 for all nodes of the input chain,
     *   as a hexadecimal string.
     * <para>
     *   The node nearest to the controller
     *   is the lowest bit of the result.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the state of input 2 for all nodes of the input chain,
     *   as a hexadecimal string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN2_INVALID</c>.
     * </para>
     */
    public string get_bitChain2()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BITCHAIN2_INVALID;
                }
            }
            res = this._bitChain2;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the state of input 3 for all nodes of the input chain,
     *   as a hexadecimal string.
     * <para>
     *   The node nearest to the controller
     *   is the lowest bit of the result.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the state of input 3 for all nodes of the input chain,
     *   as a hexadecimal string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN3_INVALID</c>.
     * </para>
     */
    public string get_bitChain3()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BITCHAIN3_INVALID;
                }
            }
            res = this._bitChain3;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the state of input 4 for all nodes of the input chain,
     *   as a hexadecimal string.
     * <para>
     *   The node nearest to the controller
     *   is the lowest bit of the result.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the state of input 4 for all nodes of the input chain,
     *   as a hexadecimal string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN4_INVALID</c>.
     * </para>
     */
    public string get_bitChain4()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BITCHAIN4_INVALID;
                }
            }
            res = this._bitChain4;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the state of input 5 for all nodes of the input chain,
     *   as a hexadecimal string.
     * <para>
     *   The node nearest to the controller
     *   is the lowest bit of the result.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the state of input 5 for all nodes of the input chain,
     *   as a hexadecimal string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN5_INVALID</c>.
     * </para>
     */
    public string get_bitChain5()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BITCHAIN5_INVALID;
                }
            }
            res = this._bitChain5;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the state of input 6 for all nodes of the input chain,
     *   as a hexadecimal string.
     * <para>
     *   The node nearest to the controller
     *   is the lowest bit of the result.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the state of input 6 for all nodes of the input chain,
     *   as a hexadecimal string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN6_INVALID</c>.
     * </para>
     */
    public string get_bitChain6()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BITCHAIN6_INVALID;
                }
            }
            res = this._bitChain6;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the state of input 7 for all nodes of the input chain,
     *   as a hexadecimal string.
     * <para>
     *   The node nearest to the controller
     *   is the lowest bit of the result.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the state of input 7 for all nodes of the input chain,
     *   as a hexadecimal string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN7_INVALID</c>.
     * </para>
     */
    public string get_bitChain7()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BITCHAIN7_INVALID;
                }
            }
            res = this._bitChain7;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the wait time in seconds before triggering an inactivity
     *   timeout error.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the wait time in seconds before triggering an inactivity
     *   timeout error
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.WATCHDOGPERIOD_INVALID</c>.
     * </para>
     */
    public int get_watchdogPeriod()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return WATCHDOGPERIOD_INVALID;
                }
            }
            res = this._watchdogPeriod;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the wait time in seconds before triggering an inactivity
     *   timeout error.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method
     *   of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the wait time in seconds before triggering an inactivity
     *   timeout error
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
    public int set_watchdogPeriod(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("watchdogPeriod", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the controller state diagnostics.
     * <para>
     *   Bit 0 indicates a chain length
     *   error, bit 1 indicates an inactivity timeout and bit 2 indicates
     *   a loopback test failure.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the controller state diagnostics
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YInputChain.CHAINDIAGS_INVALID</c>.
     * </para>
     */
    public int get_chainDiags()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CHAINDIAGS_INVALID;
                }
            }
            res = this._chainDiags;
        }
        return res;
    }


    /**
     * <summary>
     *   Retrieves a digital input chain for a given identifier.
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
     *   This function does not require that the digital input chain is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YInputChain.isOnline()</c> to test if the digital input chain is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a digital input chain by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the digital input chain, for instance
     *   <c>MyDevice.inputChain</c>.
     * </param>
     * <returns>
     *   a <c>YInputChain</c> object allowing you to drive the digital input chain.
     * </returns>
     */
    public static YInputChain FindInputChain(string func)
    {
        YInputChain obj;
        lock (YAPI.globalLock) {
            obj = (YInputChain) YFunction._FindFromCache("InputChain", func);
            if (obj == null) {
                obj = new YInputChain(func);
                YFunction._AddToCache("InputChain", func, obj);
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
        this._valueCallbackInputChain = callback;
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
        if (this._valueCallbackInputChain != null) {
            this._valueCallbackInputChain(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Resets the application watchdog countdown.
     * <para>
     *   If you have setup a non-zero <c>watchdogPeriod</c>, you should
     *   call this function on a regular basis to prevent the application
     *   inactivity error to be triggered.
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
    public virtual int resetWatchdog()
    {
        return this.set_watchdogPeriod(-1);
    }


    /**
     * <summary>
     *   Returns a string with last events observed on the digital input chain.
     * <para>
     *   This method return only events that are still buffered in the device memory.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string with last events observed (one per line).
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
     * </para>
     */
    public virtual string get_lastEvents()
    {
        byte[] content = new byte[0];

        content = this._download("events.txt");
        return YAPI.DefaultEncoding.GetString(content);
    }


    /**
     * <summary>
     *   Registers a callback function to be called each time that an event is detected on the
     *   input chain.
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer.
     *   The callback function should take four arguments:
     *   the <c>YInputChain</c> object that emitted the event, the
     *   UTC timestamp of the event, a character string describing
     *   the type of event and a character string with the event data.
     *   On failure, throws an exception or returns a negative error code.
     * </param>
     */
    public virtual int registerEventCallback(YEventCallback callback)
    {
        if (callback != null) {
            this.registerValueCallback(yInternalEventCallback);
        } else {
            this.registerValueCallback((ValueCallback) null);
        }
        // register user callback AFTER the internal pseudo-event,
        // to make sure we start with future events only
        this._eventCallback = callback;
        return 0;
    }


    public virtual int _internalEventHandler(string cbpos)
    {
        int newPos;
        string url;
        byte[] content = new byte[0];
        string contentStr;
        List<string> eventArr = new List<string>();
        int arrLen;
        string lenStr;
        int arrPos;
        string eventStr;
        int eventLen;
        string hexStamp;
        int typePos;
        int dataPos;
        int evtStamp;
        string evtType;
        string evtData;
        string evtChange;
        int chainIdx;
        newPos = YAPI._atoi(cbpos);
        if (newPos < this._prevPos) {
            this._eventPos = 0;
        }
        this._prevPos = newPos;
        if (newPos < this._eventPos) {
            return YAPI.SUCCESS;
        }
        if (!(this._eventCallback != null)) {
            // first simulated event, use it to initialize reference values
            this._eventPos = newPos;
            this._eventChains.Clear();
            this._eventChains.Add(this.get_bitChain1());
            this._eventChains.Add(this.get_bitChain2());
            this._eventChains.Add(this.get_bitChain3());
            this._eventChains.Add(this.get_bitChain4());
            this._eventChains.Add(this.get_bitChain5());
            this._eventChains.Add(this.get_bitChain6());
            this._eventChains.Add(this.get_bitChain7());
            return YAPI.SUCCESS;
        }
        url = "events.txt?pos="+Convert.ToString(this._eventPos);

        content = this._download(url);
        contentStr = YAPI.DefaultEncoding.GetString(content);
        eventArr = new List<string>(contentStr.Split(new Char[] {'\n'}));
        arrLen = eventArr.Count;
        if (!(arrLen > 0)) {
            this._throw(YAPI.IO_ERROR, "fail to download events");
            return YAPI.IO_ERROR;
        }
        // last element of array is the new position preceeded by '@'
        arrLen = arrLen - 1;
        lenStr = eventArr[arrLen];
        lenStr = (lenStr).Substring( 1, (lenStr).Length-1);
        // update processed event position pointer
        this._eventPos = YAPI._atoi(lenStr);
        // now generate callbacks for each event received
        arrPos = 0;
        while (arrPos < arrLen) {
            eventStr = eventArr[arrPos];
            eventLen = (eventStr).Length;
            if (eventLen >= 1) {
                hexStamp = (eventStr).Substring( 0, 8);
                evtStamp = YAPI._hexStrToInt(hexStamp);
                typePos = (eventStr).IndexOf(":")+1;
                if ((evtStamp >= this._eventStamp) && (typePos > 8)) {
                    this._eventStamp = evtStamp;
                    dataPos = (eventStr).IndexOf("=")+1;
                    evtType = (eventStr).Substring( typePos, 1);
                    evtData = "";
                    evtChange = "";
                    if (dataPos > 10) {
                        evtData = (eventStr).Substring( dataPos, (eventStr).Length-dataPos);
                        if (("1234567").IndexOf(evtType) >= 0) {
                            chainIdx = YAPI._atoi(evtType) - 1;
                            evtChange = this._strXor(evtData, this._eventChains[chainIdx]);
                            this._eventChains[chainIdx] = evtData;
                        }
                    }
                    this._eventCallback(this, evtStamp, evtType, evtData, evtChange);
                }
            }
            arrPos = arrPos + 1;
        }
        return YAPI.SUCCESS;
    }


    public virtual string _strXor(string a, string b)
    {
        int lenA;
        int lenB;
        string res;
        int idx;
        int digitA;
        int digitB;
        // make sure the result has the same length as first argument
        lenA = (a).Length;
        lenB = (b).Length;
        if (lenA > lenB) {
            res = (a).Substring( 0, lenA-lenB);
            a = (a).Substring( lenA-lenB, lenB);
            lenA = lenB;
        } else {
            res = "";
            b = (b).Substring( lenA-lenB, lenA);
        }
        // scan strings and compare digit by digit
        idx = 0;
        while (idx < lenA) {
            digitA = YAPI._hexStrToInt((a).Substring( idx, 1));
            digitB = YAPI._hexStrToInt((b).Substring( idx, 1));
            res = ""+ res+""+String.Format("{0:x}",((digitA) ^ (digitB)));
            idx = idx + 1;
        }
        return res;
    }


    public virtual List<int> hex2array(string hexstr)
    {
        int hexlen;
        List<int> res = new List<int>();
        int idx;
        int digit;
        hexlen = (hexstr).Length;
        res.Clear();
        idx = hexlen;
        while (idx > 0) {
            idx = idx - 1;
            digit = YAPI._hexStrToInt((hexstr).Substring( idx, 1));
            res.Add(((digit) & (1)));
            res.Add(((((digit) >> (1))) & (1)));
            res.Add(((((digit) >> (2))) & (1)));
            res.Add(((((digit) >> (3))) & (1)));
        }
        return res;
    }

    /**
     * <summary>
     *   Continues the enumeration of digital input chains started using <c>yFirstInputChain()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned digital input chains order.
     *   If you want to find a specific a digital input chain, use <c>InputChain.findInputChain()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YInputChain</c> object, corresponding to
     *   a digital input chain currently online, or a <c>null</c> pointer
     *   if there are no more digital input chains to enumerate.
     * </returns>
     */
    public YInputChain nextInputChain()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindInputChain(hwid);
    }

    //--- (end of YInputChain implementation)

    //--- (YInputChain functions)

    /**
     * <summary>
     *   Starts the enumeration of digital input chains currently accessible.
     * <para>
     *   Use the method <c>YInputChain.nextInputChain()</c> to iterate on
     *   next digital input chains.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YInputChain</c> object, corresponding to
     *   the first digital input chain currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YInputChain FirstInputChain()
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
        err = YAPI.apiGetFunctionsByClass("InputChain", 0, p, size, ref neededsize, ref errmsg);
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
        return FindInputChain(serial + "." + funcId);
    }



    //--- (end of YInputChain functions)
}
#pragma warning restore 1591
