/*********************************************************************
 *
 * $Id: yocto_gyro.cs 34989 2019-04-05 13:41:16Z seb $
 *
 * Implements yFindGyro(), the high-level API for Gyro functions
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

//--- (generated code: YQt return codes)
    //--- (end of generated code: YQt return codes)
//--- (generated code: YQt class start)
/**
 * <summary>
 *   The Yoctopuce API YQt class provides direct access to the Yocto3D attitude estimation
 *   using a quaternion.
 * <para>
 *   It is usually not needed to use the YQt class directly, as the
 *   YGyro class provides a more convenient higher-level interface.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YQt : YSensor
{
//--- (end of generated code: YQt class start)
    //--- (generated code: YQt definitions)
    public new delegate void ValueCallback(YQt func, string value);
    public new delegate void TimedReportCallback(YQt func, YMeasure measure);

    protected ValueCallback _valueCallbackQt = null;
    protected TimedReportCallback _timedReportCallbackQt = null;
    //--- (end of generated code: YQt definitions)

    public YQt(string func)
        : base(func)
    {
        _className = "Qt";
        //--- (generated code: YQt attributes initialization)
        //--- (end of generated code: YQt attributes initialization)
    }



    //--- (generated code: YQt implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Retrieves a quaternion component for a given identifier.
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
     *   This function does not require that the quaternion component is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YQt.isOnline()</c> to test if the quaternion component is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a quaternion component by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the quaternion component
     * </param>
     * <returns>
     *   a <c>YQt</c> object allowing you to drive the quaternion component.
     * </returns>
     */
    public static YQt FindQt(string func)
    {
        YQt obj;
        lock (YAPI.globalLock) {
            obj = (YQt) YFunction._FindFromCache("Qt", func);
            if (obj == null) {
                obj = new YQt(func);
                YFunction._AddToCache("Qt", func, obj);
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
        this._valueCallbackQt = callback;
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
        if (this._valueCallbackQt != null) {
            this._valueCallbackQt(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Registers the callback function that is invoked on every periodic timed notification.
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
     *   arguments: the function object of which the value has changed, and an YMeasure object describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerTimedReportCallback(TimedReportCallback callback)
    {
        YSensor sensor;
        sensor = this;
        if (callback != null) {
            YFunction._UpdateTimedReportCallbackList(sensor, true);
        } else {
            YFunction._UpdateTimedReportCallbackList(sensor, false);
        }
        this._timedReportCallbackQt = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackQt != null) {
            this._timedReportCallbackQt(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of quaternion components started using <c>yFirstQt()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned quaternion components order.
     *   If you want to find a specific a quaternion component, use <c>Qt.findQt()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YQt</c> object, corresponding to
     *   a quaternion component currently online, or a <c>null</c> pointer
     *   if there are no more quaternion components to enumerate.
     * </returns>
     */
    public YQt nextQt()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindQt(hwid);
    }

    //--- (end of generated code: YQt implementation)

    //--- (generated code: YQt functions)

    /**
     * <summary>
     *   Starts the enumeration of quaternion components currently accessible.
     * <para>
     *   Use the method <c>YQt.nextQt()</c> to iterate on
     *   next quaternion components.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YQt</c> object, corresponding to
     *   the first quaternion component currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YQt FirstQt()
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
        err = YAPI.apiGetFunctionsByClass("Qt", 0, p, size, ref neededsize, ref errmsg);
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
        return FindQt(serial + "." + funcId);
    }



    //--- (end of generated code: YQt functions)
}



//--- (generated code: YGyro return codes)
    //--- (end of generated code: YGyro return codes)
//--- (generated code: YGyro class start)
/**
 * <summary>
 *   The YSensor class is the parent class for all Yoctopuce sensors.
 * <para>
 *   It can be
 *   used to read the current value and unit of any sensor, read the min/max
 *   value, configure autonomous recording frequency and access recorded data.
 *   It also provide a function to register a callback invoked each time the
 *   observed value changes, or at a predefined interval. Using this class rather
 *   than a specific subclass makes it possible to create generic applications
 *   that work with any Yoctopuce sensor, even those that do not yet exist.
 *   Note: The YAnButton class is the only analog input which does not inherit
 *   from YSensor.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YGyro : YSensor
{
//--- (end of generated code: YGyro class start)
    public delegate void YQuatCallback(YGyro yGyro, double w, double x, double y, double z);
    public delegate void YAnglesCallback(YGyro yGyro, double roll, double pitch, double head);

    //--- (generated code: YGyro definitions)
    public new delegate void ValueCallback(YGyro func, string value);
    public new delegate void TimedReportCallback(YGyro func, YMeasure measure);

    public const int BANDWIDTH_INVALID = YAPI.INVALID_INT;
    public const double XVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const double YVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const double ZVALUE_INVALID = YAPI.INVALID_DOUBLE;
    protected int _bandwidth = BANDWIDTH_INVALID;
    protected double _xValue = XVALUE_INVALID;
    protected double _yValue = YVALUE_INVALID;
    protected double _zValue = ZVALUE_INVALID;
    protected ValueCallback _valueCallbackGyro = null;
    protected TimedReportCallback _timedReportCallbackGyro = null;
    protected int _qt_stamp = 0;
    protected YQt _qt_w;
    protected YQt _qt_x;
    protected YQt _qt_y;
    protected YQt _qt_z;
    protected double _w = 0;
    protected double _x = 0;
    protected double _y = 0;
    protected double _z = 0;
    protected int _angles_stamp = 0;
    protected double _head = 0;
    protected double _pitch = 0;
    protected double _roll = 0;
    protected YQuatCallback _quatCallback;
    protected YAnglesCallback _anglesCallback;
    //--- (end of generated code: YGyro definitions)


    public YGyro(string func)
        : base(func)
    {
        _className = "Gyro";
        //--- (generated code: YGyro attributes initialization)
        //--- (end of generated code: YGyro attributes initialization)
    }


    protected static void yInternalGyroCallback(YQt obj, String value)
    {
        YGyro gyro = (YGyro)obj.get_userData();
        if (gyro == null)
        {
            return;
        }
        string tmp = obj.get_functionId().Substring(2);
        int idx = Convert.ToInt32(tmp);
        double dbl_value = Convert.ToDouble(value);
        gyro._invokeGyroCallbacks(idx, dbl_value);
    }


    //--- (generated code: YGyro implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("bandwidth"))
        {
            _bandwidth = json_val.getInt("bandwidth");
        }
        if (json_val.has("xValue"))
        {
            _xValue = Math.Round(json_val.getDouble("xValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("yValue"))
        {
            _yValue = Math.Round(json_val.getDouble("yValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("zValue"))
        {
            _zValue = Math.Round(json_val.getDouble("zValue") * 1000.0 / 65536.0) / 1000.0;
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the measure update frequency, measured in Hz (Yocto-3D-V2 only).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the measure update frequency, measured in Hz (Yocto-3D-V2 only)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGyro.BANDWIDTH_INVALID</c>.
     * </para>
     */
    public int get_bandwidth()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BANDWIDTH_INVALID;
                }
            }
            res = this._bandwidth;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the measure update frequency, measured in Hz (Yocto-3D-V2 only).
     * <para>
     *   When the
     *   frequency is lower, the device performs averaging.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the measure update frequency, measured in Hz (Yocto-3D-V2 only)
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
    public int set_bandwidth(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("bandwidth", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the angular velocity around the X axis of the device, as a floating point number.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the angular velocity around the X axis of the device, as a
     *   floating point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGyro.XVALUE_INVALID</c>.
     * </para>
     */
    public double get_xValue()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return XVALUE_INVALID;
                }
            }
            res = this._xValue;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the angular velocity around the Y axis of the device, as a floating point number.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the angular velocity around the Y axis of the device, as a
     *   floating point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGyro.YVALUE_INVALID</c>.
     * </para>
     */
    public double get_yValue()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return YVALUE_INVALID;
                }
            }
            res = this._yValue;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the angular velocity around the Z axis of the device, as a floating point number.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the angular velocity around the Z axis of the device, as a
     *   floating point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGyro.ZVALUE_INVALID</c>.
     * </para>
     */
    public double get_zValue()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ZVALUE_INVALID;
                }
            }
            res = this._zValue;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves a gyroscope for a given identifier.
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
     *   This function does not require that the gyroscope is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YGyro.isOnline()</c> to test if the gyroscope is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a gyroscope by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the gyroscope
     * </param>
     * <returns>
     *   a <c>YGyro</c> object allowing you to drive the gyroscope.
     * </returns>
     */
    public static YGyro FindGyro(string func)
    {
        YGyro obj;
        lock (YAPI.globalLock) {
            obj = (YGyro) YFunction._FindFromCache("Gyro", func);
            if (obj == null) {
                obj = new YGyro(func);
                YFunction._AddToCache("Gyro", func, obj);
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
        this._valueCallbackGyro = callback;
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
        if (this._valueCallbackGyro != null) {
            this._valueCallbackGyro(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Registers the callback function that is invoked on every periodic timed notification.
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
     *   arguments: the function object of which the value has changed, and an YMeasure object describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerTimedReportCallback(TimedReportCallback callback)
    {
        YSensor sensor;
        sensor = this;
        if (callback != null) {
            YFunction._UpdateTimedReportCallbackList(sensor, true);
        } else {
            YFunction._UpdateTimedReportCallbackList(sensor, false);
        }
        this._timedReportCallbackGyro = callback;
        return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackGyro != null) {
            this._timedReportCallbackGyro(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    public virtual int _loadQuaternion()
    {
        int now_stamp;
        int age_ms;
        now_stamp = (int) ((YAPI.GetTickCount()) & (0x7FFFFFFF));
        age_ms = (((now_stamp - this._qt_stamp)) & (0x7FFFFFFF));
        if ((age_ms >= 10) || (this._qt_stamp == 0)) {
            if (this.load(10) != YAPI.SUCCESS) {
                return YAPI.DEVICE_NOT_FOUND;
            }
            if (this._qt_stamp == 0) {
                this._qt_w = YQt.FindQt(""+this._serial+".qt1");
                this._qt_x = YQt.FindQt(""+this._serial+".qt2");
                this._qt_y = YQt.FindQt(""+this._serial+".qt3");
                this._qt_z = YQt.FindQt(""+this._serial+".qt4");
            }
            if (this._qt_w.load(9) != YAPI.SUCCESS) {
                return YAPI.DEVICE_NOT_FOUND;
            }
            if (this._qt_x.load(9) != YAPI.SUCCESS) {
                return YAPI.DEVICE_NOT_FOUND;
            }
            if (this._qt_y.load(9) != YAPI.SUCCESS) {
                return YAPI.DEVICE_NOT_FOUND;
            }
            if (this._qt_z.load(9) != YAPI.SUCCESS) {
                return YAPI.DEVICE_NOT_FOUND;
            }
            this._w = this._qt_w.get_currentValue();
            this._x = this._qt_x.get_currentValue();
            this._y = this._qt_y.get_currentValue();
            this._z = this._qt_z.get_currentValue();
            this._qt_stamp = now_stamp;
        }
        return YAPI.SUCCESS;
    }

    public virtual int _loadAngles()
    {
        double sqw;
        double sqx;
        double sqy;
        double sqz;
        double norm;
        double delta;

        if (this._loadQuaternion() != YAPI.SUCCESS) {
            return YAPI.DEVICE_NOT_FOUND;
        }
        if (this._angles_stamp != this._qt_stamp) {
            sqw = this._w * this._w;
            sqx = this._x * this._x;
            sqy = this._y * this._y;
            sqz = this._z * this._z;
            norm = sqx + sqy + sqz + sqw;
            delta = this._y * this._w - this._x * this._z;
            if (delta > 0.499 * norm) {
                // singularity at north pole
                this._pitch = 90.0;
                this._head  = Math.Round(2.0 * 1800.0/Math.PI * Math.Atan2(this._x,-this._w)) / 10.0;
            } else {
                if (delta < -0.499 * norm) {
                    // singularity at south pole
                    this._pitch = -90.0;
                    this._head  = Math.Round(-2.0 * 1800.0/Math.PI * Math.Atan2(this._x,-this._w)) / 10.0;
                } else {
                    this._roll  = Math.Round(1800.0/Math.PI * Math.Atan2(2.0 * (this._w * this._x + this._y * this._z),sqw - sqx - sqy + sqz)) / 10.0;
                    this._pitch = Math.Round(1800.0/Math.PI * Math.Asin(2.0 * delta / norm)) / 10.0;
                    this._head  = Math.Round(1800.0/Math.PI * Math.Atan2(2.0 * (this._x * this._y + this._z * this._w),sqw + sqx - sqy - sqz)) / 10.0;
                }
            }
            this._angles_stamp = this._qt_stamp;
        }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Returns the estimated roll angle, based on the integration of
     *   gyroscopic measures combined with acceleration and
     *   magnetic field measurements.
     * <para>
     *   The axis corresponding to the roll angle can be mapped to any
     *   of the device X, Y or Z physical directions using methods of
     *   the class <c>YRefFrame</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to roll angle
     *   in degrees, between -180 and +180.
     * </returns>
     */
    public virtual double get_roll()
    {
        this._loadAngles();
        return this._roll;
    }

    /**
     * <summary>
     *   Returns the estimated pitch angle, based on the integration of
     *   gyroscopic measures combined with acceleration and
     *   magnetic field measurements.
     * <para>
     *   The axis corresponding to the pitch angle can be mapped to any
     *   of the device X, Y or Z physical directions using methods of
     *   the class <c>YRefFrame</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to pitch angle
     *   in degrees, between -90 and +90.
     * </returns>
     */
    public virtual double get_pitch()
    {
        this._loadAngles();
        return this._pitch;
    }

    /**
     * <summary>
     *   Returns the estimated heading angle, based on the integration of
     *   gyroscopic measures combined with acceleration and
     *   magnetic field measurements.
     * <para>
     *   The axis corresponding to the heading can be mapped to any
     *   of the device X, Y or Z physical directions using methods of
     *   the class <c>YRefFrame</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to heading
     *   in degrees, between 0 and 360.
     * </returns>
     */
    public virtual double get_heading()
    {
        this._loadAngles();
        return this._head;
    }

    /**
     * <summary>
     *   Returns the <c>w</c> component (real part) of the quaternion
     *   describing the device estimated orientation, based on the
     *   integration of gyroscopic measures combined with acceleration and
     *   magnetic field measurements.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the <c>w</c>
     *   component of the quaternion.
     * </returns>
     */
    public virtual double get_quaternionW()
    {
        this._loadQuaternion();
        return this._w;
    }

    /**
     * <summary>
     *   Returns the <c>x</c> component of the quaternion
     *   describing the device estimated orientation, based on the
     *   integration of gyroscopic measures combined with acceleration and
     *   magnetic field measurements.
     * <para>
     *   The <c>x</c> component is
     *   mostly correlated with rotations on the roll axis.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the <c>x</c>
     *   component of the quaternion.
     * </returns>
     */
    public virtual double get_quaternionX()
    {
        this._loadQuaternion();
        return this._x;
    }

    /**
     * <summary>
     *   Returns the <c>y</c> component of the quaternion
     *   describing the device estimated orientation, based on the
     *   integration of gyroscopic measures combined with acceleration and
     *   magnetic field measurements.
     * <para>
     *   The <c>y</c> component is
     *   mostly correlated with rotations on the pitch axis.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the <c>y</c>
     *   component of the quaternion.
     * </returns>
     */
    public virtual double get_quaternionY()
    {
        this._loadQuaternion();
        return this._y;
    }

    /**
     * <summary>
     *   Returns the <c>x</c> component of the quaternion
     *   describing the device estimated orientation, based on the
     *   integration of gyroscopic measures combined with acceleration and
     *   magnetic field measurements.
     * <para>
     *   The <c>x</c> component is
     *   mostly correlated with changes of heading.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the <c>z</c>
     *   component of the quaternion.
     * </returns>
     */
    public virtual double get_quaternionZ()
    {
        this._loadQuaternion();
        return this._z;
    }

    /**
     * <summary>
     *   Registers a callback function that will be invoked each time that the estimated
     *   device orientation has changed.
     * <para>
     *   The call frequency is typically around 95Hz during a move.
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered.
     *   For good responsiveness, remember to call one of these two functions periodically.
     *   To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to invoke, or a null pointer.
     *   The callback function should take five arguments:
     *   the YGyro object of the turning device, and the floating
     *   point values of the four components w, x, y and z
     *   (as floating-point numbers).
     * @noreturn
     * </param>
     */
    public virtual int registerQuaternionCallback(YQuatCallback callback)
    {
        this._quatCallback = callback;
        if (callback != null) {
            if (this._loadQuaternion() != YAPI.SUCCESS) {
                return YAPI.DEVICE_NOT_FOUND;
            }
            this._qt_w.set_userData(this);
            this._qt_x.set_userData(this);
            this._qt_y.set_userData(this);
            this._qt_z.set_userData(this);
            this._qt_w.registerValueCallback(yInternalGyroCallback);
            this._qt_x.registerValueCallback(yInternalGyroCallback);
            this._qt_y.registerValueCallback(yInternalGyroCallback);
            this._qt_z.registerValueCallback(yInternalGyroCallback);
        } else {
            if (!(this._anglesCallback != null)) {
                this._qt_w.registerValueCallback((YQt.ValueCallback) null);
                this._qt_x.registerValueCallback((YQt.ValueCallback) null);
                this._qt_y.registerValueCallback((YQt.ValueCallback) null);
                this._qt_z.registerValueCallback((YQt.ValueCallback) null);
            }
        }
        return 0;
    }

    /**
     * <summary>
     *   Registers a callback function that will be invoked each time that the estimated
     *   device orientation has changed.
     * <para>
     *   The call frequency is typically around 95Hz during a move.
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered.
     *   For good responsiveness, remember to call one of these two functions periodically.
     *   To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to invoke, or a null pointer.
     *   The callback function should take four arguments:
     *   the YGyro object of the turning device, and the floating
     *   point values of the three angles roll, pitch and heading
     *   in degrees (as floating-point numbers).
     * @noreturn
     * </param>
     */
    public virtual int registerAnglesCallback(YAnglesCallback callback)
    {
        this._anglesCallback = callback;
        if (callback != null) {
            if (this._loadQuaternion() != YAPI.SUCCESS) {
                return YAPI.DEVICE_NOT_FOUND;
            }
            this._qt_w.set_userData(this);
            this._qt_x.set_userData(this);
            this._qt_y.set_userData(this);
            this._qt_z.set_userData(this);
            this._qt_w.registerValueCallback(yInternalGyroCallback);
            this._qt_x.registerValueCallback(yInternalGyroCallback);
            this._qt_y.registerValueCallback(yInternalGyroCallback);
            this._qt_z.registerValueCallback(yInternalGyroCallback);
        } else {
            if (!(this._quatCallback != null)) {
                this._qt_w.registerValueCallback((YQt.ValueCallback) null);
                this._qt_x.registerValueCallback((YQt.ValueCallback) null);
                this._qt_y.registerValueCallback((YQt.ValueCallback) null);
                this._qt_z.registerValueCallback((YQt.ValueCallback) null);
            }
        }
        return 0;
    }

    public virtual int _invokeGyroCallbacks(int qtIndex, double qtValue)
    {
        switch(qtIndex - 1) {
        case 0:
            this._w = qtValue;
            break;
        case 1:
            this._x = qtValue;
            break;
        case 2:
            this._y = qtValue;
            break;
        case 3:
            this._z = qtValue;
            break;
        }
        if (qtIndex < 4) {
            return 0;
        }
        this._qt_stamp = (int) ((YAPI.GetTickCount()) & (0x7FFFFFFF));
        if (this._quatCallback != null) {
            this._quatCallback(this, this._w, this._x, this._y, this._z);
        }
        if (this._anglesCallback != null) {
            this._loadAngles();
            this._anglesCallback(this, this._roll, this._pitch, this._head);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of gyroscopes started using <c>yFirstGyro()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned gyroscopes order.
     *   If you want to find a specific a gyroscope, use <c>Gyro.findGyro()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YGyro</c> object, corresponding to
     *   a gyroscope currently online, or a <c>null</c> pointer
     *   if there are no more gyroscopes to enumerate.
     * </returns>
     */
    public YGyro nextGyro()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindGyro(hwid);
    }

    //--- (end of generated code: YGyro implementation)

    //--- (generated code: YGyro functions)

    /**
     * <summary>
     *   Starts the enumeration of gyroscopes currently accessible.
     * <para>
     *   Use the method <c>YGyro.nextGyro()</c> to iterate on
     *   next gyroscopes.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YGyro</c> object, corresponding to
     *   the first gyro currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YGyro FirstGyro()
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
        err = YAPI.apiGetFunctionsByClass("Gyro", 0, p, size, ref neededsize, ref errmsg);
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
        return FindGyro(serial + "." + funcId);
    }



    //--- (end of generated code: YGyro functions)
}
#pragma warning restore 1591