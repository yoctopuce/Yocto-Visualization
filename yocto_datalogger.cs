/********************************************************************
 *
 * $Id: yocto_datalogger.cs 27191 2017-04-20 17:02:22Z seb $
 *
 * High-level programming interface, common to all modules
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
using System.Diagnostics;


using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using YDEV_DESCR = System.Int32;// yStrRef of serial number
using YFUN_DESCR = System.Int32;	// yStrRef of serial + (ystrRef of funcId << 16)
using System.Runtime.InteropServices;




/**
 * YOldDataStream Class: Sequence of measured data, returned by the data logger
 *  
 * A data stream is a small collection of consecutive measures for a set
 * of sensors. A few properties are available directly from the object itself
 * (they are preloaded at instantiation time), while most other properties and
 * the actual data are loaded on demand when accessed for the first time.
 *
 * This is the old version of the YDataStream class, used for backward-compatibility
 * with devices with firmware < 13000
 */
public class YOldDataStream : YDataStream
{
    protected YDataLogger _dataLogger;
    protected int _timeStamp;
    protected int _interval;

    public YOldDataStream(YDataLogger parent, int run, int stamp, UInt32 utc, int itv)
    : base(parent)
    {
        this._dataLogger = parent;
        this._runNo = run;
        this._timeStamp = stamp;
        this._utcStamp = utc;
        this._interval = itv;
        this._samplesPerHour = (int)(3600 / _interval);
        this._isClosed = true;
        this._minVal = DATA_INVALID;
        this._avgVal = DATA_INVALID;
        this._maxVal = DATA_INVALID;
    }

    /**
     * <summary>
     *   Returns the relative start time of the data stream, measured in seconds.
     * <para>
     *   For recent firmwares, the value is relative to the present time,
     *   which means the value is always negative.
     *   If the device uses a firmware older than version 13000, value is
     *   relative to the start of the time the device was powered on, and
     *   is always positive.
     *   If you need an absolute UTC timestamp, use <c>get_startTimeUTC()</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of seconds
     *   between the start of the run and the beginning of this data
     *   stream.
     * </returns>
     */
    public new int get_startTime()
    {
        return (int)this._timeStamp;
    }

    /**
     * <summary>
     *   Returns the number of seconds elapsed between  two consecutive
     *   rows of this data stream.
     * <para>
     *   By default, the data logger records one row
     *   per second, but there might be alternative streams at lower resolution
     *   created by summarizing the original stream for archiving purposes.
     * </para>
     * <para>
     *   This method does not cause any access to the device, as the value
     *   is preloaded in the object at instantiation time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to a number of seconds.
     * </returns>
     */
    public new int get_dataSamplesInterval()
    {
        return (int)this._interval;
    }

