/*********************************************************************
 *
 *  $Id: yocto_segmenteddisplay.cs 34989 2019-04-05 13:41:16Z seb $
 *
 *  Implements yFindSegmentedDisplay(), the high-level API for SegmentedDisplay functions
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
    //--- (YSegmentedDisplay return codes)
    //--- (end of YSegmentedDisplay return codes)
//--- (YSegmentedDisplay dlldef)
//--- (end of YSegmentedDisplay dlldef)
//--- (YSegmentedDisplay yapiwrapper)
//--- (end of YSegmentedDisplay yapiwrapper)
//--- (YSegmentedDisplay class start)
/**
 * <summary>
 *   The SegmentedDisplay class allows you to drive segmented displays.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YSegmentedDisplay : YFunction
{
//--- (end of YSegmentedDisplay class start)
    //--- (YSegmentedDisplay definitions)
    public new delegate void ValueCallback(YSegmentedDisplay func, string value);
    public new delegate void TimedReportCallback(YSegmentedDisplay func, YMeasure measure);

    public const string DISPLAYEDTEXT_INVALID = YAPI.INVALID_STRING;
    public const int DISPLAYMODE_DISCONNECTED = 0;
    public const int DISPLAYMODE_MANUAL = 1;
    public const int DISPLAYMODE_AUTO1 = 2;
    public const int DISPLAYMODE_AUTO60 = 3;
    public const int DISPLAYMODE_INVALID = -1;
    protected string _displayedText = DISPLAYEDTEXT_INVALID;
    protected int _displayMode = DISPLAYMODE_INVALID;
    protected ValueCallback _valueCallbackSegmentedDisplay = null;
    //--- (end of YSegmentedDisplay definitions)

    public YSegmentedDisplay(string func)
        : base(func)
    {
        _className = "SegmentedDisplay";
        //--- (YSegmentedDisplay attributes initialization)
        //--- (end of YSegmentedDisplay attributes initialization)
    }

    //--- (YSegmentedDisplay implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("displayedText"))
        {
            _displayedText = json_val.getString("displayedText");
        }
        if (json_val.has("displayMode"))
        {
            _displayMode = json_val.getInt("displayMode");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the text currently displayed on the screen.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the text currently displayed on the screen
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSegmentedDisplay.DISPLAYEDTEXT_INVALID</c>.
     * </para>
     */
    public string get_displayedText()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DISPLAYEDTEXT_INVALID;
                }
            }
            res = this._displayedText;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the text currently displayed on the screen.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the text currently displayed on the screen
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
    public int set_displayedText(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("displayedText", rest_val);
        }
    }

    public int get_displayMode()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DISPLAYMODE_INVALID;
                }
            }
            res = this._displayMode;
        }
        return res;
    }

    public int set_displayMode(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("displayMode", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a segmented display for a given identifier.
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
     *   This function does not require that the segmented displays is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YSegmentedDisplay.isOnline()</c> to test if the segmented displays is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a segmented display by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the segmented displays
     * </param>
     * <returns>
     *   a <c>YSegmentedDisplay</c> object allowing you to drive the segmented displays.
     * </returns>
     */
    public static YSegmentedDisplay FindSegmentedDisplay(string func)
    {
        YSegmentedDisplay obj;
        lock (YAPI.globalLock) {
            obj = (YSegmentedDisplay) YFunction._FindFromCache("SegmentedDisplay", func);
            if (obj == null) {
                obj = new YSegmentedDisplay(func);
                YFunction._AddToCache("SegmentedDisplay", func, obj);
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
        this._valueCallbackSegmentedDisplay = callback;
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
        if (this._valueCallbackSegmentedDisplay != null) {
            this._valueCallbackSegmentedDisplay(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of segmented displays started using <c>yFirstSegmentedDisplay()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned segmented displays order.
     *   If you want to find a specific a segmented display, use <c>SegmentedDisplay.findSegmentedDisplay()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSegmentedDisplay</c> object, corresponding to
     *   a segmented display currently online, or a <c>null</c> pointer
     *   if there are no more segmented displays to enumerate.
     * </returns>
     */
    public YSegmentedDisplay nextSegmentedDisplay()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindSegmentedDisplay(hwid);
    }

    //--- (end of YSegmentedDisplay implementation)

    //--- (YSegmentedDisplay functions)

    /**
     * <summary>
     *   Starts the enumeration of segmented displays currently accessible.
     * <para>
     *   Use the method <c>YSegmentedDisplay.nextSegmentedDisplay()</c> to iterate on
     *   next segmented displays.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSegmentedDisplay</c> object, corresponding to
     *   the first segmented displays currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YSegmentedDisplay FirstSegmentedDisplay()
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
        err = YAPI.apiGetFunctionsByClass("SegmentedDisplay", 0, p, size, ref neededsize, ref errmsg);
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
        return FindSegmentedDisplay(serial + "." + funcId);
    }



    //--- (end of YSegmentedDisplay functions)
}
#pragma warning restore 1591
