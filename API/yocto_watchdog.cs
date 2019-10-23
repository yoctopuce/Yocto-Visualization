/*********************************************************************
 *
 *  $Id: yocto_watchdog.cs 37619 2019-10-11 11:52:42Z mvuilleu $
 *
 *  Implements yFindWatchdog(), the high-level API for Watchdog functions
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
    //--- (YWatchdog return codes)
    //--- (end of YWatchdog return codes)
//--- (YWatchdog dlldef)
//--- (end of YWatchdog dlldef)
//--- (YWatchdog yapiwrapper)
//--- (end of YWatchdog yapiwrapper)
//--- (YWatchdog class start)
/**
 * <summary>
 *   The watchdog function works like a relay and can cause a brief power cut
 *   to an appliance after a preset delay to force this appliance to
 *   reset.
 * <para>
 *   The Watchdog must be called from time to time to reset the
 *   timer and prevent the appliance reset.
 *   The watchdog can be driven directly with <i>pulse</i> and <i>delayedpulse</i> methods to switch
 *   off an appliance for a given duration.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YWatchdog : YFunction
{
//--- (end of YWatchdog class start)
    //--- (YWatchdog definitions)
    public new delegate void ValueCallback(YWatchdog func, string value);
    public new delegate void TimedReportCallback(YWatchdog func, YMeasure measure);

    public class YWatchdogDelayedPulse
    {
        public int target = YAPI.INVALID_INT;
        public int ms = YAPI.INVALID_INT;
        public int moving = YAPI.INVALID_UINT;
    }

    public const int STATE_A = 0;
    public const int STATE_B = 1;
    public const int STATE_INVALID = -1;
    public const int STATEATPOWERON_UNCHANGED = 0;
    public const int STATEATPOWERON_A = 1;
    public const int STATEATPOWERON_B = 2;
    public const int STATEATPOWERON_INVALID = -1;
    public const long MAXTIMEONSTATEA_INVALID = YAPI.INVALID_LONG;
    public const long MAXTIMEONSTATEB_INVALID = YAPI.INVALID_LONG;
    public const int OUTPUT_OFF = 0;
    public const int OUTPUT_ON = 1;
    public const int OUTPUT_INVALID = -1;
    public const long PULSETIMER_INVALID = YAPI.INVALID_LONG;
    public const long COUNTDOWN_INVALID = YAPI.INVALID_LONG;
    public const int AUTOSTART_OFF = 0;
    public const int AUTOSTART_ON = 1;
    public const int AUTOSTART_INVALID = -1;
    public const int RUNNING_OFF = 0;
    public const int RUNNING_ON = 1;
    public const int RUNNING_INVALID = -1;
    public const long TRIGGERDELAY_INVALID = YAPI.INVALID_LONG;
    public const long TRIGGERDURATION_INVALID = YAPI.INVALID_LONG;
    public static readonly YWatchdogDelayedPulse DELAYEDPULSETIMER_INVALID = null;
    protected int _state = STATE_INVALID;
    protected int _stateAtPowerOn = STATEATPOWERON_INVALID;
    protected long _maxTimeOnStateA = MAXTIMEONSTATEA_INVALID;
    protected long _maxTimeOnStateB = MAXTIMEONSTATEB_INVALID;
    protected int _output = OUTPUT_INVALID;
    protected long _pulseTimer = PULSETIMER_INVALID;
    protected YWatchdogDelayedPulse _delayedPulseTimer = new YWatchdogDelayedPulse();
    protected long _countdown = COUNTDOWN_INVALID;
    protected int _autoStart = AUTOSTART_INVALID;
    protected int _running = RUNNING_INVALID;
    protected long _triggerDelay = TRIGGERDELAY_INVALID;
    protected long _triggerDuration = TRIGGERDURATION_INVALID;
    protected ValueCallback _valueCallbackWatchdog = null;
    protected int _firm = 0;
    //--- (end of YWatchdog definitions)

    public YWatchdog(string func)
        : base(func)
    {
        _className = "Watchdog";
        //--- (YWatchdog attributes initialization)
        //--- (end of YWatchdog attributes initialization)
    }

    //--- (YWatchdog implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("state"))
        {
            _state = json_val.getInt("state") > 0 ? 1 : 0;
        }
        if (json_val.has("stateAtPowerOn"))
        {
            _stateAtPowerOn = json_val.getInt("stateAtPowerOn");
        }
        if (json_val.has("maxTimeOnStateA"))
        {
            _maxTimeOnStateA = json_val.getLong("maxTimeOnStateA");
        }
        if (json_val.has("maxTimeOnStateB"))
        {
            _maxTimeOnStateB = json_val.getLong("maxTimeOnStateB");
        }
        if (json_val.has("output"))
        {
            _output = json_val.getInt("output") > 0 ? 1 : 0;
        }
        if (json_val.has("pulseTimer"))
        {
            _pulseTimer = json_val.getLong("pulseTimer");
        }
        if (json_val.has("delayedPulseTimer"))
        {
            YAPI.YJSONObject subjson = json_val.getYJSONObject("delayedPulseTimer");
            if (subjson.has("moving")) {
                _delayedPulseTimer.moving = subjson.getInt("moving");
            }
            if (subjson.has("target")) {
                _delayedPulseTimer.target = subjson.getInt("target");
            }
            if (subjson.has("ms")) {
                _delayedPulseTimer.ms = subjson.getInt("ms");
            }
        }
        if (json_val.has("countdown"))
        {
            _countdown = json_val.getLong("countdown");
        }
        if (json_val.has("autoStart"))
        {
            _autoStart = json_val.getInt("autoStart") > 0 ? 1 : 0;
        }
        if (json_val.has("running"))
        {
            _running = json_val.getInt("running") > 0 ? 1 : 0;
        }
        if (json_val.has("triggerDelay"))
        {
            _triggerDelay = json_val.getLong("triggerDelay");
        }
        if (json_val.has("triggerDuration"))
        {
            _triggerDuration = json_val.getLong("triggerDuration");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the state of the watchdog (A for the idle position, B for the active position).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YWatchdog.STATE_A</c> or <c>YWatchdog.STATE_B</c>, according to the state of the watchdog
     *   (A for the idle position, B for the active position)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.STATE_INVALID</c>.
     * </para>
     */
    public int get_state()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return STATE_INVALID;
                }
            }
            res = this._state;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the state of the watchdog (A for the idle position, B for the active position).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YWatchdog.STATE_A</c> or <c>YWatchdog.STATE_B</c>, according to the state of the watchdog
     *   (A for the idle position, B for the active position)
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
    public int set_state(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("state", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the state of the watchdog at device startup (A for the idle position, B for the active position, UNCHANGED for no change).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YWatchdog.STATEATPOWERON_UNCHANGED</c>, <c>YWatchdog.STATEATPOWERON_A</c> and
     *   <c>YWatchdog.STATEATPOWERON_B</c> corresponding to the state of the watchdog at device startup (A
     *   for the idle position, B for the active position, UNCHANGED for no change)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.STATEATPOWERON_INVALID</c>.
     * </para>
     */
    public int get_stateAtPowerOn()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return STATEATPOWERON_INVALID;
                }
            }
            res = this._stateAtPowerOn;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the state of the watchdog at device startup (A for the idle position,
     *   B for the active position, UNCHANGED for no modification).
     * <para>
     *   Remember to call the matching module <c>saveToFlash()</c>
     *   method, otherwise this call will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YWatchdog.STATEATPOWERON_UNCHANGED</c>, <c>YWatchdog.STATEATPOWERON_A</c> and
     *   <c>YWatchdog.STATEATPOWERON_B</c> corresponding to the state of the watchdog at device startup (A
     *   for the idle position,
     *   B for the active position, UNCHANGED for no modification)
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
    public int set_stateAtPowerOn(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("stateAtPowerOn", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state
     *   A before automatically switching back in to B state.
     * <para>
     *   Zero means no time limit.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state
     *   A before automatically switching back in to B state
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.MAXTIMEONSTATEA_INVALID</c>.
     * </para>
     */
    public long get_maxTimeOnStateA()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MAXTIMEONSTATEA_INVALID;
                }
            }
            res = this._maxTimeOnStateA;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state A
     *   before automatically switching back in to B state.
     * <para>
     *   Use zero for no time limit.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state A
     *   before automatically switching back in to B state
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
    public int set_maxTimeOnStateA(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("maxTimeOnStateA", rest_val);
        }
    }

    /**
     * <summary>
     *   Retourne the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state B
     *   before automatically switching back in to A state.
     * <para>
     *   Zero means no time limit.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.MAXTIMEONSTATEB_INVALID</c>.
     * </para>
     */
    public long get_maxTimeOnStateB()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MAXTIMEONSTATEB_INVALID;
                }
            }
            res = this._maxTimeOnStateB;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state B before
     *   automatically switching back in to A state.
     * <para>
     *   Use zero for no time limit.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state B before
     *   automatically switching back in to A state
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
    public int set_maxTimeOnStateB(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("maxTimeOnStateB", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the output state of the watchdog, when used as a simple switch (single throw).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YWatchdog.OUTPUT_OFF</c> or <c>YWatchdog.OUTPUT_ON</c>, according to the output state of
     *   the watchdog, when used as a simple switch (single throw)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.OUTPUT_INVALID</c>.
     * </para>
     */
    public int get_output()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return OUTPUT_INVALID;
                }
            }
            res = this._output;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the output state of the watchdog, when used as a simple switch (single throw).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YWatchdog.OUTPUT_OFF</c> or <c>YWatchdog.OUTPUT_ON</c>, according to the output state of
     *   the watchdog, when used as a simple switch (single throw)
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
    public int set_output(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("output", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the number of milliseconds remaining before the watchdog is returned to idle position
     *   (state A), during a measured pulse generation.
     * <para>
     *   When there is no ongoing pulse, returns zero.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of milliseconds remaining before the watchdog is returned to
     *   idle position
     *   (state A), during a measured pulse generation
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.PULSETIMER_INVALID</c>.
     * </para>
     */
    public long get_pulseTimer()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PULSETIMER_INVALID;
                }
            }
            res = this._pulseTimer;
        }
        return res;
    }

    public int set_pulseTimer(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("pulseTimer", rest_val);
        }
    }

    /**
     * <summary>
     *   Sets the relay to output B (active) for a specified duration, then brings it
     *   automatically back to output A (idle state).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="ms_duration">
     *   pulse duration, in milliseconds
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
    public int pulse(int ms_duration)
    {
        string rest_val;
        rest_val = (ms_duration).ToString();
        return _setAttr("pulseTimer", rest_val);
    }

    public YWatchdogDelayedPulse get_delayedPulseTimer()
    {
        YWatchdogDelayedPulse res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return DELAYEDPULSETIMER_INVALID;
                }
            }
            res = this._delayedPulseTimer;
        }
        return res;
    }

    public int set_delayedPulseTimer(YWatchdogDelayedPulse newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval.target).ToString()+":"+(newval.ms).ToString();
            return _setAttr("delayedPulseTimer", rest_val);
        }
    }

    /**
     * <summary>
     *   Schedules a pulse.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="ms_delay">
     *   waiting time before the pulse, in milliseconds
     * </param>
     * <param name="ms_duration">
     *   pulse duration, in milliseconds
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
    public int delayedPulse(int ms_delay,int ms_duration)
    {
        string rest_val;
        rest_val = (ms_delay).ToString()+":"+(ms_duration).ToString();
        return _setAttr("delayedPulseTimer", rest_val);
    }

    /**
     * <summary>
     *   Returns the number of milliseconds remaining before a pulse (delayedPulse() call)
     *   When there is no scheduled pulse, returns zero.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of milliseconds remaining before a pulse (delayedPulse() call)
     *   When there is no scheduled pulse, returns zero
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.COUNTDOWN_INVALID</c>.
     * </para>
     */
    public long get_countdown()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return COUNTDOWN_INVALID;
                }
            }
            res = this._countdown;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the watchdog running state at module power on.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YWatchdog.AUTOSTART_OFF</c> or <c>YWatchdog.AUTOSTART_ON</c>, according to the watchdog
     *   running state at module power on
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.AUTOSTART_INVALID</c>.
     * </para>
     */
    public int get_autoStart()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return AUTOSTART_INVALID;
                }
            }
            res = this._autoStart;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the watchdog running state at module power on.
     * <para>
     *   Remember to call the
     *   <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YWatchdog.AUTOSTART_OFF</c> or <c>YWatchdog.AUTOSTART_ON</c>, according to the watchdog
     *   running state at module power on
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
     *   Returns the watchdog running state.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YWatchdog.RUNNING_OFF</c> or <c>YWatchdog.RUNNING_ON</c>, according to the watchdog running state
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.RUNNING_INVALID</c>.
     * </para>
     */
    public int get_running()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RUNNING_INVALID;
                }
            }
            res = this._running;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the running state of the watchdog.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YWatchdog.RUNNING_OFF</c> or <c>YWatchdog.RUNNING_ON</c>, according to the running state
     *   of the watchdog
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
    public int set_running(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("running", rest_val);
        }
    }

    /**
     * <summary>
     *   Resets the watchdog.
     * <para>
     *   When the watchdog is running, this function
     *   must be called on a regular basis to prevent the watchdog to
     *   trigger
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int resetWatchdog()
    {
        string rest_val;
        rest_val = "1";
        return _setAttr("running", rest_val);
    }

    /**
     * <summary>
     *   Returns  the waiting duration before a reset is automatically triggered by the watchdog, in milliseconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to  the waiting duration before a reset is automatically triggered by the
     *   watchdog, in milliseconds
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.TRIGGERDELAY_INVALID</c>.
     * </para>
     */
    public long get_triggerDelay()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return TRIGGERDELAY_INVALID;
                }
            }
            res = this._triggerDelay;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the waiting delay before a reset is triggered by the watchdog,
     *   in milliseconds.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the waiting delay before a reset is triggered by the watchdog,
     *   in milliseconds
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
    public int set_triggerDelay(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("triggerDelay", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the duration of resets caused by the watchdog, in milliseconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the duration of resets caused by the watchdog, in milliseconds
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWatchdog.TRIGGERDURATION_INVALID</c>.
     * </para>
     */
    public long get_triggerDuration()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return TRIGGERDURATION_INVALID;
                }
            }
            res = this._triggerDuration;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the duration of resets caused by the watchdog, in milliseconds.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the duration of resets caused by the watchdog, in milliseconds
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
    public int set_triggerDuration(long newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("triggerDuration", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a watchdog for a given identifier.
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
     *   This function does not require that the watchdog is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YWatchdog.isOnline()</c> to test if the watchdog is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a watchdog by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the watchdog
     * </param>
     * <returns>
     *   a <c>YWatchdog</c> object allowing you to drive the watchdog.
     * </returns>
     */
    public static YWatchdog FindWatchdog(string func)
    {
        YWatchdog obj;
        lock (YAPI.globalLock) {
            obj = (YWatchdog) YFunction._FindFromCache("Watchdog", func);
            if (obj == null) {
                obj = new YWatchdog(func);
                YFunction._AddToCache("Watchdog", func, obj);
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
        this._valueCallbackWatchdog = callback;
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
        if (this._valueCallbackWatchdog != null) {
            this._valueCallbackWatchdog(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Switch the relay to the opposite state.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int toggle()
    {
        int sta;
        string fw;
        YModule mo;
        if (this._firm == 0) {
            mo = this.get_module();
            fw = mo.get_firmwareRelease();
            if (fw == YModule.FIRMWARERELEASE_INVALID) {
                return STATE_INVALID;
            }
            this._firm = YAPI._atoi(fw);
        }
        if (this._firm < 34921) {
            sta = this.get_state();
            if (sta == STATE_INVALID) {
                return STATE_INVALID;
            }
            if (sta == STATE_B) {
                this.set_state(STATE_A);
            } else {
                this.set_state(STATE_B);
            }
            return YAPI.SUCCESS;
        } else {
            return this._setAttr("state","X");
        }
    }

    /**
     * <summary>
     *   Continues the enumeration of watchdog started using <c>yFirstWatchdog()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned watchdog order.
     *   If you want to find a specific a watchdog, use <c>Watchdog.findWatchdog()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YWatchdog</c> object, corresponding to
     *   a watchdog currently online, or a <c>null</c> pointer
     *   if there are no more watchdog to enumerate.
     * </returns>
     */
    public YWatchdog nextWatchdog()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindWatchdog(hwid);
    }

    //--- (end of YWatchdog implementation)

    //--- (YWatchdog functions)

    /**
     * <summary>
     *   Starts the enumeration of watchdog currently accessible.
     * <para>
     *   Use the method <c>YWatchdog.nextWatchdog()</c> to iterate on
     *   next watchdog.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YWatchdog</c> object, corresponding to
     *   the first watchdog currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YWatchdog FirstWatchdog()
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
        err = YAPI.apiGetFunctionsByClass("Watchdog", 0, p, size, ref neededsize, ref errmsg);
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
        return FindWatchdog(serial + "." + funcId);
    }



    //--- (end of YWatchdog functions)
}
#pragma warning restore 1591
