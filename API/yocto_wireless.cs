/*********************************************************************
 *
 * $Id: yocto_wireless.cs 38510 2019-11-26 15:36:38Z mvuilleu $
 *
 * Implements yFindWireless(), the high-level API for Wireless functions
 *
 * - - - - - - - - - License information: - - - - - - - - -
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
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT
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
//--- (generated code: YWlanRecord class start)
/**
 * <summary>
 *   YWlanRecord objects are used to describe a wireless network.
 * <para>
 *   These objects are  used in particular in conjunction with the
 *   YWireless class.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YWlanRecord
{
//--- (end of generated code: YWlanRecord class start)
    //--- (generated code: YWlanRecord definitions)

    protected string _ssid;
    protected int _channel = 0;
    protected string _sec;
    protected int _rssi = 0;
    //--- (end of generated code: YWlanRecord definitions)

    public YWlanRecord(string data)
    {
        //--- (generated code: YWlanRecord attributes initialization)
        //--- (end of generated code: YWlanRecord attributes initialization)
        YAPI.YJSONObject p = new YAPI.YJSONObject(data);
        p.parse();
        this._ssid = p.getString("ssid");
        this._sec = p.getString("sec");
        this._rssi = p.getInt("rssi");
        this._channel = p.getInt("channel");
    }

  //--- (generated code: YWlanRecord implementation)


    /**
     * <summary>
     *   Returns the name of the wireless network (SSID).
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string with the name of the wireless network (SSID).
     * </returns>
     */
    public virtual string get_ssid()
    {
        return this._ssid;
    }

    /**
     * <summary>
     *   Returns the 802.11 channel.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the 802.11 channel.
     * </returns>
     */
    public virtual int get_channel()
    {
        return this._channel;
    }

    /**
     * <summary>
     *   Returns the security algorithm used by the wireless network.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string with the security algorithm.
     * </returns>
     */
    public virtual string get_security()
    {
        return this._sec;
    }

    /**
     * <summary>
     *   Returns the quality of the wireless network link, in per cents.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the quality of the wireless network link, in per cents.
     * </returns>
     */
    public virtual int get_linkQuality()
    {
        return this._rssi;
    }

    //--- (end of generated code: YWlanRecord implementation)

}