    public new int loadStream()
    {
        YAPI.YJSONContent raw_json;
        YAPI.YJSONObject  json;
        int res = 0;
        List<int> coldiv = new List<int>();
        List<int> coltype = new List<int>();
        List<int> udat = new List<int>();
        List<double> date = new List<double>();
        List<double> colscl = new List<double>();
        List<int> colofs = new List<int>();
        List<int> caltyp = new List<int>();
        List<YAPI.yCalibrationHandler> calhdl = new List<YAPI.yCalibrationHandler>();
        List<List<int>> calpar = new List<List<int>>();
        List<List<double>> calraw = new List<List<double>>();
        List<List<double>> calref = new List<List<double>>();

        int x = 0;
        int j = 0;

        res = _dataLogger.getData(_runNo, _timeStamp, out raw_json);
        if ((res != YAPI.SUCCESS))
        {
            return res;
        }

        _nRows = 0;
        _nCols = 0;
        _columnNames.Clear();
        _values = new List< List<double> >();
        json = (YAPI.YJSONObject) raw_json;


        if (json.has("time")) {
            _timeStamp = json.getInt("time");
        }
        if (json.has("UTC")) {
            _utcStamp = json.getLong("UTC");
        }
         if (json.has("interval")) {
            _interval = json.getInt("interval");
        }
        if (json.has("nRows")) {
            _nRows = json.getInt("nRows");
        }
        if (json.has("keys")) {
            YAPI.YJSONArray jsonkeys = json.getYJSONArray("keys");
            _nCols = jsonkeys.Length;
            for (j = 0; j < _nCols ; j++) {
                _columnNames.Add(jsonkeys.getString(j));
            }
        }
         if (json.has("div")) {
            YAPI.YJSONArray arr = json.getYJSONArray("div");
            _nCols = arr.Length;
            for (j = 0; j < _nCols ; j++) {
                coldiv.Add(arr.getInt(j));
            }
        }
         if (json.has("type")) {
            YAPI.YJSONArray arr = json.getYJSONArray("type");
            _nCols = arr.Length;
            for (j = 0; j < _nCols; j++) {
                coltype.Add(arr.getInt(j));
            }
        }
         if (json.has("scal")) {
            YAPI.YJSONArray arr = json.getYJSONArray("type");
            _nCols = arr.Length;
            for (j = 0; j < _nCols; j++) {
                colscl.Add(arr.getInt(j) / 65536.0);
                if (coltype[j] != 0)
                    colofs.Add(-32767);
                else
                    colofs.Add(0);
            }

        }
         if (json.has("cal")) {
            //fixme no calibration
        }
        if (json.has("data")) {
            if (colscl.Count <= 0) {
                for (j = 0; j <= _nCols - 1; j++) {
                    colscl.Add(1.0/coldiv[j]);
                    if (coltype[j] != 0)
                        colofs.Add(-32767);
                    else
                        colofs.Add(0);
                }
            }
            udat.Clear();
            string data = null;
            try {
                data = json.getString("data");
                udat = YAPI._decodeWords(data);
            }
            catch (Exception ) {}
            if (data==null) {
                YAPI.YJSONArray jsonData = json.getYJSONArray("data");
                for (j = 0; j < jsonData.Length; j++) {
                    int tmp = (int) (jsonData.getInt(j));
                    udat.Add(tmp);
                }
            }
            _values = new List<List<double>>();
            List<double> dat = new List<double>();
            foreach (int uval in udat) {
                double value;
                if (coltype[x] < 2) {
                    value = (uval + colofs[x])*colscl[x];
                }
                else {
                    value = YAPI._decimalToDouble(uval - 32767);
                }
                if (caltyp[x] > 0 && calhdl[x] != null) {
                    YAPI.yCalibrationHandler handler = calhdl[x];
                    if (caltyp[x] <= 10) {
                        value = handler((uval + colofs[x])/coldiv[x], caltyp[x], calpar[x], calraw[x], calref[x]);
                    }
                    else if (caltyp[x] > 20) {
                        value = handler(value, caltyp[x], calpar[x], calraw[x], calref[x]);
                    }
                }
                dat.Add(value);
                x++;
                if (x == _nCols) {
                    _values.Add(dat);
                    dat.Clear();
                    x = 0;
                }
            }
        }
        return YAPI.SUCCESS;
    }


}



