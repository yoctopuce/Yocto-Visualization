/*********************************************************************
 *
 *  $Id: yocto_spiport.cs 35462 2019-05-16 14:37:06Z seb $
 *
 *  Implements yFindSpiPort(), the high-level API for SpiPort functions
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
    //--- (YSpiPort return codes)
    //--- (end of YSpiPort return codes)
//--- (YSpiPort dlldef)
//--- (end of YSpiPort dlldef)
//--- (YSpiPort yapiwrapper)
//--- (end of YSpiPort yapiwrapper)
//--- (YSpiPort class start)
/**
 * <summary>
 *   The SpiPort function interface allows you to fully drive a Yoctopuce
 *   SPI port, to send and receive data, and to configure communication
 *   parameters (baud rate, bit count, parity, flow control and protocol).
 * <para>
 *   Note that Yoctopuce SPI ports are not exposed as virtual COM ports.
 *   They are meant to be used in the same way as all Yoctopuce devices.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YSpiPort : YFunction
{
//--- (end of YSpiPort class start)
    //--- (YSpiPort definitions)
    public new delegate void ValueCallback(YSpiPort func, string value);
    public new delegate void TimedReportCallback(YSpiPort func, YMeasure measure);

    public const int RXCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int TXCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int ERRCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int RXMSGCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int TXMSGCOUNT_INVALID = YAPI.INVALID_UINT;
    public const string LASTMSG_INVALID = YAPI.INVALID_STRING;
    public const string CURRENTJOB_INVALID = YAPI.INVALID_STRING;
    public const string STARTUPJOB_INVALID = YAPI.INVALID_STRING;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    public const int VOLTAGELEVEL_OFF = 0;
    public const int VOLTAGELEVEL_TTL3V = 1;
    public const int VOLTAGELEVEL_TTL3VR = 2;
    public const int VOLTAGELEVEL_TTL5V = 3;
    public const int VOLTAGELEVEL_TTL5VR = 4;
    public const int VOLTAGELEVEL_RS232 = 5;
    public const int VOLTAGELEVEL_RS485 = 6;
    public const int VOLTAGELEVEL_TTL1V8 = 7;
    public const int VOLTAGELEVEL_INVALID = -1;
    public const string PROTOCOL_INVALID = YAPI.INVALID_STRING;
    public const string SPIMODE_INVALID = YAPI.INVALID_STRING;
    public const int SSPOLARITY_ACTIVE_LOW = 0;
    public const int SSPOLARITY_ACTIVE_HIGH = 1;
    public const int SSPOLARITY_INVALID = -1;
    public const int SHIFTSAMPLING_OFF = 0;
    public const int SHIFTSAMPLING_ON = 1;
    public const int SHIFTSAMPLING_INVALID = -1;
    protected int _rxCount = RXCOUNT_INVALID;
    protected int _txCount = TXCOUNT_INVALID;
    protected int _errCount = ERRCOUNT_INVALID;
    protected int _rxMsgCount = RXMSGCOUNT_INVALID;
    protected int _txMsgCount = TXMSGCOUNT_INVALID;
    protected string _lastMsg = LASTMSG_INVALID;
    protected string _currentJob = CURRENTJOB_INVALID;
    protected string _startupJob = STARTUPJOB_INVALID;
    protected string _command = COMMAND_INVALID;
    protected int _voltageLevel = VOLTAGELEVEL_INVALID;
    protected string _protocol = PROTOCOL_INVALID;
    protected string _spiMode = SPIMODE_INVALID;
    protected int _ssPolarity = SSPOLARITY_INVALID;
    protected int _shiftSampling = SHIFTSAMPLING_INVALID;
    protected ValueCallback _valueCallbackSpiPort = null;
    protected int _rxptr = 0;
    protected byte[] _rxbuff;
    protected int _rxbuffptr = 0;
    //--- (end of YSpiPort definitions)

    public YSpiPort(string func)
        : base(func)
    {
        _className = "SpiPort";
        //--- (YSpiPort attributes initialization)
        //--- (end of YSpiPort attributes initialization)
    }

    //--- (YSpiPort implementation)

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
        if (json_val.has("voltageLevel"))
        {
            _voltageLevel = json_val.getInt("voltageLevel");
        }
        if (json_val.has("protocol"))
        {
            _protocol = json_val.getString("protocol");
        }
        if (json_val.has("spiMode"))
        {
            _spiMode = json_val.getString("spiMode");
        }
        if (json_val.has("ssPolarity"))
        {
            _ssPolarity = json_val.getInt("ssPolarity") > 0 ? 1 : 0;
        }
        if (json_val.has("shiftSampling"))
        {
            _shiftSampling = json_val.getInt("shiftSampling") > 0 ? 1 : 0;
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
     *   On failure, throws an exception or returns <c>YSpiPort.RXCOUNT_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YSpiPort.TXCOUNT_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YSpiPort.ERRCOUNT_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YSpiPort.RXMSGCOUNT_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YSpiPort.TXMSGCOUNT_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YSpiPort.LASTMSG_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YSpiPort.CURRENTJOB_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YSpiPort.STARTUPJOB_INVALID</c>.
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
     *   Returns the voltage level used on the serial line.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YSpiPort.VOLTAGELEVEL_OFF</c>, <c>YSpiPort.VOLTAGELEVEL_TTL3V</c>,
     *   <c>YSpiPort.VOLTAGELEVEL_TTL3VR</c>, <c>YSpiPort.VOLTAGELEVEL_TTL5V</c>,
     *   <c>YSpiPort.VOLTAGELEVEL_TTL5VR</c>, <c>YSpiPort.VOLTAGELEVEL_RS232</c>,
     *   <c>YSpiPort.VOLTAGELEVEL_RS485</c> and <c>YSpiPort.VOLTAGELEVEL_TTL1V8</c> corresponding to the
     *   voltage level used on the serial line
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSpiPort.VOLTAGELEVEL_INVALID</c>.
     * </para>
     */
    public int get_voltageLevel()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VOLTAGELEVEL_INVALID;
                }
            }
            res = this._voltageLevel;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the voltage type used on the serial line.
     * <para>
     *   Valid
     *   values  will depend on the Yoctopuce device model featuring
     *   the serial port feature.  Check your device documentation
     *   to find out which values are valid for that specific model.
     *   Trying to set an invalid value will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YSpiPort.VOLTAGELEVEL_OFF</c>, <c>YSpiPort.VOLTAGELEVEL_TTL3V</c>,
     *   <c>YSpiPort.VOLTAGELEVEL_TTL3VR</c>, <c>YSpiPort.VOLTAGELEVEL_TTL5V</c>,
     *   <c>YSpiPort.VOLTAGELEVEL_TTL5VR</c>, <c>YSpiPort.VOLTAGELEVEL_RS232</c>,
     *   <c>YSpiPort.VOLTAGELEVEL_RS485</c> and <c>YSpiPort.VOLTAGELEVEL_TTL1V8</c> corresponding to the
     *   voltage type used on the serial line
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
    public int set_voltageLevel(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("voltageLevel", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the type of protocol used over the serial line, as a string.
     * <para>
     *   Possible values are "Line" for ASCII messages separated by CR and/or LF,
     *   "Frame:[timeout]ms" for binary messages separated by a delay time,
     *   "Char" for a continuous ASCII stream or
     *   "Byte" for a continuous binary stream.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the type of protocol used over the serial line, as a string
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSpiPort.PROTOCOL_INVALID</c>.
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
     *   Changes the type of protocol used over the serial line.
     * <para>
     *   Possible values are "Line" for ASCII messages separated by CR and/or LF,
     *   "Frame:[timeout]ms" for binary messages separated by a delay time,
     *   "Char" for a continuous ASCII stream or
     *   "Byte" for a continuous binary stream.
     *   The suffix "/[wait]ms" can be added to reduce the transmit rate so that there
     *   is always at lest the specified number of milliseconds between each bytes sent.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the type of protocol used over the serial line
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
     *   Returns the SPI port communication parameters, as a string such as
     *   "125000,0,msb".
     * <para>
     *   The string includes the baud rate, the SPI mode (between
     *   0 and 3) and the bit order.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the SPI port communication parameters, as a string such as
     *   "125000,0,msb"
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSpiPort.SPIMODE_INVALID</c>.
     * </para>
     */
    public string get_spiMode()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SPIMODE_INVALID;
                }
            }
            res = this._spiMode;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the SPI port communication parameters, with a string such as
     *   "125000,0,msb".
     * <para>
     *   The string includes the baud rate, the SPI mode (between
     *   0 and 3) and the bit order.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the SPI port communication parameters, with a string such as
     *   "125000,0,msb"
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
    public int set_spiMode(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("spiMode", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the SS line polarity.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YSpiPort.SSPOLARITY_ACTIVE_LOW</c> or <c>YSpiPort.SSPOLARITY_ACTIVE_HIGH</c>, according
     *   to the SS line polarity
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSpiPort.SSPOLARITY_INVALID</c>.
     * </para>
     */
    public int get_ssPolarity()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SSPOLARITY_INVALID;
                }
            }
            res = this._ssPolarity;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the SS line polarity.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YSpiPort.SSPOLARITY_ACTIVE_LOW</c> or <c>YSpiPort.SSPOLARITY_ACTIVE_HIGH</c>, according
     *   to the SS line polarity
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
    public int set_ssPolarity(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("ssPolarity", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns true when the SDI line phase is shifted with regards to the SDO line.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YSpiPort.SHIFTSAMPLING_OFF</c> or <c>YSpiPort.SHIFTSAMPLING_ON</c>, according to true
     *   when the SDI line phase is shifted with regards to the SDO line
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSpiPort.SHIFTSAMPLING_INVALID</c>.
     * </para>
     */
    public int get_shiftSampling()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SHIFTSAMPLING_INVALID;
                }
            }
            res = this._shiftSampling;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the SDI line sampling shift.
     * <para>
     *   When disabled, SDI line is
     *   sampled in the middle of data output time. When enabled, SDI line is
     *   samples at the end of data output time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YSpiPort.SHIFTSAMPLING_OFF</c> or <c>YSpiPort.SHIFTSAMPLING_ON</c>, according to the SDI
     *   line sampling shift
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
    public int set_shiftSampling(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("shiftSampling", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a SPI port for a given identifier.
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
     *   This function does not require that the SPI port is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YSpiPort.isOnline()</c> to test if the SPI port is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a SPI port by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the SPI port
     * </param>
     * <returns>
     *   a <c>YSpiPort</c> object allowing you to drive the SPI port.
     * </returns>
     */
    public static YSpiPort FindSpiPort(string func)
    {
        YSpiPort obj;
        lock (YAPI.globalLock) {
            obj = (YSpiPort) YFunction._FindFromCache("SpiPort", func);
            if (obj == null) {
                obj = new YSpiPort(func);
                YFunction._AddToCache("SpiPort", func, obj);
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
        this._valueCallbackSpiPort = callback;
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
        if (this._valueCallbackSpiPort != null) {
            this._valueCallbackSpiPort(this, value);
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
     *   Sends a single byte to the serial port.
     * <para>
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
        return this.sendCommand("$"+String.Format("{0:X02}",code));
    }

    /**
     * <summary>
     *   Sends an ASCII string to the serial port, as is.
     * <para>
     * </para>
     * </summary>
     * <param name="text">
     *   the text string to send
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int writeStr(string text)
    {
        byte[] buff;
        int bufflen;
        int idx;
        int ch;
        buff = YAPI.DefaultEncoding.GetBytes(text);
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
                return this.sendCommand("+"+text);
            }
        }
        // send string using file upload
        return this._upload("txdata", buff);
    }

    /**
     * <summary>
     *   Sends a binary buffer to the serial port, as is.
     * <para>
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
        return this._upload("txdata", buff);
    }

    /**
     * <summary>
     *   Sends a byte sequence (provided as a list of bytes) to the serial port.
     * <para>
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
        byte[] buff;
        int bufflen;
        int idx;
        int hexb;
        int res;
        bufflen = byteList.Count;
        buff = new byte[bufflen];
        idx = 0;
        while (idx < bufflen) {
            hexb = byteList[idx];
            buff[idx] = (byte)(hexb & 0xff);
            idx = idx + 1;
        }

        res = this._upload("txdata", buff);
        return res;
    }

    /**
     * <summary>
     *   Sends a byte sequence (provided as a hexadecimal string) to the serial port.
     * <para>
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
        byte[] buff;
        int bufflen;
        int idx;
        int hexb;
        int res;
        bufflen = (hexString).Length;
        if (bufflen < 100) {
            return this.sendCommand("$"+hexString);
        }
        bufflen = ((bufflen) >> (1));
        buff = new byte[bufflen];
        idx = 0;
        while (idx < bufflen) {
            hexb = Convert.ToInt32((hexString).Substring( 2 * idx, 2), 16);
            buff[idx] = (byte)(hexb & 0xff);
            idx = idx + 1;
        }

        res = this._upload("txdata", buff);
        return res;
    }

    /**
     * <summary>
     *   Sends an ASCII string to the serial port, followed by a line break (CR LF).
     * <para>
     * </para>
     * </summary>
     * <param name="text">
     *   the text string to send
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int writeLine(string text)
    {
        byte[] buff;
        int bufflen;
        int idx;
        int ch;
        buff = YAPI.DefaultEncoding.GetBytes(""+text+"\r\n");
        bufflen = (buff).Length-2;
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
                return this.sendCommand("!"+text);
            }
        }
        // send string using file upload
        return this._upload("txdata", buff);
    }

    /**
     * <summary>
     *   Reads one byte from the receive buffer, starting at current stream position.
     * <para>
     *   If data at current stream position is not available anymore in the receive buffer,
     *   or if there is no data available yet, the function returns YAPI.NO_MORE_DATA.
     * </para>
     * </summary>
     * <returns>
     *   the next byte
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int readByte()
    {
        int currpos;
        int reqlen;
        byte[] buff;
        int bufflen;
        int mult;
        int endpos;
        int res;
        // first check if we have the requested character in the look-ahead buffer
        bufflen = (this._rxbuff).Length;
        if ((this._rxptr >= this._rxbuffptr) && (this._rxptr < this._rxbuffptr+bufflen)) {
            res = this._rxbuff[this._rxptr-this._rxbuffptr];
            this._rxptr = this._rxptr + 1;
            return res;
        }
        // try to preload more than one byte to speed-up byte-per-byte access
        currpos = this._rxptr;
        reqlen = 1024;
        buff = this.readBin(reqlen);
        bufflen = (buff).Length;
        if (this._rxptr == currpos+bufflen) {
            res = buff[0];
            this._rxptr = currpos+1;
            this._rxbuffptr = currpos;
            this._rxbuff = buff;
            return res;
        }
        // mixed bidirectional data, retry with a smaller block
        this._rxptr = currpos;
        reqlen = 16;
        buff = this.readBin(reqlen);
        bufflen = (buff).Length;
        if (this._rxptr == currpos+bufflen) {
            res = buff[0];
            this._rxptr = currpos+1;
            this._rxbuffptr = currpos;
            this._rxbuff = buff;
            return res;
        }
        // still mixed, need to process character by character
        this._rxptr = currpos;

        buff = this._download("rxdata.bin?pos="+Convert.ToString(this._rxptr)+"&len=1");
        bufflen = (buff).Length - 1;
        endpos = 0;
        mult = 1;
        while ((bufflen > 0) && (buff[bufflen] != 64)) {
            endpos = endpos + mult * (buff[bufflen] - 48);
            mult = mult * 10;
            bufflen = bufflen - 1;
        }
        this._rxptr = endpos;
        if (bufflen == 0) {
            return YAPI.NO_MORE_DATA;
        }
        res = buff[0];
        return res;
    }

    /**
     * <summary>
     *   Reads data from the receive buffer as a string, starting at current stream position.
     * <para>
     *   If data at current stream position is not available anymore in the receive buffer, the
     *   function performs a short read.
     * </para>
     * </summary>
     * <param name="nChars">
     *   the maximum number of characters to read
     * </param>
     * <returns>
     *   a string with receive buffer contents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual string readStr(int nChars)
    {
        byte[] buff;
        int bufflen;
        int mult;
        int endpos;
        string res;
        if (nChars > 65535) {
            nChars = 65535;
        }

        buff = this._download("rxdata.bin?pos="+Convert.ToString( this._rxptr)+"&len="+Convert.ToString(nChars));
        bufflen = (buff).Length - 1;
        endpos = 0;
        mult = 1;
        while ((bufflen > 0) && (buff[bufflen] != 64)) {
            endpos = endpos + mult * (buff[bufflen] - 48);
            mult = mult * 10;
            bufflen = bufflen - 1;
        }
        this._rxptr = endpos;
        res = (YAPI.DefaultEncoding.GetString(buff)).Substring( 0, bufflen);
        return res;
    }

    /**
     * <summary>
     *   Reads data from the receive buffer as a binary buffer, starting at current stream position.
     * <para>
     *   If data at current stream position is not available anymore in the receive buffer, the
     *   function performs a short read.
     * </para>
     * </summary>
     * <param name="nChars">
     *   the maximum number of bytes to read
     * </param>
     * <returns>
     *   a binary object with receive buffer contents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual byte[] readBin(int nChars)
    {
        byte[] buff;
        int bufflen;
        int mult;
        int endpos;
        int idx;
        byte[] res;
        if (nChars > 65535) {
            nChars = 65535;
        }

        buff = this._download("rxdata.bin?pos="+Convert.ToString( this._rxptr)+"&len="+Convert.ToString(nChars));
        bufflen = (buff).Length - 1;
        endpos = 0;
        mult = 1;
        while ((bufflen > 0) && (buff[bufflen] != 64)) {
            endpos = endpos + mult * (buff[bufflen] - 48);
            mult = mult * 10;
            bufflen = bufflen - 1;
        }
        this._rxptr = endpos;
        res = new byte[bufflen];
        idx = 0;
        while (idx < bufflen) {
            res[idx] = (byte)(buff[idx] & 0xff);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Reads data from the receive buffer as a list of bytes, starting at current stream position.
     * <para>
     *   If data at current stream position is not available anymore in the receive buffer, the
     *   function performs a short read.
     * </para>
     * </summary>
     * <param name="nChars">
     *   the maximum number of bytes to read
     * </param>
     * <returns>
     *   a sequence of bytes with receive buffer contents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual List<int> readArray(int nChars)
    {
        byte[] buff;
        int bufflen;
        int mult;
        int endpos;
        int idx;
        int b;
        List<int> res = new List<int>();
        if (nChars > 65535) {
            nChars = 65535;
        }

        buff = this._download("rxdata.bin?pos="+Convert.ToString( this._rxptr)+"&len="+Convert.ToString(nChars));
        bufflen = (buff).Length - 1;
        endpos = 0;
        mult = 1;
        while ((bufflen > 0) && (buff[bufflen] != 64)) {
            endpos = endpos + mult * (buff[bufflen] - 48);
            mult = mult * 10;
            bufflen = bufflen - 1;
        }
        this._rxptr = endpos;
        res.Clear();
        idx = 0;
        while (idx < bufflen) {
            b = buff[idx];
            res.Add(b);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Reads data from the receive buffer as a hexadecimal string, starting at current stream position.
     * <para>
     *   If data at current stream position is not available anymore in the receive buffer, the
     *   function performs a short read.
     * </para>
     * </summary>
     * <param name="nBytes">
     *   the maximum number of bytes to read
     * </param>
     * <returns>
     *   a string with receive buffer contents, encoded in hexadecimal
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual string readHex(int nBytes)
    {
        byte[] buff;
        int bufflen;
        int mult;
        int endpos;
        int ofs;
        string res;
        if (nBytes > 65535) {
            nBytes = 65535;
        }

        buff = this._download("rxdata.bin?pos="+Convert.ToString( this._rxptr)+"&len="+Convert.ToString(nBytes));
        bufflen = (buff).Length - 1;
        endpos = 0;
        mult = 1;
        while ((bufflen > 0) && (buff[bufflen] != 64)) {
            endpos = endpos + mult * (buff[bufflen] - 48);
            mult = mult * 10;
            bufflen = bufflen - 1;
        }
        this._rxptr = endpos;
        res = "";
        ofs = 0;
        while (ofs + 3 < bufflen) {
            res = ""+ res+""+String.Format("{0:X02}", buff[ofs])+""+String.Format("{0:X02}", buff[ofs + 1])+""+String.Format("{0:X02}", buff[ofs + 2])+""+String.Format("{0:X02}",buff[ofs + 3]);
            ofs = ofs + 4;
        }
        while (ofs < bufflen) {
            res = ""+ res+""+String.Format("{0:X02}",buff[ofs]);
            ofs = ofs + 1;
        }
        return res;
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
     *   On failure, throws an exception or returns an empty array.
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
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
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
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
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
     *   Manually sets the state of the SS line.
     * <para>
     *   This function has no effect when
     *   the SS line is handled automatically.
     * </para>
     * </summary>
     * <param name="val">
     *   1 to turn SS active, 0 to release SS.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_SS(int val)
    {
        return this.sendCommand("S"+Convert.ToString(val));
    }

    /**
     * <summary>
     *   Continues the enumeration of SPI ports started using <c>yFirstSpiPort()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned SPI ports order.
     *   If you want to find a specific a SPI port, use <c>SpiPort.findSpiPort()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSpiPort</c> object, corresponding to
     *   a SPI port currently online, or a <c>null</c> pointer
     *   if there are no more SPI ports to enumerate.
     * </returns>
     */
    public YSpiPort nextSpiPort()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindSpiPort(hwid);
    }

    //--- (end of YSpiPort implementation)

    //--- (YSpiPort functions)

    /**
     * <summary>
     *   Starts the enumeration of SPI ports currently accessible.
     * <para>
     *   Use the method <c>YSpiPort.nextSpiPort()</c> to iterate on
     *   next SPI ports.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSpiPort</c> object, corresponding to
     *   the first SPI port currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YSpiPort FirstSpiPort()
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
        err = YAPI.apiGetFunctionsByClass("SpiPort", 0, p, size, ref neededsize, ref errmsg);
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
        return FindSpiPort(serial + "." + funcId);
    }



    //--- (end of YSpiPort functions)
}
#pragma warning restore 1591
