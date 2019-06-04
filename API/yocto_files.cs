/*********************************************************************
 *
 * $Id: yocto_files.cs 34989 2019-04-05 13:41:16Z seb $
 *
 * Implements yFindFiles(), the high-level API for Files functions
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

//--- (generated code: YFileRecord class start)
/**
 * <summary>
 *   YFileRecord objects are used to describe a file that is stored on a Yoctopuce device.
 * <para>
 *   These objects are used in particular in conjunction with the YFiles class.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YFileRecord
{
//--- (end of generated code: YFileRecord class start)
    //--- (generated code: YFileRecord definitions)

    protected string _name;
    protected int _size = 0;
    protected int _crc = 0;
    //--- (end of generated code: YFileRecord definitions)

    public YFileRecord(string data)
    {
        //--- (generated code: YFileRecord attributes initialization)
        //--- (end of generated code: YFileRecord attributes initialization)
        YAPI.YJSONObject p  = new YAPI.YJSONObject(data);
        p.parse();
        this._name = p.getString("name");
        this._size = p.getInt("size");
        this._crc = p.getInt("crc");
    }

    //--- (generated code: YFileRecord implementation)


    /**
     * <summary>
     *   Returns the name of the file.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string with the name of the file.
     * </returns>
     */
    public virtual string get_name()
    {
        return this._name;
    }

    /**
     * <summary>
     *   Returns the size of the file in bytes.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the size of the file.
     * </returns>
     */
    public virtual int get_size()
    {
        return this._size;
    }

    /**
     * <summary>
     *   Returns the 32-bit CRC of the file content.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the 32-bit CRC of the file content.
     * </returns>
     */
    public virtual int get_crc()
    {
        return this._crc;
    }

    //--- (end of generated code: YFileRecord implementation)
}



