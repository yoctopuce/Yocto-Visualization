/*********************************************************************
 *
 *  $Id: yocto_realtimeclock.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindRealTimeClock(), the high-level API for RealTimeClock functions
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
    //--- (YRealTimeClock return codes)
    //--- (end of YRealTimeClock return codes)
//--- (YRealTimeClock dlldef)
//--- (end of YRealTimeClock dlldef)
//--- (YRealTimeClock yapiwrapper)
//--- (end of YRealTimeClock yapiwrapper)
//--- (YRealTimeClock class start)
/**
 * <summary>
 *   The RealTimeClock function maintains and provides current date and time, even accross power cut
 *   lasting several days.
 * <para>
 *   It is the base for automated wake-up functions provided by the WakeUpScheduler.
 *   The current time may represent a local time as well as an UTC time, but no automatic time change
 *   will occur to account for daylight saving time.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YRealTimeClock : YFunction
{
//--- (end of YRealTimeClock class start)
    //--- (YRealTimeClock definitions)
    public new delegate void ValueCallback(YRealTimeClock func, string value);
    public new delegate void TimedReportCallback(YRealTimeClock func, YMeasure measure);

    public const long UNIXTIME_INVALID = YAPI.INVALID_LONG;
    public const string DATETIME_INVALID = YAPI.INVALID_STRING;
    public const int UTCOFFSET_INVALID = YAPI.INVALID_INT;
    public const int TIMESET_FALSE = 0;
    public const int TIMESET_TRUE = 1;
    public const int TIMESET_INVALID = -1;
    protected long _unixTime = UNIXTIME_INVALID;
    protected string _dateTime = DATETIME_INVALID;
    protected int _utcOffset = UTCOFFSET_INVALID;
    protected int _timeSet = TIMESET_INVALID;
    protected ValueCallback _valueCallbackRealTimeClock = null;
    //--- (end of YRealTimeClock definitions)

    public YRealTimeClock(string func)
        : base(func)
    {
        _className = "RealTimeClock";
        //--- (YRealTimeClock attributes initialization)
        //--- (end of YRealTimeClock attributes initialization)
    }

    //--- (YRealTimeClock implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("unixTime"))
        {
            _unixTime = json_val.getLong("unixTime");
        }
        if (json_val.has("dateTime"))
        {
            _dateTime = json_val.getString("dateTime");
        }
        if (json_val.has("utcOffset"))
        {
            _utcOffset = json_val.getInt("utcOffset");
        }
        if (json_val.has("timeSet"))
        {
            _timeSet = json_val.getInt("timeSet") > 0 ? 1 : 0;
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the current time in Unix format (number of elapsed seconds since Jan 1st, 1970).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current time in Unix format (number of elapsed seconds since Jan 1st, 1970)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRealTimeClock.UNIXTIME_INVALID</c>.
     * </para>
     */
    public long get_unixTime()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return UNIXTIME_INVALID;
                }
            }
            res = this._unixTime;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the current time.
     * <para>
     *   Time is specifid in Unix format (number of elapsed seconds since Jan 1st, 1970).
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the current time
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
    public int set_unixTime(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("unixTime", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the current time in the form "YYYY/MM/DD hh:mm:ss".
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the current time in the form "YYYY/MM/DD hh:mm:ss"
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRealTimeClock.DATETIME_INVALID</c>.
     * </para>
     */
    public string get_dateTime()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DATETIME_INVALID;
                }
            }
            res = this._dateTime;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the number of seconds between current time and UTC time (time zone).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of seconds between current time and UTC time (time zone)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRealTimeClock.UTCOFFSET_INVALID</c>.
     * </para>
     */
    public int get_utcOffset()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return UTCOFFSET_INVALID;
                }
            }
            res = this._utcOffset;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the number of seconds between current time and UTC time (time zone).
     * <para>
     *   The timezone is automatically rounded to the nearest multiple of 15 minutes.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the number of seconds between current time and UTC time (time zone)
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
    public int set_utcOffset(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("utcOffset", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns true if the clock has been set, and false otherwise.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YRealTimeClock.TIMESET_FALSE</c> or <c>YRealTimeClock.TIMESET_TRUE</c>, according to true
     *   if the clock has been set, and false otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRealTimeClock.TIMESET_INVALID</c>.
     * </para>
     */
    public int get_timeSet()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return TIMESET_INVALID;
                }
            }
            res = this._timeSet;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves a clock for a given identifier.
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
     *   This function does not require that the clock is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YRealTimeClock.isOnline()</c> to test if the clock is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a clock by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the clock
     * </param>
     * <returns>
     *   a <c>YRealTimeClock</c> object allowing you to drive the clock.
     * </returns>
     */
    public static YRealTimeClock FindRealTimeClock(string func)
    {
        YRealTimeClock obj;
        lock (YAPI.globalLock) {
            obj = (YRealTimeClock) YFunction._FindFromCache("RealTimeClock", func);
            if (obj == null) {
                obj = new YRealTimeClock(func);
                YFunction._AddToCache("RealTimeClock", func, obj);
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
        this._valueCallbackRealTimeClock = callback;
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
        if (this._valueCallbackRealTimeClock != null) {
            this._valueCallbackRealTimeClock(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of clocks started using <c>yFirstRealTimeClock()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned clocks order.
     *   If you want to find a specific a clock, use <c>RealTimeClock.findRealTimeClock()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRealTimeClock</c> object, corresponding to
     *   a clock currently online, or a <c>null</c> pointer
     *   if there are no more clocks to enumerate.
     * </returns>
     */
    public YRealTimeClock nextRealTimeClock()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindRealTimeClock(hwid);
    }

    //--- (end of YRealTimeClock implementation)

    //--- (YRealTimeClock functions)

    /**
     * <summary>
     *   Starts the enumeration of clocks currently accessible.
     * <para>
     *   Use the method <c>YRealTimeClock.nextRealTimeClock()</c> to iterate on
     *   next clocks.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRealTimeClock</c> object, corresponding to
     *   the first clock currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YRealTimeClock FirstRealTimeClock()
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
        err = YAPI.apiGetFunctionsByClass("RealTimeClock", 0, p, size, ref neededsize, ref errmsg);
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
        return FindRealTimeClock(serial + "." + funcId);
    }



    //--- (end of YRealTimeClock functions)
}
#pragma warning restore 1591
