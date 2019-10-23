/*********************************************************************
 *
 *  $Id: yocto_i2cport.cs 37168 2019-09-13 17:25:10Z mvuilleu $
 *
 *  Implements yFindI2cPort(), the high-level API for I2cPort functions
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
    //--- (YI2cPort return codes)
    //--- (end of YI2cPort return codes)
//--- (YI2cPort dlldef)
//--- (end of YI2cPort dlldef)
//--- (YI2cPort yapiwrapper)
//--- (end of YI2cPort yapiwrapper)
//--- (YI2cPort class start)
/**
 * <summary>
 *   The I2cPort function interface allows you to fully drive a Yoctopuce
 *   I2C port, to send and receive data, and to configure communication
 *   parameters (baud rate, etc).
 * <para>
 *   Note that Yoctopuce I2C ports are not exposed as virtual COM ports.
 *   They are meant to be used in the same way as all Yoctopuce devices.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YI2cPort : YFunction
{
//--- (end of YI2cPort class start)
    //--- (YI2cPort definitions)
    public new delegate void ValueCallback(YI2cPort func, string value);
    public new delegate void TimedReportCallback(YI2cPort func, YMeasure measure);

    public const int RXCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int TXCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int ERRCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int RXMSGCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int TXMSGCOUNT_INVALID = YAPI.INVALID_UINT;
    public const string LASTMSG_INVALID = YAPI.INVALID_STRING;
    public const string CURRENTJOB_INVALID = YAPI.INVALID_STRING;
    public const string STARTUPJOB_INVALID = YAPI.INVALID_STRING;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    public const string PROTOCOL_INVALID = YAPI.INVALID_STRING;
    public const int I2CVOLTAGELEVEL_OFF = 0;
    public const int I2CVOLTAGELEVEL_3V3 = 1;
    public const int I2CVOLTAGELEVEL_1V8 = 2;
    public const int I2CVOLTAGELEVEL_INVALID = -1;
    public const string I2CMODE_INVALID = YAPI.INVALID_STRING;
    protected int _rxCount = RXCOUNT_INVALID;
    protected int _txCount = TXCOUNT_INVALID;
    protected int _errCount = ERRCOUNT_INVALID;
    protected int _rxMsgCount = RXMSGCOUNT_INVALID;
    protected int _txMsgCount = TXMSGCOUNT_INVALID;
    protected string _lastMsg = LASTMSG_INVALID;
    protected string _currentJob = CURRENTJOB_INVALID;
    protected string _startupJob = STARTUPJOB_INVALID;
    protected string _command = COMMAND_INVALID;
    protected string _protocol = PROTOCOL_INVALID;
    protected int _i2cVoltageLevel = I2CVOLTAGELEVEL_INVALID;
    protected string _i2cMode = I2CMODE_INVALID;
    protected ValueCallback _valueCallbackI2cPort = null;
    protected int _rxptr = 0;
    protected byte[] _rxbuff;
    protected int _rxbuffptr = 0;
    //--- (end of YI2cPort definitions)

    public YI2cPort(string func)
        : base(func)
    {
        _className = "I2cPort";
        //--- (YI2cPort attributes initialization)
        //--- (end of YI2cPort attributes initialization)
    }

    //--- (YI2cPort implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("rxCount"))
        {
            _rxCount = json_val.getInt("rxCount");
        }
        if (json_val.has("txCount"))
        {
            _txCount = json_val.getInt("txCount");
        }
        if (json_val.has("errCount"))
        {
            _errCount = json_val.getInt("errCount");
        }
        if (json_val.has("rxMsgCount"))
        {
            _rxMsgCount = json_val.getInt("rxMsgCount");
        }
        if (json_val.has("txMsgCount"))
        {
            _txMsgCount = json_val.getInt("txMsgCount");
        }
        if (json_val.has("lastMsg"))
        {
            _lastMsg = json_val.getString("lastMsg");
        }
        if (json_val.has("currentJob"))
        {
            _currentJob = json_val.getString("currentJob");
        }
        if (json_val.has("startupJob"))
        {
            _startupJob = json_val.getString("startupJob");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        if (json_val.has("protocol"))
        {
            _protocol = json_val.getString("protocol");
        }
        if (json_val.has("i2cVoltageLevel"))
        {
            _i2cVoltageLevel = json_val.getInt("i2cVoltageLevel");
        }
        if (json_val.has("i2cMode"))
        {
            _i2cMode = json_val.getString("i2cMode");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the total number of bytes received since last reset.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the total number of bytes received since last reset
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.RXCOUNT_INVALID</c>.
     * </para>
     */
    public int get_rxCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RXCOUNT_INVALID;
                }
            }
            res = this._rxCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the total number of bytes transmitted since last reset.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the total number of bytes transmitted since last reset
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.TXCOUNT_INVALID</c>.
     * </para>
     */
    public int get_txCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return TXCOUNT_INVALID;
                }
            }
            res = this._txCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the total number of communication errors detected since last reset.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the total number of communication errors detected since last reset
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.ERRCOUNT_INVALID</c>.
     * </para>
     */
    public int get_errCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ERRCOUNT_INVALID;
                }
            }
            res = this._errCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the total number of messages received since last reset.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the total number of messages received since last reset
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.RXMSGCOUNT_INVALID</c>.
     * </para>
     */
    public int get_rxMsgCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RXMSGCOUNT_INVALID;
                }
            }
            res = this._rxMsgCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the total number of messages send since last reset.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the total number of messages send since last reset
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.TXMSGCOUNT_INVALID</c>.
     * </para>
     */
    public int get_txMsgCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return TXMSGCOUNT_INVALID;
                }
            }
            res = this._txMsgCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the latest message fully received (for Line and Frame protocols).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the latest message fully received (for Line and Frame protocols)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.LASTMSG_INVALID</c>.
     * </para>
     */
    public string get_lastMsg()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LASTMSG_INVALID;
                }
            }
            res = this._lastMsg;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the name of the job file currently in use.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the name of the job file currently in use
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.CURRENTJOB_INVALID</c>.
     * </para>
     */
    public string get_currentJob()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENTJOB_INVALID;
                }
            }
            res = this._currentJob;
        }
        return res;
    }

    /**
     * <summary>
     *   Selects a job file to run immediately.
     * <para>
     *   If an empty string is
     *   given as argument, stops running current job file.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string
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
    public int set_currentJob(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("currentJob", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the job file to use when the device is powered on.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the job file to use when the device is powered on
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.STARTUPJOB_INVALID</c>.
     * </para>
     */
    public string get_startupJob()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return STARTUPJOB_INVALID;
                }
            }
            res = this._startupJob;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the job to use when the device is powered on.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the job to use when the device is powered on
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
    public int set_startupJob(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("startupJob", rest_val);
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
     *   Returns the type of protocol used to send I2C messages, as a string.
     * <para>
     *   Possible values are
     *   "Line" for messages separated by LF or
     *   "Char" for continuous stream of codes.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the type of protocol used to send I2C messages, as a string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.PROTOCOL_INVALID</c>.
     * </para>
     */
    public string get_protocol()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PROTOCOL_INVALID;
                }
            }
            res = this._protocol;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the type of protocol used to send I2C messages.
     * <para>
     *   Possible values are
     *   "Line" for messages separated by LF or
     *   "Char" for continuous stream of codes.
     *   The suffix "/[wait]ms" can be added to reduce the transmit rate so that there
     *   is always at lest the specified number of milliseconds between each message sent.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the type of protocol used to send I2C messages
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
    public int set_protocol(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("protocol", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the voltage level used on the I2C bus.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YI2cPort.I2CVOLTAGELEVEL_OFF</c>, <c>YI2cPort.I2CVOLTAGELEVEL_3V3</c> and
     *   <c>YI2cPort.I2CVOLTAGELEVEL_1V8</c> corresponding to the voltage level used on the I2C bus
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.I2CVOLTAGELEVEL_INVALID</c>.
     * </para>
     */
    public int get_i2cVoltageLevel()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return I2CVOLTAGELEVEL_INVALID;
                }
            }
            res = this._i2cVoltageLevel;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the voltage level used on the I2C bus.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YI2cPort.I2CVOLTAGELEVEL_OFF</c>, <c>YI2cPort.I2CVOLTAGELEVEL_3V3</c> and
     *   <c>YI2cPort.I2CVOLTAGELEVEL_1V8</c> corresponding to the voltage level used on the I2C bus
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
    public int set_i2cVoltageLevel(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("i2cVoltageLevel", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the SPI port communication parameters, as a string such as
     *   "400kbps,2000ms,NoRestart".
     * <para>
     *   The string includes the baud rate, the
     *   recovery delay after communications errors, and if needed the option
     *   <c>NoRestart</c> to use a Stop/Start sequence instead of the
     *   Restart state when performing read on the I2C bus.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the SPI port communication parameters, as a string such as
     *   "400kbps,2000ms,NoRestart"
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YI2cPort.I2CMODE_INVALID</c>.
     * </para>
     */
    public string get_i2cMode()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return I2CMODE_INVALID;
                }
            }
            res = this._i2cMode;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the SPI port communication parameters, with a string such as
     *   "400kbps,2000ms".
     * <para>
     *   The string includes the baud rate, the
     *   recovery delay after communications errors, and if needed the option
     *   <c>NoRestart</c> to use a Stop/Start sequence instead of the
     *   Restart state when performing read on the I2C bus.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the SPI port communication parameters, with a string such as
     *   "400kbps,2000ms"
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
    public int set_i2cMode(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("i2cMode", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves an I2C port for a given identifier.
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
     *   This function does not require that the I2C port is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YI2cPort.isOnline()</c> to test if the I2C port is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   an I2C port by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the I2C port
     * </param>
     * <returns>
     *   a <c>YI2cPort</c> object allowing you to drive the I2C port.
     * </returns>
     */
    public static YI2cPort FindI2cPort(string func)
    {
        YI2cPort obj;
        lock (YAPI.globalLock) {
            obj = (YI2cPort) YFunction._FindFromCache("I2cPort", func);
            if (obj == null) {
                obj = new YI2cPort(func);
                YFunction._AddToCache("I2cPort", func, obj);
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
        this._valueCallbackI2cPort = callback;
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
        if (this._valueCallbackI2cPort != null) {
            this._valueCallbackI2cPort(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    public virtual int sendCommand(string text)
    {
        return this.set_command(text);
    }

    /**
     * <summary>
     *   Reads a single line (or message) from the receive buffer, starting at current stream position.
     * <para>
     *   This function is intended to be used when the serial port is configured for a message protocol,
     *   such as 'Line' mode or frame protocols.
     * </para>
     * <para>
     *   If data at current stream position is not available anymore in the receive buffer,
     *   the function returns the oldest available line and moves the stream position just after.
     *   If no new full line is received, the function returns an empty line.
     * </para>
     * </summary>
     * <returns>
     *   a string with a single line of text
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual string readLine()
    {
        string url;
        byte[] msgbin;
        List<string> msgarr = new List<string>();
        int msglen;
        string res;

        url = "rxmsg.json?pos="+Convert.ToString(this._rxptr)+"&len=1&maxw=1";
        msgbin = this._download(url);
        msgarr = this._json_get_array(msgbin);
        msglen = msgarr.Count;
        if (msglen == 0) {
            return "";
        }
        // last element of array is the new position
        msglen = msglen - 1;
        this._rxptr = YAPI._atoi(msgarr[msglen]);
        if (msglen == 0) {
            return "";
        }
        res = this._json_get_string(YAPI.DefaultEncoding.GetBytes(msgarr[0]));
        return res;
    }

    /**
     * <summary>
     *   Searches for incoming messages in the serial port receive buffer matching a given pattern,
     *   starting at current position.
     * <para>
     *   This function will only compare and return printable characters
     *   in the message strings. Binary protocols are handled as hexadecimal strings.
     * </para>
     * <para>
     *   The search returns all messages matching the expression provided as argument in the buffer.
     *   If no matching message is found, the search waits for one up to the specified maximum timeout
     *   (in milliseconds).
     * </para>
     * </summary>
     * <param name="pattern">
     *   a limited regular expression describing the expected message format,
     *   or an empty string if all messages should be returned (no filtering).
     *   When using binary protocols, the format applies to the hexadecimal
     *   representation of the message.
     * </param>
     * <param name="maxWait">
     *   the maximum number of milliseconds to wait for a message if none is found
     *   in the receive buffer.
     * </param>
     * <returns>
     *   an array of strings containing the messages found, if any.
     *   Binary messages are converted to hexadecimal representation.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<string> readMessages(string pattern, int maxWait)
    {
        string url;
        byte[] msgbin;
        List<string> msgarr = new List<string>();
        int msglen;
        List<string> res = new List<string>();
        int idx;

        url = "rxmsg.json?pos="+Convert.ToString( this._rxptr)+"&maxw="+Convert.ToString( maxWait)+"&pat="+pattern;
        msgbin = this._download(url);
        msgarr = this._json_get_array(msgbin);
        msglen = msgarr.Count;
        if (msglen == 0) {
            return res;
        }
        // last element of array is the new position
        msglen = msglen - 1;
        this._rxptr = YAPI._atoi(msgarr[msglen]);
        idx = 0;
        while (idx < msglen) {
            res.Add(this._json_get_string(YAPI.DefaultEncoding.GetBytes(msgarr[idx])));
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the current internal stream position to the specified value.
     * <para>
     *   This function
     *   does not affect the device, it only changes the value stored in the API object
     *   for the next read operations.
     * </para>
     * </summary>
     * <param name="absPos">
     *   the absolute position index for next read operations.
     * </param>
     * <returns>
     *   nothing.
     * </returns>
     */
    public virtual int read_seek(int absPos)
    {
        this._rxptr = absPos;
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Returns the current absolute stream position pointer of the API object.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the absolute position index for next read operations.
     * </returns>
     */
    public virtual int read_tell()
    {
        return this._rxptr;
    }

    /**
     * <summary>
     *   Returns the number of bytes available to read in the input buffer starting from the
     *   current absolute stream position pointer of the API object.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the number of bytes available to read
     * </returns>
     */
    public virtual int read_avail()
    {
        byte[] buff;
        int bufflen;
        int res;

        buff = this._download("rxcnt.bin?pos="+Convert.ToString(this._rxptr));
        bufflen = (buff).Length - 1;
        while ((bufflen > 0) && (buff[bufflen] != 64)) {
            bufflen = bufflen - 1;
        }
        res = YAPI._atoi((YAPI.DefaultEncoding.GetString(buff)).Substring( 0, bufflen));
        return res;
    }

    /**
     * <summary>
     *   Sends a text line query to the serial port, and reads the reply, if any.
     * <para>
     *   This function is intended to be used when the serial port is configured for 'Line' protocol.
     * </para>
     * </summary>
     * <param name="query">
     *   the line query to send (without CR/LF)
     * </param>
     * <param name="maxWait">
     *   the maximum number of milliseconds to wait for a reply.
     * </param>
     * <returns>
     *   the next text line received after sending the text query, as a string.
     *   Additional lines can be obtained by calling readLine or readMessages.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public virtual string queryLine(string query, int maxWait)
    {
        string url;
        byte[] msgbin;
        List<string> msgarr = new List<string>();
        int msglen;
        string res;

        url = "rxmsg.json?len=1&maxw="+Convert.ToString( maxWait)+"&cmd=!"+this._escapeAttr(query);
        msgbin = this._download(url);
        msgarr = this._json_get_array(msgbin);
        msglen = msgarr.Count;
        if (msglen == 0) {
            return "";
        }
        // last element of array is the new position
        msglen = msglen - 1;
        this._rxptr = YAPI._atoi(msgarr[msglen]);
        if (msglen == 0) {
            return "";
        }
        res = this._json_get_string(YAPI.DefaultEncoding.GetBytes(msgarr[0]));
        return res;
    }

    /**
     * <summary>
     *   Saves the job definition string (JSON data) into a job file.
     * <para>
     *   The job file can be later enabled using <c>selectJob()</c>.
     * </para>
     * </summary>
     * <param name="jobfile">
     *   name of the job file to save on the device filesystem
     * </param>
     * <param name="jsonDef">
     *   a string containing a JSON definition of the job
     * </param>
     * <returns>
     *   <c>YAPI_SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int uploadJob(string jobfile, string jsonDef)
    {
        this._upload(jobfile, YAPI.DefaultEncoding.GetBytes(jsonDef));
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Load and start processing the specified job file.
     * <para>
     *   The file must have
     *   been previously created using the user interface or uploaded on the
     *   device filesystem using the <c>uploadJob()</c> function.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="jobfile">
     *   name of the job file (on the device filesystem)
     * </param>
     * <returns>
     *   <c>YAPI_SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int selectJob(string jobfile)
    {
        return this.set_currentJob(jobfile);
    }

    /**
     * <summary>
     *   Clears the serial port buffer and resets counters to zero.
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
    public virtual int reset()
    {
        this._rxptr = 0;
        this._rxbuffptr = 0;
        this._rxbuff = new byte[0];

        return this.sendCommand("Z");
    }

    /**
     * <summary>
     *   Sends a one-way message (provided as a a binary buffer) to a device on the I2C bus.
     * <para>
     *   This function checks and reports communication errors on the I2C bus.
     * </para>
     * </summary>
     * <param name="slaveAddr">
     *   the 7-bit address of the slave device (without the direction bit)
     * </param>
     * <param name="buff">
     *   the binary buffer to be sent
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int i2cSendBin(int slaveAddr, byte[] buff)
    {
        int nBytes;
        int idx;
        int val;
        string msg;
        string reply;
        msg = "@"+String.Format("{0:x02}",slaveAddr)+":";
        nBytes = (buff).Length;
        idx = 0;
        while (idx < nBytes) {
            val = buff[idx];
            msg = ""+ msg+""+String.Format("{0:x02}",val);
            idx = idx + 1;
        }

        reply = this.queryLine(msg,1000);
        if (!((reply).Length > 0)) { this._throw( YAPI.IO_ERROR, "No response from I2C device"); return YAPI.IO_ERROR; }
        idx = (reply).IndexOf("[N]!");
        if (!(idx < 0)) { this._throw( YAPI.IO_ERROR, "No I2C ACK received"); return YAPI.IO_ERROR; }
        idx = (reply).IndexOf("!");
        if (!(idx < 0)) { this._throw( YAPI.IO_ERROR, "I2C protocol error"); return YAPI.IO_ERROR; }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Sends a one-way message (provided as a list of integer) to a device on the I2C bus.
     * <para>
     *   This function checks and reports communication errors on the I2C bus.
     * </para>
     * </summary>
     * <param name="slaveAddr">
     *   the 7-bit address of the slave device (without the direction bit)
     * </param>
     * <param name="values">
     *   a list of data bytes to be sent
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int i2cSendArray(int slaveAddr, List<int> values)
    {
        int nBytes;
        int idx;
        int val;
        string msg;
        string reply;
        msg = "@"+String.Format("{0:x02}",slaveAddr)+":";
        nBytes = values.Count;
        idx = 0;
        while (idx < nBytes) {
            val = values[idx];
            msg = ""+ msg+""+String.Format("{0:x02}",val);
            idx = idx + 1;
        }

        reply = this.queryLine(msg,1000);
        if (!((reply).Length > 0)) { this._throw( YAPI.IO_ERROR, "No response from I2C device"); return YAPI.IO_ERROR; }
        idx = (reply).IndexOf("[N]!");
        if (!(idx < 0)) { this._throw( YAPI.IO_ERROR, "No I2C ACK received"); return YAPI.IO_ERROR; }
        idx = (reply).IndexOf("!");
        if (!(idx < 0)) { this._throw( YAPI.IO_ERROR, "I2C protocol error"); return YAPI.IO_ERROR; }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Sends a one-way message (provided as a a binary buffer) to a device on the I2C bus,
     *   then read back the specified number of bytes from device.
     * <para>
     *   This function checks and reports communication errors on the I2C bus.
     * </para>
     * </summary>
     * <param name="slaveAddr">
     *   the 7-bit address of the slave device (without the direction bit)
     * </param>
     * <param name="buff">
     *   the binary buffer to be sent
     * </param>
     * <param name="rcvCount">
     *   the number of bytes to receive once the data bytes are sent
     * </param>
     * <returns>
     *   a list of bytes with the data received from slave device.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty binary buffer.
     * </para>
     */
    public virtual byte[] i2cSendAndReceiveBin(int slaveAddr, byte[] buff, int rcvCount)
    {
        int nBytes;
        int idx;
        int val;
        string msg;
        string reply;
        byte[] rcvbytes;
        msg = "@"+String.Format("{0:x02}",slaveAddr)+":";
        nBytes = (buff).Length;
        idx = 0;
        while (idx < nBytes) {
            val = buff[idx];
            msg = ""+ msg+""+String.Format("{0:x02}",val);
            idx = idx + 1;
        }
        idx = 0;
        while (idx < rcvCount) {
            msg = ""+msg+"xx";
            idx = idx + 1;
        }

        reply = this.queryLine(msg,1000);
        rcvbytes = new byte[0];
        if (!((reply).Length > 0)) { this._throw( YAPI.IO_ERROR, "No response from I2C device"); return rcvbytes; }
        idx = (reply).IndexOf("[N]!");
        if (!(idx < 0)) { this._throw( YAPI.IO_ERROR, "No I2C ACK received"); return rcvbytes; }
        idx = (reply).IndexOf("!");
        if (!(idx < 0)) { this._throw( YAPI.IO_ERROR, "I2C protocol error"); return rcvbytes; }
        reply = (reply).Substring( (reply).Length-2*rcvCount, 2*rcvCount);
        rcvbytes = YAPI._hexStrToBin(reply);
        return rcvbytes;
    }

    /**
     * <summary>
     *   Sends a one-way message (provided as a list of integer) to a device on the I2C bus,
     *   then read back the specified number of bytes from device.
     * <para>
     *   This function checks and reports communication errors on the I2C bus.
     * </para>
     * </summary>
     * <param name="slaveAddr">
     *   the 7-bit address of the slave device (without the direction bit)
     * </param>
     * <param name="values">
     *   a list of data bytes to be sent
     * </param>
     * <param name="rcvCount">
     *   the number of bytes to receive once the data bytes are sent
     * </param>
     * <returns>
     *   a list of bytes with the data received from slave device.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<int> i2cSendAndReceiveArray(int slaveAddr, List<int> values, int rcvCount)
    {
        int nBytes;
        int idx;
        int val;
        string msg;
        string reply;
        byte[] rcvbytes;
        List<int> res = new List<int>();
        msg = "@"+String.Format("{0:x02}",slaveAddr)+":";
        nBytes = values.Count;
        idx = 0;
        while (idx < nBytes) {
            val = values[idx];
            msg = ""+ msg+""+String.Format("{0:x02}",val);
            idx = idx + 1;
        }
        idx = 0;
        while (idx < rcvCount) {
            msg = ""+msg+"xx";
            idx = idx + 1;
        }

        reply = this.queryLine(msg,1000);
        if (!((reply).Length > 0)) { this._throw( YAPI.IO_ERROR, "No response from I2C device"); return res; }
        idx = (reply).IndexOf("[N]!");
        if (!(idx < 0)) { this._throw( YAPI.IO_ERROR, "No I2C ACK received"); return res; }
        idx = (reply).IndexOf("!");
        if (!(idx < 0)) { this._throw( YAPI.IO_ERROR, "I2C protocol error"); return res; }
        reply = (reply).Substring( (reply).Length-2*rcvCount, 2*rcvCount);
        rcvbytes = YAPI._hexStrToBin(reply);
        res.Clear();
        idx = 0;
        while (idx < rcvCount) {
            val = rcvbytes[idx];
            res.Add(val);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Sends a text-encoded I2C code stream to the I2C bus, as is.
     * <para>
     *   An I2C code stream is a string made of hexadecimal data bytes,
     *   but that may also include the I2C state transitions code:
     *   "{S}" to emit a start condition,
     *   "{R}" for a repeated start condition,
     *   "{P}" for a stop condition,
     *   "xx" for receiving a data byte,
     *   "{A}" to ack a data byte received and
     *   "{N}" to nack a data byte received.
     *   If a newline ("\n") is included in the stream, the message
     *   will be terminated and a newline will also be added to the
     *   receive stream.
     * </para>
     * </summary>
     * <param name="codes">
     *   the code stream to send
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int writeStr(string codes)
    {
        int bufflen;
        byte[] buff;
        int idx;
        int ch;
        buff = YAPI.DefaultEncoding.GetBytes(codes);
        bufflen = (buff).Length;
        if (bufflen < 100) {
            // if string is pure text, we can send it as a simple command (faster)
            ch = 0x20;
            idx = 0;
            while ((idx < bufflen) && (ch != 0)) {
                ch = buff[idx];
                if ((ch >= 0x20) && (ch < 0x7f)) {
                    idx = idx + 1;
                } else {
                    ch = 0;
                }
            }
            if (idx >= bufflen) {
                return this.sendCommand("+"+codes);
            }
        }
        // send string using file upload
        return this._upload("txdata", buff);
    }

    /**
     * <summary>
     *   Sends a text-encoded I2C code stream to the I2C bus, and terminate
     *   the message en relâchant le bus.
     * <para>
     *   An I2C code stream is a string made of hexadecimal data bytes,
     *   but that may also include the I2C state transitions code:
     *   "{S}" to emit a start condition,
     *   "{R}" for a repeated start condition,
     *   "{P}" for a stop condition,
     *   "xx" for receiving a data byte,
     *   "{A}" to ack a data byte received and
     *   "{N}" to nack a data byte received.
     *   At the end of the stream, a stop condition is added if missing
     *   and a newline is added to the receive buffer as well.
     * </para>
     * </summary>
     * <param name="codes">
     *   the code stream to send
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int writeLine(string codes)
    {
        int bufflen;
        byte[] buff;
        bufflen = (codes).Length;
        if (bufflen < 100) {
            return this.sendCommand("!"+codes);
        }
        // send string using file upload
        buff = YAPI.DefaultEncoding.GetBytes(""+codes+"\n");
        return this._upload("txdata", buff);
    }

    /**
     * <summary>
     *   Sends a single byte to the I2C bus.
     * <para>
     *   Depending on the I2C bus state, the byte
     *   will be interpreted as an address byte or a data byte.
     * </para>
     * </summary>
     * <param name="code">
     *   the byte to send
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int writeByte(int code)
    {
        return this.sendCommand("+"+String.Format("{0:X02}",code));
    }

    /**
     * <summary>
     *   Sends a byte sequence (provided as a hexadecimal string) to the I2C bus.
     * <para>
     *   Depending on the I2C bus state, the first byte will be interpreted as an
     *   address byte or a data byte.
     * </para>
     * </summary>
     * <param name="hexString">
     *   a string of hexadecimal byte codes
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int writeHex(string hexString)
    {
        int bufflen;
        byte[] buff;
        bufflen = (hexString).Length;
        if (bufflen < 100) {
            return this.sendCommand("+"+hexString);
        }
        buff = YAPI.DefaultEncoding.GetBytes(hexString);

        return this._upload("txdata", buff);
    }

    /**
     * <summary>
     *   Sends a binary buffer to the I2C bus, as is.
     * <para>
     *   Depending on the I2C bus state, the first byte will be interpreted
     *   as an address byte or a data byte.
     * </para>
     * </summary>
     * <param name="buff">
     *   the binary buffer to send
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int writeBin(byte[] buff)
    {
        int nBytes;
        int idx;
        int val;
        string msg;
        msg = "";
        nBytes = (buff).Length;
        idx = 0;
        while (idx < nBytes) {
            val = buff[idx];
            msg = ""+ msg+""+String.Format("{0:x02}",val);
            idx = idx + 1;
        }

        return this.writeHex(msg);
    }

    /**
     * <summary>
     *   Sends a byte sequence (provided as a list of bytes) to the I2C bus.
     * <para>
     *   Depending on the I2C bus state, the first byte will be interpreted as an
     *   address byte or a data byte.
     * </para>
     * </summary>
     * <param name="byteList">
     *   a list of byte codes
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int writeArray(List<int> byteList)
    {
        int nBytes;
        int idx;
        int val;
        string msg;
        msg = "";
        nBytes = byteList.Count;
        idx = 0;
        while (idx < nBytes) {
            val = byteList[idx];
            msg = ""+ msg+""+String.Format("{0:x02}",val);
            idx = idx + 1;
        }

        return this.writeHex(msg);
    }

    /**
     * <summary>
     *   Continues the enumeration of I2C ports started using <c>yFirstI2cPort()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned I2C ports order.
     *   If you want to find a specific an I2C port, use <c>I2cPort.findI2cPort()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YI2cPort</c> object, corresponding to
     *   an I2C port currently online, or a <c>null</c> pointer
     *   if there are no more I2C ports to enumerate.
     * </returns>
     */
    public YI2cPort nextI2cPort()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindI2cPort(hwid);
    }

    //--- (end of YI2cPort implementation)

    //--- (YI2cPort functions)

    /**
     * <summary>
     *   Starts the enumeration of I2C ports currently accessible.
     * <para>
     *   Use the method <c>YI2cPort.nextI2cPort()</c> to iterate on
     *   next I2C ports.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YI2cPort</c> object, corresponding to
     *   the first I2C port currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YI2cPort FirstI2cPort()
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
        err = YAPI.apiGetFunctionsByClass("I2cPort", 0, p, size, ref neededsize, ref errmsg);
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
        return FindI2cPort(serial + "." + funcId);
    }



    //--- (end of YI2cPort functions)
}
#pragma warning restore 1591
