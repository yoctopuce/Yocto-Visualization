/*********************************************************************
 *
 *  $Id: yocto_bluetoothlink.cs 37619 2019-10-11 11:52:42Z mvuilleu $
 *
 *  Implements yFindBluetoothLink(), the high-level API for BluetoothLink functions
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
    //--- (YBluetoothLink return codes)
    //--- (end of YBluetoothLink return codes)
//--- (YBluetoothLink dlldef)
//--- (end of YBluetoothLink dlldef)
//--- (YBluetoothLink yapiwrapper)
//--- (end of YBluetoothLink yapiwrapper)
//--- (YBluetoothLink class start)
/**
 * <summary>
 *   BluetoothLink function provides control over bluetooth link
 *   and status for devices that are bluetooth-enabled.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YBluetoothLink : YFunction
{
//--- (end of YBluetoothLink class start)
    //--- (YBluetoothLink definitions)
    public new delegate void ValueCallback(YBluetoothLink func, string value);
    public new delegate void TimedReportCallback(YBluetoothLink func, YMeasure measure);

    public const string OWNADDRESS_INVALID = YAPI.INVALID_STRING;
    public const string PAIRINGPIN_INVALID = YAPI.INVALID_STRING;
    public const string REMOTEADDRESS_INVALID = YAPI.INVALID_STRING;
    public const string REMOTENAME_INVALID = YAPI.INVALID_STRING;
    public const int MUTE_FALSE = 0;
    public const int MUTE_TRUE = 1;
    public const int MUTE_INVALID = -1;
    public const int PREAMPLIFIER_INVALID = YAPI.INVALID_UINT;
    public const int VOLUME_INVALID = YAPI.INVALID_UINT;
    public const int LINKSTATE_DOWN = 0;
    public const int LINKSTATE_FREE = 1;
    public const int LINKSTATE_SEARCH = 2;
    public const int LINKSTATE_EXISTS = 3;
    public const int LINKSTATE_LINKED = 4;
    public const int LINKSTATE_PLAY = 5;
    public const int LINKSTATE_INVALID = -1;
    public const int LINKQUALITY_INVALID = YAPI.INVALID_UINT;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected string _ownAddress = OWNADDRESS_INVALID;
    protected string _pairingPin = PAIRINGPIN_INVALID;
    protected string _remoteAddress = REMOTEADDRESS_INVALID;
    protected string _remoteName = REMOTENAME_INVALID;
    protected int _mute = MUTE_INVALID;
    protected int _preAmplifier = PREAMPLIFIER_INVALID;
    protected int _volume = VOLUME_INVALID;
    protected int _linkState = LINKSTATE_INVALID;
    protected int _linkQuality = LINKQUALITY_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackBluetoothLink = null;
    //--- (end of YBluetoothLink definitions)

    public YBluetoothLink(string func)
        : base(func)
    {
        _className = "BluetoothLink";
        //--- (YBluetoothLink attributes initialization)
        //--- (end of YBluetoothLink attributes initialization)
    }

    //--- (YBluetoothLink implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("ownAddress"))
        {
            _ownAddress = json_val.getString("ownAddress");
        }
        if (json_val.has("pairingPin"))
        {
            _pairingPin = json_val.getString("pairingPin");
        }
        if (json_val.has("remoteAddress"))
        {
            _remoteAddress = json_val.getString("remoteAddress");
        }
        if (json_val.has("remoteName"))
        {
            _remoteName = json_val.getString("remoteName");
        }
        if (json_val.has("mute"))
        {
            _mute = json_val.getInt("mute") > 0 ? 1 : 0;
        }
        if (json_val.has("preAmplifier"))
        {
            _preAmplifier = json_val.getInt("preAmplifier");
        }
        if (json_val.has("volume"))
        {
            _volume = json_val.getInt("volume");
        }
        if (json_val.has("linkState"))
        {
            _linkState = json_val.getInt("linkState");
        }
        if (json_val.has("linkQuality"))
        {
            _linkQuality = json_val.getInt("linkQuality");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the MAC-48 address of the bluetooth interface, which is unique on the bluetooth network.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the MAC-48 address of the bluetooth interface, which is unique on the
     *   bluetooth network
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.OWNADDRESS_INVALID</c>.
     * </para>
     */
    public string get_ownAddress()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return OWNADDRESS_INVALID;
                }
            }
            res = this._ownAddress;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns an opaque string if a PIN code has been configured in the device to access
     *   the SIM card, or an empty string if none has been configured or if the code provided
     *   was rejected by the SIM card.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to an opaque string if a PIN code has been configured in the device to access
     *   the SIM card, or an empty string if none has been configured or if the code provided
     *   was rejected by the SIM card
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.PAIRINGPIN_INVALID</c>.
     * </para>
     */
    public string get_pairingPin()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PAIRINGPIN_INVALID;
                }
            }
            res = this._pairingPin;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the PIN code used by the module for bluetooth pairing.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module to save the
     *   new value in the device flash.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the PIN code used by the module for bluetooth pairing
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
    public int set_pairingPin(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("pairingPin", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the MAC-48 address of the remote device to connect to.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the MAC-48 address of the remote device to connect to
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.REMOTEADDRESS_INVALID</c>.
     * </para>
     */
    public string get_remoteAddress()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return REMOTEADDRESS_INVALID;
                }
            }
            res = this._remoteAddress;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the MAC-48 address defining which remote device to connect to.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the MAC-48 address defining which remote device to connect to
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
    public int set_remoteAddress(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("remoteAddress", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the bluetooth name the remote device, if found on the bluetooth network.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the bluetooth name the remote device, if found on the bluetooth network
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.REMOTENAME_INVALID</c>.
     * </para>
     */
    public string get_remoteName()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return REMOTENAME_INVALID;
                }
            }
            res = this._remoteName;
        }
        return res;
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
     *   either <c>YBluetoothLink.MUTE_FALSE</c> or <c>YBluetoothLink.MUTE_TRUE</c>, according to the state
     *   of the mute function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.MUTE_INVALID</c>.
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
     *   either <c>YBluetoothLink.MUTE_FALSE</c> or <c>YBluetoothLink.MUTE_TRUE</c>, according to the state
     *   of the mute function
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
     *   Returns the audio pre-amplifier volume, in per cents.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the audio pre-amplifier volume, in per cents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.PREAMPLIFIER_INVALID</c>.
     * </para>
     */
    public int get_preAmplifier()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PREAMPLIFIER_INVALID;
                }
            }
            res = this._preAmplifier;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the audio pre-amplifier volume, in per cents.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the audio pre-amplifier volume, in per cents
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
    public int set_preAmplifier(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("preAmplifier", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the connected headset volume, in per cents.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the connected headset volume, in per cents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.VOLUME_INVALID</c>.
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
     *   Changes the connected headset volume, in per cents.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the connected headset volume, in per cents
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
     *   Returns the bluetooth link state.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YBluetoothLink.LINKSTATE_DOWN</c>, <c>YBluetoothLink.LINKSTATE_FREE</c>,
     *   <c>YBluetoothLink.LINKSTATE_SEARCH</c>, <c>YBluetoothLink.LINKSTATE_EXISTS</c>,
     *   <c>YBluetoothLink.LINKSTATE_LINKED</c> and <c>YBluetoothLink.LINKSTATE_PLAY</c> corresponding to
     *   the bluetooth link state
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.LINKSTATE_INVALID</c>.
     * </para>
     */
    public int get_linkState()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LINKSTATE_INVALID;
                }
            }
            res = this._linkState;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the bluetooth receiver signal strength, in pourcents, or 0 if no connection is established.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the bluetooth receiver signal strength, in pourcents, or 0 if no
     *   connection is established
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBluetoothLink.LINKQUALITY_INVALID</c>.
     * </para>
     */
    public int get_linkQuality()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LINKQUALITY_INVALID;
                }
            }
            res = this._linkQuality;
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
     *   Retrieves a cellular interface for a given identifier.
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
     *   This function does not require that the cellular interface is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YBluetoothLink.isOnline()</c> to test if the cellular interface is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a cellular interface by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the cellular interface
     * </param>
     * <returns>
     *   a <c>YBluetoothLink</c> object allowing you to drive the cellular interface.
     * </returns>
     */
    public static YBluetoothLink FindBluetoothLink(string func)
    {
        YBluetoothLink obj;
        lock (YAPI.globalLock) {
            obj = (YBluetoothLink) YFunction._FindFromCache("BluetoothLink", func);
            if (obj == null) {
                obj = new YBluetoothLink(func);
                YFunction._AddToCache("BluetoothLink", func, obj);
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
        this._valueCallbackBluetoothLink = callback;
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
        if (this._valueCallbackBluetoothLink != null) {
            this._valueCallbackBluetoothLink(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Attempt to connect to the previously selected remote device.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int connect()
    {
        return this.set_command("C");
    }

    /**
     * <summary>
     *   Disconnect from the previously selected remote device.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int disconnect()
    {
        return this.set_command("D");
    }

    /**
     * <summary>
     *   Continues the enumeration of cellular interfaces started using <c>yFirstBluetoothLink()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned cellular interfaces order.
     *   If you want to find a specific a cellular interface, use <c>BluetoothLink.findBluetoothLink()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YBluetoothLink</c> object, corresponding to
     *   a cellular interface currently online, or a <c>null</c> pointer
     *   if there are no more cellular interfaces to enumerate.
     * </returns>
     */
    public YBluetoothLink nextBluetoothLink()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindBluetoothLink(hwid);
    }

    //--- (end of YBluetoothLink implementation)

    //--- (YBluetoothLink functions)

    /**
     * <summary>
     *   Starts the enumeration of cellular interfaces currently accessible.
     * <para>
     *   Use the method <c>YBluetoothLink.nextBluetoothLink()</c> to iterate on
     *   next cellular interfaces.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YBluetoothLink</c> object, corresponding to
     *   the first cellular interface currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YBluetoothLink FirstBluetoothLink()
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
        err = YAPI.apiGetFunctionsByClass("BluetoothLink", 0, p, size, ref neededsize, ref errmsg);
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
        return FindBluetoothLink(serial + "." + funcId);
    }



    //--- (end of YBluetoothLink functions)
}
#pragma warning restore 1591
