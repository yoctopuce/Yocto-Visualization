/*********************************************************************
 *
 *  $Id: yocto_buzzer.cs 36554 2019-07-29 12:21:31Z mvuilleu $
 *
 *  Implements yFindBuzzer(), the high-level API for Buzzer functions
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
    //--- (YBuzzer return codes)
    //--- (end of YBuzzer return codes)
//--- (YBuzzer dlldef)
//--- (end of YBuzzer dlldef)
//--- (YBuzzer yapiwrapper)
//--- (end of YBuzzer yapiwrapper)
//--- (YBuzzer class start)
/**
 * <summary>
 *   The Yoctopuce application programming interface allows you to
 *   choose the frequency and volume at which the buzzer must sound.
 * <para>
 *   You can also pre-program a play sequence.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YBuzzer : YFunction
{
//--- (end of YBuzzer class start)
    //--- (YBuzzer definitions)
    public new delegate void ValueCallback(YBuzzer func, string value);
    public new delegate void TimedReportCallback(YBuzzer func, YMeasure measure);

    public const double FREQUENCY_INVALID = YAPI.INVALID_DOUBLE;
    public const int VOLUME_INVALID = YAPI.INVALID_UINT;
    public const int PLAYSEQSIZE_INVALID = YAPI.INVALID_UINT;
    public const int PLAYSEQMAXSIZE_INVALID = YAPI.INVALID_UINT;
    public const int PLAYSEQSIGNATURE_INVALID = YAPI.INVALID_UINT;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected double _frequency = FREQUENCY_INVALID;
    protected int _volume = VOLUME_INVALID;
    protected int _playSeqSize = PLAYSEQSIZE_INVALID;
    protected int _playSeqMaxSize = PLAYSEQMAXSIZE_INVALID;
    protected int _playSeqSignature = PLAYSEQSIGNATURE_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackBuzzer = null;
    //--- (end of YBuzzer definitions)

    public YBuzzer(string func)
        : base(func)
    {
        _className = "Buzzer";
        //--- (YBuzzer attributes initialization)
        //--- (end of YBuzzer attributes initialization)
    }

    //--- (YBuzzer implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("frequency"))
        {
            _frequency = Math.Round(json_val.getDouble("frequency") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("volume"))
        {
            _volume = json_val.getInt("volume");
        }
        if (json_val.has("playSeqSize"))
        {
            _playSeqSize = json_val.getInt("playSeqSize");
        }
        if (json_val.has("playSeqMaxSize"))
        {
            _playSeqMaxSize = json_val.getInt("playSeqMaxSize");
        }
        if (json_val.has("playSeqSignature"))
        {
            _playSeqSignature = json_val.getInt("playSeqSignature");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Changes the frequency of the signal sent to the buzzer.
     * <para>
     *   A zero value stops the buzzer.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the frequency of the signal sent to the buzzer
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
    public int set_frequency(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("frequency", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the  frequency of the signal sent to the buzzer/speaker.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the  frequency of the signal sent to the buzzer/speaker
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBuzzer.FREQUENCY_INVALID</c>.
     * </para>
     */
    public double get_frequency()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return FREQUENCY_INVALID;
                }
            }
            res = this._frequency;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the volume of the signal sent to the buzzer/speaker.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the volume of the signal sent to the buzzer/speaker
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBuzzer.VOLUME_INVALID</c>.
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
     *   Changes the volume of the signal sent to the buzzer/speaker.
     * <para>
     *   Remember to call the
     *   <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the volume of the signal sent to the buzzer/speaker
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
     *   Returns the current length of the playing sequence.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current length of the playing sequence
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBuzzer.PLAYSEQSIZE_INVALID</c>.
     * </para>
     */
    public int get_playSeqSize()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PLAYSEQSIZE_INVALID;
                }
            }
            res = this._playSeqSize;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the maximum length of the playing sequence.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximum length of the playing sequence
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBuzzer.PLAYSEQMAXSIZE_INVALID</c>.
     * </para>
     */
    public int get_playSeqMaxSize()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PLAYSEQMAXSIZE_INVALID;
                }
            }
            res = this._playSeqMaxSize;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the playing sequence signature.
     * <para>
     *   As playing
     *   sequences cannot be read from the device, this can be used
     *   to detect if a specific playing sequence is already
     *   programmed.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the playing sequence signature
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YBuzzer.PLAYSEQSIGNATURE_INVALID</c>.
     * </para>
     */
    public int get_playSeqSignature()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PLAYSEQSIGNATURE_INVALID;
                }
            }
            res = this._playSeqSignature;
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
     *   Retrieves a buzzer for a given identifier.
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
     *   This function does not require that the buzzer is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YBuzzer.isOnline()</c> to test if the buzzer is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a buzzer by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the buzzer
     * </param>
     * <returns>
     *   a <c>YBuzzer</c> object allowing you to drive the buzzer.
     * </returns>
     */
    public static YBuzzer FindBuzzer(string func)
    {
        YBuzzer obj;
        lock (YAPI.globalLock) {
            obj = (YBuzzer) YFunction._FindFromCache("Buzzer", func);
            if (obj == null) {
                obj = new YBuzzer(func);
                YFunction._AddToCache("Buzzer", func, obj);
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
        this._valueCallbackBuzzer = callback;
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
        if (this._valueCallbackBuzzer != null) {
            this._valueCallbackBuzzer(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    public virtual int sendCommand(string command)
    {
        return this.set_command(command);
    }

    /**
     * <summary>
     *   Adds a new frequency transition to the playing sequence.
     * <para>
     * </para>
     * </summary>
     * <param name="freq">
     *   desired frequency when the transition is completed, in Hz
     * </param>
     * <param name="msDelay">
     *   duration of the frequency transition, in milliseconds.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int addFreqMoveToPlaySeq(int freq, int msDelay)
    {
        return this.sendCommand("A"+Convert.ToString(freq)+","+Convert.ToString(msDelay));
    }

    /**
     * <summary>
     *   Adds a pulse to the playing sequence.
     * <para>
     * </para>
     * </summary>
     * <param name="freq">
     *   pulse frequency, in Hz
     * </param>
     * <param name="msDuration">
     *   pulse duration, in milliseconds.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int addPulseToPlaySeq(int freq, int msDuration)
    {
        return this.sendCommand("B"+Convert.ToString(freq)+","+Convert.ToString(msDuration));
    }

    /**
     * <summary>
     *   Adds a new volume transition to the playing sequence.
     * <para>
     *   Frequency stays untouched:
     *   if frequency is at zero, the transition has no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="volume">
     *   desired volume when the transition is completed, as a percentage.
     * </param>
     * <param name="msDuration">
     *   duration of the volume transition, in milliseconds.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int addVolMoveToPlaySeq(int volume, int msDuration)
    {
        return this.sendCommand("C"+Convert.ToString(volume)+","+Convert.ToString(msDuration));
    }

    /**
     * <summary>
     *   Adds notes to the playing sequence.
     * <para>
     *   Notes are provided as text words, separated by
     *   spaces. The pitch is specified using the usual letter from A to G. The duration is
     *   specified as the divisor of a whole note: 4 for a fourth, 8 for an eight note, etc.
     *   Some modifiers are supported: <c>#</c> and <c>b</c> to alter a note pitch,
     *   <c>'</c> and <c>,</c> to move to the upper/lower octave, <c>.</c> to enlarge
     *   the note duration.
     * </para>
     * </summary>
     * <param name="notes">
     *   notes to be played, as a text string.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int addNotesToPlaySeq(string notes)
    {
        int tempo;
        int prevPitch;
        int prevDuration;
        int prevFreq;
        int note;
        int num;
        int typ;
        byte[] ascNotes;
        int notesLen;
        int i;
        int ch;
        int dNote;
        int pitch;
        int freq;
        int ms;
        int ms16;
        int rest;
        tempo = 100;
        prevPitch = 3;
        prevDuration = 4;
        prevFreq = 110;
        note = -99;
        num = 0;
        typ = 3;
        ascNotes = YAPI.DefaultEncoding.GetBytes(notes);
        notesLen = (ascNotes).Length;
        i = 0;
        while (i < notesLen) {
            ch = ascNotes[i];
            // A (note))
            if (ch == 65) {
                note = 0;
            }
            // B (note)
            if (ch == 66) {
                note = 2;
            }
            // C (note)
            if (ch == 67) {
                note = 3;
            }
            // D (note)
            if (ch == 68) {
                note = 5;
            }
            // E (note)
            if (ch == 69) {
                note = 7;
            }
            // F (note)
            if (ch == 70) {
                note = 8;
            }
            // G (note)
            if (ch == 71) {
                note = 10;
            }
            // '#' (sharp modifier)
            if (ch == 35) {
                note = note + 1;
            }
            // 'b' (flat modifier)
            if (ch == 98) {
                note = note - 1;
            }
            // ' (octave up)
            if (ch == 39) {
                prevPitch = prevPitch + 12;
            }
            // , (octave down)
            if (ch == 44) {
                prevPitch = prevPitch - 12;
            }
            // R (rest)
            if (ch == 82) {
                typ = 0;
            }
            // ! (staccato modifier)
            if (ch == 33) {
                typ = 1;
            }
            // ^ (short modifier)
            if (ch == 94) {
                typ = 2;
            }
            // _ (legato modifier)
            if (ch == 95) {
                typ = 4;
            }
            // - (glissando modifier)
            if (ch == 45) {
                typ = 5;
            }
            // % (tempo change)
            if ((ch == 37) && (num > 0)) {
                tempo = num;
                num = 0;
            }
            if ((ch >= 48) && (ch <= 57)) {
                // 0-9 (number)
                num = (num * 10) + (ch - 48);
            }
            if (ch == 46) {
                // . (duration modifier)
                num = ((num * 2) / (3));
            }
            if (((ch == 32) || (i+1 == notesLen)) && ((note > -99) || (typ != 3))) {
                if (num == 0) {
                    num = prevDuration;
                } else {
                    prevDuration = num;
                }
                ms = (int) Math.Round(320000.0 / (tempo * num));
                if (typ == 0) {
                    this.addPulseToPlaySeq(0, ms);
                } else {
                    dNote = note - (((prevPitch) % (12)));
                    if (dNote > 6) {
                        dNote = dNote - 12;
                    }
                    if (dNote <= -6) {
                        dNote = dNote + 12;
                    }
                    pitch = prevPitch + dNote;
                    freq = (int) Math.Round(440 * Math.Exp(pitch * 0.05776226504666));
                    ms16 = ((ms) >> (4));
                    rest = 0;
                    if (typ == 3) {
                        rest = 2 * ms16;
                    }
                    if (typ == 2) {
                        rest = 8 * ms16;
                    }
                    if (typ == 1) {
                        rest = 12 * ms16;
                    }
                    if (typ == 5) {
                        this.addPulseToPlaySeq(prevFreq, ms16);
                        this.addFreqMoveToPlaySeq(freq, 8 * ms16);
                        this.addPulseToPlaySeq(freq, ms - 9 * ms16);
                    } else {
                        this.addPulseToPlaySeq(freq, ms - rest);
                        if (rest > 0) {
                            this.addPulseToPlaySeq(0, rest);
                        }
                    }
                    prevFreq = freq;
                    prevPitch = pitch;
                }
                note = -99;
                num = 0;
                typ = 3;
            }
            i = i + 1;
        }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Starts the preprogrammed playing sequence.
     * <para>
     *   The sequence
     *   runs in loop until it is stopped by stopPlaySeq or an explicit
     *   change. To play the sequence only once, use <c>oncePlaySeq()</c>.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int startPlaySeq()
    {
        return this.sendCommand("S");
    }

    /**
     * <summary>
     *   Stops the preprogrammed playing sequence and sets the frequency to zero.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int stopPlaySeq()
    {
        return this.sendCommand("X");
    }

    /**
     * <summary>
     *   Resets the preprogrammed playing sequence and sets the frequency to zero.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int resetPlaySeq()
    {
        return this.sendCommand("Z");
    }

    /**
     * <summary>
     *   Starts the preprogrammed playing sequence and run it once only.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int oncePlaySeq()
    {
        return this.sendCommand("s");
    }

    /**
     * <summary>
     *   Saves the preprogrammed playing sequence to flash memory.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int savePlaySeq()
    {
        return this.sendCommand("W");
    }

    /**
     * <summary>
     *   Reloads the preprogrammed playing sequence from the flash memory.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int reloadPlaySeq()
    {
        return this.sendCommand("R");
    }

    /**
     * <summary>
     *   Activates the buzzer for a short duration.
     * <para>
     * </para>
     * </summary>
     * <param name="frequency">
     *   pulse frequency, in hertz
     * </param>
     * <param name="duration">
     *   pulse duration in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int pulse(int frequency, int duration)
    {
        return this.set_command("P"+Convert.ToString(frequency)+","+Convert.ToString(duration));
    }

    /**
     * <summary>
     *   Makes the buzzer frequency change over a period of time.
     * <para>
     * </para>
     * </summary>
     * <param name="frequency">
     *   frequency to reach, in hertz. A frequency under 25Hz stops the buzzer.
     * </param>
     * <param name="duration">
     *   pulse duration in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int freqMove(int frequency, int duration)
    {
        return this.set_command("F"+Convert.ToString(frequency)+","+Convert.ToString(duration));
    }

    /**
     * <summary>
     *   Makes the buzzer volume change over a period of time, frequency  stays untouched.
     * <para>
     * </para>
     * </summary>
     * <param name="volume">
     *   volume to reach in %
     * </param>
     * <param name="duration">
     *   change duration in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int volumeMove(int volume, int duration)
    {
        return this.set_command("V"+Convert.ToString(volume)+","+Convert.ToString(duration));
    }

    /**
     * <summary>
     *   Immediately play a note sequence.
     * <para>
     *   Notes are provided as text words, separated by
     *   spaces. The pitch is specified using the usual letter from A to G. The duration is
     *   specified as the divisor of a whole note: 4 for a fourth, 8 for an eight note, etc.
     *   Some modifiers are supported: <c>#</c> and <c>b</c> to alter a note pitch,
     *   <c>'</c> and <c>,</c> to move to the upper/lower octave, <c>.</c> to enlarge
     *   the note duration.
     * </para>
     * </summary>
     * <param name="notes">
     *   notes to be played, as a text string.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public virtual int playNotes(string notes)
    {
        this.resetPlaySeq();
        this.addNotesToPlaySeq(notes);
        return this.oncePlaySeq();
    }

    /**
     * <summary>
     *   Continues the enumeration of buzzers started using <c>yFirstBuzzer()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned buzzers order.
     *   If you want to find a specific a buzzer, use <c>Buzzer.findBuzzer()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YBuzzer</c> object, corresponding to
     *   a buzzer currently online, or a <c>null</c> pointer
     *   if there are no more buzzers to enumerate.
     * </returns>
     */
    public YBuzzer nextBuzzer()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindBuzzer(hwid);
    }

    //--- (end of YBuzzer implementation)

    //--- (YBuzzer functions)

    /**
     * <summary>
     *   Starts the enumeration of buzzers currently accessible.
     * <para>
     *   Use the method <c>YBuzzer.nextBuzzer()</c> to iterate on
     *   next buzzers.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YBuzzer</c> object, corresponding to
     *   the first buzzer currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YBuzzer FirstBuzzer()
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
        err = YAPI.apiGetFunctionsByClass("Buzzer", 0, p, size, ref neededsize, ref errmsg);
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
        return FindBuzzer(serial + "." + funcId);
    }



    //--- (end of YBuzzer functions)
}
#pragma warning restore 1591
