/*********************************************************************
 *
 *  $Id: yocto_digitalio.cs 38899 2019-12-20 17:21:03Z mvuilleu $
 *
 *  Implements yFindDigitalIO(), the high-level API for DigitalIO functions
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
    //--- (YDigitalIO return codes)
    //--- (end of YDigitalIO return codes)
//--- (YDigitalIO dlldef)
//--- (end of YDigitalIO dlldef)
//--- (YDigitalIO yapiwrapper)
//--- (end of YDigitalIO yapiwrapper)
//--- (YDigitalIO class start)
/**
 * <summary>
 *   The <c>YDigitalIO</c> class allows you drive a Yoctopuce digital input/output port.
 * <para>
 *   It can be used to setup the direction of each channel, to read the state of each channel
 *   and to switch the state of each channel configures as an output.
 *   You can work on all channels at once, or one by one. Most functions
 *   use a binary representation for channels where bit 0 matches channel #0 , bit 1 matches channel
 *   #1 and so on. If you are not familiar with numbers binary representation, you will find more
 *   information here: <c>https://en.wikipedia.org/wiki/Binary_number#Representation</c>. It is also possible
 *   to automatically generate short pulses of a determined duration. Electrical behavior
 *   of each I/O can be modified (open drain and reverse polarity).
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YDigitalIO : YFunction
{
//--- (end of YDigitalIO class start)
    //--- (YDigitalIO definitions)
    public new delegate void ValueCallback(YDigitalIO func, string value);
    public new delegate void TimedReportCallback(YDigitalIO func, YMeasure measure);

    public const int PORTSTATE_INVALID = YAPI.INVALID_UINT;
    public const int PORTDIRECTION_INVALID = YAPI.INVALID_UINT;
    public const int PORTOPENDRAIN_INVALID = YAPI.INVALID_UINT;
    public const int PORTPOLARITY_INVALID = YAPI.INVALID_UINT;
    public const int PORTDIAGS_INVALID = YAPI.INVALID_UINT;
    public const int PORTSIZE_INVALID = YAPI.INVALID_UINT;
    public const int OUTPUTVOLTAGE_USB_5V = 0;
    public const int OUTPUTVOLTAGE_USB_3V = 1;
    public const int OUTPUTVOLTAGE_EXT_V = 2;
    public const int OUTPUTVOLTAGE_INVALID = -1;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _portState = PORTSTATE_INVALID;
    protected int _portDirection = PORTDIRECTION_INVALID;
    protected int _portOpenDrain = PORTOPENDRAIN_INVALID;
    protected int _portPolarity = PORTPOLARITY_INVALID;
    protected int _portDiags = PORTDIAGS_INVALID;
    protected int _portSize = PORTSIZE_INVALID;
    protected int _outputVoltage = OUTPUTVOLTAGE_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackDigitalIO = null;
    //--- (end of YDigitalIO definitions)

    public YDigitalIO(string func)
        : base(func)
    {
        _className = "DigitalIO";
        //--- (YDigitalIO attributes initialization)
        //--- (end of YDigitalIO attributes initialization)
    }

    //--- (YDigitalIO implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("portState"))
        {
            _portState = json_val.getInt("portState");
        }
        if (json_val.has("portDirection"))
        {
            _portDirection = json_val.getInt("portDirection");
        }
        if (json_val.has("portOpenDrain"))
        {
            _portOpenDrain = json_val.getInt("portOpenDrain");
        }
        if (json_val.has("portPolarity"))
        {
            _portPolarity = json_val.getInt("portPolarity");
        }
        if (json_val.has("portDiags"))
        {
            _portDiags = json_val.getInt("portDiags");
        }
        if (json_val.has("portSize"))
        {
            _portSize = json_val.getInt("portSize");
        }
        if (json_val.has("outputVoltage"))
        {
            _outputVoltage = json_val.getInt("outputVoltage");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }


    /**
     * <summary>
     *   Returns the digital IO port state as an integer with each bit
     *   representing a channel.
     * <para>
     *   value 0 = <c>0b00000000</c> -> all channels are OFF
     *   value 1 = <c>0b00000001</c> -> channel #0 is ON
     *   value 2 = <c>0b00000010</c> -> channel #1 is ON
     *   value 3 = <c>0b00000011</c> -> channels #0 and #1 are ON
     *   value 4 = <c>0b00000100</c> -> channel #2 is ON
     *   and so on...
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the digital IO port state as an integer with each bit
     *   representing a channel
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDigitalIO.PORTSTATE_INVALID</c>.
     * </para>
     */
    public int get_portState()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PORTSTATE_INVALID;
                }
            }
            res = this._portState;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the state of all digital IO port's channels at once: the parameter
     *   is an integer where each bit represents a channel, with bit 0 matching channel #0.
     * <para>
     *   To set all channels to  0 -> <c>0b00000000</c> -> parameter = 0
     *   To set channel #0 to 1 -> <c>0b00000001</c> -> parameter =  1
     *   To set channel #1 to  1 -> <c>0b00000010</c> -> parameter = 2
     *   To set channel #0 and #1 -> <c>0b00000011</c> -> parameter =  3
     *   To set channel #2 to 1 -> <c>0b00000100</c> -> parameter =  4
     *   an so on....
     *   Only channels configured as outputs will be affecter, according to the value
     *   configured using <c>set_portDirection</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the state of all digital IO port's channels at once: the parameter
     *   is an integer where each bit represents a channel, with bit 0 matching channel #0
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
    public int set_portState(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("portState", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the I/O direction of all channels of the port (bitmap): 0 makes a bit an input, 1 makes it an output.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the I/O direction of all channels of the port (bitmap): 0 makes a bit
     *   an input, 1 makes it an output
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDigitalIO.PORTDIRECTION_INVALID</c>.
     * </para>
     */
    public int get_portDirection()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PORTDIRECTION_INVALID;
                }
            }
            res = this._portDirection;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the I/O direction of all channels of the port (bitmap): 0 makes a bit an input, 1 makes it an output.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method  to make sure the setting is kept after a reboot.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the I/O direction of all channels of the port (bitmap): 0 makes a bit
     *   an input, 1 makes it an output
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
    public int set_portDirection(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("portDirection", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the electrical interface for each bit of the port.
     * <para>
     *   For each bit set to 0  the matching I/O works in the regular,
     *   intuitive way, for each bit set to 1, the I/O works in reverse mode.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the electrical interface for each bit of the port
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDigitalIO.PORTOPENDRAIN_INVALID</c>.
     * </para>
     */
    public int get_portOpenDrain()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PORTOPENDRAIN_INVALID;
                }
            }
            res = this._portOpenDrain;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the electrical interface for each bit of the port.
     * <para>
     *   0 makes a bit a regular input/output, 1 makes
     *   it an open-drain (open-collector) input/output. Remember to call the
     *   <c>saveToFlash()</c> method  to make sure the setting is kept after a reboot.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the electrical interface for each bit of the port
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
    public int set_portOpenDrain(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("portOpenDrain", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the polarity of all the bits of the port.
     * <para>
     *   For each bit set to 0, the matching I/O works the regular,
     *   intuitive way; for each bit set to 1, the I/O works in reverse mode.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the polarity of all the bits of the port
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDigitalIO.PORTPOLARITY_INVALID</c>.
     * </para>
     */
    public int get_portPolarity()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PORTPOLARITY_INVALID;
                }
            }
            res = this._portPolarity;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the polarity of all the bits of the port: For each bit set to 0, the matching I/O works the regular,
     *   intuitive way; for each bit set to 1, the I/O works in reverse mode.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method  to make sure the setting will be kept after a reboot.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the polarity of all the bits of the port: For each bit set to 0, the
     *   matching I/O works the regular,
     *   intuitive way; for each bit set to 1, the I/O works in reverse mode
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
    public int set_portPolarity(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("portPolarity", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the port state diagnostics (Yocto-IO and Yocto-MaxiIO-V2 only).
     * <para>
     *   Bit 0 indicates a shortcut on
     *   output 0, etc. Bit 8 indicates a power failure, and bit 9 signals overheating (overcurrent).
     *   During normal use, all diagnostic bits should stay clear.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the port state diagnostics (Yocto-IO and Yocto-MaxiIO-V2 only)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDigitalIO.PORTDIAGS_INVALID</c>.
     * </para>
     */
    public int get_portDiags()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PORTDIAGS_INVALID;
                }
            }
            res = this._portDiags;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the number of bits (i.e.
     * <para>
     *   channels)implemented in the I/O port.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of bits (i.e
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDigitalIO.PORTSIZE_INVALID</c>.
     * </para>
     */
    public int get_portSize()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PORTSIZE_INVALID;
                }
            }
            res = this._portSize;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the voltage source used to drive output bits.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YDigitalIO.OUTPUTVOLTAGE_USB_5V</c>, <c>YDigitalIO.OUTPUTVOLTAGE_USB_3V</c> and
     *   <c>YDigitalIO.OUTPUTVOLTAGE_EXT_V</c> corresponding to the voltage source used to drive output bits
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDigitalIO.OUTPUTVOLTAGE_INVALID</c>.
     * </para>
     */
    public int get_outputVoltage()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return OUTPUTVOLTAGE_INVALID;
                }
            }
            res = this._outputVoltage;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the voltage source used to drive output bits.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method  to make sure the setting is kept after a reboot.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YDigitalIO.OUTPUTVOLTAGE_USB_5V</c>, <c>YDigitalIO.OUTPUTVOLTAGE_USB_3V</c> and
     *   <c>YDigitalIO.OUTPUTVOLTAGE_EXT_V</c> corresponding to the voltage source used to drive output bits
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
    public int set_outputVoltage(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("outputVoltage", rest_val);
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
     *   Retrieves a digital IO port for a given identifier.
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
     *   This function does not require that the digital IO port is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YDigitalIO.isOnline()</c> to test if the digital IO port is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a digital IO port by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the digital IO port, for instance
     *   <c>YMINIIO0.digitalIO</c>.
     * </param>
     * <returns>
     *   a <c>YDigitalIO</c> object allowing you to drive the digital IO port.
     * </returns>
     */
    public static YDigitalIO FindDigitalIO(string func)
    {
        YDigitalIO obj;
        lock (YAPI.globalLock) {
            obj = (YDigitalIO) YFunction._FindFromCache("DigitalIO", func);
            if (obj == null) {
                obj = new YDigitalIO(func);
                YFunction._AddToCache("DigitalIO", func, obj);
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
        this._valueCallbackDigitalIO = callback;
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
        if (this._valueCallbackDigitalIO != null) {
            this._valueCallbackDigitalIO(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Sets a single bit (i.e.
     * <para>
     *   channel) of the I/O port.
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <param name="bitstate">
     *   the state of the bit (1 or 0)
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_bitState(int bitno, int bitstate)
    {
        if (!(bitstate >= 0)) { this._throw( YAPI.INVALID_ARGUMENT, "invalid bit state"); return YAPI.INVALID_ARGUMENT; }
        if (!(bitstate <= 1)) { this._throw( YAPI.INVALID_ARGUMENT, "invalid bit state"); return YAPI.INVALID_ARGUMENT; }
        return this.set_command(""+((char)(82+bitstate)).ToString()+""+Convert.ToString(bitno));
    }


    /**
     * <summary>
     *   Returns the state of a single bit (i.e.
     * <para>
     *   channel)  of the I/O port.
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <returns>
     *   the bit state (0 or 1)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int get_bitState(int bitno)
    {
        int portVal;
        portVal = this.get_portState();
        return ((((portVal) >> (bitno))) & (1));
    }


    /**
     * <summary>
     *   Reverts a single bit (i.e.
     * <para>
     *   channel) of the I/O port.
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int toggle_bitState(int bitno)
    {
        return this.set_command("T"+Convert.ToString(bitno));
    }


    /**
     * <summary>
     *   Changes  the direction of a single bit (i.e.
     * <para>
     *   channel) from the I/O port.
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <param name="bitdirection">
     *   direction to set, 0 makes the bit an input, 1 makes it an output.
     *   Remember to call the   <c>saveToFlash()</c> method to make sure the setting is kept after a reboot.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_bitDirection(int bitno, int bitdirection)
    {
        if (!(bitdirection >= 0)) { this._throw( YAPI.INVALID_ARGUMENT, "invalid direction"); return YAPI.INVALID_ARGUMENT; }
        if (!(bitdirection <= 1)) { this._throw( YAPI.INVALID_ARGUMENT, "invalid direction"); return YAPI.INVALID_ARGUMENT; }
        return this.set_command(""+((char)(73+6*bitdirection)).ToString()+""+Convert.ToString(bitno));
    }


    /**
     * <summary>
     *   Returns the direction of a single bit (i.e.
     * <para>
     *   channel) from the I/O port (0 means the bit is an input, 1  an output).
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int get_bitDirection(int bitno)
    {
        int portDir;
        portDir = this.get_portDirection();
        return ((((portDir) >> (bitno))) & (1));
    }


    /**
     * <summary>
     *   Changes the polarity of a single bit from the I/O port.
     * <para>
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0.
     * </param>
     * <param name="bitpolarity">
     *   polarity to set, 0 makes the I/O work in regular mode, 1 makes the I/O  works in reverse mode.
     *   Remember to call the   <c>saveToFlash()</c> method to make sure the setting is kept after a reboot.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_bitPolarity(int bitno, int bitpolarity)
    {
        if (!(bitpolarity >= 0)) { this._throw( YAPI.INVALID_ARGUMENT, "invalid bit polarity"); return YAPI.INVALID_ARGUMENT; }
        if (!(bitpolarity <= 1)) { this._throw( YAPI.INVALID_ARGUMENT, "invalid bit polarity"); return YAPI.INVALID_ARGUMENT; }
        return this.set_command(""+((char)(110+4*bitpolarity)).ToString()+""+Convert.ToString(bitno));
    }


    /**
     * <summary>
     *   Returns the polarity of a single bit from the I/O port (0 means the I/O works in regular mode, 1 means the I/O  works in reverse mode).
     * <para>
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int get_bitPolarity(int bitno)
    {
        int portPol;
        portPol = this.get_portPolarity();
        return ((((portPol) >> (bitno))) & (1));
    }


    /**
     * <summary>
     *   Changes  the electrical interface of a single bit from the I/O port.
     * <para>
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <param name="opendrain">
     *   0 makes a bit a regular input/output, 1 makes
     *   it an open-drain (open-collector) input/output. Remember to call the
     *   <c>saveToFlash()</c> method to make sure the setting is kept after a reboot.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_bitOpenDrain(int bitno, int opendrain)
    {
        if (!(opendrain >= 0)) { this._throw( YAPI.INVALID_ARGUMENT, "invalid state"); return YAPI.INVALID_ARGUMENT; }
        if (!(opendrain <= 1)) { this._throw( YAPI.INVALID_ARGUMENT, "invalid state"); return YAPI.INVALID_ARGUMENT; }
        return this.set_command(""+((char)(100-32*opendrain)).ToString()+""+Convert.ToString(bitno));
    }


    /**
     * <summary>
     *   Returns the type of electrical interface of a single bit from the I/O port.
     * <para>
     *   (0 means the bit is an input, 1  an output).
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <returns>
     *   0 means the a bit is a regular input/output, 1 means the bit is an open-drain
     *   (open-collector) input/output.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int get_bitOpenDrain(int bitno)
    {
        int portOpenDrain;
        portOpenDrain = this.get_portOpenDrain();
        return ((((portOpenDrain) >> (bitno))) & (1));
    }


    /**
     * <summary>
     *   Triggers a pulse on a single bit for a specified duration.
     * <para>
     *   The specified bit
     *   will be turned to 1, and then back to 0 after the given duration.
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <param name="ms_duration">
     *   desired pulse duration in milliseconds. Be aware that the device time
     *   resolution is not guaranteed up to the millisecond.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int pulse(int bitno, int ms_duration)
    {
        return this.set_command("Z"+Convert.ToString( bitno)+",0,"+Convert.ToString(ms_duration));
    }


    /**
     * <summary>
     *   Schedules a pulse on a single bit for a specified duration.
     * <para>
     *   The specified bit
     *   will be turned to 1, and then back to 0 after the given duration.
     * </para>
     * </summary>
     * <param name="bitno">
     *   the bit number; lowest bit has index 0
     * </param>
     * <param name="ms_delay">
     *   waiting time before the pulse, in milliseconds
     * </param>
     * <param name="ms_duration">
     *   desired pulse duration in milliseconds. Be aware that the device time
     *   resolution is not guaranteed up to the millisecond.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int delayedPulse(int bitno, int ms_delay, int ms_duration)
    {
        return this.set_command("Z"+Convert.ToString(bitno)+","+Convert.ToString(ms_delay)+","+Convert.ToString(ms_duration));
    }

    /**
     * <summary>
     *   Continues the enumeration of digital IO ports started using <c>yFirstDigitalIO()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned digital IO ports order.
     *   If you want to find a specific a digital IO port, use <c>DigitalIO.findDigitalIO()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YDigitalIO</c> object, corresponding to
     *   a digital IO port currently online, or a <c>null</c> pointer
     *   if there are no more digital IO ports to enumerate.
     * </returns>
     */
    public YDigitalIO nextDigitalIO()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindDigitalIO(hwid);
    }

    //--- (end of YDigitalIO implementation)

    //--- (YDigitalIO functions)

    /**
     * <summary>
     *   Starts the enumeration of digital IO ports currently accessible.
     * <para>
     *   Use the method <c>YDigitalIO.nextDigitalIO()</c> to iterate on
     *   next digital IO ports.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YDigitalIO</c> object, corresponding to
     *   the first digital IO port currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YDigitalIO FirstDigitalIO()
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
        err = YAPI.apiGetFunctionsByClass("DigitalIO", 0, p, size, ref neededsize, ref errmsg);
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
        return FindDigitalIO(serial + "." + funcId);
    }



    //--- (end of YDigitalIO functions)
}
#pragma warning restore 1591
