/*********************************************************************
 *
 *  $Id: yocto_colorled.cs 37827 2019-10-25 13:07:48Z mvuilleu $
 *
 *  Implements yFindColorLed(), the high-level API for ColorLed functions
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
    //--- (YColorLed return codes)
    //--- (end of YColorLed return codes)
//--- (YColorLed dlldef)
//--- (end of YColorLed dlldef)
//--- (YColorLed yapiwrapper)
//--- (end of YColorLed yapiwrapper)
//--- (YColorLed class start)
/**
 * <summary>
 *   The YColorLed class allows you to drive a color LED, for instance using a Yocto-Color-V2 or a Yocto-PowerColor.
 * <para>
 *   The color can be specified using RGB coordinates as well as HSL coordinates.
 *   The module performs all conversions form RGB to HSL automatically. It is then
 *   self-evident to turn on a LED with a given hue and to progressively vary its
 *   saturation or lightness. If needed, you can find more information on the
 *   difference between RGB and HSL in the section following this one.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YColorLed : YFunction
{
//--- (end of YColorLed class start)
    //--- (YColorLed definitions)
    public new delegate void ValueCallback(YColorLed func, string value);
    public new delegate void TimedReportCallback(YColorLed func, YMeasure measure);

    public class YColorLedMove
    {
        public int target = YAPI.INVALID_INT;
        public int ms = YAPI.INVALID_INT;
        public int moving = YAPI.INVALID_UINT;
    }

    public const int RGBCOLOR_INVALID = YAPI.INVALID_UINT;
    public const int HSLCOLOR_INVALID = YAPI.INVALID_UINT;
    public const int RGBCOLORATPOWERON_INVALID = YAPI.INVALID_UINT;
    public const int BLINKSEQSIZE_INVALID = YAPI.INVALID_UINT;
    public const int BLINKSEQMAXSIZE_INVALID = YAPI.INVALID_UINT;
    public const int BLINKSEQSIGNATURE_INVALID = YAPI.INVALID_UINT;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    public static readonly YColorLedMove RGBMOVE_INVALID = null;
    public static readonly YColorLedMove HSLMOVE_INVALID = null;
    protected int _rgbColor = RGBCOLOR_INVALID;
    protected int _hslColor = HSLCOLOR_INVALID;
    protected YColorLedMove _rgbMove = new YColorLedMove();
    protected YColorLedMove _hslMove = new YColorLedMove();
    protected int _rgbColorAtPowerOn = RGBCOLORATPOWERON_INVALID;
    protected int _blinkSeqSize = BLINKSEQSIZE_INVALID;
    protected int _blinkSeqMaxSize = BLINKSEQMAXSIZE_INVALID;
    protected int _blinkSeqSignature = BLINKSEQSIGNATURE_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackColorLed = null;
    //--- (end of YColorLed definitions)

    public YColorLed(string func)
        : base(func)
    {
        _className = "ColorLed";
        //--- (YColorLed attributes initialization)
        //--- (end of YColorLed attributes initialization)
    }

    //--- (YColorLed implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("rgbColor"))
        {
            _rgbColor = json_val.getInt("rgbColor");
        }
        if (json_val.has("hslColor"))
        {
            _hslColor = json_val.getInt("hslColor");
        }
        if (json_val.has("rgbMove"))
        {
            YAPI.YJSONObject subjson = json_val.getYJSONObject("rgbMove");
            if (subjson.has("moving")) {
                _rgbMove.moving = subjson.getInt("moving");
            }
            if (subjson.has("target")) {
                _rgbMove.target = subjson.getInt("target");
            }
            if (subjson.has("ms")) {
                _rgbMove.ms = subjson.getInt("ms");
            }
        }
        if (json_val.has("hslMove"))
        {
            YAPI.YJSONObject subjson = json_val.getYJSONObject("hslMove");
            if (subjson.has("moving")) {
                _hslMove.moving = subjson.getInt("moving");
            }
            if (subjson.has("target")) {
                _hslMove.target = subjson.getInt("target");
            }
            if (subjson.has("ms")) {
                _hslMove.ms = subjson.getInt("ms");
            }
        }
        if (json_val.has("rgbColorAtPowerOn"))
        {
            _rgbColorAtPowerOn = json_val.getInt("rgbColorAtPowerOn");
        }
        if (json_val.has("blinkSeqSize"))
        {
            _blinkSeqSize = json_val.getInt("blinkSeqSize");
        }
        if (json_val.has("blinkSeqMaxSize"))
        {
            _blinkSeqMaxSize = json_val.getInt("blinkSeqMaxSize");
        }
        if (json_val.has("blinkSeqSignature"))
        {
            _blinkSeqSignature = json_val.getInt("blinkSeqSignature");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the current RGB color of the LED.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current RGB color of the LED
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLed.RGBCOLOR_INVALID</c>.
     * </para>
     */
    public int get_rgbColor()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RGBCOLOR_INVALID;
                }
            }
            res = this._rgbColor;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the current color of the LED, using an RGB color.
     * <para>
     *   Encoding is done as follows: 0xRRGGBB.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the current color of the LED, using an RGB color
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
    public int set_rgbColor(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = "0x"+(newval).ToString("X");
            return _setAttr("rgbColor", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the current HSL color of the LED.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current HSL color of the LED
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLed.HSLCOLOR_INVALID</c>.
     * </para>
     */
    public int get_hslColor()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return HSLCOLOR_INVALID;
                }
            }
            res = this._hslColor;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the current color of the LED, using a color HSL.
     * <para>
     *   Encoding is done as follows: 0xHHSSLL.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the current color of the LED, using a color HSL
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
    public int set_hslColor(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = "0x"+(newval).ToString("X");
            return _setAttr("hslColor", rest_val);
        }
    }

    public YColorLedMove get_rgbMove()
    {
        YColorLedMove res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RGBMOVE_INVALID;
                }
            }
            res = this._rgbMove;
        }
        return res;
    }

    public int set_rgbMove(YColorLedMove newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval.target).ToString()+":"+(newval.ms).ToString();
            return _setAttr("rgbMove", rest_val);
        }
    }

    /**
     * <summary>
     *   Performs a smooth transition in the RGB color space between the current color and a target color.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="rgb_target">
     *   desired RGB color at the end of the transition
     * </param>
     * <param name="ms_duration">
     *   duration of the transition, in millisecond
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
    public int rgbMove(int rgb_target,int ms_duration)
    {
        string rest_val;
        rest_val = (rgb_target).ToString()+":"+(ms_duration).ToString();
        return _setAttr("rgbMove", rest_val);
    }

    public YColorLedMove get_hslMove()
    {
        YColorLedMove res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return HSLMOVE_INVALID;
                }
            }
            res = this._hslMove;
        }
        return res;
    }

    public int set_hslMove(YColorLedMove newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval.target).ToString()+":"+(newval.ms).ToString();
            return _setAttr("hslMove", rest_val);
        }
    }

    /**
     * <summary>
     *   Performs a smooth transition in the HSL color space between the current color and a target color.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="hsl_target">
     *   desired HSL color at the end of the transition
     * </param>
     * <param name="ms_duration">
     *   duration of the transition, in millisecond
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
    public int hslMove(int hsl_target,int ms_duration)
    {
        string rest_val;
        rest_val = (hsl_target).ToString()+":"+(ms_duration).ToString();
        return _setAttr("hslMove", rest_val);
    }

    /**
     * <summary>
     *   Returns the configured color to be displayed when the module is turned on.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the configured color to be displayed when the module is turned on
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLed.RGBCOLORATPOWERON_INVALID</c>.
     * </para>
     */
    public int get_rgbColorAtPowerOn()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RGBCOLORATPOWERON_INVALID;
                }
            }
            res = this._rgbColorAtPowerOn;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the color that the LED displays by default when the module is turned on.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the color that the LED displays by default when the module is turned on
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
    public int set_rgbColorAtPowerOn(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = "0x"+(newval).ToString("X");
            return _setAttr("rgbColorAtPowerOn", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the current length of the blinking sequence.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current length of the blinking sequence
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLed.BLINKSEQSIZE_INVALID</c>.
     * </para>
     */
    public int get_blinkSeqSize()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BLINKSEQSIZE_INVALID;
                }
            }
            res = this._blinkSeqSize;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the maximum length of the blinking sequence.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximum length of the blinking sequence
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLed.BLINKSEQMAXSIZE_INVALID</c>.
     * </para>
     */
    public int get_blinkSeqMaxSize()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BLINKSEQMAXSIZE_INVALID;
                }
            }
            res = this._blinkSeqMaxSize;
        }
        return res;
    }

    /**
     * <summary>
     *   Return the blinking sequence signature.
     * <para>
     *   Since blinking
     *   sequences cannot be read from the device, this can be used
     *   to detect if a specific blinking sequence is already
     *   programmed.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLed.BLINKSEQSIGNATURE_INVALID</c>.
     * </para>
     */
    public int get_blinkSeqSignature()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BLINKSEQSIGNATURE_INVALID;
                }
            }
            res = this._blinkSeqSignature;
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
     *   Retrieves an RGB LED for a given identifier.
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
     *   This function does not require that the RGB LED is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YColorLed.isOnline()</c> to test if the RGB LED is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   an RGB LED by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the RGB LED, for instance
     *   <c>YRGBLED2.colorLed1</c>.
     * </param>
     * <returns>
     *   a <c>YColorLed</c> object allowing you to drive the RGB LED.
     * </returns>
     */
    public static YColorLed FindColorLed(string func)
    {
        YColorLed obj;
        lock (YAPI.globalLock) {
            obj = (YColorLed) YFunction._FindFromCache("ColorLed", func);
            if (obj == null) {
                obj = new YColorLed(func);
                YFunction._AddToCache("ColorLed", func, obj);
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
        this._valueCallbackColorLed = callback;
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
        if (this._valueCallbackColorLed != null) {
            this._valueCallbackColorLed(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    public virtual int sendCommand(string command)
    {
        return this.set_command(command);
    }

    /**
     * <summary>
     *   Add a new transition to the blinking sequence, the move will
     *   be performed in the HSL space.
     * <para>
     * </para>
     * </summary>
     * <param name="HSLcolor">
     *   desired HSL color when the transition is completed
     * </param>
     * <param name="msDelay">
     *   duration of the color transition, in milliseconds.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int addHslMoveToBlinkSeq(int HSLcolor, int msDelay)
    {
        return this.sendCommand("H"+Convert.ToString(HSLcolor)+","+Convert.ToString(msDelay));
    }

    /**
     * <summary>
     *   Adds a new transition to the blinking sequence, the move is
     *   performed in the RGB space.
     * <para>
     * </para>
     * </summary>
     * <param name="RGBcolor">
     *   desired RGB color when the transition is completed
     * </param>
     * <param name="msDelay">
     *   duration of the color transition, in milliseconds.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int addRgbMoveToBlinkSeq(int RGBcolor, int msDelay)
    {
        return this.sendCommand("R"+Convert.ToString(RGBcolor)+","+Convert.ToString(msDelay));
    }

    /**
     * <summary>
     *   Starts the preprogrammed blinking sequence.
     * <para>
     *   The sequence is
     *   run in a loop until it is stopped by stopBlinkSeq or an explicit
     *   change.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int startBlinkSeq()
    {
        return this.sendCommand("S");
    }

    /**
     * <summary>
     *   Stops the preprogrammed blinking sequence.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int stopBlinkSeq()
    {
        return this.sendCommand("X");
    }

    /**
     * <summary>
     *   Resets the preprogrammed blinking sequence.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int resetBlinkSeq()
    {
        return this.sendCommand("Z");
    }

    /**
     * <summary>
     *   Continues the enumeration of RGB LEDs started using <c>yFirstColorLed()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned RGB LEDs order.
     *   If you want to find a specific an RGB LED, use <c>ColorLed.findColorLed()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YColorLed</c> object, corresponding to
     *   an RGB LED currently online, or a <c>null</c> pointer
     *   if there are no more RGB LEDs to enumerate.
     * </returns>
     */
    public YColorLed nextColorLed()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindColorLed(hwid);
    }

    //--- (end of YColorLed implementation)

    //--- (YColorLed functions)

    /**
     * <summary>
     *   Starts the enumeration of RGB LEDs currently accessible.
     * <para>
     *   Use the method <c>YColorLed.nextColorLed()</c> to iterate on
     *   next RGB LEDs.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YColorLed</c> object, corresponding to
     *   the first RGB LED currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YColorLed FirstColorLed()
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
        err = YAPI.apiGetFunctionsByClass("ColorLed", 0, p, size, ref neededsize, ref errmsg);
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
        return FindColorLed(serial + "." + funcId);
    }



    //--- (end of YColorLed functions)
}
#pragma warning restore 1591
