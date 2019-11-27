/*********************************************************************
 *
 *  $Id: yocto_wakeupschedule.cs 38510 2019-11-26 15:36:38Z mvuilleu $
 *
 *  Implements yFindWakeUpSchedule(), the high-level API for WakeUpSchedule functions
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
    //--- (YWakeUpSchedule return codes)
    //--- (end of YWakeUpSchedule return codes)
//--- (YWakeUpSchedule dlldef)
//--- (end of YWakeUpSchedule dlldef)
//--- (YWakeUpSchedule yapiwrapper)
//--- (end of YWakeUpSchedule yapiwrapper)
//--- (YWakeUpSchedule class start)
/**
 * <summary>
 *   The YWakeUpSchedule class implements a wake up condition, for instance using a YoctoHub-GSM-3G-EU, a YoctoHub-GSM-3G-NA, a YoctoHub-Wireless-SR or a YoctoHub-Wireless-g.
 * <para>
 *   The wake up time is
 *   specified as a set of months and/or days and/or hours and/or minutes when the
 *   wake up should happen.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YWakeUpSchedule : YFunction
{
//--- (end of YWakeUpSchedule class start)
    //--- (YWakeUpSchedule definitions)
    public new delegate void ValueCallback(YWakeUpSchedule func, string value);
    public new delegate void TimedReportCallback(YWakeUpSchedule func, YMeasure measure);

    public const int MINUTESA_INVALID = YAPI.INVALID_UINT;
    public const int MINUTESB_INVALID = YAPI.INVALID_UINT;
    public const int HOURS_INVALID = YAPI.INVALID_UINT;
    public const int WEEKDAYS_INVALID = YAPI.INVALID_UINT;
    public const int MONTHDAYS_INVALID = YAPI.INVALID_UINT;
    public const int MONTHS_INVALID = YAPI.INVALID_UINT;
    public const long NEXTOCCURENCE_INVALID = YAPI.INVALID_LONG;
    protected int _minutesA = MINUTESA_INVALID;
    protected int _minutesB = MINUTESB_INVALID;
    protected int _hours = HOURS_INVALID;
    protected int _weekDays = WEEKDAYS_INVALID;
    protected int _monthDays = MONTHDAYS_INVALID;
    protected int _months = MONTHS_INVALID;
    protected long _nextOccurence = NEXTOCCURENCE_INVALID;
    protected ValueCallback _valueCallbackWakeUpSchedule = null;
    //--- (end of YWakeUpSchedule definitions)

    public YWakeUpSchedule(string func)
        : base(func)
    {
        _className = "WakeUpSchedule";
        //--- (YWakeUpSchedule attributes initialization)
        //--- (end of YWakeUpSchedule attributes initialization)
    }

    //--- (YWakeUpSchedule implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("minutesA"))
        {
            _minutesA = json_val.getInt("minutesA");
        }
        if (json_val.has("minutesB"))
        {
            _minutesB = json_val.getInt("minutesB");
        }
        if (json_val.has("hours"))
        {
            _hours = json_val.getInt("hours");
        }
        if (json_val.has("weekDays"))
        {
            _weekDays = json_val.getInt("weekDays");
        }
        if (json_val.has("monthDays"))
        {
            _monthDays = json_val.getInt("monthDays");
        }
        if (json_val.has("months"))
        {
            _months = json_val.getInt("months");
        }
        if (json_val.has("nextOccurence"))
        {
            _nextOccurence = json_val.getLong("nextOccurence");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the minutes in the 00-29 interval of each hour scheduled for wake up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the minutes in the 00-29 interval of each hour scheduled for wake up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWakeUpSchedule.MINUTESA_INVALID</c>.
     * </para>
     */
    public int get_minutesA()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MINUTESA_INVALID;
                }
            }
            res = this._minutesA;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the minutes in the 00-29 interval when a wake up must take place.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the minutes in the 00-29 interval when a wake up must take place
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
    public int set_minutesA(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("minutesA", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the minutes in the 30-59 interval of each hour scheduled for wake up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the minutes in the 30-59 interval of each hour scheduled for wake up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWakeUpSchedule.MINUTESB_INVALID</c>.
     * </para>
     */
    public int get_minutesB()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MINUTESB_INVALID;
                }
            }
            res = this._minutesB;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the minutes in the 30-59 interval when a wake up must take place.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the minutes in the 30-59 interval when a wake up must take place
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
    public int set_minutesB(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("minutesB", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the hours scheduled for wake up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the hours scheduled for wake up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWakeUpSchedule.HOURS_INVALID</c>.
     * </para>
     */
    public int get_hours()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return HOURS_INVALID;
                }
            }
            res = this._hours;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the hours when a wake up must take place.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the hours when a wake up must take place
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
    public int set_hours(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("hours", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the days of the week scheduled for wake up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the days of the week scheduled for wake up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWakeUpSchedule.WEEKDAYS_INVALID</c>.
     * </para>
     */
    public int get_weekDays()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return WEEKDAYS_INVALID;
                }
            }
            res = this._weekDays;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the days of the week when a wake up must take place.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the days of the week when a wake up must take place
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
    public int set_weekDays(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("weekDays", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the days of the month scheduled for wake up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the days of the month scheduled for wake up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWakeUpSchedule.MONTHDAYS_INVALID</c>.
     * </para>
     */
    public int get_monthDays()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MONTHDAYS_INVALID;
                }
            }
            res = this._monthDays;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the days of the month when a wake up must take place.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the days of the month when a wake up must take place
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
    public int set_monthDays(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("monthDays", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the months scheduled for wake up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the months scheduled for wake up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWakeUpSchedule.MONTHS_INVALID</c>.
     * </para>
     */
    public int get_months()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MONTHS_INVALID;
                }
            }
            res = this._months;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the months when a wake up must take place.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the months when a wake up must take place
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
    public int set_months(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("months", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the date/time (seconds) of the next wake up occurrence.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the date/time (seconds) of the next wake up occurrence
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWakeUpSchedule.NEXTOCCURENCE_INVALID</c>.
     * </para>
     */
    public long get_nextOccurence()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return NEXTOCCURENCE_INVALID;
                }
            }
            res = this._nextOccurence;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves a wake up schedule for a given identifier.
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
     *   This function does not require that the wake up schedule is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YWakeUpSchedule.isOnline()</c> to test if the wake up schedule is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a wake up schedule by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the wake up schedule, for instance
     *   <c>YHUBGSM3.wakeUpSchedule1</c>.
     * </param>
     * <returns>
     *   a <c>YWakeUpSchedule</c> object allowing you to drive the wake up schedule.
     * </returns>
     */
    public static YWakeUpSchedule FindWakeUpSchedule(string func)
    {
        YWakeUpSchedule obj;
        lock (YAPI.globalLock) {
            obj = (YWakeUpSchedule) YFunction._FindFromCache("WakeUpSchedule", func);
            if (obj == null) {
                obj = new YWakeUpSchedule(func);
                YFunction._AddToCache("WakeUpSchedule", func, obj);
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
        this._valueCallbackWakeUpSchedule = callback;
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
        if (this._valueCallbackWakeUpSchedule != null) {
            this._valueCallbackWakeUpSchedule(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Returns all the minutes of each hour that are scheduled for wake up.
     * <para>
     * </para>
     * </summary>
     */
    public virtual long get_minutes()
    {
        long res;

        res = this.get_minutesB();
        res = ((res) << (30));
        res = res + this.get_minutesA();
        return res;
    }

    /**
     * <summary>
     *   Changes all the minutes where a wake up must take place.
     * <para>
     * </para>
     * </summary>
     * <param name="bitmap">
     *   Minutes 00-59 of each hour scheduled for wake up.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_minutes(long bitmap)
    {
        this.set_minutesA((int)(((bitmap) & (0x3fffffff))));
        bitmap = ((bitmap) >> (30));
        return this.set_minutesB((int)(((bitmap) & (0x3fffffff))));
    }

    /**
     * <summary>
     *   Continues the enumeration of wake up schedules started using <c>yFirstWakeUpSchedule()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned wake up schedules order.
     *   If you want to find a specific a wake up schedule, use <c>WakeUpSchedule.findWakeUpSchedule()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YWakeUpSchedule</c> object, corresponding to
     *   a wake up schedule currently online, or a <c>null</c> pointer
     *   if there are no more wake up schedules to enumerate.
     * </returns>
     */
    public YWakeUpSchedule nextWakeUpSchedule()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindWakeUpSchedule(hwid);
    }

    //--- (end of YWakeUpSchedule implementation)

    //--- (YWakeUpSchedule functions)

    /**
     * <summary>
     *   Starts the enumeration of wake up schedules currently accessible.
     * <para>
     *   Use the method <c>YWakeUpSchedule.nextWakeUpSchedule()</c> to iterate on
     *   next wake up schedules.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YWakeUpSchedule</c> object, corresponding to
     *   the first wake up schedule currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YWakeUpSchedule FirstWakeUpSchedule()
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
        err = YAPI.apiGetFunctionsByClass("WakeUpSchedule", 0, p, size, ref neededsize, ref errmsg);
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
        return FindWakeUpSchedule(serial + "." + funcId);
    }



    //--- (end of YWakeUpSchedule functions)
}
#pragma warning restore 1591
