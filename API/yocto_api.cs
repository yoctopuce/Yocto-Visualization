/*********************************************************************
 *
 * $Id: yocto_api.cs 38510 2019-11-26 15:36:38Z mvuilleu $
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
using System.Runtime.Serialization;
using System.Security;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using YHANDLE = System.Int32;
using YRETCODE = System.Int32;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

// yStrRef of serial number
using YDEV_DESCR = System.Int32;
// yStrRef of serial + (ystrRef of funcId << 16)
using YFUN_DESCR = System.Int32;
// measured in milliseconds
using yTime = System.UInt32;
using yHash = System.Int16;
// (yHash << 1) + [0,1]
using yBlkHdl = System.Char;
using yStrRef = System.Int16;
using yUrlRef = System.Int16;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;

#pragma warning disable 1591
[Serializable]
public class YAPI_Exception : Exception
{
    public YRETCODE errorType;

    public YAPI_Exception(YRETCODE errType, string errMsg) : base(errMsg)
    {
        errorType = errType;
    }

    protected YAPI_Exception(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        if (info == null)
            throw new ArgumentNullException("info");

        errorType = info.GetInt32("errorType");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        if (info != null) {
            info.AddValue("errorType", this.errorType);
        }
    }
}


static class NativeMethods
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dllToLoad);
}

[SuppressUnmanagedCodeSecurityAttribute]
internal static class SafeNativeMethods
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct yDeviceSt
    {
        public u16 vendorid;
        public u16 deviceid;
        public u16 devrelease;
        public u16 nbinbterfaces;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_MANUFACTURER_LEN)]
        public string manufacturer;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_PRODUCTNAME_LEN)]
        public string productname;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_SERIAL_LEN)]
        public string serial;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_LOGICAL_LEN)]
        public string logicalname;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_FIRMWARE_LEN)]
        public string firmware;

        public u8 beacon;
        public u8 pad;
    }

    internal static yDeviceSt emptyDeviceSt()
    {
        yDeviceSt infos = default(yDeviceSt);
        infos.vendorid = 0;
        infos.deviceid = 0;
        infos.devrelease = 0;
        infos.nbinbterfaces = 0;
        infos.manufacturer = "";
        infos.productname = "";
        infos.serial = "";
        infos.logicalname = "";
        infos.firmware = "";
        infos.beacon = 0;
        return infos;
    }


    internal const int YIOHDL_SIZE = 8;

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct YIOHDL
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = YIOHDL_SIZE)]
        public u8 raw0;

        public u8 raw1;
        public u8 raw2;
        public u8 raw3;
        public u8 raw4;
        public u8 raw5;
        public u8 raw6;
        public u8 raw7;
    }

    internal enum yDEVICE_PROP
    {
        PROP_VENDORID,
        PROP_DEVICEID,
        PROP_DEVRELEASE,
        PROP_FIRMWARELEVEL,
        PROP_MANUFACTURER,
        PROP_PRODUCTNAME,
        PROP_SERIAL,
        PROP_LOGICALNAME,
        PROP_URL
    }


    internal enum yFACE_STATUS
    {
        YFACE_EMPTY,
        YFACE_RUNNING,
        YFACE_ERROR
    }


    internal enum YAPIDLL_VERSION
    {
        WIN32,
        WIN64,
        MACOS32,
        MACOS64,
        LIN32,
        LIN64,
        LINARMHF,
        LINAARCH64
    }

    internal static YAPIDLL_VERSION _dllVersion = YAPIDLL_VERSION.WIN32;

    private static bool IsMacOS()
    {
        return (Environment.OSVersion.Platform == PlatformID.Unix && Directory.Exists("/Applications") && Directory.Exists("/System") && Directory.Exists("/Users") && Directory.Exists("/Volumes"));
    }

    internal static u16 tryGetAPIVersion(ref IntPtr version, ref IntPtr dat_)
    {
        Boolean is64 = IntPtr.Size == 8;

        PlatformID platform = Environment.OSVersion.Platform;
        if (platform == PlatformID.MacOSX) {
            if (is64) {
                _dllVersion = YAPIDLL_VERSION.MACOS64;
            } else {
                _dllVersion = YAPIDLL_VERSION.MACOS32;
            }
        } else if (platform == PlatformID.Unix) {
            if (IsMacOS()) {
                if (is64) {
                    _dllVersion = YAPIDLL_VERSION.MACOS64;
                } else {
                    _dllVersion = YAPIDLL_VERSION.MACOS32;
                }
            } else {
                if (is64) {
                    _dllVersion = YAPIDLL_VERSION.LIN64;
                } else {
                    _dllVersion = YAPIDLL_VERSION.LIN32;
                }
            }
        } else {
            if (is64) {
                _dllVersion = YAPIDLL_VERSION.WIN64;
            } else {
                _dllVersion = YAPIDLL_VERSION.WIN32;
            }
        }

        debugDll("Detected platform is " + _dllVersion.ToString());

        System.Reflection.Assembly ass = Assembly.GetEntryAssembly();
        if (ass != null) {
            string currentDirectory = Directory.GetCurrentDirectory();
            debugDll("Current Dir is " + currentDirectory);
            string assemblyDir = Path.GetDirectoryName(ass.Location);
            debugDll("Assembly Dir is " + assemblyDir);
            if (currentDirectory != assemblyDir) {
                string dll_path;
                switch (_dllVersion) {
                    default:
                    case YAPIDLL_VERSION.WIN32:
                        dll_path = assemblyDir + "\\yapi.dll";
                        break;
                    case YAPIDLL_VERSION.WIN64:
                        dll_path = assemblyDir + "\\amd64\\yapi.dll";
                        break;
                    case YAPIDLL_VERSION.MACOS32:
                        dll_path = assemblyDir + "\\libyapi32";
                        break;
                    case YAPIDLL_VERSION.MACOS64:
                        dll_path = assemblyDir + "\\libyapi64";
                        break;
                    case YAPIDLL_VERSION.LIN64:
                        dll_path = assemblyDir + "\\libyapi-amd64";
                        break;
                    case YAPIDLL_VERSION.LIN32:
                        dll_path = assemblyDir + "\\libyapi-i386";
                        break;
                    case YAPIDLL_VERSION.LINARMHF:
                        dll_path = assemblyDir + "\\libyapi-armhf";
                        break;
                    case YAPIDLL_VERSION.LINAARCH64:
                        dll_path = assemblyDir + "\\libyapi-aarch64";
                        break;
                }
                debugDll("try to load library using assembly directory("+dll_path+")");
                try {
                    IntPtr loadLibrary = NativeMethods.LoadLibrary(dll_path);
                    if (loadLibrary == IntPtr.Zero) {
                        debugDll("Unable to preload dll with :" + dll_path);
                    } else {
                        debugDll("YAPI preloaded from " + dll_path);
                    }
                } catch (System.EntryPointNotFoundException ex) {
                    debugDll("Entry point not found:"+ex.Message);
                }
            }
        }

        bool can_retry = true;
        do {
            try {
                return _yapiGetAPIVersion(ref version, ref dat_);
            } catch (System.DllNotFoundException ex) {
                debugDll(ex.ToString());
            } catch (System.BadImageFormatException ex) {
                debugDll(ex.ToString());
            }

            switch (_dllVersion) {
                default:
                case YAPIDLL_VERSION.WIN32:
                    throw new System.DllNotFoundException("Unable to load YAPI dynamic library");
                case YAPIDLL_VERSION.WIN64:
                case YAPIDLL_VERSION.MACOS32:
                case YAPIDLL_VERSION.LINARMHF:
                case YAPIDLL_VERSION.LINAARCH64:
                    _dllVersion = YAPIDLL_VERSION.WIN32;
                    break;
                case YAPIDLL_VERSION.MACOS64:
                    _dllVersion = YAPIDLL_VERSION.MACOS32;
                    break;
                case YAPIDLL_VERSION.LIN32:
                    _dllVersion = YAPIDLL_VERSION.LINARMHF;
                    break;
                case YAPIDLL_VERSION.LIN64:
                    _dllVersion = YAPIDLL_VERSION.LINAARCH64;
                    break;
            }

            debugDll("retry with platform " + _dllVersion.ToString());
        } while (can_retry);

        return 0;
    }

    private static void debugDll(string line)
    {
        /*
        DateTime localDate = DateTime.Now;
        string fmt = "[yyyy-MM-dd/hh:mm:ss] ";
        System.IO.File.AppendAllText("c:\\tmp\\debugDllLoad.txt", localDate.ToString(fmt) + line + "\n");
        */
    }


    //--- (generated code: YFunction dlldef)
    [DllImport("yapi", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiInitAPIWIN32(int mode, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiInitAPIWIN64(int mode, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiInitAPIMACOS32(int mode, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiInitAPIMACOS64(int mode, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiInitAPILIN64(int mode, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiInitAPILIN32(int mode, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiInitAPILINARMHF(int mode, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiInitAPILINAARCH64(int mode, StringBuilder errmsg);
    internal static int _yapiInitAPI(int mode, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiInitAPIWIN32(mode, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiInitAPIWIN64(mode, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiInitAPIMACOS32(mode, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiInitAPIMACOS64(mode, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiInitAPILIN64(mode, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiInitAPILIN32(mode, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiInitAPILINARMHF(mode, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiInitAPILINAARCH64(mode, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeAPIWIN32();
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeAPIWIN64();
    [DllImport("libyapi32", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeAPIMACOS32();
    [DllImport("libyapi64", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeAPIMACOS64();
    [DllImport("libyapi-amd64", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeAPILIN64();
    [DllImport("libyapi-i386", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeAPILIN32();
    [DllImport("libyapi-armhf", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeAPILINARMHF();
    [DllImport("libyapi-aarch64", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeAPILINAARCH64();
    internal static void _yapiFreeAPI()
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiFreeAPIWIN32();
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiFreeAPIWIN64();
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiFreeAPIMACOS32();
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiFreeAPIMACOS64();
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiFreeAPILIN64();
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiFreeAPILIN32();
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiFreeAPILINARMHF();
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiFreeAPILINAARCH64();
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiSetTraceFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetTraceFileWIN32(StringBuilder tracefile);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiSetTraceFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetTraceFileWIN64(StringBuilder tracefile);
    [DllImport("libyapi32", EntryPoint = "yapiSetTraceFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetTraceFileMACOS32(StringBuilder tracefile);
    [DllImport("libyapi64", EntryPoint = "yapiSetTraceFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetTraceFileMACOS64(StringBuilder tracefile);
    [DllImport("libyapi-amd64", EntryPoint = "yapiSetTraceFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetTraceFileLIN64(StringBuilder tracefile);
    [DllImport("libyapi-i386", EntryPoint = "yapiSetTraceFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetTraceFileLIN32(StringBuilder tracefile);
    [DllImport("libyapi-armhf", EntryPoint = "yapiSetTraceFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetTraceFileLINARMHF(StringBuilder tracefile);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiSetTraceFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetTraceFileLINAARCH64(StringBuilder tracefile);
    internal static void _yapiSetTraceFile(StringBuilder tracefile)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiSetTraceFileWIN32(tracefile);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiSetTraceFileWIN64(tracefile);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiSetTraceFileMACOS32(tracefile);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiSetTraceFileMACOS64(tracefile);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiSetTraceFileLIN64(tracefile);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiSetTraceFileLIN32(tracefile);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiSetTraceFileLINARMHF(tracefile);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiSetTraceFileLINAARCH64(tracefile);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterLogFunctionWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterLogFunctionWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterLogFunctionMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterLogFunctionMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterLogFunctionLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterLogFunctionLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterLogFunctionLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterLogFunctionLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterLogFunction(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterLogFunctionWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterLogFunctionWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterLogFunctionMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterLogFunctionMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterLogFunctionLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterLogFunctionLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterLogFunctionLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterLogFunctionLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceArrivalCallbackWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceArrivalCallbackWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceArrivalCallbackMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceArrivalCallbackMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceArrivalCallbackLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceArrivalCallbackLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceArrivalCallbackLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceArrivalCallbackLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterDeviceArrivalCallback(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterDeviceArrivalCallbackWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterDeviceArrivalCallbackWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterDeviceArrivalCallbackMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterDeviceArrivalCallbackMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterDeviceArrivalCallbackLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterDeviceArrivalCallbackLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterDeviceArrivalCallbackLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterDeviceArrivalCallbackLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceRemovalCallbackWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceRemovalCallbackWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceRemovalCallbackMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceRemovalCallbackMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceRemovalCallbackLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceRemovalCallbackLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceRemovalCallbackLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceRemovalCallbackLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterDeviceRemovalCallback(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterDeviceRemovalCallbackWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterDeviceRemovalCallbackWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterDeviceRemovalCallbackMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterDeviceRemovalCallbackMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterDeviceRemovalCallbackLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterDeviceRemovalCallbackLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterDeviceRemovalCallbackLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterDeviceRemovalCallbackLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceChangeCallbackWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceChangeCallbackWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceChangeCallbackMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceChangeCallbackMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceChangeCallbackLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceChangeCallbackLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceChangeCallbackLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceChangeCallbackLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterDeviceChangeCallback(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterDeviceChangeCallbackWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterDeviceChangeCallbackWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterDeviceChangeCallbackMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterDeviceChangeCallbackMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterDeviceChangeCallbackLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterDeviceChangeCallbackLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterDeviceChangeCallbackLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterDeviceChangeCallbackLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterDeviceConfigChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceConfigChangeCallbackWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterDeviceConfigChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceConfigChangeCallbackWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterDeviceConfigChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceConfigChangeCallbackMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterDeviceConfigChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceConfigChangeCallbackMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterDeviceConfigChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceConfigChangeCallbackLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterDeviceConfigChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceConfigChangeCallbackLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterDeviceConfigChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceConfigChangeCallbackLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterDeviceConfigChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceConfigChangeCallbackLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterDeviceConfigChangeCallback(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterDeviceConfigChangeCallbackWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterDeviceConfigChangeCallbackWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterDeviceConfigChangeCallbackMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterDeviceConfigChangeCallbackMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterDeviceConfigChangeCallbackLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterDeviceConfigChangeCallbackLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterDeviceConfigChangeCallbackLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterDeviceConfigChangeCallbackLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterFunctionUpdateCallbackWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterFunctionUpdateCallbackWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterFunctionUpdateCallbackMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterFunctionUpdateCallbackMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterFunctionUpdateCallbackLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterFunctionUpdateCallbackLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterFunctionUpdateCallbackLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterFunctionUpdateCallbackLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterFunctionUpdateCallback(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterFunctionUpdateCallbackWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterFunctionUpdateCallbackWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterFunctionUpdateCallbackMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterFunctionUpdateCallbackMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterFunctionUpdateCallbackLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterFunctionUpdateCallbackLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterFunctionUpdateCallbackLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterFunctionUpdateCallbackLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterTimedReportCallbackWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterTimedReportCallbackWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterTimedReportCallbackMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterTimedReportCallbackMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterTimedReportCallbackLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterTimedReportCallbackLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterTimedReportCallbackLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterTimedReportCallbackLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterTimedReportCallback(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterTimedReportCallbackWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterTimedReportCallbackWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterTimedReportCallbackMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterTimedReportCallbackMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterTimedReportCallbackLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterTimedReportCallbackLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterTimedReportCallbackLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterTimedReportCallbackLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockDeviceCallBackWIN32(StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockDeviceCallBackWIN64(StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockDeviceCallBackMACOS32(StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockDeviceCallBackMACOS64(StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockDeviceCallBackLIN64(StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockDeviceCallBackLIN32(StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockDeviceCallBackLINARMHF(StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockDeviceCallBackLINAARCH64(StringBuilder errmsg);
    internal static int _yapiLockDeviceCallBack(StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiLockDeviceCallBackWIN32(errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiLockDeviceCallBackWIN64(errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiLockDeviceCallBackMACOS32(errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiLockDeviceCallBackMACOS64(errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiLockDeviceCallBackLIN64(errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiLockDeviceCallBackLIN32(errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiLockDeviceCallBackLINARMHF(errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiLockDeviceCallBackLINAARCH64(errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockDeviceCallBackWIN32(StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockDeviceCallBackWIN64(StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockDeviceCallBackMACOS32(StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockDeviceCallBackMACOS64(StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockDeviceCallBackLIN64(StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockDeviceCallBackLIN32(StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockDeviceCallBackLINARMHF(StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockDeviceCallBackLINAARCH64(StringBuilder errmsg);
    internal static int _yapiUnlockDeviceCallBack(StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiUnlockDeviceCallBackWIN32(errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiUnlockDeviceCallBackWIN64(errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiUnlockDeviceCallBackMACOS32(errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiUnlockDeviceCallBackMACOS64(errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiUnlockDeviceCallBackLIN64(errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiUnlockDeviceCallBackLIN32(errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiUnlockDeviceCallBackLINARMHF(errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiUnlockDeviceCallBackLINAARCH64(errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockFunctionCallBackWIN32(StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockFunctionCallBackWIN64(StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockFunctionCallBackMACOS32(StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockFunctionCallBackMACOS64(StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockFunctionCallBackLIN64(StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockFunctionCallBackLIN32(StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockFunctionCallBackLINARMHF(StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiLockFunctionCallBackLINAARCH64(StringBuilder errmsg);
    internal static int _yapiLockFunctionCallBack(StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiLockFunctionCallBackWIN32(errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiLockFunctionCallBackWIN64(errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiLockFunctionCallBackMACOS32(errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiLockFunctionCallBackMACOS64(errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiLockFunctionCallBackLIN64(errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiLockFunctionCallBackLIN32(errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiLockFunctionCallBackLINARMHF(errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiLockFunctionCallBackLINAARCH64(errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockFunctionCallBackWIN32(StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockFunctionCallBackWIN64(StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockFunctionCallBackMACOS32(StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockFunctionCallBackMACOS64(StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockFunctionCallBackLIN64(StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockFunctionCallBackLIN32(StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockFunctionCallBackLINARMHF(StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUnlockFunctionCallBackLINAARCH64(StringBuilder errmsg);
    internal static int _yapiUnlockFunctionCallBack(StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiUnlockFunctionCallBackWIN32(errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiUnlockFunctionCallBackWIN64(errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiUnlockFunctionCallBackMACOS32(errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiUnlockFunctionCallBackMACOS64(errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiUnlockFunctionCallBackLIN64(errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiUnlockFunctionCallBackLIN32(errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiUnlockFunctionCallBackLINARMHF(errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiUnlockFunctionCallBackLINAARCH64(errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiRegisterHubWIN32(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiRegisterHubWIN64(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiRegisterHubMACOS32(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiRegisterHubMACOS64(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiRegisterHubLIN64(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiRegisterHubLIN32(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiRegisterHubLINARMHF(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiRegisterHubLINAARCH64(StringBuilder rootUrl, StringBuilder errmsg);
    internal static int _yapiRegisterHub(StringBuilder rootUrl, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiRegisterHubWIN32(rootUrl, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiRegisterHubWIN64(rootUrl, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiRegisterHubMACOS32(rootUrl, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiRegisterHubMACOS64(rootUrl, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiRegisterHubLIN64(rootUrl, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiRegisterHubLIN32(rootUrl, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiRegisterHubLINARMHF(rootUrl, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiRegisterHubLINAARCH64(rootUrl, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiPreregisterHubWIN32(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiPreregisterHubWIN64(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiPreregisterHubMACOS32(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiPreregisterHubMACOS64(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiPreregisterHubLIN64(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiPreregisterHubLIN32(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiPreregisterHubLINARMHF(StringBuilder rootUrl, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiPreregisterHubLINAARCH64(StringBuilder rootUrl, StringBuilder errmsg);
    internal static int _yapiPreregisterHub(StringBuilder rootUrl, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiPreregisterHubWIN32(rootUrl, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiPreregisterHubWIN64(rootUrl, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiPreregisterHubMACOS32(rootUrl, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiPreregisterHubMACOS64(rootUrl, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiPreregisterHubLIN64(rootUrl, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiPreregisterHubLIN32(rootUrl, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiPreregisterHubLINARMHF(rootUrl, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiPreregisterHubLINAARCH64(rootUrl, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiUnregisterHubWIN32(StringBuilder rootUrl);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiUnregisterHubWIN64(StringBuilder rootUrl);
    [DllImport("libyapi32", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiUnregisterHubMACOS32(StringBuilder rootUrl);
    [DllImport("libyapi64", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiUnregisterHubMACOS64(StringBuilder rootUrl);
    [DllImport("libyapi-amd64", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiUnregisterHubLIN64(StringBuilder rootUrl);
    [DllImport("libyapi-i386", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiUnregisterHubLIN32(StringBuilder rootUrl);
    [DllImport("libyapi-armhf", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiUnregisterHubLINARMHF(StringBuilder rootUrl);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiUnregisterHubLINAARCH64(StringBuilder rootUrl);
    internal static void _yapiUnregisterHub(StringBuilder rootUrl)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiUnregisterHubWIN32(rootUrl);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiUnregisterHubWIN64(rootUrl);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiUnregisterHubMACOS32(rootUrl);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiUnregisterHubMACOS64(rootUrl);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiUnregisterHubLIN64(rootUrl);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiUnregisterHubLIN32(rootUrl);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiUnregisterHubLINARMHF(rootUrl);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiUnregisterHubLINAARCH64(rootUrl);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUpdateDeviceListWIN32(u32 force, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUpdateDeviceListWIN64(u32 force, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUpdateDeviceListMACOS32(u32 force, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUpdateDeviceListMACOS64(u32 force, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUpdateDeviceListLIN64(u32 force, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUpdateDeviceListLIN32(u32 force, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUpdateDeviceListLINARMHF(u32 force, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiUpdateDeviceListLINAARCH64(u32 force, StringBuilder errmsg);
    internal static int _yapiUpdateDeviceList(u32 force, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiUpdateDeviceListWIN32(force, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiUpdateDeviceListWIN64(force, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiUpdateDeviceListMACOS32(force, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiUpdateDeviceListMACOS64(force, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiUpdateDeviceListLIN64(force, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiUpdateDeviceListLIN32(force, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiUpdateDeviceListLINARMHF(force, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiUpdateDeviceListLINAARCH64(force, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHandleEventsWIN32(StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHandleEventsWIN64(StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHandleEventsMACOS32(StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHandleEventsMACOS64(StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHandleEventsLIN64(StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHandleEventsLIN32(StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHandleEventsLINARMHF(StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHandleEventsLINAARCH64(StringBuilder errmsg);
    internal static int _yapiHandleEvents(StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHandleEventsWIN32(errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHandleEventsWIN64(errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHandleEventsMACOS32(errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHandleEventsMACOS64(errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHandleEventsLIN64(errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHandleEventsLIN32(errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHandleEventsLINARMHF(errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHandleEventsLINAARCH64(errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u64 _yapiGetTickCountWIN32();
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u64 _yapiGetTickCountWIN64();
    [DllImport("libyapi32", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u64 _yapiGetTickCountMACOS32();
    [DllImport("libyapi64", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u64 _yapiGetTickCountMACOS64();
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u64 _yapiGetTickCountLIN64();
    [DllImport("libyapi-i386", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u64 _yapiGetTickCountLIN32();
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u64 _yapiGetTickCountLINARMHF();
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u64 _yapiGetTickCountLINAARCH64();
    internal static u64 _yapiGetTickCount()
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetTickCountWIN32();
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetTickCountWIN64();
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetTickCountMACOS32();
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetTickCountMACOS64();
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetTickCountLIN64();
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetTickCountLIN32();
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetTickCountLINARMHF();
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetTickCountLINAARCH64();
        }
    }
    [DllImport("yapi", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiCheckLogicalNameWIN32(StringBuilder name);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiCheckLogicalNameWIN64(StringBuilder name);
    [DllImport("libyapi32", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiCheckLogicalNameMACOS32(StringBuilder name);
    [DllImport("libyapi64", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiCheckLogicalNameMACOS64(StringBuilder name);
    [DllImport("libyapi-amd64", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiCheckLogicalNameLIN64(StringBuilder name);
    [DllImport("libyapi-i386", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiCheckLogicalNameLIN32(StringBuilder name);
    [DllImport("libyapi-armhf", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiCheckLogicalNameLINARMHF(StringBuilder name);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiCheckLogicalNameLINAARCH64(StringBuilder name);
    internal static int _yapiCheckLogicalName(StringBuilder name)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiCheckLogicalNameWIN32(name);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiCheckLogicalNameWIN64(name);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiCheckLogicalNameMACOS32(name);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiCheckLogicalNameMACOS64(name);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiCheckLogicalNameLIN64(name);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiCheckLogicalNameLIN32(name);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiCheckLogicalNameLINARMHF(name);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiCheckLogicalNameLINAARCH64(name);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u16 _yapiGetAPIVersionWIN32(ref IntPtr version, ref IntPtr dat_);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u16 _yapiGetAPIVersionWIN64(ref IntPtr version, ref IntPtr dat_);
    [DllImport("libyapi32", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u16 _yapiGetAPIVersionMACOS32(ref IntPtr version, ref IntPtr dat_);
    [DllImport("libyapi64", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u16 _yapiGetAPIVersionMACOS64(ref IntPtr version, ref IntPtr dat_);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u16 _yapiGetAPIVersionLIN64(ref IntPtr version, ref IntPtr dat_);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u16 _yapiGetAPIVersionLIN32(ref IntPtr version, ref IntPtr dat_);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u16 _yapiGetAPIVersionLINARMHF(ref IntPtr version, ref IntPtr dat_);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static u16 _yapiGetAPIVersionLINAARCH64(ref IntPtr version, ref IntPtr dat_);
    internal static u16 _yapiGetAPIVersion(ref IntPtr version, ref IntPtr dat_)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetAPIVersionWIN32(ref version, ref dat_);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetAPIVersionWIN64(ref version, ref dat_);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetAPIVersionMACOS32(ref version, ref dat_);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetAPIVersionMACOS64(ref version, ref dat_);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetAPIVersionLIN64(ref version, ref dat_);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetAPIVersionLIN32(ref version, ref dat_);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetAPIVersionLINARMHF(ref version, ref dat_);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetAPIVersionLINAARCH64(ref version, ref dat_);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YDEV_DESCR _yapiGetDeviceWIN32(StringBuilder device_str, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YDEV_DESCR _yapiGetDeviceWIN64(StringBuilder device_str, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YDEV_DESCR _yapiGetDeviceMACOS32(StringBuilder device_str, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YDEV_DESCR _yapiGetDeviceMACOS64(StringBuilder device_str, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YDEV_DESCR _yapiGetDeviceLIN64(StringBuilder device_str, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YDEV_DESCR _yapiGetDeviceLIN32(StringBuilder device_str, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YDEV_DESCR _yapiGetDeviceLINARMHF(StringBuilder device_str, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YDEV_DESCR _yapiGetDeviceLINAARCH64(StringBuilder device_str, StringBuilder errmsg);
    internal static YDEV_DESCR _yapiGetDevice(StringBuilder device_str, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetDeviceWIN32(device_str, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetDeviceWIN64(device_str, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetDeviceMACOS32(device_str, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetDeviceMACOS64(device_str, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetDeviceLIN64(device_str, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetDeviceLIN32(device_str, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetDeviceLINARMHF(device_str, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetDeviceLINAARCH64(device_str, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDeviceInfoWIN32(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDeviceInfoWIN64(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDeviceInfoMACOS32(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDeviceInfoMACOS64(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDeviceInfoLIN64(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDeviceInfoLIN32(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDeviceInfoLINARMHF(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDeviceInfoLINAARCH64(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);
    internal static int _yapiGetDeviceInfo(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetDeviceInfoWIN32(d, ref infos, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetDeviceInfoWIN64(d, ref infos, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetDeviceInfoMACOS32(d, ref infos, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetDeviceInfoMACOS64(d, ref infos, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetDeviceInfoLIN64(d, ref infos, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetDeviceInfoLIN32(d, ref infos, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetDeviceInfoLINARMHF(d, ref infos, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetDeviceInfoLINAARCH64(d, ref infos, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YFUN_DESCR _yapiGetFunctionWIN32(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YFUN_DESCR _yapiGetFunctionWIN64(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YFUN_DESCR _yapiGetFunctionMACOS32(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YFUN_DESCR _yapiGetFunctionMACOS64(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YFUN_DESCR _yapiGetFunctionLIN64(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YFUN_DESCR _yapiGetFunctionLIN32(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YFUN_DESCR _yapiGetFunctionLINARMHF(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YFUN_DESCR _yapiGetFunctionLINAARCH64(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);
    internal static YFUN_DESCR _yapiGetFunction(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetFunctionWIN32(class_str, function_str, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetFunctionWIN64(class_str, function_str, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetFunctionMACOS32(class_str, function_str, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetFunctionMACOS64(class_str, function_str, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetFunctionLIN64(class_str, function_str, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetFunctionLIN32(class_str, function_str, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetFunctionLINARMHF(class_str, function_str, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetFunctionLINAARCH64(class_str, function_str, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByClassWIN32(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByClassWIN64(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByClassMACOS32(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByClassMACOS64(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByClassLIN64(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByClassLIN32(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByClassLINARMHF(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByClassLINAARCH64(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    internal static int _yapiGetFunctionsByClass(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetFunctionsByClassWIN32(class_str, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetFunctionsByClassWIN64(class_str, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetFunctionsByClassMACOS32(class_str, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetFunctionsByClassMACOS64(class_str, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetFunctionsByClassLIN64(class_str, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetFunctionsByClassLIN32(class_str, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetFunctionsByClassLINARMHF(class_str, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetFunctionsByClassLINAARCH64(class_str, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByDeviceWIN32(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByDeviceWIN64(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByDeviceMACOS32(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByDeviceMACOS64(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByDeviceLIN64(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByDeviceLIN32(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByDeviceLINARMHF(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionsByDeviceLINAARCH64(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);
    internal static int _yapiGetFunctionsByDevice(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetFunctionsByDeviceWIN32(device, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetFunctionsByDeviceWIN64(device, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetFunctionsByDeviceMACOS32(device, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetFunctionsByDeviceMACOS64(device, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetFunctionsByDeviceLIN64(device, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetFunctionsByDeviceLIN32(device, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetFunctionsByDeviceLINARMHF(device, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetFunctionsByDeviceLINAARCH64(device, precFuncDesc, buffer, maxsize, ref neededsize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetFunctionInfoEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionInfoExWIN32(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetFunctionInfoEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionInfoExWIN64(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetFunctionInfoEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionInfoExMACOS32(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetFunctionInfoEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionInfoExMACOS64(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetFunctionInfoEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionInfoExLIN64(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetFunctionInfoEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionInfoExLIN32(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetFunctionInfoEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionInfoExLINARMHF(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetFunctionInfoEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetFunctionInfoExLINAARCH64(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);
    internal static int _yapiGetFunctionInfoEx(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder baseType, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetFunctionInfoExWIN32(fundesc, ref devdesc, serial, funcId, baseType, funcName, funcVal, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetFunctionInfoExWIN64(fundesc, ref devdesc, serial, funcId, baseType, funcName, funcVal, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetFunctionInfoExMACOS32(fundesc, ref devdesc, serial, funcId, baseType, funcName, funcVal, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetFunctionInfoExMACOS64(fundesc, ref devdesc, serial, funcId, baseType, funcName, funcVal, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetFunctionInfoExLIN64(fundesc, ref devdesc, serial, funcId, baseType, funcName, funcVal, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetFunctionInfoExLIN32(fundesc, ref devdesc, serial, funcId, baseType, funcName, funcVal, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetFunctionInfoExLINARMHF(fundesc, ref devdesc, serial, funcId, baseType, funcName, funcVal, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetFunctionInfoExLINAARCH64(fundesc, ref devdesc, serial, funcId, baseType, funcName, funcVal, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartWIN32(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartWIN64(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartMACOS32(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartMACOS64(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartLIN64(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartLIN32(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartLINARMHF(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartLINAARCH64(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    internal static int _yapiHTTPRequestSyncStart(ref YIOHDL iohdl, StringBuilder device, StringBuilder request, ref IntPtr reply, ref int replysize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHTTPRequestSyncStartWIN32(ref iohdl, device, request, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHTTPRequestSyncStartWIN64(ref iohdl, device, request, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHTTPRequestSyncStartMACOS32(ref iohdl, device, request, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHTTPRequestSyncStartMACOS64(ref iohdl, device, request, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHTTPRequestSyncStartLIN64(ref iohdl, device, request, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHTTPRequestSyncStartLIN32(ref iohdl, device, request, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHTTPRequestSyncStartLINARMHF(ref iohdl, device, request, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHTTPRequestSyncStartLINAARCH64(ref iohdl, device, request, ref reply, ref replysize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartExWIN32(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartExWIN64(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartExMACOS32(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartExMACOS64(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartExLIN64(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartExLIN32(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartExLINARMHF(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncStartExLINAARCH64(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);
    internal static int _yapiHTTPRequestSyncStartEx(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHTTPRequestSyncStartExWIN32(ref iohdl, device, request, requestlen, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHTTPRequestSyncStartExWIN64(ref iohdl, device, request, requestlen, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHTTPRequestSyncStartExMACOS32(ref iohdl, device, request, requestlen, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHTTPRequestSyncStartExMACOS64(ref iohdl, device, request, requestlen, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHTTPRequestSyncStartExLIN64(ref iohdl, device, request, requestlen, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHTTPRequestSyncStartExLIN32(ref iohdl, device, request, requestlen, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHTTPRequestSyncStartExLINARMHF(ref iohdl, device, request, requestlen, ref reply, ref replysize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHTTPRequestSyncStartExLINAARCH64(ref iohdl, device, request, requestlen, ref reply, ref replysize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncDoneWIN32(ref YIOHDL iohdl, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncDoneWIN64(ref YIOHDL iohdl, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncDoneMACOS32(ref YIOHDL iohdl, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncDoneMACOS64(ref YIOHDL iohdl, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncDoneLIN64(ref YIOHDL iohdl, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncDoneLIN32(ref YIOHDL iohdl, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncDoneLINARMHF(ref YIOHDL iohdl, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestSyncDoneLINAARCH64(ref YIOHDL iohdl, StringBuilder errmsg);
    internal static int _yapiHTTPRequestSyncDone(ref YIOHDL iohdl, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHTTPRequestSyncDoneWIN32(ref iohdl, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHTTPRequestSyncDoneWIN64(ref iohdl, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHTTPRequestSyncDoneMACOS32(ref iohdl, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHTTPRequestSyncDoneMACOS64(ref iohdl, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHTTPRequestSyncDoneLIN64(ref iohdl, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHTTPRequestSyncDoneLIN32(ref iohdl, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHTTPRequestSyncDoneLINARMHF(ref iohdl, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHTTPRequestSyncDoneLINAARCH64(ref iohdl, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncWIN32(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncWIN64(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncMACOS32(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncMACOS64(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncLIN64(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncLIN32(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncLINARMHF(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncLINAARCH64(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);
    internal static int _yapiHTTPRequestAsync(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHTTPRequestAsyncWIN32(device, request, callback, context, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHTTPRequestAsyncWIN64(device, request, callback, context, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHTTPRequestAsyncMACOS32(device, request, callback, context, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHTTPRequestAsyncMACOS64(device, request, callback, context, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHTTPRequestAsyncLIN64(device, request, callback, context, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHTTPRequestAsyncLIN32(device, request, callback, context, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHTTPRequestAsyncLINARMHF(device, request, callback, context, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHTTPRequestAsyncLINAARCH64(device, request, callback, context, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncExWIN32(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncExWIN64(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncExMACOS32(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncExMACOS64(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncExLIN64(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncExLIN32(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncExLINARMHF(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestAsyncExLINAARCH64(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);
    internal static int _yapiHTTPRequestAsyncEx(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHTTPRequestAsyncExWIN32(device, request, requestlen, callback, context, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHTTPRequestAsyncExWIN64(device, request, requestlen, callback, context, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHTTPRequestAsyncExMACOS32(device, request, requestlen, callback, context, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHTTPRequestAsyncExMACOS64(device, request, requestlen, callback, context, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHTTPRequestAsyncExLIN64(device, request, requestlen, callback, context, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHTTPRequestAsyncExLIN32(device, request, requestlen, callback, context, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHTTPRequestAsyncExLINARMHF(device, request, requestlen, callback, context, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHTTPRequestAsyncExLINAARCH64(device, request, requestlen, callback, context, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestWIN32(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestWIN64(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestMACOS32(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestMACOS64(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestLIN64(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestLIN32(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestLINARMHF(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiHTTPRequestLINAARCH64(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);
    internal static int _yapiHTTPRequest(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHTTPRequestWIN32(device, url, buffer, buffsize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHTTPRequestWIN64(device, url, buffer, buffsize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHTTPRequestMACOS32(device, url, buffer, buffsize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHTTPRequestMACOS64(device, url, buffer, buffsize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHTTPRequestLIN64(device, url, buffer, buffsize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHTTPRequestLIN32(device, url, buffer, buffsize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHTTPRequestLINARMHF(device, url, buffer, buffsize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHTTPRequestLINAARCH64(device, url, buffer, buffsize, ref fullsize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDevicePathWIN32(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDevicePathWIN64(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDevicePathMACOS32(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDevicePathMACOS64(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDevicePathLIN64(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDevicePathLIN32(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDevicePathLINARMHF(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetDevicePathLINAARCH64(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    internal static int _yapiGetDevicePath(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetDevicePathWIN32(devdesc, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetDevicePathWIN64(devdesc, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetDevicePathMACOS32(devdesc, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetDevicePathMACOS64(devdesc, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetDevicePathLIN64(devdesc, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetDevicePathLIN32(devdesc, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetDevicePathLINARMHF(devdesc, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetDevicePathLINAARCH64(devdesc, rootdevice, path, pathsize, ref neededsize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiSleepWIN32(int duration_ms, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiSleepWIN64(int duration_ms, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiSleepMACOS32(int duration_ms, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiSleepMACOS64(int duration_ms, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiSleepLIN64(int duration_ms, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiSleepLIN32(int duration_ms, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiSleepLINARMHF(int duration_ms, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiSleepLINAARCH64(int duration_ms, StringBuilder errmsg);
    internal static int _yapiSleep(int duration_ms, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiSleepWIN32(duration_ms, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiSleepWIN64(duration_ms, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiSleepMACOS32(duration_ms, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiSleepMACOS64(duration_ms, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiSleepLIN64(duration_ms, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiSleepLIN32(duration_ms, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiSleepLINARMHF(duration_ms, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiSleepLINAARCH64(duration_ms, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterHubDiscoveryCallbackWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterHubDiscoveryCallbackWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterHubDiscoveryCallbackMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterHubDiscoveryCallbackMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterHubDiscoveryCallbackLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterHubDiscoveryCallbackLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterHubDiscoveryCallbackLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterHubDiscoveryCallbackLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterHubDiscoveryCallback(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterHubDiscoveryCallbackWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterHubDiscoveryCallbackWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterHubDiscoveryCallbackMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterHubDiscoveryCallbackMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterHubDiscoveryCallbackLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterHubDiscoveryCallbackLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterHubDiscoveryCallbackLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterHubDiscoveryCallbackLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiTriggerHubDiscoveryWIN32(StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiTriggerHubDiscoveryWIN64(StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiTriggerHubDiscoveryMACOS32(StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiTriggerHubDiscoveryMACOS64(StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiTriggerHubDiscoveryLIN64(StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiTriggerHubDiscoveryLIN32(StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiTriggerHubDiscoveryLINARMHF(StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiTriggerHubDiscoveryLINAARCH64(StringBuilder errmsg);
    internal static int _yapiTriggerHubDiscovery(StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiTriggerHubDiscoveryWIN32(errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiTriggerHubDiscoveryWIN64(errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiTriggerHubDiscoveryMACOS32(errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiTriggerHubDiscoveryMACOS64(errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiTriggerHubDiscoveryLIN64(errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiTriggerHubDiscoveryLIN32(errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiTriggerHubDiscoveryLINARMHF(errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiTriggerHubDiscoveryLINAARCH64(errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceLogCallbackWIN32(IntPtr fct);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceLogCallbackWIN64(IntPtr fct);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceLogCallbackMACOS32(IntPtr fct);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceLogCallbackMACOS64(IntPtr fct);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceLogCallbackLIN64(IntPtr fct);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceLogCallbackLIN32(IntPtr fct);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceLogCallbackLINARMHF(IntPtr fct);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterDeviceLogCallbackLINAARCH64(IntPtr fct);
    internal static void _yapiRegisterDeviceLogCallback(IntPtr fct)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterDeviceLogCallbackWIN32(fct);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterDeviceLogCallbackWIN64(fct);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterDeviceLogCallbackMACOS32(fct);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterDeviceLogCallbackMACOS64(fct);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterDeviceLogCallbackLIN64(fct);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterDeviceLogCallbackLIN32(fct);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterDeviceLogCallbackLINARMHF(fct);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterDeviceLogCallbackLINAARCH64(fct);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetAllJsonKeys", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetAllJsonKeysWIN32(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetAllJsonKeys", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetAllJsonKeysWIN64(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetAllJsonKeys", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetAllJsonKeysMACOS32(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetAllJsonKeys", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetAllJsonKeysMACOS64(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetAllJsonKeys", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetAllJsonKeysLIN64(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetAllJsonKeys", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetAllJsonKeysLIN32(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetAllJsonKeys", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetAllJsonKeysLINARMHF(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetAllJsonKeys", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetAllJsonKeysLINAARCH64(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg);
    internal static YRETCODE _yapiGetAllJsonKeys(StringBuilder jsonbuffer, StringBuilder out_buffer, int out_buffersize, ref int fullsize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetAllJsonKeysWIN32(jsonbuffer, out_buffer, out_buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetAllJsonKeysWIN64(jsonbuffer, out_buffer, out_buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetAllJsonKeysMACOS32(jsonbuffer, out_buffer, out_buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetAllJsonKeysMACOS64(jsonbuffer, out_buffer, out_buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetAllJsonKeysLIN64(jsonbuffer, out_buffer, out_buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetAllJsonKeysLIN32(jsonbuffer, out_buffer, out_buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetAllJsonKeysLINARMHF(jsonbuffer, out_buffer, out_buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetAllJsonKeysLINAARCH64(jsonbuffer, out_buffer, out_buffersize, ref fullsize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiCheckFirmware", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiCheckFirmwareWIN32(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiCheckFirmware", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiCheckFirmwareWIN64(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiCheckFirmware", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiCheckFirmwareMACOS32(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiCheckFirmware", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiCheckFirmwareMACOS64(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiCheckFirmware", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiCheckFirmwareLIN64(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiCheckFirmware", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiCheckFirmwareLIN32(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiCheckFirmware", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiCheckFirmwareLINARMHF(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiCheckFirmware", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiCheckFirmwareLINAARCH64(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg);
    internal static YRETCODE _yapiCheckFirmware(StringBuilder serial, StringBuilder rev, StringBuilder path, StringBuilder buffer, int buffersize, ref int fullsize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiCheckFirmwareWIN32(serial, rev, path, buffer, buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiCheckFirmwareWIN64(serial, rev, path, buffer, buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiCheckFirmwareMACOS32(serial, rev, path, buffer, buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiCheckFirmwareMACOS64(serial, rev, path, buffer, buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiCheckFirmwareLIN64(serial, rev, path, buffer, buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiCheckFirmwareLIN32(serial, rev, path, buffer, buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiCheckFirmwareLINARMHF(serial, rev, path, buffer, buffersize, ref fullsize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiCheckFirmwareLINAARCH64(serial, rev, path, buffer, buffersize, ref fullsize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetBootloaders", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetBootloadersWIN32(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetBootloaders", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetBootloadersWIN64(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetBootloaders", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetBootloadersMACOS32(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetBootloaders", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetBootloadersMACOS64(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetBootloaders", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetBootloadersLIN64(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetBootloaders", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetBootloadersLIN32(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetBootloaders", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetBootloadersLINARMHF(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetBootloaders", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetBootloadersLINAARCH64(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    internal static YRETCODE _yapiGetBootloaders(StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetBootloadersWIN32(buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetBootloadersWIN64(buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetBootloadersMACOS32(buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetBootloadersMACOS64(buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetBootloadersLIN64(buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetBootloadersLIN32(buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetBootloadersLINARMHF(buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetBootloadersLINAARCH64(buffer, buffersize, ref totalSize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiUpdateFirmwareEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiUpdateFirmwareExWIN32(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiUpdateFirmwareEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiUpdateFirmwareExWIN64(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiUpdateFirmwareEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiUpdateFirmwareExMACOS32(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiUpdateFirmwareEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiUpdateFirmwareExMACOS64(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiUpdateFirmwareEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiUpdateFirmwareExLIN64(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiUpdateFirmwareEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiUpdateFirmwareExLIN32(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiUpdateFirmwareEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiUpdateFirmwareExLINARMHF(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiUpdateFirmwareEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiUpdateFirmwareExLINAARCH64(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg);
    internal static YRETCODE _yapiUpdateFirmwareEx(StringBuilder serial, StringBuilder firmwarePath, StringBuilder settings, int force, int startUpdate, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiUpdateFirmwareExWIN32(serial, firmwarePath, settings, force, startUpdate, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiUpdateFirmwareExWIN64(serial, firmwarePath, settings, force, startUpdate, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiUpdateFirmwareExMACOS32(serial, firmwarePath, settings, force, startUpdate, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiUpdateFirmwareExMACOS64(serial, firmwarePath, settings, force, startUpdate, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiUpdateFirmwareExLIN64(serial, firmwarePath, settings, force, startUpdate, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiUpdateFirmwareExLIN32(serial, firmwarePath, settings, force, startUpdate, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiUpdateFirmwareExLINARMHF(serial, firmwarePath, settings, force, startUpdate, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiUpdateFirmwareExLINAARCH64(serial, firmwarePath, settings, force, startUpdate, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHTTPRequestSyncStartOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestSyncStartOutOfBandWIN32(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHTTPRequestSyncStartOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestSyncStartOutOfBandWIN64(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHTTPRequestSyncStartOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestSyncStartOutOfBandMACOS32(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHTTPRequestSyncStartOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestSyncStartOutOfBandMACOS64(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHTTPRequestSyncStartOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestSyncStartOutOfBandLIN64(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHTTPRequestSyncStartOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestSyncStartOutOfBandLIN32(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHTTPRequestSyncStartOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestSyncStartOutOfBandLINARMHF(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHTTPRequestSyncStartOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestSyncStartOutOfBandLINAARCH64(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg);
    internal static YRETCODE _yapiHTTPRequestSyncStartOutOfBand(ref YIOHDL iohdl, int channel, StringBuilder device, StringBuilder request, int requestsize, ref IntPtr reply, ref int replysize, IntPtr progress_cb, IntPtr progress_ctx, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHTTPRequestSyncStartOutOfBandWIN32(ref iohdl, channel, device, request, requestsize, ref reply, ref replysize, progress_cb, progress_ctx, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHTTPRequestSyncStartOutOfBandWIN64(ref iohdl, channel, device, request, requestsize, ref reply, ref replysize, progress_cb, progress_ctx, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHTTPRequestSyncStartOutOfBandMACOS32(ref iohdl, channel, device, request, requestsize, ref reply, ref replysize, progress_cb, progress_ctx, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHTTPRequestSyncStartOutOfBandMACOS64(ref iohdl, channel, device, request, requestsize, ref reply, ref replysize, progress_cb, progress_ctx, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHTTPRequestSyncStartOutOfBandLIN64(ref iohdl, channel, device, request, requestsize, ref reply, ref replysize, progress_cb, progress_ctx, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHTTPRequestSyncStartOutOfBandLIN32(ref iohdl, channel, device, request, requestsize, ref reply, ref replysize, progress_cb, progress_ctx, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHTTPRequestSyncStartOutOfBandLINARMHF(ref iohdl, channel, device, request, requestsize, ref reply, ref replysize, progress_cb, progress_ctx, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHTTPRequestSyncStartOutOfBandLINAARCH64(ref iohdl, channel, device, request, requestsize, ref reply, ref replysize, progress_cb, progress_ctx, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiHTTPRequestAsyncOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestAsyncOutOfBandWIN32(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiHTTPRequestAsyncOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestAsyncOutOfBandWIN64(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiHTTPRequestAsyncOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestAsyncOutOfBandMACOS32(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiHTTPRequestAsyncOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestAsyncOutOfBandMACOS64(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiHTTPRequestAsyncOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestAsyncOutOfBandLIN64(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiHTTPRequestAsyncOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestAsyncOutOfBandLIN32(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiHTTPRequestAsyncOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestAsyncOutOfBandLINARMHF(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiHTTPRequestAsyncOutOfBand", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiHTTPRequestAsyncOutOfBandLINAARCH64(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg);
    internal static YRETCODE _yapiHTTPRequestAsyncOutOfBand(int channel, StringBuilder device, StringBuilder request, int requestsize, IntPtr callback, IntPtr context, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiHTTPRequestAsyncOutOfBandWIN32(channel, device, request, requestsize, callback, context, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiHTTPRequestAsyncOutOfBandWIN64(channel, device, request, requestsize, callback, context, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiHTTPRequestAsyncOutOfBandMACOS32(channel, device, request, requestsize, callback, context, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiHTTPRequestAsyncOutOfBandMACOS64(channel, device, request, requestsize, callback, context, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiHTTPRequestAsyncOutOfBandLIN64(channel, device, request, requestsize, callback, context, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiHTTPRequestAsyncOutOfBandLIN32(channel, device, request, requestsize, callback, context, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiHTTPRequestAsyncOutOfBandLINARMHF(channel, device, request, requestsize, callback, context, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiHTTPRequestAsyncOutOfBandLINAARCH64(channel, device, request, requestsize, callback, context, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiTestHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiTestHubWIN32(StringBuilder url, int mstimeout, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiTestHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiTestHubWIN64(StringBuilder url, int mstimeout, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiTestHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiTestHubMACOS32(StringBuilder url, int mstimeout, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiTestHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiTestHubMACOS64(StringBuilder url, int mstimeout, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiTestHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiTestHubLIN64(StringBuilder url, int mstimeout, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiTestHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiTestHubLIN32(StringBuilder url, int mstimeout, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiTestHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiTestHubLINARMHF(StringBuilder url, int mstimeout, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiTestHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiTestHubLINAARCH64(StringBuilder url, int mstimeout, StringBuilder errmsg);
    internal static YRETCODE _yapiTestHub(StringBuilder url, int mstimeout, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiTestHubWIN32(url, mstimeout, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiTestHubWIN64(url, mstimeout, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiTestHubMACOS32(url, mstimeout, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiTestHubMACOS64(url, mstimeout, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiTestHubLIN64(url, mstimeout, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiTestHubLIN32(url, mstimeout, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiTestHubLINARMHF(url, mstimeout, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiTestHubLINAARCH64(url, mstimeout, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiJsonGetPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonGetPathWIN32(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiJsonGetPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonGetPathWIN64(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiJsonGetPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonGetPathMACOS32(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiJsonGetPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonGetPathMACOS64(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiJsonGetPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonGetPathLIN64(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiJsonGetPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonGetPathLIN32(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiJsonGetPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonGetPathLINARMHF(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiJsonGetPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonGetPathLINAARCH64(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg);
    internal static int _yapiJsonGetPath(StringBuilder path, StringBuilder json_data, int json_len, ref IntPtr result, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiJsonGetPathWIN32(path, json_data, json_len, ref result, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiJsonGetPathWIN64(path, json_data, json_len, ref result, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiJsonGetPathMACOS32(path, json_data, json_len, ref result, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiJsonGetPathMACOS64(path, json_data, json_len, ref result, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiJsonGetPathLIN64(path, json_data, json_len, ref result, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiJsonGetPathLIN32(path, json_data, json_len, ref result, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiJsonGetPathLINARMHF(path, json_data, json_len, ref result, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiJsonGetPathLINAARCH64(path, json_data, json_len, ref result, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiJsonDecodeString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonDecodeStringWIN32(StringBuilder json_data, StringBuilder output);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiJsonDecodeString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonDecodeStringWIN64(StringBuilder json_data, StringBuilder output);
    [DllImport("libyapi32", EntryPoint = "yapiJsonDecodeString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonDecodeStringMACOS32(StringBuilder json_data, StringBuilder output);
    [DllImport("libyapi64", EntryPoint = "yapiJsonDecodeString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonDecodeStringMACOS64(StringBuilder json_data, StringBuilder output);
    [DllImport("libyapi-amd64", EntryPoint = "yapiJsonDecodeString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonDecodeStringLIN64(StringBuilder json_data, StringBuilder output);
    [DllImport("libyapi-i386", EntryPoint = "yapiJsonDecodeString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonDecodeStringLIN32(StringBuilder json_data, StringBuilder output);
    [DllImport("libyapi-armhf", EntryPoint = "yapiJsonDecodeString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonDecodeStringLINARMHF(StringBuilder json_data, StringBuilder output);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiJsonDecodeString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiJsonDecodeStringLINAARCH64(StringBuilder json_data, StringBuilder output);
    internal static int _yapiJsonDecodeString(StringBuilder json_data, StringBuilder output)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiJsonDecodeStringWIN32(json_data, output);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiJsonDecodeStringWIN64(json_data, output);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiJsonDecodeStringMACOS32(json_data, output);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiJsonDecodeStringMACOS64(json_data, output);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiJsonDecodeStringLIN64(json_data, output);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiJsonDecodeStringLIN32(json_data, output);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiJsonDecodeStringLINARMHF(json_data, output);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiJsonDecodeStringLINAARCH64(json_data, output);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetSubdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetSubdevicesWIN32(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetSubdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetSubdevicesWIN64(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetSubdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetSubdevicesMACOS32(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetSubdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetSubdevicesMACOS64(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetSubdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetSubdevicesLIN64(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetSubdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetSubdevicesLIN32(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetSubdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetSubdevicesLINARMHF(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetSubdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetSubdevicesLINAARCH64(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg);
    internal static YRETCODE _yapiGetSubdevices(StringBuilder serial, StringBuilder buffer, int buffersize, ref int totalSize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetSubdevicesWIN32(serial, buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetSubdevicesWIN64(serial, buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetSubdevicesMACOS32(serial, buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetSubdevicesMACOS64(serial, buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetSubdevicesLIN64(serial, buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetSubdevicesLIN32(serial, buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetSubdevicesLINARMHF(serial, buffer, buffersize, ref totalSize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetSubdevicesLINAARCH64(serial, buffer, buffersize, ref totalSize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiFreeMem", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeMemWIN32(IntPtr buffer);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiFreeMem", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeMemWIN64(IntPtr buffer);
    [DllImport("libyapi32", EntryPoint = "yapiFreeMem", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeMemMACOS32(IntPtr buffer);
    [DllImport("libyapi64", EntryPoint = "yapiFreeMem", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeMemMACOS64(IntPtr buffer);
    [DllImport("libyapi-amd64", EntryPoint = "yapiFreeMem", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeMemLIN64(IntPtr buffer);
    [DllImport("libyapi-i386", EntryPoint = "yapiFreeMem", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeMemLIN32(IntPtr buffer);
    [DllImport("libyapi-armhf", EntryPoint = "yapiFreeMem", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeMemLINARMHF(IntPtr buffer);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiFreeMem", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiFreeMemLINAARCH64(IntPtr buffer);
    internal static void _yapiFreeMem(IntPtr buffer)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiFreeMemWIN32(buffer);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiFreeMemWIN64(buffer);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiFreeMemMACOS32(buffer);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiFreeMemMACOS64(buffer);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiFreeMemLIN64(buffer);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiFreeMemLIN32(buffer);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiFreeMemLINARMHF(buffer);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiFreeMemLINAARCH64(buffer);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetDevicePathEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDevicePathExWIN32(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetDevicePathEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDevicePathExWIN64(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetDevicePathEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDevicePathExMACOS32(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetDevicePathEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDevicePathExMACOS64(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetDevicePathEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDevicePathExLIN64(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetDevicePathEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDevicePathExLIN32(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetDevicePathEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDevicePathExLINARMHF(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetDevicePathEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDevicePathExLINAARCH64(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);
    internal static YRETCODE _yapiGetDevicePathEx(StringBuilder serial, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetDevicePathExWIN32(serial, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetDevicePathExWIN64(serial, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetDevicePathExMACOS32(serial, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetDevicePathExMACOS64(serial, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetDevicePathExLIN64(serial, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetDevicePathExLIN32(serial, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetDevicePathExLINARMHF(serial, rootdevice, path, pathsize, ref neededsize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetDevicePathExLINAARCH64(serial, rootdevice, path, pathsize, ref neededsize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiSetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetDevListValidityWIN32(int sValidity);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiSetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetDevListValidityWIN64(int sValidity);
    [DllImport("libyapi32", EntryPoint = "yapiSetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetDevListValidityMACOS32(int sValidity);
    [DllImport("libyapi64", EntryPoint = "yapiSetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetDevListValidityMACOS64(int sValidity);
    [DllImport("libyapi-amd64", EntryPoint = "yapiSetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetDevListValidityLIN64(int sValidity);
    [DllImport("libyapi-i386", EntryPoint = "yapiSetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetDevListValidityLIN32(int sValidity);
    [DllImport("libyapi-armhf", EntryPoint = "yapiSetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetDevListValidityLINARMHF(int sValidity);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiSetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetDevListValidityLINAARCH64(int sValidity);
    internal static void _yapiSetNetDevListValidity(int sValidity)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiSetNetDevListValidityWIN32(sValidity);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiSetNetDevListValidityWIN64(sValidity);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiSetNetDevListValidityMACOS32(sValidity);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiSetNetDevListValidityMACOS64(sValidity);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiSetNetDevListValidityLIN64(sValidity);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiSetNetDevListValidityLIN32(sValidity);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiSetNetDevListValidityLINARMHF(sValidity);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiSetNetDevListValidityLINAARCH64(sValidity);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetDevListValidityWIN32();
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetDevListValidityWIN64();
    [DllImport("libyapi32", EntryPoint = "yapiGetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetDevListValidityMACOS32();
    [DllImport("libyapi64", EntryPoint = "yapiGetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetDevListValidityMACOS64();
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetDevListValidityLIN64();
    [DllImport("libyapi-i386", EntryPoint = "yapiGetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetDevListValidityLIN32();
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetDevListValidityLINARMHF();
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetNetDevListValidity", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetDevListValidityLINAARCH64();
    internal static int _yapiGetNetDevListValidity()
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetNetDevListValidityWIN32();
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetNetDevListValidityWIN64();
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetNetDevListValidityMACOS32();
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetNetDevListValidityMACOS64();
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetNetDevListValidityLIN64();
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetNetDevListValidityLIN32();
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetNetDevListValidityLINARMHF();
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetNetDevListValidityLINAARCH64();
        }
    }
    [DllImport("yapi", EntryPoint = "yapiRegisterBeaconCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterBeaconCallbackWIN32(IntPtr beaconCallback);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiRegisterBeaconCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterBeaconCallbackWIN64(IntPtr beaconCallback);
    [DllImport("libyapi32", EntryPoint = "yapiRegisterBeaconCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterBeaconCallbackMACOS32(IntPtr beaconCallback);
    [DllImport("libyapi64", EntryPoint = "yapiRegisterBeaconCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterBeaconCallbackMACOS64(IntPtr beaconCallback);
    [DllImport("libyapi-amd64", EntryPoint = "yapiRegisterBeaconCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterBeaconCallbackLIN64(IntPtr beaconCallback);
    [DllImport("libyapi-i386", EntryPoint = "yapiRegisterBeaconCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterBeaconCallbackLIN32(IntPtr beaconCallback);
    [DllImport("libyapi-armhf", EntryPoint = "yapiRegisterBeaconCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterBeaconCallbackLINARMHF(IntPtr beaconCallback);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiRegisterBeaconCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiRegisterBeaconCallbackLINAARCH64(IntPtr beaconCallback);
    internal static void _yapiRegisterBeaconCallback(IntPtr beaconCallback)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiRegisterBeaconCallbackWIN32(beaconCallback);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiRegisterBeaconCallbackWIN64(beaconCallback);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiRegisterBeaconCallbackMACOS32(beaconCallback);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiRegisterBeaconCallbackMACOS64(beaconCallback);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiRegisterBeaconCallbackLIN64(beaconCallback);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiRegisterBeaconCallbackLIN32(beaconCallback);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiRegisterBeaconCallbackLINARMHF(beaconCallback);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiRegisterBeaconCallbackLINAARCH64(beaconCallback);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiStartStopDeviceLogCallbackWIN32(StringBuilder serial, int start);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiStartStopDeviceLogCallbackWIN64(StringBuilder serial, int start);
    [DllImport("libyapi32", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiStartStopDeviceLogCallbackMACOS32(StringBuilder serial, int start);
    [DllImport("libyapi64", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiStartStopDeviceLogCallbackMACOS64(StringBuilder serial, int start);
    [DllImport("libyapi-amd64", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiStartStopDeviceLogCallbackLIN64(StringBuilder serial, int start);
    [DllImport("libyapi-i386", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiStartStopDeviceLogCallbackLIN32(StringBuilder serial, int start);
    [DllImport("libyapi-armhf", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiStartStopDeviceLogCallbackLINARMHF(StringBuilder serial, int start);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiStartStopDeviceLogCallbackLINAARCH64(StringBuilder serial, int start);
    internal static void _yapiStartStopDeviceLogCallback(StringBuilder serial, int start)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiStartStopDeviceLogCallbackWIN32(serial, start);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiStartStopDeviceLogCallbackWIN64(serial, start);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiStartStopDeviceLogCallbackMACOS32(serial, start);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiStartStopDeviceLogCallbackMACOS64(serial, start);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiStartStopDeviceLogCallbackLIN64(serial, start);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiStartStopDeviceLogCallbackLIN32(serial, start);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiStartStopDeviceLogCallbackLINARMHF(serial, start);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiStartStopDeviceLogCallbackLINAARCH64(serial, start);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiIsModuleWritable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiIsModuleWritableWIN32(StringBuilder serial, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiIsModuleWritable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiIsModuleWritableWIN64(StringBuilder serial, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiIsModuleWritable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiIsModuleWritableMACOS32(StringBuilder serial, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiIsModuleWritable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiIsModuleWritableMACOS64(StringBuilder serial, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiIsModuleWritable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiIsModuleWritableLIN64(StringBuilder serial, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiIsModuleWritable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiIsModuleWritableLIN32(StringBuilder serial, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiIsModuleWritable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiIsModuleWritableLINARMHF(StringBuilder serial, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiIsModuleWritable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiIsModuleWritableLINAARCH64(StringBuilder serial, StringBuilder errmsg);
    internal static int _yapiIsModuleWritable(StringBuilder serial, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiIsModuleWritableWIN32(serial, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiIsModuleWritableWIN64(serial, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiIsModuleWritableMACOS32(serial, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiIsModuleWritableMACOS64(serial, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiIsModuleWritableLIN64(serial, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiIsModuleWritableLIN32(serial, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiIsModuleWritableLINARMHF(serial, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiIsModuleWritableLINAARCH64(serial, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetDLLPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDLLPathWIN32(StringBuilder path, int pathsize, StringBuilder errmsg);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetDLLPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDLLPathWIN64(StringBuilder path, int pathsize, StringBuilder errmsg);
    [DllImport("libyapi32", EntryPoint = "yapiGetDLLPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDLLPathMACOS32(StringBuilder path, int pathsize, StringBuilder errmsg);
    [DllImport("libyapi64", EntryPoint = "yapiGetDLLPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDLLPathMACOS64(StringBuilder path, int pathsize, StringBuilder errmsg);
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetDLLPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDLLPathLIN64(StringBuilder path, int pathsize, StringBuilder errmsg);
    [DllImport("libyapi-i386", EntryPoint = "yapiGetDLLPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDLLPathLIN32(StringBuilder path, int pathsize, StringBuilder errmsg);
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetDLLPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDLLPathLINARMHF(StringBuilder path, int pathsize, StringBuilder errmsg);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetDLLPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static YRETCODE _yapiGetDLLPathLINAARCH64(StringBuilder path, int pathsize, StringBuilder errmsg);
    internal static YRETCODE _yapiGetDLLPath(StringBuilder path, int pathsize, StringBuilder errmsg)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetDLLPathWIN32(path, pathsize, errmsg);
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetDLLPathWIN64(path, pathsize, errmsg);
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetDLLPathMACOS32(path, pathsize, errmsg);
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetDLLPathMACOS64(path, pathsize, errmsg);
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetDLLPathLIN64(path, pathsize, errmsg);
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetDLLPathLIN32(path, pathsize, errmsg);
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetDLLPathLINARMHF(path, pathsize, errmsg);
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetDLLPathLINAARCH64(path, pathsize, errmsg);
        }
    }
    [DllImport("yapi", EntryPoint = "yapiSetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetworkTimeoutWIN32(int sValidity);
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiSetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetworkTimeoutWIN64(int sValidity);
    [DllImport("libyapi32", EntryPoint = "yapiSetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetworkTimeoutMACOS32(int sValidity);
    [DllImport("libyapi64", EntryPoint = "yapiSetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetworkTimeoutMACOS64(int sValidity);
    [DllImport("libyapi-amd64", EntryPoint = "yapiSetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetworkTimeoutLIN64(int sValidity);
    [DllImport("libyapi-i386", EntryPoint = "yapiSetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetworkTimeoutLIN32(int sValidity);
    [DllImport("libyapi-armhf", EntryPoint = "yapiSetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetworkTimeoutLINARMHF(int sValidity);
    [DllImport("libyapi-aarch64", EntryPoint = "yapiSetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static void _yapiSetNetworkTimeoutLINAARCH64(int sValidity);
    internal static void _yapiSetNetworkTimeout(int sValidity)
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  _yapiSetNetworkTimeoutWIN32(sValidity);
                  return;
             case YAPIDLL_VERSION.WIN64:
                  _yapiSetNetworkTimeoutWIN64(sValidity);
                  return;
             case YAPIDLL_VERSION.MACOS32:
                  _yapiSetNetworkTimeoutMACOS32(sValidity);
                  return;
             case YAPIDLL_VERSION.MACOS64:
                  _yapiSetNetworkTimeoutMACOS64(sValidity);
                  return;
             case YAPIDLL_VERSION.LIN64:
                  _yapiSetNetworkTimeoutLIN64(sValidity);
                  return;
             case YAPIDLL_VERSION.LIN32:
                  _yapiSetNetworkTimeoutLIN32(sValidity);
                  return;
             case YAPIDLL_VERSION.LINARMHF:
                  _yapiSetNetworkTimeoutLINARMHF(sValidity);
                  return;
             case YAPIDLL_VERSION.LINAARCH64:
                  _yapiSetNetworkTimeoutLINAARCH64(sValidity);
                  return;
        }
    }
    [DllImport("yapi", EntryPoint = "yapiGetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetworkTimeoutWIN32();
    [DllImport("amd64\\yapi.dll", EntryPoint = "yapiGetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetworkTimeoutWIN64();
    [DllImport("libyapi32", EntryPoint = "yapiGetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetworkTimeoutMACOS32();
    [DllImport("libyapi64", EntryPoint = "yapiGetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetworkTimeoutMACOS64();
    [DllImport("libyapi-amd64", EntryPoint = "yapiGetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetworkTimeoutLIN64();
    [DllImport("libyapi-i386", EntryPoint = "yapiGetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetworkTimeoutLIN32();
    [DllImport("libyapi-armhf", EntryPoint = "yapiGetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetworkTimeoutLINARMHF();
    [DllImport("libyapi-aarch64", EntryPoint = "yapiGetNetworkTimeout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private extern static int _yapiGetNetworkTimeoutLINAARCH64();
    internal static int _yapiGetNetworkTimeout()
    {
        switch (_dllVersion) {
             default:
             case YAPIDLL_VERSION.WIN32:
                  return _yapiGetNetworkTimeoutWIN32();
             case YAPIDLL_VERSION.WIN64:
                  return _yapiGetNetworkTimeoutWIN64();
             case YAPIDLL_VERSION.MACOS32:
                  return _yapiGetNetworkTimeoutMACOS32();
             case YAPIDLL_VERSION.MACOS64:
                  return _yapiGetNetworkTimeoutMACOS64();
             case YAPIDLL_VERSION.LIN64:
                  return _yapiGetNetworkTimeoutLIN64();
             case YAPIDLL_VERSION.LIN32:
                  return _yapiGetNetworkTimeoutLIN32();
             case YAPIDLL_VERSION.LINARMHF:
                  return _yapiGetNetworkTimeoutLINARMHF();
             case YAPIDLL_VERSION.LINAARCH64:
                  return _yapiGetNetworkTimeoutLINAARCH64();
        }
    }
//--- (end of generated code: YFunction dlldef)
}


public class YAPI
{
    public static Encoding DefaultEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");

    // Switch to turn off exceptions and use return codes instead, for source-code compatibility
    // with languages without exception support like C
    public static bool ExceptionsDisabled = false;

    internal static Object globalLock = new Object();

    static bool _apiInitialized = false;
    internal static YAPIContext _yapiContext = new YAPIContext();
    public const string INVALID_STRING = "!INVALID!";
    public const double INVALID_DOUBLE = -1.79769313486231E+308;
    public const double MIN_DOUBLE = Double.MinValue;
    public const double MAX_DOUBLE = Double.MaxValue;

    public const int INVALID_INT = -2147483648;
    public const int INVALID_UINT = -1;
    public const long INVALID_LONG = -9223372036854775807L;
    public const string HARDWAREID_INVALID = INVALID_STRING;
    public const string FUNCTIONID_INVALID = INVALID_STRING;
    public const string FRIENDLYNAME_INVALID = INVALID_STRING;

    public const int INVALID_UNSIGNED = -1;

    // yInitAPI argument
    public const int Y_DETECT_NONE = 0;
    public const int Y_DETECT_USB = 1;
    public const int Y_DETECT_NET = 2;
    public const int Y_RESEND_MISSING_PKT = 4;
    public const int Y_DETECT_ALL = Y_DETECT_USB | Y_DETECT_NET;


    public const int DETECT_NONE = 0;
    public const int DETECT_USB = 1;
    public const int DETECT_NET = 2;
    public const int RESEND_MISSING_PKT = 4;
    public const int DETECT_ALL = DETECT_USB | DETECT_NET;


    public const string YOCTO_API_VERSION_STR = "1.10";
    public const int YOCTO_API_VERSION_BCD = 0x0110;

    public const string YOCTO_API_BUILD_NO = "38532";
    public const int YOCTO_DEFAULT_PORT = 4444;
    public const int YOCTO_VENDORID = 0x24e0;
    public const int YOCTO_DEVID_FACTORYBOOT = 1;

    public const int YOCTO_DEVID_BOOTLOADER = 2;
    public const int YOCTO_ERRMSG_LEN = 256;
    public const int YOCTO_MANUFACTURER_LEN = 20;
    public const int YOCTO_SERIAL_LEN = 20;
    public const int YOCTO_BASE_SERIAL_LEN = 8;
    public const int YOCTO_PRODUCTNAME_LEN = 28;
    public const int YOCTO_FIRMWARE_LEN = 22;
    public const int YOCTO_LOGICAL_LEN = 20;

    public const int YOCTO_FUNCTION_LEN = 20;

    // Size of the data (can be non null terminated)
    public const int YOCTO_PUBVAL_SIZE = 6;

    // Temporary storage, > YOCTO_PUBVAL_SIZE
    public const int YOCTO_PUBVAL_LEN = 16;
    public const int YOCTO_PASS_LEN = 20;
    public const int YOCTO_REALM_LEN = 20;
    public const int INVALID_YHANDLE = 0;
    public const int HASH_BUF_SIZE = 28;

    // Calibration types
    public const int YOCTO_CALIB_TYPE_OFS = 30;

    public const int yUnknowSize = 1024;


    // --- (generated code: YFunction return codes)
// Yoctopuce error codes, used by default as function return value
    public const int SUCCESS = 0;                   // everything worked all right
    public const int NOT_INITIALIZED = -1;          // call yInitAPI() first !
    public const int INVALID_ARGUMENT = -2;         // one of the arguments passed to the function is invalid
    public const int NOT_SUPPORTED = -3;            // the operation attempted is (currently) not supported
    public const int DEVICE_NOT_FOUND = -4;         // the requested device is not reachable
    public const int VERSION_MISMATCH = -5;         // the device firmware is incompatible with this API version
    public const int DEVICE_BUSY = -6;              // the device is busy with another task and cannot answer
    public const int TIMEOUT = -7;                  // the device took too long to provide an answer
    public const int IO_ERROR = -8;                 // there was an I/O problem while talking to the device
    public const int NO_MORE_DATA = -9;             // there is no more data to read from
    public const int EXHAUSTED = -10;               // you have run out of a limited resource, check the documentation
    public const int DOUBLE_ACCES = -11;            // you have two process that try to access to the same device
    public const int UNAUTHORIZED = -12;            // unauthorized access to password-protected device
    public const int RTC_NOT_READY = -13;           // real-time clock has not been initialized (or time was lost)
    public const int FILE_NOT_FOUND = -14;          // the file is not found
    //--- (end of generated code: YFunction return codes)


    static List<YDevice> YDevice_devCache;


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void HTTPRequestCallback(YDevice device, ref blockingCallbackCtx context, YRETCODE returnval, string result, string errmsg);

    // - Types used for public yocto_api callbacks
    public delegate void yLogFunc(string log);

    public delegate void yDeviceUpdateFunc(YModule modul);

    public delegate double yCalibrationHandler(double rawValue, int calibType, List<int> parameters, List<double> rawValues, List<double> refValues);

    public delegate void YHubDiscoveryCallback(String serial, String url);

    // - Types used for internal yapi callbacks
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiLogFunc(IntPtr log, u32 loglen);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiDeviceUpdateFunc(YDEV_DESCR dev);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiBeaconUpdateFunc(YDEV_DESCR dev, int beacon);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiFunctionUpdateFunc(YFUN_DESCR dev, IntPtr value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiTimedReportFunc(YFUN_DESCR dev, double timestamp, IntPtr data, u32 len, double duration);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiHubDiscoveryCallback(IntPtr serial, IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiDeviceLogCallback(YFUN_DESCR dev, IntPtr data);

    // - Variables used to store public yocto_api callbacks
    private static yLogFunc ylog = null;
    private static yDeviceUpdateFunc yArrival = null;
    private static yDeviceUpdateFunc yRemoval = null;
    private static yDeviceUpdateFunc yChange = null;
    private static YHubDiscoveryCallback _HubDiscoveryCallback = null;

    public static bool YISERR(YRETCODE retcode)
    {
        if (retcode < 0)
            return true;
        return false;
    }

    public class blockingCallbackCtx
    {
        public YRETCODE res;
        public string response;
        public string errmsg;
    }

    public static void YblockingCallback(YDevice device, ref blockingCallbackCtx context, YRETCODE returnval, string result, string errmsg)
    {
        context.res = returnval;
        context.response = result;
        context.errmsg = errmsg;
    }


    internal static int ParseHTTP(string data, int start, int stop, out int headerlen, out string errmsg)
    {
        const string httpheader = "HTTP/1.1 ";
        const string okHeader = "OK\r\n";
        int p1 = 0;
        int p2 = 0;
        const string CR = "\r\n";
        int httpcode;

        if ((stop - start) > okHeader.Length && data.Substring(start, okHeader.Length) == okHeader) {
            httpcode = 200;
            errmsg = "";
        } else {
            if ((stop - start) < httpheader.Length || data.Substring(start, httpheader.Length) != httpheader) {
                errmsg = "data should start with " + httpheader;
                headerlen = 0;
                return -1;
            }

            p1 = data.IndexOf(" ", start + httpheader.Length - 1);
            p2 = data.IndexOf(" ", p1 + 1);
            if (p1 < 0 || p2 < 0) {
                errmsg = "Invalid HTTP header (invalid first line)";
                headerlen = 0;
                return -1;
            }

            httpcode = Convert.ToInt32(data.Substring(p1, p2 - p1 + 1));
            if (httpcode != 200) {
                errmsg = string.Format("Unexpected HTTP return code:{0}", httpcode);
            } else {
                errmsg = "";
            }
        }

        p1 = data.IndexOf(CR + CR, start); //json data is a structure
        if (p1 < 0) {
            errmsg = "Invalid HTTP header (missing header end)";
            headerlen = 0;
            return -1;
        }

        headerlen = p1 + 4;
        return httpcode;
    }


    public abstract class YJSONContent
    {
        internal string _data;
        internal int _data_start;
        protected int _data_len;
        internal int _data_boundary;

        protected YJSONType _type;
        //protected string debug;

        public enum YJSONType
        {
            STRING,
            NUMBER,
            ARRAY,
            OBJECT
        }

        protected enum Tjstate
        {
            JSTART,
            JWAITFORNAME,
            JWAITFORENDOFNAME,
            JWAITFORCOLON,
            JWAITFORDATA,
            JWAITFORNEXTSTRUCTMEMBER,
            JWAITFORNEXTARRAYITEM,
            JWAITFORSTRINGVALUE,
            JWAITFORSTRINGVALUE_ESC,
            JWAITFORINTVALUE,
            JWAITFORBOOLVALUE
        }

        public static YJSONContent ParseJson(string data, int start, int stop)
        {
            int cur_pos = SkipGarbage(data, start, stop);
            YJSONContent res;
            if (data[cur_pos] == '[') {
                res = new YJSONArray(data, start, stop);
            } else if (data[cur_pos] == '{') {
                res = new YJSONObject(data, start, stop);
            } else if (data[cur_pos] == '"') {
                res = new YJSONString(data, start, stop);
            } else {
                res = new YJSONNumber(data, start, stop);
            }

            res.parse();
            return res;
        }

        protected YJSONContent(string data, int start, int stop, YJSONType type)
        {
            _data = data;
            _data_start = start;
            _data_boundary = stop;
            _type = type;
        }

        protected YJSONContent(YJSONType type)
        {
            _data = null;
        }

        public YJSONType getJSONType()
        {
            return _type;
        }

        public abstract int parse();

        protected static int SkipGarbage(string data, int start, int stop)
        {
            if (data.Length <= start) {
                return start;
            }

            char sti = data[start];
            while (start < stop && (sti == '\n' || sti == '\r' || sti == ' ')) {
                start++;
                sti = data[start];
            }

            return start;
        }

        protected string FormatError(string errmsg, int cur_pos)
        {
            int ststart = cur_pos - 10;
            int stend = cur_pos + 10;
            if (ststart < 0)
                ststart = 0;
            if (stend > _data_boundary)
                stend = _data_boundary;
            if (_data == null) {
                return errmsg;
            }

            return errmsg + " near " + _data.Substring(ststart, cur_pos - ststart) + _data.Substring(cur_pos, stend - cur_pos);
        }

        public abstract string toJSON();
    }

    internal class YJSONArray : YJSONContent
    {
        private List<YJSONContent> _arrayValue = new List<YJSONContent>();

        public YJSONArray(string data, int start, int stop) : base(data, start, stop, YJSONType.ARRAY)
        { }

        public YJSONArray(string data) : this(data, 0, data.Length)
        { }

        public YJSONArray() : base(YJSONType.ARRAY)
        { }

        public int Length {
            get { return _arrayValue.Count; }
        }

        public override int parse()
        {
            int cur_pos = SkipGarbage(_data, _data_start, _data_boundary);

            if (cur_pos >= _data_boundary || _data[cur_pos] != '[') {
                throw new System.Exception(FormatError("Opening braces was expected", cur_pos));
            }

            cur_pos++;
            Tjstate state = Tjstate.JWAITFORDATA;

            while (cur_pos < _data_boundary) {
                char sti = _data[cur_pos];
                switch (state) {
                    case Tjstate.JWAITFORDATA:
                        if (sti == '{') {
                            YJSONObject jobj = new YJSONObject(_data, cur_pos, _data_boundary);
                            int len = jobj.parse();
                            cur_pos += len;
                            _arrayValue.Add(jobj);
                            state = Tjstate.JWAITFORNEXTARRAYITEM;
                            //cur_pos is already incremented
                            continue;
                        } else if (sti == '[') {
                            YJSONArray jobj = new YJSONArray(_data, cur_pos, _data_boundary);
                            int len = jobj.parse();
                            cur_pos += len;
                            _arrayValue.Add(jobj);
                            state = Tjstate.JWAITFORNEXTARRAYITEM;
                            //cur_pos is already incremented
                            continue;
                        } else if (sti == '"') {
                            YJSONString jobj = new YJSONString(_data, cur_pos, _data_boundary);
                            int len = jobj.parse();
                            cur_pos += len;
                            _arrayValue.Add(jobj);
                            state = Tjstate.JWAITFORNEXTARRAYITEM;
                            //cur_pos is already incremented
                            continue;
                        } else if (sti == '-' || (sti >= '0' && sti <= '9')) {
                            YJSONNumber jobj = new YJSONNumber(_data, cur_pos, _data_boundary);
                            int len = jobj.parse();
                            cur_pos += len;
                            _arrayValue.Add(jobj);
                            state = Tjstate.JWAITFORNEXTARRAYITEM;
                            //cur_pos is already incremented
                            continue;
                        } else if (sti == ']') {
                            _data_len = cur_pos + 1 - _data_start;
                            return _data_len;
                        } else if (sti != ' ' && sti != '\n' && sti != '\r') {
                            throw new System.Exception(FormatError("invalid char: was expecting  \",0..9,t or f", cur_pos));
                        }

                        break;
                    case Tjstate.JWAITFORNEXTARRAYITEM:
                        if (sti == ',') {
                            state = Tjstate.JWAITFORDATA;
                        } else if (sti == ']') {
                            _data_len = cur_pos + 1 - _data_start;
                            return _data_len;
                        } else {
                            if (sti != ' ' && sti != '\n' && sti != '\r') {
                                throw new System.Exception(FormatError("invalid char: was expecting ,", cur_pos));
                            }
                        }

                        break;
                    default:
                        throw new System.Exception(FormatError("invalid state for YJSONObject", cur_pos));
                }

                cur_pos++;
            }

            throw new System.Exception(FormatError("unexpected end of data", cur_pos));
        }

        public YJSONObject getYJSONObject(int i)
        {
            return (YJSONObject) _arrayValue[i];
        }

        public string getString(int i)
        {
            YJSONString ystr = (YJSONString) _arrayValue[i];
            return ystr.getString();
        }

        public YJSONContent get(int i)
        {
            return _arrayValue[i];
        }

        public YJSONArray getYJSONArray(int i)
        {
            return (YJSONArray) _arrayValue[i];
        }

        public int getInt(int i)
        {
            YJSONNumber ystr = (YJSONNumber) _arrayValue[i];
            return ystr.getInt();
        }

        public long getLong(int i)
        {
            YJSONNumber ystr = (YJSONNumber) _arrayValue[i];
            return ystr.getLong();
        }

        public void put(string flatAttr)
        {
            YJSONString strobj = new YJSONString();
            strobj.setContent(flatAttr);
            _arrayValue.Add(strobj);
        }

        public override string toJSON()
        {
            StringBuilder res = new StringBuilder();
            res.Append('[');
            string sep = "";
            foreach (YJSONContent yjsonContent in _arrayValue) {
                string subres = yjsonContent.toJSON();
                res.Append(sep);
                res.Append(subres);
                sep = ",";
            }

            res.Append(']');
            return res.ToString();
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.Append('[');
            string sep = "";
            foreach (YJSONContent yjsonContent in _arrayValue) {
                string subres = yjsonContent.ToString();
                res.Append(sep);
                res.Append(subres);
                sep = ",";
            }

            res.Append(']');
            return res.ToString();
        }
    }

    internal class YJSONString : YJSONContent
    {
        private string _stringValue;

        public YJSONString(string data, int start, int stop) : base(data, start, stop, YJSONType.STRING)
        { }

        public YJSONString(string data) : this(data, 0, data.Length)
        { }

        public YJSONString() : base(YJSONType.STRING)
        { }

        public override int parse()
        {
            string value = "";
            int cur_pos = SkipGarbage(_data, _data_start, _data_boundary);

            if (cur_pos >= _data_boundary || _data[cur_pos] != '"') {
                throw new System.Exception(FormatError("double quote was expected", cur_pos));
            }

            cur_pos++;
            int str_start = cur_pos;
            Tjstate state = Tjstate.JWAITFORSTRINGVALUE;

            while (cur_pos < _data_boundary) {
                char sti = _data[cur_pos];
                switch (state) {
                    case Tjstate.JWAITFORSTRINGVALUE:
                        if (sti == '\\') {
                            value += _data.Substring(str_start, cur_pos - str_start);
                            str_start = cur_pos;
                            state = Tjstate.JWAITFORSTRINGVALUE_ESC;
                        } else if (sti == '"') {
                            value += _data.Substring(str_start, cur_pos - str_start);
                            _stringValue = value;
                            _data_len = (cur_pos + 1) - _data_start;
                            return _data_len;
                        } else if (sti < 32) {
                            throw new System.Exception(FormatError("invalid char: was expecting string value", cur_pos));
                        }

                        break;
                    case Tjstate.JWAITFORSTRINGVALUE_ESC:
                        value += sti;
                        state = Tjstate.JWAITFORSTRINGVALUE;
                        str_start = cur_pos + 1;
                        break;
                    default:
                        throw new System.Exception(FormatError("invalid state for YJSONObject", cur_pos));
                }

                cur_pos++;
            }

            throw new System.Exception(FormatError("unexpected end of data", cur_pos));
        }

        public override string toJSON()
        {
            StringBuilder res = new StringBuilder(_stringValue.Length * 2);
            res.Append('"');
            foreach (char c in _stringValue) {
                switch (c) {
                    case '"':
                        res.Append("\\\"");
                        break;
                    case '\\':
                        res.Append("\\\\");
                        break;
                    case '/':
                        res.Append("\\/");
                        break;
                    case '\b':
                        res.Append("\\b");
                        break;
                    case '\f':
                        res.Append("\\f");
                        break;
                    case '\n':
                        res.Append("\\n");
                        break;
                    case '\r':
                        res.Append("\\r");
                        break;
                    case '\t':
                        res.Append("\\t");
                        break;
                    default:
                        res.Append(c);
                        break;
                }
            }

            res.Append('"');
            return res.ToString();
        }

        public string getString()
        {
            return _stringValue;
        }

        public override string ToString()
        {
            return _stringValue;
        }

        public void setContent(string value)
        {
            _stringValue = value;
        }
    }


    internal class YJSONNumber : YJSONContent
    {
        private long _intValue = 0;
        private double _doubleValue = 0;
        private bool _isFloat = false;

        public YJSONNumber(string data, int start, int stop) : base(data, start, stop, YJSONType.NUMBER)
        { }

        public override int parse()
        {
            bool neg = false;
            int start, dotPos;
            char sti;
            int cur_pos = SkipGarbage(_data, _data_start, _data_boundary);
            sti = _data[cur_pos];
            if (sti == '-') {
                neg = true;
                cur_pos++;
            }

            start = cur_pos;
            dotPos = start;
            while (cur_pos < _data_boundary) {
                sti = _data[cur_pos];
                if (sti == '.' && _isFloat == false) {
                    string int_part = _data.Substring(start, cur_pos - start);
                    _intValue = Convert.ToInt64(int_part);
                    _isFloat = true;
                } else if (sti < '0' || sti > '9') {
                    string numberpart = _data.Substring(start, cur_pos - start);
                    if (_isFloat) {
                        _doubleValue = Convert.ToDouble(numberpart);
                    } else {
                        _intValue = Convert.ToInt64(numberpart);
                    }

                    if (neg) {
                        _doubleValue = 0 - _doubleValue;
                        _intValue = 0 - _intValue;
                    }

                    return cur_pos - _data_start;
                }

                cur_pos++;
            }

            throw new System.Exception(FormatError("unexpected end of data", cur_pos));
        }

        public override string toJSON()
        {
            if (_isFloat)
                return _doubleValue.ToString();
            else
                return _intValue.ToString();
        }

        public long getLong()
        {
            if (_isFloat)
                return (long) _doubleValue;
            else
                return _intValue;
        }

        public int getInt()
        {
            if (_isFloat)
                return (int) _doubleValue;
            else
                return (int) _intValue;
        }

        public double getDouble()
        {
            if (_isFloat)
                return _doubleValue;
            else
                return _intValue;
        }

        public override string ToString()
        {
            if (_isFloat)
                return _doubleValue.ToString();
            else
                return _intValue.ToString();
        }
    }


    public class YJSONObject : YJSONContent
    {
        readonly Dictionary<string, YJSONContent> parsed = new Dictionary<string, YJSONContent>();
        readonly List<string> _keys = new List<string>(16);

        public YJSONObject(string data) : base(data, 0, data.Length, YJSONType.OBJECT)
        { }

        public YJSONObject(string data, int start, int len) : base(data, start, len, YJSONType.OBJECT)
        { }

        public override int parse()
        {
            string current_name = "";
            int name_start = _data_start;
            int cur_pos = SkipGarbage(_data, _data_start, _data_boundary);

            if (_data.Length <= cur_pos || cur_pos >= _data_boundary || _data[cur_pos] != '{') {
                throw new System.Exception(FormatError("Opening braces was expected", cur_pos));
            }

            cur_pos++;
            Tjstate state = Tjstate.JWAITFORNAME;

            while (cur_pos < _data_boundary) {
                char sti = _data[cur_pos];
                switch (state) {
                    case Tjstate.JWAITFORNAME:
                        if (sti == '"') {
                            state = Tjstate.JWAITFORENDOFNAME;
                            name_start = cur_pos + 1;
                        } else if (sti == '}') {
                            _data_len = cur_pos + 1 - _data_start;
                            return _data_len;
                        } else {
                            if (sti != ' ' && sti != '\n' && sti != '\r') {
                                throw new System.Exception(FormatError("invalid char: was expecting \"", cur_pos));
                            }
                        }

                        break;
                    case Tjstate.JWAITFORENDOFNAME:
                        if (sti == '"') {
                            current_name = _data.Substring(name_start, cur_pos - name_start);
                            state = Tjstate.JWAITFORCOLON;
                        } else {
                            if (sti < 32) {
                                throw new System.Exception(FormatError("invalid char: was expecting an identifier compliant char", cur_pos));
                            }
                        }

                        break;
                    case Tjstate.JWAITFORCOLON:
                        if (sti == ':') {
                            state = Tjstate.JWAITFORDATA;
                        } else {
                            if (sti != ' ' && sti != '\n' && sti != '\r') {
                                throw new System.Exception(FormatError("invalid char: was expecting \"", cur_pos));
                            }
                        }

                        break;
                    case Tjstate.JWAITFORDATA:
                        if (sti == '{') {
                            YJSONObject jobj = new YJSONObject(_data, cur_pos, _data_boundary);
                            int len = jobj.parse();
                            cur_pos += len;
                            parsed.Add(current_name, jobj);
                            _keys.Add(current_name);
                            state = Tjstate.JWAITFORNEXTSTRUCTMEMBER;
                            //cur_pos is already incremented
                            continue;
                        } else if (sti == '[') {
                            YJSONArray jobj = new YJSONArray(_data, cur_pos, _data_boundary);
                            int len = jobj.parse();
                            cur_pos += len;
                            parsed.Add(current_name, jobj);
                            _keys.Add(current_name);
                            state = Tjstate.JWAITFORNEXTSTRUCTMEMBER;
                            //cur_pos is already incremented
                            continue;
                        } else if (sti == '"') {
                            YJSONString jobj = new YJSONString(_data, cur_pos, _data_boundary);
                            int len = jobj.parse();
                            cur_pos += len;
                            parsed.Add(current_name, jobj);
                            _keys.Add(current_name);
                            state = Tjstate.JWAITFORNEXTSTRUCTMEMBER;
                            //cur_pos is already incremented
                            continue;
                        } else if (sti == '-' || (sti >= '0' && sti <= '9')) {
                            YJSONNumber jobj = new YJSONNumber(_data, cur_pos, _data_boundary);
                            int len = jobj.parse();
                            cur_pos += len;
                            parsed.Add(current_name, jobj);
                            _keys.Add(current_name);
                            state = Tjstate.JWAITFORNEXTSTRUCTMEMBER;
                            //cur_pos is already incremented
                            continue;
                        } else if (sti != ' ' && sti != '\n' && sti != '\r') {
                            throw new System.Exception(FormatError("invalid char: was expecting  \",0..9,t or f", cur_pos));
                        }

                        break;
                    case Tjstate.JWAITFORNEXTSTRUCTMEMBER:
                        if (sti == ',') {
                            state = Tjstate.JWAITFORNAME;
                            name_start = cur_pos + 1;
                        } else if (sti == '}') {
                            _data_len = cur_pos + 1 - _data_start;
                            return _data_len;
                        } else {
                            if (sti != ' ' && sti != '\n' && sti != '\r') {
                                throw new System.Exception(FormatError("invalid char: was expecting ,", cur_pos));
                            }
                        }

                        break;
                    case Tjstate.JWAITFORNEXTARRAYITEM:
                    case Tjstate.JWAITFORSTRINGVALUE:
                    case Tjstate.JWAITFORINTVALUE:
                    case Tjstate.JWAITFORBOOLVALUE:
                        throw new System.Exception(FormatError("invalid state for YJSONObject", cur_pos));
                }

                cur_pos++;
            }

            throw new System.Exception(FormatError("unexpected end of data", cur_pos));
        }

        public bool has(string key)
        {
            return parsed.ContainsKey(key);
        }

        public YJSONObject getYJSONObject(string key)
        {
            return (YJSONObject) parsed[key];
        }

        internal YJSONString getYJSONString(string key)
        {
            return (YJSONString) parsed[key];
        }

        internal YJSONArray getYJSONArray(string key)
        {
            return (YJSONArray) parsed[key];
        }

        public List<string> keys()
        {
            return parsed.Keys.ToList();
        }

        internal YJSONNumber getYJSONNumber(string key)
        {
            return (YJSONNumber) parsed[key];
        }

        public void remove(string key)
        {
            parsed.Remove(key);
        }

        public string getString(string key)
        {
            YJSONString ystr = (YJSONString) parsed[key];
            return ystr.getString();
        }

        public int getInt(string key)
        {
            YJSONNumber yint = (YJSONNumber) parsed[key];
            return yint.getInt();
        }

        public YJSONContent get(string key)
        {
            return parsed[key];
        }

        public long getLong(string key)
        {
            YJSONNumber yint = (YJSONNumber) parsed[key];
            return yint.getLong();
        }

        public double getDouble(string key)
        {
            YJSONNumber yint = (YJSONNumber) parsed[key];
            return yint.getDouble();
        }

        public override string toJSON()
        {
            StringBuilder res = new StringBuilder();
            res.Append('{');
            string sep = "";
            foreach (string key in parsed.Keys.ToArray()) {
                YJSONContent subContent = parsed[key];
                string subres = subContent.toJSON();
                res.Append(sep);
                res.Append('"');
                res.Append(key);
                res.Append("\":");
                res.Append(subres);
                sep = ",";
            }

            res.Append('}');
            return res.ToString();
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.Append('{');
            string sep = "";
            foreach (string key in parsed.Keys.ToArray()) {
                YJSONContent subContent = parsed[key];
                string subres = subContent.ToString();
                res.Append(sep);
                res.Append(key);
                res.Append("=>");
                res.Append(subres);
                sep = ",";
            }

            res.Append('}');
            return res.ToString();
        }


        public void parseWithRef(YJSONObject reference)
        {
            if (reference != null) {
                try {
                    YJSONArray yzon = new YJSONArray(_data, _data_start, _data_boundary);
                    yzon.parse();
                    convert(reference, yzon);
                    return;
                } catch (Exception) { }
            }

            this.parse();
        }

        private void convert(YJSONObject reference, YJSONArray newArray)
        {
            int length = newArray.Length;
            for (int i = 0; i < length; i++) {
                string key = reference.getKeyFromIdx(i);
                YJSONContent new_item = newArray.get(i);
                YJSONContent reference_item = reference.get(key);

                if (new_item.getJSONType() == reference_item.getJSONType()) {
                    parsed.Add(key, new_item);
                    _keys.Add(key);
                } else if (new_item.getJSONType() == YJSONType.ARRAY && reference_item.getJSONType() == YJSONType.OBJECT) {
                    YJSONObject jobj = new YJSONObject(new_item._data, new_item._data_start, reference_item._data_boundary);
                    jobj.convert((YJSONObject) reference_item, (YJSONArray) new_item);
                    parsed.Add(key, jobj);
                    _keys.Add(key);
                } else {
                    throw new System.Exception("Unable to convert " + new_item.getJSONType().ToString() + " to " + reference.getJSONType().ToString());
                }
            }
        }

        private string getKeyFromIdx(int i)
        {
            return _keys[i];
        }
    }


    public class YDevice
    {
        private readonly YDEV_DESCR _devdescr;
        private ulong _cacheStamp;
        private YJSONObject _cacheJson;
        private readonly Object _lock = new Object();
        private readonly List<u32> _functions = new List<u32>();

        private string _rootdevice;
        private string _subpath;

        private bool _subpathinit;

        private YDevice(YDEV_DESCR devdesc)
        {
            _devdescr = devdesc;
            _cacheStamp = 0;
            _cacheJson = null;
        }


        internal void dispose()
        {
            clearCache(true);
        }


        internal void clearCache(bool clearSubpath)
        {
            lock (_lock) {
                _cacheStamp = 0;
                if (clearSubpath) {
                    _cacheJson = null;
                    _subpathinit = false;
                }
            }
        }


        internal static void PlugDevice(YDEV_DESCR devdescr)
        {
            lock (YAPI.globalLock) {
                for (int idx = 0; idx <= YAPI.YDevice_devCache.Count - 1; idx++) {
                    YDevice dev = YAPI.YDevice_devCache[idx];
                    if (dev._devdescr == devdescr) {
                        dev.clearCache(true);
                        break;
                    }
                }
            }
        }

        internal static YDevice getDevice(YDEV_DESCR devdescr)
        {
            int idx;
            YDevice dev = null;
            lock (YAPI.globalLock) {
                for (idx = 0; idx <= YAPI.YDevice_devCache.Count - 1; idx++) {
                    if (YAPI.YDevice_devCache[idx]._devdescr == devdescr) {
                        return YAPI.YDevice_devCache[idx];
                    }
                }

                dev = new YDevice(devdescr);
                YAPI.YDevice_devCache.Add(dev);
            }

            return dev;
        }

        private YRETCODE HTTPRequestSync(byte[] request_org, ref byte[] reply, ref string errmsg)
        {
            SafeNativeMethods.YIOHDL iohdl;
            IntPtr requestbuf = IntPtr.Zero;
            StringBuilder buffer = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
            IntPtr preply = default(IntPtr);
            int replysize = 0;
            byte[] fullrequest = null;
            YRETCODE res;
            bool enter;
            do {
                enter = Monitor.TryEnter(_lock);
                if (!enter) {
                    Thread.Sleep(50);
                }
            } while (!enter);

            try {
                res = HTTPRequestPrepare(request_org, ref fullrequest, ref errmsg);
                if (YAPI.YISERR(res))
                    return res;

                iohdl.raw0 = 0; // dummy, useless init to avoid compiler warning
                iohdl.raw1 = 0;
                iohdl.raw2 = 0;
                iohdl.raw3 = 0;
                iohdl.raw4 = 0;
                iohdl.raw5 = 0;
                iohdl.raw6 = 0;
                iohdl.raw7 = 0;

                requestbuf = Marshal.AllocHGlobal(fullrequest.Length);
                Marshal.Copy(fullrequest, 0, requestbuf, fullrequest.Length);

                res = SafeNativeMethods._yapiHTTPRequestSyncStartEx(ref iohdl, new StringBuilder(_rootdevice), requestbuf, fullrequest.Length, ref preply, ref replysize, buffer);
                Marshal.FreeHGlobal(requestbuf);
                if (res < 0) {
                    errmsg = buffer.ToString();
                    return res;
                }

                reply = new byte[replysize];
                if (reply.Length > 0 && preply != null) {
                    Marshal.Copy(preply, reply, 0, replysize);
                }

                res = SafeNativeMethods._yapiHTTPRequestSyncDone(ref iohdl, buffer);
            } finally {
                Monitor.Exit(_lock);
            }

            errmsg = buffer.ToString();
            return res;
        }

        private YRETCODE HTTPRequestAsync(byte[] request, ref string errmsg)
        {
            byte[] fullrequest = null;
            IntPtr requestbuf = IntPtr.Zero;
            StringBuilder buffer = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
            YRETCODE res;
            lock (_lock) {
                res = HTTPRequestPrepare(request, ref fullrequest, ref errmsg);
                if (YAPI.YISERR(res))
                    return res;

                requestbuf = Marshal.AllocHGlobal(fullrequest.Length);
                Marshal.Copy(fullrequest, 0, requestbuf, fullrequest.Length);
                res = SafeNativeMethods._yapiHTTPRequestAsyncEx(new StringBuilder(_rootdevice), requestbuf, fullrequest.Length, default(IntPtr), default(IntPtr), buffer);
            }

            Marshal.FreeHGlobal(requestbuf);
            errmsg = buffer.ToString();
            return res;
        }

        private YRETCODE HTTPRequestPrepare(byte[] request, ref byte[] fullrequest, ref string errmsg)
        {
            YRETCODE res = default(YRETCODE);
            StringBuilder errbuf = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
            StringBuilder b = null;
            int neededsize = 0;
            int p = 0;
            StringBuilder root = new StringBuilder(YAPI.YOCTO_SERIAL_LEN);
            int tmp = 0;

            // no need to lock since it's already done by the called.
            if (!_subpathinit) {
                res = SafeNativeMethods._yapiGetDevicePath(_devdescr, root, null, 0, ref neededsize, errbuf);

                if (YAPI.YISERR(res)) {
                    errmsg = errbuf.ToString();
                    return res;
                }

                b = new StringBuilder(neededsize);
                res = SafeNativeMethods._yapiGetDevicePath(_devdescr, root, b, neededsize, ref tmp, errbuf);
                if (YAPI.YISERR(res)) {
                    errmsg = errbuf.ToString();
                    return res;
                }

                _rootdevice = root.ToString();
                _subpath = b.ToString();
                _subpathinit = true;
            }

            // search for the first '/'
            p = 0;
            while (p < request.Length && request[p] != 47)
                p++;
            fullrequest = new byte[request.Length - 1 + _subpath.Length];
            Buffer.BlockCopy(request, 0, fullrequest, 0, p);
            Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes(_subpath), 0, fullrequest, p, _subpath.Length);
            Buffer.BlockCopy(request, p + 1, fullrequest, p + _subpath.Length, request.Length - p - 1);

            return YAPI.SUCCESS;
        }


        internal YRETCODE requestAPI(out YJSONObject apires, ref string errmsg)
        {
            string buffer = "";
            int res = 0;
            int http_headerlen;

            apires = null;
            lock (_lock) {
                // Check if we have a valid cache value
                if (_cacheStamp > YAPI.GetTickCount()) {
                    apires = _cacheJson;
                    return YAPI.SUCCESS;
                }

                string request;
                if (_cacheJson == null) {
                    request = "GET /api.json \r\n\r\n";
                } else {
                    string fwrelease = _cacheJson.getYJSONObject("module").getString("firmwareRelease");
                    fwrelease = YAPI.__escapeAttr(fwrelease);
                    request = "GET /api.json?fw=" + fwrelease + " \r\n\r\n";
                }

                res = HTTPRequest(request, out buffer, ref errmsg);
                if (YAPI.YISERR(res)) {
                    // make sure a device scan does not solve the issue
                    res = YAPI.yapiUpdateDeviceList(1, ref errmsg);
                    if (YAPI.YISERR(res)) {
                        return res;
                    }

                    res = HTTPRequest(request, out buffer, ref errmsg);
                    if (YAPI.YISERR(res)) {
                        return res;
                    }
                }

                int httpcode = YAPI.ParseHTTP(buffer, 0, buffer.Length, out http_headerlen, out errmsg);
                if (httpcode != 200) {
                    return YAPI.IO_ERROR;
                }

                try {
                    apires = new YJSONObject(buffer, http_headerlen, buffer.Length);
                    apires.parseWithRef(_cacheJson);
                } catch (Exception E) {
                    errmsg = "unexpected JSON structure: " + E.Message;
                    return YAPI.IO_ERROR;
                }


                // store result in cache
                _cacheJson = apires;
                _cacheStamp = YAPI.GetTickCount() + YAPI._yapiContext.GetCacheValidity();
            }

            return YAPI.SUCCESS;
        }


        internal YRETCODE getFunctions(ref List<u32> functions, ref string errmsg)
        {
            int res = 0;
            int neededsize = 0;
            int i = 0;
            int count = 0;
            IntPtr p = default(IntPtr);
            s32[] ids = null;
            lock (_lock) {
                if (_functions.Count == 0) {
                    res = YAPI.apiGetFunctionsByDevice(_devdescr, 0, IntPtr.Zero, 64, ref neededsize, ref errmsg);
                    if (YAPI.YISERR(res)) {
                        return res;
                    }

                    p = Marshal.AllocHGlobal(neededsize);

                    res = YAPI.apiGetFunctionsByDevice(_devdescr, 0, p, 64, ref neededsize, ref errmsg);
                    if (YAPI.YISERR(res)) {
                        Marshal.FreeHGlobal(p);
                        return res;
                    }

                    count = Convert.ToInt32(neededsize / Marshal.SizeOf(i));
                    //  i is an 32 bits integer
                    Array.Resize(ref ids, count + 1);
                    Marshal.Copy(p, ids, 0, count);
                    for (i = 0; i <= count - 1; i++) {
                        _functions.Add(Convert.ToUInt32(ids[i]));
                    }

                    Marshal.FreeHGlobal(p);
                }

                functions = _functions;
            }

            return YAPI.SUCCESS;
        }

        /*
         * Thread safe hepers
         */

        internal YRETCODE HTTPRequest(byte[] request, ref byte[] buffer, ref string errmsg)
        {
            return HTTPRequestSync(request, ref buffer, ref errmsg);
        }


        internal YRETCODE HTTPRequest(string request, out string buffer, ref string errmsg)
        {
            byte[] binreply = new byte[0];
            YRETCODE res = HTTPRequestSync(YAPI.DefaultEncoding.GetBytes(request), ref binreply, ref errmsg);
            buffer = YAPI.DefaultEncoding.GetString(binreply);
            return res;
        }

        internal YRETCODE HTTPRequest(string request, ref byte[] buffer, ref string errmsg)
        {
            return HTTPRequestSync(YAPI.DefaultEncoding.GetBytes(request), ref buffer, ref errmsg);
        }


        internal YRETCODE HTTPRequestAsync(string request, ref string errmsg)
        {
            return this.HTTPRequestAsync(YAPI.DefaultEncoding.GetBytes(request), ref errmsg);
        }
    }


    public static string GetYAPIDllPath()
    {
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        StringBuilder smallbuff = new StringBuilder(1024);
        int yapi_res;
        string path;
        YAPI.GetAPIVersion();
        yapi_res = SafeNativeMethods._yapiGetDLLPath(smallbuff, 1024, errmsg);
        if (yapi_res < 0) {
            return "";
        }

        path = smallbuff.ToString();
        return path;
    }


    /**
     * <summary>
     *   Disables the use of exceptions to report runtime errors.
     * <para>
     *   When exceptions are disabled, every function returns a specific
     *   error value which depends on its type and which is documented in
     *   this reference manual.
     * </para>
     * </summary>
     */
    public static void DisableExceptions()
    {
        ExceptionsDisabled = true;
    }

    /**
     * <summary>
     *   Re-enables the use of exceptions for runtime error handling.
     * <para>
     *   Be aware than when exceptions are enabled, every function that fails
     *   triggers an exception. If the exception is not caught by the user code,
     *   it  either fires the debugger or aborts (i.e. crash) the program.
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     * </summary>
     */
    public static void EnableExceptions()
    {
        ExceptionsDisabled = false;
    }

    // - Internal callback registered into YAPI using a protected delegate
    private static void native_yLogFunction(IntPtr log, u32 loglen)
    {
        if (ylog != null)
            ylog(Marshal.PtrToStringAnsi(log));
    }


    private static void native_yDeviceLogCallback(YFUN_DESCR devdescr, IntPtr data)
    {
        SafeNativeMethods.yDeviceSt infos = SafeNativeMethods.emptyDeviceSt();
        YModule modul;
        String errmsg = "";
        YModule.LogCallback callback;

        if (yapiGetDeviceInfo(devdescr, ref infos, ref errmsg) != YAPI.SUCCESS) {
            return;
        }

        modul = YModule.FindModule(infos.serial + ".module");
        callback = modul.get_logCallback();
        if (callback != null) {
            callback(modul, Marshal.PtrToStringAnsi(data));
        }
    }


    /**
     * <summary>
     *   Registers a log callback function.
     * <para>
     *   This callback will be called each time
     *   the API have something to say. Quite useful to debug the API.
     * </para>
     * </summary>
     * <param name="logfun">
     *   a procedure taking a string parameter, or <c>null</c>
     *   to unregister a previously registered  callback.
     * </param>
     */
    public static void RegisterLogFunction(yLogFunc logfun)
    {
        ylog = logfun;
    }


    private class DataEvent
    {
        private YFunction _fun;
        private YSensor _sensor;
        private YModule _module;
        private String _value;
        private List<int> _report;
        private double _timestamp;
        private double _duration;
        private int _beacon;

        public DataEvent(YFunction fun, String value)
        {
            _fun = fun;
            _sensor = null;
            _module = null;
            _value = value;
            _report = null;
            _timestamp = 0;
            _beacon = -1;
        }

        public DataEvent(YSensor sensor, double timestamp, double duration, List<int> report)
        {
            _fun = null;
            _sensor = sensor;
            _module = null;
            _value = null;
            _timestamp = timestamp;
            _duration = duration;
            _report = report;
            _beacon = -1;
        }

        public DataEvent(YModule module)
        {
            _fun = null;
            _sensor = null;
            _module = module;
            _value = null;
            _report = null;
            _timestamp = 0;
            _beacon = -1;
        }

        public DataEvent(YModule module, int beacon)
        {
            _fun = null;
            _sensor = null;
            _module = module;
            _value = null;
            _report = null;
            _timestamp = 0;
            _beacon = beacon;
        }


        public void invoke()
        {
            if (_sensor != null) {
                YMeasure mesure = _sensor._decodeTimedReport(_timestamp, _duration, _report);
                _sensor._invokeTimedReportCallback(mesure);
            } else if (_fun != null) {
                // new value
                _fun._invokeValueCallback(_value);
            } else {
                if (_beacon < 0) {
                    _module._invokeConfigChangeCallback();
                } else {
                    _module._invokeBeaconCallback(_beacon);
                }
            }
        }
    }

    private class PlugEvent
    {
        public enum EVTYPE
        {
            ARRIVAL,
            REMOVAL,
            CHANGE,
            HUB_DISCOVERY
        }

        private EVTYPE _eventtype;
        private YModule _module;
        private String _serial;
        private String _url;

        public PlugEvent(EVTYPE type, YModule mod)
        {
            _eventtype = type;
            _module = mod;
        }

        public PlugEvent(String serial, String url)
        {
            _eventtype = EVTYPE.HUB_DISCOVERY;
            _serial = serial;
            _url = url;
        }

        public void invoke()
        {
            switch (_eventtype) {
                case EVTYPE.ARRIVAL:
                    if (yArrival != null)
                        yArrival(_module);
                    break;
                case EVTYPE.REMOVAL:
                    if (yRemoval != null)
                        yRemoval(_module);

                    break;
                case EVTYPE.CHANGE:
                    if (yChange != null)
                        yChange(_module);
                    break;
                case EVTYPE.HUB_DISCOVERY:
                    if (_HubDiscoveryCallback != null)
                        _HubDiscoveryCallback(_serial, _url);
                    break;
            }
        }
    }


    static List<PlugEvent> _PlugEvents;
    static List<DataEvent> _DataEvents;

    private static void native_HubDiscoveryCallback(IntPtr serial_ptr, IntPtr url_ptr)
    {
        String serial = Marshal.PtrToStringAnsi(serial_ptr);
        String url = Marshal.PtrToStringAnsi(url_ptr);
        PlugEvent ev = new PlugEvent(serial, url);
        _PlugEvents.Add(ev);
    }

    /**
     * <summary>
     *   Register a callback function, to be called each time an Network Hub send
     *   an SSDP message.
     * <para>
     *   The callback has two string parameter, the first one
     *   contain the serial number of the hub and the second contain the URL of the
     *   network hub (this URL can be passed to RegisterHub). This callback will be invoked
     *   while yUpdateDeviceList is running. You will have to call this function on a regular basis.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="hubDiscoveryCallback">
     *   a procedure taking two string parameter, the serial
     *   number and the hub URL. Use <c>null</c> to unregister a previously registered  callback.
     * </param>
     */
    public static void RegisterHubDiscoveryCallback(YHubDiscoveryCallback hubDiscoveryCallback)
    {
        String errmsg = "";
        _HubDiscoveryCallback = hubDiscoveryCallback;
        TriggerHubDiscovery(ref errmsg);
    }

    private static void native_yDeviceArrivalCallback(YDEV_DESCR d)
    {
        SafeNativeMethods.yDeviceSt infos = SafeNativeMethods.emptyDeviceSt();
        PlugEvent ev;
        string errmsg = "";

        if (yapiGetDeviceInfo(d, ref infos, ref errmsg) != SUCCESS) {
            return;
        }

        YDevice.PlugDevice(d);
        YModule modul = YModule.FindModule(infos.serial + ".module");
        modul.setImmutableAttributes(infos);
        ev = new PlugEvent(PlugEvent.EVTYPE.ARRIVAL, modul);
        if (yArrival != null)
            _PlugEvents.Add(ev);
    }

    /**
     * <summary>
     *   Register a callback function, to be called each time
     *   a device is plugged.
     * <para>
     *   This callback will be invoked while <c>yUpdateDeviceList</c>
     *   is running. You will have to call this function on a regular basis.
     * </para>
     * </summary>
     * <param name="arrivalCallback">
     *   a procedure taking a <c>YModule</c> parameter, or <c>null</c>
     *   to unregister a previously registered  callback.
     * </param>
     */
    public static void RegisterDeviceArrivalCallback(yDeviceUpdateFunc arrivalCallback)
    {
        yArrival = arrivalCallback;
        if (arrivalCallback != null) {
            string error = "";
            YModule mod = YModule.FirstModule();
            while (mod != null) {
                if (mod.isOnline()) {
                    yapiLockDeviceCallBack(ref error);
                    native_yDeviceArrivalCallback(mod.functionDescriptor());
                    yapiUnlockDeviceCallBack(ref error);
                }

                mod = mod.nextModule();
            }
        }
    }

    private static void native_yDeviceRemovalCallback(YDEV_DESCR d)
    {
        PlugEvent ev;
        SafeNativeMethods.yDeviceSt infos = SafeNativeMethods.emptyDeviceSt();
        string errmsg = "";
        if (yRemoval == null)
            return;
        infos.deviceid = 0;
        if (yapiGetDeviceInfo(d, ref infos, ref errmsg) != SUCCESS)
            return;
        YModule modul = YModule.FindModule(infos.serial + ".module");
        ev = new PlugEvent(PlugEvent.EVTYPE.REMOVAL, modul);
        _PlugEvents.Add(ev);
    }

    /**
     * <summary>
     *   Register a callback function, to be called each time
     *   a device is unplugged.
     * <para>
     *   This callback will be invoked while <c>yUpdateDeviceList</c>
     *   is running. You will have to call this function on a regular basis.
     * </para>
     * </summary>
     * <param name="removalCallback">
     *   a procedure taking a <c>YModule</c> parameter, or <c>null</c>
     *   to unregister a previously registered  callback.
     * </param>
     */
    public static void RegisterDeviceRemovalCallback(yDeviceUpdateFunc removalCallback)
    {
        yRemoval = removalCallback;
    }

    public static void native_yDeviceChangeCallback(YDEV_DESCR d)
    {
        PlugEvent ev;
        SafeNativeMethods.yDeviceSt infos = SafeNativeMethods.emptyDeviceSt();
        string errmsg = "";

        if (yChange == null)
            return;
        if (yapiGetDeviceInfo(d, ref infos, ref errmsg) != SUCCESS)
            return;
        YModule modul = YModule.FindModule(infos.serial + ".module");
        ev = new PlugEvent(PlugEvent.EVTYPE.CHANGE, modul);
        _PlugEvents.Add(ev);
    }

    public static void RegisterDeviceChangeCallback(yDeviceUpdateFunc callback)
    {
        yChange = callback;
    }

    public static void native_yDeviceConfigChangeCallback(YDEV_DESCR d)
    {
        DataEvent ev;
        SafeNativeMethods.yDeviceSt infos = SafeNativeMethods.emptyDeviceSt();
        string errmsg = "";

        if (yapiGetDeviceInfo(d, ref infos, ref errmsg) != SUCCESS)
            return;
        YModule modul = YModule.FindModule(infos.serial + ".module");
        if (YModule._moduleCallbackList.ContainsKey(modul) && YModule._moduleCallbackList[modul] > 0) {
            ev = new DataEvent(modul);
            _DataEvents.Add(ev);
        }
    }

    public static void native_yBeaconChangeCallback(YDEV_DESCR d, int beacon)
    {
        DataEvent ev;
        SafeNativeMethods.yDeviceSt infos = SafeNativeMethods.emptyDeviceSt();
        string errmsg = "";

        if (yapiGetDeviceInfo(d, ref infos, ref errmsg) != SUCCESS)
            return;
        YModule modul = YModule.FindModule(infos.serial + ".module");
        if (YModule._moduleCallbackList.ContainsKey(modul) && YModule._moduleCallbackList[modul] > 0) {
            ev = new DataEvent(modul, beacon);
            _DataEvents.Add(ev);
        }
    }


    private static void queuesCleanUp()
    {
        _PlugEvents.Clear();
        _PlugEvents = null;
        _DataEvents.Clear();
        _DataEvents = null;
    }

    private static void native_yFunctionUpdateCallback(YFUN_DESCR fundesc, IntPtr data)
    {
        if (!IntPtr.Zero.Equals(data)) {
            for (int i = 0; i < YFunction._ValueCallbackList.Count; i++) {
                if (YFunction._ValueCallbackList[i].get_functionDescriptor() == fundesc) {
                    DataEvent ev = new DataEvent(YFunction._ValueCallbackList[i], Marshal.PtrToStringAnsi(data));
                    _DataEvents.Add(ev);
                }
            }
        }
    }

    private static void native_yTimedReportCallback(YFUN_DESCR fundesc, double timestamp, IntPtr rawdata, u32 len, double duration)
    {
        for (int i = 0; i < YFunction._TimedReportCallbackList.Count; i++) {
            if (YFunction._TimedReportCallbackList[i].get_functionDescriptor() == fundesc) {
                byte[] data = new byte[len];
                Marshal.Copy(rawdata, data, 0, (int) len);
                if ((data[0] & 0xff) <= 2) {
                    List<int> report = new List<int>((int) len);
                    int p = 0;
                    while (p < len) {
                        report.Add(data[p++] & 0xff);
                    }

                    DataEvent ev = new DataEvent(YFunction._TimedReportCallbackList[i], timestamp, duration, report);
                    _DataEvents.Add(ev);
                }
            }
        }
    }


    public static void RegisterCalibrationHandler(int calibType, YAPI.yCalibrationHandler callback)
    {
        string key;
        key = calibType.ToString();
        YFunction._CalibHandlers.Add(key, callback);
    }

    private static double yLinearCalibrationHandler(double rawValue, int calibType, List<int> parameters, List<double> rawValues, List<double> refValues)
    {
        int npt;
        double x, adj;
        double x2, adj2;
        int i;

        x = rawValues[0];
        adj = refValues[0] - x;
        i = 0;
        if (calibType < YAPI.YOCTO_CALIB_TYPE_OFS) {
            npt = calibType % 10;
            if (npt > rawValues.Count) npt = rawValues.Count;
            if (npt > refValues.Count) npt = refValues.Count;
        } else {
            npt = refValues.Count;
        }

        while ((rawValue > rawValues[i]) && (i + 1 < npt)) {
            i++;
            x2 = x;
            adj2 = adj;
            x = rawValues[i];
            adj = refValues[i] - x;
            if ((rawValue < x) && (x > x2)) {
                adj = adj2 + (adj - adj2) * (rawValue - x2) / (x - x2);
            }
        }

        return rawValue + adj;
    }


    private static int yapiLockDeviceCallBack(ref string errmsg)
    {
        int functionReturnValue = 0;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiLockDeviceCallBack(buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    private static int yapiUnlockDeviceCallBack(ref string errmsg)
    {
        int functionReturnValue = 0;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiUnlockDeviceCallBack(buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    private static int yapiLockFunctionCallBack(ref string errmsg)
    {
        int functionReturnValue = 0;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiLockFunctionCallBack(buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    private static int yapiUnlockFunctionCallBack(ref string errmsg)
    {
        int functionReturnValue = 0;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiUnlockFunctionCallBack(buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    public static yCalibrationHandler _getCalibrationHandler(int calType)
    {
        string key;

        key = calType.ToString();
        if (YFunction._CalibHandlers.ContainsKey(key))
            return YFunction._CalibHandlers[key];
        return null;
    }

    private static double[] decExp = new double[] {1.0e-6, 1.0e-5, 1.0e-4, 1.0e-3, 1.0e-2, 1.0e-1, 1.0, 1.0e1, 1.0e2, 1.0e3, 1.0e4, 1.0e5, 1.0e6, 1.0e7, 1.0e8, 1.0e9};

    // Convert Yoctopuce 16-bit decimal floats to standard double-precision floats
    //
    public static double _decimalToDouble(int val)
    {
        bool negate = false;
        double res;
        int mantis = val & 2047;
        if (mantis == 0)
            return 0.0;
        if (val > 32767) {
            negate = true;
            val = 65536 - val;
        } else if (val < 0) {
            negate = true;
            val = -val;
        }

        int exp = val >> 11;
        res = (double) (mantis) * decExp[exp];
        return (negate ? -res : res);
    }

    // Convert standard double-precision floats to Yoctopuce 16-bit decimal floats
    //
    public static long _doubleToDecimal(double val)
    {
        int negate = 0;
        double comp, mant;
        int decpow;
        long res;

        if (val == 0.0) {
            return 0;
        }

        if (val < 0) {
            negate = 1;
            val = -val;
        }

        comp = val / 1999.0;
        decpow = 0;
        while (comp > decExp[decpow] && decpow < 15) {
            decpow++;
        }

        mant = val / decExp[decpow];
        if (decpow == 15 && mant > 2047.0) {
            res = (15 << 11) + 2047; // overflow
        } else {
            res = (decpow << 11) + Convert.ToInt32(mant);
        }

        return (negate != 0 ? -res : res);
    }


    public static List<int> _decodeWords(string sdat)
    {
        List<int> udat = new List<int>();

        for (int p = 0; p < sdat.Length;) {
            uint val;
            uint c = sdat[p++];
            if (c == '*') {
                val = 0;
            } else if (c == 'X') {
                val = 0xffff;
            } else if (c == 'Y') {
                val = 0x7fff;
            } else if (c >= 'a') {
                int srcpos = (int) (udat.Count - 1 - (c - 'a'));
                if (srcpos < 0) {
                    val = 0;
                } else {
                    val = (uint) udat[srcpos];
                }
            } else {
                if (p + 2 > sdat.Length) {
                    return udat;
                }

                val = (c - '0');
                c = sdat[p++];
                val += (c - '0') << 5;
                c = sdat[p++];
                if (c == 'z') c = '\\';
                val += (c - '0') << 10;
            }

            udat.Add((int) val);
        }

        return udat;
    }

    public static List<int> _decodeFloats(string sdat)
    {
        List<int> idat = new List<int>();

        for (int p = 0; p < sdat.Length;) {
            int val = 0;
            int sign = 1;
            int dec = 0;
            int decInc = 0;
            int c = sdat[p++];
            while (c != (int) '-' && (c < (int) '0' || c > (int) '9')) {
                if (p >= sdat.Length) {
                    return idat;
                }

                c = sdat[p++];
            }

            if (c == '-') {
                if (p >= sdat.Length) {
                    return idat;
                }

                sign = -sign;
                c = sdat[p++];
            }

            while ((c >= '0' && c <= '9') || c == '.') {
                if (c == '.') {
                    decInc = 1;
                } else if (dec < 3) {
                    val = val * 10 + (c - '0');
                    dec += decInc;
                }

                if (p < sdat.Length) {
                    c = sdat[p++];
                } else {
                    c = 0;
                }
            }

            if (dec < 3) {
                if (dec == 0) val *= 1000;
                else if (dec == 1)
                    val *= 100;
                else
                    val *= 10;
            }

            idat.Add(sign * val);
        }

        return idat;
    }

    public static string _floatToStr(double value)
    {
        int rounded = (int) Math.Round(value * 1000);
        string res = "";
        if (rounded < 0) {
            res += "-";
            rounded = -rounded;
        }

        res += Convert.ToString((int) (rounded / 1000));
        int decim = rounded % 1000;
        if (decim > 0) {
            res += ".";
            if (decim < 100) res += "0";
            if (decim < 10) res += "0";
            if ((decim % 10) == 0) decim /= 10;
            if ((decim % 10) == 0) decim /= 10;
            res += Convert.ToString(decim);
        }

        return res;
    }

    public static int _atoi(string val)
    {
        int p = 0;
        while (p < val.Length && Char.IsWhiteSpace(val[p])) {
            p++;
        }

        int start = p;
        if (p < val.Length && (val[p] == '-' || val[p] == '+'))
            p++;
        while (p < val.Length && Char.IsDigit(val[p])) {
            p++;
        }

        if (start < p) {
            return int.Parse(val.Substring(start, p - start));
        }

        return 0;
    }

    protected const string _hexArray = "0123456789ABCDEF";

    public static string _bytesToHexStr(byte[] bytes, int offset, int len)
    {
        char[] hexChars = new char[len * 2];
        for (int j = 0; j < len; j++) {
            int v = bytes[offset + j] & 0xFF;
            hexChars[j * 2] = _hexArray[v >> 4];
            hexChars[j * 2 + 1] = _hexArray[v & 0x0F];
        }

        return new string(hexChars);
    }

    public static byte[] _hexStrToBin(string hex_str)
    {
        int len = hex_str.Length / 2;
        byte[] res = new byte[len];
        for (int i = 0; i < len; i++) {
            int val = 0;
            for (int n = 0; n < 2; n++) {
                char c = hex_str[i * 2 + n];
                val <<= 4;
                if (c <= '9') {
                    val += c - '0';
                } else if (c <= 'F') {
                    val += c - 'A' + 10;
                } else {
                    val += c - 'a' + 10;
                }
            }

            res[i] = (byte) val;
        }

        return res;
    }

    public static byte[] _bytesMerge(byte[] array_a, byte[] array_b)
    {
        byte[] res = new byte[array_a.Length + array_b.Length];
        System.Buffer.BlockCopy(array_a, 0, res, 0, array_a.Length);
        System.Buffer.BlockCopy(array_b, 0, res, array_a.Length, array_b.Length);
        return res;
    }


    public static string __escapeAttr(string changeval)
    {
        string espcaped = "";
        int i = 0;
        char c = '\0';
        string h = null;
        for (i = 0; i < changeval.Length; i++) {
            c = changeval[i];
            if (c <= ' ' || (c > 'z' && c != '~') || c == '"' || c == '%' || c == '&' || c == '+' || c == '<' || c == '=' || c == '>' || c == '\\' || c == '^' || c == '`') {
                int hh;
                if ((c == 0xc2 || c == 0xc3) && (i + 1 < changeval.Length) && (changeval[i + 1] & 0xc0) == 0x80) {
                    // UTF8-encoded ISO-8859-1 character: translate to plain ISO-8859-1
                    hh = (c & 1) * 0x40;
                    i++;
                    hh += changeval[i];
                } else {
                    hh = c;
                }

                h = hh.ToString("X");
                if ((h.Length < 2))
                    h = "0" + h;
                espcaped += "%" + h;
            } else {
                espcaped += c;
            }
        }

        return espcaped;
    }

    // - Delegate object for our internal callback, protected from GC
    public static _yapiLogFunc native_yLogFunctionDelegate = native_yLogFunction;
    static GCHandle native_yLogFunctionAnchor = GCHandle.Alloc(native_yLogFunctionDelegate);

    public static _yapiFunctionUpdateFunc native_yFunctionUpdateDelegate = native_yFunctionUpdateCallback;
    static GCHandle native_yFunctionUpdateAnchor = GCHandle.Alloc(native_yFunctionUpdateDelegate);

    public static _yapiTimedReportFunc native_yTimedReportDelegate = native_yTimedReportCallback;
    static GCHandle native_yTimedReportAnchor = GCHandle.Alloc(native_yTimedReportDelegate);

    public static _yapiHubDiscoveryCallback native_yapiHubDiscoveryDelegate = native_HubDiscoveryCallback;
    static GCHandle native_yapiHubDiscoveryAnchor = GCHandle.Alloc(native_yapiHubDiscoveryDelegate);

    public static _yapiDeviceUpdateFunc native_yDeviceArrivalDelegate = native_yDeviceArrivalCallback;
    static GCHandle native_yDeviceArrivalAnchor = GCHandle.Alloc(native_yDeviceArrivalDelegate);

    public static _yapiDeviceUpdateFunc native_yDeviceRemovalDelegate = native_yDeviceRemovalCallback;
    static GCHandle native_yDeviceRemovalAnchor = GCHandle.Alloc(native_yDeviceRemovalDelegate);

    public static _yapiDeviceUpdateFunc native_yDeviceChangeDelegate = native_yDeviceChangeCallback;
    static GCHandle native_yDeviceChangeAnchor = GCHandle.Alloc(native_yDeviceChangeDelegate);

    public static _yapiDeviceUpdateFunc native_yDeviceConfigChangeDelegate = native_yDeviceConfigChangeCallback;
    static GCHandle native_yDeviceConfigChangeAnchor = GCHandle.Alloc(native_yDeviceConfigChangeDelegate);

    public static _yapiBeaconUpdateFunc native_yBeaconChangeDelegate = native_yBeaconChangeCallback;
    static GCHandle native_yBeaconChangeAnchor = GCHandle.Alloc(native_yBeaconChangeDelegate);

    public static _yapiDeviceLogCallback native_yDeviceLogDelegate = native_yDeviceLogCallback;
    static GCHandle native_yDeviceLogAnchor = GCHandle.Alloc(native_yDeviceLogDelegate);

    public static String GetDllArchitecture()
    {
        switch (SafeNativeMethods._dllVersion)
        {
            default:
                return "Unknown";
            case SafeNativeMethods.YAPIDLL_VERSION.WIN32:
                return "Win32";
            case SafeNativeMethods.YAPIDLL_VERSION.WIN64:
                return "Win64";
            case SafeNativeMethods.YAPIDLL_VERSION.MACOS32:
                return "MacOs32";
            case SafeNativeMethods.YAPIDLL_VERSION.MACOS64:
                return "MacOs64";
            case SafeNativeMethods.YAPIDLL_VERSION.LIN64:
                return "Linux64";
            case SafeNativeMethods.YAPIDLL_VERSION.LIN32:
                return "Linux32";
            case SafeNativeMethods.YAPIDLL_VERSION.LINARMHF:
                return "Armhf32";
            case SafeNativeMethods.YAPIDLL_VERSION.LINAARCH64:
                return "Aarch64";
        }
    }


    /**
     * <summary>
     *   Returns the version identifier for the Yoctopuce library in use.
     * <para>
     *   The version is a string in the form <c>"Major.Minor.Build"</c>,
     *   for instance <c>"1.01.5535"</c>. For languages using an external
     *   DLL (for instance C#, VisualBasic or Delphi), the character string
     *   includes as well the DLL version, for instance
     *   <c>"1.01.5535 (1.01.5439)"</c>.
     * </para>
     * <para>
     *   If you want to verify in your code that the library version is
     *   compatible with the version that you have used during development,
     *   verify that the major number is strictly equal and that the minor
     *   number is greater or equal. The build number is not relevant
     *   with respect to the library compatibility.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a character string describing the library version.
     * </returns>
     */
    public static String GetAPIVersion()
    {
        string version = default(string);
        string date = default(string);
        try {
            apiGetAPIVersion(ref version, ref date);
        } catch (System.DllNotFoundException ex) {
            if (YAPI.ExceptionsDisabled) {
                return "Unable to load yapi.dll (" + ex.Message + ")";
            }
            throw;
        }
        return YOCTO_API_VERSION_STR + "." + YOCTO_API_BUILD_NO + " (" + version + ")";
    }

    /**
     * <summary>
     *   Initializes the Yoctopuce programming library explicitly.
     * <para>
     *   It is not strictly needed to call <c>yInitAPI()</c>, as the library is
     *   automatically  initialized when calling <c>yRegisterHub()</c> for the
     *   first time.
     * </para>
     * <para>
     *   When <c>YAPI.DETECT_NONE</c> is used as detection <c>mode</c>,
     *   you must explicitly use <c>yRegisterHub()</c> to point the API to the
     *   VirtualHub on which your devices are connected before trying to access them.
     * </para>
     * </summary>
     * <param name="mode">
     *   an integer corresponding to the type of automatic
     *   device detection to use. Possible values are
     *   <c>YAPI.DETECT_NONE</c>, <c>YAPI.DETECT_USB</c>, <c>YAPI.DETECT_NET</c>,
     *   and <c>YAPI.DETECT_ALL</c>.
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static int InitAPI(int mode, ref string errmsg)
    {
        int i;
        int functionReturnValue = 0;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        YRETCODE res = default(YRETCODE);

        if (_apiInitialized) {
            functionReturnValue = SUCCESS;
            return functionReturnValue;
        }

        string version = default(string);
        string date = default(string);
        u16 api_ver;
        try {
            api_ver = apiGetAPIVersion(ref version, ref date);
        } catch (System.DllNotFoundException ex) {
            errmsg = "Unable to load yapi.dll (" + ex.Message + ")";
            return FILE_NOT_FOUND;
        } catch (System.BadImageFormatException) {
            if (IntPtr.Size == 4) {
                errmsg = "Invalid yapi.dll (Using 64 bits yapi.dll with 32 bit application)";
            } else {
                errmsg = "Invalid yapi.dll (Using 32 bits yapi.dll with 64 bit application)";
            }

            return VERSION_MISMATCH;
        }

        if (api_ver != YOCTO_API_VERSION_BCD) {
            errmsg = "yapi.dll does does not match the version of the Library (Library=" + YOCTO_API_VERSION_STR + "." + YOCTO_API_BUILD_NO;
            errmsg += " yapi.dll=" + version + ")";
            return VERSION_MISMATCH;
        }


        YDevice_devCache = new List<YDevice>();
        _PlugEvents = new List<PlugEvent>(5);
        _DataEvents = new List<DataEvent>(10);

        yArrival = null;
        yRemoval = null;
        yChange = null;
        _HubDiscoveryCallback = null;
        buffer.Length = 0;
        res = SafeNativeMethods._yapiInitAPI(mode, buffer);
        if (res != YAPI.DEVICE_BUSY) {
            errmsg = buffer.ToString();
            if (YISERR(res)) {
                return res;
            }
        }

        SafeNativeMethods._yapiRegisterDeviceArrivalCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceArrivalDelegate));
        SafeNativeMethods._yapiRegisterDeviceRemovalCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceRemovalDelegate));
        SafeNativeMethods._yapiRegisterDeviceChangeCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceChangeDelegate));
        SafeNativeMethods._yapiRegisterDeviceConfigChangeCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceConfigChangeDelegate));
        SafeNativeMethods._yapiRegisterBeaconCallback(Marshal.GetFunctionPointerForDelegate(native_yBeaconChangeDelegate));
        SafeNativeMethods._yapiRegisterFunctionUpdateCallback(Marshal.GetFunctionPointerForDelegate(native_yFunctionUpdateDelegate));
        SafeNativeMethods._yapiRegisterTimedReportCallback(Marshal.GetFunctionPointerForDelegate(native_yTimedReportDelegate));
        SafeNativeMethods._yapiRegisterHubDiscoveryCallback(Marshal.GetFunctionPointerForDelegate(native_yapiHubDiscoveryDelegate));
        SafeNativeMethods._yapiRegisterDeviceLogCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceLogDelegate));
        SafeNativeMethods._yapiRegisterLogFunction(Marshal.GetFunctionPointerForDelegate(native_yLogFunctionDelegate));
        for (i = 1; i <= 20; i++)
            RegisterCalibrationHandler(i, yLinearCalibrationHandler);
        RegisterCalibrationHandler(30, yLinearCalibrationHandler);

        _apiInitialized = true;
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Frees dynamically allocated memory blocks used by the Yoctopuce library.
     * <para>
     *   It is generally not required to call this function, unless you
     *   want to free all dynamically allocated memory blocks in order to
     *   track a memory leak for instance.
     *   You should not call any other library function after calling
     *   <c>yFreeAPI()</c>, or your program will crash.
     * </para>
     * </summary>
     */
    public static void FreeAPI()
    {
        if (_apiInitialized) {
            SafeNativeMethods._yapiFreeAPI();
            YDevice_devCache.Clear();
            YDevice_devCache = null;
            _PlugEvents.Clear();
            _PlugEvents = null;
            _DataEvents.Clear();
            _DataEvents = null;
            YFunction._ValueCallbackList.Clear();
            YFunction._TimedReportCallbackList.Clear();
            YFunction._ClearCache();
            YFunction._CalibHandlers.Clear();
            YModule._moduleCallbackList.Clear();
            ylog = null;
            _apiInitialized = false;
        }
    }

    /**
     * <summary>
     *   Setup the Yoctopuce library to use modules connected on a given machine.
     * <para>
     *   The
     *   parameter will determine how the API will work. Use the following values:
     * </para>
     * <para>
     *   <b>usb</b>: When the <c>usb</c> keyword is used, the API will work with
     *   devices connected directly to the USB bus. Some programming languages such a JavaScript,
     *   PHP, and Java don't provide direct access to USB hardware, so <c>usb</c> will
     *   not work with these. In this case, use a VirtualHub or a networked YoctoHub (see below).
     * </para>
     * <para>
     *   <b><i>x.x.x.x</i></b> or <b><i>hostname</i></b>: The API will use the devices connected to the
     *   host with the given IP address or hostname. That host can be a regular computer
     *   running a VirtualHub, or a networked YoctoHub such as YoctoHub-Ethernet or
     *   YoctoHub-Wireless. If you want to use the VirtualHub running on you local
     *   computer, use the IP address 127.0.0.1.
     * </para>
     * <para>
     *   <b>callback</b>: that keyword make the API run in "<i>HTTP Callback</i>" mode.
     *   This a special mode allowing to take control of Yoctopuce devices
     *   through a NAT filter when using a VirtualHub or a networked YoctoHub. You only
     *   need to configure your hub to call your server script on a regular basis.
     *   This mode is currently available for PHP and Node.JS only.
     * </para>
     * <para>
     *   Be aware that only one application can use direct USB access at a
     *   given time on a machine. Multiple access would cause conflicts
     *   while trying to access the USB modules. In particular, this means
     *   that you must stop the VirtualHub software before starting
     *   an application that uses direct USB access. The workaround
     *   for this limitation is to setup the library to use the VirtualHub
     *   rather than direct USB access.
     * </para>
     * <para>
     *   If access control has been activated on the hub, virtual or not, you want to
     *   reach, the URL parameter should look like:
     * </para>
     * <para>
     *   <c>http://username:password@address:port</c>
     * </para>
     * <para>
     *   You can call <i>RegisterHub</i> several times to connect to several machines.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="url">
     *   a string containing either <c>"usb"</c>,<c>"callback"</c> or the
     *   root URL of the hub to monitor
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static int RegisterHub(string url, ref string errmsg)
    {
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        YRETCODE res;

        if (!_apiInitialized) {
            res = InitAPI(0, ref errmsg);
            if (YISERR(res))
                return res;
        }

        buffer.Length = 0;
        res = SafeNativeMethods._yapiRegisterHub(new StringBuilder(url), buffer);
        if (YISERR(res)) {
            errmsg = buffer.ToString();
        }

        return res;
    }

    /**
     * <summary>
     *   Fault-tolerant alternative to <c>RegisterHub()</c>.
     * <para>
     *   This function has the same
     *   purpose and same arguments as <c>RegisterHub()</c>, but does not trigger
     *   an error when the selected hub is not available at the time of the function call.
     *   This makes it possible to register a network hub independently of the current
     *   connectivity, and to try to contact it only when a device is actively needed.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="url">
     *   a string containing either <c>"usb"</c>,<c>"callback"</c> or the
     *   root URL of the hub to monitor
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static int PreregisterHub(string url, ref string errmsg)
    {
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        YRETCODE res;

        if (!_apiInitialized) {
            res = InitAPI(0, ref errmsg);
            if (YISERR(res))
                return res;
        }

        buffer.Length = 0;
        res = SafeNativeMethods._yapiPreregisterHub(new StringBuilder(url), buffer);
        if (YISERR(res)) {
            errmsg = buffer.ToString();
        }

        return res;
    }

    /**
     * <summary>
     *   Test if the hub is reachable.
     * <para>
     *   This method do not register the hub, it only test if the
     *   hub is usable. The url parameter follow the same convention as the <c>RegisterHub</c>
     *   method. This method is useful to verify the authentication parameters for a hub. It
     *   is possible to force this method to return after mstimeout milliseconds.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="url">
     *   a string containing either <c>"usb"</c>,<c>"callback"</c> or the
     *   root URL of the hub to monitor
     * </param>
     * <param name="mstimeout">
     *   the number of millisecond available to test the connection.
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure returns a negative error code.
     * </para>
     */
    public static int TestHub(string url, int mstimeout, ref string errmsg)
    {
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        YRETCODE res;

        buffer.Length = 0;
        res = SafeNativeMethods._yapiTestHub(new StringBuilder(url), mstimeout, buffer);
        if (YISERR(res)) {
            errmsg = buffer.ToString();
        }

        return res;
    }


    /**
     * <summary>
     *   Setup the Yoctopuce library to no more use modules connected on a previously
     *   registered machine with RegisterHub.
     * <para>
     * </para>
     * </summary>
     * <param name="url">
     *   a string containing either <c>"usb"</c> or the
     *   root URL of the hub to monitor
     * </param>
     */
    public static void UnregisterHub(string url)
    {
        if (!_apiInitialized) {
            return;
        }

        SafeNativeMethods._yapiUnregisterHub(new StringBuilder(url));
    }

    /**
     * <summary>
     *   Triggers a (re)detection of connected Yoctopuce modules.
     * <para>
     *   The library searches the machines or USB ports previously registered using
     *   <c>yRegisterHub()</c>, and invokes any user-defined callback function
     *   in case a change in the list of connected devices is detected.
     * </para>
     * <para>
     *   This function can be called as frequently as desired to refresh the device list
     *   and to make the application aware of hot-plug events. However, since device
     *   detection is quite a heavy process, UpdateDeviceList shouldn't be called more
     *   than once every two seconds.
     * </para>
     * </summary>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static YRETCODE UpdateDeviceList(ref string errmsg)
    {
        StringBuilder errbuffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        YRETCODE res = default(YRETCODE);
        PlugEvent p;

        if (!_apiInitialized) {
            res = InitAPI(0, ref errmsg);
            if (YISERR(res))
                return res;
        }

        res = yapiUpdateDeviceList(0, ref errmsg);
        if (YISERR(res)) {
            return res;
        }

        errbuffer.Length = 0;
        res = SafeNativeMethods._yapiHandleEvents(errbuffer);
        if (YISERR(res)) {
            errmsg = errbuffer.ToString();
            return res;
        }

        while (_PlugEvents.Count > 0) {
            yapiLockDeviceCallBack(ref errmsg);
            p = _PlugEvents[0];
            _PlugEvents.RemoveAt(0);
            yapiUnlockDeviceCallBack(ref errmsg);
            p.invoke();
        }

        return SUCCESS;
    }


    /**
     * <summary>
     *   Maintains the device-to-library communication channel.
     * <para>
     *   If your program includes significant loops, you may want to include
     *   a call to this function to make sure that the library takes care of
     *   the information pushed by the modules on the communication channels.
     *   This is not strictly necessary, but it may improve the reactivity
     *   of the library for the following commands.
     * </para>
     * <para>
     *   This function may signal an error in case there is a communication problem
     *   while contacting a module.
     * </para>
     * </summary>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static YRETCODE HandleEvents(ref string errmsg)
    {
        YRETCODE functionReturnValue = default(YRETCODE);

        StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        YRETCODE res = default(YRETCODE);


        errBuffer.Length = 0;
        res = SafeNativeMethods._yapiHandleEvents(errBuffer);

        if ((YISERR(res))) {
            errmsg = errBuffer.ToString();
            functionReturnValue = res;
            return functionReturnValue;
        }

        while ((_DataEvents.Count > 0)) {
            yapiLockFunctionCallBack(ref errmsg);
            if (_DataEvents.Count == 0) {
                yapiUnlockFunctionCallBack(ref errmsg);
                break;
            }

            DataEvent ev = _DataEvents[0];
            _DataEvents.RemoveAt(0);
            yapiUnlockFunctionCallBack(ref errmsg);
            ev.invoke();
        }

        functionReturnValue = SUCCESS;
        return functionReturnValue;
    }

    /**
     * <summary>
     *   Pauses the execution flow for a specified duration.
     * <para>
     *   This function implements a passive waiting loop, meaning that it does not
     *   consume CPU cycles significantly. The processor is left available for
     *   other threads and processes. During the pause, the library nevertheless
     *   reads from time to time information from the Yoctopuce modules by
     *   calling <c>yHandleEvents()</c>, in order to stay up-to-date.
     * </para>
     * <para>
     *   This function may signal an error in case there is a communication problem
     *   while contacting a module.
     * </para>
     * </summary>
     * <param name="ms_duration">
     *   an integer corresponding to the duration of the pause,
     *   in milliseconds.
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static int Sleep(int ms_duration, ref string errmsg)
    {
        int functionReturnValue = 0;

        StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        ulong timeout = 0;
        int res = 0;


        timeout = GetTickCount() + (ulong) ms_duration;
        res = SUCCESS;
        errBuffer.Length = 0;

        do {
            res = HandleEvents(ref errmsg);
            if ((YISERR(res))) {
                functionReturnValue = res;
                return functionReturnValue;
            }

            if ((GetTickCount() < timeout)) {
                res = SafeNativeMethods._yapiSleep(2, errBuffer);
                if ((YISERR(res))) {
                    functionReturnValue = res;
                    errmsg = errBuffer.ToString();
                    return functionReturnValue;
                }
            }
        } while (!(GetTickCount() >= timeout));

        errmsg = errBuffer.ToString();
        functionReturnValue = res;
        return functionReturnValue;
    }


    /**
     * <summary>
     *   Force a hub discovery, if a callback as been registered with <c>yRegisterHubDiscoveryCallback</c> it
     *   will be called for each net work hub that will respond to the discovery.
     * <para>
     * </para>
     * </summary>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public static int TriggerHubDiscovery(ref string errmsg)
    {
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        YRETCODE res;

        if (!_apiInitialized) {
            res = InitAPI(0, ref errmsg);
            if (YISERR(res))
                return res;
        }

        buffer.Length = 0;
        res = SafeNativeMethods._yapiTriggerHubDiscovery(buffer);
        if (YISERR(res)) {
            errmsg = buffer.ToString();
        }

        return res;
    }

    /**
     * <summary>
     *   Returns the current value of a monotone millisecond-based time counter.
     * <para>
     *   This counter can be used to compute delays in relation with
     *   Yoctopuce devices, which also uses the millisecond as timebase.
     * </para>
     * </summary>
     * <returns>
     *   a long integer corresponding to the millisecond counter.
     * </returns>
     */
    public static ulong GetTickCount()
    {
        return Convert.ToUInt64((ulong) SafeNativeMethods._yapiGetTickCount());
    }

    /**
     * <summary>
     *   Checks if a given string is valid as logical name for a module or a function.
     * <para>
     *   A valid logical name has a maximum of 19 characters, all among
     *   <c>A..Z</c>, <c>a..z</c>, <c>0..9</c>, <c>_</c>, and <c>-</c>.
     *   If you try to configure a logical name with an incorrect string,
     *   the invalid characters are ignored.
     * </para>
     * </summary>
     * <param name="name">
     *   a string containing the name to check.
     * </param>
     * <returns>
     *   <c>true</c> if the name is valid, <c>false</c> otherwise.
     * </returns>
     */
    public static bool CheckLogicalName(string name)
    {
        bool functionReturnValue = false;
        if ((SafeNativeMethods._yapiCheckLogicalName(new StringBuilder(name)) == 0)) {
            functionReturnValue = false;
        } else {
            functionReturnValue = true;
        }

        return functionReturnValue;
    }

    public static int yapiGetFunctionInfo(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, ref string serial, ref string funcId, ref string funcName, ref string funcVal, ref string errmsg)
    {
        int functionReturnValue = 0;

        StringBuilder serialBuffer = new StringBuilder(YOCTO_SERIAL_LEN);
        StringBuilder funcIdBuffer = new StringBuilder(YOCTO_FUNCTION_LEN);
        StringBuilder funcNameBuffer = new StringBuilder(YOCTO_LOGICAL_LEN);
        StringBuilder funcValBuffer = new StringBuilder(YOCTO_PUBVAL_LEN);
        StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);

        serialBuffer.Length = 0;
        funcIdBuffer.Length = 0;
        funcNameBuffer.Length = 0;
        funcValBuffer.Length = 0;
        errBuffer.Length = 0;

        functionReturnValue = SafeNativeMethods._yapiGetFunctionInfoEx(fundesc, ref devdesc, serialBuffer, funcIdBuffer, null, funcNameBuffer, funcValBuffer, errBuffer);
        serial = serialBuffer.ToString();
        funcId = funcIdBuffer.ToString();
        funcName = funcNameBuffer.ToString();
        funcVal = funcValBuffer.ToString();
        errmsg = funcValBuffer.ToString();
        return functionReturnValue;
    }

    public static int yapiGetFunctionInfoEx(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, ref string serial, ref string funcId, ref string baseType, ref string funcName, ref string funcVal, ref string errmsg)
    {
        int functionReturnValue = 0;

        StringBuilder serialBuffer = new StringBuilder(YOCTO_SERIAL_LEN);
        StringBuilder funcIdBuffer = new StringBuilder(YOCTO_FUNCTION_LEN);
        StringBuilder baseTypeBuffer = new StringBuilder(YOCTO_FUNCTION_LEN);
        StringBuilder funcNameBuffer = new StringBuilder(YOCTO_LOGICAL_LEN);
        StringBuilder funcValBuffer = new StringBuilder(YOCTO_PUBVAL_LEN);
        StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);

        serialBuffer.Length = 0;
        funcIdBuffer.Length = 0;
        funcNameBuffer.Length = 0;
        funcValBuffer.Length = 0;
        errBuffer.Length = 0;

        functionReturnValue = SafeNativeMethods._yapiGetFunctionInfoEx(fundesc, ref devdesc, serialBuffer, funcIdBuffer, baseTypeBuffer, funcNameBuffer, funcValBuffer, errBuffer);
        serial = serialBuffer.ToString();
        funcId = funcIdBuffer.ToString();
        baseType = baseTypeBuffer.ToString();
        funcName = funcNameBuffer.ToString();
        funcVal = funcValBuffer.ToString();
        errmsg = funcValBuffer.ToString();
        return functionReturnValue;
    }

    internal static int yapiGetDeviceByFunction(YFUN_DESCR fundesc, ref string errmsg)
    {
        int functionReturnValue = 0;
        StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        YDEV_DESCR devdesc = default(YDEV_DESCR);
        int res = 0;
        errBuffer.Length = 0;
        res = SafeNativeMethods._yapiGetFunctionInfoEx(fundesc, ref devdesc, null, null, null, null, null, errBuffer);
        errmsg = errBuffer.ToString();
        if ((res < 0)) {
            functionReturnValue = res;
        } else {
            functionReturnValue = devdesc;
        }

        return functionReturnValue;
    }

    public static u16 apiGetAPIVersion(ref string version, ref string date)
    {
        IntPtr pversion = default(IntPtr);
        IntPtr pdate = default(IntPtr);
        u16 res = default(u16);
        res = SafeNativeMethods.tryGetAPIVersion(ref pversion, ref pdate);
        version = Marshal.PtrToStringAnsi(pversion);
        date = Marshal.PtrToStringAnsi(pdate);
        return res;
    }


    internal static YRETCODE yapiUpdateDeviceList(uint force, ref string errmsg)
    {
        YRETCODE res = YAPI.SUCCESS;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        res = SafeNativeMethods._yapiUpdateDeviceList(force, buffer);
        if (YAPI.YISERR(res)) {
            errmsg = buffer.ToString();
        }

        return res;
    }

    protected static YDEV_DESCR yapiGetDevice(ref string device_str, string errmsg)
    {
        YDEV_DESCR functionReturnValue = default(YDEV_DESCR);
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiGetDevice(new StringBuilder(device_str), buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    internal static int yapiGetDeviceInfo(YDEV_DESCR d, ref SafeNativeMethods.yDeviceSt infos, ref string errmsg)
    {
        int functionReturnValue = 0;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiGetDeviceInfo(d, ref infos, buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    internal static YFUN_DESCR yapiGetFunction(string class_str, string function_str, ref string errmsg)
    {
        YFUN_DESCR functionReturnValue = default(YFUN_DESCR);
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiGetFunction(new StringBuilder(class_str), new StringBuilder(function_str), buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    public static int apiGetFunctionsByClass(string class_str, YFUN_DESCR precFuncDesc, IntPtr dbuffer, int maxsize, ref int neededsize, ref string errmsg)
    {
        int functionReturnValue = 0;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiGetFunctionsByClass(new StringBuilder(class_str), precFuncDesc, dbuffer, maxsize, ref neededsize, buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    protected static int apiGetFunctionsByDevice(YDEV_DESCR devdesc, YFUN_DESCR precFuncDesc, IntPtr dbuffer, int maxsize, ref int neededsize, ref string errmsg)
    {
        int functionReturnValue = 0;
        StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
        buffer.Length = 0;
        functionReturnValue = SafeNativeMethods._yapiGetFunctionsByDevice(devdesc, precFuncDesc, dbuffer, maxsize, ref neededsize, buffer);
        errmsg = buffer.ToString();
        return functionReturnValue;
    }

    //--- (generated code: YAPIContext yapiwrapper)
    /**
     * <summary>
     *   Modifies the delay between each forced enumeration of the used YoctoHubs.
     * <para>
     *   By default, the library performs a full enumeration every 10 seconds.
     *   To reduce network traffic, you can increase this delay.
     *   It's particularly useful when a YoctoHub is connected to the GSM network
     *   where traffic is billed. This parameter doesn't impact modules connected by USB,
     *   nor the working of module arrival/removal callbacks.
     *   Note: you must call this function after <c>yInitAPI</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="deviceListValidity">
     *   nubmer of seconds between each enumeration.
     * @noreturn
     * </param>
     */
    public static void SetDeviceListValidity(int deviceListValidity)
    {
        _yapiContext.SetDeviceListValidity(deviceListValidity);
    }
    /**
     * <summary>
     *   Returns the delay between each forced enumeration of the used YoctoHubs.
     * <para>
     *   Note: you must call this function after <c>yInitAPI</c>.
     * </para>
     * </summary>
     * <returns>
     *   the number of seconds between each enumeration.
     * </returns>
     */
    public static int GetDeviceListValidity()
    {
        return _yapiContext.GetDeviceListValidity();
    }
    /**
     * <summary>
     *   Modifies the network connection delay for <c>yRegisterHub()</c> and <c>yUpdateDeviceList()</c>.
     * <para>
     *   This delay impacts only the YoctoHubs and VirtualHub
     *   which are accessible through the network. By default, this delay is of 20000 milliseconds,
     *   but depending or you network you may want to change this delay.
     *   For example if your network infrastructure uses a GSM connection.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="networkMsTimeout">
     *   the network connection delay in milliseconds.
     * @noreturn
     * </param>
     */
    public static void SetNetworkTimeout(int networkMsTimeout)
    {
        _yapiContext.SetNetworkTimeout(networkMsTimeout);
    }
    /**
     * <summary>
     *   Returns the network connection delay for <c>yRegisterHub()</c> and <c>yUpdateDeviceList()</c>.
     * <para>
     *   This delay impacts only the YoctoHubs and VirtualHub
     *   which are accessible through the network. By default, this delay is of 20000 milliseconds,
     *   but depending or you network you may want to change this delay.
     *   For example if your network infrastructure uses a GSM connection.
     * </para>
     * </summary>
     * <returns>
     *   the network connection delay in milliseconds.
     * </returns>
     */
    public static int GetNetworkTimeout()
    {
        return _yapiContext.GetNetworkTimeout();
    }
    /**
     * <summary>
     *   Change the validity period of the data loaded by the library.
     * <para>
     *   By default, when accessing a module, all the attributes of the
     *   module functions are automatically kept in cache for the standard
     *   duration (5 ms). This method can be used to change this standard duration,
     *   for example in order to reduce network or USB traffic. This parameter
     *   does not affect value change callbacks
     *   Note: This function must be called after <c>yInitAPI</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="cacheValidityMs">
     *   an integer corresponding to the validity attributed to the
     *   loaded function parameters, in milliseconds.
     * @noreturn
     * </param>
     */
    public static void SetCacheValidity(ulong cacheValidityMs)
    {
        _yapiContext.SetCacheValidity(cacheValidityMs);
    }
    /**
     * <summary>
     *   Returns the validity period of the data loaded by the library.
     * <para>
     *   This method returns the cache validity of all attributes
     *   module functions.
     *   Note: This function must be called after <c>yInitAPI </c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the validity attributed to the
     *   loaded function parameters, in milliseconds
     * </returns>
     */
    public static ulong GetCacheValidity()
    {
        return _yapiContext.GetCacheValidity();
    }
//--- (end of generated code: YAPIContext yapiwrapper)
}


//--- (generated code: YAPIContext return codes)
    //--- (end of generated code: YAPIContext return codes)
//--- (generated code: YAPIContext dlldef)
//--- (end of generated code: YAPIContext dlldef)
//--- (generated code: YAPIContext class start)
public class YAPIContext
{
//--- (end of generated code: YAPIContext class start)

    //--- (generated code: YAPIContext definitions)

    protected ulong _defaultCacheValidity = 5;
    //--- (end of generated code: YAPIContext definitions)

    public YAPIContext()
    {
        //--- (generated code: YAPIContext attributes initialization)
        //--- (end of generated code: YAPIContext attributes initialization)
    }

    //--- (generated code: YAPIContext implementation)


    /**
     * <summary>
     *   Modifies the delay between each forced enumeration of the used YoctoHubs.
     * <para>
     *   By default, the library performs a full enumeration every 10 seconds.
     *   To reduce network traffic, you can increase this delay.
     *   It's particularly useful when a YoctoHub is connected to the GSM network
     *   where traffic is billed. This parameter doesn't impact modules connected by USB,
     *   nor the working of module arrival/removal callbacks.
     *   Note: you must call this function after <c>yInitAPI</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="deviceListValidity">
     *   nubmer of seconds between each enumeration.
     * @noreturn
     * </param>
     */
    public virtual void SetDeviceListValidity(int deviceListValidity)
    {
        SafeNativeMethods._yapiSetNetDevListValidity(deviceListValidity);
    }

    /**
     * <summary>
     *   Returns the delay between each forced enumeration of the used YoctoHubs.
     * <para>
     *   Note: you must call this function after <c>yInitAPI</c>.
     * </para>
     * </summary>
     * <returns>
     *   the number of seconds between each enumeration.
     * </returns>
     */
    public virtual int GetDeviceListValidity()
    {
        int res;
        res = SafeNativeMethods._yapiGetNetDevListValidity();
        return res;
    }

    /**
     * <summary>
     *   Modifies the network connection delay for <c>yRegisterHub()</c> and <c>yUpdateDeviceList()</c>.
     * <para>
     *   This delay impacts only the YoctoHubs and VirtualHub
     *   which are accessible through the network. By default, this delay is of 20000 milliseconds,
     *   but depending or you network you may want to change this delay.
     *   For example if your network infrastructure uses a GSM connection.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="networkMsTimeout">
     *   the network connection delay in milliseconds.
     * @noreturn
     * </param>
     */
    public virtual void SetNetworkTimeout(int networkMsTimeout)
    {
        SafeNativeMethods._yapiSetNetworkTimeout(networkMsTimeout);
    }

    /**
     * <summary>
     *   Returns the network connection delay for <c>yRegisterHub()</c> and <c>yUpdateDeviceList()</c>.
     * <para>
     *   This delay impacts only the YoctoHubs and VirtualHub
     *   which are accessible through the network. By default, this delay is of 20000 milliseconds,
     *   but depending or you network you may want to change this delay.
     *   For example if your network infrastructure uses a GSM connection.
     * </para>
     * </summary>
     * <returns>
     *   the network connection delay in milliseconds.
     * </returns>
     */
    public virtual int GetNetworkTimeout()
    {
        int res;
        res = SafeNativeMethods._yapiGetNetworkTimeout();
        return res;
    }

    /**
     * <summary>
     *   Change the validity period of the data loaded by the library.
     * <para>
     *   By default, when accessing a module, all the attributes of the
     *   module functions are automatically kept in cache for the standard
     *   duration (5 ms). This method can be used to change this standard duration,
     *   for example in order to reduce network or USB traffic. This parameter
     *   does not affect value change callbacks
     *   Note: This function must be called after <c>yInitAPI</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="cacheValidityMs">
     *   an integer corresponding to the validity attributed to the
     *   loaded function parameters, in milliseconds.
     * @noreturn
     * </param>
     */
    public virtual void SetCacheValidity(ulong cacheValidityMs)
    {
        this._defaultCacheValidity = cacheValidityMs;
    }

    /**
     * <summary>
     *   Returns the validity period of the data loaded by the library.
     * <para>
     *   This method returns the cache validity of all attributes
     *   module functions.
     *   Note: This function must be called after <c>yInitAPI </c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the validity attributed to the
     *   loaded function parameters, in milliseconds
     * </returns>
     */
    public virtual ulong GetCacheValidity()
    {
        return this._defaultCacheValidity;
    }

    //--- (end of generated code: YAPIContext implementation)

    //--- (generated code: YAPIContext functions)



    //--- (end of generated code: YAPIContext functions)
}


//--- (generated code: YFirmwareUpdate class start)
/**
 * <summary>
 *   The YFirmwareUpdate class let you control the firmware update of a Yoctopuce
 *   module.
 * <para>
 *   This class should not be instantiate directly, instead the method
 *   <c>updateFirmware</c> should be called to get an instance of YFirmwareUpdate.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YFirmwareUpdate
{
//--- (end of generated code: YFirmwareUpdate class start)

    public const double DATA_INVALID = YAPI.INVALID_DOUBLE;
    public const int DURATION_INVALID = -1;

    //--- (generated code: YFirmwareUpdate definitions)

    protected string _serial;
    protected byte[] _settings;
    protected string _firmwarepath;
    protected string _progress_msg;
    protected int _progress_c = 0;
    protected int _progress = 0;
    protected int _restore_step = 0;
    protected bool _force;
    //--- (end of generated code: YFirmwareUpdate definitions)


    public YFirmwareUpdate(string serial, string path, byte[] settings, bool force)
    {
        _serial = serial;
        _firmwarepath = path;
        _settings = settings;
        _force = force;
        //--- (generated code: YFirmwareUpdate attributes initialization)
        //--- (end of generated code: YFirmwareUpdate attributes initialization)
    }


    public YFirmwareUpdate(string serial, string path, byte[] settings)
    {
        _serial = serial;
        _firmwarepath = path;
        _settings = settings;
        _force = false;
        //--- (generated code: YFirmwareUpdate attributes initialization)
        //--- (end of generated code: YFirmwareUpdate attributes initialization)
    }


    //--- (generated code: YFirmwareUpdate implementation)


    public virtual int _processMore(int newupdate)
    {
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        YModule m;
        int res;
        string serial;
        string firmwarepath;
        string settings;
        string prod_prefix;
        int force;
        if ((this._progress_c < 100) && (this._progress_c != YAPI.VERSION_MISMATCH)) {
            serial = this._serial;
            firmwarepath = this._firmwarepath;
            settings = YAPI.DefaultEncoding.GetString(this._settings);
            if (this._force) {
                force = 1;
            } else {
                force = 0;
            }
            res = SafeNativeMethods._yapiUpdateFirmwareEx(new StringBuilder(serial), new StringBuilder(firmwarepath), new StringBuilder(settings), force, newupdate, errmsg);
            if ((res == YAPI.VERSION_MISMATCH) && ((this._settings).Length != 0)) {
                this._progress_c = res;
                this._progress_msg = errmsg.ToString();
                return this._progress;
            }
            if (res < 0) {
                this._progress = res;
                this._progress_msg = errmsg.ToString();
                return res;
            }
            this._progress_c = res;
            this._progress = ((this._progress_c * 9) / (10));
            this._progress_msg = errmsg.ToString();
        } else {
            if (((this._settings).Length != 0)) {
                this._progress_msg = "restoring settings";
                m = YModule.FindModule(this._serial + ".module");
                if (!(m.isOnline())) {
                    return this._progress;
                }
                if (this._progress < 95) {
                    prod_prefix = (m.get_productName()).Substring( 0, 8);
                    if (prod_prefix == "YoctoHub") {
                        {string ignore=""; YAPI.Sleep(1000, ref ignore);};
                        this._progress = this._progress + 1;
                        return this._progress;
                    } else {
                        this._progress = 95;
                    }
                }
                if (this._progress < 100) {
                    m.set_allSettingsAndFiles(this._settings);
                    m.saveToFlash();
                    this._settings = new byte[0];
                    if (this._progress_c == YAPI.VERSION_MISMATCH) {
                        this._progress = YAPI.IO_ERROR;
                        this._progress_msg = "Unable to update firmware";
                    } else {
                        this._progress = 100;
                        this._progress_msg = "success";
                    }
                }
            } else {
                this._progress = 100;
                this._progress_msg = "success";
            }
        }
        return this._progress;
    }

    /**
     * <summary>
     *   Returns a list of all the modules in "firmware update" mode.
     * <para>
     *   Only devices
     *   connected over USB are listed. For devices connected to a YoctoHub, you
     *   must connect yourself to the YoctoHub web interface.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an array of strings containing the serial numbers of devices in "firmware update" mode.
     * </returns>
     */
    public static List<string> GetAllBootLoaders()
    {
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        StringBuilder smallbuff = new StringBuilder(1024);
        StringBuilder bigbuff = null;
        int buffsize;
        int fullsize;
        int yapi_res;
        string bootloader_list;
        List<string> bootladers = new List<string>();
        fullsize = 0;
        yapi_res = SafeNativeMethods._yapiGetBootloaders(smallbuff, 1024, ref fullsize, errmsg);
        if (yapi_res < 0) {
            return bootladers;
        }
        if (fullsize <= 1024) {
            bootloader_list = smallbuff.ToString();
        } else {
            buffsize = fullsize;
            bigbuff = new StringBuilder(buffsize);
            yapi_res = SafeNativeMethods._yapiGetBootloaders(bigbuff, buffsize, ref fullsize, errmsg);
            if (yapi_res < 0) {
                bigbuff = null;
                return bootladers;
            } else {
                bootloader_list = bigbuff.ToString();
            }
            bigbuff = null;
        }
        if (!(bootloader_list == "")) {
            bootladers = new List<string>(bootloader_list.Split(new Char[] {','}));
        }
        return bootladers;
    }

    /**
     * <summary>
     *   Test if the byn file is valid for this module.
     * <para>
     *   It is possible to pass a directory instead of a file.
     *   In that case, this method returns the path of the most recent appropriate byn file. This method will
     *   ignore any firmware older than minrelease.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="serial">
     *   the serial number of the module to update
     * </param>
     * <param name="path">
     *   the path of a byn file or a directory that contains byn files
     * </param>
     * <param name="minrelease">
     *   a positive integer
     * </param>
     * <returns>
     *   : the path of the byn file to use, or an empty string if no byn files matches the requirement
     * </returns>
     * <para>
     *   On failure, returns a string that starts with "error:".
     * </para>
     */
    public static string CheckFirmware(string serial, string path, int minrelease)
    {
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        StringBuilder smallbuff = new StringBuilder(1024);
        StringBuilder bigbuff = null;
        int buffsize;
        int fullsize;
        int res;
        string firmware_path;
        string release;
        fullsize = 0;
        release = (minrelease).ToString();
        res = SafeNativeMethods._yapiCheckFirmware(new StringBuilder(serial), new StringBuilder(release), new StringBuilder(path), smallbuff, 1024, ref fullsize, errmsg);
        if (res < 0) {
            firmware_path = "error:" + errmsg.ToString();
            return "error:" + errmsg.ToString();
        }
        if (fullsize <= 1024) {
            firmware_path = smallbuff.ToString();
        } else {
            buffsize = fullsize;
            bigbuff = new StringBuilder(buffsize);
            res = SafeNativeMethods._yapiCheckFirmware(new StringBuilder(serial), new StringBuilder(release), new StringBuilder(path), bigbuff, buffsize, ref fullsize, errmsg);
            if (res < 0) {
                firmware_path = "error:" + errmsg.ToString();
            } else {
                firmware_path = bigbuff.ToString();
            }
            bigbuff = null;
        }
        return firmware_path;
    }

    /**
     * <summary>
     *   Returns the progress of the firmware update, on a scale from 0 to 100.
     * <para>
     *   When the object is
     *   instantiated, the progress is zero. The value is updated during the firmware update process until
     *   the value of 100 is reached. The 100 value means that the firmware update was completed
     *   successfully. If an error occurs during the firmware update, a negative value is returned, and the
     *   error message can be retrieved with <c>get_progressMessage</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer in the range 0 to 100 (percentage of completion)
     *   or a negative error code in case of failure.
     * </returns>
     */
    public virtual int get_progress()
    {
        if (this._progress >= 0) {
            this._processMore(0);
        }
        return this._progress;
    }

    /**
     * <summary>
     *   Returns the last progress message of the firmware update process.
     * <para>
     *   If an error occurs during the
     *   firmware update process, the error message is returned
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string  with the latest progress message, or the error message.
     * </returns>
     */
    public virtual string get_progressMessage()
    {
        return this._progress_msg;
    }

    /**
     * <summary>
     *   Starts the firmware update process.
     * <para>
     *   This method starts the firmware update process in background. This method
     *   returns immediately. You can monitor the progress of the firmware update with the <c>get_progress()</c>
     *   and <c>get_progressMessage()</c> methods.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer in the range 0 to 100 (percentage of completion),
     *   or a negative error code in case of failure.
     * </returns>
     * <para>
     *   On failure returns a negative error code.
     * </para>
     */
    public virtual int startUpdate()
    {
        string err;
        int leng;
        err = YAPI.DefaultEncoding.GetString(this._settings);
        leng = (err).Length;
        if ((leng >= 6) && ("error:" == (err).Substring(0, 6))) {
            this._progress = -1;
            this._progress_msg = (err).Substring( 6, leng - 6);
        } else {
            this._progress = 0;
            this._progress_c = 0;
            this._processMore(1);
        }
        return this._progress;
    }

    //--- (end of generated code: YFirmwareUpdate implementation)
}


//--- (generated code: YDataStream class start)
/**
 * <summary>
 *   YDataStream objects represent bare recorded measure sequences,
 *   exactly as found within the data logger present on Yoctopuce
 *   sensors.
 * <para>
 * </para>
 * <para>
 *   In most cases, it is not necessary to use YDataStream objects
 *   directly, as the YDataSet objects (returned by the
 *   <c>get_recordedData()</c> method from sensors and the
 *   <c>get_dataSets()</c> method from the data logger) provide
 *   a more convenient interface.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YDataStream
{
//--- (end of generated code: YDataStream class start)

    public const double DATA_INVALID = YAPI.INVALID_DOUBLE;
    public const int DURATION_INVALID = -1;

    //--- (generated code: YDataStream definitions)

    protected YFunction _parent;
    protected int _runNo = 0;
    protected long _utcStamp = 0;
    protected int _nCols = 0;
    protected int _nRows = 0;
    protected double _startTime = 0;
    protected double _duration = 0;
    protected double _dataSamplesInterval = 0;
    protected double _firstMeasureDuration = 0;
    protected List<string> _columnNames = new List<string>();
    protected string _functionId;
    protected bool _isClosed;
    protected bool _isAvg;
    protected double _minVal = 0;
    protected double _avgVal = 0;
    protected double _maxVal = 0;
    protected int _caltyp = 0;
    protected List<int> _calpar = new List<int>();
    protected List<double> _calraw = new List<double>();
    protected List<double> _calref = new List<double>();
    protected List<List<double>> _values = new List<List<double>>();
    //--- (end of generated code: YDataStream definitions)

    protected YAPI.yCalibrationHandler _calhdl;

    public YDataStream(YFunction parent)
    {
        this._parent = parent;
        //--- (generated code: YDataStream attributes initialization)
        //--- (end of generated code: YDataStream attributes initialization)
    }

    public YDataStream(YFunction parent, YDataSet dataset, List<int> encoded)
    {
        this._parent = parent;
        //--- (generated code: YDataStream attributes initialization)
        //--- (end of generated code: YDataStream attributes initialization)
        this._initFromDataSet(dataset, encoded);
    }


    //--- (generated code: YDataStream implementation)


    public int _initFromDataSet(YDataSet dataset, List<int> encoded)
    {
        int val;
        int i;
        int maxpos;
        int ms_offset;
        int samplesPerHour;
        double fRaw;
        double fRef;
        List<int> iCalib = new List<int>();
        // decode sequence header to extract data
        this._runNo = encoded[0] + (((encoded[1]) << (16)));
        this._utcStamp = encoded[2] + (((encoded[3]) << (16)));
        val = encoded[4];
        this._isAvg = (((val) & (0x100)) == 0);
        samplesPerHour = ((val) & (0xff));
        if (((val) & (0x100)) != 0) {
            samplesPerHour = samplesPerHour * 3600;
        } else {
            if (((val) & (0x200)) != 0) {
                samplesPerHour = samplesPerHour * 60;
            }
        }
        this._dataSamplesInterval = 3600.0 / samplesPerHour;
        ms_offset = encoded[6];
        if (ms_offset < 1000) {
            // new encoding . add the ms to the UTC timestamp
            this._startTime = this._utcStamp + (ms_offset / 1000.0);
        } else {
            // legacy encoding subtract the measure interval form the UTC timestamp
            this._startTime = this._utcStamp -  this._dataSamplesInterval;
        }
        this._firstMeasureDuration = encoded[5];
        if (!(this._isAvg)) {
            this._firstMeasureDuration = this._firstMeasureDuration / 1000.0;
        }
        val = encoded[7];
        this._isClosed = (val != 0xffff);
        if (val == 0xffff) {
            val = 0;
        }
        this._nRows = val;
        if (this._nRows > 0) {
            if (this._firstMeasureDuration > 0) {
                this._duration = this._firstMeasureDuration + (this._nRows - 1) * this._dataSamplesInterval;
            } else {
                this._duration = this._nRows * this._dataSamplesInterval;
            }
        } else {
            this._duration = 0;
        }
        // precompute decoding parameters
        iCalib = dataset._get_calibration();
        this._caltyp = iCalib[0];
        if (this._caltyp != 0) {
            this._calhdl = YAPI._getCalibrationHandler(this._caltyp);
            maxpos = iCalib.Count;
            this._calpar.Clear();
            this._calraw.Clear();
            this._calref.Clear();
            i = 1;
            while (i < maxpos) {
                this._calpar.Add(iCalib[i]);
                i = i + 1;
            }
            i = 1;
            while (i + 1 < maxpos) {
                fRaw = iCalib[i];
                fRaw = fRaw / 1000.0;
                fRef = iCalib[i + 1];
                fRef = fRef / 1000.0;
                this._calraw.Add(fRaw);
                this._calref.Add(fRef);
                i = i + 2;
            }
        }
        // preload column names for backward-compatibility
        this._functionId = dataset.get_functionId();
        if (this._isAvg) {
            this._columnNames.Clear();
            this._columnNames.Add(""+this._functionId+"_min");
            this._columnNames.Add(""+this._functionId+"_avg");
            this._columnNames.Add(""+this._functionId+"_max");
            this._nCols = 3;
        } else {
            this._columnNames.Clear();
            this._columnNames.Add(this._functionId);
            this._nCols = 1;
        }
        // decode min/avg/max values for the sequence
        if (this._nRows > 0) {
            this._avgVal = this._decodeAvg(encoded[8] + (((((encoded[9]) ^ (0x8000))) << (16))), 1);
            this._minVal = this._decodeVal(encoded[10] + (((encoded[11]) << (16))));
            this._maxVal = this._decodeVal(encoded[12] + (((encoded[13]) << (16))));
        }
        return 0;
    }

    public virtual int _parseStream(byte[] sdata)
    {
        int idx;
        List<int> udat = new List<int>();
        List<double> dat = new List<double>();
        if ((sdata).Length == 0) {
            this._nRows = 0;
            return YAPI.SUCCESS;
        }

        udat = YAPI._decodeWords(this._parent._json_get_string(sdata));
        this._values.Clear();
        idx = 0;
        if (this._isAvg) {
            while (idx + 3 < udat.Count) {
                dat.Clear();
                dat.Add(this._decodeVal(udat[idx + 2] + (((udat[idx + 3]) << (16)))));
                dat.Add(this._decodeAvg(udat[idx] + (((((udat[idx + 1]) ^ (0x8000))) << (16))), 1));
                dat.Add(this._decodeVal(udat[idx + 4] + (((udat[idx + 5]) << (16)))));
                idx = idx + 6;
                this._values.Add(new List<double>(dat));
            }
        } else {
            while (idx + 1 < udat.Count) {
                dat.Clear();
                dat.Add(this._decodeAvg(udat[idx] + (((((udat[idx + 1]) ^ (0x8000))) << (16))), 1));
                this._values.Add(new List<double>(dat));
                idx = idx + 2;
            }
        }

        this._nRows = this._values.Count;
        return YAPI.SUCCESS;
    }

    public virtual string _get_url()
    {
        string url;
        url = "logger.json?id="+
        this._functionId+"&run="+Convert.ToString(this._runNo)+"&utc="+Convert.ToString(this._utcStamp);
        return url;
    }

    public virtual int loadStream()
    {
        return this._parseStream(this._parent._download(this._get_url()));
    }

    public double _decodeVal(int w)
    {
        double val;
        val = w;
        val = val / 1000.0;
        if (this._caltyp != 0) {
            if (this._calhdl != null) {
                val = this._calhdl(val, this._caltyp, this._calpar, this._calraw, this._calref);
            }
        }
        return val;
    }

    public double _decodeAvg(int dw, int count)
    {
        double val;
        val = dw;
        val = val / 1000.0;
        if (this._caltyp != 0) {
            if (this._calhdl != null) {
                val = this._calhdl(val, this._caltyp, this._calpar, this._calraw, this._calref);
            }
        }
        return val;
    }

    public virtual bool isClosed()
    {
        return this._isClosed;
    }

    /**
     * <summary>
     *   Returns the run index of the data stream.
     * <para>
     *   A run can be made of
     *   multiple datastreams, for different time intervals.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the run index.
     * </returns>
     */
    public virtual int get_runIndex()
    {
        return this._runNo;
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
     *   If you need an absolute UTC timestamp, use <c>get_realStartTimeUTC()</c>.
     * </para>
     * <para>
     *   <b>DEPRECATED</b>: This method has been replaced by <c>get_realStartTimeUTC()</c>.
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
    public virtual int get_startTime()
    {
        return (int)(this._utcStamp - Convert.ToUInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds));
    }

    /**
     * <summary>
     *   Returns the start time of the data stream, relative to the Jan 1, 1970.
     * <para>
     *   If the UTC time was not set in the datalogger at the time of the recording
     *   of this data stream, this method returns 0.
     * </para>
     * <para>
     *   <b>DEPRECATED</b>: This method has been replaced by <c>get_realStartTimeUTC()</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of seconds
     *   between the Jan 1, 1970 and the beginning of this data
     *   stream (i.e. Unix time representation of the absolute time).
     * </returns>
     */
    public virtual long get_startTimeUTC()
    {
        return (int) Math.Round(this._startTime);
    }

    /**
     * <summary>
     *   Returns the start time of the data stream, relative to the Jan 1, 1970.
     * <para>
     *   If the UTC time was not set in the datalogger at the time of the recording
     *   of this data stream, this method returns 0.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number  corresponding to the number of seconds
     *   between the Jan 1, 1970 and the beginning of this data
     *   stream (i.e. Unix time representation of the absolute time).
     * </returns>
     */
    public virtual double get_realStartTimeUTC()
    {
        return this._startTime;
    }

    /**
     * <summary>
     *   Returns the number of milliseconds between two consecutive
     *   rows of this data stream.
     * <para>
     *   By default, the data logger records one row
     *   per second, but the recording frequency can be changed for
     *   each device function
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to a number of milliseconds.
     * </returns>
     */
    public virtual int get_dataSamplesIntervalMs()
    {
        return (int) Math.Round(this._dataSamplesInterval*1000);
    }

    public virtual double get_dataSamplesInterval()
    {
        return this._dataSamplesInterval;
    }

    public virtual double get_firstDataSamplesInterval()
    {
        return this._firstMeasureDuration;
    }

    /**
     * <summary>
     *   Returns the number of data rows present in this stream.
     * <para>
     * </para>
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method fetches the whole data stream from the device
     *   if not yet done, which can cause a little delay.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of rows.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns zero.
     * </para>
     */
    public virtual int get_rowCount()
    {
        if ((this._nRows != 0) && this._isClosed) {
            return this._nRows;
        }
        this.loadStream();
        return this._nRows;
    }

    /**
     * <summary>
     *   Returns the number of data columns present in this stream.
     * <para>
     *   The meaning of the values present in each column can be obtained
     *   using the method <c>get_columnNames()</c>.
     * </para>
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method fetches the whole data stream from the device
     *   if not yet done, which can cause a little delay.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of columns.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns zero.
     * </para>
     */
    public virtual int get_columnCount()
    {
        if (this._nCols != 0) {
            return this._nCols;
        }
        this.loadStream();
        return this._nCols;
    }

    /**
     * <summary>
     *   Returns the title (or meaning) of each data column present in this stream.
     * <para>
     *   In most case, the title of the data column is the hardware identifier
     *   of the sensor that produced the data. For streams recorded at a lower
     *   recording rate, the dataLogger stores the min, average and max value
     *   during each measure interval into three columns with suffixes _min,
     *   _avg and _max respectively.
     * </para>
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method fetches the whole data stream from the device
     *   if not yet done, which can cause a little delay.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a list containing as many strings as there are columns in the
     *   data stream.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<string> get_columnNames()
    {
        if (this._columnNames.Count != 0) {
            return this._columnNames;
        }
        this.loadStream();
        return this._columnNames;
    }

    /**
     * <summary>
     *   Returns the smallest measure observed within this stream.
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method will always return YDataStream.DATA_INVALID.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the smallest value,
     *   or YDataStream.DATA_INVALID if the stream is not yet complete (still recording).
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DATA_INVALID.
     * </para>
     */
    public virtual double get_minValue()
    {
        return this._minVal;
    }

    /**
     * <summary>
     *   Returns the average of all measures observed within this stream.
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method will always return YDataStream.DATA_INVALID.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the average value,
     *   or YDataStream.DATA_INVALID if the stream is not yet complete (still recording).
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DATA_INVALID.
     * </para>
     */
    public virtual double get_averageValue()
    {
        return this._avgVal;
    }

    /**
     * <summary>
     *   Returns the largest measure observed within this stream.
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method will always return YDataStream.DATA_INVALID.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the largest value,
     *   or YDataStream.DATA_INVALID if the stream is not yet complete (still recording).
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DATA_INVALID.
     * </para>
     */
    public virtual double get_maxValue()
    {
        return this._maxVal;
    }

    public virtual double get_realDuration()
    {
        if (this._isClosed) {
            return this._duration;
        }
        return (double) Convert.ToUInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) - this._utcStamp;
    }

    /**
     * <summary>
     *   Returns the whole data set contained in the stream, as a bidimensional
     *   table of numbers.
     * <para>
     *   The meaning of the values present in each column can be obtained
     *   using the method <c>get_columnNames()</c>.
     * </para>
     * <para>
     *   This method fetches the whole data stream from the device,
     *   if not yet done.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a list containing as many elements as there are rows in the
     *   data stream. Each row itself is a list of floating-point
     *   numbers.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<List<double>> get_dataRows()
    {
        if ((this._values.Count == 0) || !(this._isClosed)) {
            this.loadStream();
        }
        return this._values;
    }

    /**
     * <summary>
     *   Returns a single measure from the data stream, specified by its
     *   row and column index.
     * <para>
     *   The meaning of the values present in each column can be obtained
     *   using the method get_columnNames().
     * </para>
     * <para>
     *   This method fetches the whole data stream from the device,
     *   if not yet done.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="row">
     *   row index
     * </param>
     * <param name="col">
     *   column index
     * </param>
     * <returns>
     *   a floating-point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DATA_INVALID.
     * </para>
     */
    public virtual double get_data(int row, int col)
    {
        if ((this._values.Count == 0) || !(this._isClosed)) {
            this.loadStream();
        }
        if (row >= this._values.Count) {
            return DATA_INVALID;
        }
        if (col >= this._values[row].Count) {
            return DATA_INVALID;
        }
        return this._values[row][col];
    }

    //--- (end of generated code: YDataStream implementation)
}


//--- (generated code: YMeasure class start)
/**
 * <summary>
 *   YMeasure objects are used within the API to represent
 *   a value measured at a specified time.
 * <para>
 *   These objects are
 *   used in particular in conjunction with the YDataSet class.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YMeasure
{
//--- (end of generated code: YMeasure class start)
    //--- (generated code: YMeasure definitions)

    protected double _start = 0;
    protected double _end = 0;
    protected double _minVal = 0;
    protected double _avgVal = 0;
    protected double _maxVal = 0;
    //--- (end of generated code: YMeasure definitions)
    protected DateTime _start_datetime;
    protected DateTime _end_datetime;
    private static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public YMeasure(double start, double end, double minVal, double avgVal, double maxVal)
    {
        //--- (generated code: YMeasure attributes initialization)
        //--- (end of generated code: YMeasure attributes initialization)
        this._start = start;
        this._end = end;
        this._minVal = minVal;
        this._avgVal = avgVal;
        this._maxVal = maxVal;
        this._start_datetime = _epoch.AddSeconds(this._start);
        this._end_datetime = _epoch.AddSeconds(this._end);
    }

    public YMeasure()
    { }

    public DateTime get_startTimeUTC_asDateTime()
    {
        return this._start_datetime;
    }

    public DateTime get_endTimeUTC_asDateTime()
    {
        return this._end_datetime;
    }


    //--- (generated code: YMeasure implementation)


    /**
     * <summary>
     *   Returns the start time of the measure, relative to the Jan 1, 1970 UTC
     *   (Unix timestamp).
     * <para>
     *   When the recording rate is higher then 1 sample
     *   per second, the timestamp may have a fractional part.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an floating point number corresponding to the number of seconds
     *   between the Jan 1, 1970 UTC and the beginning of this measure.
     * </returns>
     */
    public virtual double get_startTimeUTC()
    {
        return this._start;
    }

    /**
     * <summary>
     *   Returns the end time of the measure, relative to the Jan 1, 1970 UTC
     *   (Unix timestamp).
     * <para>
     *   When the recording rate is higher than 1 sample
     *   per second, the timestamp may have a fractional part.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an floating point number corresponding to the number of seconds
     *   between the Jan 1, 1970 UTC and the end of this measure.
     * </returns>
     */
    public virtual double get_endTimeUTC()
    {
        return this._end;
    }

    /**
     * <summary>
     *   Returns the smallest value observed during the time interval
     *   covered by this measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the smallest value observed.
     * </returns>
     */
    public virtual double get_minValue()
    {
        return this._minVal;
    }

    /**
     * <summary>
     *   Returns the average value observed during the time interval
     *   covered by this measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the average value observed.
     * </returns>
     */
    public virtual double get_averageValue()
    {
        return this._avgVal;
    }

    /**
     * <summary>
     *   Returns the largest value observed during the time interval
     *   covered by this measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the largest value observed.
     * </returns>
     */
    public virtual double get_maxValue()
    {
        return this._maxVal;
    }

    //--- (end of generated code: YMeasure implementation)
}

//--- (generated code: YDataSet class start)
/**
 * <summary>
 *   YDataSet objects make it possible to retrieve a set of recorded measures
 *   for a given sensor and a specified time interval.
 * <para>
 *   They can be used
 *   to load data points with a progress report. When the YDataSet object is
 *   instantiated by the <c>get_recordedData()</c>  function, no data is
 *   yet loaded from the module. It is only when the <c>loadMore()</c>
 *   method is called over and over than data will be effectively loaded
 *   from the dataLogger.
 * </para>
 * <para>
 *   A preview of available measures is available using the function
 *   <c>get_preview()</c> as soon as <c>loadMore()</c> has been called
 *   once. Measures themselves are available using function <c>get_measures()</c>
 *   when loaded by subsequent calls to <c>loadMore()</c>.
 * </para>
 * <para>
 *   This class can only be used on devices that use a recent firmware,
 *   as YDataSet objects are not supported by firmwares older than version 13000.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YDataSet
{
//--- (end of generated code: YDataSet class start)
    //--- (generated code: YDataSet definitions)

    protected YFunction _parent;
    protected string _hardwareId;
    protected string _functionId;
    protected string _unit;
    protected double _startTimeMs = 0;
    protected double _endTimeMs = 0;
    protected int _progress = 0;
    protected List<int> _calib = new List<int>();
    protected List<YDataStream> _streams = new List<YDataStream>();
    protected YMeasure _summary;
    protected List<YMeasure> _preview = new List<YMeasure>();
    protected List<YMeasure> _measures = new List<YMeasure>();
    protected double _summaryMinVal = 0;
    protected double _summaryMaxVal = 0;
    protected double _summaryTotalAvg = 0;
    protected double _summaryTotalTime = 0;
    //--- (end of generated code: YDataSet definitions)

    // YDataSet constructor, when instantiated directly by a function
    public YDataSet(YFunction parent, string functionId, string unit, double startTime, double endTime)
    {
        //--- (generated code: YDataSet attributes initialization)
        //--- (end of generated code: YDataSet attributes initialization)
        this._parent = parent;
        this._functionId = functionId;
        this._unit = unit;
        this._startTimeMs = startTime * 1000;
        this._endTimeMs = endTime * 1000;
        this._progress = -1;
        this._summary = new YMeasure();
    }

    // YDataSet constructor for the new datalogger
    public YDataSet(YFunction parent)
    {
        //--- (generated code: YDataSet attributes initialization)
        //--- (end of generated code: YDataSet attributes initialization)
        this._parent = parent;
        this._startTimeMs = 0;
        this._endTimeMs = 0;
        this._summary = new YMeasure();
    }

    public int _parse(string data)
    {
        YAPI.YJSONObject p = new YAPI.YJSONObject(data);
        YAPI.YJSONArray arr;

        if (!YAPI.ExceptionsDisabled) {
            p.parse();
        } else {
            try {
                p.parse();
            } catch {
                return YAPI.IO_ERROR;
            }
        }

        YDataStream stream;
        double streamStartTime;
        double streamEndTime;

        this._functionId = p.getString("id");
        this._unit = p.getString("unit");
        if (p.has("calib")) {
            this._calib = YAPI._decodeFloats(p.getString("calib"));
            this._calib[0] = this._calib[0] / 1000;
        } else {
            this._calib = YAPI._decodeWords(p.getString("cal"));
        }

        arr = p.getYJSONArray("streams");
        this._streams = new List<YDataStream>();
        this._preview = new List<YMeasure>();
        this._measures = new List<YMeasure>();
        for (int i = 0; i < arr.Length; i++) {
            stream = _parent._findDataStream(this, arr.getString(i));
            streamStartTime = Math.Round(stream.get_realStartTimeUTC() * 1000);
            streamEndTime = streamStartTime + Math.Round(stream.get_realDuration() * 1000);
            if (_startTimeMs > 0 && streamEndTime <= _startTimeMs) {
                // this stream is too early, drop it
            } else if (_endTimeMs > 0 && streamStartTime >= this._endTimeMs) {
                // this stream is too late, drop it
            } else {
                _streams.Add(stream);
            }
        }

        this._progress = 0;
        return this.get_progress();
    }

    //--- (generated code: YDataSet implementation)


    public virtual List<int> _get_calibration()
    {
        return this._calib;
    }

    public virtual int loadSummary(byte[] data)
    {
        List<List<double>> dataRows = new List<List<double>>();
        double tim;
        double mitv;
        double itv;
        double fitv;
        double end_;
        int nCols;
        int minCol;
        int avgCol;
        int maxCol;
        int res;
        int m_pos;
        double previewTotalTime;
        double previewTotalAvg;
        double previewMinVal;
        double previewMaxVal;
        double previewAvgVal;
        double previewStartMs;
        double previewStopMs;
        double previewDuration;
        double streamStartTimeMs;
        double streamDuration;
        double streamEndTimeMs;
        double minVal;
        double avgVal;
        double maxVal;
        double summaryStartMs;
        double summaryStopMs;
        double summaryTotalTime;
        double summaryTotalAvg;
        double summaryMinVal;
        double summaryMaxVal;
        string url;
        string strdata;
        List<double> measure_data = new List<double>();

        if (this._progress < 0) {
            strdata = YAPI.DefaultEncoding.GetString(data);
            if (strdata == "{}") {
                this._parent._throw(YAPI.VERSION_MISMATCH, "device firmware is too old");
                return YAPI.VERSION_MISMATCH;
            }
            res = this._parse(strdata);
            if (res < 0) {
                return res;
            }
        }
        summaryTotalTime = 0;
        summaryTotalAvg = 0;
        summaryMinVal = YAPI.MAX_DOUBLE;
        summaryMaxVal = YAPI.MIN_DOUBLE;
        summaryStartMs = YAPI.MAX_DOUBLE;
        summaryStopMs = YAPI.MIN_DOUBLE;

        // Parse complete streams
        for (int ii = 0; ii <  this._streams.Count; ii++) {
            streamStartTimeMs = Math.Round( this._streams[ii].get_realStartTimeUTC() *1000);
            streamDuration =  this._streams[ii].get_realDuration() ;
            streamEndTimeMs = streamStartTimeMs + Math.Round(streamDuration * 1000);
            if ((streamStartTimeMs >= this._startTimeMs) && ((this._endTimeMs == 0) || (streamEndTimeMs <= this._endTimeMs))) {
                // stream that are completely inside the dataset
                previewMinVal =  this._streams[ii].get_minValue();
                previewAvgVal =  this._streams[ii].get_averageValue();
                previewMaxVal =  this._streams[ii].get_maxValue();
                previewStartMs = streamStartTimeMs;
                previewStopMs = streamEndTimeMs;
                previewDuration = streamDuration;
            } else {
                // stream that are partially in the dataset
                // we need to parse data to filter value outside the dataset
                url =  this._streams[ii]._get_url();
                data = this._parent._download(url);
                this._streams[ii]._parseStream(data);
                dataRows =  this._streams[ii].get_dataRows();
                if (dataRows.Count == 0) {
                    return this.get_progress();
                }
                tim = streamStartTimeMs;
                fitv = Math.Round( this._streams[ii].get_firstDataSamplesInterval() * 1000);
                itv = Math.Round( this._streams[ii].get_dataSamplesInterval() * 1000);
                nCols = dataRows[0].Count;
                minCol = 0;
                if (nCols > 2) {
                    avgCol = 1;
                } else {
                    avgCol = 0;
                }
                if (nCols > 2) {
                    maxCol = 2;
                } else {
                    maxCol = 0;
                }
                previewTotalTime = 0;
                previewTotalAvg = 0;
                previewStartMs = streamEndTimeMs;
                previewStopMs = streamStartTimeMs;
                previewMinVal = YAPI.MAX_DOUBLE;
                previewMaxVal = YAPI.MIN_DOUBLE;
                m_pos = 0;
                while (m_pos < dataRows.Count) {
                    measure_data  = dataRows[m_pos];
                    if (m_pos == 0) {
                        mitv = fitv;
                    } else {
                        mitv = itv;
                    }
                    end_ = tim + mitv;
                    if ((end_ > this._startTimeMs) && ((this._endTimeMs == 0) || (tim < this._endTimeMs))) {
                        minVal = measure_data[minCol];
                        avgVal = measure_data[avgCol];
                        maxVal = measure_data[maxCol];
                        if (previewStartMs > tim) {
                            previewStartMs = tim;
                        }
                        if (previewStopMs < end_) {
                            previewStopMs = end_;
                        }
                        if (previewMinVal > minVal) {
                            previewMinVal = minVal;
                        }
                        if (previewMaxVal < maxVal) {
                            previewMaxVal = maxVal;
                        }
                        previewTotalAvg = previewTotalAvg + (avgVal * mitv);
                        previewTotalTime = previewTotalTime + mitv;
                    }
                    tim = end_;
                    m_pos = m_pos + 1;
                }
                if (previewTotalTime > 0) {
                    previewAvgVal = previewTotalAvg / previewTotalTime;
                    previewDuration = (previewStopMs - previewStartMs) / 1000.0;
                } else {
                    previewAvgVal = 0.0;
                    previewDuration = 0.0;
                }
            }
            this._preview.Add(new YMeasure(previewStartMs / 1000.0, previewStopMs / 1000.0, previewMinVal, previewAvgVal, previewMaxVal));
            if (summaryMinVal > previewMinVal) {
                summaryMinVal = previewMinVal;
            }
            if (summaryMaxVal < previewMaxVal) {
                summaryMaxVal = previewMaxVal;
            }
            if (summaryStartMs > previewStartMs) {
                summaryStartMs = previewStartMs;
            }
            if (summaryStopMs < previewStopMs) {
                summaryStopMs = previewStopMs;
            }
            summaryTotalAvg = summaryTotalAvg + (previewAvgVal * previewDuration);
            summaryTotalTime = summaryTotalTime + previewDuration;
        }
        if ((this._startTimeMs == 0) || (this._startTimeMs > summaryStartMs)) {
            this._startTimeMs = summaryStartMs;
        }
        if ((this._endTimeMs == 0) || (this._endTimeMs < summaryStopMs)) {
            this._endTimeMs = summaryStopMs;
        }
        if (summaryTotalTime > 0) {
            this._summary = new YMeasure(summaryStartMs / 1000.0, summaryStopMs / 1000.0, summaryMinVal, summaryTotalAvg / summaryTotalTime, summaryMaxVal);
        } else {
            this._summary = new YMeasure(0.0, 0.0, YAPI.INVALID_DOUBLE, YAPI.INVALID_DOUBLE, YAPI.INVALID_DOUBLE);
        }
        return this.get_progress();
    }

    public virtual int processMore(int progress, byte[] data)
    {
        YDataStream stream;
        List<List<double>> dataRows = new List<List<double>>();
        double tim;
        double itv;
        double fitv;
        double end_;
        int nCols;
        int minCol;
        int avgCol;
        int maxCol;
        bool firstMeasure;

        if (progress != this._progress) {
            return this._progress;
        }
        if (this._progress < 0) {
            return this.loadSummary(data);
        }
        stream = this._streams[this._progress];
        stream._parseStream(data);
        dataRows = stream.get_dataRows();
        this._progress = this._progress + 1;
        if (dataRows.Count == 0) {
            return this.get_progress();
        }
        tim = Math.Round(stream.get_realStartTimeUTC() * 1000);
        fitv = Math.Round(stream.get_firstDataSamplesInterval() * 1000);
        itv = Math.Round(stream.get_dataSamplesInterval() * 1000);
        if (fitv == 0) {
            fitv = itv;
        }
        if (tim < itv) {
            tim = itv;
        }
        nCols = dataRows[0].Count;
        minCol = 0;
        if (nCols > 2) {
            avgCol = 1;
        } else {
            avgCol = 0;
        }
        if (nCols > 2) {
            maxCol = 2;
        } else {
            maxCol = 0;
        }

        firstMeasure = true;
        for (int ii = 0; ii < dataRows.Count; ii++) {
            if (firstMeasure) {
                end_ = tim + fitv;
                firstMeasure = false;
            } else {
                end_ = tim + itv;
            }
            if ((end_ > this._startTimeMs) && ((this._endTimeMs == 0) || (tim < this._endTimeMs))) {
                this._measures.Add(new YMeasure(tim / 1000, end_ / 1000, dataRows[ii][minCol], dataRows[ii][avgCol], dataRows[ii][maxCol]));
            }
            tim = end_;
        }
        return this.get_progress();
    }

    public virtual List<YDataStream> get_privateDataStreams()
    {
        return this._streams;
    }

    /**
     * <summary>
     *   Returns the unique hardware identifier of the function who performed the measures,
     *   in the form <c>SERIAL.FUNCTIONID</c>.
     * <para>
     *   The unique hardware identifier is composed of the
     *   device serial number and of the hardware identifier of the function
     *   (for example <c>THRMCPL1-123456.temperature1</c>)
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the function (ex: <c>THRMCPL1-123456.temperature1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YDataSet.HARDWAREID_INVALID</c>.
     * </para>
     */
    public virtual string get_hardwareId()
    {
        YModule mo;
        if (!(this._hardwareId == "")) {
            return this._hardwareId;
        }
        mo = this._parent.get_module();
        this._hardwareId = ""+ mo.get_serialNumber()+"."+this.get_functionId();
        return this._hardwareId;
    }

    /**
     * <summary>
     *   Returns the hardware identifier of the function that performed the measure,
     *   without reference to the module.
     * <para>
     *   For example <c>temperature1</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that identifies the function (ex: <c>temperature1</c>)
     * </returns>
     */
    public virtual string get_functionId()
    {
        return this._functionId;
    }

    /**
     * <summary>
     *   Returns the measuring unit for the measured value.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that represents a physical unit.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YDataSet.UNIT_INVALID</c>.
     * </para>
     */
    public virtual string get_unit()
    {
        return this._unit;
    }

    /**
     * <summary>
     *   Returns the start time of the dataset, relative to the Jan 1, 1970.
     * <para>
     *   When the YDataSet is created, the start time is the value passed
     *   in parameter to the <c>get_dataSet()</c> function. After the
     *   very first call to <c>loadMore()</c>, the start time is updated
     *   to reflect the timestamp of the first measure actually found in the
     *   dataLogger within the specified range.
     * </para>
     * <para>
     *   <b>DEPRECATED</b>: This method has been replaced by <c>get_summary()</c>
     *   which contain more precise informations on the YDataSet.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of seconds
     *   between the Jan 1, 1970 and the beginning of this data
     *   set (i.e. Unix time representation of the absolute time).
     * </returns>
     */
    public virtual long get_startTimeUTC()
    {
        return this.imm_get_startTimeUTC();
    }

    public virtual long imm_get_startTimeUTC()
    {
        return (long) (this._startTimeMs / 1000.0);
    }

    /**
     * <summary>
     *   Returns the end time of the dataset, relative to the Jan 1, 1970.
     * <para>
     *   When the YDataSet is created, the end time is the value passed
     *   in parameter to the <c>get_dataSet()</c> function. After the
     *   very first call to <c>loadMore()</c>, the end time is updated
     *   to reflect the timestamp of the last measure actually found in the
     *   dataLogger within the specified range.
     * </para>
     * <para>
     *   <b>DEPRECATED</b>: This method has been replaced by <c>get_summary()</c>
     *   which contain more precise informations on the YDataSet.
     * </para>
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of seconds
     *   between the Jan 1, 1970 and the end of this data
     *   set (i.e. Unix time representation of the absolute time).
     * </returns>
     */
    public virtual long get_endTimeUTC()
    {
        return this.imm_get_endTimeUTC();
    }

    public virtual long imm_get_endTimeUTC()
    {
        return (long) Math.Round(this._endTimeMs / 1000.0);
    }

    /**
     * <summary>
     *   Returns the progress of the downloads of the measures from the data logger,
     *   on a scale from 0 to 100.
     * <para>
     *   When the object is instantiated by <c>get_dataSet</c>,
     *   the progress is zero. Each time <c>loadMore()</c> is invoked, the progress
     *   is updated, to reach the value 100 only once all measures have been loaded.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer in the range 0 to 100 (percentage of completion).
     * </returns>
     */
    public virtual int get_progress()
    {
        if (this._progress < 0) {
            return 0;
        }
        // index not yet loaded
        if (this._progress >= this._streams.Count) {
            return 100;
        }
        return ((1 + (1 + this._progress) * 98) / ((1 + this._streams.Count)));
    }

    /**
     * <summary>
     *   Loads the the next block of measures from the dataLogger, and updates
     *   the progress indicator.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer in the range 0 to 100 (percentage of completion),
     *   or a negative error code in case of failure.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int loadMore()
    {
        string url;
        YDataStream stream;
        if (this._progress < 0) {
            url = "logger.json?id="+this._functionId;
            if (this._startTimeMs != 0) {
                url = ""+url+"&from="+Convert.ToString(this.imm_get_startTimeUTC());
            }
            if (this._endTimeMs != 0) {
                url = ""+url+"&to="+Convert.ToString(this.imm_get_endTimeUTC()+1);
            }
        } else {
            if (this._progress >= this._streams.Count) {
                return 100;
            } else {
                stream = this._streams[this._progress];
                url = stream._get_url();
            }
        }
        try {
            return this.processMore(this._progress, this._parent._download(url));
        } catch {
            return this.processMore(this._progress, this._parent._download(url));
        }
    }

    /**
     * <summary>
     *   Returns an YMeasure object which summarizes the whole
     *   DataSet.
     * <para>
     *   In includes the following information:
     *   - the start of a time interval
     *   - the end of a time interval
     *   - the minimal value observed during the time interval
     *   - the average value observed during the time interval
     *   - the maximal value observed during the time interval
     * </para>
     * <para>
     *   This summary is available as soon as <c>loadMore()</c> has
     *   been called for the first time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an YMeasure object
     * </returns>
     */
    public virtual YMeasure get_summary()
    {
        return this._summary;
    }

    /**
     * <summary>
     *   Returns a condensed version of the measures that can
     *   retrieved in this YDataSet, as a list of YMeasure
     *   objects.
     * <para>
     *   Each item includes:
     *   - the start of a time interval
     *   - the end of a time interval
     *   - the minimal value observed during the time interval
     *   - the average value observed during the time interval
     *   - the maximal value observed during the time interval
     * </para>
     * <para>
     *   This preview is available as soon as <c>loadMore()</c> has
     *   been called for the first time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a table of records, where each record depicts the
     *   measured values during a time interval
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<YMeasure> get_preview()
    {
        return this._preview;
    }

    /**
     * <summary>
     *   Returns the detailed set of measures for the time interval corresponding
     *   to a given condensed measures previously returned by <c>get_preview()</c>.
     * <para>
     *   The result is provided as a list of YMeasure objects.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="measure">
     *   condensed measure from the list previously returned by
     *   <c>get_preview()</c>.
     * </param>
     * <returns>
     *   a table of records, where each record depicts the
     *   measured values during a time interval
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<YMeasure> get_measuresAt(YMeasure measure)
    {
        double startUtcMs;
        YDataStream stream;
        List<List<double>> dataRows = new List<List<double>>();
        List<YMeasure> measures = new List<YMeasure>();
        double tim;
        double itv;
        double end_;
        int nCols;
        int minCol;
        int avgCol;
        int maxCol;

        startUtcMs = measure.get_startTimeUTC() * 1000;
        stream = null;
        for (int ii = 0; ii < this._streams.Count; ii++) {
            if (Math.Round(this._streams[ii].get_realStartTimeUTC() *1000) == startUtcMs) {
                stream = this._streams[ii];
            }
        }
        if (stream == null) {
            return measures;
        }
        dataRows = stream.get_dataRows();
        if (dataRows.Count == 0) {
            return measures;
        }
        tim = Math.Round(stream.get_realStartTimeUTC() * 1000);
        itv = Math.Round(stream.get_dataSamplesInterval() * 1000);
        if (tim < itv) {
            tim = itv;
        }
        nCols = dataRows[0].Count;
        minCol = 0;
        if (nCols > 2) {
            avgCol = 1;
        } else {
            avgCol = 0;
        }
        if (nCols > 2) {
            maxCol = 2;
        } else {
            maxCol = 0;
        }

        for (int ii = 0; ii < dataRows.Count; ii++) {
            end_ = tim + itv;
            if ((end_ > this._startTimeMs) && ((this._endTimeMs == 0) || (tim < this._endTimeMs))) {
                measures.Add(new YMeasure(tim / 1000.0, end_ / 1000.0, dataRows[ii][minCol], dataRows[ii][avgCol], dataRows[ii][maxCol]));
            }
            tim = end_;
        }
        return measures;
    }

    /**
     * <summary>
     *   Returns all measured values currently available for this DataSet,
     *   as a list of YMeasure objects.
     * <para>
     *   Each item includes:
     *   - the start of the measure time interval
     *   - the end of the measure time interval
     *   - the minimal value observed during the time interval
     *   - the average value observed during the time interval
     *   - the maximal value observed during the time interval
     * </para>
     * <para>
     *   Before calling this method, you should call <c>loadMore()</c>
     *   to load data from the device. You may have to call loadMore()
     *   several time until all rows are loaded, but you can start
     *   looking at available data rows before the load is complete.
     * </para>
     * <para>
     *   The oldest measures are always loaded first, and the most
     *   recent measures will be loaded last. As a result, timestamps
     *   are normally sorted in ascending order within the measure table,
     *   unless there was an unexpected adjustment of the datalogger UTC
     *   clock.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a table of records, where each record depicts the
     *   measured value for a given time interval
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<YMeasure> get_measures()
    {
        return this._measures;
    }

    //--- (end of generated code: YDataSet implementation)
}


//--- (generated code: YConsolidatedDataSet class start)
/**
 * <summary>
 *   YConsolidatedDataSet objects make it possible to retrieve a set of
 *   recorded measures from multiple sensors, for a specified time interval.
 * <para>
 *   They can be used to load data points progressively, and to receive
 *   data records by timestamp, one by one..
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YConsolidatedDataSet
{
//--- (end of generated code: YConsolidatedDataSet class start)
    //--- (generated code: YConsolidatedDataSet definitions)

    protected double _start = 0;
    protected double _end = 0;
    protected int _nsensors = 0;
    protected List<YSensor> _sensors = new List<YSensor>();
    protected List<YDataSet> _datasets = new List<YDataSet>();
    protected List<int> _progresss = new List<int>();
    protected List<int> _nextidx = new List<int>();
    protected List<double> _nexttim = new List<double>();
    //--- (end of generated code: YConsolidatedDataSet definitions)

    // YConsolidatedDataSet constructor
    public YConsolidatedDataSet(double startTime, double endTime, List<YSensor> sensorList)
    {
        //--- (generated code: YConsolidatedDataSet attributes initialization)
        //--- (end of generated code: YConsolidatedDataSet attributes initialization)
        this.imm_init(startTime, endTime, sensorList);
    }

    //--- (generated code: YConsolidatedDataSet implementation)


    public virtual int imm_init(double startt, double endt, List<YSensor> sensorList)
    {
        this._start = startt;
        this._end = endt;
        this._sensors = sensorList;
        this._nsensors = -1;
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Extracts the next data record from the dataLogger of all sensors linked to this
     *   object.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="datarec">
     *   array of floating point numbers, that will be filled by the
     *   function with the timestamp of the measure in first position,
     *   followed by the measured value in next positions.
     * </param>
     * <returns>
     *   an integer in the range 0 to 100 (percentage of completion),
     *   or a negative error code in case of failure.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int nextRecord(List<double> datarec)
    {
        int s;
        int idx;
        YSensor sensor;
        YDataSet newdataset;
        int globprogress;
        int currprogress;
        double currnexttim;
        double newvalue;
        List<YMeasure> measures = new List<YMeasure>();
        double nexttime;
        //
        // Ensure the dataset have been retrieved
        //
        if (this._nsensors == -1) {
            this._nsensors = this._sensors.Count;
            this._datasets.Clear();
            this._progresss.Clear();
            this._nextidx.Clear();
            this._nexttim.Clear();
            s = 0;
            while (s < this._nsensors) {
                sensor = this._sensors[s];
                newdataset = sensor.get_recordedData(this._start, this._end);
                this._datasets.Add(newdataset);
                this._progresss.Add(0);
                this._nextidx.Add(0);
                this._nexttim.Add(0.0);
                s = s + 1;
            }
        }
        datarec.Clear();
        //
        // Find next timestamp to process
        //
        nexttime = 0;
        s = 0;
        while (s < this._nsensors) {
            currnexttim = this._nexttim[s];
            if (currnexttim == 0) {
                idx = this._nextidx[s];
                measures = this._datasets[s].get_measures();
                currprogress = this._progresss[s];
                while ((idx >= measures.Count) && (currprogress < 100)) {
                    currprogress = this._datasets[s].loadMore();
                    if (currprogress < 0) {
                        currprogress = 100;
                    }
                    this._progresss[s] = currprogress;
                    measures = this._datasets[s].get_measures();
                }
                if (idx < measures.Count) {
                    currnexttim = measures[idx].get_endTimeUTC();
                    this._nexttim[s] = currnexttim;
                }
            }
            if (currnexttim > 0) {
                if ((nexttime == 0) || (nexttime > currnexttim)) {
                    nexttime = currnexttim;
                }
            }
            s = s + 1;
        }
        if (nexttime == 0) {
            return 100;
        }
        //
        // Extract data for this timestamp
        //
        datarec.Clear();
        datarec.Add(nexttime);
        globprogress = 0;
        s = 0;
        while (s < this._nsensors) {
            if (this._nexttim[s] == nexttime) {
                idx = this._nextidx[s];
                measures = this._datasets[s].get_measures();
                newvalue = measures[idx].get_averageValue();
                datarec.Add(newvalue);
                this._nexttim[s] = 0.0;
                this._nextidx[s] = idx+1;
            } else {
                datarec.Add(Double.NaN);
            }
            currprogress = this._progresss[s];
            globprogress = globprogress + currprogress;
            s = s + 1;
        }
        if (globprogress > 0) {
            globprogress = ((globprogress) / (this._nsensors));
            if (globprogress > 99) {
                globprogress = 99;
            }
        }
        return globprogress;
    }

    //--- (end of generated code: YConsolidatedDataSet implementation)
}


//
// TYFunction Class (virtual class, used internally)
//
// This is the parent class for all public objects representing device functions documented in
// the high-level programming API. This abstract class does all the real job, but without
// knowledge of the specific function attributes.
//
// Instantiating a child class of YFunction does not cause any communication.
// The instance simply keeps track of its function identifier, and will dynamically bind
// to a matching device at the time it is really beeing used to read or set an attribute.
// In order to allow true hot-plug replacement of one device by another, the binding stay
// dynamic through the life of the object.
//
// The YFunction class implements a generic high-level cache for the attribute values of
// the specified function, pre-parsed from the REST API string.
//


//--- (generated code: YFunction class start)
/**
 * <summary>
 *   This is the parent class for all public objects representing device functions documented in
 *   the high-level programming API.
 * <para>
 *   This abstract class does all the real job, but without
 *   knowledge of the specific function attributes.
 * </para>
 * <para>
 *   Instantiating a child class of YFunction does not cause any communication.
 *   The instance simply keeps track of its function identifier, and will dynamically bind
 *   to a matching device at the time it is really being used to read or set an attribute.
 *   In order to allow true hot-plug replacement of one device by another, the binding stay
 *   dynamic through the life of the object.
 * </para>
 * <para>
 *   The YFunction class implements a generic high-level cache for the attribute values of
 *   the specified function, pre-parsed from the REST API string.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YFunction
{
//--- (end of generated code: YFunction class start)
    //--- (generated code: YFunction definitions)
    public delegate void ValueCallback(YFunction func, string value);
    public delegate void TimedReportCallback(YFunction func, YMeasure measure);

    public const string LOGICALNAME_INVALID = YAPI.INVALID_STRING;
    public const string ADVERTISEDVALUE_INVALID = YAPI.INVALID_STRING;
    protected string _logicalName = LOGICALNAME_INVALID;
    protected string _advertisedValue = ADVERTISEDVALUE_INVALID;
    protected ValueCallback _valueCallbackFunction = null;
    protected ulong _cacheExpiration = 0;
    protected string _serial;
    protected string _funId;
    protected string _hwId;
    //--- (end of generated code: YFunction definitions)
    public static Dictionary<string, YFunction> _cache = new Dictionary<string, YFunction>();
    public static List<YFunction> _ValueCallbackList = new List<YFunction>();
    public static List<YSensor> _TimedReportCallbackList = new List<YSensor>();
    public static Dictionary<string, YAPI.yCalibrationHandler> _CalibHandlers = new Dictionary<string, YAPI.yCalibrationHandler>();

    public delegate void GenericUpdateCallback(YFunction func, string value);

    public const YFUN_DESCR FUNCTIONDESCRIPTOR_INVALID = -1;
    protected Object _thisLock = new Object();
    protected string _className;
    protected string _func;
    protected YRETCODE _lastErrorType;
    protected string _lastErrorMsg;
    protected YFUN_DESCR _fundescr;
    protected object _userData;
    private static Hashtable _dataStreams = new Hashtable();

    public YFunction(string func)
    {
        _className = "Function";
        //--- (generated code: YFunction attributes initialization)
        //--- (end of generated code: YFunction attributes initialization)
        _func = func;
        _lastErrorType = YAPI.SUCCESS;
        _lastErrorMsg = "";
        _fundescr = FUNCTIONDESCRIPTOR_INVALID;
        _userData = null;
    }

    public void _throw(YRETCODE errType, string errMsg)
    {
        _lastErrorType = errType;
        _lastErrorMsg = errMsg;
        if (!(YAPI.ExceptionsDisabled)) {
            throw new YAPI_Exception(errType, "YoctoApi error : " + errMsg);
        }
    }


    // function cache methods
    static protected YFunction _FindFromCache(string classname, string func)
    {
        if (_cache.ContainsKey(classname + "_" + func))
            return _cache[classname + "_" + func];
        return null;
    }

    static protected void _AddToCache(string classname, string func, YFunction obj)
    {
        _cache[classname + "_" + func] = obj;
    }

    static internal void _ClearCache()
    {
        _cache.Clear();
    }


    static protected void _UpdateValueCallbackList(YFunction func, Boolean add)
    {
        if (add) {
            func.isOnline();
            if (!_ValueCallbackList.Contains(func)) {
                _ValueCallbackList.Add(func);
            }
        } else {
            _ValueCallbackList.Remove(func);
        }
    }

    static protected void _UpdateTimedReportCallbackList(YSensor func, Boolean add)
    {
        if (add) {
            func.isOnline();
            if (!_TimedReportCallbackList.Contains(func)) {
                _TimedReportCallbackList.Add(func);
            }
        } else {
            _TimedReportCallbackList.Remove(func);
        }
    }


    //  Method used to resolve our name to our unique function descriptor (may trigger a hub scan)
    protected YRETCODE _getDescriptor(ref YFUN_DESCR fundescr, ref string errMsg)
    {
        int res = 0;
        YFUN_DESCR tmp_fundescr;

        tmp_fundescr = YAPI.yapiGetFunction(_className, _func, ref errMsg);
        if (YAPI.YISERR(tmp_fundescr)) {
            res = YAPI.yapiUpdateDeviceList(1, ref errMsg);
            if (YAPI.YISERR(res)) {
                return res;
            }

            tmp_fundescr = YAPI.yapiGetFunction(_className, _func, ref errMsg);
            if (YAPI.YISERR(tmp_fundescr)) {
                return tmp_fundescr;
            }
        }

        _fundescr = fundescr = tmp_fundescr;
        return YAPI.SUCCESS;
    }


    // Return a pointer to our device caching object (may trigger a hub scan)
    protected YRETCODE _getDevice(ref YAPI.YDevice dev, ref string errMsg)
    {
        YRETCODE functionReturnValue = default(YRETCODE);
        YFUN_DESCR fundescr = default(YFUN_DESCR);
        YDEV_DESCR devdescr = default(YDEV_DESCR);
        YRETCODE res = default(YRETCODE);

        // Resolve function name
        res = _getDescriptor(ref fundescr, ref errMsg);
        if ((YAPI.YISERR(res))) {
            functionReturnValue = res;
            return functionReturnValue;
        }

        // Get device descriptor
        devdescr = YAPI.yapiGetDeviceByFunction(fundescr, ref errMsg);
        if ((YAPI.YISERR(devdescr))) {
            return devdescr;
        }

        // Get device object
        dev = YAPI.YDevice.getDevice(devdescr);

        functionReturnValue = YAPI.SUCCESS;
        return functionReturnValue;
    }

    // Return the next known function of current class listed in the yellow pages
    protected YRETCODE _nextFunction(ref string hwid)
    {
        YRETCODE functionReturnValue = default(YRETCODE);

        YFUN_DESCR fundescr = default(YFUN_DESCR);
        YDEV_DESCR devdescr = default(YDEV_DESCR);
        string serial = "";
        string funcId = "";
        string funcName = "";
        string funcVal = "";
        string errmsg = "";
        int res = 0;
        int count = 0;
        int neededsize = 0;
        int maxsize = 0;
        IntPtr p = default(IntPtr);

        const int n_element = 1;
        int[] pdata = new int[1];

        res = _getDescriptor(ref fundescr, ref errmsg);
        if ((YAPI.YISERR(res))) {
            _throw(res, errmsg);
            functionReturnValue = res;
            return functionReturnValue;
        }

        maxsize = n_element * Marshal.SizeOf(pdata[0]);
        p = Marshal.AllocHGlobal(maxsize);
        res = YAPI.apiGetFunctionsByClass(_className, fundescr, p, maxsize, ref neededsize, ref errmsg);
        Marshal.Copy(p, pdata, 0, n_element);
        Marshal.FreeHGlobal(p);
        if ((YAPI.YISERR(res))) {
            _throw(res, errmsg);
            functionReturnValue = res;
            return functionReturnValue;
        }

        count = Convert.ToInt32(neededsize / Marshal.SizeOf(pdata[0]));
        if (count == 0) {
            hwid = "";
            functionReturnValue = YAPI.SUCCESS;
            return functionReturnValue;
        }

        res = YAPI.yapiGetFunctionInfo(pdata[0], ref devdescr, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg);

        if ((YAPI.YISERR(res))) {
            _throw(res, errmsg);
            functionReturnValue = YAPI.SUCCESS;
            return functionReturnValue;
        }

        hwid = serial + "." + funcId;
        functionReturnValue = YAPI.SUCCESS;
        return functionReturnValue;
    }

    protected string _escapeAttr(string changeval)
    {
        return YAPI.__escapeAttr(changeval);
    }


    private YRETCODE _buildSetRequest(string changeattr, string changeval, ref string request, ref string errmsg)
    {
        YRETCODE functionReturnValue = default(YRETCODE);

        int res = 0;
        YFUN_DESCR fundesc = default(YFUN_DESCR);
        StringBuilder funcid = new StringBuilder(YAPI.YOCTO_FUNCTION_LEN);
        StringBuilder errbuff = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        string uchangeval = null;
        YDEV_DESCR devdesc = default(YDEV_DESCR);
        funcid.Length = 0;
        errbuff.Length = 0;
        // Resolve the function name
        res = _getDescriptor(ref fundesc, ref errmsg);

        if ((YAPI.YISERR(res))) {
            functionReturnValue = res;
            return functionReturnValue;
        }

        res = SafeNativeMethods._yapiGetFunctionInfoEx(fundesc, ref devdesc, null, funcid, null, null, null, errbuff);
        if (YAPI.YISERR(res)) {
            errmsg = errbuff.ToString();
            _throw(res, errmsg);
            functionReturnValue = res;
            return functionReturnValue;
        }

        request = "GET /api/" + funcid.ToString() + "/";
        uchangeval = "";
        if (changeattr != "") {
            request = request + changeattr + "?" + changeattr + "=" + _escapeAttr(changeval);
        }

        request = request + uchangeval + "&. \r\n\r\n";
        functionReturnValue = YAPI.SUCCESS;
        return functionReturnValue;
    }


    // Set an attribute in the function, and parse the resulting new function state
    protected YRETCODE _setAttr(string attrname, string newvalue)
    {
        string errmsg = "";
        string request = "";
        int res = 0;
        YAPI.YDevice dev = null;

        //  Execute http request
        res = _buildSetRequest(attrname, newvalue, ref request, ref errmsg);
        if (YAPI.YISERR(res)) {
            _throw(res, errmsg);
            return res;
        }

        // Get device Object
        res = _getDevice(ref dev, ref errmsg);
        if (YAPI.YISERR(res)) {
            _throw(res, errmsg);
            return res;
        }

        res = dev.HTTPRequestAsync(request, ref errmsg);
        if (YAPI.YISERR(res)) {
            // make sure a device scan does not solve the issue
            res = YAPI.yapiUpdateDeviceList(1, ref errmsg);
            if (YAPI.YISERR(res)) {
                _throw(res, errmsg);
                return res;
            }

            res = dev.HTTPRequestAsync(request, ref errmsg);
            if (YAPI.YISERR(res)) {
                _throw(res, errmsg);
                return res;
            }
        }

        dev.clearCache(false);
        if (_cacheExpiration != 0) {
            _cacheExpiration = YAPI.GetTickCount();
        }

        return YAPI.SUCCESS;
    }

    // Method used to send http request to the device (not the function)
    protected byte[] _request(string request)
    {
        return this._request(YAPI.DefaultEncoding.GetBytes(request));
    }

    // Method used to send http request to the device (not the function)
    protected byte[] _request(byte[] request)
    {
        YAPI.YDevice dev = null;
        string errmsg = "";
        byte[] buffer = null;
        byte[] check;
        int res;

        // Resolve our reference to our device, load REST API
        res = _getDevice(ref dev, ref errmsg);
        if (YAPI.YISERR(res)) {
            _throw(res, errmsg);
            return new byte[0];
        }

        res = dev.HTTPRequest(request, ref buffer, ref errmsg);
        if (YAPI.YISERR(res)) {
            // Check if an update of the device list does notb solve the issue
            res = YAPI.yapiUpdateDeviceList(1, ref errmsg);
            if (YAPI.YISERR(res)) {
                _throw(res, errmsg);
                return new byte[0];
            }

            res = dev.HTTPRequest(request, ref buffer, ref errmsg);
            if (YAPI.YISERR(res)) {
                _throw(res, errmsg);
                return new byte[0];
            }
        }

        if (buffer.Length >= 4) {
            check = new byte[4];
            Buffer.BlockCopy(buffer, 0, check, 0, 4);
            if (YAPI.DefaultEncoding.GetString(check) == "OK\r\n") {
                return buffer;
            }

            if (buffer.Length >= 17) {
                check = new byte[17];
                Buffer.BlockCopy(buffer, 0, check, 0, 17);
                if (YAPI.DefaultEncoding.GetString(check) == "HTTP/1.1 200 OK\r\n") {
                    return buffer;
                }
            }
        }

        _throw(YAPI.IO_ERROR, "http request failed");
        return new byte[0];
    }

    private byte[] _strip_http_header(byte[] buffer)
    {
        int found, body;
        for (found = 0; found < buffer.Length - 4; found++) {
            if (buffer[found] == 13 && buffer[found + 1] == 10 && buffer[found + 2] == 13 && buffer[found + 3] == 10)
                break;
        }

        if (found > buffer.Length - 4) {
            _throw(YAPI.IO_ERROR, "http request failed");
            return null;
        } else if (found == buffer.Length - 4) {
            return new byte[0];
        }

        body = found + 4;
        byte[] res = new byte[buffer.Length - body];
        Buffer.BlockCopy(buffer, body, res, 0, buffer.Length - body);
        return res;
    }

    // Method used to send http request to the device (not the function)
    public byte[] _download(string path)
    {
        string request;
        byte[] buffer, res;
        request = "GET /" + path + " HTTP/1.1\r\n\r\n";
        buffer = _request(request);
        res = _strip_http_header(buffer);
        return res;
    }

    // Method used to upload a file to the device
    public int _upload(string path, string content)
    {
        return this._upload(path, YAPI.DefaultEncoding.GetBytes(content));
    }

    // Method used to upload a file to the device
    public int _upload(string path, List<byte> content)
    {
        return this._upload(path, content.ToArray());
    }

    // Method used to upload a file to the device
    public byte[] _uploadEx(string path, byte[] content)
    {
        string bodystr, boundary;
        byte[] body, bb, header, footer, fullrequest, buffer;

        bodystr = "Content-Disposition: form-data; name=\"" + path + "\"; filename=\"api\"\r\n" + "Content-Type: application/octet-stream\r\n" + "Content-Transfer-Encoding: binary\r\n\r\n";
        body = new byte[bodystr.Length + content.Length];
        Buffer.BlockCopy(YAPI.DefaultEncoding.GetBytes(bodystr), 0, body, 0, bodystr.Length);
        Buffer.BlockCopy(content, 0, body, bodystr.Length, content.Length);

        Random random = new Random();
        int pos, i;
        do {
            boundary = "Zz" + ((int) random.Next(100000, 999999)).ToString() + "zZ";
            bb = YAPI.DefaultEncoding.GetBytes(boundary);
            pos = 0;
            while (pos <= body.Length - bb.Length) {
                if (body[pos] == 90) // 'Z'
                {
                    i = 1;
                    while (i < bb.Length && body[pos + i] == bb[i]) i++;
                    if (i >= bb.Length) break;
                    pos += i;
                } else pos++;
            }
        } while (pos <= body.Length - bb.Length);

        header = YAPI.DefaultEncoding.GetBytes("POST /upload.html HTTP/1.1\r\n" + "Content-Type: multipart/form-data, boundary=" + boundary + "\r\n" + "\r\n--" + boundary + "\r\n");
        footer = YAPI.DefaultEncoding.GetBytes("\r\n--" + boundary + "--\r\n");
        fullrequest = new byte[header.Length + body.Length + footer.Length];
        Buffer.BlockCopy(header, 0, fullrequest, 0, header.Length);
        Buffer.BlockCopy(body, 0, fullrequest, header.Length, body.Length);
        Buffer.BlockCopy(footer, 0, fullrequest, header.Length + body.Length, footer.Length);
        buffer = _request(fullrequest);
        return _strip_http_header(buffer);
    }

    public int _upload(string path, byte[] content)
    {
        byte[] buffer;
        buffer = _uploadEx(path, content);
        if (buffer == null) {
            return YAPI.IO_ERROR;
        }

        return YAPI.SUCCESS;
    }

    // Method used to cache DataStream objects (new DataLogger)
    public YDataStream _findDataStream(YDataSet dataset, string def)
    {
        string key = dataset.get_functionId() + ":" + def;
        if (_dataStreams.ContainsKey(key))
            return (YDataStream) _dataStreams[key];
        List<int> words = YAPI._decodeWords(def);
        if (words.Count < 14) {
            _throw(YAPI.VERSION_MISMATCH, "device firmware too old");
            return null;
        }

        YDataStream newDataStream = new YDataStream(this, dataset, words);
        _dataStreams.Add(key, newDataStream);
        return newDataStream;
    }

    // Method used to clear cache of DataStream object (undocumented)
    public void _clearDataStreamCache()
    {
        _dataStreams.Clear();
    }

    protected int _parse(YAPI.YJSONObject j)
    {
        _parseAttr(j);
        _parserHelper();
        return 0;
    }


    //--- (generated code: YFunction implementation)

    protected virtual void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("logicalName"))
        {
            _logicalName = json_val.getString("logicalName");
        }
        if (json_val.has("advertisedValue"))
        {
            _advertisedValue = json_val.getString("advertisedValue");
        }
    }

    /**
     * <summary>
     *   Returns the logical name of the function.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the logical name of the function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YFunction.LOGICALNAME_INVALID</c>.
     * </para>
     */
    public string get_logicalName()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LOGICALNAME_INVALID;
                }
            }
            res = this._logicalName;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the logical name of the function.
     * <para>
     *   You can use <c>yCheckLogicalName()</c>
     *   prior to this call to make sure that your parameter is valid.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the logical name of the function
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
    public int set_logicalName(string newval)
    {
        string rest_val;
        if (!YAPI.CheckLogicalName(newval))
        {
            _throw(YAPI.INVALID_ARGUMENT, "Invalid name :" + newval);
            return YAPI.INVALID_ARGUMENT;
        }
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("logicalName", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns a short string representing the current state of the function.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to a short string representing the current state of the function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YFunction.ADVERTISEDVALUE_INVALID</c>.
     * </para>
     */
    public string get_advertisedValue()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ADVERTISEDVALUE_INVALID;
                }
            }
            res = this._advertisedValue;
        }
        return res;
    }

    public int set_advertisedValue(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("advertisedValue", rest_val);
        }
    }

    /**
     * <summary>
     *   Retrieves a function for a given identifier.
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
     *   This function does not require that the function is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YFunction.isOnline()</c> to test if the function is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a function by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the function, for instance
     *   <c>MyDevice.</c>.
     * </param>
     * <returns>
     *   a <c>YFunction</c> object allowing you to drive the function.
     * </returns>
     */
    public static YFunction FindFunction(string func)
    {
        YFunction obj;
        lock (YAPI.globalLock) {
            obj = (YFunction) YFunction._FindFromCache("Function", func);
            if (obj == null) {
                obj = new YFunction(func);
                YFunction._AddToCache("Function", func, obj);
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
    public virtual int registerValueCallback(ValueCallback callback)
    {
        string val;
        if (callback != null) {
            YFunction._UpdateValueCallbackList(this, true);
        } else {
            YFunction._UpdateValueCallbackList(this, false);
        }
        this._valueCallbackFunction = callback;
        // Immediately invoke value callback with current value
        if (callback != null && this.isOnline()) {
            val = this._advertisedValue;
            if (!(val == "")) {
                this._invokeValueCallback(val);
            }
        }
        return 0;
    }

    public virtual int _invokeValueCallback(string value)
    {
        if (this._valueCallbackFunction != null) {
            this._valueCallbackFunction(this, value);
        } else {
        }
        return 0;
    }

    /**
     * <summary>
     *   Disables the propagation of every new advertised value to the parent hub.
     * <para>
     *   You can use this function to save bandwidth and CPU on computers with limited
     *   resources, or to prevent unwanted invocations of the HTTP callback.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int muteValueCallbacks()
    {
        return this.set_advertisedValue("SILENT");
    }

    /**
     * <summary>
     *   Re-enables the propagation of every new advertised value to the parent hub.
     * <para>
     *   This function reverts the effect of a previous call to <c>muteValueCallbacks()</c>.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int unmuteValueCallbacks()
    {
        return this.set_advertisedValue("");
    }

    /**
     * <summary>
     *   Returns the current value of a single function attribute, as a text string, as quickly as
     *   possible but without using the cached value.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="attrName">
     *   the name of the requested attribute
     * </param>
     * <returns>
     *   a string with the value of the the attribute
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public virtual string loadAttribute(string attrName)
    {
        string url;
        byte[] attrVal;
        url = "api/"+ this.get_functionId()+"/"+attrName;
        attrVal = this._download(url);
        return YAPI.DefaultEncoding.GetString(attrVal);
    }

    /**
     * <summary>
     *   Test if the function is readOnly.
     * <para>
     *   Return <c>true</c> if the function is write protected
     *   or that the function is not available.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>true</c> if the function is readOnly or not online.
     * </returns>
     */
    public virtual bool isReadOnly()
    {
        string serial;
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        int res;
        try {
            serial = this.get_serialNumber();
        } catch {
            return true;
        }
        res = SafeNativeMethods._yapiIsModuleWritable(new StringBuilder(serial), errmsg);
        if (res > 0) {
            return false;
        }
        return true;
    }

    /**
     * <summary>
     *   Returns the serial number of the module, as set by the factory.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the serial number of the module, as set by the factory.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YModule.SERIALNUMBER_INVALID.
     * </para>
     */
    public virtual string get_serialNumber()
    {
        YModule m;
        m = this.get_module();
        return m.get_serialNumber();
    }

    public virtual int _parserHelper()
    {
        return 0;
    }

    /**
     * <summary>
     *   c
     * <para>
     *   omment from .yc definition
     * </para>
     * </summary>
     */
    public YFunction nextFunction()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindFunction(hwid);
    }

    //--- (end of generated code: YFunction implementation)

    //--- (generated code: YFunction functions)

    /**
     * <summary>
     *   c
     * <para>
     *   omment from .yc definition
     * </para>
     * </summary>
     */
    public static YFunction FirstFunction()
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
        err = YAPI.apiGetFunctionsByClass("Function", 0, p, size, ref neededsize, ref errmsg);
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
        return FindFunction(serial + "." + funcId);
    }



    //--- (end of generated code: YFunction functions)


    /**
     * <summary>
     *   Returns a short text that describes unambiguously the instance of the function in the form <c>TYPE(NAME)=SERIAL&#46;FUNCTIONID</c>.
     * <para>
     *   More precisely,
     *   <c>TYPE</c>       is the type of the function,
     *   <c>NAME</c>       it the name used for the first access to the function,
     *   <c>SERIAL</c>     is the serial number of the module if the module is connected or <c>"unresolved"</c>, and
     *   <c>FUNCTIONID</c> is  the hardware identifier of the function if the module is connected.
     *   For example, this method returns <c>Relay(MyCustomName.relay1)=RELAYLO1-123456.relay1</c> if the
     *   module is already connected or <c>Relay(BadCustomeName.relay1)=unresolved</c> if the module has
     *   not yet been connected. This method does not trigger any USB or TCP transaction and can therefore be used in
     *   a debugger.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that describes the function
     *   (ex: <c>Relay(MyCustomName.relay1)=RELAYLO1-123456.relay1</c>)
     * </returns>
     */
    public string describe()
    {
        YFUN_DESCR fundescr = default(YFUN_DESCR);
        YDEV_DESCR devdescr = default(YDEV_DESCR);
        string errmsg = "";
        string serial = "";
        string funcId = "";
        string funcName = "";
        string funcValue = "";
        lock (_thisLock) {
            fundescr = YAPI.yapiGetFunction(_className, _func, ref errmsg);
            if ((!(YAPI.YISERR(fundescr)))) {
                if ((!(YAPI.YISERR(YAPI.yapiGetFunctionInfo(fundescr, ref devdescr, ref serial, ref funcId, ref funcName, ref funcValue, ref errmsg))))) {
                    return _className + "(" + _func + ")=" + serial + "." + funcId;
                }
            }

            return _className + "(" + _func + ")=unresolved";
        }
    }


    /**
     * <summary>
     *   Returns the unique hardware identifier of the function in the form <c>SERIAL.FUNCTIONID</c>.
     * <para>
     *   The unique hardware identifier is composed of the device serial
     *   number and of the hardware identifier of the function (for example <c>RELAYLO1-123456.relay1</c>).
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the function (ex: <c>RELAYLO1-123456.relay1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YFunction.HARDWAREID_INVALID</c>.
     * </para>
     */

    public virtual string get_hardwareId()
    {
        YRETCODE retcode;
        YFUN_DESCR fundesc = 0;
        YDEV_DESCR devdesc = 0;
        string funcName = "";
        string funcVal = "";
        string errmsg = "";
        string snum = "";
        string funcid = "";

        lock (_thisLock) {
            // Resolve the function name
            retcode = _getDescriptor(ref fundesc, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.HARDWAREID_INVALID;
            }

            retcode = YAPI.yapiGetFunctionInfo(fundesc, ref devdesc, ref snum, ref funcid, ref funcName, ref funcVal, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.HARDWAREID_INVALID;
            }

            return snum + '.' + funcid;
        }
    }


    /**
     * <summary>
     *   Returns the hardware identifier of the function, without reference to the module.
     * <para>
     *   For example
     *   <c>relay1</c>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that identifies the function (ex: <c>relay1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YFunction.FUNCTIONID_INVALID</c>.
     * </para>
     */

    public string get_functionId()
    {
        YRETCODE retcode;
        YFUN_DESCR fundesc = 0;
        YDEV_DESCR devdesc = 0;
        string funcName = "";
        string funcVal = "";
        string errmsg = "";
        string snum = "";
        string funcid = "";

        lock (_thisLock) {
            // Resolve the function name
            retcode = _getDescriptor(ref fundesc, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.FUNCTIONID_INVALID;
            }

            retcode = YAPI.yapiGetFunctionInfo(fundesc, ref devdesc, ref snum, ref funcid, ref funcName, ref funcVal, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.FUNCTIONID_INVALID;
            }

            return funcid;
        }
    }


    public string FriendlyName {
        get { return this.get_friendlyName(); }
    }

    /**
     * <summary>
     *   Returns a global identifier of the function in the format <c>MODULE_NAME&#46;FUNCTION_NAME</c>.
     * <para>
     *   The returned string uses the logical names of the module and of the function if they are defined,
     *   otherwise the serial number of the module and the hardware identifier of the function
     *   (for example: <c>MyCustomName.relay1</c>)
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the function using logical names
     *   (ex: <c>MyCustomName.relay1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YFunction.FRIENDLYNAME_INVALID</c>.
     * </para>
     */

    public virtual string get_friendlyName()
    {
        YRETCODE retcode;
        YFUN_DESCR fundesc = 0;
        YDEV_DESCR devdesc = 0;
        YDEV_DESCR moddescr = 0;
        string funcName = "";
        string dummy = "";
        string errmsg = "";
        string snum = "";
        string funcid = "";
        string friendly = "";
        string mod_name = "";

        lock (_thisLock) {
            // Resolve the function name
            retcode = _getDescriptor(ref fundesc, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.FRIENDLYNAME_INVALID;
            }

            retcode = YAPI.yapiGetFunctionInfo(fundesc, ref devdesc, ref snum, ref funcid, ref funcName, ref dummy, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.FRIENDLYNAME_INVALID;
            }

            moddescr = YAPI.yapiGetFunction("Module", snum, ref errmsg);
            if (YAPI.YISERR(moddescr)) {
                _throw(retcode, errmsg);
                return YAPI.FRIENDLYNAME_INVALID;
            }

            retcode = YAPI.yapiGetFunctionInfo(moddescr, ref devdesc, ref snum, ref dummy, ref mod_name, ref dummy, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.FRIENDLYNAME_INVALID;
            }

            if (mod_name != "") {
                friendly = mod_name + '.';
            } else {
                friendly = snum + '.';
            }

            if (funcName != "") {
                friendly += funcName;
            } else {
                friendly += funcid;
            }

            return friendly;
        }
    }


    public override string ToString()
    {
        return describe();
    }

    /**
     * <summary>
     *   Returns the numerical error code of the latest error with the function.
     * <para>
     *   This method is mostly useful when using the Yoctopuce library with
     *   exceptions disabled.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a number corresponding to the code of the latest error that occurred while
     *   using the function object
     * </returns>
     */
    public YRETCODE get_errorType()
    {
        return _lastErrorType;
    }

    public YRETCODE errorType()
    {
        return _lastErrorType;
    }

    public YRETCODE errType()
    {
        return _lastErrorType;
    }

    /**
     * <summary>
     *   Returns the error message of the latest error with the function.
     * <para>
     *   This method is mostly useful when using the Yoctopuce library with
     *   exceptions disabled.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the latest error message that occured while
     *   using the function object
     * </returns>
     */
    public string get_errorMessage()
    {
        return _lastErrorMsg;
    }

    public string errorMessage()
    {
        return _lastErrorMsg;
    }

    public string errMessage()
    {
        return _lastErrorMsg;
    }

    /**
     * <summary>
     *   Checks if the function is currently reachable, without raising any error.
     * <para>
     *   If there is a cached value for the function in cache, that has not yet
     *   expired, the device is considered reachable.
     *   No exception is raised if there is an error while trying to contact the
     *   device hosting the function.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>true</c> if the function can be reached, and <c>false</c> otherwise
     * </returns>
     */
    public bool isOnline()
    {
        YAPI.YDevice dev = null;
        string errmsg = "";
        YAPI.YJSONObject apires;
        lock (_thisLock) {
            //  A valid value in cache means that the device is online
            if (_cacheExpiration > YAPI.GetTickCount()) {
                return true;
            }

            // Check that the function is available, without throwing exceptions
            if (YAPI.YISERR(_getDevice(ref dev, ref errmsg))) {
                return false;
            }

            // Try to execute a function request to be positively sure that the device is ready
            if (YAPI.YISERR(dev.requestAPI(out apires, ref errmsg))) {
                return false;
            }

            // Preload the function data, since we have it in device cache
            load(YAPI._yapiContext.GetCacheValidity());
            return true;
        }
    }


    protected string _json_get_key(byte[] data, string key)
    {
        YAPI.YJSONObject obj = new YAPI.YJSONObject(YAPI.DefaultEncoding.GetString(data));
        obj.parse();
        if (obj.has(key)) {
            string val = obj.getString(key);
            if (val == null) {
                val = obj.ToString();
            }

            return val;
        }

        throw new YAPI_Exception(YAPI.INVALID_ARGUMENT, "No key " + key + "in JSON struct");
    }

    protected List<string> _json_get_array(byte[] data)
    {
        string debug = YAPI.DefaultEncoding.GetString(data);
        YAPI.YJSONArray array = new YAPI.YJSONArray(debug);
        array.parse();
        List<string> list = new List<string>();
        int len = array.Length;
        for (int i = 0; i < len; i++) {
            YAPI.YJSONContent o = array.get(i);
            list.Add(o.toJSON());
        }

        return list;
    }

    public string _json_get_string(byte[] data)
    {
        string s = YAPI.DefaultEncoding.GetString(data);
        YAPI.YJSONString jstring = new YAPI.YJSONString(s, 0, s.Length);
        jstring.parse();
        return jstring.getString();
    }


    private string get_json_path_struct(YAPI.YJSONObject jsonObject, string[] paths, int ofs)
    {
        string key = paths[ofs];
        if (!jsonObject.has(key)) {
            return "";
        }

        YAPI.YJSONContent obj = jsonObject.get(key);
        if (obj != null) {
            if (paths.Length == ofs + 1) {
                return obj.toJSON();
            }

            if (obj is YAPI.YJSONArray) {
                return get_json_path_array(jsonObject.getYJSONArray(key), paths, ofs + 1);
            } else if (obj is YAPI.YJSONObject) {
                return get_json_path_struct(jsonObject.getYJSONObject(key), paths, ofs + 1);
            }
        }

        return "";
    }

    private string get_json_path_array(YAPI.YJSONArray jsonArray, string[] paths, int ofs)
    {
        int key = Convert.ToInt32(paths[ofs]);
        if (jsonArray.Length <= key) {
            return "";
        }

        YAPI.YJSONContent obj = jsonArray.get(key);
        if (obj != null) {
            if (paths.Length == ofs + 1) {
                return obj.ToString();
            }

            if (obj is YAPI.YJSONArray) {
                return get_json_path_array(jsonArray.getYJSONArray(key), paths, ofs + 1);
            } else if (obj is YAPI.YJSONObject) {
                return get_json_path_struct(jsonArray.getYJSONObject(key), paths, ofs + 1);
            }
        }

        return "";
    }

    public string _get_json_path(string json, string path)
    {
        YAPI.YJSONObject jsonObject = null;
        jsonObject = new YAPI.YJSONObject(json);
        jsonObject.parse();
        string[] split = path.Split(new char[] {'\\', '|'});
        return get_json_path_struct(jsonObject, split, 0);
    }

    public string _decode_json_string(string json)
    {
        int len = json.Length;
        StringBuilder buffer = new StringBuilder(len);
        int decoded_len = SafeNativeMethods._yapiJsonDecodeString(new StringBuilder(json), buffer);
        return buffer.ToString();
    }


    /**
     * <summary>
     *   Preloads the function cache with a specified validity duration.
     * <para>
     *   By default, whenever accessing a device, all function attributes
     *   are kept in cache for the standard duration (5 ms). This method can be
     *   used to temporarily mark the cache as valid for a longer period, in order
     *   to reduce network traffic for instance.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="msValidity">
     *   an integer corresponding to the validity attributed to the
     *   loaded function parameters, in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public YRETCODE load(ulong msValidity)
    {
        YRETCODE functionReturnValue = default(YRETCODE);
        YAPI.YDevice dev = null;
        string errmsg = "";
        YAPI.YJSONObject apires;
        YFUN_DESCR fundescr = default(YFUN_DESCR);
        int res = 0;
        string errbuf = "";
        string funcId = "";
        YDEV_DESCR devdesc = default(YDEV_DESCR);
        string serial = "";
        string funcName = "";
        string funcVal = "";
        YAPI.YJSONObject node;

        lock (_thisLock) {
            // Resolve our reference to our device, load REST API
            res = _getDevice(ref dev, ref errmsg);
            if ((YAPI.YISERR(res))) {
                _throw(res, errmsg);
                functionReturnValue = res;
                return functionReturnValue;
            }

            res = dev.requestAPI(out apires, ref errmsg);
            if (YAPI.YISERR(res)) {
                _throw(res, errmsg);
                functionReturnValue = res;
                return functionReturnValue;
            }

            // Get our function Id
            fundescr = YAPI.yapiGetFunction(_className, _func, ref errmsg);
            if (YAPI.YISERR(fundescr)) {
                _throw(res, errmsg);
                functionReturnValue = fundescr;
                return functionReturnValue;
            }

            devdesc = 0;
            res = YAPI.yapiGetFunctionInfo(fundescr, ref devdesc, ref serial, ref funcId, ref funcName, ref funcVal, ref errbuf);
            if (YAPI.YISERR(res)) {
                _throw(res, errmsg);
                functionReturnValue = res;
                return functionReturnValue;
            }

            _cacheExpiration = YAPI.GetTickCount() + (ulong) msValidity;
            _serial = serial;
            _funId = funcId;
            _hwId = _serial + '.' + _funId;

            try {
                node = apires.getYJSONObject(funcId);
            } catch (Exception) {
                _throw(YAPI.IO_ERROR, "unexpected JSON structure: missing function " + funcId);
                functionReturnValue = YAPI.IO_ERROR;
                return functionReturnValue;
            }

            _parse(node);
            functionReturnValue = YAPI.SUCCESS;
            return functionReturnValue;
        }
    }

    /**
     * <summary>
     *   Invalidates the cache.
     * <para>
     *   Invalidates the cache of the function attributes. Forces the
     *   next call to get_xxx() or loadxxx() to use values that come from the device.
     * </para>
     * <para>
     * @noreturn
     * </para>
     * </summary>
     */
    public void clearCache()
    {
        int res = 0;
        YAPI.YDevice dev = null;
        string errmsg = "";

        lock (_thisLock) {
            // Resolve our reference to our device, load REST API
            res = _getDevice(ref dev, ref errmsg);
            if ((YAPI.YISERR(res))) {
                return;
            }

            dev.clearCache(false);
            if (_cacheExpiration != 0) {
                _cacheExpiration = YAPI.GetTickCount();
            }
        }
    }


    /**
     * <summary>
     *   Gets the <c>YModule</c> object for the device on which the function is located.
     * <para>
     *   If the function cannot be located on any module, the returned instance of
     *   <c>YModule</c> is not shown as on-line.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an instance of <c>YModule</c>
     * </returns>
     */
    public YModule get_module()
    {
        YFUN_DESCR fundescr = default(YFUN_DESCR);
        YDEV_DESCR devdescr = default(YDEV_DESCR);
        string errmsg = "";
        string serial = "";
        string funcId = "";
        string funcName = "";
        string funcValue = "";

        lock (_thisLock) {
            fundescr = YAPI.yapiGetFunction(_className, _func, ref errmsg);
            if (!YAPI.YISERR(fundescr)) {
                if (!YAPI.YISERR(YAPI.yapiGetFunctionInfo(fundescr, ref devdescr, ref serial, ref funcId, ref funcName, ref funcValue, ref errmsg))) {
                    return YModule.FindModule(serial + ".module");
                }
            }

            // return a true YModule object even if it is not a module valid for communicating
            return YModule.FindModule("module_of_" + _className + "_" + _func);
        }
    }

    public YModule module()
    {
        return get_module();
    }

    /**
     * <summary>
     *   Returns a unique identifier of type <c>YFUN_DESCR</c> corresponding to the function.
     * <para>
     *   This identifier can be used to test if two instances of <c>YFunction</c> reference the same
     *   physical function on the same physical device.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an identifier of type <c>YFUN_DESCR</c>.
     * </returns>
     * <para>
     *   If the function has never been contacted, the returned value is <c>YFunction.FUNCTIONDESCRIPTOR_INVALID</c>.
     * </para>
     */
    public YFUN_DESCR get_functionDescriptor()
    {
        return _fundescr;
    }

    public YFUN_DESCR functionDescriptor()
    {
        return get_functionDescriptor();
    }


    /**
     * <summary>
     *   Returns the value of the userData attribute, as previously stored using method
     *   <c>set_userData</c>.
     * <para>
     *   This attribute is never touched directly by the API, and is at disposal of the caller to
     *   store a context.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the object stored previously by the caller.
     * </returns>
     */
    public object get_userData()
    {
        lock (_thisLock) {
            return _userData;
        }
    }

    public object userData()
    {
        return get_userData();
    }

    /**
     * <summary>
     *   Stores a user context provided as argument in the userData attribute of the function.
     * <para>
     *   This attribute is never touched by the API, and is at disposal of the caller to store a context.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="data">
     *   any kind of object to be stored
     * @noreturn
     * </param>
     */
    public void set_userData(object data)
    {
        lock (_thisLock) {
            _userData = data;
        }
    }

    public void setUserData(object data)
    {
        set_userData(data);
    }
}


//--- (generated code: YModule class start)
/**
 * <summary>
 *   The YModule class can be used with all Yoctopuce USB devices.
 * <para>
 *   It can be used to control the module global parameters, and
 *   to enumerate the functions provided by each module.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YModule : YFunction
{
//--- (end of generated code: YModule class start)
    //--- (generated code: YModule definitions)
    public delegate void LogCallback(YModule module, string logline);
    public delegate void ConfigChangeCallback(YModule module);
    public delegate void BeaconCallback(YModule module, int beacon);
    public new delegate void ValueCallback(YModule func, string value);
    public new delegate void TimedReportCallback(YModule func, YMeasure measure);

    public const string PRODUCTNAME_INVALID = YAPI.INVALID_STRING;
    public const string SERIALNUMBER_INVALID = YAPI.INVALID_STRING;
    public const int PRODUCTID_INVALID = YAPI.INVALID_UINT;
    public const int PRODUCTRELEASE_INVALID = YAPI.INVALID_UINT;
    public const string FIRMWARERELEASE_INVALID = YAPI.INVALID_STRING;
    public const int PERSISTENTSETTINGS_LOADED = 0;
    public const int PERSISTENTSETTINGS_SAVED = 1;
    public const int PERSISTENTSETTINGS_MODIFIED = 2;
    public const int PERSISTENTSETTINGS_INVALID = -1;
    public const int LUMINOSITY_INVALID = YAPI.INVALID_UINT;
    public const int BEACON_OFF = 0;
    public const int BEACON_ON = 1;
    public const int BEACON_INVALID = -1;
    public const long UPTIME_INVALID = YAPI.INVALID_LONG;
    public const int USBCURRENT_INVALID = YAPI.INVALID_UINT;
    public const int REBOOTCOUNTDOWN_INVALID = YAPI.INVALID_INT;
    public const int USERVAR_INVALID = YAPI.INVALID_INT;
    protected string _productName = PRODUCTNAME_INVALID;
    protected string _serialNumber = SERIALNUMBER_INVALID;
    protected int _productId = PRODUCTID_INVALID;
    protected int _productRelease = PRODUCTRELEASE_INVALID;
    protected string _firmwareRelease = FIRMWARERELEASE_INVALID;
    protected int _persistentSettings = PERSISTENTSETTINGS_INVALID;
    protected int _luminosity = LUMINOSITY_INVALID;
    protected int _beacon = BEACON_INVALID;
    protected long _upTime = UPTIME_INVALID;
    protected int _usbCurrent = USBCURRENT_INVALID;
    protected int _rebootCountdown = REBOOTCOUNTDOWN_INVALID;
    protected int _userVar = USERVAR_INVALID;
    protected ValueCallback _valueCallbackModule = null;
    protected LogCallback _logCallback = null;
    protected ConfigChangeCallback _confChangeCallback = null;
    protected BeaconCallback _beaconCallback = null;
    //--- (end of generated code: YModule definitions)
    public static Dictionary<YModule, int> _moduleCallbackList = new Dictionary<YModule, int>();

    public YModule(string func) : base(func)
    {
        _className = "Module";
        //--- (generated code: YModule attributes initialization)
        //--- (end of generated code: YModule attributes initialization)
    }

    static void _updateModuleCallbackList(YModule module, bool add)
    {
        if (add) {
            module.isOnline();
            if (!_moduleCallbackList.ContainsKey(module)) {
                _moduleCallbackList[module] = 1;
            } else {
                _moduleCallbackList[module] += 1;
            }
        } else {
            if (_moduleCallbackList.ContainsKey(module) && _moduleCallbackList[module] > 1) {
                _moduleCallbackList[module] -= 1;
            }
        }
    }


    //--- (generated code: YModule implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("productName"))
        {
            _productName = json_val.getString("productName");
        }
        if (json_val.has("serialNumber"))
        {
            _serialNumber = json_val.getString("serialNumber");
        }
        if (json_val.has("productId"))
        {
            _productId = json_val.getInt("productId");
        }
        if (json_val.has("productRelease"))
        {
            _productRelease = json_val.getInt("productRelease");
        }
        if (json_val.has("firmwareRelease"))
        {
            _firmwareRelease = json_val.getString("firmwareRelease");
        }
        if (json_val.has("persistentSettings"))
        {
            _persistentSettings = json_val.getInt("persistentSettings");
        }
        if (json_val.has("luminosity"))
        {
            _luminosity = json_val.getInt("luminosity");
        }
        if (json_val.has("beacon"))
        {
            _beacon = json_val.getInt("beacon") > 0 ? 1 : 0;
        }
        if (json_val.has("upTime"))
        {
            _upTime = json_val.getLong("upTime");
        }
        if (json_val.has("usbCurrent"))
        {
            _usbCurrent = json_val.getInt("usbCurrent");
        }
        if (json_val.has("rebootCountdown"))
        {
            _rebootCountdown = json_val.getInt("rebootCountdown");
        }
        if (json_val.has("userVar"))
        {
            _userVar = json_val.getInt("userVar");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the commercial name of the module, as set by the factory.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the commercial name of the module, as set by the factory
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.PRODUCTNAME_INVALID</c>.
     * </para>
     */
    public string get_productName()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PRODUCTNAME_INVALID;
                }
            }
            res = this._productName;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the serial number of the module, as set by the factory.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the serial number of the module, as set by the factory
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.SERIALNUMBER_INVALID</c>.
     * </para>
     */
    public override string get_serialNumber()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SERIALNUMBER_INVALID;
                }
            }
            res = this._serialNumber;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the USB device identifier of the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the USB device identifier of the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.PRODUCTID_INVALID</c>.
     * </para>
     */
    public int get_productId()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PRODUCTID_INVALID;
                }
            }
            res = this._productId;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the release number of the module hardware, preprogrammed at the factory.
     * <para>
     *   The original hardware release returns value 1, revision B returns value 2, etc.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the release number of the module hardware, preprogrammed at the factory
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.PRODUCTRELEASE_INVALID</c>.
     * </para>
     */
    public int get_productRelease()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PRODUCTRELEASE_INVALID;
                }
            }
            res = this._productRelease;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the version of the firmware embedded in the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the version of the firmware embedded in the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.FIRMWARERELEASE_INVALID</c>.
     * </para>
     */
    public string get_firmwareRelease()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return FIRMWARERELEASE_INVALID;
                }
            }
            res = this._firmwareRelease;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the current state of persistent module settings.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YModule.PERSISTENTSETTINGS_LOADED</c>, <c>YModule.PERSISTENTSETTINGS_SAVED</c> and
     *   <c>YModule.PERSISTENTSETTINGS_MODIFIED</c> corresponding to the current state of persistent module settings
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.PERSISTENTSETTINGS_INVALID</c>.
     * </para>
     */
    public int get_persistentSettings()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PERSISTENTSETTINGS_INVALID;
                }
            }
            res = this._persistentSettings;
        }
        return res;
    }

    public int set_persistentSettings(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("persistentSettings", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the luminosity of the  module informative LEDs (from 0 to 100).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the luminosity of the  module informative LEDs (from 0 to 100)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.LUMINOSITY_INVALID</c>.
     * </para>
     */
    public int get_luminosity()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LUMINOSITY_INVALID;
                }
            }
            res = this._luminosity;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the luminosity of the module informative leds.
     * <para>
     *   The parameter is a
     *   value between 0 and 100.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the luminosity of the module informative leds
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
    public int set_luminosity(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("luminosity", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the state of the localization beacon.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YModule.BEACON_OFF</c> or <c>YModule.BEACON_ON</c>, according to the state of the localization beacon
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.BEACON_INVALID</c>.
     * </para>
     */
    public int get_beacon()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BEACON_INVALID;
                }
            }
            res = this._beacon;
        }
        return res;
    }

    /**
     * <summary>
     *   Turns on or off the module localization beacon.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YModule.BEACON_OFF</c> or <c>YModule.BEACON_ON</c>
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
    public int set_beacon(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval > 0 ? "1" : "0");
            return _setAttr("beacon", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the number of milliseconds spent since the module was powered on.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of milliseconds spent since the module was powered on
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.UPTIME_INVALID</c>.
     * </para>
     */
    public long get_upTime()
    {
        long res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return UPTIME_INVALID;
                }
            }
            res = this._upTime;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the current consumed by the module on the USB bus, in milli-amps.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current consumed by the module on the USB bus, in milli-amps
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.USBCURRENT_INVALID</c>.
     * </para>
     */
    public int get_usbCurrent()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return USBCURRENT_INVALID;
                }
            }
            res = this._usbCurrent;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the remaining number of seconds before the module restarts, or zero when no
     *   reboot has been scheduled.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the remaining number of seconds before the module restarts, or zero when no
     *   reboot has been scheduled
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.REBOOTCOUNTDOWN_INVALID</c>.
     * </para>
     */
    public int get_rebootCountdown()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return REBOOTCOUNTDOWN_INVALID;
                }
            }
            res = this._rebootCountdown;
        }
        return res;
    }

    public int set_rebootCountdown(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("rebootCountdown", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the value previously stored in this attribute.
     * <para>
     *   On startup and after a device reboot, the value is always reset to zero.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the value previously stored in this attribute
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.USERVAR_INVALID</c>.
     * </para>
     */
    public int get_userVar()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return USERVAR_INVALID;
                }
            }
            res = this._userVar;
        }
        return res;
    }

    /**
     * <summary>
     *   Stores a 32 bit value in the device RAM.
     * <para>
     *   This attribute is at programmer disposal,
     *   should he need to store a state variable.
     *   On startup and after a device reboot, the value is always reset to zero.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer
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
    public int set_userVar(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("userVar", rest_val);
        }
    }

    /**
     * <summary>
     *   Allows you to find a module from its serial number or from its logical name.
     * <para>
     * </para>
     * <para>
     *   This function does not require that the module is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YModule.isOnline()</c> to test if the module is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a module by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * <para>
     * </para>
     * <para>
     *   If a call to this object's is_online() method returns FALSE although
     *   you are certain that the device is plugged, make sure that you did
     *   call registerHub() at application initialization time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="func">
     *   a string containing either the serial number or
     *   the logical name of the desired module
     * </param>
     * <returns>
     *   a <c>YModule</c> object allowing you to drive the module
     *   or get additional information on the module.
     * </returns>
     */
    public static YModule FindModule(string func)
    {
        YModule obj;
        string cleanHwId;
        int modpos;
        cleanHwId = func;
        modpos = (func).IndexOf(".module");
        if (modpos != ((func).Length - 7)) {
            cleanHwId = func + ".module";
        }
        lock (YAPI.globalLock) {
            obj = (YModule) YFunction._FindFromCache("Module", cleanHwId);
            if (obj == null) {
                obj = new YModule(cleanHwId);
                YFunction._AddToCache("Module", cleanHwId, obj);
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
        this._valueCallbackModule = callback;
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
        if (this._valueCallbackModule != null) {
            this._valueCallbackModule(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    public virtual string get_productNameAndRevision()
    {
        string prodname;
        int prodrel;
        string fullname;

        prodname = this.get_productName();
        prodrel = this.get_productRelease();
        if (prodrel > 1) {
            fullname = ""+ prodname+" rev. "+((char)(64+prodrel)).ToString();
        } else {
            fullname = prodname;
        }
        return fullname;
    }

    /**
     * <summary>
     *   Saves current settings in the nonvolatile memory of the module.
     * <para>
     *   Warning: the number of allowed save operations during a module life is
     *   limited (about 100000 cycles). Do not call this function within a loop.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int saveToFlash()
    {
        return this.set_persistentSettings(PERSISTENTSETTINGS_SAVED);
    }

    /**
     * <summary>
     *   Reloads the settings stored in the nonvolatile memory, as
     *   when the module is powered on.
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
    public virtual int revertFromFlash()
    {
        return this.set_persistentSettings(PERSISTENTSETTINGS_LOADED);
    }

    /**
     * <summary>
     *   Schedules a simple module reboot after the given number of seconds.
     * <para>
     * </para>
     * </summary>
     * <param name="secBeforeReboot">
     *   number of seconds before rebooting
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int reboot(int secBeforeReboot)
    {
        return this.set_rebootCountdown(secBeforeReboot);
    }

    /**
     * <summary>
     *   Schedules a module reboot into special firmware update mode.
     * <para>
     * </para>
     * </summary>
     * <param name="secBeforeReboot">
     *   number of seconds before rebooting
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int triggerFirmwareUpdate(int secBeforeReboot)
    {
        return this.set_rebootCountdown(-secBeforeReboot);
    }

    public virtual void _startStopDevLog(string serial, bool start)
    {
        int i_start;
        if (start) {
            i_start = 1;
        } else {
            i_start = 0;
        }

        SafeNativeMethods._yapiStartStopDeviceLogCallback(new StringBuilder(serial), i_start);
    }

    /**
     * <summary>
     *   Registers a device log callback function.
     * <para>
     *   This callback will be called each time
     *   that a module sends a new log message. Mostly useful to debug a Yoctopuce module.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the module object that emitted the log message, and the character string containing the log.
     *   On failure, throws an exception or returns a negative error code.
     * </param>
     */
    public virtual int registerLogCallback(LogCallback callback)
    {
        string serial;

        serial = this.get_serialNumber();
        if (serial == YAPI.INVALID_STRING) {
            return YAPI.DEVICE_NOT_FOUND;
        }
        this._logCallback = callback;
        this._startStopDevLog(serial, callback != null);
        return 0;
    }

    public virtual LogCallback get_logCallback()
    {
        return this._logCallback;
    }

    /**
     * <summary>
     *   Register a callback function, to be called when a persistent settings in
     *   a device configuration has been changed (e.g.
     * <para>
     *   change of unit, etc).
     * </para>
     * </summary>
     * <param name="callback">
     *   a procedure taking a YModule parameter, or <c>null</c>
     *   to unregister a previously registered  callback.
     * </param>
     */
    public virtual int registerConfigChangeCallback(ConfigChangeCallback callback)
    {
        if (callback != null) {
            YModule._updateModuleCallbackList(this, true);
        } else {
            YModule._updateModuleCallbackList(this, false);
        }
        this._confChangeCallback = callback;
        return 0;
    }

    public virtual int _invokeConfigChangeCallback()
    {
        if (this._confChangeCallback != null) {
            this._confChangeCallback(this);
        }
        return 0;
    }

    /**
     * <summary>
     *   Register a callback function, to be called when the localization beacon of the module
     *   has been changed.
     * <para>
     *   The callback function should take two arguments: the YModule object of
     *   which the beacon has changed, and an integer describing the new beacon state.
     * </para>
     * </summary>
     * <param name="callback">
     *   The callback function to call, or <c>null</c> to unregister a
     *   previously registered callback.
     * </param>
     */
    public virtual int registerBeaconCallback(BeaconCallback callback)
    {
        if (callback != null) {
            YModule._updateModuleCallbackList(this, true);
        } else {
            YModule._updateModuleCallbackList(this, false);
        }
        this._beaconCallback = callback;
        return 0;
    }

    public virtual int _invokeBeaconCallback(int beaconState)
    {
        if (this._beaconCallback != null) {
            this._beaconCallback(this, beaconState);
        }
        return 0;
    }

    /**
     * <summary>
     *   Triggers a configuration change callback, to check if they are supported or not.
     * <para>
     * </para>
     * </summary>
     */
    public virtual int triggerConfigChangeCallback()
    {
        this._setAttr("persistentSettings", "2");
        return 0;
    }

    /**
     * <summary>
     *   Tests whether the byn file is valid for this module.
     * <para>
     *   This method is useful to test if the module needs to be updated.
     *   It is possible to pass a directory as argument instead of a file. In this case, this method returns
     *   the path of the most recent
     *   appropriate <c>.byn</c> file. If the parameter <c>onlynew</c> is true, the function discards
     *   firmwares that are older or
     *   equal to the installed firmware.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="path">
     *   the path of a byn file or a directory that contains byn files
     * </param>
     * <param name="onlynew">
     *   returns only files that are strictly newer
     * </param>
     * <para>
     * </para>
     * <returns>
     *   the path of the byn file to use or a empty string if no byn files matches the requirement
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a string that start with "error:".
     * </para>
     */
    public virtual string checkFirmware(string path, bool onlynew)
    {
        string serial;
        int release;
        string tmp_res;
        if (onlynew) {
            release = YAPI._atoi(this.get_firmwareRelease());
        } else {
            release = 0;
        }
        //may throw an exception
        serial = this.get_serialNumber();
        tmp_res = YFirmwareUpdate.CheckFirmware(serial, path, release);
        if ((tmp_res).IndexOf("error:") == 0) {
            this._throw(YAPI.INVALID_ARGUMENT, tmp_res);
        }
        return tmp_res;
    }

    /**
     * <summary>
     *   Prepares a firmware update of the module.
     * <para>
     *   This method returns a <c>YFirmwareUpdate</c> object which
     *   handles the firmware update process.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="path">
     *   the path of the <c>.byn</c> file to use.
     * </param>
     * <param name="force">
     *   true to force the firmware update even if some prerequisites appear not to be met
     * </param>
     * <returns>
     *   a <c>YFirmwareUpdate</c> object or NULL on error.
     * </returns>
     */
    public virtual YFirmwareUpdate updateFirmwareEx(string path, bool force)
    {
        string serial;
        byte[] settings;

        serial = this.get_serialNumber();
        settings = this.get_allSettings();
        if ((settings).Length == 0) {
            this._throw(YAPI.IO_ERROR, "Unable to get device settings");
            settings = YAPI.DefaultEncoding.GetBytes("error:Unable to get device settings");
        }
        return new YFirmwareUpdate(serial, path, settings, force);
    }

    /**
     * <summary>
     *   Prepares a firmware update of the module.
     * <para>
     *   This method returns a <c>YFirmwareUpdate</c> object which
     *   handles the firmware update process.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="path">
     *   the path of the <c>.byn</c> file to use.
     * </param>
     * <returns>
     *   a <c>YFirmwareUpdate</c> object or NULL on error.
     * </returns>
     */
    public virtual YFirmwareUpdate updateFirmware(string path)
    {
        return this.updateFirmwareEx(path, false);
    }

    /**
     * <summary>
     *   Returns all the settings and uploaded files of the module.
     * <para>
     *   Useful to backup all the
     *   logical names, calibrations parameters, and uploaded files of a device.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a binary buffer with all the settings.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an binary object of size 0.
     * </para>
     */
    public virtual byte[] get_allSettings()
    {
        byte[] settings;
        byte[] json;
        byte[] res;
        string sep;
        string name;
        string item;
        string t_type;
        string id;
        string url;
        string file_data;
        byte[] file_data_bin;
        byte[] temp_data_bin;
        string ext_settings;
        List<string> filelist = new List<string>();
        List<string> templist = new List<string>();

        settings = this._download("api.json");
        if ((settings).Length == 0) {
            return settings;
        }
        ext_settings = ", \"extras\":[";
        templist = this.get_functionIds("Temperature");
        sep = "";
        for (int ii = 0; ii <  templist.Count; ii++) {
            if (YAPI._atoi(this.get_firmwareRelease()) > 9000) {
                url = "api/"+ templist[ii]+"/sensorType";
                t_type = YAPI.DefaultEncoding.GetString(this._download(url));
                if (t_type == "RES_NTC" || t_type == "RES_LINEAR") {
                    id = ( templist[ii]).Substring( 11, ( templist[ii]).Length - 11);
                    if (id == "") {
                        id = "1";
                    }
                    temp_data_bin = this._download("extra.json?page="+id);
                    if ((temp_data_bin).Length > 0) {
                        item = ""+ sep+"{\"fid\":\""+  templist[ii]+"\", \"json\":"+YAPI.DefaultEncoding.GetString(temp_data_bin)+"}\n";
                        ext_settings = ext_settings + item;
                        sep = ",";
                    }
                }
            }
        }
        ext_settings = ext_settings + "],\n\"files\":[";
        if (this.hasFunction("files")) {
            json = this._download("files.json?a=dir&f=");
            if ((json).Length == 0) {
                return json;
            }
            filelist = this._json_get_array(json);
            sep = "";
            for (int ii = 0; ii <  filelist.Count; ii++) {
                name = this._json_get_key(YAPI.DefaultEncoding.GetBytes( filelist[ii]), "name");
                if (((name).Length > 0) && !(name == "startupConf.json")) {
                    file_data_bin = this._download(this._escapeAttr(name));
                    file_data = YAPI._bytesToHexStr(file_data_bin, 0, file_data_bin.Length);
                    item = ""+ sep+"{\"name\":\""+ name+"\", \"data\":\""+file_data+"\"}\n";
                    ext_settings = ext_settings + item;
                    sep = ",";
                }
            }
        }
        res = YAPI.DefaultEncoding.GetBytes("{ \"api\":" + YAPI.DefaultEncoding.GetString(settings) + ext_settings + "]}");
        return res;
    }

    public virtual int loadThermistorExtra(string funcId, string jsonExtra)
    {
        List<string> values = new List<string>();
        string url;
        string curr;
        string currTemp;
        int ofs;
        int size;
        url = "api/" + funcId + ".json?command=Z";

        this._download(url);
        // add records in growing resistance value
        values = this._json_get_array(YAPI.DefaultEncoding.GetBytes(jsonExtra));
        ofs = 0;
        size = values.Count;
        while (ofs + 1 < size) {
            curr = values[ofs];
            currTemp = values[ofs + 1];
            url = "api/"+ funcId+".json?command=m"+ curr+":"+currTemp;
            this._download(url);
            ofs = ofs + 2;
        }
        return YAPI.SUCCESS;
    }

    public virtual int set_extraSettings(string jsonExtra)
    {
        List<string> extras = new List<string>();
        string functionId;
        string data;
        extras = this._json_get_array(YAPI.DefaultEncoding.GetBytes(jsonExtra));
        for (int ii = 0; ii <  extras.Count; ii++) {
            functionId = this._get_json_path( extras[ii], "fid");
            functionId = this._decode_json_string(functionId);
            data = this._get_json_path( extras[ii], "json");
            if (this.hasFunction(functionId)) {
                this.loadThermistorExtra(functionId, data);
            }
        }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Restores all the settings and uploaded files to the module.
     * <para>
     *   This method is useful to restore all the logical names and calibrations parameters,
     *   uploaded files etc. of a device from a backup.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modifications must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="settings">
     *   a binary buffer with all the settings.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_allSettingsAndFiles(byte[] settings)
    {
        byte[] down;
        string json;
        string json_api;
        string json_files;
        string json_extra;
        json = YAPI.DefaultEncoding.GetString(settings);
        json_api = this._get_json_path(json, "api");
        if (json_api == "") {
            return this.set_allSettings(settings);
        }
        json_extra = this._get_json_path(json, "extras");
        if (!(json_extra == "")) {
            this.set_extraSettings(json_extra);
        }
        this.set_allSettings(YAPI.DefaultEncoding.GetBytes(json_api));
        if (this.hasFunction("files")) {
            List<string> files = new List<string>();
            string res;
            string name;
            string data;
            down = this._download("files.json?a=format");
            res = this._get_json_path(YAPI.DefaultEncoding.GetString(down), "res");
            res = this._decode_json_string(res);
            if (!(res == "ok")) { this._throw( YAPI.IO_ERROR, "format failed"); return YAPI.IO_ERROR; }
            json_files = this._get_json_path(json, "files");
            files = this._json_get_array(YAPI.DefaultEncoding.GetBytes(json_files));
            for (int ii = 0; ii <  files.Count; ii++) {
                name = this._get_json_path( files[ii], "name");
                name = this._decode_json_string(name);
                data = this._get_json_path( files[ii], "data");
                data = this._decode_json_string(data);
                this._upload(name, YAPI._hexStrToBin(data));
            }
        }
        // Apply settings a second time for file-dependent settings and dynamic sensor nodes
        return this.set_allSettings(YAPI.DefaultEncoding.GetBytes(json_api));
    }

    /**
     * <summary>
     *   Tests if the device includes a specific function.
     * <para>
     *   This method takes a function identifier
     *   and returns a boolean.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="funcId">
     *   the requested function identifier
     * </param>
     * <returns>
     *   true if the device has the function identifier
     * </returns>
     */
    public virtual bool hasFunction(string funcId)
    {
        int count;
        int i;
        string fid;

        count = this.functionCount();
        i = 0;
        while (i < count) {
            fid = this.functionId(i);
            if (fid == funcId) {
                return true;
            }
            i = i + 1;
        }
        return false;
    }

    /**
     * <summary>
     *   Retrieve all hardware identifier that match the type passed in argument.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="funType">
     *   The type of function (Relay, LightSensor, Voltage,...)
     * </param>
     * <returns>
     *   an array of strings.
     * </returns>
     */
    public virtual List<string> get_functionIds(string funType)
    {
        int count;
        int i;
        string ftype;
        List<string> res = new List<string>();

        count = this.functionCount();
        i = 0;
        while (i < count) {
            ftype = this.functionType(i);
            if (ftype == funType) {
                res.Add(this.functionId(i));
            } else {
                ftype = this.functionBaseType(i);
                if (ftype == funType) {
                    res.Add(this.functionId(i));
                }
            }
            i = i + 1;
        }
        return res;
    }

    public virtual byte[] _flattenJsonStruct(byte[] jsoncomplex)
    {
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        StringBuilder smallbuff = new StringBuilder(1024);
        StringBuilder bigbuff = null;
        int buffsize;
        int fullsize;
        int res;
        string jsonflat;
        string jsoncomplexstr;
        fullsize = 0;
        jsoncomplexstr = YAPI.DefaultEncoding.GetString(jsoncomplex);
        res = SafeNativeMethods._yapiGetAllJsonKeys(new StringBuilder(jsoncomplexstr), smallbuff, 1024, ref fullsize, errmsg);
        if (res < 0) {
            this._throw(YAPI.INVALID_ARGUMENT, errmsg.ToString());
            jsonflat = "error:" + errmsg.ToString();
            return YAPI.DefaultEncoding.GetBytes(jsonflat);
        }
        if (fullsize <= 1024) {
            jsonflat = smallbuff.ToString();
        } else {
            fullsize = fullsize * 2;
            buffsize = fullsize;
            bigbuff = new StringBuilder(buffsize);
            res = SafeNativeMethods._yapiGetAllJsonKeys(new StringBuilder(jsoncomplexstr), bigbuff, buffsize, ref fullsize, errmsg);
            if (res < 0) {
                this._throw(YAPI.INVALID_ARGUMENT, errmsg.ToString());
                jsonflat = "error:" + errmsg.ToString();
            } else {
                jsonflat = bigbuff.ToString();
            }
            bigbuff = null;
        }
        return YAPI.DefaultEncoding.GetBytes(jsonflat);
    }

    public virtual int calibVersion(string cparams)
    {
        if (cparams == "0,") {
            return 3;
        }
        if ((cparams).IndexOf(",") >= 0) {
            if ((cparams).IndexOf(" ") > 0) {
                return 3;
            } else {
                return 1;
            }
        }
        if (cparams == "" || cparams == "0") {
            return 1;
        }
        if (((cparams).Length < 2) || ((cparams).IndexOf(".") >= 0)) {
            return 0;
        } else {
            return 2;
        }
    }

    public virtual int calibScale(string unit_name, string sensorType)
    {
        if (unit_name == "g" || unit_name == "gauss" || unit_name == "W") {
            return 1000;
        }
        if (unit_name == "C") {
            if (sensorType == "") {
                return 16;
            }
            if (YAPI._atoi(sensorType) < 8) {
                return 16;
            } else {
                return 100;
            }
        }
        if (unit_name == "m" || unit_name == "deg") {
            return 10;
        }
        return 1;
    }

    public virtual int calibOffset(string unit_name)
    {
        if (unit_name == "% RH" || unit_name == "mbar" || unit_name == "lx") {
            return 0;
        }
        return 32767;
    }

    public virtual string calibConvert(string param, string currentFuncValue, string unit_name, string sensorType)
    {
        int paramVer;
        int funVer;
        int funScale;
        int funOffset;
        int paramScale;
        int paramOffset;
        List<int> words = new List<int>();
        List<string> words_str = new List<string>();
        List<double> calibData = new List<double>();
        List<int> iCalib = new List<int>();
        int calibType;
        int i;
        int maxSize;
        double ratio;
        int nPoints;
        double wordVal;
        // Initial guess for parameter encoding
        paramVer = this.calibVersion(param);
        funVer = this.calibVersion(currentFuncValue);
        funScale = this.calibScale(unit_name, sensorType);
        funOffset = this.calibOffset(unit_name);
        paramScale = funScale;
        paramOffset = funOffset;
        if (funVer < 3) {
            // Read the effective device scale if available
            if (funVer == 2) {
                words = YAPI._decodeWords(currentFuncValue);
                if ((words[0] == 1366) && (words[1] == 12500)) {
                    // Yocto-3D RefFrame used a special encoding
                    funScale = 1;
                    funOffset = 0;
                } else {
                    funScale = words[1];
                    funOffset = words[0];
                }
            } else {
                if (funVer == 1) {
                    if (currentFuncValue == "" || (YAPI._atoi(currentFuncValue) > 10)) {
                        funScale = 0;
                    }
                }
            }
        }
        calibData.Clear();
        calibType = 0;
        if (paramVer < 3) {
            // Handle old 16 bit parameters formats
            if (paramVer == 2) {
                words = YAPI._decodeWords(param);
                if ((words[0] == 1366) && (words[1] == 12500)) {
                    // Yocto-3D RefFrame used a special encoding
                    paramScale = 1;
                    paramOffset = 0;
                } else {
                    paramScale = words[1];
                    paramOffset = words[0];
                }
                if ((words.Count >= 3) && (words[2] > 0)) {
                    maxSize = 3 + 2 * ((words[2]) % (10));
                    if (maxSize > words.Count) {
                        maxSize = words.Count;
                    }
                    i = 3;
                    while (i < maxSize) {
                        calibData.Add((double) words[i]);
                        i = i + 1;
                    }
                }
            } else {
                if (paramVer == 1) {
                    words_str = new List<string>(param.Split(new Char[] {','}));
                    for (int ii = 0; ii < words_str.Count; ii++) {
                        words.Add(YAPI._atoi(words_str[ii]));
                    }
                    if (param == "" || (words[0] > 10)) {
                        paramScale = 0;
                    }
                    if ((words.Count > 0) && (words[0] > 0)) {
                        maxSize = 1 + 2 * ((words[0]) % (10));
                        if (maxSize > words.Count) {
                            maxSize = words.Count;
                        }
                        i = 1;
                        while (i < maxSize) {
                            calibData.Add((double) words[i]);
                            i = i + 1;
                        }
                    }
                } else {
                    if (paramVer == 0) {
                        ratio = Double.Parse(param);
                        if (ratio > 0) {
                            calibData.Add(0.0);
                            calibData.Add(0.0);
                            calibData.Add(Math.Round(65535 / ratio));
                            calibData.Add(65535.0);
                        }
                    }
                }
            }
            i = 0;
            while (i < calibData.Count) {
                if (paramScale > 0) {
                    // scalar decoding
                    calibData[i] = (calibData[i] - paramOffset) / paramScale;
                } else {
                    // floating-point decoding
                    calibData[i] = YAPI._decimalToDouble((int) Math.Round(calibData[i]));
                }
                i = i + 1;
            }
        } else {
            // Handle latest 32bit parameter format
            iCalib = YAPI._decodeFloats(param);
            calibType = (int) Math.Round(iCalib[0] / 1000.0);
            if (calibType >= 30) {
                calibType = calibType - 30;
            }
            i = 1;
            while (i < iCalib.Count) {
                calibData.Add(iCalib[i] / 1000.0);
                i = i + 1;
            }
        }
        if (funVer >= 3) {
            // Encode parameters in new format
            if (calibData.Count == 0) {
                param = "0,";
            } else {
                param = (30 + calibType).ToString();
                i = 0;
                while (i < calibData.Count) {
                    if (((i) & (1)) > 0) {
                        param = param + ":";
                    } else {
                        param = param + " ";
                    }
                    param = param + ((int) Math.Round(calibData[i] * 1000.0 / 1000.0)).ToString();
                    i = i + 1;
                }
                param = param + ",";
            }
        } else {
            if (funVer >= 1) {
                // Encode parameters for older devices
                nPoints = ((calibData.Count) / (2));
                param = (nPoints).ToString();
                i = 0;
                while (i < 2 * nPoints) {
                    if (funScale == 0) {
                        wordVal = YAPI._doubleToDecimal((int) Math.Round(calibData[i]));
                    } else {
                        wordVal = calibData[i] * funScale + funOffset;
                    }
                    param = param + "," + (Math.Round(wordVal)).ToString();
                    i = i + 1;
                }
            } else {
                // Initial V0 encoding used for old Yocto-Light
                if (calibData.Count == 4) {
                    param = (Math.Round(1000 * (calibData[3] - calibData[1]) / calibData[2] - calibData[0])).ToString();
                }
            }
        }
        return param;
    }

    public virtual int _tryExec(string url)
    {
        int res;
        int done;
        res = YAPI.SUCCESS;
        done = 1;
        try {
            this._download(url);
        } catch {
            done = 0;
        }
        if (done == 0) {
            // retry silently after a short wait
            try {
                {string ignore=""; YAPI.Sleep(500, ref ignore);};
                this._download(url);
            } catch {
                // second failure, return error code
                res = this.get_errorType();
            }
        }
        return res;
    }

    /**
     * <summary>
     *   Restores all the settings of the device.
     * <para>
     *   Useful to restore all the logical names and calibrations parameters
     *   of a module from a backup.Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modifications must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="settings">
     *   a binary buffer with all the settings.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int set_allSettings(byte[] settings)
    {
        List<string> restoreLast = new List<string>();
        byte[] old_json_flat;
        List<string> old_dslist = new List<string>();
        List<string> old_jpath = new List<string>();
        List<int> old_jpath_len = new List<int>();
        List<string> old_val_arr = new List<string>();
        byte[] actualSettings;
        List<string> new_dslist = new List<string>();
        List<string> new_jpath = new List<string>();
        List<int> new_jpath_len = new List<int>();
        List<string> new_val_arr = new List<string>();
        int cpos;
        int eqpos;
        int leng;
        int i;
        int j;
        int subres;
        int res;
        string njpath;
        string jpath;
        string fun;
        string attr;
        string value;
        string url;
        string tmp;
        string new_calib;
        string sensorType;
        string unit_name;
        string newval;
        string oldval;
        string old_calib;
        string each_str;
        bool do_update;
        bool found;
        res = YAPI.SUCCESS;
        tmp = YAPI.DefaultEncoding.GetString(settings);
        tmp = this._get_json_path(tmp, "api");
        if (!(tmp == "")) {
            settings = YAPI.DefaultEncoding.GetBytes(tmp);
        }
        oldval = "";
        newval = "";
        old_json_flat = this._flattenJsonStruct(settings);
        old_dslist = this._json_get_array(old_json_flat);
        for (int ii = 0; ii < old_dslist.Count; ii++) {
            each_str = this._json_get_string(YAPI.DefaultEncoding.GetBytes(old_dslist[ii]));
            // split json path and attr
            leng = (each_str).Length;
            eqpos = (each_str).IndexOf("=");
            if ((eqpos < 0) || (leng == 0)) {
                this._throw(YAPI.INVALID_ARGUMENT, "Invalid settings");
                return YAPI.INVALID_ARGUMENT;
            }
            jpath = (each_str).Substring( 0, eqpos);
            eqpos = eqpos + 1;
            value = (each_str).Substring( eqpos, leng - eqpos);
            old_jpath.Add(jpath);
            old_jpath_len.Add((jpath).Length);
            old_val_arr.Add(value);
        }

        try {
            actualSettings = this._download("api.json");
        } catch {
            // retry silently after a short wait
            {string ignore=""; YAPI.Sleep(500, ref ignore);};
            actualSettings = this._download("api.json");
        }
        actualSettings = this._flattenJsonStruct(actualSettings);
        new_dslist = this._json_get_array(actualSettings);
        for (int ii = 0; ii < new_dslist.Count; ii++) {
            // remove quotes
            each_str = this._json_get_string(YAPI.DefaultEncoding.GetBytes(new_dslist[ii]));
            // split json path and attr
            leng = (each_str).Length;
            eqpos = (each_str).IndexOf("=");
            if ((eqpos < 0) || (leng == 0)) {
                this._throw(YAPI.INVALID_ARGUMENT, "Invalid settings");
                return YAPI.INVALID_ARGUMENT;
            }
            jpath = (each_str).Substring( 0, eqpos);
            eqpos = eqpos + 1;
            value = (each_str).Substring( eqpos, leng - eqpos);
            new_jpath.Add(jpath);
            new_jpath_len.Add((jpath).Length);
            new_val_arr.Add(value);
        }
        i = 0;
        while (i < new_jpath.Count) {
            njpath = new_jpath[i];
            leng = (njpath).Length;
            cpos = (njpath).IndexOf("/");
            if ((cpos < 0) || (leng == 0)) {
                continue;
            }
            fun = (njpath).Substring( 0, cpos);
            cpos = cpos + 1;
            attr = (njpath).Substring( cpos, leng - cpos);
            do_update = true;
            if (fun == "services") {
                do_update = false;
            }
            if ((do_update) && (attr == "firmwareRelease")) {
                do_update = false;
            }
            if ((do_update) && (attr == "usbCurrent")) {
                do_update = false;
            }
            if ((do_update) && (attr == "upTime")) {
                do_update = false;
            }
            if ((do_update) && (attr == "persistentSettings")) {
                do_update = false;
            }
            if ((do_update) && (attr == "adminPassword")) {
                do_update = false;
            }
            if ((do_update) && (attr == "userPassword")) {
                do_update = false;
            }
            if ((do_update) && (attr == "rebootCountdown")) {
                do_update = false;
            }
            if ((do_update) && (attr == "advertisedValue")) {
                do_update = false;
            }
            if ((do_update) && (attr == "poeCurrent")) {
                do_update = false;
            }
            if ((do_update) && (attr == "readiness")) {
                do_update = false;
            }
            if ((do_update) && (attr == "ipAddress")) {
                do_update = false;
            }
            if ((do_update) && (attr == "subnetMask")) {
                do_update = false;
            }
            if ((do_update) && (attr == "router")) {
                do_update = false;
            }
            if ((do_update) && (attr == "linkQuality")) {
                do_update = false;
            }
            if ((do_update) && (attr == "ssid")) {
                do_update = false;
            }
            if ((do_update) && (attr == "channel")) {
                do_update = false;
            }
            if ((do_update) && (attr == "security")) {
                do_update = false;
            }
            if ((do_update) && (attr == "message")) {
                do_update = false;
            }
            if ((do_update) && (attr == "signalValue")) {
                do_update = false;
            }
            if ((do_update) && (attr == "currentValue")) {
                do_update = false;
            }
            if ((do_update) && (attr == "currentRawValue")) {
                do_update = false;
            }
            if ((do_update) && (attr == "currentRunIndex")) {
                do_update = false;
            }
            if ((do_update) && (attr == "pulseTimer")) {
                do_update = false;
            }
            if ((do_update) && (attr == "lastTimePressed")) {
                do_update = false;
            }
            if ((do_update) && (attr == "lastTimeReleased")) {
                do_update = false;
            }
            if ((do_update) && (attr == "filesCount")) {
                do_update = false;
            }
            if ((do_update) && (attr == "freeSpace")) {
                do_update = false;
            }
            if ((do_update) && (attr == "timeUTC")) {
                do_update = false;
            }
            if ((do_update) && (attr == "rtcTime")) {
                do_update = false;
            }
            if ((do_update) && (attr == "unixTime")) {
                do_update = false;
            }
            if ((do_update) && (attr == "dateTime")) {
                do_update = false;
            }
            if ((do_update) && (attr == "rawValue")) {
                do_update = false;
            }
            if ((do_update) && (attr == "lastMsg")) {
                do_update = false;
            }
            if ((do_update) && (attr == "delayedPulseTimer")) {
                do_update = false;
            }
            if ((do_update) && (attr == "rxCount")) {
                do_update = false;
            }
            if ((do_update) && (attr == "txCount")) {
                do_update = false;
            }
            if ((do_update) && (attr == "msgCount")) {
                do_update = false;
            }
            if ((do_update) && (attr == "rxMsgCount")) {
                do_update = false;
            }
            if ((do_update) && (attr == "txMsgCount")) {
                do_update = false;
            }
            if (do_update) {
                do_update = false;
                newval = new_val_arr[i];
                j = 0;
                found = false;
                while ((j < old_jpath.Count) && !(found)) {
                    if ((new_jpath_len[i] == old_jpath_len[j]) && (new_jpath[i] == old_jpath[j])) {
                        found = true;
                        oldval = old_val_arr[j];
                        if (!(newval == oldval)) {
                            do_update = true;
                        }
                    }
                    j = j + 1;
                }
            }
            if (do_update) {
                if (attr == "calibrationParam") {
                    old_calib = "";
                    unit_name = "";
                    sensorType = "";
                    new_calib = newval;
                    j = 0;
                    found = false;
                    while ((j < old_jpath.Count) && !(found)) {
                        if ((new_jpath_len[i] == old_jpath_len[j]) && (new_jpath[i] == old_jpath[j])) {
                            found = true;
                            old_calib = old_val_arr[j];
                        }
                        j = j + 1;
                    }
                    tmp = fun + "/unit";
                    j = 0;
                    found = false;
                    while ((j < new_jpath.Count) && !(found)) {
                        if (tmp == new_jpath[j]) {
                            found = true;
                            unit_name = new_val_arr[j];
                        }
                        j = j + 1;
                    }
                    tmp = fun + "/sensorType";
                    j = 0;
                    found = false;
                    while ((j < new_jpath.Count) && !(found)) {
                        if (tmp == new_jpath[j]) {
                            found = true;
                            sensorType = new_val_arr[j];
                        }
                        j = j + 1;
                    }
                    newval = this.calibConvert(old_calib, new_val_arr[i], unit_name, sensorType);
                    url = "api/" + fun + ".json?" + attr + "=" + this._escapeAttr(newval);
                    subres = this._tryExec(url);
                    if ((res == YAPI.SUCCESS) && (subres != YAPI.SUCCESS)) {
                        res = subres;
                    }
                } else {
                    url = "api/" + fun + ".json?" + attr + "=" + this._escapeAttr(oldval);
                    if (attr == "resolution") {
                        restoreLast.Add(url);
                    } else {
                        subres = this._tryExec(url);
                        if ((res == YAPI.SUCCESS) && (subres != YAPI.SUCCESS)) {
                            res = subres;
                        }
                    }
                }
            }
            i = i + 1;
        }
        for (int ii = 0; ii < restoreLast.Count; ii++) {
            subres = this._tryExec(restoreLast[ii]);
            if ((res == YAPI.SUCCESS) && (subres != YAPI.SUCCESS)) {
                res = subres;
            }
        }
        this.clearCache();
        return res;
    }

    /**
     * <summary>
     *   Returns the unique hardware identifier of the module.
     * <para>
     *   The unique hardware identifier is made of the device serial
     *   number followed by string ".module".
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the module
     * </returns>
     */
    public override string get_hardwareId()
    {
        string serial;

        serial = this.get_serialNumber();
        return serial + ".module";
    }

    /**
     * <summary>
     *   Downloads the specified built-in file and returns a binary buffer with its content.
     * <para>
     * </para>
     * </summary>
     * <param name="pathname">
     *   name of the new file to load
     * </param>
     * <returns>
     *   a binary buffer with the file content
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
     * </para>
     */
    public virtual byte[] download(string pathname)
    {
        return this._download(pathname);
    }

    /**
     * <summary>
     *   Returns the icon of the module.
     * <para>
     *   The icon is a PNG image and does not
     *   exceeds 1536 bytes.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a binary buffer with module icon, in png format.
     *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
     * </returns>
     */
    public virtual byte[] get_icon2d()
    {
        return this._download("icon2d.png");
    }

    /**
     * <summary>
     *   Returns a string with last logs of the module.
     * <para>
     *   This method return only
     *   logs that are still in the module.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string with last logs of the module.
     *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
     * </returns>
     */
    public virtual string get_lastLogs()
    {
        byte[] content;

        content = this._download("logs.txt");
        return YAPI.DefaultEncoding.GetString(content);
    }

    /**
     * <summary>
     *   Adds a text message to the device logs.
     * <para>
     *   This function is useful in
     *   particular to trace the execution of HTTP callbacks. If a newline
     *   is desired after the message, it must be included in the string.
     * </para>
     * </summary>
     * <param name="text">
     *   the string to append to the logs.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int log(string text)
    {
        return this._upload("logs.txt", YAPI.DefaultEncoding.GetBytes(text));
    }

    /**
     * <summary>
     *   Returns a list of all the modules that are plugged into the current module.
     * <para>
     *   This method only makes sense when called for a YoctoHub/VirtualHub.
     *   Otherwise, an empty array will be returned.
     * </para>
     * </summary>
     * <returns>
     *   an array of strings containing the sub modules.
     * </returns>
     */
    public virtual List<string> get_subDevices()
    {
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        StringBuilder smallbuff = new StringBuilder(1024);
        StringBuilder bigbuff = null;
        int buffsize;
        int fullsize;
        int yapi_res;
        string subdevice_list;
        List<string> subdevices = new List<string>();
        string serial;

        serial = this.get_serialNumber();
        fullsize = 0;
        yapi_res = SafeNativeMethods._yapiGetSubdevices(new StringBuilder(serial), smallbuff, 1024, ref fullsize, errmsg);
        if (yapi_res < 0) {
            return subdevices;
        }
        if (fullsize <= 1024) {
            subdevice_list = smallbuff.ToString();
        } else {
            buffsize = fullsize;
            bigbuff = new StringBuilder(buffsize);
            yapi_res = SafeNativeMethods._yapiGetSubdevices(new StringBuilder(serial), bigbuff, buffsize, ref fullsize, errmsg);
            if (yapi_res < 0) {
                bigbuff = null;
                return subdevices;
            } else {
                subdevice_list = bigbuff.ToString();
            }
            bigbuff = null;
        }
        if (!(subdevice_list == "")) {
            subdevices = new List<string>(subdevice_list.Split(new Char[] {','}));
        }
        return subdevices;
    }

    /**
     * <summary>
     *   Returns the serial number of the YoctoHub on which this module is connected.
     * <para>
     *   If the module is connected by USB, or if the module is the root YoctoHub, an
     *   empty string is returned.
     * </para>
     * </summary>
     * <returns>
     *   a string with the serial number of the YoctoHub or an empty string
     * </returns>
     */
    public virtual string get_parentHub()
    {
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        StringBuilder hubserial = new StringBuilder(YAPI.YOCTO_SERIAL_LEN);
        int pathsize;
        int yapi_res;
        string serial;

        serial = this.get_serialNumber();
        // retrieve device object
        pathsize = 0;
        yapi_res = SafeNativeMethods._yapiGetDevicePathEx(new StringBuilder(serial), hubserial, null, 0, ref pathsize, errmsg);
        if (yapi_res < 0) {
            return "";
        }
        return hubserial.ToString();
    }

    /**
     * <summary>
     *   Returns the URL used to access the module.
     * <para>
     *   If the module is connected by USB, the
     *   string 'usb' is returned.
     * </para>
     * </summary>
     * <returns>
     *   a string with the URL of the module.
     * </returns>
     */
    public virtual string get_url()
    {
        StringBuilder errmsg = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        StringBuilder path = new StringBuilder(1024);
        int pathsize;
        int yapi_res;
        string serial;

        serial = this.get_serialNumber();
        // retrieve device object
        pathsize = 0;
        yapi_res = SafeNativeMethods._yapiGetDevicePathEx(new StringBuilder(serial), null, path, 1024, ref pathsize, errmsg);
        if (yapi_res < 0) {
            return "";
        }
        return path.ToString();
    }

    /**
     * <summary>
     *   Continues the module enumeration started using <c>yFirstModule()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned modules order.
     *   If you want to find a specific module, use <c>Module.findModule()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YModule</c> object, corresponding to
     *   the next module found, or a <c>null</c> pointer
     *   if there are no more modules to enumerate.
     * </returns>
     */
    public YModule nextModule()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindModule(hwid);
    }

    //--- (end of generated code: YModule implementation)
    /**
     * <summary>
     *   Returns a global identifier of the function in the format <c>MODULE_NAME&#46;FUNCTION_NAME</c>.
     * <para>
     *   The returned string uses the logical names of the module and of the function if they are defined,
     *   otherwise the serial number of the module and the hardware identifier of the function
     *   (for example: <c>MyCustomName.relay1</c>)
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the function using logical names
     *   (ex: <c>MyCustomName.relay1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YModule.FRIENDLYNAME_INVALID</c>.
     * </para>
     */

    public override string get_friendlyName()
    {
        YRETCODE retcode;
        YFUN_DESCR fundesc = 0;
        YDEV_DESCR devdesc = 0;
        string funcName = "";
        string dummy = "";
        string errmsg = "";
        string snum = "";
        string funcid = "";

        lock (_thisLock) {
            // Resolve the function name
            retcode = _getDescriptor(ref fundesc, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.FRIENDLYNAME_INVALID;
            }

            retcode = YAPI.yapiGetFunctionInfo(fundesc, ref devdesc, ref snum, ref funcid, ref funcName, ref dummy, ref errmsg);
            if (YAPI.YISERR(retcode)) {
                _throw(retcode, errmsg);
                return YAPI.FRIENDLYNAME_INVALID;
            }

            if (funcName != "") {
                return funcName;
            }

            return snum;
        }
    }

    internal void setImmutableAttributes(SafeNativeMethods.yDeviceSt infos)
    {
        _serialNumber = infos.serial;
        _productName = infos.productname;
        _productId = infos.deviceid;
        _cacheExpiration = YAPI.GetTickCount();
    }

    // Return the properties of the nth function of our device
    private YRETCODE _getFunction(int idx, ref string serial, ref string funcId, ref string baseType, ref string funcName, ref string funcVal, ref string errmsg)
    {
        YRETCODE functionReturnValue = default(YRETCODE);

        List<u32> functions = null;
        YAPI.YDevice dev = null;
        int res = 0;
        YFUN_DESCR fundescr = default(YFUN_DESCR);
        YDEV_DESCR devdescr = default(YDEV_DESCR);

        // retrieve device object
        res = _getDevice(ref dev, ref errmsg);
        if ((YAPI.YISERR(res))) {
            _throw(res, errmsg);
            functionReturnValue = res;
            return functionReturnValue;
        }


        // get reference to all functions from the device
        res = dev.getFunctions(ref functions, ref errmsg);
        if ((YAPI.YISERR(res))) {
            functionReturnValue = res;
            return functionReturnValue;
        }

        // get latest function info from yellow pages
        fundescr = Convert.ToInt32(functions[idx]);

        res = YAPI.yapiGetFunctionInfoEx(fundescr, ref devdescr, ref serial, ref funcId, ref baseType, ref funcName, ref funcVal, ref errmsg);
        if ((YAPI.YISERR(res))) {
            functionReturnValue = res;
            return functionReturnValue;
        }

        functionReturnValue = YAPI.SUCCESS;
        return functionReturnValue;
    }

    /**
     * <summary>
     *   Returns the number of functions (beside the "module" interface) available on the module.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the number of functions on the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int functionCount()
    {
        List<u32> functions = null;
        YAPI.YDevice dev = null;
        string errmsg = "";
        int res;
        lock (_thisLock) {
            res = _getDevice(ref dev, ref errmsg);
            if ((YAPI.YISERR(res))) {
                _throw(res, errmsg);
                return res;
            }

            res = dev.getFunctions(ref functions, ref errmsg);
            if ((YAPI.YISERR(res))) {
                functions = null;
                _throw(res, errmsg);
                return res;
            }

            return functions.Count;
        }
    }

    /**
     * <summary>
     *   Retrieves the hardware identifier of the <i>n</i>th function on the module.
     * <para>
     * </para>
     * </summary>
     * <param name="functionIndex">
     *   the index of the function for which the information is desired, starting at 0 for the first function.
     * </param>
     * <returns>
     *   a string corresponding to the unambiguous hardware identifier of the requested module function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public string functionId(int functionIndex)
    {
        string serial = "";
        string funcId = "";
        string baseType = "";
        string funcName = "";
        string funcVal = "";
        string errmsg = "";
        int res = 0;
        lock (_thisLock) {
            res = _getFunction(functionIndex, ref serial, ref funcId, ref baseType, ref funcName, ref funcVal, ref errmsg);
            if ((YAPI.YISERR(res))) {
                _throw(res, errmsg);
                return YAPI.INVALID_STRING;
            }

            return funcId;
        }
    }

    /**
     * <summary>
     *   Retrieves the type of the <i>n</i>th function on the module.
     * <para>
     * </para>
     * </summary>
     * <param name="functionIndex">
     *   the index of the function for which the information is desired, starting at 0 for the first function.
     * </param>
     * <returns>
     *   a string corresponding to the type of the function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public string functionType(int functionIndex)
    {
        string serial = "";
        string funcId = "";
        string baseType = "";
        string funcName = "";
        string funcVal = "";
        string errmsg = "";
        int res = 0;
        lock (_thisLock) {
            res = _getFunction(functionIndex, ref serial, ref funcId, ref baseType, ref funcName, ref funcVal, ref errmsg);
            if ((YAPI.YISERR(res))) {
                _throw(res, errmsg);
                return YAPI.INVALID_STRING;
            }

            char first = funcId[0];
            int i;
            for (i = funcId.Length; i > 0; i--) {
                if (Char.IsLetter(funcId[i - 1])) {
                    break;
                }
            }

            return Char.ToUpper(first) + funcId.Substring(1, i - 1);
        }
    }


    /**
     * <summary>
     *   Retrieves the base type of the <i>n</i>th function on the module.
     * <para>
     *   For instance, the base type of all measuring functions is "Sensor".
     * </para>
     * </summary>
     * <param name="functionIndex">
     *   the index of the function for which the information is desired, starting at 0 for the first function.
     * </param>
     * <returns>
     *   a string corresponding to the base type of the function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public string functionBaseType(int functionIndex)
    {
        string serial = "";
        string funcId = "";
        string baseType = "";
        string funcName = "";
        string funcVal = "";
        string errmsg = "";
        int res = 0;
        lock (_thisLock) {
            res = _getFunction(functionIndex, ref serial, ref funcId, ref baseType, ref funcName, ref funcVal, ref errmsg);
            if ((YAPI.YISERR(res))) {
                _throw(res, errmsg);
                return YAPI.INVALID_STRING;
            }

            return baseType;
        }
    }


    /**
     * <summary>
     *   Retrieves the logical name of the <i>n</i>th function on the module.
     * <para>
     * </para>
     * </summary>
     * <param name="functionIndex">
     *   the index of the function for which the information is desired, starting at 0 for the first function.
     * </param>
     * <returns>
     *   a string corresponding to the logical name of the requested module function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public string functionName(int functionIndex)
    {
        string serial = "";
        string funcId = "";
        string baseType = "";
        string funcName = "";
        string funcVal = "";
        string errmsg = "";
        int res = 0;
        lock (_thisLock) {
            res = _getFunction(functionIndex, ref serial, ref funcId, ref baseType, ref funcName, ref funcVal, ref errmsg);
            if ((YAPI.YISERR(res))) {
                _throw(res, errmsg);
                return YAPI.INVALID_STRING;
            }

            return funcName;
        }
    }

    /**
     * <summary>
     *   Retrieves the advertised value of the <i>n</i>th function on the module.
     * <para>
     * </para>
     * </summary>
     * <param name="functionIndex">
     *   the index of the function for which the information is desired, starting at 0 for the first function.
     * </param>
     * <returns>
     *   a short string (up to 6 characters) corresponding to the advertised value of the requested module function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public string functionValue(int functionIndex)
    {
        string serial = "";
        string funcId = "";
        string baseType = "";
        string funcName = "";
        string funcVal = "";
        string errmsg = "";
        int res = 0;
        lock (_thisLock) {
            res = _getFunction(functionIndex, ref serial, ref funcId, ref baseType, ref funcName, ref funcVal, ref errmsg);
            if ((YAPI.YISERR(res))) {
                _throw(res, errmsg);
                return YAPI.INVALID_STRING;
            }

            return funcVal;
        }
    }

    //--- (generated code: YModule functions)

    /**
     * <summary>
     *   Starts the enumeration of modules currently accessible.
     * <para>
     *   Use the method <c>YModule.nextModule()</c> to iterate on the
     *   next modules.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YModule</c> object, corresponding to
     *   the first module currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YModule FirstModule()
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
        err = YAPI.apiGetFunctionsByClass("Module", 0, p, size, ref neededsize, ref errmsg);
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
        return FindModule(serial + "." + funcId);
    }



    //--- (end of generated code: YModule functions)
}


//--- (generated code: YSensor class start)
/**
 * <summary>
 *   The YSensor class is the parent class for all Yoctopuce sensor types.
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
public class YSensor : YFunction
{
//--- (end of generated code: YSensor class start)
    //--- (generated code: YSensor definitions)
    public new delegate void ValueCallback(YSensor func, string value);
    public new delegate void TimedReportCallback(YSensor func, YMeasure measure);

    public const string UNIT_INVALID = YAPI.INVALID_STRING;
    public const double CURRENTVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const double LOWESTVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const double HIGHESTVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const double CURRENTRAWVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const string LOGFREQUENCY_INVALID = YAPI.INVALID_STRING;
    public const string REPORTFREQUENCY_INVALID = YAPI.INVALID_STRING;
    public const int ADVMODE_IMMEDIATE = 0;
    public const int ADVMODE_PERIOD_AVG = 1;
    public const int ADVMODE_PERIOD_MIN = 2;
    public const int ADVMODE_PERIOD_MAX = 3;
    public const int ADVMODE_INVALID = -1;
    public const string CALIBRATIONPARAM_INVALID = YAPI.INVALID_STRING;
    public const double RESOLUTION_INVALID = YAPI.INVALID_DOUBLE;
    public const int SENSORSTATE_INVALID = YAPI.INVALID_INT;
    protected string _unit = UNIT_INVALID;
    protected double _currentValue = CURRENTVALUE_INVALID;
    protected double _lowestValue = LOWESTVALUE_INVALID;
    protected double _highestValue = HIGHESTVALUE_INVALID;
    protected double _currentRawValue = CURRENTRAWVALUE_INVALID;
    protected string _logFrequency = LOGFREQUENCY_INVALID;
    protected string _reportFrequency = REPORTFREQUENCY_INVALID;
    protected int _advMode = ADVMODE_INVALID;
    protected string _calibrationParam = CALIBRATIONPARAM_INVALID;
    protected double _resolution = RESOLUTION_INVALID;
    protected int _sensorState = SENSORSTATE_INVALID;
    protected ValueCallback _valueCallbackSensor = null;
    protected TimedReportCallback _timedReportCallbackSensor = null;
    protected double _prevTimedReport = 0;
    protected double _iresol = 0;
    protected double _offset = 0;
    protected double _scale = 0;
    protected double _decexp = 0;
    protected int _caltyp = 0;
    protected List<int> _calpar = new List<int>();
    protected List<double> _calraw = new List<double>();
    protected List<double> _calref = new List<double>();
    protected YAPI.yCalibrationHandler _calhdl = null;
    //--- (end of generated code: YSensor definitions)

    public YSensor(string func) : base(func)
    {
        _className = "Sensor";
        //--- (generated code: YSensor attributes initialization)
        //--- (end of generated code: YSensor attributes initialization)
    }


    //--- (generated code: YSensor implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("unit"))
        {
            _unit = json_val.getString("unit");
        }
        if (json_val.has("currentValue"))
        {
            _currentValue = Math.Round(json_val.getDouble("currentValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("lowestValue"))
        {
            _lowestValue = Math.Round(json_val.getDouble("lowestValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("highestValue"))
        {
            _highestValue = Math.Round(json_val.getDouble("highestValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("currentRawValue"))
        {
            _currentRawValue = Math.Round(json_val.getDouble("currentRawValue") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("logFrequency"))
        {
            _logFrequency = json_val.getString("logFrequency");
        }
        if (json_val.has("reportFrequency"))
        {
            _reportFrequency = json_val.getString("reportFrequency");
        }
        if (json_val.has("advMode"))
        {
            _advMode = json_val.getInt("advMode");
        }
        if (json_val.has("calibrationParam"))
        {
            _calibrationParam = json_val.getString("calibrationParam");
        }
        if (json_val.has("resolution"))
        {
            _resolution = Math.Round(json_val.getDouble("resolution") * 1000.0 / 65536.0) / 1000.0;
        }
        if (json_val.has("sensorState"))
        {
            _sensorState = json_val.getInt("sensorState");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the measuring unit for the measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the measuring unit for the measure
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.UNIT_INVALID</c>.
     * </para>
     */
    public string get_unit()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return UNIT_INVALID;
                }
            }
            res = this._unit;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the current value of the measure, in the specified unit, as a floating point number.
     * <para>
     *   Note that a get_currentValue() call will *not* start a measure in the device, it
     *   will just return the last measure that occurred in the device. Indeed, internally, each Yoctopuce
     *   devices is continuously making measurements at a hardware specific frequency.
     * </para>
     * <para>
     *   If continuously calling  get_currentValue() leads you to performances issues, then
     *   you might consider to switch to callback programming model. Check the "advanced
     *   programming" chapter in in your device user manual for more information.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current value of the measure, in the specified unit,
     *   as a floating point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.CURRENTVALUE_INVALID</c>.
     * </para>
     */
    public double get_currentValue()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENTVALUE_INVALID;
                }
            }
            res = this._applyCalibration(this._currentRawValue);
            if (res == CURRENTVALUE_INVALID) {
                res = this._currentValue;
            }
            res = res * this._iresol;
            res = Math.Round(res) / this._iresol;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the recorded minimal value observed.
     * <para>
     *   Can be used to reset the value returned
     *   by get_lowestValue().
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the recorded minimal value observed
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
    public int set_lowestValue(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("lowestValue", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the minimal value observed for the measure since the device was started.
     * <para>
     *   Can be reset to an arbitrary value thanks to set_lowestValue().
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the minimal value observed for the measure since the device was started
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.LOWESTVALUE_INVALID</c>.
     * </para>
     */
    public double get_lowestValue()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LOWESTVALUE_INVALID;
                }
            }
            res = this._lowestValue * this._iresol;
            res = Math.Round(res) / this._iresol;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the recorded maximal value observed.
     * <para>
     *   Can be used to reset the value returned
     *   by get_lowestValue().
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the recorded maximal value observed
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
    public int set_highestValue(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("highestValue", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the maximal value observed for the measure since the device was started.
     * <para>
     *   Can be reset to an arbitrary value thanks to set_highestValue().
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the maximal value observed for the measure since the device was started
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.HIGHESTVALUE_INVALID</c>.
     * </para>
     */
    public double get_highestValue()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return HIGHESTVALUE_INVALID;
                }
            }
            res = this._highestValue * this._iresol;
            res = Math.Round(res) / this._iresol;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the uncalibrated, unrounded raw value returned by the
     *   sensor, in the specified unit, as a floating point number.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the uncalibrated, unrounded raw value returned by the
     *   sensor, in the specified unit, as a floating point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.CURRENTRAWVALUE_INVALID</c>.
     * </para>
     */
    public double get_currentRawValue()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CURRENTRAWVALUE_INVALID;
                }
            }
            res = this._currentRawValue;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the datalogger recording frequency for this function, or "OFF"
     *   when measures are not stored in the data logger flash memory.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the datalogger recording frequency for this function, or "OFF"
     *   when measures are not stored in the data logger flash memory
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.LOGFREQUENCY_INVALID</c>.
     * </para>
     */
    public string get_logFrequency()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return LOGFREQUENCY_INVALID;
                }
            }
            res = this._logFrequency;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the datalogger recording frequency for this function.
     * <para>
     *   The frequency can be specified as samples per second,
     *   as sample per minute (for instance "15/m") or in samples per
     *   hour (eg. "4/h"). To disable recording for this function, use
     *   the value "OFF". Note that setting the  datalogger recording frequency
     *   to a greater value than the sensor native sampling frequency is useless,
     *   and even counterproductive: those two frequencies are not related.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the datalogger recording frequency for this function
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
    public int set_logFrequency(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("logFrequency", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the timed value notification frequency, or "OFF" if timed
     *   value notifications are disabled for this function.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the timed value notification frequency, or "OFF" if timed
     *   value notifications are disabled for this function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.REPORTFREQUENCY_INVALID</c>.
     * </para>
     */
    public string get_reportFrequency()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return REPORTFREQUENCY_INVALID;
                }
            }
            res = this._reportFrequency;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the timed value notification frequency for this function.
     * <para>
     *   The frequency can be specified as samples per second,
     *   as sample per minute (for instance "15/m") or in samples per
     *   hour (e.g. "4/h"). To disable timed value notifications for this
     *   function, use the value "OFF". Note that setting the  timed value
     *   notification frequency to a greater value than the sensor native
     *   sampling frequency is unless, and even counterproductive: those two
     *   frequencies are not related.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the timed value notification frequency for this function
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
    public int set_reportFrequency(string newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = newval;
            return _setAttr("reportFrequency", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the measuring mode used for the advertised value pushed to the parent hub.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YSensor.ADVMODE_IMMEDIATE</c>, <c>YSensor.ADVMODE_PERIOD_AVG</c>,
     *   <c>YSensor.ADVMODE_PERIOD_MIN</c> and <c>YSensor.ADVMODE_PERIOD_MAX</c> corresponding to the
     *   measuring mode used for the advertised value pushed to the parent hub
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.ADVMODE_INVALID</c>.
     * </para>
     */
    public int get_advMode()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return ADVMODE_INVALID;
                }
            }
            res = this._advMode;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the measuring mode used for the advertised value pushed to the parent hub.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YSensor.ADVMODE_IMMEDIATE</c>, <c>YSensor.ADVMODE_PERIOD_AVG</c>,
     *   <c>YSensor.ADVMODE_PERIOD_MIN</c> and <c>YSensor.ADVMODE_PERIOD_MAX</c> corresponding to the
     *   measuring mode used for the advertised value pushed to the parent hub
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
    public int set_advMode(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("advMode", rest_val);
        }
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
     *   Changes the resolution of the measured physical values.
     * <para>
     *   The resolution corresponds to the numerical precision
     *   when displaying value. It does not change the precision of the measure itself.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the resolution of the measured physical values
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
    public int set_resolution(double newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = Math.Round(newval * 65536.0).ToString();
            return _setAttr("resolution", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the resolution of the measured values.
     * <para>
     *   The resolution corresponds to the numerical precision
     *   of the measures, which is not always the same as the actual precision of the sensor.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the resolution of the measured values
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.RESOLUTION_INVALID</c>.
     * </para>
     */
    public double get_resolution()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RESOLUTION_INVALID;
                }
            }
            res = this._resolution;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the sensor health state code, which is zero when there is an up-to-date measure
     *   available or a positive code if the sensor is not able to provide a measure right now.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the sensor health state code, which is zero when there is an up-to-date measure
     *   available or a positive code if the sensor is not able to provide a measure right now
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.SENSORSTATE_INVALID</c>.
     * </para>
     */
    public int get_sensorState()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SENSORSTATE_INVALID;
                }
            }
            res = this._sensorState;
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves a sensor for a given identifier.
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
     *   This function does not require that the sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YSensor.isOnline()</c> to test if the sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a sensor by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the sensor, for instance
     *   <c>MyDevice.</c>.
     * </param>
     * <returns>
     *   a <c>YSensor</c> object allowing you to drive the sensor.
     * </returns>
     */
    public static YSensor FindSensor(string func)
    {
        YSensor obj;
        lock (YAPI.globalLock) {
            obj = (YSensor) YFunction._FindFromCache("Sensor", func);
            if (obj == null) {
                obj = new YSensor(func);
                YFunction._AddToCache("Sensor", func, obj);
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
        this._valueCallbackSensor = callback;
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
        if (this._valueCallbackSensor != null) {
            this._valueCallbackSensor(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    public override int _parserHelper()
    {
        int position;
        int maxpos;
        List<int> iCalib = new List<int>();
        int iRaw;
        int iRef;
        double fRaw;
        double fRef;
        this._caltyp = -1;
        this._scale = -1;
        this._calpar.Clear();
        this._calraw.Clear();
        this._calref.Clear();
        // Store inverted resolution, to provide better rounding
        if (this._resolution > 0) {
            this._iresol = Math.Round(1.0 / this._resolution);
        } else {
            this._iresol = 10000;
            this._resolution = 0.0001;
        }
        // Old format: supported when there is no calibration
        if (this._calibrationParam == "" || this._calibrationParam == "0") {
            this._caltyp = 0;
            return 0;
        }
        if ((this._calibrationParam).IndexOf(",") >= 0) {
            // Plain text format
            iCalib = YAPI._decodeFloats(this._calibrationParam);
            this._caltyp = ((iCalib[0]) / (1000));
            if (this._caltyp > 0) {
                if (this._caltyp < YAPI.YOCTO_CALIB_TYPE_OFS) {
                    // Unknown calibration type: calibrated value will be provided by the device
                    this._caltyp = -1;
                    return 0;
                }
                this._calhdl = YAPI._getCalibrationHandler(this._caltyp);
                if (!(this._calhdl != null)) {
                    // Unknown calibration type: calibrated value will be provided by the device
                    this._caltyp = -1;
                    return 0;
                }
            }
            // New 32 bits text format
            this._offset = 0;
            this._scale = 1000;
            maxpos = iCalib.Count;
            this._calpar.Clear();
            position = 1;
            while (position < maxpos) {
                this._calpar.Add(iCalib[position]);
                position = position + 1;
            }
            this._calraw.Clear();
            this._calref.Clear();
            position = 1;
            while (position + 1 < maxpos) {
                fRaw = iCalib[position];
                fRaw = fRaw / 1000.0;
                fRef = iCalib[position + 1];
                fRef = fRef / 1000.0;
                this._calraw.Add(fRaw);
                this._calref.Add(fRef);
                position = position + 2;
            }
        } else {
            // Recorder-encoded format, including encoding
            iCalib = YAPI._decodeWords(this._calibrationParam);
            // In case of unknown format, calibrated value will be provided by the device
            if (iCalib.Count < 2) {
                this._caltyp = -1;
                return 0;
            }
            // Save variable format (scale for scalar, or decimal exponent)
            this._offset = 0;
            this._scale = 1;
            this._decexp = 1.0;
            position = iCalib[0];
            while (position > 0) {
                this._decexp = this._decexp * 10;
                position = position - 1;
            }
            // Shortcut when there is no calibration parameter
            if (iCalib.Count == 2) {
                this._caltyp = 0;
                return 0;
            }
            this._caltyp = iCalib[2];
            this._calhdl = YAPI._getCalibrationHandler(this._caltyp);
            // parse calibration points
            if (this._caltyp <= 10) {
                maxpos = this._caltyp;
            } else {
                if (this._caltyp <= 20) {
                    maxpos = this._caltyp - 10;
                } else {
                    maxpos = 5;
                }
            }
            maxpos = 3 + 2 * maxpos;
            if (maxpos > iCalib.Count) {
                maxpos = iCalib.Count;
            }
            this._calpar.Clear();
            this._calraw.Clear();
            this._calref.Clear();
            position = 3;
            while (position + 1 < maxpos) {
                iRaw = iCalib[position];
                iRef = iCalib[position + 1];
                this._calpar.Add(iRaw);
                this._calpar.Add(iRef);
                this._calraw.Add(YAPI._decimalToDouble(iRaw));
                this._calref.Add(YAPI._decimalToDouble(iRef));
                position = position + 2;
            }
        }
        return 0;
    }

    /**
     * <summary>
     *   Checks if the sensor is currently able to provide an up-to-date measure.
     * <para>
     *   Returns false if the device is unreachable, or if the sensor does not have
     *   a current measure to transmit. No exception is raised if there is an error
     *   while trying to contact the device hosting $THEFUNCTION$.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>true</c> if the sensor can provide an up-to-date measure, and <c>false</c> otherwise
     * </returns>
     */
    public virtual bool isSensorReady()
    {
        if (!(this.isOnline())) {
            return false;
        }
        if (!(this._sensorState == 0)) {
            return false;
        }
        return true;
    }

    /**
     * <summary>
     *   Returns the YDatalogger object of the device hosting the sensor.
     * <para>
     *   This method returns an object of
     *   class YDatalogger that can control global parameters of the data logger. The returned object
     *   should not be freed.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an YDataLogger object or null on error.
     * </returns>
     */
    public virtual YDataLogger get_dataLogger()
    {
        YDataLogger logger;
        YModule modu;
        string serial;
        string hwid;

        modu = this.get_module();
        serial = modu.get_serialNumber();
        if (serial == YAPI.INVALID_STRING) {
            return null;
        }
        hwid = serial + ".dataLogger";
        logger = YDataLogger.FindDataLogger(hwid);
        return logger;
    }

    /**
     * <summary>
     *   Starts the data logger on the device.
     * <para>
     *   Note that the data logger
     *   will only save the measures on this sensor if the logFrequency
     *   is not set to "OFF".
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     */
    public virtual int startDataLogger()
    {
        byte[] res;

        res = this._download("api/dataLogger/recording?recording=1");
        if (!((res).Length>0)) { this._throw( YAPI.IO_ERROR, "unable to start datalogger"); return YAPI.IO_ERROR; }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Stops the datalogger on the device.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     */
    public virtual int stopDataLogger()
    {
        byte[] res;

        res = this._download("api/dataLogger/recording?recording=0");
        if (!((res).Length>0)) { this._throw( YAPI.IO_ERROR, "unable to stop datalogger"); return YAPI.IO_ERROR; }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Retrieves a DataSet object holding historical data for this
     *   sensor, for a specified time interval.
     * <para>
     *   The measures will be
     *   retrieved from the data logger, which must have been turned
     *   on at the desired time. See the documentation of the DataSet
     *   class for information on how to get an overview of the
     *   recorded data, and how to load progressively a large set
     *   of measures from the data logger.
     * </para>
     * <para>
     *   This function only works if the device uses a recent firmware,
     *   as DataSet objects are not supported by firmwares older than
     *   version 13000.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="startTime">
     *   the start of the desired measure time interval,
     *   as a Unix timestamp, i.e. the number of seconds since
     *   January 1, 1970 UTC. The special value 0 can be used
     *   to include any measure, without initial limit.
     * </param>
     * <param name="endTime">
     *   the end of the desired measure time interval,
     *   as a Unix timestamp, i.e. the number of seconds since
     *   January 1, 1970 UTC. The special value 0 can be used
     *   to include any measure, without ending limit.
     * </param>
     * <returns>
     *   an instance of YDataSet, providing access to historical
     *   data. Past measures can be loaded progressively
     *   using methods from the YDataSet object.
     * </returns>
     */
    public virtual YDataSet get_recordedData(double startTime, double endTime)
    {
        string funcid;
        string funit;

        funcid = this.get_functionId();
        funit = this.get_unit();
        return new YDataSet(this, funcid, funit, startTime, endTime);
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
    public virtual int registerTimedReportCallback(TimedReportCallback callback)
    {
        YSensor sensor;
        sensor = this;
        if (callback != null) {
            YFunction._UpdateTimedReportCallbackList(sensor, true);
        } else {
            YFunction._UpdateTimedReportCallbackList(sensor, false);
        }
        this._timedReportCallbackSensor = callback;
        return 0;
    }

    public virtual int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackSensor != null) {
            this._timedReportCallbackSensor(this, value);
        } else {
        }
        return 0;
    }

    /**
     * <summary>
     *   Configures error correction data points, in particular to compensate for
     *   a possible perturbation of the measure caused by an enclosure.
     * <para>
     *   It is possible
     *   to configure up to five correction points. Correction points must be provided
     *   in ascending order, and be in the range of the sensor. The device will automatically
     *   perform a linear interpolation of the error correction between specified
     *   points. Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     *   For more information on advanced capabilities to refine the calibration of
     *   sensors, please contact support@yoctopuce.com.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="rawValues">
     *   array of floating point numbers, corresponding to the raw
     *   values returned by the sensor for the correction points.
     * </param>
     * <param name="refValues">
     *   array of floating point numbers, corresponding to the corrected
     *   values for the correction points.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int calibrateFromPoints(List<double> rawValues, List<double> refValues)
    {
        string rest_val;
        int res;

        lock (_thisLock) {
            rest_val = this._encodeCalibrationPoints(rawValues, refValues);
            res = this._setAttr("calibrationParam", rest_val);
        }
        return res;
    }

    /**
     * <summary>
     *   Retrieves error correction data points previously entered using the method
     *   <c>calibrateFromPoints</c>.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="rawValues">
     *   array of floating point numbers, that will be filled by the
     *   function with the raw sensor values for the correction points.
     * </param>
     * <param name="refValues">
     *   array of floating point numbers, that will be filled by the
     *   function with the desired values for the correction points.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int loadCalibrationPoints(List<double> rawValues, List<double> refValues)
    {
        rawValues.Clear();
        refValues.Clear();
        // Load function parameters if not yet loaded
        lock (_thisLock) {
            if (this._scale == 0) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return YAPI.DEVICE_NOT_FOUND;
                }
            }
            if (this._caltyp < 0) {
                this._throw(YAPI.NOT_SUPPORTED, "Calibration parameters format mismatch. Please upgrade your library or firmware.");
                return YAPI.NOT_SUPPORTED;
            }
            rawValues.Clear();
            refValues.Clear();
            for (int ii = 0; ii < this._calraw.Count; ii++) {
                rawValues.Add(this._calraw[ii]);
            }
            for (int ii = 0; ii < this._calref.Count; ii++) {
                refValues.Add(this._calref[ii]);
            }
        }
        return YAPI.SUCCESS;
    }

    public virtual string _encodeCalibrationPoints(List<double> rawValues, List<double> refValues)
    {
        string res;
        int npt;
        int idx;
        npt = rawValues.Count;
        if (npt != refValues.Count) {
            this._throw(YAPI.INVALID_ARGUMENT, "Invalid calibration parameters (size mismatch)");
            return YAPI.INVALID_STRING;
        }
        // Shortcut when building empty calibration parameters
        if (npt == 0) {
            return "0";
        }
        // Load function parameters if not yet loaded
        if (this._scale == 0) {
            if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                return YAPI.INVALID_STRING;
            }
        }
        // Detect old firmware
        if ((this._caltyp < 0) || (this._scale < 0)) {
            this._throw(YAPI.NOT_SUPPORTED, "Calibration parameters format mismatch. Please upgrade your library or firmware.");
            return "0";
        }
        // 32-bit fixed-point encoding
        res = ""+Convert.ToString(YAPI.YOCTO_CALIB_TYPE_OFS);
        idx = 0;
        while (idx < npt) {
            res = ""+ res+","+YAPI._floatToStr( rawValues[idx])+","+YAPI._floatToStr(refValues[idx]);
            idx = idx + 1;
        }
        return res;
    }

    public virtual double _applyCalibration(double rawValue)
    {
        if (rawValue == CURRENTVALUE_INVALID) {
            return CURRENTVALUE_INVALID;
        }
        if (this._caltyp == 0) {
            return rawValue;
        }
        if (this._caltyp < 0) {
            return CURRENTVALUE_INVALID;
        }
        if (!(this._calhdl != null)) {
            return CURRENTVALUE_INVALID;
        }
        return this._calhdl(rawValue, this._caltyp, this._calpar, this._calraw, this._calref);
    }

    public virtual YMeasure _decodeTimedReport(double timestamp, double duration, List<int> report)
    {
        int i;
        int byteVal;
        double poww;
        double minRaw;
        double avgRaw;
        double maxRaw;
        int sublen;
        double difRaw;
        double startTime;
        double endTime;
        double minVal;
        double avgVal;
        double maxVal;
        if (duration > 0) {
            startTime = timestamp - duration;
        } else {
            startTime = this._prevTimedReport;
        }
        endTime = timestamp;
        this._prevTimedReport = endTime;
        if (startTime == 0) {
            startTime = endTime;
        }
        // 32 bits timed report format
        if (report.Count <= 5) {
            // sub-second report, 1-4 bytes
            poww = 1;
            avgRaw = 0;
            byteVal = 0;
            i = 1;
            while (i < report.Count) {
                byteVal = report[i];
                avgRaw = avgRaw + poww * byteVal;
                poww = poww * 0x100;
                i = i + 1;
            }
            if (((byteVal) & (0x80)) != 0) {
                avgRaw = avgRaw - poww;
            }
            avgVal = avgRaw / 1000.0;
            if (this._caltyp != 0) {
                if (this._calhdl != null) {
                    avgVal = this._calhdl(avgVal, this._caltyp, this._calpar, this._calraw, this._calref);
                }
            }
            minVal = avgVal;
            maxVal = avgVal;
        } else {
            // averaged report: avg,avg-min,max-avg
            sublen = 1 + ((report[1]) & (3));
            poww = 1;
            avgRaw = 0;
            byteVal = 0;
            i = 2;
            while ((sublen > 0) && (i < report.Count)) {
                byteVal = report[i];
                avgRaw = avgRaw + poww * byteVal;
                poww = poww * 0x100;
                i = i + 1;
                sublen = sublen - 1;
            }
            if (((byteVal) & (0x80)) != 0) {
                avgRaw = avgRaw - poww;
            }
            sublen = 1 + ((((report[1]) >> (2))) & (3));
            poww = 1;
            difRaw = 0;
            while ((sublen > 0) && (i < report.Count)) {
                byteVal = report[i];
                difRaw = difRaw + poww * byteVal;
                poww = poww * 0x100;
                i = i + 1;
                sublen = sublen - 1;
            }
            minRaw = avgRaw - difRaw;
            sublen = 1 + ((((report[1]) >> (4))) & (3));
            poww = 1;
            difRaw = 0;
            while ((sublen > 0) && (i < report.Count)) {
                byteVal = report[i];
                difRaw = difRaw + poww * byteVal;
                poww = poww * 0x100;
                i = i + 1;
                sublen = sublen - 1;
            }
            maxRaw = avgRaw + difRaw;
            avgVal = avgRaw / 1000.0;
            minVal = minRaw / 1000.0;
            maxVal = maxRaw / 1000.0;
            if (this._caltyp != 0) {
                if (this._calhdl != null) {
                    avgVal = this._calhdl(avgVal, this._caltyp, this._calpar, this._calraw, this._calref);
                    minVal = this._calhdl(minVal, this._caltyp, this._calpar, this._calraw, this._calref);
                    maxVal = this._calhdl(maxVal, this._caltyp, this._calpar, this._calraw, this._calref);
                }
            }
        }
        return new YMeasure(startTime, endTime, minVal, avgVal, maxVal);
    }

    public double _decodeVal(int w)
    {
        double val;
        val = w;
        if (this._caltyp != 0) {
            if (this._calhdl != null) {
                val = this._calhdl(val, this._caltyp, this._calpar, this._calraw, this._calref);
            }
        }
        return val;
    }

    public double _decodeAvg(int dw)
    {
        double val;
        val = dw;
        if (this._caltyp != 0) {
            if (this._calhdl != null) {
                val = this._calhdl(val, this._caltyp, this._calpar, this._calraw, this._calref);
            }
        }
        return val;
    }

    /**
     * <summary>
     *   Continues the enumeration of sensors started using <c>yFirstSensor()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned sensors order.
     *   If you want to find a specific a sensor, use <c>Sensor.findSensor()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSensor</c> object, corresponding to
     *   a sensor currently online, or a <c>null</c> pointer
     *   if there are no more sensors to enumerate.
     * </returns>
     */
    public YSensor nextSensor()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindSensor(hwid);
    }

    //--- (end of generated code: YSensor implementation)

    //--- (generated code: YSensor functions)

    /**
     * <summary>
     *   Starts the enumeration of sensors currently accessible.
     * <para>
     *   Use the method <c>YSensor.nextSensor()</c> to iterate on
     *   next sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSensor</c> object, corresponding to
     *   the first sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YSensor FirstSensor()
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
        err = YAPI.apiGetFunctionsByClass("Sensor", 0, p, size, ref neededsize, ref errmsg);
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
        return FindSensor(serial + "." + funcId);
    }



    //--- (end of generated code: YSensor functions)
}


//--- (generated code: YDataLogger class start)
/**
 * <summary>
 *   A non-volatile memory for storing ongoing measured data is available on most Yoctopuce
 *   sensors, for instance using a Yocto-3D-V2, a Yocto-Light-V3, a Yocto-Meteo-V2 or a Yocto-Watt.
 * <para>
 *   Recording can happen automatically, without requiring a permanent
 *   connection to a computer.
 *   The YDataLogger class controls the global parameters of the internal data
 *   logger. Recording control (start/stop) as well as data retreival is done at
 *   sensor objects level.
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
    public const int USAGE_INVALID = YAPI.INVALID_UINT;
    public const int CLEARHISTORY_FALSE = 0;
    public const int CLEARHISTORY_TRUE = 1;
    public const int CLEARHISTORY_INVALID = -1;
    protected int _currentRunIndex = CURRENTRUNINDEX_INVALID;
    protected long _timeUTC = TIMEUTC_INVALID;
    protected int _recording = RECORDING_INVALID;
    protected int _autoStart = AUTOSTART_INVALID;
    protected int _beaconDriven = BEACONDRIVEN_INVALID;
    protected int _usage = USAGE_INVALID;
    protected int _clearHistory = CLEARHISTORY_INVALID;
    protected ValueCallback _valueCallbackDataLogger = null;
    //--- (end of generated code: YDataLogger definitions)
    protected string _dataLoggerURL = "";

    public YDataLogger(string func) : base(func)
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
        if (json_val.has("usage"))
        {
            _usage = json_val.getInt("usage");
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
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
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
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
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
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
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
     *   Changes the default activation state of the data logger on power up.
     * <para>
     *   Do not forget to call the <c>saveToFlash()</c> method of the module to save the
     *   configuration change.  Note: if the device doesn't have any time source at his disposal when
     *   starting up, it will wait for ~8 seconds before automatically starting to record  with
     *   an arbitrary timestamp
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
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
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

    /**
     * <summary>
     *   Returns the percentage of datalogger memory in use.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the percentage of datalogger memory in use
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDataLogger.USAGE_INVALID</c>.
     * </para>
     */
    public int get_usage()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return USAGE_INVALID;
                }
            }
            res = this._usage;
        }
        return res;
    }

    public int get_clearHistory()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
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
     * <para>
     *   If a call to this object's is_online() method returns FALSE although
     *   you are certain that the matching device is plugged, make sure that you did
     *   call registerHub() at application initialization time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the data logger, for instance
     *   <c>Y3DMK002.dataLogger</c>.
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
     *   Caution: You can't make any assumption about the returned data loggers order.
     *   If you want to find a specific a data logger, use <c>DataLogger.findDataLogger()</c>
     *   and a hardwareID or a logical name.
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
        if (YAPI.YISERR(res)) {
            _throw(res, errmsg);
            jsondata = null;
            return res;
        }

        if (timeIdx > 0) {
            query = "GET " + _dataLoggerURL + "?run=" + runIdx.ToString() + "&time=" + timeIdx.ToString() + " HTTP/1.1\r\n\r\n";
        } else {
            query = "GET " + _dataLoggerURL + " HTTP/1.1\r\n\r\n";
        }

        res = dev.HTTPRequest(query, out buffer, ref errmsg);
        if (YAPI.YISERR(res)) {
            res = YAPI.yapiUpdateDeviceList(1, ref errmsg);
            if (YAPI.YISERR(res)) {
                _throw(res, errmsg);
                jsondata = null;
                return res;
            }

            res = dev.HTTPRequest("GET " + _dataLoggerURL + " HTTP/1.1\r\n\r\n", out buffer, ref errmsg);
            if (YAPI.YISERR(res)) {
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


    //--- (generated code: YDataLogger functions)

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



    //--- (end of generated code: YDataLogger functions)
}

#pragma warning restore 1591