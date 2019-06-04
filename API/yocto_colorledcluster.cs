/*********************************************************************
 *
 *  $Id: yocto_colorledcluster.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindColorLedCluster(), the high-level API for ColorLedCluster functions
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
    //--- (YColorLedCluster return codes)
    //--- (end of YColorLedCluster return codes)
//--- (YColorLedCluster dlldef)
//--- (end of YColorLedCluster dlldef)
//--- (YColorLedCluster yapiwrapper)
//--- (end of YColorLedCluster yapiwrapper)
//--- (YColorLedCluster class start)
/**
 * <summary>
 *   The Yoctopuce application programming interface
 *   allows you to drive a color LED cluster.
 * <para>
 *   Unlike the ColorLed class, the ColorLedCluster
 *   allows to handle several LEDs at one. Color changes can be done   using RGB coordinates as well as
 *   HSL coordinates.
 *   The module performs all conversions form RGB to HSL automatically. It is then
 *   self-evident to turn on a LED with a given hue and to progressively vary its
 *   saturation or lightness. If needed, you can find more information on the
 *   difference between RGB and HSL in the section following this one.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YColorLedCluster : YFunction
{
//--- (end of YColorLedCluster class start)
    //--- (YColorLedCluster definitions)
    public new delegate void ValueCallback(YColorLedCluster func, string value);
    public new delegate void TimedReportCallback(YColorLedCluster func, YMeasure measure);

    public const int ACTIVELEDCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int LEDTYPE_RGB = 0;
    public const int LEDTYPE_RGBW = 1;
    public const int LEDTYPE_INVALID = -1;
    public const int MAXLEDCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int BLINKSEQMAXCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int BLINKSEQMAXSIZE_INVALID = YAPI.INVALID_UINT;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _activeLedCount = ACTIVELEDCOUNT_INVALID;
    protected int _ledType = LEDTYPE_INVALID;
    protected int _maxLedCount = MAXLEDCOUNT_INVALID;
    protected int _blinkSeqMaxCount = BLINKSEQMAXCOUNT_INVALID;
    protected int _blinkSeqMaxSize = BLINKSEQMAXSIZE_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackColorLedCluster = null;
    //--- (end of YColorLedCluster definitions)

    public YColorLedCluster(string func)
        : base(func)
    {
        _className = "ColorLedCluster";
        //--- (YColorLedCluster attributes initialization)
        //--- (end of YColorLedCluster attributes initialization)
    }

    //--- (YColorLedCluster implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("activeLedCount"))
        {
            _activeLedCount = json_val.getInt("activeLedCount");
        }
        if (json_val.has("ledType"))
        {
            _ledType = json_val.getInt("ledType");
        }
        if (json_val.has("maxLedCount"))
        {
            _maxLedCount = json_val.getInt("maxLedCount");
        }
        if (json_val.has("blinkSeqMaxCount"))
        {
            _blinkSeqMaxCount = json_val.getInt("blinkSeqMaxCount");
        }
        if (json_val.has("blinkSeqMaxSize"))
        {
            _blinkSeqMaxSize = json_val.getInt("blinkSeqMaxSize");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the number of LEDs currently handled by the device.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of LEDs currently handled by the device
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLedCluster.ACTIVELEDCOUNT_INVALID</c>.
     * </para>
     */
    public int get_activeLedCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ACTIVELEDCOUNT_INVALID;
                }
            }
            res = this._activeLedCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the number of LEDs currently handled by the device.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the number of LEDs currently handled by the device
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
    public int set_activeLedCount(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("activeLedCount", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the RGB LED type currently handled by the device.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YColorLedCluster.LEDTYPE_RGB</c> or <c>YColorLedCluster.LEDTYPE_RGBW</c>, according to
     *   the RGB LED type currently handled by the device
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLedCluster.LEDTYPE_INVALID</c>.
     * </para>
     */
    public int get_ledType()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LEDTYPE_INVALID;
                }
            }
            res = this._ledType;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the RGB LED type currently handled by the device.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YColorLedCluster.LEDTYPE_RGB</c> or <c>YColorLedCluster.LEDTYPE_RGBW</c>, according to
     *   the RGB LED type currently handled by the device
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
    public int set_ledType(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("ledType", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the maximum number of LEDs that the device can handle.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximum number of LEDs that the device can handle
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLedCluster.MAXLEDCOUNT_INVALID</c>.
     * </para>
     */
    public int get_maxLedCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MAXLEDCOUNT_INVALID;
                }
            }
            res = this._maxLedCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the maximum number of sequences that the device can handle.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximum number of sequences that the device can handle
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLedCluster.BLINKSEQMAXCOUNT_INVALID</c>.
     * </para>
     */
    public int get_blinkSeqMaxCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BLINKSEQMAXCOUNT_INVALID;
                }
            }
            res = this._blinkSeqMaxCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the maximum length of sequences.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximum length of sequences
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YColorLedCluster.BLINKSEQMAXSIZE_INVALID</c>.
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
     *   Retrieves a RGB LED cluster for a given identifier.
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
     *   This function does not require that the RGB LED cluster is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YColorLedCluster.isOnline()</c> to test if the RGB LED cluster is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a RGB LED cluster by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the RGB LED cluster
     * </param>
     * <returns>
     *   a <c>YColorLedCluster</c> object allowing you to drive the RGB LED cluster.
     * </returns>
     */
    public static YColorLedCluster FindColorLedCluster(string func)
    {
        YColorLedCluster obj;
        lock (YAPI.globalLock) {
            obj = (YColorLedCluster) YFunction._FindFromCache("ColorLedCluster", func);
            if (obj == null) {
                obj = new YColorLedCluster(func);
                YFunction._AddToCache("ColorLedCluster", func, obj);
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
        this._valueCallbackColorLedCluster = callback;
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
        if (this._valueCallbackColorLedCluster != null) {
            this._valueCallbackColorLedCluster(this, value);
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
     *   Changes the current color of consecutive LEDs in the cluster, using a RGB color.
     * <para>
     *   Encoding is done as follows: 0xRRGGBB.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="rgbValue">
     *   new color.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_rgbColor(int ledIndex, int count, int rgbValue)
    {
        return this.sendCommand("SR"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+String.Format("{0:x}",rgbValue));
    }

    /**
     * <summary>
     *   Changes the  color at device startup of consecutive LEDs in the cluster, using a RGB color.
     * <para>
     *   Encoding is done as follows: 0xRRGGBB.
     *   Don't forget to call <c>saveLedsConfigAtPowerOn()</c> to make sure the modification is saved in the
     *   device flash memory.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="rgbValue">
     *   new color.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_rgbColorAtPowerOn(int ledIndex, int count, int rgbValue)
    {
        return this.sendCommand("SC"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+String.Format("{0:x}",rgbValue));
    }

    /**
     * <summary>
     *   Changes the  color at device startup of consecutive LEDs in the cluster, using a HSL color.
     * <para>
     *   Encoding is done as follows: 0xHHSSLL.
     *   Don't forget to call <c>saveLedsConfigAtPowerOn()</c> to make sure the modification is saved in the
     *   device flash memory.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="hslValue">
     *   new color.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_hslColorAtPowerOn(int ledIndex, int count, int hslValue)
    {
        int rgbValue;
        rgbValue = this.hsl2rgb(hslValue);
        return this.sendCommand("SC"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+String.Format("{0:x}",rgbValue));
    }

    /**
     * <summary>
     *   Changes the current color of consecutive LEDs in the cluster, using a HSL color.
     * <para>
     *   Encoding is done as follows: 0xHHSSLL.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="hslValue">
     *   new color.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_hslColor(int ledIndex, int count, int hslValue)
    {
        return this.sendCommand("SH"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+String.Format("{0:x}",hslValue));
    }

    /**
     * <summary>
     *   Allows you to modify the current color of a group of adjacent LEDs to another color, in a seamless and
     *   autonomous manner.
     * <para>
     *   The transition is performed in the RGB space.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="rgbValue">
     *   new color (0xRRGGBB).
     * </param>
     * <param name="delay">
     *   transition duration in ms
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int rgb_move(int ledIndex, int count, int rgbValue, int delay)
    {
        return this.sendCommand("MR"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+String.Format("{0:x}",rgbValue)+","+Convert.ToString(delay));
    }

    /**
     * <summary>
     *   Allows you to modify the current color of a group of adjacent LEDs  to another color, in a seamless and
     *   autonomous manner.
     * <para>
     *   The transition is performed in the HSL space. In HSL, hue is a circular
     *   value (0..360°). There are always two paths to perform the transition: by increasing
     *   or by decreasing the hue. The module selects the shortest transition.
     *   If the difference is exactly 180°, the module selects the transition which increases
     *   the hue.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="hslValue">
     *   new color (0xHHSSLL).
     * </param>
     * <param name="delay">
     *   transition duration in ms
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int hsl_move(int ledIndex, int count, int hslValue, int delay)
    {
        return this.sendCommand("MH"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+String.Format("{0:x}",hslValue)+","+Convert.ToString(delay));
    }

    /**
     * <summary>
     *   Adds an RGB transition to a sequence.
     * <para>
     *   A sequence is a transition list, which can
     *   be executed in loop by a group of LEDs.  Sequences are persistent and are saved
     *   in the device flash memory as soon as the <c>saveBlinkSeq()</c> method is called.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   sequence index.
     * </param>
     * <param name="rgbValue">
     *   target color (0xRRGGBB)
     * </param>
     * <param name="delay">
     *   transition duration in ms
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int addRgbMoveToBlinkSeq(int seqIndex, int rgbValue, int delay)
    {
        return this.sendCommand("AR"+Convert.ToString(seqIndex)+","+String.Format("{0:x}",rgbValue)+","+Convert.ToString(delay));
    }

    /**
     * <summary>
     *   Adds an HSL transition to a sequence.
     * <para>
     *   A sequence is a transition list, which can
     *   be executed in loop by an group of LEDs.  Sequences are persistent and are saved
     *   in the device flash memory as soon as the <c>saveBlinkSeq()</c> method is called.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   sequence index.
     * </param>
     * <param name="hslValue">
     *   target color (0xHHSSLL)
     * </param>
     * <param name="delay">
     *   transition duration in ms
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int addHslMoveToBlinkSeq(int seqIndex, int hslValue, int delay)
    {
        return this.sendCommand("AH"+Convert.ToString(seqIndex)+","+String.Format("{0:x}",hslValue)+","+Convert.ToString(delay));
    }

    /**
     * <summary>
     *   Adds a mirror ending to a sequence.
     * <para>
     *   When the sequence will reach the end of the last
     *   transition, its running speed will automatically be reversed so that the sequence plays
     *   in the reverse direction, like in a mirror. After the first transition of the sequence
     *   is played at the end of the reverse execution, the sequence starts again in
     *   the initial direction.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   sequence index.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int addMirrorToBlinkSeq(int seqIndex)
    {
        return this.sendCommand("AC"+Convert.ToString(seqIndex)+",0,0");
    }

    /**
     * <summary>
     *   Adds to a sequence a jump to another sequence.
     * <para>
     *   When a pixel will reach this jump,
     *   it will be automatically relinked to the new sequence, and will run it starting
     *   from the beginning.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   sequence index.
     * </param>
     * <param name="linkSeqIndex">
     *   index of the sequence to chain.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int addJumpToBlinkSeq(int seqIndex, int linkSeqIndex)
    {
        return this.sendCommand("AC"+Convert.ToString(seqIndex)+",100,"+Convert.ToString(linkSeqIndex)+",1000");
    }

    /**
     * <summary>
     *   Adds a to a sequence a hard stop code.
     * <para>
     *   When a pixel will reach this stop code,
     *   instead of restarting the sequence in a loop it will automatically be unlinked
     *   from the sequence.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   sequence index.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int addUnlinkToBlinkSeq(int seqIndex)
    {
        return this.sendCommand("AC"+Convert.ToString(seqIndex)+",100,-1,1000");
    }

    /**
     * <summary>
     *   Links adjacent LEDs to a specific sequence.
     * <para>
     *   These LEDs start to execute
     *   the sequence as soon as  startBlinkSeq is called. It is possible to add an offset
     *   in the execution: that way we  can have several groups of LED executing the same
     *   sequence, with a  temporal offset. A LED cannot be linked to more than one sequence.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="seqIndex">
     *   sequence index.
     * </param>
     * <param name="offset">
     *   execution offset in ms.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int linkLedToBlinkSeq(int ledIndex, int count, int seqIndex, int offset)
    {
        return this.sendCommand("LS"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+Convert.ToString(seqIndex)+","+Convert.ToString(offset));
    }

    /**
     * <summary>
     *   Links adjacent LEDs to a specific sequence at device power-on.
     * <para>
     *   Don't forget to configure
     *   the sequence auto start flag as well and call <c>saveLedsConfigAtPowerOn()</c>. It is possible to add an offset
     *   in the execution: that way we  can have several groups of LEDs executing the same
     *   sequence, with a  temporal offset. A LED cannot be linked to more than one sequence.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="seqIndex">
     *   sequence index.
     * </param>
     * <param name="offset">
     *   execution offset in ms.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int linkLedToBlinkSeqAtPowerOn(int ledIndex, int count, int seqIndex, int offset)
    {
        return this.sendCommand("LO"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+Convert.ToString(seqIndex)+","+Convert.ToString(offset));
    }

    /**
     * <summary>
     *   Links adjacent LEDs to a specific sequence.
     * <para>
     *   These LED start to execute
     *   the sequence as soon as  startBlinkSeq is called. This function automatically
     *   introduces a shift between LEDs so that the specified number of sequence periods
     *   appears on the group of LEDs (wave effect).
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <param name="seqIndex">
     *   sequence index.
     * </param>
     * <param name="periods">
     *   number of periods to show on LEDs.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int linkLedToPeriodicBlinkSeq(int ledIndex, int count, int seqIndex, int periods)
    {
        return this.sendCommand("LP"+Convert.ToString(ledIndex)+","+Convert.ToString(count)+","+Convert.ToString(seqIndex)+","+Convert.ToString(periods));
    }

    /**
     * <summary>
     *   Unlinks adjacent LEDs from a  sequence.
     * <para>
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first affected LED.
     * </param>
     * <param name="count">
     *   affected LED count.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int unlinkLedFromBlinkSeq(int ledIndex, int count)
    {
        return this.sendCommand("US"+Convert.ToString(ledIndex)+","+Convert.ToString(count));
    }

    /**
     * <summary>
     *   Starts a sequence execution: every LED linked to that sequence starts to
     *   run it in a loop.
     * <para>
     *   Note that a sequence with a zero duration can't be started.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the sequence to start.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int startBlinkSeq(int seqIndex)
    {
        return this.sendCommand("SS"+Convert.ToString(seqIndex));
    }

    /**
     * <summary>
     *   Stops a sequence execution.
     * <para>
     *   If started again, the execution
     *   restarts from the beginning.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the sequence to stop.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int stopBlinkSeq(int seqIndex)
    {
        return this.sendCommand("XS"+Convert.ToString(seqIndex));
    }

    /**
     * <summary>
     *   Stops a sequence execution and resets its contents.
     * <para>
     *   LEDs linked to this
     *   sequence are not automatically updated anymore.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the sequence to reset
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int resetBlinkSeq(int seqIndex)
    {
        return this.sendCommand("ZS"+Convert.ToString(seqIndex));
    }

    /**
     * <summary>
     *   Configures a sequence to make it start automatically at device
     *   startup.
     * <para>
     *   Note that a sequence with a zero duration can't be started.
     *   Don't forget to call <c>saveBlinkSeq()</c> to make sure the
     *   modification is saved in the device flash memory.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the sequence to reset.
     * </param>
     * <param name="autostart">
     *   0 to keep the sequence turned off and 1 to start it automatically.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_blinkSeqStateAtPowerOn(int seqIndex, int autostart)
    {
        return this.sendCommand("AS"+Convert.ToString(seqIndex)+","+Convert.ToString(autostart));
    }

    /**
     * <summary>
     *   Changes the execution speed of a sequence.
     * <para>
     *   The natural execution speed is 1000 per
     *   thousand. If you configure a slower speed, you can play the sequence in slow-motion.
     *   If you set a negative speed, you can play the sequence in reverse direction.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the sequence to start.
     * </param>
     * <param name="speed">
     *   sequence running speed (-1000...1000).
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_blinkSeqSpeed(int seqIndex, int speed)
    {
        return this.sendCommand("CS"+Convert.ToString(seqIndex)+","+Convert.ToString(speed));
    }

    /**
     * <summary>
     *   Saves the LEDs power-on configuration.
     * <para>
     *   This includes the start-up color or
     *   sequence binding for all LEDs. Warning: if some LEDs are linked to a sequence, the
     *   method <c>saveBlinkSeq()</c> must also be called to save the sequence definition.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int saveLedsConfigAtPowerOn()
    {
        return this.sendCommand("WL");
    }

    public virtual int saveLedsState()
    {
        return this.sendCommand("WL");
    }

    /**
     * <summary>
     *   Saves the definition of a sequence.
     * <para>
     *   Warning: only sequence steps and flags are saved.
     *   to save the LEDs startup bindings, the method <c>saveLedsConfigAtPowerOn()</c>
     *   must be called.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the sequence to start.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int saveBlinkSeq(int seqIndex)
    {
        return this.sendCommand("WS"+Convert.ToString(seqIndex));
    }

    /**
     * <summary>
     *   Sends a binary buffer to the LED RGB buffer, as is.
     * <para>
     *   First three bytes are RGB components for LED specified as parameter, the
     *   next three bytes for the next LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be updated
     * </param>
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
    public virtual int set_rgbColorBuffer(int ledIndex, byte[] buff)
    {
        return this._upload("rgb:0:"+Convert.ToString(ledIndex), buff);
    }

    /**
     * <summary>
     *   Sends 24bit RGB colors (provided as a list of integers) to the LED RGB buffer, as is.
     * <para>
     *   The first number represents the RGB value of the LED specified as parameter, the second
     *   number represents the RGB value of the next LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be updated
     * </param>
     * <param name="rgbList">
     *   a list of 24bit RGB codes, in the form 0xRRGGBB
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_rgbColorArray(int ledIndex, List<int> rgbList)
    {
        int listlen;
        byte[] buff;
        int idx;
        int rgb;
        int res;
        listlen = rgbList.Count;
        buff = new byte[3*listlen];
        idx = 0;
        while (idx < listlen) {
            rgb = rgbList[idx];
            buff[3*idx] = (byte)(((((rgb) >> (16))) & (255)) & 0xff);
            buff[3*idx+1] = (byte)(((((rgb) >> (8))) & (255)) & 0xff);
            buff[3*idx+2] = (byte)(((rgb) & (255)) & 0xff);
            idx = idx + 1;
        }

        res = this._upload("rgb:0:"+Convert.ToString(ledIndex), buff);
        return res;
    }

    /**
     * <summary>
     *   Sets up a smooth RGB color transition to the specified pixel-by-pixel list of RGB
     *   color codes.
     * <para>
     *   The first color code represents the target RGB value of the first LED,
     *   the next color code represents the target value of the next LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be updated
     * </param>
     * <param name="rgbList">
     *   a list of target 24bit RGB codes, in the form 0xRRGGBB
     * </param>
     * <param name="delay">
     *   transition duration in ms
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int rgbArrayOfs_move(int ledIndex, List<int> rgbList, int delay)
    {
        int listlen;
        byte[] buff;
        int idx;
        int rgb;
        int res;
        listlen = rgbList.Count;
        buff = new byte[3*listlen];
        idx = 0;
        while (idx < listlen) {
            rgb = rgbList[idx];
            buff[3*idx] = (byte)(((((rgb) >> (16))) & (255)) & 0xff);
            buff[3*idx+1] = (byte)(((((rgb) >> (8))) & (255)) & 0xff);
            buff[3*idx+2] = (byte)(((rgb) & (255)) & 0xff);
            idx = idx + 1;
        }

        res = this._upload("rgb:"+Convert.ToString(delay)+":"+Convert.ToString(ledIndex), buff);
        return res;
    }

    /**
     * <summary>
     *   Sets up a smooth RGB color transition to the specified pixel-by-pixel list of RGB
     *   color codes.
     * <para>
     *   The first color code represents the target RGB value of the first LED,
     *   the next color code represents the target value of the next LED, etc.
     * </para>
     * </summary>
     * <param name="rgbList">
     *   a list of target 24bit RGB codes, in the form 0xRRGGBB
     * </param>
     * <param name="delay">
     *   transition duration in ms
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int rgbArray_move(List<int> rgbList, int delay)
    {
        int res;

        res = this.rgbArrayOfs_move(0,rgbList,delay);
        return res;
    }

    /**
     * <summary>
     *   Sends a binary buffer to the LED HSL buffer, as is.
     * <para>
     *   First three bytes are HSL components for the LED specified as parameter, the
     *   next three bytes for the second LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be updated
     * </param>
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
    public virtual int set_hslColorBuffer(int ledIndex, byte[] buff)
    {
        return this._upload("hsl:0:"+Convert.ToString(ledIndex), buff);
    }

    /**
     * <summary>
     *   Sends 24bit HSL colors (provided as a list of integers) to the LED HSL buffer, as is.
     * <para>
     *   The first number represents the HSL value of the LED specified as parameter, the second number represents
     *   the HSL value of the second LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be updated
     * </param>
     * <param name="hslList">
     *   a list of 24bit HSL codes, in the form 0xHHSSLL
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_hslColorArray(int ledIndex, List<int> hslList)
    {
        int listlen;
        byte[] buff;
        int idx;
        int hsl;
        int res;
        listlen = hslList.Count;
        buff = new byte[3*listlen];
        idx = 0;
        while (idx < listlen) {
            hsl = hslList[idx];
            buff[3*idx] = (byte)(((((hsl) >> (16))) & (255)) & 0xff);
            buff[3*idx+1] = (byte)(((((hsl) >> (8))) & (255)) & 0xff);
            buff[3*idx+2] = (byte)(((hsl) & (255)) & 0xff);
            idx = idx + 1;
        }

        res = this._upload("hsl:0:"+Convert.ToString(ledIndex), buff);
        return res;
    }

    /**
     * <summary>
     *   Sets up a smooth HSL color transition to the specified pixel-by-pixel list of HSL
     *   color codes.
     * <para>
     *   The first color code represents the target HSL value of the first LED,
     *   the second color code represents the target value of the second LED, etc.
     * </para>
     * </summary>
     * <param name="hslList">
     *   a list of target 24bit HSL codes, in the form 0xHHSSLL
     * </param>
     * <param name="delay">
     *   transition duration in ms
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int hslArray_move(List<int> hslList, int delay)
    {
        int res;

        res = this.hslArrayOfs_move(0,hslList, delay);
        return res;
    }

    /**
     * <summary>
     *   Sets up a smooth HSL color transition to the specified pixel-by-pixel list of HSL
     *   color codes.
     * <para>
     *   The first color code represents the target HSL value of the first LED,
     *   the second color code represents the target value of the second LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be updated
     * </param>
     * <param name="hslList">
     *   a list of target 24bit HSL codes, in the form 0xHHSSLL
     * </param>
     * <param name="delay">
     *   transition duration in ms
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int hslArrayOfs_move(int ledIndex, List<int> hslList, int delay)
    {
        int listlen;
        byte[] buff;
        int idx;
        int hsl;
        int res;
        listlen = hslList.Count;
        buff = new byte[3*listlen];
        idx = 0;
        while (idx < listlen) {
            hsl = hslList[idx];
            buff[3*idx] = (byte)(((((hsl) >> (16))) & (255)) & 0xff);
            buff[3*idx+1] = (byte)(((((hsl) >> (8))) & (255)) & 0xff);
            buff[3*idx+2] = (byte)(((hsl) & (255)) & 0xff);
            idx = idx + 1;
        }

        res = this._upload("hsl:"+Convert.ToString(delay)+":"+Convert.ToString(ledIndex), buff);
        return res;
    }

    /**
     * <summary>
     *   Returns a binary buffer with content from the LED RGB buffer, as is.
     * <para>
     *   First three bytes are RGB components for the first LED in the interval,
     *   the next three bytes for the second LED in the interval, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be returned
     * </param>
     * <param name="count">
     *   number of LEDs which should be returned
     * </param>
     * <returns>
     *   a binary buffer with RGB components of selected LEDs.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty binary buffer.
     * </para>
     */
    public virtual byte[] get_rgbColorBuffer(int ledIndex, int count)
    {
        return this._download("rgb.bin?typ=0&pos="+Convert.ToString(3*ledIndex)+"&len="+Convert.ToString(3*count));
    }

    /**
     * <summary>
     *   Returns a list on 24bit RGB color values with the current colors displayed on
     *   the RGB LEDs.
     * <para>
     *   The first number represents the RGB value of the first LED,
     *   the second number represents the RGB value of the second LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be returned
     * </param>
     * <param name="count">
     *   number of LEDs which should be returned
     * </param>
     * <returns>
     *   a list of 24bit color codes with RGB components of selected LEDs, as 0xRRGGBB.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<int> get_rgbColorArray(int ledIndex, int count)
    {
        byte[] buff;
        List<int> res = new List<int>();
        int idx;
        int r;
        int g;
        int b;

        buff = this._download("rgb.bin?typ=0&pos="+Convert.ToString(3*ledIndex)+"&len="+Convert.ToString(3*count));
        res.Clear();
        idx = 0;
        while (idx < count) {
            r = buff[3*idx];
            g = buff[3*idx+1];
            b = buff[3*idx+2];
            res.Add(r*65536+g*256+b);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns a list on 24bit RGB color values with the RGB LEDs startup colors.
     * <para>
     *   The first number represents the startup RGB value of the first LED,
     *   the second number represents the RGB value of the second LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED  which should be returned
     * </param>
     * <param name="count">
     *   number of LEDs which should be returned
     * </param>
     * <returns>
     *   a list of 24bit color codes with RGB components of selected LEDs, as 0xRRGGBB.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<int> get_rgbColorArrayAtPowerOn(int ledIndex, int count)
    {
        byte[] buff;
        List<int> res = new List<int>();
        int idx;
        int r;
        int g;
        int b;

        buff = this._download("rgb.bin?typ=4&pos="+Convert.ToString(3*ledIndex)+"&len="+Convert.ToString(3*count));
        res.Clear();
        idx = 0;
        while (idx < count) {
            r = buff[3*idx];
            g = buff[3*idx+1];
            b = buff[3*idx+2];
            res.Add(r*65536+g*256+b);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns a list on sequence index for each RGB LED.
     * <para>
     *   The first number represents the
     *   sequence index for the the first LED, the second number represents the sequence
     *   index for the second LED, etc.
     * </para>
     * </summary>
     * <param name="ledIndex">
     *   index of the first LED which should be returned
     * </param>
     * <param name="count">
     *   number of LEDs which should be returned
     * </param>
     * <returns>
     *   a list of integers with sequence index
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<int> get_linkedSeqArray(int ledIndex, int count)
    {
        byte[] buff;
        List<int> res = new List<int>();
        int idx;
        int seq;

        buff = this._download("rgb.bin?typ=1&pos="+Convert.ToString(ledIndex)+"&len="+Convert.ToString(count));
        res.Clear();
        idx = 0;
        while (idx < count) {
            seq = buff[idx];
            res.Add(seq);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns a list on 32 bit signatures for specified blinking sequences.
     * <para>
     *   Since blinking sequences cannot be read from the device, this can be used
     *   to detect if a specific blinking sequence is already programmed.
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the first blinking sequence which should be returned
     * </param>
     * <param name="count">
     *   number of blinking sequences which should be returned
     * </param>
     * <returns>
     *   a list of 32 bit integer signatures
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<int> get_blinkSeqSignatures(int seqIndex, int count)
    {
        byte[] buff;
        List<int> res = new List<int>();
        int idx;
        int hh;
        int hl;
        int lh;
        int ll;

        buff = this._download("rgb.bin?typ=2&pos="+Convert.ToString(4*seqIndex)+"&len="+Convert.ToString(4*count));
        res.Clear();
        idx = 0;
        while (idx < count) {
            hh = buff[4*idx];
            hl = buff[4*idx+1];
            lh = buff[4*idx+2];
            ll = buff[4*idx+3];
            res.Add(((hh) << (24))+((hl) << (16))+((lh) << (8))+ll);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns a list of integers with the current speed for specified blinking sequences.
     * <para>
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the first sequence speed which should be returned
     * </param>
     * <param name="count">
     *   number of sequence speeds which should be returned
     * </param>
     * <returns>
     *   a list of integers, 0 for sequences turned off and 1 for sequences running
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<int> get_blinkSeqStateSpeed(int seqIndex, int count)
    {
        byte[] buff;
        List<int> res = new List<int>();
        int idx;
        int lh;
        int ll;

        buff = this._download("rgb.bin?typ=6&pos="+Convert.ToString(seqIndex)+"&len="+Convert.ToString(count));
        res.Clear();
        idx = 0;
        while (idx < count) {
            lh = buff[2*idx];
            ll = buff[2*idx+1];
            res.Add(((lh) << (8))+ll);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns a list of integers with the "auto-start at power on" flag state for specified blinking sequences.
     * <para>
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the first blinking sequence which should be returned
     * </param>
     * <param name="count">
     *   number of blinking sequences which should be returned
     * </param>
     * <returns>
     *   a list of integers, 0 for sequences turned off and 1 for sequences running
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<int> get_blinkSeqStateAtPowerOn(int seqIndex, int count)
    {
        byte[] buff;
        List<int> res = new List<int>();
        int idx;
        int started;

        buff = this._download("rgb.bin?typ=5&pos="+Convert.ToString(seqIndex)+"&len="+Convert.ToString(count));
        res.Clear();
        idx = 0;
        while (idx < count) {
            started = buff[idx];
            res.Add(started);
            idx = idx + 1;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns a list of integers with the started state for specified blinking sequences.
     * <para>
     * </para>
     * </summary>
     * <param name="seqIndex">
     *   index of the first blinking sequence which should be returned
     * </param>
     * <param name="count">
     *   number of blinking sequences which should be returned
     * </param>
     * <returns>
     *   a list of integers, 0 for sequences turned off and 1 for sequences running
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<int> get_blinkSeqState(int seqIndex, int count)
    {
        byte[] buff;
        List<int> res = new List<int>();
        int idx;
        int started;

        buff = this._download("rgb.bin?typ=3&pos="+Convert.ToString(seqIndex)+"&len="+Convert.ToString(count));
        res.Clear();
        idx = 0;
        while (idx < count) {
            started = buff[idx];
            res.Add(started);
            idx = idx + 1;
        }
        return res;
    }

    public virtual int hsl2rgbInt(int temp1, int temp2, int temp3)
    {
        if (temp3 >= 170) {
            return (((temp1 + 127)) / (255));
        }
        if (temp3 > 42) {
            if (temp3 <= 127) {
                return (((temp2 + 127)) / (255));
            }
            temp3 = 170 - temp3;
        }
        return (((temp1*255 + (temp2-temp1) * (6 * temp3) + 32512)) / (65025));
    }

    public virtual int hsl2rgb(int hslValue)
    {
        int R;
        int G;
        int B;
        int H;
        int S;
        int L;
        int temp1;
        int temp2;
        int temp3;
        int res;
        L = ((hslValue) & (0xff));
        S = ((((hslValue) >> (8))) & (0xff));
        H = ((((hslValue) >> (16))) & (0xff));
        if (S==0) {
            res = ((L) << (16))+((L) << (8))+L;
            return res;
        }
        if (L<=127) {
            temp2 = L * (255 + S);
        } else {
            temp2 = (L+S) * 255 - L*S;
        }
        temp1 = 510 * L - temp2;
        // R
        temp3 = (H + 85);
        if (temp3 > 255) {
            temp3 = temp3-255;
        }
        R = this.hsl2rgbInt(temp1, temp2, temp3);
        // G
        temp3 = H;
        if (temp3 > 255) {
            temp3 = temp3-255;
        }
        G = this.hsl2rgbInt(temp1, temp2, temp3);
        // B
        if (H >= 85) {
            temp3 = H - 85 ;
        } else {
            temp3 = H + 170;
        }
        B = this.hsl2rgbInt(temp1, temp2, temp3);
        // just in case
        if (R>255) {
            R=255;
        }
        if (G>255) {
            G=255;
        }
        if (B>255) {
            B=255;
        }
        res = ((R) << (16))+((G) << (8))+B;
        return res;
    }

    /**
     * <summary>
     *   Continues the enumeration of RGB LED clusters started using <c>yFirstColorLedCluster()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned RGB LED clusters order.
     *   If you want to find a specific a RGB LED cluster, use <c>ColorLedCluster.findColorLedCluster()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YColorLedCluster</c> object, corresponding to
     *   a RGB LED cluster currently online, or a <c>null</c> pointer
     *   if there are no more RGB LED clusters to enumerate.
     * </returns>
     */
    public YColorLedCluster nextColorLedCluster()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindColorLedCluster(hwid);
    }

    //--- (end of YColorLedCluster implementation)

    //--- (YColorLedCluster functions)

    /**
     * <summary>
     *   Starts the enumeration of RGB LED clusters currently accessible.
     * <para>
     *   Use the method <c>YColorLedCluster.nextColorLedCluster()</c> to iterate on
     *   next RGB LED clusters.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YColorLedCluster</c> object, corresponding to
     *   the first RGB LED cluster currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YColorLedCluster FirstColorLedCluster()
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
        err = YAPI.apiGetFunctionsByClass("ColorLedCluster", 0, p, size, ref neededsize, ref errmsg);
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
        return FindColorLedCluster(serial + "." + funcId);
    }



    //--- (end of YColorLedCluster functions)
}
#pragma warning restore 1591
