/*********************************************************************
 *
 * $Id: yocto_cellular.cs 37619 2019-10-11 11:52:42Z mvuilleu $
 *
 * Implements yFindCellular(), the high-level API for Cellular functions
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

    //--- (generated code: YCellRecord return codes)
    //--- (end of generated code: YCellRecord return codes)
    //--- (generated code: YCellular return codes)
    //--- (end of generated code: YCellular return codes)

//--- (generated code: YCellRecord dlldef)
//--- (end of generated code: YCellRecord dlldef)
//--- (generated code: YCellRecord class start)
public class YCellRecord
{
//--- (end of generated code: YCellRecord class start)
    //--- (generated code: YCellRecord definitions)

    protected string _oper;
    protected int _mcc = 0;
    protected int _mnc = 0;
    protected int _lac = 0;
    protected int _cid = 0;
    protected int _dbm = 0;
    protected int _tad = 0;
    //--- (end of generated code: YCellRecord definitions)

    public YCellRecord(int mcc,int mnc,int lac,int cellId,int dbm,int tad,string oper)
    {
        //--- (generated code: YCellRecord attributes initialization)
        //--- (end of generated code: YCellRecord attributes initialization)
        _oper = oper;
        _mcc = mcc;
        _mnc = mnc;
        _lac = lac;
        _cid = cellId;
        _dbm = dbm;
        _tad = tad;
    }

    //--- (generated code: YCellRecord implementation)


    public virtual string get_cellOperator()
    {
        return this._oper;
    }

    public virtual int get_mobileCountryCode()
    {
        return this._mcc;
    }

    public virtual int get_mobileNetworkCode()
    {
        return this._mnc;
    }

    public virtual int get_locationAreaCode()
    {
        return this._lac;
    }

    public virtual int get_cellId()
    {
        return this._cid;
    }

    public virtual int get_signalStrength()
    {
        return this._dbm;
    }

    public virtual int get_timingAdvance()
    {
        return this._tad;
    }

    //--- (end of generated code: YCellRecord implementation)

    //--- (generated code: YCellRecord functions)



    //--- (end of generated code: YCellRecord functions)
}

//--- (generated code: YCellular dlldef)
//--- (end of generated code: YCellular dlldef)
//--- (generated code: YCellular class start)
/**
 * <summary>
 *   YCellular functions provides control over cellular network parameters
 *   and status for devices that are GSM-enabled.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YCellular : YFunction
{
//--- (end of generated code: YCellular class start)
    //--- (generated code: YCellular definitions)
    public new delegate void ValueCallback(YCellular func, string value);
    public new delegate void TimedReportCallback(YCellular func, YMeasure measure);

    public const int LINKQUALITY_INVALID = YAPI.INVALID_UINT;
    public const string CELLOPERATOR_INVALID = YAPI.INVALID_STRING;
    public const string CELLIDENTIFIER_INVALID = YAPI.INVALID_STRING;
    public const int CELLTYPE_GPRS = 0;
    public const int CELLTYPE_EGPRS = 1;
    public const int CELLTYPE_WCDMA = 2;
    public const int CELLTYPE_HSDPA = 3;
    public const int CELLTYPE_NONE = 4;
    public const int CELLTYPE_CDMA = 5;
    public const int CELLTYPE_INVALID = -1;
    public const string IMSI_INVALID = YAPI.INVALID_STRING;
    public const string MESSAGE_INVALID = YAPI.INVALID_STRING;
    public const string PIN_INVALID = YAPI.INVALID_STRING;
    public const string LOCKEDOPERATOR_INVALID = YAPI.INVALID_STRING;
    public const int AIRPLANEMODE_OFF = 0;
    public const int AIRPLANEMODE_ON = 1;
    public const int AIRPLANEMODE_INVALID = -1;
    public const int ENABLEDATA_HOMENETWORK = 0;
    public const int ENABLEDATA_ROAMING = 1;
    public const int ENABLEDATA_NEVER = 2;
    public const int ENABLEDATA_NEUTRALITY = 3;
    public const int ENABLEDATA_INVALID = -1;
    public const string APN_INVALID = YAPI.INVALID_STRING;
    public const string APNSECRET_INVALID = YAPI.INVALID_STRING;
    public const int PINGINTERVAL_INVALID = YAPI.INVALID_UINT;
    public const int DATASENT_INVALID = YAPI.INVALID_UINT;
    public const int DATARECEIVED_INVALID = YAPI.INVALID_UINT;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _linkQuality = LINKQUALITY_INVALID;
    protected string _cellOperator = CELLOPERATOR_INVALID;
    protected string _cellIdentifier = CELLIDENTIFIER_INVALID;
    protected int _cellType = CELLTYPE_INVALID;
    protected string _imsi = IMSI_INVALID;
    protected string _message = MESSAGE_INVALID;
    protected string _pin = PIN_INVALID;
    protected string _lockedOperator = LOCKEDOPERATOR_INVALID;
    protected int _airplaneMode = AIRPLANEMODE_INVALID;
    protected int _enableData = ENABLEDATA_INVALID;
    protected string _apn = APN_INVALID;
    protected string _apnSecret = APNSECRET_INVALID;
    protected int _pingInterval = PINGINTERVAL_INVALID;
    protected int _dataSent = DATASENT_INVALID;
    protected int _dataReceived = DATARECEIVED_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackCellular = null;
    //--- (end of generated code: YCellular definitions)

    public YCellular(string func)
        : base(func)
    {
        _className = "Cellular";
        //--- (generated code: YCellular attributes initialization)
        //--- (end of generated code: YCellular attributes initialization)
    }

    //--- (generated code: YCellular implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("linkQuality"))
        {
            _linkQuality = json_val.getInt("linkQuality");
        }
        if (json_val.has("cellOperator"))
        {
            _cellOperator = json_val.getString("cellOperator");
        }
        if (json_val.has("cellIdentifier"))
        {
            _cellIdentifier = json_val.getString("cellIdentifier");
        }
        if (json_val.has("cellType"))
        {
            _cellType = json_val.getInt("cellType");
        }
        if (json_val.has("imsi"))
        {
            _imsi = json_val.getString("imsi");
        }
        if (json_val.has("message"))
        {
            _message = json_val.getString("message");
        }
        if (json_val.has("pin"))
        {
            _pin = json_val.getString("pin");
        }
        if (json_val.has("lockedOperator"))
        {
            _lockedOperator = json_val.getString("lockedOperator");
        }
        if (json_val.has("airplaneMode"))
        {
            _airplaneMode = json_val.getInt("airplaneMode") > 0 ? 1 : 0;
        }
        if (json_val.has("enableData"))
        {
            _enableData = json_val.getInt("enableData");
        }
        if (json_val.has("apn"))
        {
            _apn = json_val.getString("apn");
        }
        if (json_val.has("apnSecret"))
        {
            _apnSecret = json_val.getString("apnSecret");
        }
        if (json_val.has("pingInterval"))
        {
            _pingInterval = json_val.getInt("pingInterval");
        }
        if (json_val.has("dataSent"))
        {
            _dataSent = json_val.getInt("dataSent");
        }
        if (json_val.has("dataReceived"))
        {
            _dataReceived = json_val.getInt("dataReceived");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
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
     *   On failure, throws an exception or returns <c>YCellular.LINKQUALITY_INVALID</c>.
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
     *   Returns the name of the cell operator currently in use.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the name of the cell operator currently in use
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.CELLOPERATOR_INVALID</c>.
     * </para>
     */
    public string get_cellOperator()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CELLOPERATOR_INVALID;
                }
            }
            res = this._cellOperator;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the unique identifier of the cellular antenna in use: MCC, MNC, LAC and Cell ID.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the unique identifier of the cellular antenna in use: MCC, MNC, LAC and Cell ID
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.CELLIDENTIFIER_INVALID</c>.
     * </para>
     */
    public string get_cellIdentifier()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CELLIDENTIFIER_INVALID;
                }
            }
            res = this._cellIdentifier;
        }
        return res;
    }

    /**
     * <summary>
     *   Active cellular connection type.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YCellular.CELLTYPE_GPRS</c>, <c>YCellular.CELLTYPE_EGPRS</c>,
     *   <c>YCellular.CELLTYPE_WCDMA</c>, <c>YCellular.CELLTYPE_HSDPA</c>, <c>YCellular.CELLTYPE_NONE</c>
     *   and <c>YCellular.CELLTYPE_CDMA</c>
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.CELLTYPE_INVALID</c>.
     * </para>
     */
    public int get_cellType()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CELLTYPE_INVALID;
                }
            }
            res = this._cellType;
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
     *   On failure, throws an exception or returns <c>YCellular.IMSI_INVALID</c>.
     * </para>
     */
    public string get_imsi()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return IMSI_INVALID;
                }
            }
            res = this._imsi;
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
     *   On failure, throws an exception or returns <c>YCellular.MESSAGE_INVALID</c>.
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
     *   On failure, throws an exception or returns <c>YCellular.PIN_INVALID</c>.
     * </para>
     */
    public string get_pin()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PIN_INVALID;
                }
            }
            res = this._pin;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the PIN code used by the module to access the SIM card.
     * <para>
     *   This function does not change the code on the SIM card itself, but only changes
     *   the parameter used by the device to try to get access to it. If the SIM code
     *   does not work immediately on first try, it will be automatically forgotten
     *   and the message will be set to "Enter SIM PIN". The method should then be
     *   invoked again with right correct PIN code. After three failed attempts in a row,
     *   the message is changed to "Enter SIM PUK" and the SIM card PUK code must be
     *   provided using method <c>sendPUK</c>.
     * </para>
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module to save the
     *   new value in the device flash.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the PIN code used by the module to access the SIM card
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
    public int set_pin(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("pin", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the name of the only cell operator to use if automatic choice is disabled,
     *   or an empty string if the SIM card will automatically choose among available
     *   cell operators.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the name of the only cell operator to use if automatic choice is disabled,
     *   or an empty string if the SIM card will automatically choose among available
     *   cell operators
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.LOCKEDOPERATOR_INVALID</c>.
     * </para>
     */
    public string get_lockedOperator()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LOCKEDOPERATOR_INVALID;
                }
            }
            res = this._lockedOperator;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the name of the cell operator to be used.
     * <para>
     *   If the name is an empty
     *   string, the choice will be made automatically based on the SIM card. Otherwise,
     *   the selected operator is the only one that will be used.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the name of the cell operator to be used
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
    public int set_lockedOperator(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("lockedOperator", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns true if the airplane mode is active (radio turned off).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YCellular.AIRPLANEMODE_OFF</c> or <c>YCellular.AIRPLANEMODE_ON</c>, according to true if
     *   the airplane mode is active (radio turned off)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.AIRPLANEMODE_INVALID</c>.
     * </para>
     */
    public int get_airplaneMode()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return AIRPLANEMODE_INVALID;
                }
            }
            res = this._airplaneMode;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the activation state of airplane mode (radio turned off).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YCellular.AIRPLANEMODE_OFF</c> or <c>YCellular.AIRPLANEMODE_ON</c>, according to the
     *   activation state of airplane mode (radio turned off)
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
    public int set_airplaneMode(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("airplaneMode", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the condition for enabling IP data services (GPRS).
     * <para>
     *   When data services are disabled, SMS are the only mean of communication.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YCellular.ENABLEDATA_HOMENETWORK</c>, <c>YCellular.ENABLEDATA_ROAMING</c>,
     *   <c>YCellular.ENABLEDATA_NEVER</c> and <c>YCellular.ENABLEDATA_NEUTRALITY</c> corresponding to the
     *   condition for enabling IP data services (GPRS)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.ENABLEDATA_INVALID</c>.
     * </para>
     */
    public int get_enableData()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ENABLEDATA_INVALID;
                }
            }
            res = this._enableData;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the condition for enabling IP data services (GPRS).
     * <para>
     *   The service can be either fully deactivated, or limited to the SIM home network,
     *   or enabled for all partner networks (roaming). Caution: enabling data services
     *   on roaming networks may cause prohibitive communication costs !
     * </para>
     * <para>
     *   When data services are disabled, SMS are the only mean of communication.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YCellular.ENABLEDATA_HOMENETWORK</c>, <c>YCellular.ENABLEDATA_ROAMING</c>,
     *   <c>YCellular.ENABLEDATA_NEVER</c> and <c>YCellular.ENABLEDATA_NEUTRALITY</c> corresponding to the
     *   condition for enabling IP data services (GPRS)
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
    public int set_enableData(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("enableData", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the Access Point Name (APN) to be used, if needed.
     * <para>
     *   When left blank, the APN suggested by the cell operator will be used.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the Access Point Name (APN) to be used, if needed
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.APN_INVALID</c>.
     * </para>
     */
    public string get_apn()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return APN_INVALID;
                }
            }
            res = this._apn;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the Access Point Name (APN) to be used, if needed.
     * <para>
     *   When left blank, the APN suggested by the cell operator will be used.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
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
    public int set_apn(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("apn", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns an opaque string if APN authentication parameters have been configured
     *   in the device, or an empty string otherwise.
     * <para>
     *   To configure these parameters, use <c>set_apnAuth()</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to an opaque string if APN authentication parameters have been configured
     *   in the device, or an empty string otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.APNSECRET_INVALID</c>.
     * </para>
     */
    public string get_apnSecret()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return APNSECRET_INVALID;
                }
            }
            res = this._apnSecret;
        }
        return res;
    }

    public int set_apnSecret(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("apnSecret", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the automated connectivity check interval, in seconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the automated connectivity check interval, in seconds
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.PINGINTERVAL_INVALID</c>.
     * </para>
     */
    public int get_pingInterval()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PINGINTERVAL_INVALID;
                }
            }
            res = this._pingInterval;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the automated connectivity check interval, in seconds.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the automated connectivity check interval, in seconds
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
    public int set_pingInterval(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("pingInterval", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the number of bytes sent so far.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of bytes sent so far
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.DATASENT_INVALID</c>.
     * </para>
     */
    public int get_dataSent()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DATASENT_INVALID;
                }
            }
            res = this._dataSent;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the value of the outgoing data counter.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the value of the outgoing data counter
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
    public int set_dataSent(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("dataSent", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the number of bytes received so far.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of bytes received so far
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCellular.DATARECEIVED_INVALID</c>.
     * </para>
     */
    public int get_dataReceived()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DATARECEIVED_INVALID;
                }
            }
            res = this._dataReceived;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the value of the incoming data counter.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the value of the incoming data counter
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
    public int set_dataReceived(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("dataReceived", rest_val);
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
     *   Use the method <c>YCellular.isOnline()</c> to test if the cellular interface is
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
     *   a <c>YCellular</c> object allowing you to drive the cellular interface.
     * </returns>
     */
    public static YCellular FindCellular(string func)
    {
        YCellular obj;
        lock (YAPI.globalLock) {
            obj = (YCellular) YFunction._FindFromCache("Cellular", func);
            if (obj == null) {
                obj = new YCellular(func);
                YFunction._AddToCache("Cellular", func, obj);
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
        this._valueCallbackCellular = callback;
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
        if (this._valueCallbackCellular != null) {
            this._valueCallbackCellular(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Sends a PUK code to unlock the SIM card after three failed PIN code attempts, and
     *   setup a new PIN into the SIM card.
     * <para>
     *   Only ten consecutive tentatives are permitted:
     *   after that, the SIM card will be blocked permanently without any mean of recovery
     *   to use it again. Note that after calling this method, you have usually to invoke
     *   method <c>set_pin()</c> to tell the YoctoHub which PIN to use in the future.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="puk">
     *   the SIM PUK code
     * </param>
     * <param name="newPin">
     *   new PIN code to configure into the SIM card
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int sendPUK(string puk, string newPin)
    {
        string gsmMsg;
        gsmMsg = this.get_message();
        if (!((gsmMsg).Substring(0, 13) == "Enter SIM PUK")) { this._throw(YAPI.INVALID_ARGUMENT, "PUK not expected at this time"); return YAPI.INVALID_ARGUMENT; }
        if (newPin == "") {
            return this.set_command("AT+CPIN="+puk+",0000;+CLCK=SC,0,0000");
        }
        return this.set_command("AT+CPIN="+puk+","+newPin);
    }

    /**
     * <summary>
     *   Configure authentication parameters to connect to the APN.
     * <para>
     *   Both
     *   PAP and CHAP authentication are supported.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="username">
     *   APN username
     * </param>
     * <param name="password">
     *   APN password
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_apnAuth(string username, string password)
    {
        return this.set_apnSecret(""+username+","+password);
    }

    /**
     * <summary>
     *   Clear the transmitted data counters.
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
    public virtual int clearDataCounters()
    {
        int retcode;

        retcode = this.set_dataReceived(0);
        if (retcode != YAPI.SUCCESS) {
            return retcode;
        }
        retcode = this.set_dataSent(0);
        return retcode;
    }

    /**
     * <summary>
     *   Sends an AT command to the GSM module and returns the command output.
     * <para>
     *   The command will only execute when the GSM module is in standard
     *   command state, and should leave it in the exact same state.
     *   Use this function with great care !
     * </para>
     * </summary>
     * <param name="cmd">
     *   the AT command to execute, like for instance: "+CCLK?".
     * </param>
     * <para>
     * </para>
     * <returns>
     *   a string with the result of the commands. Empty lines are
     *   automatically removed from the output.
     * </returns>
     */
    public virtual string _AT(string cmd)
    {
        int chrPos;
        int cmdLen;
        int waitMore;
        string res;
        byte[] buff;
        int bufflen;
        string buffstr;
        int buffstrlen;
        int idx;
        int suffixlen;
        // quote dangerous characters used in AT commands
        cmdLen = (cmd).Length;
        chrPos = (cmd).IndexOf("#");
        while (chrPos >= 0) {
            cmd = ""+ (cmd).Substring( 0, chrPos)+""+((char)( 37)).ToString()+"23"+(cmd).Substring( chrPos+1, cmdLen-chrPos-1);
            cmdLen = cmdLen + 2;
            chrPos = (cmd).IndexOf("#");
        }
        chrPos = (cmd).IndexOf("+");
        while (chrPos >= 0) {
            cmd = ""+ (cmd).Substring( 0, chrPos)+""+((char)( 37)).ToString()+"2B"+(cmd).Substring( chrPos+1, cmdLen-chrPos-1);
            cmdLen = cmdLen + 2;
            chrPos = (cmd).IndexOf("+");
        }
        chrPos = (cmd).IndexOf("=");
        while (chrPos >= 0) {
            cmd = ""+ (cmd).Substring( 0, chrPos)+""+((char)( 37)).ToString()+"3D"+(cmd).Substring( chrPos+1, cmdLen-chrPos-1);
            cmdLen = cmdLen + 2;
            chrPos = (cmd).IndexOf("=");
        }
        cmd = "at.txt?cmd="+cmd;
        res = "";
        // max 2 minutes (each iteration may take up to 5 seconds if waiting)
        waitMore = 24;
        while (waitMore > 0) {
            buff = this._download(cmd);
            bufflen = (buff).Length;
            buffstr = YAPI.DefaultEncoding.GetString(buff);
            buffstrlen = (buffstr).Length;
            idx = bufflen - 1;
            while ((idx > 0) && (buff[idx] != 64) && (buff[idx] != 10) && (buff[idx] != 13)) {
                idx = idx - 1;
            }
            if (buff[idx] == 64) {
                // continuation detected
                suffixlen = bufflen - idx;
                cmd = "at.txt?cmd="+(buffstr).Substring( buffstrlen - suffixlen, suffixlen);
                buffstr = (buffstr).Substring( 0, buffstrlen - suffixlen);
                waitMore = waitMore - 1;
            } else {
                // request complete
                waitMore = 0;
            }
            res = ""+ res+""+buffstr;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the list detected cell operators in the neighborhood.
     * <para>
     *   This function will typically take between 30 seconds to 1 minute to
     *   return. Note that any SIM card can usually only connect to specific
     *   operators. All networks returned by this function might therefore
     *   not be available for connection.
     * </para>
     * </summary>
     * <returns>
     *   a list of string (cell operator names).
     * </returns>
     */
    public virtual List<string> get_availableOperators()
    {
        string cops;
        int idx;
        int slen;
        List<string> res = new List<string>();

        cops = this._AT("+COPS=?");
        slen = (cops).Length;
        res.Clear();
        idx = (cops).IndexOf("(");
        while (idx >= 0) {
            slen = slen - (idx+1);
            cops = (cops).Substring( idx+1, slen);
            idx = (cops).IndexOf("\"");
            if (idx > 0) {
                slen = slen - (idx+1);
                cops = (cops).Substring( idx+1, slen);
                idx = (cops).IndexOf("\"");
                if (idx > 0) {
                    res.Add((cops).Substring( 0, idx));
                }
            }
            idx = (cops).IndexOf("(");
        }
        return res;
    }

    /**
     * <summary>
     *   Returns a list of nearby cellular antennas, as required for quick
     *   geolocation of the device.
     * <para>
     *   The first cell listed is the serving
     *   cell, and the next ones are the neighbor cells reported by the
     *   serving cell.
     * </para>
     * </summary>
     * <returns>
     *   a list of YCellRecords.
     * </returns>
     */
    public virtual List<YCellRecord> quickCellSurvey()
    {
        string moni;
        List<string> recs = new List<string>();
        int llen;
        string mccs;
        int mcc;
        string mncs;
        int mnc;
        int lac;
        int cellId;
        string dbms;
        int dbm;
        string tads;
        int tad;
        string oper;
        List<YCellRecord> res = new List<YCellRecord>();

        moni = this._AT("+CCED=0;#MONI=7;#MONI");
        mccs = (moni).Substring(7, 3);
        if ((mccs).Substring(0, 1) == "0") {
            mccs = (mccs).Substring(1, 2);
        }
        if ((mccs).Substring(0, 1) == "0") {
            mccs = (mccs).Substring(1, 1);
        }
        mcc = YAPI._atoi(mccs);
        mncs = (moni).Substring(11, 3);
        if ((mncs).Substring(2, 1) == ",") {
            mncs = (mncs).Substring(0, 2);
        }
        if ((mncs).Substring(0, 1) == "0") {
            mncs = (mncs).Substring(1, (mncs).Length-1);
        }
        mnc = YAPI._atoi(mncs);
        recs = new List<string>(moni.Split(new Char[] {'#'}));
        // process each line in turn
        res.Clear();
        for (int ii = 0; ii < recs.Count; ii++) {
            llen = (recs[ii]).Length - 2;
            if (llen >= 44) {
                if ((recs[ii]).Substring(41, 3) == "dbm") {
                    lac = Convert.ToInt32((recs[ii]).Substring(16, 4), 16);
                    cellId = Convert.ToInt32((recs[ii]).Substring(23, 4), 16);
                    dbms = (recs[ii]).Substring(37, 4);
                    if ((dbms).Substring(0, 1) == " ") {
                        dbms = (dbms).Substring(1, 3);
                    }
                    dbm = YAPI._atoi(dbms);
                    if (llen > 66) {
                        tads = (recs[ii]).Substring(54, 2);
                        if ((tads).Substring(0, 1) == " ") {
                            tads = (tads).Substring(1, 3);
                        }
                        tad = YAPI._atoi(tads);
                        oper = (recs[ii]).Substring(66, llen-66);
                    } else {
                        tad = -1;
                        oper = "";
                    }
                    if (lac < 65535) {
                        res.Add(new YCellRecord(mcc, mnc, lac, cellId, dbm, tad, oper));
                    }
                }
            }
        }
        return res;
    }

    /**
     * <summary>
     *   Continues the enumeration of cellular interfaces started using <c>yFirstCellular()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned cellular interfaces order.
     *   If you want to find a specific a cellular interface, use <c>Cellular.findCellular()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCellular</c> object, corresponding to
     *   a cellular interface currently online, or a <c>null</c> pointer
     *   if there are no more cellular interfaces to enumerate.
     * </returns>
     */
    public YCellular nextCellular()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindCellular(hwid);
    }

    //--- (end of generated code: YCellular implementation)

    //--- (generated code: YCellular functions)

    /**
     * <summary>
     *   Starts the enumeration of cellular interfaces currently accessible.
     * <para>
     *   Use the method <c>YCellular.nextCellular()</c> to iterate on
     *   next cellular interfaces.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCellular</c> object, corresponding to
     *   the first cellular interface currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YCellular FirstCellular()
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
        err = YAPI.apiGetFunctionsByClass("Cellular", 0, p, size, ref neededsize, ref errmsg);
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
        return FindCellular(serial + "." + funcId);
    }



    //--- (end of generated code: YCellular functions)
}
#pragma warning restore 1591