//--- (generated code: YFiles class start)
/**
 * <summary>
 *   The filesystem interface makes it possible to store files
 *   on some devices, for instance to design a custom web UI
 *   (for networked devices) or to add fonts (on display
 *   devices).
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YFiles : YFunction
{
//--- (end of generated code: YFiles class start)
    //--- (generated code: YFiles definitions)
    public new delegate void ValueCallback(YFiles func, string value);
    public new delegate void TimedReportCallback(YFiles func, YMeasure measure);

    public const int FILESCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int FREESPACE_INVALID = YAPI.INVALID_UINT;
    protected int _filesCount = FILESCOUNT_INVALID;
    protected int _freeSpace = FREESPACE_INVALID;
    protected ValueCallback _valueCallbackFiles = null;
    //--- (end of generated code: YFiles definitions)

    public YFiles(string func)
        : base(func)
    {
        _className = "Files";
        //--- (generated code: YFiles attributes initialization)
        //--- (end of generated code: YFiles attributes initialization)
    }


    //--- (generated code: YFiles implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("filesCount"))
        {
            _filesCount = json_val.getInt("filesCount");
        }
        if (json_val.has("freeSpace"))
        {
            _freeSpace = json_val.getInt("freeSpace");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the number of files currently loaded in the filesystem.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of files currently loaded in the filesystem
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YFiles.FILESCOUNT_INVALID</c>.
     * </para>
     */
    public int get_filesCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return FILESCOUNT_INVALID;
                }
            }
            res = this._filesCount;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the free space for uploading new files to the filesystem, in bytes.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the free space for uploading new files to the filesystem, in bytes
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YFiles.FREESPACE_INVALID</c>.
     * </para>
     */
    public int get_freeSpace()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return FREESPACE_INVALID;
                }
            }
            res = this._freeSpace;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves a filesystem for a given identifier.
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
     *   This function does not require that the filesystem is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YFiles.isOnline()</c> to test if the filesystem is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a filesystem by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the filesystem
     * </param>
     * <returns>
     *   a <c>YFiles</c> object allowing you to drive the filesystem.
     * </returns>
     */
    public static YFiles FindFiles(string func)
    {
        YFiles obj;
        lock (YAPI.globalLock) {
            obj = (YFiles) YFunction._FindFromCache("Files", func);
            if (obj == null) {
                obj = new YFiles(func);
                YFunction._AddToCache("Files", func, obj);
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
        this._valueCallbackFiles = callback;
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
        if (this._valueCallbackFiles != null) {
            this._valueCallbackFiles(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    public virtual byte[] sendCommand(string command)
    {
        string url;
        url = "files.json?a="+command;

        return this._download(url);
    }

    /**
     * <summary>
     *   Reinitialize the filesystem to its clean, unfragmented, empty state.
     * <para>
     *   All files previously uploaded are permanently lost.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int format_fs()
    {
        byte[] json;
        string res;
        json = this.sendCommand("format");
        res = this._json_get_key(json, "res");
        if (!(res == "ok")) { this._throw( YAPI.IO_ERROR, "format failed"); return YAPI.IO_ERROR; }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Returns a list of YFileRecord objects that describe files currently loaded
     *   in the filesystem.
     * <para>
     * </para>
     * </summary>
     * <param name="pattern">
     *   an optional filter pattern, using star and question marks
     *   as wild cards. When an empty pattern is provided, all file records
     *   are returned.
     * </param>
     * <returns>
     *   a list of <c>YFileRecord</c> objects, containing the file path
     *   and name, byte size and 32-bit CRC of the file content.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list.
     * </para>
     */
    public virtual List<YFileRecord> get_list(string pattern)
    {
        byte[] json;
        List<string> filelist = new List<string>();
        List<YFileRecord> res = new List<YFileRecord>();
        json = this.sendCommand("dir&f="+pattern);
        filelist = this._json_get_array(json);
        res.Clear();
        for (int ii = 0; ii < filelist.Count; ii++) {
            res.Add(new YFileRecord(filelist[ii]));
        }
        return res;
    }

    /**
     * <summary>
     *   Test if a file exist on the filesystem of the module.
     * <para>
     * </para>
     * </summary>
     * <param name="filename">
     *   the file name to test.
     * </param>
     * <returns>
     *   a true if the file exist, false otherwise.
     * </returns>
     * <para>
     *   On failure, throws an exception.
     * </para>
     */
    public virtual bool fileExist(string filename)
    {
        byte[] json;
        List<string> filelist = new List<string>();
        if ((filename).Length == 0) {
            return false;
        }
        json = this.sendCommand("dir&f="+filename);
        filelist = this._json_get_array(json);
        if (filelist.Count > 0) {
            return true;
        }
        return false;
    }

    /**
     * <summary>
     *   Downloads the requested file and returns a binary buffer with its content.
     * <para>
     * </para>
     * </summary>
     * <param name="pathname">
     *   path and name of the file to download
     * </param>
     * <returns>
     *   a binary buffer with the file content
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty content.
     * </para>
     */
    public virtual byte[] download(string pathname)
    {
        return this._download(pathname);
    }

    /**
     * <summary>
     *   Uploads a file to the filesystem, to the specified full path name.
     * <para>
     *   If a file already exists with the same path name, its content is overwritten.
     * </para>
     * </summary>
     * <param name="pathname">
     *   path and name of the new file to create
     * </param>
     * <param name="content">
     *   binary buffer with the content to set
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int upload(string pathname, byte[] content)
    {
        return this._upload(pathname, content);
    }

    /**
     * <summary>
     *   Deletes a file, given by its full path name, from the filesystem.
     * <para>
     *   Because of filesystem fragmentation, deleting a file may not always
     *   free up the whole space used by the file. However, rewriting a file
     *   with the same path name will always reuse any space not freed previously.
     *   If you need to ensure that no space is taken by previously deleted files,
     *   you can use <c>format_fs</c> to fully reinitialize the filesystem.
     * </para>
     * </summary>
     * <param name="pathname">
     *   path and name of the file to remove.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int remove(string pathname)
    {
        byte[] json;
        string res;
        json = this.sendCommand("del&f="+pathname);
        res  = this._json_get_key(json, "res");
        if (!(res == "ok")) { this._throw( YAPI.IO_ERROR, "unable to remove file"); return YAPI.IO_ERROR; }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Continues the enumeration of filesystems started using <c>yFirstFiles()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned filesystems order.
     *   If you want to find a specific a filesystem, use <c>Files.findFiles()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YFiles</c> object, corresponding to
     *   a filesystem currently online, or a <c>null</c> pointer
     *   if there are no more filesystems to enumerate.
     * </returns>
     */
    public YFiles nextFiles()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindFiles(hwid);
    }

    //--- (end of generated code: YFiles implementation)

  //--- (generated code: YFiles functions)

    /**
     * <summary>
     *   Starts the enumeration of filesystems currently accessible.
     * <para>
     *   Use the method <c>YFiles.nextFiles()</c> to iterate on
     *   next filesystems.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YFiles</c> object, corresponding to
     *   the first filesystem currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YFiles FirstFiles()
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
        err = YAPI.apiGetFunctionsByClass("Files", 0, p, size, ref neededsize, ref errmsg);
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
        return FindFiles(serial + "." + funcId);
    }



    //--- (end of generated code: YFiles functions)

}
#pragma warning restore 1591