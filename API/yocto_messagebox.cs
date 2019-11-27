/*********************************************************************
 *
 * $Id: yocto_messagebox.cs 38510 2019-11-26 15:36:38Z mvuilleu $
 *
 * Implements yFindMessageBox(), the high-level API for MessageBox functions
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

    //--- (generated code: YSms return codes)
    //--- (end of generated code: YSms return codes)
    //--- (generated code: YMessageBox return codes)
    //--- (end of generated code: YMessageBox return codes)

//--- (generated code: YSms dlldef)
//--- (end of generated code: YSms dlldef)
//--- (generated code: YSms class start)
/**
 * <summary>
 *   YSms objects are used to describe a SMS.
 * <para>
 *   These objects are used in particular in conjunction with the YMessageBox class.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YSms
{
//--- (end of generated code: YSms class start)
    //--- (generated code: YSms definitions)

    protected YMessageBox _mbox;
    protected int _slot = 0;
    protected bool _deliv;
    protected string _smsc;
    protected int _mref = 0;
    protected string _orig;
    protected string _dest;
    protected int _pid = 0;
    protected int _alphab = 0;
    protected int _mclass = 0;
    protected string _stamp;
    protected byte[] _udh;
    protected byte[] _udata;
    protected int _npdu = 0;
    protected byte[] _pdu;
    protected List<YSms> _parts = new List<YSms>();
    protected string _aggSig;
    protected int _aggIdx = 0;
    protected int _aggCnt = 0;
    //--- (end of generated code: YSms definitions)

    public YSms(YMessageBox mbox)
    {
        //--- (generated code: YSms attributes initialization)
        //--- (end of generated code: YSms attributes initialization)
        _mbox = mbox;
    }

    //--- (generated code: YSms implementation)


    public virtual int get_slot()
    {
        return this._slot;
    }

    public virtual string get_smsc()
    {
        return this._smsc;
    }

    public virtual int get_msgRef()
    {
        return this._mref;
    }

    public virtual string get_sender()
    {
        return this._orig;
    }

    public virtual string get_recipient()
    {
        return this._dest;
    }

    public virtual int get_protocolId()
    {
        return this._pid;
    }

    public virtual bool isReceived()
    {
        return this._deliv;
    }

    public virtual int get_alphabet()
    {
        return this._alphab;
    }

    public virtual int get_msgClass()
    {
        if (((this._mclass) & (16)) == 0) {
            return -1;
        }
        return ((this._mclass) & (3));
    }

    public virtual int get_dcs()
    {
        return ((this._mclass) | ((((this._alphab) << (2)))));
    }

    public virtual string get_timestamp()
    {
        return this._stamp;
    }

    public virtual byte[] get_userDataHeader()
    {
        return this._udh;
    }

    public virtual byte[] get_userData()
    {
        return this._udata;
    }

    /**
     * <summary>
     *   Returns the content of the message.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string with the content of the message.
     * </returns>
     */
    public virtual string get_textData()
    {
        byte[] isolatin;
        int isosize;
        int i;
        if (this._alphab == 0) {
            // using GSM standard 7-bit alphabet
            return this._mbox.gsm2str(this._udata);
        }
        if (this._alphab == 2) {
            // using UCS-2 alphabet
            isosize = (((this._udata).Length) >> (1));
            isolatin = new byte[isosize];
            i = 0;
            while (i < isosize) {
                isolatin[i] = (byte)(this._udata[2*i+1] & 0xff);
                i = i + 1;
            }
            return YAPI.DefaultEncoding.GetString(isolatin);
        }
        // default: convert 8 bit to string as-is
        return YAPI.DefaultEncoding.GetString(this._udata);
    }

    public virtual List<int> get_unicodeData()
    {
        List<int> res = new List<int>();
        int unisize;
        int unival;
        int i;
        if (this._alphab == 0) {
            // using GSM standard 7-bit alphabet
            return this._mbox.gsm2unicode(this._udata);
        }
        if (this._alphab == 2) {
            // using UCS-2 alphabet
            unisize = (((this._udata).Length) >> (1));
            res.Clear();
            i = 0;
            while (i < unisize) {
                unival = 256*this._udata[2*i]+this._udata[2*i+1];
                res.Add(unival);
                i = i + 1;
            }
        } else {
            // return straight 8-bit values
            unisize = (this._udata).Length;
            res.Clear();
            i = 0;
            while (i < unisize) {
                res.Add(this._udata[i]+0);
                i = i + 1;
            }
        }
        return res;
    }

    public virtual int get_partCount()
    {
        if (this._npdu == 0) {
            this.generatePdu();
        }
        return this._npdu;
    }

    public virtual byte[] get_pdu()
    {
        if (this._npdu == 0) {
            this.generatePdu();
        }
        return this._pdu;
    }

    public virtual List<YSms> get_parts()
    {
        if (this._npdu == 0) {
            this.generatePdu();
        }
        return this._parts;
    }

    public virtual string get_concatSignature()
    {
        if (this._npdu == 0) {
            this.generatePdu();
        }
        return this._aggSig;
    }

    public virtual int get_concatIndex()
    {
        if (this._npdu == 0) {
            this.generatePdu();
        }
        return this._aggIdx;
    }

    public virtual int get_concatCount()
    {
        if (this._npdu == 0) {
            this.generatePdu();
        }
        return this._aggCnt;
    }

    public virtual int set_slot(int val)
    {
        this._slot = val;
        return YAPI.SUCCESS;
    }

    public virtual int set_received(bool val)
    {
        this._deliv = val;
        return YAPI.SUCCESS;
    }

    public virtual int set_smsc(string val)
    {
        this._smsc = val;
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_msgRef(int val)
    {
        this._mref = val;
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_sender(string val)
    {
        this._orig = val;
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_recipient(string val)
    {
        this._dest = val;
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_protocolId(int val)
    {
        this._pid = val;
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_alphabet(int val)
    {
        this._alphab = val;
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_msgClass(int val)
    {
        if (val == -1) {
            this._mclass = 0;
        } else {
            this._mclass = 16+val;
        }
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_dcs(int val)
    {
        this._alphab = (((((val) >> (2)))) & (3));
        this._mclass = ((val) & (16+3));
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_timestamp(string val)
    {
        this._stamp = val;
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int set_userDataHeader(byte[] val)
    {
        this._udh = val;
        this._npdu = 0;
        this.parseUserDataHeader();
        return YAPI.SUCCESS;
    }

    public virtual int set_userData(byte[] val)
    {
        this._udata = val;
        this._npdu = 0;
        return YAPI.SUCCESS;
    }

    public virtual int convertToUnicode()
    {
        List<int> ucs2 = new List<int>();
        int udatalen;
        int i;
        int uni;
        if (this._alphab == 2) {
            return YAPI.SUCCESS;
        }
        if (this._alphab == 0) {
            ucs2 = this._mbox.gsm2unicode(this._udata);
        } else {
            udatalen = (this._udata).Length;
            ucs2.Clear();
            i = 0;
            while (i < udatalen) {
                uni = this._udata[i];
                ucs2.Add(uni);
                i = i + 1;
            }
        }
        this._alphab = 2;
        this._udata = new byte[0];
        this.addUnicodeData(ucs2);
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Add a regular text to the SMS.
     * <para>
     *   This function support messages
     *   of more than 160 characters. ISO-latin accented characters
     *   are supported. For messages with special unicode characters such as asian
     *   characters and emoticons, use the  <c>addUnicodeData</c> method.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="val">
     *   the text to be sent in the message
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     */
    public virtual int addText(string val)
    {
        byte[] udata;
        int udatalen;
        byte[] newdata;
        int newdatalen;
        int i;
        if ((val).Length == 0) {
            return YAPI.SUCCESS;
        }
        if (this._alphab == 0) {
            // Try to append using GSM 7-bit alphabet
            newdata = this._mbox.str2gsm(val);
            newdatalen = (newdata).Length;
            if (newdatalen == 0) {
                // 7-bit not possible, switch to unicode
                this.convertToUnicode();
                newdata = YAPI.DefaultEncoding.GetBytes(val);
                newdatalen = (newdata).Length;
            }
        } else {
            newdata = YAPI.DefaultEncoding.GetBytes(val);
            newdatalen = (newdata).Length;
        }
        udatalen = (this._udata).Length;
        if (this._alphab == 2) {
            // Append in unicode directly
            udata = new byte[udatalen + 2*newdatalen];
            i = 0;
            while (i < udatalen) {
                udata[i] = (byte)(this._udata[i] & 0xff);
                i = i + 1;
            }
            i = 0;
            while (i < newdatalen) {
                udata[udatalen+1] = (byte)(newdata[i] & 0xff);
                udatalen = udatalen + 2;
                i = i + 1;
            }
        } else {
            // Append binary buffers
            udata = new byte[udatalen+newdatalen];
            i = 0;
            while (i < udatalen) {
                udata[i] = (byte)(this._udata[i] & 0xff);
                i = i + 1;
            }
            i = 0;
            while (i < newdatalen) {
                udata[udatalen] = (byte)(newdata[i] & 0xff);
                udatalen = udatalen + 1;
                i = i + 1;
            }
        }
        return this.set_userData(udata);
    }

    /**
     * <summary>
     *   Add a unicode text to the SMS.
     * <para>
     *   This function support messages
     *   of more than 160 characters, using SMS concatenation.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="val">
     *   an array of special unicode characters
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     */
    public virtual int addUnicodeData(List<int> val)
    {
        int arrlen;
        int newdatalen;
        int i;
        int uni;
        byte[] udata;
        int udatalen;
        int surrogate;
        if (this._alphab != 2) {
            this.convertToUnicode();
        }
        // compute number of 16-bit code units
        arrlen = val.Count;
        newdatalen = arrlen;
        i = 0;
        while (i < arrlen) {
            uni = val[i];
            if (uni > 65535) {
                newdatalen = newdatalen + 1;
            }
            i = i + 1;
        }
        // now build utf-16 buffer
        udatalen = (this._udata).Length;
        udata = new byte[udatalen+2*newdatalen];
        i = 0;
        while (i < udatalen) {
            udata[i] = (byte)(this._udata[i] & 0xff);
            i = i + 1;
        }
        i = 0;
        while (i < arrlen) {
            uni = val[i];
            if (uni >= 65536) {
                surrogate = uni - 65536;
                uni = (((((surrogate) >> (10))) & (1023))) + 55296;
                udata[udatalen] = (byte)(((uni) >> (8)) & 0xff);
                udata[udatalen+1] = (byte)(((uni) & (255)) & 0xff);
                udatalen = udatalen + 2;
                uni = (((surrogate) & (1023))) + 56320;
            }
            udata[udatalen] = (byte)(((uni) >> (8)) & 0xff);
            udata[udatalen+1] = (byte)(((uni) & (255)) & 0xff);
            udatalen = udatalen + 2;
            i = i + 1;
        }
        return this.set_userData(udata);
    }

    public virtual int set_pdu(byte[] pdu)
    {
        this._pdu = pdu;
        this._npdu = 1;
        return this.parsePdu(pdu);
    }

    public virtual int set_parts(List<YSms> parts)
    {
        List<YSms> sorted = new List<YSms>();
        int partno;
        int initpartno;
        int i;
        int retcode;
        int totsize;
        YSms subsms;
        byte[] subdata;
        byte[] res;
        this._npdu = parts.Count;
        if (this._npdu == 0) {
            return YAPI.INVALID_ARGUMENT;
        }
        sorted.Clear();
        partno = 0;
        while (partno < this._npdu) {
            initpartno = partno;
            i = 0;
            while (i < this._npdu) {
                subsms = parts[i];
                if (subsms.get_concatIndex() == partno) {
                    sorted.Add(subsms);
                    partno = partno + 1;
                }
                i = i + 1;
            }
            if (initpartno == partno) {
                partno = partno + 1;
            }
        }
        this._parts = sorted;
        this._npdu = sorted.Count;
        // inherit header fields from first part
        subsms = this._parts[0];
        retcode = this.parsePdu(subsms.get_pdu());
        if (retcode != YAPI.SUCCESS) {
            return retcode;
        }
        // concatenate user data from all parts
        totsize = 0;
        partno = 0;
        while (partno < this._parts.Count) {
            subsms = this._parts[partno];
            subdata = subsms.get_userData();
            totsize = totsize + (subdata).Length;
            partno = partno + 1;
        }
        res = new byte[totsize];
        totsize = 0;
        partno = 0;
        while (partno < this._parts.Count) {
            subsms = this._parts[partno];
            subdata = subsms.get_userData();
            i = 0;
            while (i < (subdata).Length) {
                res[totsize] = (byte)(subdata[i] & 0xff);
                totsize = totsize + 1;
                i = i + 1;
            }
            partno = partno + 1;
        }
        this._udata = res;
        return YAPI.SUCCESS;
    }

    public virtual byte[] encodeAddress(string addr)
    {
        byte[] bytes;
        int srclen;
        int numlen;
        int i;
        int val;
        int digit;
        byte[] res;
        bytes = YAPI.DefaultEncoding.GetBytes(addr);
        srclen = (bytes).Length;
        numlen = 0;
        i = 0;
        while (i < srclen) {
            val = bytes[i];
            if ((val >= 48) && (val < 58)) {
                numlen = numlen + 1;
            }
            i = i + 1;
        }
        if (numlen == 0) {
            res = new byte[1];
            res[0] = (byte)(0 & 0xff);
            return res;
        }
        res = new byte[2+((numlen+1) >> (1))];
        res[0] = (byte)(numlen & 0xff);
        if (bytes[0] == 43) {
            res[1] = (byte)(145 & 0xff);
        } else {
            res[1] = (byte)(129 & 0xff);
        }
        numlen = 4;
        digit = 0;
        i = 0;
        while (i < srclen) {
            val = bytes[i];
            if ((val >= 48) && (val < 58)) {
                if (((numlen) & (1)) == 0) {
                    digit = val - 48;
                } else {
                    res[((numlen) >> (1))] = (byte)(digit + 16*(val-48) & 0xff);
                }
                numlen = numlen + 1;
            }
            i = i + 1;
        }
        // pad with F if needed
        if (((numlen) & (1)) != 0) {
            res[((numlen) >> (1))] = (byte)(digit + 240 & 0xff);
        }
        return res;
    }

    public virtual string decodeAddress(byte[] addr, int ofs, int siz)
    {
        int addrType;
        byte[] gsm7;
        string res;
        int i;
        int rpos;
        int carry;
        int nbits;
        int byt;
        if (siz == 0) {
            return "";
        }
        res = "";
        addrType = ((addr[ofs]) & (112));
        if (addrType == 80) {
            // alphanumeric number
            siz = ((4*siz) / (7));
            gsm7 = new byte[siz];
            rpos = 1;
            carry = 0;
            nbits = 0;
            i = 0;
            while (i < siz) {
                if (nbits == 7) {
                    gsm7[i] = (byte)(carry & 0xff);
                    carry = 0;
                    nbits = 0;
                } else {
                    byt = addr[ofs+rpos];
                    rpos = rpos + 1;
                    gsm7[i] = (byte)(((carry) | ((((((byt) << (nbits)))) & (127)))) & 0xff);
                    carry = ((byt) >> ((7 - nbits)));
                    nbits = nbits + 1;
                }
                i = i + 1;
            }
            return this._mbox.gsm2str(gsm7);
        } else {
            // standard phone number
            if (addrType == 16) {
                res = "+";
            }
            siz = (((siz+1)) >> (1));
            i = 0;
            while (i < siz) {
                byt = addr[ofs+i+1];
                res = ""+ res+""+String.Format("{0:x}", ((byt) & (15)))+""+String.Format("{0:x}",((byt) >> (4)));
                i = i + 1;
            }
            // remove padding digit if needed
            if (((addr[ofs+siz]) >> (4)) == 15) {
                res = (res).Substring( 0, (res).Length-1);
            }
            return res;
        }
    }

    public virtual byte[] encodeTimeStamp(string exp)
    {
        int explen;
        int i;
        byte[] res;
        int n;
        byte[] expasc;
        int v1;
        int v2;
        explen = (exp).Length;
        if (explen == 0) {
            res = new byte[0];
            return res;
        }
        if ((exp).Substring(0, 1) == "+") {
            n = YAPI._atoi((exp).Substring(1, explen-1));
            res = new byte[1];
            if (n > 30*86400) {
                n = 192+(((n+6*86400)) / ((7*86400)));
            } else {
                if (n > 86400) {
                    n = 166+(((n+86399)) / (86400));
                } else {
                    if (n > 43200) {
                        n = 143+(((n-43200+1799)) / (1800));
                    } else {
                        n = -1+(((n+299)) / (300));
                    }
                }
            }
            if (n < 0) {
                n = 0;
            }
            res[0] = (byte)(n & 0xff);
            return res;
        }
        if ((exp).Substring(4, 1) == "-" || (exp).Substring(4, 1) == "/") {
            // ignore century
            exp = (exp).Substring( 2, explen-2);
            explen = (exp).Length;
        }
        expasc = YAPI.DefaultEncoding.GetBytes(exp);
        res = new byte[7];
        n = 0;
        i = 0;
        while ((i+1 < explen) && (n < 7)) {
            v1 = expasc[i];
            if ((v1 >= 48) && (v1 < 58)) {
                v2 = expasc[i+1];
                if ((v2 >= 48) && (v2 < 58)) {
                    v1 = v1 - 48;
                    v2 = v2 - 48;
                    res[n] = (byte)((((v2) << (4))) + v1 & 0xff);
                    n = n + 1;
                    i = i + 1;
                }
            }
            i = i + 1;
        }
        while (n < 7) {
            res[n] = (byte)(0 & 0xff);
            n = n + 1;
        }
        if (i+2 < explen) {
            // convert for timezone in cleartext ISO format +/-nn:nn
            v1 = expasc[i-3];
            v2 = expasc[i];
            if (((v1 == 43) || (v1 == 45)) && (v2 == 58)) {
                v1 = expasc[i+1];
                v2 = expasc[i+2];
                if ((v1 >= 48) && (v1 < 58) && (v1 >= 48) && (v1 < 58)) {
                    v1 = (((10*(v1 - 48)+(v2 - 48))) / (15));
                    n = n - 1;
                    v2 = 4 * res[n] + v1;
                    if (expasc[i-3] == 45) {
                        v2 += 128;
                    }
                    res[n] = (byte)(v2 & 0xff);
                }
            }
        }
        return res;
    }

    public virtual string decodeTimeStamp(byte[] exp, int ofs, int siz)
    {
        int n;
        string res;
        int i;
        int byt;
        string sign;
        string hh;
        string ss;
        if (siz < 1) {
            return "";
        }
        if (siz == 1) {
            n = exp[ofs];
            if (n < 144) {
                n = n * 300;
            } else {
                if (n < 168) {
                    n = (n-143) * 1800;
                } else {
                    if (n < 197) {
                        n = (n-166) * 86400;
                    } else {
                        n = (n-192) * 7 * 86400;
                    }
                }
            }
            return "+"+Convert.ToString(n);
        }
        res = "20";
        i = 0;
        while ((i < siz) && (i < 6)) {
            byt = exp[ofs+i];
            res = ""+ res+""+String.Format("{0:x}", ((byt) & (15)))+""+String.Format("{0:x}",((byt) >> (4)));
            if (i < 3) {
                if (i < 2) {
                    res = ""+res+"-";
                } else {
                    res = ""+res+" ";
                }
            } else {
                if (i < 5) {
                    res = ""+res+":";
                }
            }
            i = i + 1;
        }
        if (siz == 7) {
            byt = exp[ofs+i];
            sign = "+";
            if (((byt) & (8)) != 0) {
                byt = byt - 8;
                sign = "-";
            }
            byt = (10*(((byt) & (15)))) + (((byt) >> (4)));
            hh = ""+Convert.ToString(((byt) >> (2)));
            ss = ""+Convert.ToString(15*(((byt) & (3))));
            if ((hh).Length<2) {
                hh = "0"+hh;
            }
            if ((ss).Length<2) {
                ss = "0"+ss;
            }
            res = ""+ res+""+ sign+""+ hh+":"+ss;
        }
        return res;
    }

    public virtual int udataSize()
    {
        int res;
        int udhsize;
        udhsize = (this._udh).Length;
        res = (this._udata).Length;
        if (this._alphab == 0) {
            if (udhsize > 0) {
                res = res + (((8 + 8*udhsize + 6)) / (7));
            }
            res = (((res * 7 + 7)) / (8));
        } else {
            if (udhsize > 0) {
                res = res + 1 + udhsize;
            }
        }
        return res;
    }

    public virtual byte[] encodeUserData()
    {
        int udsize;
        int udlen;
        int udhsize;
        int udhlen;
        byte[] res;
        int i;
        int wpos;
        int carry;
        int nbits;
        int thi_b;
        // nbits = number of bits in carry
        udsize = this.udataSize();
        udhsize = (this._udh).Length;
        udlen = (this._udata).Length;
        res = new byte[1+udsize];
        udhlen = 0;
        nbits = 0;
        carry = 0;
        // 1. Encode UDL
        if (this._alphab == 0) {
            // 7-bit encoding
            if (udhsize > 0) {
                udhlen = (((8 + 8*udhsize + 6)) / (7));
                nbits = 7*udhlen - 8 - 8*udhsize;
            }
            res[0] = (byte)(udhlen+udlen & 0xff);
        } else {
            // 8-bit encoding
            res[0] = (byte)(udsize & 0xff);
        }
        // 2. Encode UDHL and UDL
        wpos = 1;
        if (udhsize > 0) {
            res[wpos] = (byte)(udhsize & 0xff);
            wpos = wpos + 1;
            i = 0;
            while (i < udhsize) {
                res[wpos] = (byte)(this._udh[i] & 0xff);
                wpos = wpos + 1;
                i = i + 1;
            }
        }
        // 3. Encode UD
        if (this._alphab == 0) {
            // 7-bit encoding
            i = 0;
            while (i < udlen) {
                if (nbits == 0) {
                    carry = this._udata[i];
                    nbits = 7;
                } else {
                    thi_b = this._udata[i];
                    res[wpos] = (byte)(((carry) | ((((((thi_b) << (nbits)))) & (255)))) & 0xff);
                    wpos = wpos + 1;
                    nbits = nbits - 1;
                    carry = ((thi_b) >> ((7 - nbits)));
                }
                i = i + 1;
            }
            if (nbits > 0) {
                res[wpos] = (byte)(carry & 0xff);
            }
        } else {
            // 8-bit encoding
            i = 0;
            while (i < udlen) {
                res[wpos] = (byte)(this._udata[i] & 0xff);
                wpos = wpos + 1;
                i = i + 1;
            }
        }
        return res;
    }

    public virtual int generateParts()
    {
        int udhsize;
        int udlen;
        int mss;
        int partno;
        int partlen;
        byte[] newud;
        byte[] newudh;
        YSms newpdu;
        int i;
        int wpos;
        udhsize = (this._udh).Length;
        udlen = (this._udata).Length;
        mss = 140 - 1 - 5 - udhsize;
        if (this._alphab == 0) {
            mss = (((mss * 8 - 6)) / (7));
        }
        this._npdu = (((udlen+mss-1)) / (mss));
        this._parts.Clear();
        partno = 0;
        wpos = 0;
        while (wpos < udlen) {
            partno = partno + 1;
            newudh = new byte[5+udhsize];
            newudh[0] = (byte)(0 & 0xff);           // IEI: concatenated message
            newudh[1] = (byte)(3 & 0xff);           // IEDL: 3 bytes
            newudh[2] = (byte)(this._mref & 0xff);
            newudh[3] = (byte)(this._npdu & 0xff);
            newudh[4] = (byte)(partno & 0xff);
            i = 0;
            while (i < udhsize) {
                newudh[5+i] = (byte)(this._udh[i] & 0xff);
                i = i + 1;
            }
            if (wpos+mss < udlen) {
                partlen = mss;
            } else {
                partlen = udlen-wpos;
            }
            newud = new byte[partlen];
            i = 0;
            while (i < partlen) {
                newud[i] = (byte)(this._udata[wpos] & 0xff);
                wpos = wpos + 1;
                i = i + 1;
            }
            newpdu = new YSms(this._mbox);
            newpdu.set_received(this.isReceived());
            newpdu.set_smsc(this.get_smsc());
            newpdu.set_msgRef(this.get_msgRef());
            newpdu.set_sender(this.get_sender());
            newpdu.set_recipient(this.get_recipient());
            newpdu.set_protocolId(this.get_protocolId());
            newpdu.set_dcs(this.get_dcs());
            newpdu.set_timestamp(this.get_timestamp());
            newpdu.set_userDataHeader(newudh);
            newpdu.set_userData(newud);
            this._parts.Add(newpdu);
        }
        return YAPI.SUCCESS;
    }

    public virtual int generatePdu()
    {
        byte[] sca;
        byte[] hdr;
        byte[] addr;
        byte[] stamp;
        byte[] udata;
        int pdutyp;
        int pdulen;
        int i;
        // Determine if the message can fit within a single PDU
        this._parts.Clear();
        if (this.udataSize() > 140) {
            // multiple PDU are needed
            this._pdu = new byte[0];
            return this.generateParts();
        }
        sca = this.encodeAddress(this._smsc);
        if ((sca).Length > 0) {
            sca[0] = (byte)((sca).Length-1 & 0xff);
        }
        stamp = this.encodeTimeStamp(this._stamp);
        udata = this.encodeUserData();
        if (this._deliv) {
            addr = this.encodeAddress(this._orig);
            hdr = new byte[1];
            pdutyp = 0;
        } else {
            addr = this.encodeAddress(this._dest);
            this._mref = this._mbox.nextMsgRef();
            hdr = new byte[2];
            hdr[1] = (byte)(this._mref & 0xff);
            pdutyp = 1;
            if ((stamp).Length > 0) {
                pdutyp = pdutyp + 16;
            }
            if ((stamp).Length == 7) {
                pdutyp = pdutyp + 8;
            }
        }
        if ((this._udh).Length > 0) {
            pdutyp = pdutyp + 64;
        }
        hdr[0] = (byte)(pdutyp & 0xff);
        pdulen = (sca).Length+(hdr).Length+(addr).Length+2+(stamp).Length+(udata).Length;
        this._pdu = new byte[pdulen];
        pdulen = 0;
        i = 0;
        while (i < (sca).Length) {
            this._pdu[pdulen] = (byte)(sca[i] & 0xff);
            pdulen = pdulen + 1;
            i = i + 1;
        }
        i = 0;
        while (i < (hdr).Length) {
            this._pdu[pdulen] = (byte)(hdr[i] & 0xff);
            pdulen = pdulen + 1;
            i = i + 1;
        }
        i = 0;
        while (i < (addr).Length) {
            this._pdu[pdulen] = (byte)(addr[i] & 0xff);
            pdulen = pdulen + 1;
            i = i + 1;
        }
        this._pdu[pdulen] = (byte)(this._pid & 0xff);
        pdulen = pdulen + 1;
        this._pdu[pdulen] = (byte)(this.get_dcs() & 0xff);
        pdulen = pdulen + 1;
        i = 0;
        while (i < (stamp).Length) {
            this._pdu[pdulen] = (byte)(stamp[i] & 0xff);
            pdulen = pdulen + 1;
            i = i + 1;
        }
        i = 0;
        while (i < (udata).Length) {
            this._pdu[pdulen] = (byte)(udata[i] & 0xff);
            pdulen = pdulen + 1;
            i = i + 1;
        }
        this._npdu = 1;
        return YAPI.SUCCESS;
    }

    public virtual int parseUserDataHeader()
    {
        int udhlen;
        int i;
        int iei;
        int ielen;
        string sig;
        this._aggSig = "";
        this._aggIdx = 0;
        this._aggCnt = 0;
        udhlen = (this._udh).Length;
        i = 0;
        while (i+1 < udhlen) {
            iei = this._udh[i];
            ielen = this._udh[i+1];
            i = i + 2;
            if (i + ielen <= udhlen) {
                if ((iei == 0) && (ielen == 3)) {
                    // concatenated SMS, 8-bit ref
                    sig = ""+ this._orig+"-"+ this._dest+"-"+String.Format("{0:x02}",
                    this._mref)+"-"+String.Format("{0:x02}",this._udh[i]);
                    this._aggSig = sig;
                    this._aggCnt = this._udh[i+1];
                    this._aggIdx = this._udh[i+2];
                }
                if ((iei == 8) && (ielen == 4)) {
                    // concatenated SMS, 16-bit ref
                    sig = ""+ this._orig+"-"+ this._dest+"-"+String.Format("{0:x02}",
                    this._mref)+"-"+String.Format("{0:x02}", this._udh[i])+""+String.Format("{0:x02}",this._udh[i+1]);
                    this._aggSig = sig;
                    this._aggCnt = this._udh[i+2];
                    this._aggIdx = this._udh[i+3];
                }
            }
            i = i + ielen;
        }
        return YAPI.SUCCESS;
    }

    public virtual int parsePdu(byte[] pdu)
    {
        int rpos;
        int addrlen;
        int pdutyp;
        int tslen;
        int dcs;
        int udlen;
        int udhsize;
        int udhlen;
        int i;
        int carry;
        int nbits;
        int thi_b;
        this._pdu = pdu;
        this._npdu = 1;
        // parse meta-data
        this._smsc = this.decodeAddress(pdu, 1, 2*(pdu[0]-1));
        rpos = 1+pdu[0];
        pdutyp = pdu[rpos];
        rpos = rpos + 1;
        this._deliv = (((pdutyp) & (3)) == 0);
        if (this._deliv) {
            addrlen = pdu[rpos];
            rpos = rpos + 1;
            this._orig = this.decodeAddress(pdu, rpos, addrlen);
            this._dest = "";
            tslen = 7;
        } else {
            this._mref = pdu[rpos];
            rpos = rpos + 1;
            addrlen = pdu[rpos];
            rpos = rpos + 1;
            this._dest = this.decodeAddress(pdu, rpos, addrlen);
            this._orig = "";
            if ((((pdutyp) & (16))) != 0) {
                if ((((pdutyp) & (8))) != 0) {
                    tslen = 7;
                } else {
                    tslen= 1;
                }
            } else {
                tslen = 0;
            }
        }
        rpos = rpos + ((((addrlen+3)) >> (1)));
        this._pid = pdu[rpos];
        rpos = rpos + 1;
        dcs = pdu[rpos];
        rpos = rpos + 1;
        this._alphab = (((((dcs) >> (2)))) & (3));
        this._mclass = ((dcs) & (16+3));
        this._stamp = this.decodeTimeStamp(pdu, rpos, tslen);
        rpos = rpos + tslen;
        // parse user data (including udh)
        nbits = 0;
        carry = 0;
        udlen = pdu[rpos];
        rpos = rpos + 1;
        if (((pdutyp) & (64)) != 0) {
            udhsize = pdu[rpos];
            rpos = rpos + 1;
            this._udh = new byte[udhsize];
            i = 0;
            while (i < udhsize) {
                this._udh[i] = (byte)(pdu[rpos] & 0xff);
                rpos = rpos + 1;
                i = i + 1;
            }
            if (this._alphab == 0) {
                // 7-bit encoding
                udhlen = (((8 + 8*udhsize + 6)) / (7));
                nbits = 7*udhlen - 8 - 8*udhsize;
                if (nbits > 0) {
                    thi_b = pdu[rpos];
                    rpos = rpos + 1;
                    carry = ((thi_b) >> (nbits));
                    nbits = 8 - nbits;
                }
            } else {
                // byte encoding
                udhlen = 1+udhsize;
            }
            udlen = udlen - udhlen;
        } else {
            udhsize = 0;
            this._udh = new byte[0];
        }
        this._udata = new byte[udlen];
        if (this._alphab == 0) {
            // 7-bit encoding
            i = 0;
            while (i < udlen) {
                if (nbits == 7) {
                    this._udata[i] = (byte)(carry & 0xff);
                    carry = 0;
                    nbits = 0;
                } else {
                    thi_b = pdu[rpos];
                    rpos = rpos + 1;
                    this._udata[i] = (byte)(((carry) | ((((((thi_b) << (nbits)))) & (127)))) & 0xff);
                    carry = ((thi_b) >> ((7 - nbits)));
                    nbits = nbits + 1;
                }
                i = i + 1;
            }
        } else {
            // 8-bit encoding
            i = 0;
            while (i < udlen) {
                this._udata[i] = (byte)(pdu[rpos] & 0xff);
                rpos = rpos + 1;
                i = i + 1;
            }
        }
        this.parseUserDataHeader();
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Sends the SMS to the recipient.
     * <para>
     *   Messages of more than 160 characters are supported
     *   using SMS concatenation.
     * </para>
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
    public virtual int send()
    {
        int i;
        int retcode;
        YSms pdu;

        if (this._npdu == 0) {
            this.generatePdu();
        }
        if (this._npdu == 1) {
            return this._mbox._upload("sendSMS", this._pdu);
        }
        retcode = YAPI.SUCCESS;
        i = 0;
        while ((i < this._npdu) && (retcode == YAPI.SUCCESS)) {
            pdu = this._parts[i];
            retcode= pdu.send();
            i = i + 1;
        }
        return retcode;
    }

    public virtual int deleteFromSIM()
    {
        int i;
        int retcode;
        YSms pdu;

        if (this._slot > 0) {
            return this._mbox.clearSIMSlot(this._slot);
        }
        retcode = YAPI.SUCCESS;
        i = 0;
        while ((i < this._npdu) && (retcode == YAPI.SUCCESS)) {
            pdu = this._parts[i];
            retcode= pdu.deleteFromSIM();
            i = i + 1;
        }
        return retcode;
    }

    //--- (end of generated code: YSms implementation)

    //--- (generated code: YSms functions)



    //--- (end of generated code: YSms functions)
}

//--- (generated code: YMessageBox dlldef)
//--- (end of generated code: YMessageBox dlldef)
//--- (generated code: YMessageBox class start)
/**
 * <summary>
 *   The YMessageBox class provides SMS sending and receiving capability to
 *   GSM-enabled Yoctopuce devices, for instance using a YoctoHub-GSM-2G, a YoctoHub-GSM-3G-EU or a YoctoHub-GSM-3G-NA.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YMessageBox : YFunction
{
//--- (end of generated code: YMessageBox class start)
    //--- (generated code: YMessageBox definitions)
    public new delegate void ValueCallback(YMessageBox func, string value);
    public new delegate void TimedReportCallback(YMessageBox func, YMeasure measure);

    public const int SLOTSINUSE_INVALID = YAPI.INVALID_UINT;
    public const int SLOTSCOUNT_INVALID = YAPI.INVALID_UINT;
    public const string SLOTSBITMAP_INVALID = YAPI.INVALID_STRING;
    public const int PDUSENT_INVALID = YAPI.INVALID_UINT;
    public const int PDURECEIVED_INVALID = YAPI.INVALID_UINT;
    public const string COMMAND_INVALID = YAPI.INVALID_STRING;
    protected int _slotsInUse = SLOTSINUSE_INVALID;
    protected int _slotsCount = SLOTSCOUNT_INVALID;
    protected string _slotsBitmap = SLOTSBITMAP_INVALID;
    protected int _pduSent = PDUSENT_INVALID;
    protected int _pduReceived = PDURECEIVED_INVALID;
    protected string _command = COMMAND_INVALID;
    protected ValueCallback _valueCallbackMessageBox = null;
    protected int _nextMsgRef = 0;
    protected string _prevBitmapStr;
    protected List<YSms> _pdus = new List<YSms>();
    protected List<YSms> _messages = new List<YSms>();
    protected bool _gsm2unicodeReady;
    protected List<int> _gsm2unicode = new List<int>();
    protected byte[] _iso2gsm;
    //--- (end of generated code: YMessageBox definitions)

    public YMessageBox(string func)
        : base(func)
    {
        _className = "MessageBox";
        //--- (generated code: YMessageBox attributes initialization)
        //--- (end of generated code: YMessageBox attributes initialization)
    }

    //--- (generated code: YMessageBox implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("slotsInUse"))
        {
            _slotsInUse = json_val.getInt("slotsInUse");
        }
        if (json_val.has("slotsCount"))
        {
            _slotsCount = json_val.getInt("slotsCount");
        }
        if (json_val.has("slotsBitmap"))
        {
            _slotsBitmap = json_val.getString("slotsBitmap");
        }
        if (json_val.has("pduSent"))
        {
            _pduSent = json_val.getInt("pduSent");
        }
        if (json_val.has("pduReceived"))
        {
            _pduReceived = json_val.getInt("pduReceived");
        }
        if (json_val.has("command"))
        {
            _command = json_val.getString("command");
        }
        base._parseAttr(json_val);
    }

    /**
     * <summary>
     *   Returns the number of message storage slots currently in use.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of message storage slots currently in use
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMessageBox.SLOTSINUSE_INVALID</c>.
     * </para>
     */
    public int get_slotsInUse()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SLOTSINUSE_INVALID;
                }
            }
            res = this._slotsInUse;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the total number of message storage slots on the SIM card.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the total number of message storage slots on the SIM card
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMessageBox.SLOTSCOUNT_INVALID</c>.
     * </para>
     */
    public int get_slotsCount()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SLOTSCOUNT_INVALID;
                }
            }
            res = this._slotsCount;
        }
        return res;
    }

    public string get_slotsBitmap()
    {
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return SLOTSBITMAP_INVALID;
                }
            }
            res = this._slotsBitmap;
        }
        return res;
    }

    /**
     * <summary>
     *   Returns the number of SMS units sent so far.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of SMS units sent so far
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMessageBox.PDUSENT_INVALID</c>.
     * </para>
     */
    public int get_pduSent()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PDUSENT_INVALID;
                }
            }
            res = this._pduSent;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the value of the outgoing SMS units counter.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the value of the outgoing SMS units counter
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
    public int set_pduSent(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("pduSent", rest_val);
        }
    }

    /**
     * <summary>
     *   Returns the number of SMS units received so far.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of SMS units received so far
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YMessageBox.PDURECEIVED_INVALID</c>.
     * </para>
     */
    public int get_pduReceived()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PDURECEIVED_INVALID;
                }
            }
            res = this._pduReceived;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the value of the incoming SMS units counter.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the value of the incoming SMS units counter
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
    public int set_pduReceived(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("pduReceived", rest_val);
        }
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
     *   Retrieves a MessageBox interface for a given identifier.
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
     *   This function does not require that the MessageBox interface is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YMessageBox.isOnline()</c> to test if the MessageBox interface is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a MessageBox interface by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the MessageBox interface, for instance
     *   <c>YHUBGSM1.messageBox</c>.
     * </param>
     * <returns>
     *   a <c>YMessageBox</c> object allowing you to drive the MessageBox interface.
     * </returns>
     */
    public static YMessageBox FindMessageBox(string func)
    {
        YMessageBox obj;
        lock (YAPI.globalLock) {
            obj = (YMessageBox) YFunction._FindFromCache("MessageBox", func);
            if (obj == null) {
                obj = new YMessageBox(func);
                YFunction._AddToCache("MessageBox", func, obj);
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
        this._valueCallbackMessageBox = callback;
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
        if (this._valueCallbackMessageBox != null) {
            this._valueCallbackMessageBox(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    public virtual int nextMsgRef()
    {
        this._nextMsgRef = this._nextMsgRef + 1;
        return this._nextMsgRef;
    }

    public virtual int clearSIMSlot(int slot)
    {
        this._prevBitmapStr = "";
        return this.set_command("DS"+Convert.ToString(slot));
    }

    public virtual YSms fetchPdu(int slot)
    {
        byte[] binPdu;
        List<string> arrPdu = new List<string>();
        string hexPdu;
        YSms sms;

        binPdu = this._download("sms.json?pos="+Convert.ToString(slot)+"&len=1");
        arrPdu = this._json_get_array(binPdu);
        hexPdu = this._decode_json_string(arrPdu[0]);
        sms = new YSms(this);
        sms.set_slot(slot);
        sms.parsePdu(YAPI._hexStrToBin(hexPdu));
        return sms;
    }

    public virtual int initGsm2Unicode()
    {
        int i;
        int uni;
        this._gsm2unicode.Clear();
        // 00-07
        this._gsm2unicode.Add(64);
        this._gsm2unicode.Add(163);
        this._gsm2unicode.Add(36);
        this._gsm2unicode.Add(165);
        this._gsm2unicode.Add(232);
        this._gsm2unicode.Add(233);
        this._gsm2unicode.Add(249);
        this._gsm2unicode.Add(236);
        // 08-0F
        this._gsm2unicode.Add(242);
        this._gsm2unicode.Add(199);
        this._gsm2unicode.Add(10);
        this._gsm2unicode.Add(216);
        this._gsm2unicode.Add(248);
        this._gsm2unicode.Add(13);
        this._gsm2unicode.Add(197);
        this._gsm2unicode.Add(229);
        // 10-17
        this._gsm2unicode.Add(916);
        this._gsm2unicode.Add(95);
        this._gsm2unicode.Add(934);
        this._gsm2unicode.Add(915);
        this._gsm2unicode.Add(923);
        this._gsm2unicode.Add(937);
        this._gsm2unicode.Add(928);
        this._gsm2unicode.Add(936);
        // 18-1F
        this._gsm2unicode.Add(931);
        this._gsm2unicode.Add(920);
        this._gsm2unicode.Add(926);
        this._gsm2unicode.Add(27);
        this._gsm2unicode.Add(198);
        this._gsm2unicode.Add(230);
        this._gsm2unicode.Add(223);
        this._gsm2unicode.Add(201);
        // 20-7A
        i = 32;
        while (i <= 122) {
            this._gsm2unicode.Add(i);
            i = i + 1;
        }
        // exceptions in range 20-7A
        this._gsm2unicode[36] = 164;
        this._gsm2unicode[64] = 161;
        this._gsm2unicode[91] = 196;
        this._gsm2unicode[92] = 214;
        this._gsm2unicode[93] = 209;
        this._gsm2unicode[94] = 220;
        this._gsm2unicode[95] = 167;
        this._gsm2unicode[96] = 191;
        // 7B-7F
        this._gsm2unicode.Add(228);
        this._gsm2unicode.Add(246);
        this._gsm2unicode.Add(241);
        this._gsm2unicode.Add(252);
        this._gsm2unicode.Add(224);
        // Invert table as well wherever possible
        this._iso2gsm = new byte[256];
        i = 0;
        while (i <= 127) {
            uni = this._gsm2unicode[i];
            if (uni <= 255) {
                this._iso2gsm[uni] = (byte)(i & 0xff);
            }
            i = i + 1;
        }
        i = 0;
        while (i < 4) {
            // mark escape sequences
            this._iso2gsm[91+i] = (byte)(27 & 0xff);
            this._iso2gsm[123+i] = (byte)(27 & 0xff);
            i = i + 1;
        }
        // Done
        this._gsm2unicodeReady = true;
        return YAPI.SUCCESS;
    }

    public virtual List<int> gsm2unicode(byte[] gsm)
    {
        int i;
        int gsmlen;
        int reslen;
        List<int> res = new List<int>();
        int uni;
        if (!(this._gsm2unicodeReady)) {
            this.initGsm2Unicode();
        }
        gsmlen = (gsm).Length;
        reslen = gsmlen;
        i = 0;
        while (i < gsmlen) {
            if (gsm[i] == 27) {
                reslen = reslen - 1;
            }
            i = i + 1;
        }
        res.Clear();
        i = 0;
        while (i < gsmlen) {
            uni = this._gsm2unicode[gsm[i]];
            if ((uni == 27) && (i+1 < gsmlen)) {
                i = i + 1;
                uni = gsm[i];
                if (uni < 60) {
                    if (uni < 41) {
                        if (uni==20) {
                            uni=94;
                        } else {
                            if (uni==40) {
                                uni=123;
                            } else {
                                uni=0;
                            }
                        }
                    } else {
                        if (uni==41) {
                            uni=125;
                        } else {
                            if (uni==47) {
                                uni=92;
                            } else {
                                uni=0;
                            }
                        }
                    }
                } else {
                    if (uni < 62) {
                        if (uni==60) {
                            uni=91;
                        } else {
                            if (uni==61) {
                                uni=126;
                            } else {
                                uni=0;
                            }
                        }
                    } else {
                        if (uni==62) {
                            uni=93;
                        } else {
                            if (uni==64) {
                                uni=124;
                            } else {
                                if (uni==101) {
                                    uni=164;
                                } else {
                                    uni=0;
                                }
                            }
                        }
                    }
                }
            }
            if (uni > 0) {
                res.Add(uni);
            }
            i = i + 1;
        }
        return res;
    }

    public virtual string gsm2str(byte[] gsm)
    {
        int i;
        int gsmlen;
        int reslen;
        byte[] resbin;
        string resstr;
        int uni;
        if (!(this._gsm2unicodeReady)) {
            this.initGsm2Unicode();
        }
        gsmlen = (gsm).Length;
        reslen = gsmlen;
        i = 0;
        while (i < gsmlen) {
            if (gsm[i] == 27) {
                reslen = reslen - 1;
            }
            i = i + 1;
        }
        resbin = new byte[reslen];
        i = 0;
        reslen = 0;
        while (i < gsmlen) {
            uni = this._gsm2unicode[gsm[i]];
            if ((uni == 27) && (i+1 < gsmlen)) {
                i = i + 1;
                uni = gsm[i];
                if (uni < 60) {
                    if (uni < 41) {
                        if (uni==20) {
                            uni=94;
                        } else {
                            if (uni==40) {
                                uni=123;
                            } else {
                                uni=0;
                            }
                        }
                    } else {
                        if (uni==41) {
                            uni=125;
                        } else {
                            if (uni==47) {
                                uni=92;
                            } else {
                                uni=0;
                            }
                        }
                    }
                } else {
                    if (uni < 62) {
                        if (uni==60) {
                            uni=91;
                        } else {
                            if (uni==61) {
                                uni=126;
                            } else {
                                uni=0;
                            }
                        }
                    } else {
                        if (uni==62) {
                            uni=93;
                        } else {
                            if (uni==64) {
                                uni=124;
                            } else {
                                if (uni==101) {
                                    uni=164;
                                } else {
                                    uni=0;
                                }
                            }
                        }
                    }
                }
            }
            if ((uni > 0) && (uni < 256)) {
                resbin[reslen] = (byte)(uni & 0xff);
                reslen = reslen + 1;
            }
            i = i + 1;
        }
        resstr = YAPI.DefaultEncoding.GetString(resbin);
        if ((resstr).Length > reslen) {
            resstr = (resstr).Substring(0, reslen);
        }
        return resstr;
    }

    public virtual byte[] str2gsm(string msg)
    {
        byte[] asc;
        int asclen;
        int i;
        int ch;
        int gsm7;
        int extra;
        byte[] res;
        int wpos;
        if (!(this._gsm2unicodeReady)) {
            this.initGsm2Unicode();
        }
        asc = YAPI.DefaultEncoding.GetBytes(msg);
        asclen = (asc).Length;
        extra = 0;
        i = 0;
        while (i < asclen) {
            ch = asc[i];
            gsm7 = this._iso2gsm[ch];
            if (gsm7 == 27) {
                extra = extra + 1;
            }
            if (gsm7 == 0) {
                // cannot use standard GSM encoding
                res = new byte[0];
                return res;
            }
            i = i + 1;
        }
        res = new byte[asclen+extra];
        wpos = 0;
        i = 0;
        while (i < asclen) {
            ch = asc[i];
            gsm7 = this._iso2gsm[ch];
            res[wpos] = (byte)(gsm7 & 0xff);
            wpos = wpos + 1;
            if (gsm7 == 27) {
                if (ch < 100) {
                    if (ch<93) {
                        if (ch<92) {
                            gsm7=60;
                        } else {
                            gsm7=47;
                        }
                    } else {
                        if (ch<94) {
                            gsm7=62;
                        } else {
                            gsm7=20;
                        }
                    }
                } else {
                    if (ch<125) {
                        if (ch<124) {
                            gsm7=40;
                        } else {
                            gsm7=64;
                        }
                    } else {
                        if (ch<126) {
                            gsm7=41;
                        } else {
                            gsm7=61;
                        }
                    }
                }
                res[wpos] = (byte)(gsm7 & 0xff);
                wpos = wpos + 1;
            }
            i = i + 1;
        }
        return res;
    }

    public virtual int checkNewMessages()
    {
        string bitmapStr;
        byte[] prevBitmap;
        byte[] newBitmap;
        int slot;
        int nslots;
        int pduIdx;
        int idx;
        int bitVal;
        int prevBit;
        int i;
        int nsig;
        int cnt;
        string sig;
        List<YSms> newArr = new List<YSms>();
        List<YSms> newMsg = new List<YSms>();
        List<YSms> newAgg = new List<YSms>();
        List<string> signatures = new List<string>();
        YSms sms;

        bitmapStr = this.get_slotsBitmap();
        if (bitmapStr == this._prevBitmapStr) {
            return YAPI.SUCCESS;
        }
        prevBitmap = YAPI._hexStrToBin(this._prevBitmapStr);
        newBitmap = YAPI._hexStrToBin(bitmapStr);
        this._prevBitmapStr = bitmapStr;
        nslots = 8*(newBitmap).Length;
        newArr.Clear();
        newMsg.Clear();
        signatures.Clear();
        nsig = 0;
        // copy known messages
        pduIdx = 0;
        while (pduIdx < this._pdus.Count) {
            sms = this._pdus[pduIdx];
            slot = sms.get_slot();
            idx = ((slot) >> (3));
            if (idx < (newBitmap).Length) {
                bitVal = ((1) << ((((slot) & (7)))));
                if ((((newBitmap[idx]) & (bitVal))) != 0) {
                    newArr.Add(sms);
                    if (sms.get_concatCount() == 0) {
                        newMsg.Add(sms);
                    } else {
                        sig = sms.get_concatSignature();
                        i = 0;
                        while ((i < nsig) && ((sig).Length > 0)) {
                            if (signatures[i] == sig) {
                                sig = "";
                            }
                            i = i + 1;
                        }
                        if ((sig).Length > 0) {
                            signatures.Add(sig);
                            nsig = nsig + 1;
                        }
                    }
                }
            }
            pduIdx = pduIdx + 1;
        }
        // receive new messages
        slot = 0;
        while (slot < nslots) {
            idx = ((slot) >> (3));
            bitVal = ((1) << ((((slot) & (7)))));
            prevBit = 0;
            if (idx < (prevBitmap).Length) {
                prevBit = ((prevBitmap[idx]) & (bitVal));
            }
            if ((((newBitmap[idx]) & (bitVal))) != 0) {
                if (prevBit == 0) {
                    sms = this.fetchPdu(slot);
                    newArr.Add(sms);
                    if (sms.get_concatCount() == 0) {
                        newMsg.Add(sms);
                    } else {
                        sig = sms.get_concatSignature();
                        i = 0;
                        while ((i < nsig) && ((sig).Length > 0)) {
                            if (signatures[i] == sig) {
                                sig = "";
                            }
                            i = i + 1;
                        }
                        if ((sig).Length > 0) {
                            signatures.Add(sig);
                            nsig = nsig + 1;
                        }
                    }
                }
            }
            slot = slot + 1;
        }
        this._pdus = newArr;
        // append complete concatenated messages
        i = 0;
        while (i < nsig) {
            sig = signatures[i];
            cnt = 0;
            pduIdx = 0;
            while (pduIdx < this._pdus.Count) {
                sms = this._pdus[pduIdx];
                if (sms.get_concatCount() > 0) {
                    if (sms.get_concatSignature() == sig) {
                        if (cnt == 0) {
                            cnt = sms.get_concatCount();
                            newAgg.Clear();
                        }
                        newAgg.Add(sms);
                    }
                }
                pduIdx = pduIdx + 1;
            }
            if ((cnt > 0) && (newAgg.Count == cnt)) {
                sms = new YSms(this);
                sms.set_parts(newAgg);
                newMsg.Add(sms);
            }
            i = i + 1;
        }
        this._messages = newMsg;
        return YAPI.SUCCESS;
    }

    public virtual List<YSms> get_pdus()
    {
        this.checkNewMessages();
        return this._pdus;
    }

    /**
     * <summary>
     *   Clear the SMS units counters.
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
    public virtual int clearPduCounters()
    {
        int retcode;

        retcode = this.set_pduReceived(0);
        if (retcode != YAPI.SUCCESS) {
            return retcode;
        }
        retcode = this.set_pduSent(0);
        return retcode;
    }

    /**
     * <summary>
     *   Sends a regular text SMS, with standard parameters.
     * <para>
     *   This function can send messages
     *   of more than 160 characters, using SMS concatenation. ISO-latin accented characters
     *   are supported. For sending messages with special unicode characters such as asian
     *   characters and emoticons, use <c>newMessage</c> to create a new message and define
     *   the content of using methods <c>addText</c> and <c>addUnicodeData</c>.
     * </para>
     * </summary>
     * <param name="recipient">
     *   a text string with the recipient phone number, either as a
     *   national number, or in international format starting with a plus sign
     * </param>
     * <param name="message">
     *   the text to be sent in the message
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int sendTextMessage(string recipient, string message)
    {
        YSms sms;

        sms = new YSms(this);
        sms.set_recipient(recipient);
        sms.addText(message);
        return sms.send();
    }

    /**
     * <summary>
     *   Sends a Flash SMS (class 0 message).
     * <para>
     *   Flash messages are displayed on the handset
     *   immediately and are usually not saved on the SIM card. This function can send messages
     *   of more than 160 characters, using SMS concatenation. ISO-latin accented characters
     *   are supported. For sending messages with special unicode characters such as asian
     *   characters and emoticons, use <c>newMessage</c> to create a new message and define
     *   the content of using methods <c>addText</c> et <c>addUnicodeData</c>.
     * </para>
     * </summary>
     * <param name="recipient">
     *   a text string with the recipient phone number, either as a
     *   national number, or in international format starting with a plus sign
     * </param>
     * <param name="message">
     *   the text to be sent in the message
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int sendFlashMessage(string recipient, string message)
    {
        YSms sms;

        sms = new YSms(this);
        sms.set_recipient(recipient);
        sms.set_msgClass(0);
        sms.addText(message);
        return sms.send();
    }

    /**
     * <summary>
     *   Creates a new empty SMS message, to be configured and sent later on.
     * <para>
     * </para>
     * </summary>
     * <param name="recipient">
     *   a text string with the recipient phone number, either as a
     *   national number, or in international format starting with a plus sign
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual YSms newMessage(string recipient)
    {
        YSms sms;
        sms = new YSms(this);
        sms.set_recipient(recipient);
        return sms;
    }

    /**
     * <summary>
     *   Returns the list of messages received and not deleted.
     * <para>
     *   This function
     *   will automatically decode concatenated SMS.
     * </para>
     * </summary>
     * <returns>
     *   an YSms object list.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list.
     * </para>
     */
    public virtual List<YSms> get_messages()
    {
        this.checkNewMessages();
        return this._messages;
    }

    /**
     * <summary>
     *   Continues the enumeration of MessageBox interfaces started using <c>yFirstMessageBox()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned MessageBox interfaces order.
     *   If you want to find a specific a MessageBox interface, use <c>MessageBox.findMessageBox()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YMessageBox</c> object, corresponding to
     *   a MessageBox interface currently online, or a <c>null</c> pointer
     *   if there are no more MessageBox interfaces to enumerate.
     * </returns>
     */
    public YMessageBox nextMessageBox()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindMessageBox(hwid);
    }

    //--- (end of generated code: YMessageBox implementation)

    //--- (generated code: YMessageBox functions)

    /**
     * <summary>
     *   Starts the enumeration of MessageBox interfaces currently accessible.
     * <para>
     *   Use the method <c>YMessageBox.nextMessageBox()</c> to iterate on
     *   next MessageBox interfaces.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YMessageBox</c> object, corresponding to
     *   the first MessageBox interface currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YMessageBox FirstMessageBox()
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
        err = YAPI.apiGetFunctionsByClass("MessageBox", 0, p, size, ref neededsize, ref errmsg);
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
        return FindMessageBox(serial + "." + funcId);
    }



    //--- (end of generated code: YMessageBox functions)
}
#pragma warning restore 1591