//--- (generated code: YDataLogger class start)
/**
 * <summary>
 *   Yoctopuce sensors include a non-volatile memory capable of storing ongoing measured
 *   data automatically, without requiring a permanent connection to a computer.
 * <para>
 *   The DataLogger function controls the global parameters of the internal data
 *   logger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YDataLogger : YFunction
{
//--- (end of generated code: YDataLogger class start)

    public const double Y_DATA_INVALID = YAPI.INVALID_DOUBLE;
    //--- (generated code: YDataLogger definitions)
    public new delegate void ValueCallback(YDataLogger func, string value);
    public new delegate void TimedReportCallback(YDataLogger func, YMeasure measure);

    public const int CURRENTRUNINDEX_INVALID = YAPI.INVALID_UINT;
    public const long TIMEUTC_INVALID = YAPI.INVALID_LONG;
    public const int RECORDING_OFF = 0;
    public const int RECORDING_ON = 1;
    public const int RECORDING_PENDING = 2;
    public const int RECORDING_INVALID = -1;
    public const int AUTOSTART_OFF = 0;
    public const int AUTOSTART_ON = 1;
    public const int AUTOSTART_INVALID = -1;
    public const int BEACONDRIVEN_OFF = 0;
    public const int BEACONDRIVEN_ON = 1;
    public const int BEACONDRIVEN_INVALID = -1;
    public const int CLEARHISTORY_FALSE = 0;
    public const int CLEARHISTORY_TRUE = 1;
    public const int CLEARHISTORY_INVALID = -1;
    protected int _currentRunIndex = CURRENTRUNINDEX_INVALID;
    protected long _timeUTC = TIMEUTC_INVALID;
    protected int _recording = RECORDING_INVALID;
    protected int _autoStart = AUTOSTART_INVALID;
    protected int _beaconDriven = BEACONDRIVEN_INVALID;
    protected int _clearHistory = CLEARHISTORY_INVALID;
    protected ValueCallback _valueCallbackDataLogger = null;
    //--- (end of generated code: YDataLogger definitions)
    protected string _dataLoggerURL = "";

    public YDataLogger(string func)
        : base(func)
    {
        _className = "DataLogger";
        //--- (generated code: YDataLogger attributes initialization)
        //--- (end of generated code: YDataLogger attributes initialization)
    }

    //--- (generated code: YDataLogger implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("currentRunIndex"))
        {
            _currentRunIndex = json_val.getInt("currentRunIndex");
        }
        if (json_val.has("timeUTC"))
        {
            _timeUTC = json_val.getLong("timeUTC");
        }
        if (json_val.has("recording"))
        {
            _recording = json_val.getInt("recording");
        }
        if (json_val.has("autoStart"))
        {
            _autoStart = json_val.getInt("autoStart") > 0 ? 1 : 0;
        }
        if (json_val.has("beaconDriven"))
        {
            _beaconDriven = json_val.getInt("beaconDriven") > 0 ? 1 : 0;
        }
        if (json_val.has("clearHistory"))
        {
            _clearHistory = json_val.getInt("clearHistory") > 0 ? 1 : 0;
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the current run number, corresponding to the number of times the module was
     *   powered on with the dataLogger enabled at some point.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current run number, corresponding to the number of times the module was
     *   powered on with the dataLogger enabled at some point
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDataLogger.CURRENTRUNINDEX_INVALID</c>.
     * </para>
     */
    public int get_currentRunIndex()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS) {
                    return CURRENTRUNINDEX_INVALID;
                }
            }
            res = this._currentRunIndex;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the Unix timestamp for current UTC time, if known.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the Unix timestamp for current UTC time, if known
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDataLogger.TIMEUTC_INVALID</c>.
     * </para>
     */
    public long get_timeUTC()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS) {
                    return TIMEUTC_INVALID;
                }
            }
            res = this._timeUTC;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the current UTC time reference used for recorded data.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the current UTC time reference used for recorded data
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
    public int set_timeUTC(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("timeUTC", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the current activation state of the data logger.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YDataLogger.RECORDING_OFF</c>, <c>YDataLogger.RECORDING_ON</c> and
     *   <c>YDataLogger.RECORDING_PENDING</c> corresponding to the current activation state of the data logger
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDataLogger.RECORDING_INVALID</c>.
     * </para>
     */
    public int get_recording()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS) {
                    return RECORDING_INVALID;
                }
            }
            res = this._recording;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the activation state of the data logger to start/stop recording data.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YDataLogger.RECORDING_OFF</c>, <c>YDataLogger.RECORDING_ON</c> and
     *   <c>YDataLogger.RECORDING_PENDING</c> corresponding to the activation state of the data logger to
     *   start/stop recording data
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
    public int set_recording(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("recording", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the default activation state of the data logger on power up.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YDataLogger.AUTOSTART_OFF</c> or <c>YDataLogger.AUTOSTART_ON</c>, according to the
     *   default activation state of the data logger on power up
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDataLogger.AUTOSTART_INVALID</c>.
     * </para>
     */
    public int get_autoStart()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS) {
                    return AUTOSTART_INVALID;
                }
            }
            res = this._autoStart;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the default activation state of the data logger on power up.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YDataLogger.AUTOSTART_OFF</c> or <c>YDataLogger.AUTOSTART_ON</c>, according to the
     *   default activation state of the data logger on power up
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
    public int set_autoStart(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("autoStart", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns true if the data logger is synchronised with the localization beacon.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YDataLogger.BEACONDRIVEN_OFF</c> or <c>YDataLogger.BEACONDRIVEN_ON</c>, according to true
     *   if the data logger is synchronised with the localization beacon
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDataLogger.BEACONDRIVEN_INVALID</c>.
     * </para>
     */
    public int get_beaconDriven()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS) {
                    return BEACONDRIVEN_INVALID;
                }
            }
            res = this._beaconDriven;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the type of synchronisation of the data logger.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YDataLogger.BEACONDRIVEN_OFF</c> or <c>YDataLogger.BEACONDRIVEN_ON</c>, according to the
     *   type of synchronisation of the data logger
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
    public int set_beaconDriven(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("beaconDriven", rest_val);
        }
    }

    public int get_clearHistory()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS) {
                    return CLEARHISTORY_INVALID;
                }
            }
            res = this._clearHistory;
        }
        return res;
    }

    public int set_clearHistory(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("clearHistory", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a data logger for a given identifier.
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
     *   This function does not require that the data logger is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YDataLogger.isOnline()</c> to test if the data logger is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a data logger by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the data logger
     * </param>
     * <returns>
     *   a <c>YDataLogger</c> object allowing you to drive the data logger.
     * </returns>
     */
    public static YDataLogger FindDataLogger(string func)
    {
        YDataLogger obj;
        lock (YAPI.globalLock) {
            obj = (YDataLogger) YFunction._FindFromCache("DataLogger", func);
            if (obj == null) {
                obj = new YDataLogger(func);
                YFunction._AddToCache("DataLogger", func, obj);
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
        this._valueCallbackDataLogger = callback;
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
        if (this._valueCallbackDataLogger != null) {
            this._valueCallbackDataLogger(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Clears the data logger memory and discards all recorded data streams.
     * <para>
     *   This method also resets the current run index to zero.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int forgetAllDataStreams()
    {
        return this.set_clearHistory(CLEARHISTORY_TRUE);
    }

    /**
     * <summary>
     *   Returns a list of YDataSet objects that can be used to retrieve
     *   all measures stored by the data logger.
     * <para>
     * </para>
     * <para>
     *   This function only works if the device uses a recent firmware,
     *   as YDataSet objects are not supported by firmwares older than
     *   version 13000.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a list of YDataSet object.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list.
     * </para>
     */
    public virtual List<YDataSet> get_dataSets()
    {
        return this.parse_dataSets(this._download("logger.json"));
    }

    public virtual List<YDataSet> parse_dataSets(byte[] json)
    {
        List<string> dslist = new List<string>();
        YDataSet dataset;
        List<YDataSet> res = new List<YDataSet>();
        
        dslist = this._json_get_array(json);
        res.Clear();
        for (int ii = 0; ii < dslist.Count; ii++) {
            dataset = new YDataSet(this);
            dataset._parse(dslist[ii]);
            res.Add(dataset);
        }
        return res;
    }

    /**
     * <summary>
     *   Continues the enumeration of data loggers started using <c>yFirstDataLogger()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YDataLogger</c> object, corresponding to
     *   a data logger currently online, or a <c>null</c> pointer
     *   if there are no more data loggers to enumerate.
     * </returns>
     */
    public YDataLogger nextDataLogger()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindDataLogger(hwid);
    }

    //--- (end of generated code: YDataLogger implementation)

   

    public int getData(long runIdx, long timeIdx, out YAPI.YJSONContent jsondata)
    {
        YAPI.YDevice dev = null;
        string errmsg = "";
        string query = null;
        string buffer = "";
        int res = 0;
        int http_headerlen;

        if (_dataLoggerURL == "") _dataLoggerURL = "/logger.json";

        // Resolve our reference to our device, load REST API
        res = _getDevice(ref dev, ref errmsg);
        if (YAPI.YISERR(res))
        {
            _throw(res, errmsg);
            jsondata = null;
            return res;
        }

        if (timeIdx > 0)
        {
            query = "GET " + _dataLoggerURL + "?run=" + runIdx.ToString() + "&time=" + timeIdx.ToString() + " HTTP/1.1\r\n\r\n";
        }
        else
        {
            query = "GET " + _dataLoggerURL + " HTTP/1.1\r\n\r\n";
        }

        res = dev.HTTPRequest(query, out buffer, ref errmsg);
        if (YAPI.YISERR(res))
        {
            res = YAPI.UpdateDeviceList(ref errmsg);
            if (YAPI.YISERR(res))
            {
                _throw(res, errmsg);
                jsondata = null;
                return res;
            }

            res = dev.HTTPRequest("GET " + _dataLoggerURL + " HTTP/1.1\r\n\r\n", out buffer, ref errmsg);
            if (YAPI.YISERR(res))
            {
                _throw(res, errmsg);
                jsondata = null;
                return res;
            }
        }

        int httpcode = YAPI.ParseHTTP(buffer, 0, buffer.Length, out http_headerlen, out errmsg);
        if (httpcode == 404 && _dataLoggerURL != "/dataLogger.json") {
            // retry using backward-compatible datalogger URL
            _dataLoggerURL = "/dataLogger.json";
            return this.getData(runIdx, timeIdx, out jsondata);
        }

        if (httpcode != 200) {
            jsondata = null;
            return YAPI.IO_ERROR;
        }
        try {
            jsondata = YAPI.YJSONContent.ParseJson(buffer, http_headerlen, buffer.Length);
        } catch (Exception E) {
            errmsg = "unexpected JSON structure: " + E.Message;
            _throw(YAPI.IO_ERROR, errmsg);
            jsondata = null;
            return YAPI.IO_ERROR;
        }      
        return YAPI.SUCCESS;
    }

   /**
    * <summary>
    *   Builds a list of all data streams hold by the data logger (legacy method).
    * <para>
    *   The caller must pass by reference an empty array to hold YDataStream
    *   objects, and the function fills it with objects describing available
    *   data sequences.
    * </para>
    * <para>
    *   This is the old way to retrieve data from the DataLogger.
    *   For new applications, you should rather use <c>get_dataSets()</c>
    *   method, or call directly <c>get_recordedData()</c> on the
    *   sensor object.
    * </para>
    * <para>
    * </para>
    * </summary>
    * <param name="v">
    *   an array of YDataStream objects to be filled in
    * </param>
    * <returns>
    *   <c>YAPI.SUCCESS</c> if the call succeeds.
    * </returns>
    * <para>
    *   On failure, throws an exception or returns a negative error code.
    * </para>
    */
    public int get_dataStreams(List<YDataStream> v)
    {
        YAPI.YJSONContent raw_json;
        YAPI.YJSONArray root;
        int i = 0;
        int res = 0;

        v.Clear();
        res = getData(0, 0, out raw_json);
        if (res != YAPI.SUCCESS)
        {
            return res;
        }
        root = (YAPI.YJSONArray) raw_json;


        if (root.Length == 0) 
            return YAPI.SUCCESS;
        if (root.get(0).getJSONType() == YAPI.YJSONContent.YJSONType.ARRAY) {
            // old datalogger format: [runIdx, timerel, utc, interval]
            for (i = 0; i < root.Length ; i++)
            {
                YAPI.YJSONArray el = root.getYJSONArray(i);
                v.Add(new YOldDataStream(this,el.getInt(0), el.getInt(1), (uint) el.getLong(2), el.getInt(1)));
            }
        } else {
            // new datalogger format: {"id":"...","unit":"...","streams":["...",...]}
            
            string json_buffer = root.toJSON();
            List<YDataSet> sets = this.parse_dataSets(YAPI.DefaultEncoding.GetBytes(json_buffer));
            for (int sj = 0; sj < sets.Count; sj++)
            {
                List<YDataStream> ds = sets[sj].get_privateDataStreams();
                for (int si = 0; si < ds.Count; si++)
                {
                    v.Add(ds[si]);
                }
            }
        }
        return YAPI.SUCCESS;
    }

  
    //--- (generated code: DataLogger functions)

    /**
     * <summary>
     *   Starts the enumeration of data loggers currently accessible.
     * <para>
     *   Use the method <c>YDataLogger.nextDataLogger()</c> to iterate on
     *   next data loggers.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YDataLogger</c> object, corresponding to
     *   the first data logger currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YDataLogger FirstDataLogger()
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
        err = YAPI.apiGetFunctionsByClass("DataLogger", 0, p, size, ref neededsize, ref errmsg);
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
        return FindDataLogger(serial + "." + funcId);
    }



    //--- (end of generated code: DataLogger functions)
}