//--- (generated code: YWireless class start)
/**
 * <summary>
 *   The YWireless class provides control over wireless network parameters
 *   and status for devices that are wireless-enabled, for instance using a YoctoHub-Wireless, a YoctoHub-Wireless-SR or a YoctoHub-Wireless-g.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YWireless : YFunction
{
//--- (end of generated code: YWireless class start)
    //--- (generated code: YWireless definitions)
    public new delegate void ValueCallback(YWireless func, string value);
    public new delegate void TimedReportCallback(YWireless func, YMeasure measure);

    public const int LINKQUALITY_INVALID = YAPI.INVALID_UINT;
    public const string SSID_INVALID = YAPI.INVALID_STRING;
    public const int CHANNEL_INVALID = YAPI.INVALID_UINT;
    public const int SECURITY_UNKNOWN = 0;
    public const int SECURITY_OPEN = 1;
    public const int SECURITY_WEP = 2;
    public const int SECURITY_WPA = 3;
    public const int SECURITY_WPA2 = 4;
    public const int SECURITY_INVALID = -1;
    public const string MESSAGE_INVALID = YAPI.INVALID_STRING;
    public const string WLANCONFIG_INVALID = YAPI.INVALID_STRING;
    public const int WLANSTATE_DOWN = 0;
    public const int WLANSTATE_SCANNING = 1;
    public const int WLANSTATE_CONNECTED = 2;
    public const int WLANSTATE_REJECTED = 3;
    public const int WLANSTATE_INVALID = -1;
    protected int _linkQuality = LINKQUALITY_INVALID;
    protected string _ssid = SSID_INVALID;
    protected int _channel = CHANNEL_INVALID;
    protected int _security = SECURITY_INVALID;
    protected string _message = MESSAGE_INVALID;
    protected string _wlanConfig = WLANCONFIG_INVALID;
    protected int _wlanState = WLANSTATE_INVALID;
    protected ValueCallback _valueCallbackWireless = null;
    //--- (end of generated code: YWireless definitions)

    public YWireless(string func)
        : base(func)
    {
        _className = "Wireless";
        //--- (generated code: YWireless attributes initialization)
        //--- (end of generated code: YWireless attributes initialization)
    }

  //--- (generated code: YWireless implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("linkQuality"))
        {
            _linkQuality = json_val.getInt("linkQuality");
        }
        if (json_val.has("ssid"))
        {
            _ssid = json_val.getString("ssid");
        }
        if (json_val.has("channel"))
        {
            _channel = json_val.getInt("channel");
        }
        if (json_val.has("security"))
        {
            _security = json_val.getInt("security");
        }
        if (json_val.has("message"))
        {
            _message = json_val.getString("message");
        }
        if (json_val.has("wlanConfig"))
        {
            _wlanConfig = json_val.getString("wlanConfig");
        }
        if (json_val.has("wlanState"))
        {
            _wlanState = json_val.getInt("wlanState");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the link quality, expressed in percent.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the link quality, expressed in percent
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.LINKQUALITY_INVALID</c>.
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

    /**
     * <summary>
     *   Returns the wireless network name (SSID).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the wireless network name (SSID)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.SSID_INVALID</c>.
     * </para>
     */
    public string get_ssid()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SSID_INVALID;
                }
            }
            res = this._ssid;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the 802.11 channel currently used, or 0 when the selected network has not been found.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the 802.11 channel currently used, or 0 when the selected network has not been found
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.CHANNEL_INVALID</c>.
     * </para>
     */
    public int get_channel()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CHANNEL_INVALID;
                }
            }
            res = this._channel;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the security algorithm used by the selected wireless network.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YWireless.SECURITY_UNKNOWN</c>, <c>YWireless.SECURITY_OPEN</c>,
     *   <c>YWireless.SECURITY_WEP</c>, <c>YWireless.SECURITY_WPA</c> and <c>YWireless.SECURITY_WPA2</c>
     *   corresponding to the security algorithm used by the selected wireless network
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.SECURITY_INVALID</c>.
     * </para>
     */
    public int get_security()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SECURITY_INVALID;
                }
            }
            res = this._security;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the latest status message from the wireless interface.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the latest status message from the wireless interface
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.MESSAGE_INVALID</c>.
     * </para>
     */
    public string get_message()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MESSAGE_INVALID;
                }
            }
            res = this._message;
        }
        return res;
    }

    public string get_wlanConfig()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return WLANCONFIG_INVALID;
                }
            }
            res = this._wlanConfig;
        }
        return res;
    }

    public int set_wlanConfig(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("wlanConfig", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the current state of the wireless interface.
     * <para>
     *   The state <c>YWireless.WLANSTATE_DOWN</c> means that the network interface is
     *   not connected to a network. The state <c>YWireless.WLANSTATE_SCANNING</c> means that the network
     *   interface is scanning available
     *   frequencies. During this stage, the device is not reachable, and the network settings are not yet
     *   applied. The state
     *   <c>YWireless.WLANSTATE_CONNECTED</c> means that the network settings have been successfully applied
     *   ant that the device is reachable
     *   from the wireless network. If the device is configured to use ad-hoc or Soft AP mode, it means that
     *   the wireless network
     *   is up and that other devices can join the network. The state <c>YWireless.WLANSTATE_REJECTED</c>
     *   means that the network interface has
     *   not been able to join the requested network. The description of the error can be obtain with the
     *   <c>get_message()</c> method.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YWireless.WLANSTATE_DOWN</c>, <c>YWireless.WLANSTATE_SCANNING</c>,
     *   <c>YWireless.WLANSTATE_CONNECTED</c> and <c>YWireless.WLANSTATE_REJECTED</c> corresponding to the
     *   current state of the wireless interface
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.WLANSTATE_INVALID</c>.
     * </para>
     */
    public int get_wlanState()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return WLANSTATE_INVALID;
                }
            }
            res = this._wlanState;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves a wireless lan interface for a given identifier.
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
     *   This function does not require that the wireless lan interface is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YWireless.isOnline()</c> to test if the wireless lan interface is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a wireless lan interface by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the wireless lan interface, for instance
     *   <c>YHUBWLN1.wireless</c>.
     * </param>
     * <returns>
     *   a <c>YWireless</c> object allowing you to drive the wireless lan interface.
     * </returns>
     */
    public static YWireless FindWireless(string func)
    {
        YWireless obj;
        lock (YAPI.globalLock) {
            obj = (YWireless) YFunction._FindFromCache("Wireless", func);
            if (obj == null) {
                obj = new YWireless(func);
                YFunction._AddToCache("Wireless", func, obj);
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
        this._valueCallbackWireless = callback;
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
        if (this._valueCallbackWireless != null) {
            this._valueCallbackWireless(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Triggers a scan of the wireless frequency and builds the list of available networks.
     * <para>
     *   The scan forces a disconnection from the current network. At then end of the process, the
     *   the network interface attempts to reconnect to the previous network. During the scan, the <c>wlanState</c>
     *   switches to <c>YWireless.WLANSTATE_DOWN</c>, then to <c>YWireless.WLANSTATE_SCANNING</c>. When the
     *   scan is completed,
     *   <c>get_wlanState()</c> returns either <c>YWireless.WLANSTATE_DOWN</c> or
     *   <c>YWireless.WLANSTATE_SCANNING</c>. At this
     *   point, the list of detected network can be retrieved with the <c>get_detectedWlans()</c> method.
     * </para>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     * </summary>
     */
    public virtual int startWlanScan()
    {
        string config;
        config = this.get_wlanConfig();
        // a full scan is triggered when a config is applied
        return this.set_wlanConfig(config);
    }

    /**
     * <summary>
     *   Changes the configuration of the wireless lan interface to connect to an existing
     *   access point (infrastructure mode).
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * </summary>
     * <param name="ssid">
     *   the name of the network to connect to
     * </param>
     * <param name="securityKey">
     *   the network key, as a character string
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int joinNetwork(string ssid, string securityKey)
    {
        return this.set_wlanConfig("INFRA:"+ ssid+"\\"+securityKey);
    }

    /**
     * <summary>
     *   Changes the configuration of the wireless lan interface to create an ad-hoc
     *   wireless network, without using an access point.
     * <para>
     *   On the YoctoHub-Wireless-g
     *   and YoctoHub-Wireless-n,
     *   you should use <c>softAPNetwork()</c> instead, which emulates an access point
     *   (Soft AP) which is more efficient and more widely supported than ad-hoc networks.
     * </para>
     * <para>
     *   When a security key is specified for an ad-hoc network, the network is protected
     *   by a WEP40 key (5 characters or 10 hexadecimal digits) or WEP128 key (13 characters
     *   or 26 hexadecimal digits). It is recommended to use a well-randomized WEP128 key
     *   using 26 hexadecimal digits to maximize security.
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module
     *   to apply this setting.
     * </para>
     * </summary>
     * <param name="ssid">
     *   the name of the network to connect to
     * </param>
     * <param name="securityKey">
     *   the network key, as a character string
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int adhocNetwork(string ssid, string securityKey)
    {
        return this.set_wlanConfig("ADHOC:"+ ssid+"\\"+securityKey);
    }

    /**
     * <summary>
     *   Changes the configuration of the wireless lan interface to create a new wireless
     *   network by emulating a WiFi access point (Soft AP).
     * <para>
     *   This function can only be
     *   used with the YoctoHub-Wireless-g and the YoctoHub-Wireless-n.
     * </para>
     * <para>
     *   On the YoctoHub-Wireless-g, when a security key is specified for a SoftAP network,
     *   the network is protected by a WEP40 key (5 characters or 10 hexadecimal digits) or
     *   WEP128 key (13 characters or 26 hexadecimal digits). It is recommended to use a
     *   well-randomized WEP128 key using 26 hexadecimal digits to maximize security.
     * </para>
     * <para>
     *   On the YoctoHub-Wireless-n, when a security key is specified for a SoftAP network,
     *   the network will be protected by WPA2.
     * </para>
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * </summary>
     * <param name="ssid">
     *   the name of the network to connect to
     * </param>
     * <param name="securityKey">
     *   the network key, as a character string
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int softAPNetwork(string ssid, string securityKey)
    {
        return this.set_wlanConfig("SOFTAP:"+ ssid+"\\"+securityKey);
    }

    /**
     * <summary>
     *   Returns a list of YWlanRecord objects that describe detected Wireless networks.
     * <para>
     *   This list is not updated when the module is already connected to an access point (infrastructure mode).
     *   To force an update of this list, <c>startWlanScan()</c> must be called.
     *   Note that an languages without garbage collections, the returned list must be freed by the caller.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a list of <c>YWlanRecord</c> objects, containing the SSID, channel,
     *   link quality and the type of security of the wireless network.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list.
     * </para>
     */
    public virtual List<YWlanRecord> get_detectedWlans()
    {
        byte[] json;
        List<string> wlanlist = new List<string>();
        List<YWlanRecord> res = new List<YWlanRecord>();

        json = this._download("wlan.json?by=name");
        wlanlist = this._json_get_array(json);
        res.Clear();
        for (int ii = 0; ii < wlanlist.Count; ii++) {
            res.Add(new YWlanRecord(wlanlist[ii]));
        }
        return res;
    }

    /**
     * <summary>
     *   Continues the enumeration of wireless lan interfaces started using <c>yFirstWireless()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned wireless lan interfaces order.
     *   If you want to find a specific a wireless lan interface, use <c>Wireless.findWireless()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YWireless</c> object, corresponding to
     *   a wireless lan interface currently online, or a <c>null</c> pointer
     *   if there are no more wireless lan interfaces to enumerate.
     * </returns>
     */
    public YWireless nextWireless()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindWireless(hwid);
    }

    //--- (end of generated code: YWireless implementation)

  //--- (generated code: YWireless functions)

    /**
     * <summary>
     *   Starts the enumeration of wireless lan interfaces currently accessible.
     * <para>
     *   Use the method <c>YWireless.nextWireless()</c> to iterate on
     *   next wireless lan interfaces.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YWireless</c> object, corresponding to
     *   the first wireless lan interface currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YWireless FirstWireless()
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
        err = YAPI.apiGetFunctionsByClass("Wireless", 0, p, size, ref neededsize, ref errmsg);
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
        return FindWireless(serial + "." + funcId);
    }



    //--- (end of generated code: YWireless functions)
}
#pragma warning restore 1591
