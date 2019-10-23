/*********************************************************************
 *
 *  $Id: yocto_audioout.cs 37619 2019-10-11 11:52:42Z mvuilleu $
 *
 *  Implements yFindAudioOut(), the high-level API for AudioOut functions
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
    //--- (YAudioOut return codes)
    //--- (end of YAudioOut return codes)
//--- (YAudioOut dlldef)
//--- (end of YAudioOut dlldef)
//--- (YAudioOut yapiwrapper)
//--- (end of YAudioOut yapiwrapper)
//--- (YAudioOut class start)
/**
 * <summary>
 *   The Yoctopuce application programming interface allows you to configure the volume of the outout.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YAudioOut : YFunction
{
//--- (end of YAudioOut class start)
    //--- (YAudioOut definitions)
    public new delegate void ValueCallback(YAudioOut func, string value);
    public new delegate void TimedReportCallback(YAudioOut func, YMeasure measure);

    public const int VOLUME_INVALID = YAPI.INVALID_UINT;
    public const int MUTE_FALSE = 0;
    public const int MUTE_TRUE = 1;
    public const int MUTE_INVALID = -1;
    public const string VOLUMERANGE_INVALID = YAPI.INVALID_STRING;
    public const int SIGNAL_INVALID = YAPI.INVALID_INT;
    public const int NOSIGNALFOR_INVALID = YAPI.INVALID_INT;
    protected int _volume = VOLUME_INVALID;
    protected int _mute = MUTE_INVALID;
    protected string _volumeRange = VOLUMERANGE_INVALID;
    protected int _signal = SIGNAL_INVALID;
    protected int _noSignalFor = NOSIGNALFOR_INVALID;
    protected ValueCallback _valueCallbackAudioOut = null;
    //--- (end of YAudioOut definitions)

    public YAudioOut(string func)
        : base(func)
    {
        _className = "AudioOut";
        //--- (YAudioOut attributes initialization)
        //--- (end of YAudioOut attributes initialization)
    }

    //--- (YAudioOut implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("volume"))
        {
            _volume = json_val.getInt("volume");
        }
        if (json_val.has("mute"))
        {
            _mute = json_val.getInt("mute") > 0 ? 1 : 0;
        }
        if (json_val.has("volumeRange"))
        {
            _volumeRange = json_val.getString("volumeRange");
        }
        if (json_val.has("signal"))
        {
            _signal = json_val.getInt("signal");
        }
        if (json_val.has("noSignalFor"))
        {
            _noSignalFor = json_val.getInt("noSignalFor");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns audio output volume, in per cents.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to audio output volume, in per cents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAudioOut.VOLUME_INVALID</c>.
     * </para>
     */
    public int get_volume()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VOLUME_INVALID;
                }
            }
            res = this._volume;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes audio output volume, in per cents.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to audio output volume, in per cents
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
    public int set_volume(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("volume", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the state of the mute function.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YAudioOut.MUTE_FALSE</c> or <c>YAudioOut.MUTE_TRUE</c>, according to the state of the mute function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAudioOut.MUTE_INVALID</c>.
     * </para>
     */
    public int get_mute()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MUTE_INVALID;
                }
            }
            res = this._mute;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the state of the mute function.
     * <para>
     *   Remember to call the matching module
     *   <c>saveToFlash()</c> method to save the setting permanently.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YAudioOut.MUTE_FALSE</c> or <c>YAudioOut.MUTE_TRUE</c>, according to the state of the mute function
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
    public int set_mute(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("mute", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the supported volume range.
     * <para>
     *   The low value of the
     *   range corresponds to the minimal audible value. To
     *   completely mute the sound, use <c>set_mute()</c>
     *   instead of the <c>set_volume()</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the supported volume range
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAudioOut.VOLUMERANGE_INVALID</c>.
     * </para>
     */
    public string get_volumeRange()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return VOLUMERANGE_INVALID;
                }
            }
            res = this._volumeRange;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the detected output current level.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the detected output current level
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAudioOut.SIGNAL_INVALID</c>.
     * </para>
     */
    public int get_signal()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SIGNAL_INVALID;
                }
            }
            res = this._signal;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the number of seconds elapsed without detecting a signal.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of seconds elapsed without detecting a signal
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAudioOut.NOSIGNALFOR_INVALID</c>.
     * </para>
     */
    public int get_noSignalFor()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return NOSIGNALFOR_INVALID;
                }
            }
            res = this._noSignalFor;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves an audio output for a given identifier.
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
     *   This function does not require that the audio output is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YAudioOut.isOnline()</c> to test if the audio output is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   an audio output by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the audio output
     * </param>
     * <returns>
     *   a <c>YAudioOut</c> object allowing you to drive the audio output.
     * </returns>
     */
    public static YAudioOut FindAudioOut(string func)
    {
        YAudioOut obj;
        lock (YAPI.globalLock) {
            obj = (YAudioOut) YFunction._FindFromCache("AudioOut", func);
            if (obj == null) {
                obj = new YAudioOut(func);
                YFunction._AddToCache("AudioOut", func, obj);
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
        this._valueCallbackAudioOut = callback;
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
        if (this._valueCallbackAudioOut != null) {
            this._valueCallbackAudioOut(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of audio outputs started using <c>yFirstAudioOut()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned audio outputs order.
     *   If you want to find a specific an audio output, use <c>AudioOut.findAudioOut()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YAudioOut</c> object, corresponding to
     *   an audio output currently online, or a <c>null</c> pointer
     *   if there are no more audio outputs to enumerate.
     * </returns>
     */
    public YAudioOut nextAudioOut()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindAudioOut(hwid);
    }

    //--- (end of YAudioOut implementation)

    //--- (YAudioOut functions)

    /**
     * <summary>
     *   Starts the enumeration of audio outputs currently accessible.
     * <para>
     *   Use the method <c>YAudioOut.nextAudioOut()</c> to iterate on
     *   next audio outputs.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YAudioOut</c> object, corresponding to
     *   the first audio output currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YAudioOut FirstAudioOut()
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
        err = YAPI.apiGetFunctionsByClass("AudioOut", 0, p, size, ref neededsize, ref errmsg);
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
        return FindAudioOut(serial + "." + funcId);
    }



    //--- (end of YAudioOut functions)
}
#pragma warning restore 1591
