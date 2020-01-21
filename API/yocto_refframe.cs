/*********************************************************************
 *
 *  $Id: yocto_refframe.cs 38899 2019-12-20 17:21:03Z mvuilleu $
 *
 *  Implements yFindRefFrame(), the high-level API for RefFrame functions
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
    //--- (YRefFrame return codes)
    //--- (end of YRefFrame return codes)
//--- (YRefFrame dlldef)
//--- (end of YRefFrame dlldef)
//--- (YRefFrame yapiwrapper)
//--- (end of YRefFrame yapiwrapper)
//--- (YRefFrame class start)
/**
 * <summary>
 *   The <c>YRefFrame</c> class is used to setup the base orientation of the Yoctopuce inertial
 *   sensors.
 * <para>
 *   Thanks to this, orientation functions relative to the earth surface plane
 *   can use the proper reference frame. The class also implements a tridimensional
 *   sensor calibration process, which can compensate for local variations
 *   of standard gravity and improve the precision of the tilt sensors.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YRefFrame : YFunction
{
//--- (end of YRefFrame class start)
    //--- (YRefFrame definitions)
    public new delegate void ValueCallback(YRefFrame func, string value);
    public new delegate void TimedReportCallback(YRefFrame func, YMeasure measure);

public enum   MOUNTPOSITION
    {   BOTTOM = 0,
        TOP = 1,
        FRONT = 2,
        REAR = 3,
        RIGHT = 4,
        LEFT = 5,
        INVALID = 6
     };
public enum   MOUNTORIENTATION
    {   TWELVE = 0,
        THREE = 1,
        SIX = 2,
        NINE = 3,
        INVALID = 4
     };
    public const int MOUNTPOS_INVALID = YAPI.INVALID_UINT;
    public const double BEARING_INVALID = YAPI.INVALID_DOUBLE;
    public const string CALIBRATIONPARAM_INVALID = YAPI.INVALID_STRING;
    public const int FUSIONMODE_NDOF = 0;
    public const int FUSIONMODE_NDOF_FMC_OFF = 1;
    public const int FUSIONMODE_M4G = 2;
    public const int FUSIONMODE_COMPASS = 3;
    public const int FUSIONMODE_IMU = 4;
    public const int FUSIONMODE_INVALID = -1;
    protected int _mountPos = MOUNTPOS_INVALID;
    protected double _bearing = BEARING_INVALID;
    protected string _calibrationParam = CALIBRATIONPARAM_INVALID;
    protected int _fusionMode = FUSIONMODE_INVALID;
    protected ValueCallback _valueCallbackRefFrame = null;
    protected bool _calibV2;
    protected int _calibStage = 0;
    protected string _calibStageHint;
    protected int _calibStageProgress = 0;
    protected int _calibProgress = 0;
    protected string _calibLogMsg;
    protected string _calibSavedParams;
    protected int _calibCount = 0;
    protected int _calibInternalPos = 0;
    protected int _calibPrevTick = 0;
    protected List<int> _calibOrient = new List<int>();
    protected List<double> _calibDataAccX = new List<double>();
    protected List<double> _calibDataAccY = new List<double>();
    protected List<double> _calibDataAccZ = new List<double>();
    protected List<double> _calibDataAcc = new List<double>();
    protected double _calibAccXOfs = 0;
    protected double _calibAccYOfs = 0;
    protected double _calibAccZOfs = 0;
    protected double _calibAccXScale = 0;
    protected double _calibAccYScale = 0;
    protected double _calibAccZScale = 0;
    //--- (end of YRefFrame definitions)

    public YRefFrame(string func)
        : base(func)
    {
        _className = "RefFrame";
        //--- (YRefFrame attributes initialization)
        //--- (end of YRefFrame attributes initialization)
    }

    //--- (YRefFrame implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("mountPos"))
        {
            _mountPos = json_val.getInt("mountPos");
        }
        if (json_val.has("bearing"))
        {
            _bearing = Math.Round(json_val.getDouble("bearing") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("calibrationParam"))
        {
            _calibrationParam = json_val.getString("calibrationParam");
        }
        if (json_val.has("fusionMode"))
        {
            _fusionMode = json_val.getInt("fusionMode");
        }
        base._parseAttr(json_val);
    }


    public int get_mountPos()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MOUNTPOS_INVALID;
                }
            }
            res = this._mountPos;
        }
        return res;
    }

    public int set_mountPos(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("mountPos", rest_val);
        }
    }

    /**
     * <summary>
     *   Changes the reference bearing used by the compass.
     * <para>
     *   The relative bearing
     *   indicated by the compass is the difference between the measured magnetic
     *   heading and the reference bearing indicated here.
     * </para>
     * <para>
     *   For instance, if you setup as reference bearing the value of the earth
     *   magnetic declination, the compass will provide the orientation relative
     *   to the geographic North.
     * </para>
     * <para>
     *   Similarly, when the sensor is not mounted along the standard directions
     *   because it has an additional yaw angle, you can set this angle in the reference
     *   bearing so that the compass provides the expected natural direction.
     * </para>
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the reference bearing used by the compass
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
    public int set_bearing(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("bearing", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the reference bearing used by the compass.
     * <para>
     *   The relative bearing
     *   indicated by the compass is the difference between the measured magnetic
     *   heading and the reference bearing indicated here.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the reference bearing used by the compass
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRefFrame.BEARING_INVALID</c>.
     * </para>
     */
    public double get_bearing()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BEARING_INVALID;
                }
            }
            res = this._bearing;
        }
        return res;
    }


    public string get_calibrationParam()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CALIBRATIONPARAM_INVALID;
                }
            }
            res = this._calibrationParam;
        }
        return res;
    }

    public int set_calibrationParam(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("calibrationParam", rest_val);
        }
    }


    /**
     * <summary>
     *   Returns the BNO055 fusion mode.
     * <para>
     *   Note this feature is only availabe on Yocto-3D-V2.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YRefFrame.FUSIONMODE_NDOF</c>, <c>YRefFrame.FUSIONMODE_NDOF_FMC_OFF</c>,
     *   <c>YRefFrame.FUSIONMODE_M4G</c>, <c>YRefFrame.FUSIONMODE_COMPASS</c> and
     *   <c>YRefFrame.FUSIONMODE_IMU</c> corresponding to the BNO055 fusion mode
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRefFrame.FUSIONMODE_INVALID</c>.
     * </para>
     */
    public int get_fusionMode()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return FUSIONMODE_INVALID;
                }
            }
            res = this._fusionMode;
        }
        return res;
    }

    /**
     * <summary>
     *   Change the BNO055 fusion mode.
     * <para>
     *   Note: this feature is only availabe on Yocto-3D-V2.
     *   Remember to call the matching module <c>saveToFlash()</c> method to save the setting permanently.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YRefFrame.FUSIONMODE_NDOF</c>, <c>YRefFrame.FUSIONMODE_NDOF_FMC_OFF</c>,
     *   <c>YRefFrame.FUSIONMODE_M4G</c>, <c>YRefFrame.FUSIONMODE_COMPASS</c> and <c>YRefFrame.FUSIONMODE_IMU</c>
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
    public int set_fusionMode(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("fusionMode", rest_val);
        }
    }


    /**
     * <summary>
     *   Retrieves a reference frame for a given identifier.
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
     *   This function does not require that the reference frame is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YRefFrame.isOnline()</c> to test if the reference frame is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a reference frame by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the reference frame, for instance
     *   <c>Y3DMK002.refFrame</c>.
     * </param>
     * <returns>
     *   a <c>YRefFrame</c> object allowing you to drive the reference frame.
     * </returns>
     */
    public static YRefFrame FindRefFrame(string func)
    {
        YRefFrame obj;
        lock (YAPI.globalLock) {
            obj = (YRefFrame) YFunction._FindFromCache("RefFrame", func);
            if (obj == null) {
                obj = new YRefFrame(func);
                YFunction._AddToCache("RefFrame", func, obj);
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
        this._valueCallbackRefFrame = callback;
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
        if (this._valueCallbackRefFrame != null) {
            this._valueCallbackRefFrame(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Returns the installation position of the device, as configured
     *   in order to define the reference frame for the compass and the
     *   pitch/roll tilt sensors.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among the <c>YRefFrame.MOUNTPOSITION</c> enumeration
     *   (<c>YRefFrame.MOUNTPOSITION.BOTTOM</c>,   <c>YRefFrame.MOUNTPOSITION.TOP</c>,
     *   <c>YRefFrame.MOUNTPOSITION.FRONT</c>,    <c>YRefFrame.MOUNTPOSITION.RIGHT</c>,
     *   <c>YRefFrame.MOUNTPOSITION.REAR</c>,     <c>YRefFrame.MOUNTPOSITION.LEFT</c>),
     *   corresponding to the installation in a box, on one of the six faces.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YRefFrame.MOUNTPOSITION.INVALID.
     * </para>
     */
    public virtual MOUNTPOSITION get_mountPosition()
    {
        int position;
        position = this.get_mountPos();
        if (position < 0) {
            return MOUNTPOSITION.INVALID;
        }
        return (MOUNTPOSITION) ((position) >> (2));
    }


    /**
     * <summary>
     *   Returns the installation orientation of the device, as configured
     *   in order to define the reference frame for the compass and the
     *   pitch/roll tilt sensors.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among the enumeration <c>YRefFrame.MOUNTORIENTATION</c>
     *   (<c>YRefFrame.MOUNTORIENTATION.TWELVE</c>, <c>YRefFrame.MOUNTORIENTATION.THREE</c>,
     *   <c>YRefFrame.MOUNTORIENTATION.SIX</c>,     <c>YRefFrame.MOUNTORIENTATION.NINE</c>)
     *   corresponding to the orientation of the "X" arrow on the device,
     *   as on a clock dial seen from an observer in the center of the box.
     *   On the bottom face, the 12H orientation points to the front, while
     *   on the top face, the 12H orientation points to the rear.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YRefFrame.MOUNTORIENTATION.INVALID.
     * </para>
     */
    public virtual MOUNTORIENTATION get_mountOrientation()
    {
        int position;
        position = this.get_mountPos();
        if (position < 0) {
            return MOUNTORIENTATION.INVALID;
        }
        return (MOUNTORIENTATION) ((position) & (3));
    }


    /**
     * <summary>
     *   Changes the compass and tilt sensor frame of reference.
     * <para>
     *   The magnetic compass
     *   and the tilt sensors (pitch and roll) naturally work in the plane
     *   parallel to the earth surface. In case the device is not installed upright
     *   and horizontally, you must select its reference orientation (parallel to
     *   the earth surface) so that the measures are made relative to this position.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="position">
     *   a value among the <c>YRefFrame.MOUNTPOSITION</c> enumeration
     *   (<c>YRefFrame.MOUNTPOSITION.BOTTOM</c>,   <c>YRefFrame.MOUNTPOSITION.TOP</c>,
     *   <c>YRefFrame.MOUNTPOSITION.FRONT</c>,    <c>YRefFrame.MOUNTPOSITION.RIGHT</c>,
     *   <c>YRefFrame.MOUNTPOSITION.REAR</c>,     <c>YRefFrame.MOUNTPOSITION.LEFT</c>),
     *   corresponding to the installation in a box, on one of the six faces.
     * </param>
     * <param name="orientation">
     *   a value among the enumeration <c>YRefFrame.MOUNTORIENTATION</c>
     *   (<c>YRefFrame.MOUNTORIENTATION.TWELVE</c>, <c>YRefFrame.MOUNTORIENTATION.THREE</c>,
     *   <c>YRefFrame.MOUNTORIENTATION.SIX</c>,     <c>YRefFrame.MOUNTORIENTATION.NINE</c>)
     *   corresponding to the orientation of the "X" arrow on the device,
     *   as on a clock dial seen from an observer in the center of the box.
     *   On the bottom face, the 12H orientation points to the front, while
     *   on the top face, the 12H orientation points to the rear.
     * </param>
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_mountPosition(MOUNTPOSITION position, MOUNTORIENTATION orientation)
    {
        int mixedPos;
        mixedPos = (((int)position) << (2)) + (int)orientation;
        return this.set_mountPos(mixedPos);
    }


    /**
     * <summary>
     *   Returns the 3D sensor calibration state (Yocto-3D-V2 only).
     * <para>
     *   This function returns
     *   an integer representing the calibration state of the 3 inertial sensors of
     *   the BNO055 chip, found in the Yocto-3D-V2. Hundredths show the calibration state
     *   of the accelerometer, tenths show the calibration state of the magnetometer while
     *   units show the calibration state of the gyroscope. For each sensor, the value 0
     *   means no calibration and the value 3 means full calibration.
     * </para>
     * </summary>
     * <returns>
     *   an integer representing the calibration state of Yocto-3D-V2:
     *   333 when fully calibrated, 0 when not calibrated at all.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     *   For the Yocto-3D (V1), this function always return -3 (unsupported function).
     * </para>
     */
    public virtual int get_calibrationState()
    {
        string calibParam;
        List<int> iCalib = new List<int>();
        int caltyp;
        int res;

        calibParam = this.get_calibrationParam();
        iCalib = YAPI._decodeFloats(calibParam);
        caltyp = ((iCalib[0]) / (1000));
        if (caltyp != 33) {
            return YAPI.NOT_SUPPORTED;
        }
        res = ((iCalib[1]) / (1000));
        return res;
    }


    /**
     * <summary>
     *   Returns estimated quality of the orientation (Yocto-3D-V2 only).
     * <para>
     *   This function returns
     *   an integer between 0 and 3 representing the degree of confidence of the position
     *   estimate. When the value is 3, the estimation is reliable. Below 3, one should
     *   expect sudden corrections, in particular for heading (<c>compass</c> function).
     *   The most frequent causes for values below 3 are magnetic interferences, and
     *   accelerations or rotations beyond the sensor range.
     * </para>
     * </summary>
     * <returns>
     *   an integer between 0 and 3 (3 when the measure is reliable)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     *   For the Yocto-3D (V1), this function always return -3 (unsupported function).
     * </para>
     */
    public virtual int get_measureQuality()
    {
        string calibParam;
        List<int> iCalib = new List<int>();
        int caltyp;
        int res;

        calibParam = this.get_calibrationParam();
        iCalib = YAPI._decodeFloats(calibParam);
        caltyp = ((iCalib[0]) / (1000));
        if (caltyp != 33) {
            return YAPI.NOT_SUPPORTED;
        }
        res = ((iCalib[2]) / (1000));
        return res;
    }


    public virtual int _calibSort(int start, int stopidx)
    {
        int idx;
        int changed;
        double a;
        double b;
        double xa;
        double xb;
        // bubble sort is good since we will re-sort again after offset adjustment
        changed = 1;
        while (changed > 0) {
            changed = 0;
            a = this._calibDataAcc[start];
            idx = start + 1;
            while (idx < stopidx) {
                b = this._calibDataAcc[idx];
                if (a > b) {
                    this._calibDataAcc[idx-1] = b;
                    this._calibDataAcc[idx] = a;
                    xa = this._calibDataAccX[idx-1];
                    xb = this._calibDataAccX[idx];
                    this._calibDataAccX[idx-1] = xb;
                    this._calibDataAccX[idx] = xa;
                    xa = this._calibDataAccY[idx-1];
                    xb = this._calibDataAccY[idx];
                    this._calibDataAccY[idx-1] = xb;
                    this._calibDataAccY[idx] = xa;
                    xa = this._calibDataAccZ[idx-1];
                    xb = this._calibDataAccZ[idx];
                    this._calibDataAccZ[idx-1] = xb;
                    this._calibDataAccZ[idx] = xa;
                    changed = changed + 1;
                } else {
                    a = b;
                }
                idx = idx + 1;
            }
        }
        return 0;
    }


    /**
     * <summary>
     *   Initiates the sensors tridimensional calibration process.
     * <para>
     *   This calibration is used at low level for inertial position estimation
     *   and to enhance the precision of the tilt sensors.
     * </para>
     * <para>
     *   After calling this method, the device should be moved according to the
     *   instructions provided by method <c>get_3DCalibrationHint</c>,
     *   and <c>more3DCalibration</c> should be invoked about 5 times per second.
     *   The calibration procedure is completed when the method
     *   <c>get_3DCalibrationProgress</c> returns 100. At this point,
     *   the computed calibration parameters can be applied using method
     *   <c>save3DCalibration</c>. The calibration process can be cancelled
     *   at any time using method <c>cancel3DCalibration</c>.
     * </para>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     * </summary>
     */
    public virtual int start3DCalibration()
    {
        if (!(this.isOnline())) {
            return YAPI.DEVICE_NOT_FOUND;
        }
        if (this._calibStage != 0) {
            this.cancel3DCalibration();
        }
        this._calibSavedParams = this.get_calibrationParam();
        this._calibV2 = (YAPI._atoi(this._calibSavedParams) == 33);
        this.set_calibrationParam("0");
        this._calibCount = 50;
        this._calibStage = 1;
        this._calibStageHint = "Set down the device on a steady horizontal surface";
        this._calibStageProgress = 0;
        this._calibProgress = 1;
        this._calibInternalPos = 0;
        this._calibPrevTick = (int) ((YAPI.GetTickCount()) & (0x7FFFFFFF));
        this._calibOrient.Clear();
        this._calibDataAccX.Clear();
        this._calibDataAccY.Clear();
        this._calibDataAccZ.Clear();
        this._calibDataAcc.Clear();
        return YAPI.SUCCESS;
    }


    /**
     * <summary>
     *   Continues the sensors tridimensional calibration process previously
     *   initiated using method <c>start3DCalibration</c>.
     * <para>
     *   This method should be called approximately 5 times per second, while
     *   positioning the device according to the instructions provided by method
     *   <c>get_3DCalibrationHint</c>. Note that the instructions change during
     *   the calibration process.
     * </para>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     * </summary>
     */
    public virtual int more3DCalibration()
    {
        if (this._calibV2) {
            return this.more3DCalibrationV2();
        }
        return this.more3DCalibrationV1();
    }


    public virtual int more3DCalibrationV1()
    {
        int currTick;
        byte[] jsonData;
        double xVal;
        double yVal;
        double zVal;
        double xSq;
        double ySq;
        double zSq;
        double norm;
        int orient;
        int idx;
        int intpos;
        int err;
        // make sure calibration has been started
        if (this._calibStage == 0) {
            return YAPI.INVALID_ARGUMENT;
        }
        if (this._calibProgress == 100) {
            return YAPI.SUCCESS;
        }
        // make sure we leave at least 160 ms between samples
        currTick =  (int) ((YAPI.GetTickCount()) & (0x7FFFFFFF));
        if (((currTick - this._calibPrevTick) & (0x7FFFFFFF)) < 160) {
            return YAPI.SUCCESS;
        }
        // load current accelerometer values, make sure we are on a straight angle
        // (default timeout to 0,5 sec without reading measure when out of range)
        this._calibStageHint = "Set down the device on a steady horizontal surface";
        this._calibPrevTick = ((currTick + 500) & (0x7FFFFFFF));
        jsonData = this._download("api/accelerometer.json");
        xVal = YAPI._atoi(this._json_get_key(jsonData, "xValue")) / 65536.0;
        yVal = YAPI._atoi(this._json_get_key(jsonData, "yValue")) / 65536.0;
        zVal = YAPI._atoi(this._json_get_key(jsonData, "zValue")) / 65536.0;
        xSq = xVal * xVal;
        if (xSq >= 0.04 && xSq < 0.64) {
            return YAPI.SUCCESS;
        }
        if (xSq >= 1.44) {
            return YAPI.SUCCESS;
        }
        ySq = yVal * yVal;
        if (ySq >= 0.04 && ySq < 0.64) {
            return YAPI.SUCCESS;
        }
        if (ySq >= 1.44) {
            return YAPI.SUCCESS;
        }
        zSq = zVal * zVal;
        if (zSq >= 0.04 && zSq < 0.64) {
            return YAPI.SUCCESS;
        }
        if (zSq >= 1.44) {
            return YAPI.SUCCESS;
        }
        norm = Math.Sqrt(xSq + ySq + zSq);
        if (norm < 0.8 || norm > 1.2) {
            return YAPI.SUCCESS;
        }
        this._calibPrevTick = currTick;
        // Determine the device orientation index
        orient = 0;
        if (zSq > 0.5) {
            if (zVal > 0) {
                orient = 0;
            } else {
                orient = 1;
            }
        }
        if (xSq > 0.5) {
            if (xVal > 0) {
                orient = 2;
            } else {
                orient = 3;
            }
        }
        if (ySq > 0.5) {
            if (yVal > 0) {
                orient = 4;
            } else {
                orient = 5;
            }
        }
        // Discard measures that are not in the proper orientation
        if (this._calibStageProgress == 0) {
            // New stage, check that this orientation is not yet done
            idx = 0;
            err = 0;
            while (idx + 1 < this._calibStage) {
                if (this._calibOrient[idx] == orient) {
                    err = 1;
                }
                idx = idx + 1;
            }
            if (err != 0) {
                this._calibStageHint = "Turn the device on another face";
                return YAPI.SUCCESS;
            }
            this._calibOrient.Add(orient);
        } else {
            // Make sure device is not turned before stage is completed
            if (orient != this._calibOrient[this._calibStage-1]) {
                this._calibStageHint = "Not yet done, please move back to the previous face";
                return YAPI.SUCCESS;
            }
        }
        // Save measure
        this._calibStageHint = "calibrating..";
        this._calibDataAccX.Add(xVal);
        this._calibDataAccY.Add(yVal);
        this._calibDataAccZ.Add(zVal);
        this._calibDataAcc.Add(norm);
        this._calibInternalPos = this._calibInternalPos + 1;
        this._calibProgress = 1 + 16 * (this._calibStage - 1) + ((16 * this._calibInternalPos) / (this._calibCount));
        if (this._calibInternalPos < this._calibCount) {
            this._calibStageProgress = 1 + ((99 * this._calibInternalPos) / (this._calibCount));
            return YAPI.SUCCESS;
        }
        // Stage done, compute preliminary result
        intpos = (this._calibStage - 1) * this._calibCount;
        this._calibSort(intpos, intpos + this._calibCount);
        intpos = intpos + ((this._calibCount) / (2));
        this._calibLogMsg = "Stage "+Convert.ToString( this._calibStage)+": median is "+Convert.ToString(
        (int) Math.Round(1000*this._calibDataAccX[intpos]))+","+Convert.ToString(
        (int) Math.Round(1000*this._calibDataAccY[intpos]))+","+Convert.ToString((int) Math.Round(1000*this._calibDataAccZ[intpos]));
        // move to next stage
        this._calibStage = this._calibStage + 1;
        if (this._calibStage < 7) {
            this._calibStageHint = "Turn the device on another face";
            this._calibPrevTick = ((currTick + 500) & (0x7FFFFFFF));
            this._calibStageProgress = 0;
            this._calibInternalPos = 0;
            return YAPI.SUCCESS;
        }
        // Data collection completed, compute accelerometer shift
        xVal = 0;
        yVal = 0;
        zVal = 0;
        idx = 0;
        while (idx < 6) {
            intpos = idx * this._calibCount + ((this._calibCount) / (2));
            orient = this._calibOrient[idx];
            if (orient == 0 || orient == 1) {
                zVal = zVal + this._calibDataAccZ[intpos];
            }
            if (orient == 2 || orient == 3) {
                xVal = xVal + this._calibDataAccX[intpos];
            }
            if (orient == 4 || orient == 5) {
                yVal = yVal + this._calibDataAccY[intpos];
            }
            idx = idx + 1;
        }
        this._calibAccXOfs = xVal / 2.0;
        this._calibAccYOfs = yVal / 2.0;
        this._calibAccZOfs = zVal / 2.0;
        // Recompute all norms, taking into account the computed shift, and re-sort
        intpos = 0;
        while (intpos < this._calibDataAcc.Count) {
            xVal = this._calibDataAccX[intpos] - this._calibAccXOfs;
            yVal = this._calibDataAccY[intpos] - this._calibAccYOfs;
            zVal = this._calibDataAccZ[intpos] - this._calibAccZOfs;
            norm = Math.Sqrt(xVal * xVal + yVal * yVal + zVal * zVal);
            this._calibDataAcc[intpos] = norm;
            intpos = intpos + 1;
        }
        idx = 0;
        while (idx < 6) {
            intpos = idx * this._calibCount;
            this._calibSort(intpos, intpos + this._calibCount);
            idx = idx + 1;
        }
        // Compute the scaling factor for each axis
        xVal = 0;
        yVal = 0;
        zVal = 0;
        idx = 0;
        while (idx < 6) {
            intpos = idx * this._calibCount + ((this._calibCount) / (2));
            orient = this._calibOrient[idx];
            if (orient == 0 || orient == 1) {
                zVal = zVal + this._calibDataAcc[intpos];
            }
            if (orient == 2 || orient == 3) {
                xVal = xVal + this._calibDataAcc[intpos];
            }
            if (orient == 4 || orient == 5) {
                yVal = yVal + this._calibDataAcc[intpos];
            }
            idx = idx + 1;
        }
        this._calibAccXScale = xVal / 2.0;
        this._calibAccYScale = yVal / 2.0;
        this._calibAccZScale = zVal / 2.0;
        // Report completion
        this._calibProgress = 100;
        this._calibStageHint = "Calibration data ready for saving";
        return YAPI.SUCCESS;
    }


    public virtual int more3DCalibrationV2()
    {
        int currTick;
        byte[] calibParam;
        List<int> iCalib = new List<int>();
        int cal3;
        int calAcc;
        int calMag;
        int calGyr;
        // make sure calibration has been started
        if (this._calibStage == 0) {
            return YAPI.INVALID_ARGUMENT;
        }
        if (this._calibProgress == 100) {
            return YAPI.SUCCESS;
        }
        // make sure we don't start before previous calibration is cleared
        if (this._calibStage == 1) {
            currTick = (int) ((YAPI.GetTickCount()) & (0x7FFFFFFF));
            currTick = ((currTick - this._calibPrevTick) & (0x7FFFFFFF));
            if (currTick < 1600) {
                this._calibStageHint = "Set down the device on a steady horizontal surface";
                this._calibStageProgress = ((currTick) / (40));
                this._calibProgress = 1;
                return YAPI.SUCCESS;
            }
        }

        calibParam = this._download("api/refFrame/calibrationParam.txt");
        iCalib = YAPI._decodeFloats(YAPI.DefaultEncoding.GetString(calibParam));
        cal3 = ((iCalib[1]) / (1000));
        calAcc = ((cal3) / (100));
        calMag = ((cal3) / (10)) - 10*calAcc;
        calGyr = ((cal3) % (10));
        if (calGyr < 3) {
            this._calibStageHint = "Set down the device on a steady horizontal surface";
            this._calibStageProgress = 40 + calGyr*20;
            this._calibProgress = 4 + calGyr*2;
        } else {
            this._calibStage = 2;
            if (calMag < 3) {
                this._calibStageHint = "Slowly draw '8' shapes along the 3 axis";
                this._calibStageProgress = 1 + calMag*33;
                this._calibProgress = 10 + calMag*5;
            } else {
                this._calibStage = 3;
                if (calAcc < 3) {
                    this._calibStageHint = "Slowly turn the device, stopping at each 90 degrees";
                    this._calibStageProgress = 1 + calAcc*33;
                    this._calibProgress = 25 + calAcc*25;
                } else {
                    this._calibStageProgress = 99;
                    this._calibProgress = 100;
                }
            }
        }
        return YAPI.SUCCESS;
    }


    /**
     * <summary>
     *   Returns instructions to proceed to the tridimensional calibration initiated with
     *   method <c>start3DCalibration</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a character string.
     * </returns>
     */
    public virtual string get_3DCalibrationHint()
    {
        return this._calibStageHint;
    }


    /**
     * <summary>
     *   Returns the global process indicator for the tridimensional calibration
     *   initiated with method <c>start3DCalibration</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer between 0 (not started) and 100 (stage completed).
     * </returns>
     */
    public virtual int get_3DCalibrationProgress()
    {
        return this._calibProgress;
    }


    /**
     * <summary>
     *   Returns index of the current stage of the calibration
     *   initiated with method <c>start3DCalibration</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer, growing each time a calibration stage is completed.
     * </returns>
     */
    public virtual int get_3DCalibrationStage()
    {
        return this._calibStage;
    }


    /**
     * <summary>
     *   Returns the process indicator for the current stage of the calibration
     *   initiated with method <c>start3DCalibration</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer between 0 (not started) and 100 (stage completed).
     * </returns>
     */
    public virtual int get_3DCalibrationStageProgress()
    {
        return this._calibStageProgress;
    }


    /**
     * <summary>
     *   Returns the latest log message from the calibration process.
     * <para>
     *   When no new message is available, returns an empty string.
     * </para>
     * </summary>
     * <returns>
     *   a character string.
     * </returns>
     */
    public virtual string get_3DCalibrationLogMsg()
    {
        string msg;
        msg = this._calibLogMsg;
        this._calibLogMsg = "";
        return msg;
    }


    /**
     * <summary>
     *   Applies the sensors tridimensional calibration parameters that have just been computed.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>  method of the module if the changes
     *   must be kept when the device is restarted.
     * </para>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     * </summary>
     */
    public virtual int save3DCalibration()
    {
        if (this._calibV2) {
            return this.save3DCalibrationV2();
        }
        return this.save3DCalibrationV1();
    }


    public virtual int save3DCalibrationV1()
    {
        int shiftX;
        int shiftY;
        int shiftZ;
        int scaleExp;
        int scaleX;
        int scaleY;
        int scaleZ;
        int scaleLo;
        int scaleHi;
        string newcalib;
        if (this._calibProgress != 100) {
            return YAPI.INVALID_ARGUMENT;
        }
        // Compute integer values (correction unit is 732ug/count)
        shiftX = -(int) Math.Round(this._calibAccXOfs / 0.000732);
        if (shiftX < 0) {
            shiftX = shiftX + 65536;
        }
        shiftY = -(int) Math.Round(this._calibAccYOfs / 0.000732);
        if (shiftY < 0) {
            shiftY = shiftY + 65536;
        }
        shiftZ = -(int) Math.Round(this._calibAccZOfs / 0.000732);
        if (shiftZ < 0) {
            shiftZ = shiftZ + 65536;
        }
        scaleX = (int) Math.Round(2048.0 / this._calibAccXScale) - 2048;
        scaleY = (int) Math.Round(2048.0 / this._calibAccYScale) - 2048;
        scaleZ = (int) Math.Round(2048.0 / this._calibAccZScale) - 2048;
        if (scaleX < -2048 || scaleX >= 2048 || scaleY < -2048 || scaleY >= 2048 || scaleZ < -2048 || scaleZ >= 2048) {
            scaleExp = 3;
        } else {
            if (scaleX < -1024 || scaleX >= 1024 || scaleY < -1024 || scaleY >= 1024 || scaleZ < -1024 || scaleZ >= 1024) {
                scaleExp = 2;
            } else {
                if (scaleX < -512 || scaleX >= 512 || scaleY < -512 || scaleY >= 512 || scaleZ < -512 || scaleZ >= 512) {
                    scaleExp = 1;
                } else {
                    scaleExp = 0;
                }
            }
        }
        if (scaleExp > 0) {
            scaleX = ((scaleX) >> (scaleExp));
            scaleY = ((scaleY) >> (scaleExp));
            scaleZ = ((scaleZ) >> (scaleExp));
        }
        if (scaleX < 0) {
            scaleX = scaleX + 1024;
        }
        if (scaleY < 0) {
            scaleY = scaleY + 1024;
        }
        if (scaleZ < 0) {
            scaleZ = scaleZ + 1024;
        }
        scaleLo = ((((scaleY) & (15))) << (12)) + ((scaleX) << (2)) + scaleExp;
        scaleHi = ((scaleZ) << (6)) + ((scaleY) >> (4));
        // Save calibration parameters
        newcalib = "5,"+Convert.ToString( shiftX)+","+Convert.ToString( shiftY)+","+Convert.ToString( shiftZ)+","+Convert.ToString( scaleLo)+","+Convert.ToString(scaleHi);
        this._calibStage = 0;
        return this.set_calibrationParam(newcalib);
    }


    public virtual int save3DCalibrationV2()
    {
        return this.set_calibrationParam("5,5,5,5,5,5");
    }


    /**
     * <summary>
     *   Aborts the sensors tridimensional calibration process et restores normal settings.
     * <para>
     * </para>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     * </summary>
     */
    public virtual int cancel3DCalibration()
    {
        if (this._calibStage == 0) {
            return YAPI.SUCCESS;
        }

        this._calibStage = 0;
        return this.set_calibrationParam(this._calibSavedParams);
    }

    /**
     * <summary>
     *   Continues the enumeration of reference frames started using <c>yFirstRefFrame()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned reference frames order.
     *   If you want to find a specific a reference frame, use <c>RefFrame.findRefFrame()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRefFrame</c> object, corresponding to
     *   a reference frame currently online, or a <c>null</c> pointer
     *   if there are no more reference frames to enumerate.
     * </returns>
     */
    public YRefFrame nextRefFrame()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindRefFrame(hwid);
    }

    //--- (end of YRefFrame implementation)

    //--- (YRefFrame functions)

    /**
     * <summary>
     *   Starts the enumeration of reference frames currently accessible.
     * <para>
     *   Use the method <c>YRefFrame.nextRefFrame()</c> to iterate on
     *   next reference frames.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRefFrame</c> object, corresponding to
     *   the first reference frame currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YRefFrame FirstRefFrame()
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
        err = YAPI.apiGetFunctionsByClass("RefFrame", 0, p, size, ref neededsize, ref errmsg);
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
        return FindRefFrame(serial + "." + funcId);
    }



    //--- (end of YRefFrame functions)
}
#pragma warning restore 1591
