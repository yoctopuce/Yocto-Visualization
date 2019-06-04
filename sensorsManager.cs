﻿
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Sensor abstraction class
 * 
 *  
 *   - - - - - - - - - License information: - - - - - - - - -
 *
 *  Copyright (C) 2017 and beyond by Yoctopuce Sarl, Switzerland.
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
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Xml;
using System.Diagnostics;

namespace YoctoVisualisation
{

 


  public class TimedSensorValue
  {
    public double DateTime { get; set; }
    public double Value { get; set; }
  }



  public class NullYSensor : CustomYSensor
  {
    public NullYSensor() : base(null, "",null)
    {
      hwdName = "NOTAREALSENSOR";
      friendlyname = "NOTAREALSENSOR";

    }
    public new string get_unit() { return ""; }
    public new void registerCallback(Form f) { }
    public new void forceUpdate() { }
    public new string get_frequency() { return ""; }
    public new void set_frequency(string frequencyToSet) { }
    public new YSensor get_sensor() { return null; }
    public override string ToString() { return "(none)"; }

    public override void setAlarmCondition(int index, int condition) {  }
    public override int getAlarmCondition(int index) { return 0; }
    public override void setAlarmValue(int index,double value) {  }
    public override double getAlarmValue(int index) { return 0; }
    public override void setAlarmDelay(int index, int value) {  }
    public override int getAlarmDelay(int index) { return 0; }
    public override void setAlarmCommandline(int index, string value) {  }
    public override  string getAlarmCommandline(int index) {  return ""; }



  }

  public class AlarmSettings
  { int index;
    int Condition = 0;
    int Source = 0;
    double Value = 0;
    int Delay = 15;
    string Commandline = "";
    CustomYSensor parent;
    DateTime lastAlarm = DateTime.MinValue;


    static void ExecuteCommand(string source, string command)
    {

      string shell = "cmd.exe";
      string shellcommand = "/c " + command;
      if (constants.MonoRunning) { shell = "bash"; shellcommand = "-c \"" + command + "\""; }
      LogManager.Log(source + " executing :" + shell + " " + shellcommand);
      var processInfo = new ProcessStartInfo(shell, shellcommand);
      processInfo.CreateNoWindow = true;
      processInfo.UseShellExecute = false;
      processInfo.RedirectStandardError = true;
      processInfo.RedirectStandardOutput = true;

      try
      {
        var process = Process.Start(processInfo);

        process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            LogManager.Log(source + " output :" + e.Data);
        process.BeginOutputReadLine();

        process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            LogManager.Log(source + " error : " + e.Data);
        process.BeginErrorReadLine();

        process.WaitForExit();

        //    Console.WriteLine(source + " ExitCode: " + process.ExitCode.ToString());
        process.Close();
      }
      catch (Exception e)
      {
        LogManager.Log(source + " execution raised an exception :" + e.Message);

      }


    }



    public AlarmSettings(int index, CustomYSensor owner, XmlNode xmldata)
    {
      this.index = index;
      parent = owner;
      if (xmldata != null)
      {
        Source = int.Parse(xmldata.Attributes["Source"].InnerText);
        Condition = int.Parse(xmldata.Attributes["Condition"].InnerText);
        Value = double.Parse(xmldata.Attributes["Value"].InnerText);
        Commandline = xmldata.Attributes["Cmd"].InnerText;
        Delay = int.Parse(xmldata.Attributes["Delay"].InnerText);


      }

    }

    public AlarmSettings(int index, CustomYSensor owner) 
      : this(index, owner, null) { }
   

    public string getXmlData()
    { return "<Alarm "
            + "Source=\"" + Source.ToString() + "\" "
            + "Condition=\"" + Condition.ToString() + "\" "
            + "Value=\"" + Value.ToString() + "\" "
            + "Cmd=\"" + System.Security.SecurityElement.Escape(Commandline) + "\" "
            + "Delay=\"" + Delay.ToString() + "\"/>\n";

    }

  

    public  void setCondition( int condition) { this.Condition = condition; }
    public  int getCondition() { return this.Condition; }
    public void setSource(int source) { this.Source = source; }
    public int getSource() { return this.Source; }
    public  void setValue( double value) { this.Value = value; }
    public  double getValue() { return this.Value; }
    public  void setDelay( int value) { this.Delay = value; }
    public  int getDelay() { return this.Delay; }
    public  void setCommandline( string value) { this.Commandline = value; }
    public  string getCommandline() { return this.Commandline; }

    public void check(YMeasure m )
    { bool alarm = false;
      string reason = "";
      string src = "";
      double SensorValue = 0;
     
      switch (Source)
      {
        case 1:  src = "MIN"; SensorValue = m.get_minValue(); break;
        case 2:  src = "MAX"; SensorValue = m.get_maxValue();  break;
        default: src = "AVG"; SensorValue = m.get_averageValue();  break;

      }

      switch (Condition)
        { default : return;  // alarm disabled
          case 1: reason = ">";  if  (SensorValue > Value) alarm = true;break;
          case 2: reason = ">="; if (SensorValue >= Value) alarm = true; break;
          case 3: reason = "="; if (SensorValue == Value) alarm = true; break;
          case 4: reason = "<="; if (SensorValue <= Value) alarm = true; break;
          case 5: reason = "<"; if (SensorValue < Value) alarm = true; break;
      }
      if (!alarm) return;
      if (((DateTime.Now - lastAlarm).Seconds) < Delay) return;

      string source = "ALARM " + (index + 1).ToString();
      LogManager.Log(source+" on " + parent.get_hardwareId() + "/" + parent.get_friendlyName() + " (" + SensorValue.ToString() + reason + Value.ToString() + ")");
     
      string Execute = Commandline;
      Execute = Execute.Replace("$SENSORVALUE$", SensorValue.ToString());
      Execute = Execute.Replace("$HWDID$", parent.get_hardwareId());
      Execute = Execute.Replace("$NAME$", parent.get_friendlyName());
      Execute = Execute.Replace("$CONDITION$", reason);
      Execute = Execute.Replace("$DATATYPE$", src);
      Execute = Execute.Replace("$TRIGGER$", Value.ToString());
      Execute = Execute.Replace("$NOW$", DateTime.Now.ToString("yyyy/MM/dd h:mm:ss.ff"));



      new Thread(() =>
      {
        Thread.CurrentThread.IsBackground = true;
        ExecuteCommand(source,Execute);
       }).Start();

     
      lastAlarm = DateTime.Now;
    }

  }


  public class CustomYSensor
  {
    YSensor sensor;
    protected string hwdName;
    protected string friendlyname;
    string unit = "";
    string frequency = "";
    bool recording = false;
    private List<Form> FormsToNotify;
    bool online = false;
    bool preloadDone = false;
    bool loadDone = false;
    bool dataLoggerFeature = false;
    private readonly Mutex dataMutex = new Mutex();
    bool cfgChgNotificationsSupported = false;
    bool mustReloadConfig = false;
   // private bool _isReadOnly = false;

    public bool isReadOnly { get { return sensor.isReadOnly(); } }


    ulong lastGetConfig = 0;
    int recordedDataLoadProgress = 0;
    YDataSet recordedData = null;
    bool loadFailed = false;
    double firstLiveDataTimeStamp = 0;
    double firstDataloggerTimeStamp = 0;
    double lastDataTimeStamp = 0;

    int globalDataLoadProgress = 0;
    public List<TimedSensorValue> minData = new List<TimedSensorValue>();
    public List<TimedSensorValue> curData = new List<TimedSensorValue>();
    public List<TimedSensorValue> maxData = new List<TimedSensorValue>();

    public List<TimedSensorValue> previewMinData;
    public List<TimedSensorValue> previewCurData;
    public List<TimedSensorValue> previewMaxData;

    BackgroundWorker predloadProcess;
    BackgroundWorker loadProcess;
    long dataLoggerStartReadTime = 0;
    List<AlarmSettings> Alarms = new List<AlarmSettings>();
    private static  int _MaxDataRecords = 0;
    public static int MaxDataRecords
    {
      get { return _MaxDataRecords; }
      set { _MaxDataRecords = value; }

      
    }

    private static int _MaxLoggerRecords = 0;
    public static int MaxLoggerRecords
    {
      get { return _MaxLoggerRecords; }
      set { _MaxLoggerRecords = value; }


    }

    public void ConfigHasChanged()
    {
       cfgChgNotificationsSupported = true;
       mustReloadConfig = true;

    }

    public CustomYSensor(YSensor s, string name, XmlNode SensorLocalConfig)
    {
      FormsToNotify = new List<Form>();

      sensor = s;
      hwdName = name;
      friendlyname = name;
      if (s == null) return;

      predloadProcess = new BackgroundWorker();

      predloadProcess.DoWork += new DoWorkEventHandler(preload_DoWork);
      predloadProcess.RunWorkerCompleted += new RunWorkerCompletedEventHandler(preload_Completed);
      predloadProcess.ProgressChanged += new ProgressChangedEventHandler(load_ProgressChanged);


      loadProcess = new BackgroundWorker();
      loadProcess.WorkerReportsProgress = true;
      loadProcess.WorkerSupportsCancellation = true;
      loadProcess.DoWork += new DoWorkEventHandler(load_DoWork);
      loadProcess.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_Completed);
      loadProcess.ProgressChanged += new ProgressChangedEventHandler(load_ProgressChanged);


      if (s.isOnline())
      {
        hwdName = s.get_hardwareId();
        friendlyname = s.get_friendlyName();
        configureSensor();

      
       
         if (this.isReadOnly) LogManager.Log(hwdName + " is read only");
        

        online = true;
      //  loadDatalogger();  // will be done automatically at device arrival

      }
      if (SensorLocalConfig != null)
      {
        int index = 0;
        foreach (XmlNode n in SensorLocalConfig)
        {
          
          if (n.Name == "Alarm")
          {
            checkAlarmIndex(index);
            Alarms[index] =new AlarmSettings(index, this, n);
            index++;

          }

        }
      }

    }

    private void checkAlarmIndex(int index)
      {  while (Alarms.Count < index + 1) Alarms.Add(new AlarmSettings(Alarms.Count,    this));
     
      }

    public int getAlarmCount()
    {
      return Alarms.Count;
    }



    public virtual void setAlarmCondition(int index,int condition) { checkAlarmIndex(index); this.Alarms[index].setCondition( condition);  }
    public virtual int getAlarmCondition(int index) { checkAlarmIndex(index); return this.Alarms[index].getCondition(); }
    public virtual void setAlarmSource(int index, int source) { checkAlarmIndex(index); this.Alarms[index].setSource(source); }
    public virtual int getAlarmSource(int index) { checkAlarmIndex(index); return this.Alarms[index].getSource(); }

    public virtual void setAlarmValue(int index, double value) { checkAlarmIndex(index); this.Alarms[index].setValue(value); }
    public virtual double getAlarmValue(int index) { checkAlarmIndex(index); return this.Alarms[index].getValue(); }
    public virtual void setAlarmDelay(int index, int value) { checkAlarmIndex(index); this.Alarms[index].setDelay( value); }
    public virtual int getAlarmDelay(int index) { checkAlarmIndex(index); return this.Alarms[index].getDelay(); }
    public virtual void setAlarmCommandline(int index, string value) { checkAlarmIndex(index); this.Alarms[index].setCommandline(  value); }
    public virtual string getAlarmCommandline(int index) { checkAlarmIndex(index);  return this.Alarms[index].getCommandline(); }

    public string GetXmlData()
    { string res =  "<Sensor ID=\"" + get_hardwareId() + "\">\n";
      for (int i = 0; i < getAlarmCount(); i++)
        res = res + Alarms[i].getXmlData();
      res = res + "</Sensor>\n";
      return res;
    }


  public int getGetaLoadProgress() { return globalDataLoadProgress; }


    public double get_firstLiveDataTimeStamp()
    { return firstLiveDataTimeStamp; }
    public double get_firstDataloggerTimeStamp()
    { return firstDataloggerTimeStamp; }
    public double get_lastDataTimeStamp()
    { return lastDataTimeStamp; }

    protected void preload_DoWork(object sender, DoWorkEventArgs e)
    {
      recordedData = sensor.get_recordedData(0, 0);
      
      try
      {
        recordedDataLoadProgress = recordedData.loadMore();
      }
      catch (Exception ex) { LogManager.Log(hwdName + ": load more caused an exception " + ex.ToString()); }

      
      globalDataLoadProgress = recordedDataLoadProgress;

      List<YMeasure> measures = recordedData.get_preview();
      previewMinData = new List<TimedSensorValue>();
      previewCurData = new List<TimedSensorValue>();
      previewMaxData = new List<TimedSensorValue>();
      int startIndex = 0;
      if ((_MaxDataRecords > 0) && (measures.Count > _MaxDataRecords)) startIndex = measures.Count - _MaxDataRecords;

      for (int i = startIndex; i < measures.Count; i++)
      {
        double t = measures[i].get_endTimeUTC();
        if ((t < firstLiveDataTimeStamp) || (firstLiveDataTimeStamp == 0))
        {
          previewMinData.Add(new TimedSensorValue { DateTime = t, Value = measures[i].get_minValue() });
          previewCurData.Add(new TimedSensorValue { DateTime = t, Value = measures[i].get_averageValue() });
          previewMaxData.Add(new TimedSensorValue { DateTime = t, Value = measures[i].get_maxValue() });
        }
      }



      if (_MaxLoggerRecords > 0)
      { // find out where to start reading datalogger to make sure we don't read more the _MaxLoggerRecords records

        List<YDataStream> list = recordedData.get_privateDataStreams();
        int index = list.Count - 1;
        int totalRecords = 0;
        while ((index > 0) && (totalRecords< _MaxLoggerRecords))
         { totalRecords += list[index].get_rowCount();
           dataLoggerStartReadTime = list[index].get_startTimeUTC();
           index--;
         }

        int n = 0;
        while ((n < previewMinData.Count) && (previewMinData[n].DateTime < dataLoggerStartReadTime)) n++;
        if (n>1)
        {
          previewMinData.RemoveRange(0, n-1);
          previewCurData.RemoveRange(0, n-1);
          previewMaxData.RemoveRange(0, n-1);
        }
        
      }
    }

    protected void preload_Completed(object sender, RunWorkerCompletedEventArgs e)
    {


      LogManager.Log(hwdName + " : datalogger preloading completed ("+ previewMinData.Count + " rows )");

      int RangeTo = -1;
      if (previewMinData == null) return;


      if (previewMinData.Count <= 0) return;


      if (firstLiveDataTimeStamp > 0)
      {
        while ((RangeTo < previewMinData.Count - 1) && (previewMinData[RangeTo + 1].DateTime < firstLiveDataTimeStamp)) RangeTo++;
      }
      else RangeTo = previewMinData.Count - 1;

      if (RangeTo < 0) return;

      firstDataloggerTimeStamp = previewMinData[0].DateTime;


      previewMinData = previewMinData.GetRange(0, RangeTo);
      previewCurData = previewCurData.GetRange(0, RangeTo);
      previewMaxData = previewMaxData.GetRange(0, RangeTo);
      dataMutex.WaitOne();
      minData = previewMinData.Union(minData).ToList();
      curData = previewCurData.Union(curData).ToList();
      maxData = previewMinData.Union(maxData).ToList();
      dataMutex.ReleaseMutex();

      preloadDone = true;


      int count = curData.Count;
      if (count > 0)
        if (curData[count - 1].DateTime > lastDataTimeStamp)
          lastDataTimeStamp = curData[count - 1].DateTime;

      foreach (Form f in FormsToNotify)
        if (f is GraphForm)
          ((GraphForm)f).SensorNewDataBlock(this, 0, RangeTo - 1, 0, true);

      if (recordedDataLoadProgress < 100)
      {
        LogManager.Log(hwdName + " : start datalogger loading");
        loadProcess.RunWorkerAsync(null);

      }

    }

    public string get_frequency()
    {
      if (online)
      {
        if (sensor.isOnline())
        {
          frequency = sensor.get_reportFrequency();
          return frequency;
        }
        else online = false;
      }
      return "";
    }

    public void set_frequency(string frequencyToSet)
    {
      if (online)
      {
        if (sensor.isOnline())
        {
          frequency = frequencyToSet;
          sensor.set_reportFrequency(frequency);
          string lfreq = sensor.get_logFrequency();
          try
          {
            if (lfreq != "OFF") sensor.set_logFrequency(frequency);
            sensor.get_module().saveToFlash();
          } catch (Exception e)
           {
            LogManager.Log("failed to change "+hwdName + " log frequency (" + e.Message+")");

          }
        }
        else online = false;
      }

    }

    public bool get_recording()
    {
      if (!dataLoggerFeature) return false;
      if (online)
      {
        if (sensor.isOnline())
        {
          recording = sensor.get_logFrequency() != "OFF";
          return recording;
        }
        else online = false;
      }
      return false;
    }
    public void set_recording(bool recordingStatus)
    {
      if (!dataLoggerFeature) return;
      if (online)
      {
        if (sensor.isOnline())
        {
          recording = recordingStatus;
          try
          {
            sensor.set_logFrequency(recording ? frequency : "OFF");
            YDataLogger dl = YDataLogger.FindDataLogger(sensor.get_module().get_serialNumber() + ".dataLogger");    
            dl.set_recording(recording ? YDataLogger.RECORDING_ON : YDataLogger.RECORDING_OFF);
            dl.set_autoStart(recording ? YDataLogger.AUTOSTART_ON : YDataLogger.RECORDING_OFF);
            sensor.get_module().saveToFlash();
          }
          catch (Exception e)
          {
            LogManager.Log("failed to change " + hwdName + " recording (" + e.Message + ")");

          }
        }
        else online = false;
      }
    }




 

    protected void load_DoWork(object sender, DoWorkEventArgs e)
    {
     
      if (dataLoggerStartReadTime>0)
       {
         recordedData = sensor.get_recordedData(dataLoggerStartReadTime, 0);
       }

      while (recordedDataLoadProgress < 100)
      {
        if ((((BackgroundWorker)sender).CancellationPending == true))
        {
          globalDataLoadProgress = 100;
          loadDone = true;
          loadFailed = false;
          e.Cancel = true;
          break;
        }


        try
        {
          recordedDataLoadProgress = recordedData.loadMore();
          //LogManager.Log(hwdName + "loading " + recordedDataLoadProgress.ToString() + "%");


        }
        catch (Exception) { loadFailed = true; return; }


                if (globalDataLoadProgress != (int)(recordedDataLoadProgress))
        {
          
          globalDataLoadProgress = (int)(recordedDataLoadProgress);
          ((BackgroundWorker)sender).ReportProgress(globalDataLoadProgress);
        }
      }

      List<YMeasure> measures = recordedData.get_measures();
      previewMinData = new List<TimedSensorValue>();
      previewCurData = new List<TimedSensorValue>();
      previewMaxData = new List<TimedSensorValue>();
      for (int i = 0; i < measures.Count; i++)

      {
        double t = measures[i].get_endTimeUTC();
        if ((previewMinData.Count == 0) || (t > previewMinData[previewMinData.Count - 1].DateTime))
        {
          previewMinData.Add(new TimedSensorValue { DateTime = t, Value = measures[i].get_minValue() });
          previewCurData.Add(new TimedSensorValue { DateTime = t, Value = measures[i].get_averageValue() });
          previewMaxData.Add(new TimedSensorValue { DateTime = t, Value = measures[i].get_maxValue() });
        }
      }

      if (_MaxDataRecords > 0) previewDataCleanUp();

      for (int i = 0; i < previewMinData.Count - 1; i++)
      {
        if (previewMinData[i].DateTime >= previewMinData[i + 1].DateTime)
          throw new Exception("Time-stamp inconsistency");
      }

     



      if (previewMinData.Count > 0)
      {
        globalDataLoadProgress = 100;
        int index = 0;
        dataMutex.WaitOne();
        double lastPreviewTimeStamp = previewMinData[previewMinData.Count - 1].DateTime;
        while ((index < minData.Count) && (minData[index].DateTime < lastPreviewTimeStamp)) index++;
        //LogManager.Log(hwdName + " time range is ["+constants.UnixTimeStampToDateTime(previewMinData[0].DateTime)+".."+ constants.UnixTimeStampToDateTime(lastPreviewTimeStamp)+"]");

        minData.RemoveRange(0, index);
        curData.RemoveRange(0, index);
        maxData.RemoveRange(0, index);
        minData.InsertRange(0, previewMinData);
        curData.InsertRange(0, previewCurData);
        maxData.InsertRange(0, previewMaxData);
        dataMutex.ReleaseMutex();
      }
      firstDataloggerTimeStamp = curData[0].DateTime;
      loadDone = true;
      loadFailed = false;

      int count = curData.Count;
      if (count > 0)
        if (curData[count - 1].DateTime > lastDataTimeStamp)
          lastDataTimeStamp = curData[count - 1].DateTime;
    }



    protected void load_Completed(object sender, RunWorkerCompletedEventArgs e)
    {

      if (loadFailed)
      {
        LogManager.Log(hwdName + " : datalogger loading failed");
        return;
      }

      if ((e.Cancelled == true))
      {
        LogManager.Log(hwdName + " : datalogger loading was canceled");
        previewMinData.Clear();
        previewCurData.Clear();
        previewMaxData.Clear();
        return;
      }

      LogManager.Log(hwdName + " : datalogger loading completed  (" + previewMinData.Count + " rows )");

      loadDone = true;
      globalDataLoadProgress = 100;
      if (previewMinData.Count <= 0)
      {
        foreach (Form f in FormsToNotify)
          if (f is GraphForm)
            ((GraphForm)f).DataLoggerProgress();
        return;
      }
      foreach (Form f in FormsToNotify)
        if (f is GraphForm)
        {
          ((GraphForm)f).DataloggerCompleted(this);
          ((GraphForm)f).DataLoggerProgress();
        }
      previewMinData.Clear();
      previewCurData.Clear();
      previewMaxData.Clear();

    }

    private void dataCleanUp()
    { if (_MaxDataRecords <= 0) return;
      int newsize = (_MaxDataRecords * 90) / 100;
      if ((curData!=null) && (_MaxDataRecords< curData.Count))
      {
        
        minData.RemoveRange(0,minData.Count - newsize);
        curData.RemoveRange(0,curData.Count - newsize);
        maxData.RemoveRange(0,maxData.Count - newsize);
      }
    

    }

    private void previewDataCleanUp()
    {
      if (_MaxDataRecords <= 0) return;
      int newsize = (_MaxDataRecords * 90) / 100;  
      if ((previewMinData != null) && (_MaxDataRecords < previewMinData.Count))
      {
        previewMinData.RemoveRange(0,previewMinData.Count - newsize);
        previewCurData.RemoveRange(0,previewCurData.Count - newsize);
        previewMaxData.RemoveRange(0,previewMaxData.Count - newsize);
      }

    }


    public void stopDataloggerloading()
    {
      dataMutex.WaitOne();
      minData.Clear();
      curData.Clear();
      maxData.Clear();
      dataMutex.ReleaseMutex();
      firstLiveDataTimeStamp = 0;
      firstDataloggerTimeStamp = 0;
      lastDataTimeStamp = 0;

      if (!loadProcess.CancellationPending) loadProcess.CancelAsync();
      load_ProgressChanged(null, null);

    }
    protected void load_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      foreach (Form f in FormsToNotify)
        if (f is GraphForm)
        {
          ((GraphForm)f).DataLoggerProgress();
        }

    }


    public bool isOnline()
    {
      return online;
    }

    public void reloadConfig()
    {
      if (online)
      {
        bool ison = sensor.isOnline();

        if (ison)
        {
          try
          {
            unit         = sensor.get_unit();
            friendlyname = sensor.get_friendlyName();
            lastGetConfig = YAPI.GetTickCount();
            mustReloadConfig = false;
          }
          catch (Exception e) { LogManager.Log("reload configuration error: " + e.Message); }
        }
        else online = false;
      }

    }

    public string get_unit()
    {
      
      if ((cfgChgNotificationsSupported) && (!mustReloadConfig)) return unit;
      if ((lastGetConfig <= 0) || (YAPI.GetTickCount() - lastGetConfig > 5000)) reloadConfig();
      return unit;

    }

    public void  loadDatalogger()
    {
      if (constants.maxPointsPerDataloggerSerie < 0)
      {
        LogManager.Log(hwdName + " : datalogger access is disabled");
        return;
      }

      if (this.isReadOnly)
      {
        LogManager.Log(hwdName + " is read only, cannot load the datalogger contents  (yes that's a bug)");
        return;
      }


      if ((!preloadDone) && (!predloadProcess.IsBusy) && dataLoggerFeature)
      {
        LogManager.Log(hwdName + " : start datalogger preloading");
        predloadProcess.RunWorkerAsync(null);
      }
      else if ( (preloadDone) && (!loadDone) && (!loadProcess.IsBusy)  && dataLoggerFeature)
      {
        LogManager.Log(hwdName + " : start datalogger loading");
        loadProcess.RunWorkerAsync(null);

      }

    }


    public void arrival(bool dataloggerOn)
    {
      configureSensor();
      online = true;
      loadDatalogger();
      if (isReadOnly) LogManager.Log(hwdName + " is read only");


    }
    public void removal()
    {
      online = false;
      forceUpdate();

    }


    public void configureSensor()
    {
      bool mustSave = false;
      if (!sensor.isOnline()) return;


      string lfreq = sensor.get_logFrequency();
      string rfreq = sensor.get_reportFrequency();


      YModule m = sensor.get_module();
      for (int i = 0; i < m.functionCount(); i++)
        if (m.functionType(i) == "DataLogger")
          dataLoggerFeature = true;

      try
      {
        if (dataLoggerFeature)
        {
          YDataLogger dl = YDataLogger.FindDataLogger(sensor.get_module().get_serialNumber() + ".dataLogger");
          bool dataloggerOn = dl.get_recording() != YDataLogger.RECORDING_OFF;

          if ((!dataloggerOn) && (lfreq != "OFF")) { lfreq = "OFF"; mustSave = true; }

          if (lfreq != "OFF") { rfreq = lfreq; sensor.set_reportFrequency(rfreq); mustSave = true; }
          else if (rfreq == "OFF") { rfreq = "1/s"; sensor.set_reportFrequency(rfreq); mustSave = true; }
          if (mustSave) sensor.set_logFrequency(lfreq);

          if (lfreq != "OFF")
          {
            dl.set_recording(YDataLogger.RECORDING_ON);
            dl.set_autoStart(YDataLogger.AUTOSTART_ON);
            mustSave = true;
          }

        }
        else
        {
          lfreq = "OFF";
          rfreq = sensor.get_reportFrequency();
          if (rfreq == "OFF") { rfreq = "1/s"; sensor.set_reportFrequency(rfreq); mustSave = true; }

        }


        if (mustSave) sensor.get_module().saveToFlash();

      }
      catch (Exception e)
      {
        LogManager.Log("failed to configure " + hwdName + "  (" + e.Message + ")");

      }

      sensor.registerTimedReportCallback(TimedCallback);
      recording = (lfreq != "OFF");
      frequency = rfreq;
    }

    public void registerCallback(Form f)
    {
      if (FormsToNotify.Contains(f)) return;
      FormsToNotify.Add(f);
      return;
    }


    public void forceUpdate()
    {
      TimedCallback(sensor, null);

    }



    public void TimedCallback(YFunction source, YMeasure M)
    {
      

      if (M != null)
      {
        online = true;
        double t = M.get_endTimeUTC();
        if (firstLiveDataTimeStamp == 0) firstLiveDataTimeStamp = t;
        if (t > lastDataTimeStamp) lastDataTimeStamp = t;
        dataMutex.WaitOne();
        curData.Add(new TimedSensorValue { DateTime = t, Value = M.get_averageValue() });
        minData.Add(new TimedSensorValue { DateTime = t, Value = M.get_minValue() });
        maxData.Add(new TimedSensorValue { DateTime = t, Value = M.get_maxValue() });
        if (_MaxDataRecords > 0) dataCleanUp();
        dataMutex.ReleaseMutex();

        for (int i = 0; i < Alarms.Count; i++)
          Alarms[i].check(M);

      }

      foreach (Form f in FormsToNotify)
      {
        if (f is gaugeForm) ((gaugeForm)f).SensorValuecallback(this, M);
        if (f is angularGaugeForm) ((angularGaugeForm)f).SensorValuecallback(this, M);
        if (f is digitalDisplayForm) ((digitalDisplayForm)f).SensorValuecallback(this, M);
        if (f is GraphForm) ((GraphForm)f).SensorValuecallback(this, M);

      }



    }



    public YSensor get_sensor()
    {
      return sensor;
    }

    public string get_hardwareId()
    { return hwdName; }

    public string get_friendlyName()
    {
      if ((cfgChgNotificationsSupported) && (!mustReloadConfig)) return friendlyname;
      reloadConfig();
      return friendlyname;

    }



    public override string ToString()
    {
      if (online)
      {
        try
        {
          friendlyname = sensor.get_friendlyName();
        }
        catch { online = false; }
      }
      if (online)
      { if (this.isReadOnly) return friendlyname + " (READ ONLY)";
        return friendlyname;
      }
      return friendlyname + " (OFFLINE)";

    }



  }





 

  public static class sensorsManager
  {

    public delegate void SensorManagerChangeCallback();


    private static int counter = 0;
    
    
    public static List<CustomYSensor> sensorList;  // actual list of sensors
    public static CustomYSensor NullSensor;
    private static XmlNode KnownSensors = null;  // sensors list picked up from XML configuration file

    private static SensorManagerChangeCallback _changeCallback=null;

    public static void registerChangeCallback(SensorManagerChangeCallback changeCallback)
       { _changeCallback = changeCallback; }

    public static string  getXMLSensorsConfig()
    {
      string res = "<Sensors>\n";
      foreach (CustomYSensor s in sensorList)
        if (!(s is NullYSensor))
             res = res + s.GetXmlData();   
      res =res+"</Sensors>\n";
      return res;

    }

    public static void setKnownSensors(XmlNode sensorXMLList)
    {
      KnownSensors = sensorXMLList;

    }

    public static XmlNode FindSensorLastLocalConfig(string hwdId)
    {
      XmlNode SensorConfig = null;
      if (KnownSensors != null)
        foreach (XmlNode node in KnownSensors)
          if (node.Name == "Sensor")
            if (node.Attributes["ID"].InnerText == hwdId)
              SensorConfig = node;
      return SensorConfig;
    }

    public static void deviceConfigChanged(YModule m)
    {
      LogManager.Log("Configuration change on device  " + m.get_serialNumber());
      string serialprefix = m.get_serialNumber().Substring(0, 8);
      for (int i = 0; i < sensorList.Count; i++)
        if (sensorList[i].get_hardwareId().Substring(0, 8) == serialprefix)
          sensorList[i].ConfigHasChanged();

      if (_changeCallback != null) _changeCallback();
    }

    public static void deviceArrival(YModule m)
    {
      try
      {
        int count = m.functionCount();
        string serial = m.get_serialNumber();
        int luminosity = m.get_luminosity();
        LogManager.Log("Device Arrival " + serial);
        bool recording = false;
        // first loop to find network and datalogger settings
        for (var i = 0; i < count; i++)
        {
          string ftype = m.functionType(i);
          string fid = m.functionId(i);

          if (ftype == "Network")
          {
            YNetwork net = YNetwork.FindNetwork(serial + "." + fid);
            StartForm.NetworkArrival(net);
          }
          else if (ftype== "DataLogger")
          {
            YDataLogger dlog = YDataLogger.FindDataLogger(serial + "." + fid);
            int state = dlog.get_recording();

            if ((state == YDataLogger.RECORDING_ON) || (state == YDataLogger.RECORDING_PENDING))
              recording = true;
          }
        }

        // second loop to register all Sensors
        for (var i = 0; i < count; i++)
        {
          string fbasetype = m.functionBaseType(i);
          string fid = m.functionId(i);
          if (fbasetype == "Sensor")
          {
            string hwdID = serial + "." + fid;
            LogManager.Log("New sensor arrival: " + hwdID);
            bool found = false;
            foreach (CustomYSensor alreadyThereSensor in sensorList)
            {

              if (alreadyThereSensor.get_hardwareId() == hwdID)
              {
                alreadyThereSensor.arrival(recording);
                found = true;
              }
            }
            if (!found)
            {
              YSensor s = YSensor.FindSensor(serial + "." + fid);
              string hwd = s.get_hardwareId();

              //if (s.isReadOnly()) LogManager.Log(hwd + " is read only!");

              sensorList.Add(new CustomYSensor(s, hwd, FindSensorLastLocalConfig(hwd)));
             
            }
          }
        }

        // register configuration change callback then tries to trigger a configuration 
        // change to check to the devices supports that feature 
        // (depends on firmware version)

        m.registerConfigChangeCallback(deviceConfigChanged);
        try
        {
          m.triggerConfigChangeCallback();
        } catch   (Exception )
        {
          
        }

        if (_changeCallback != null) _changeCallback();
      }
      catch (Exception e) {
         LogManager.Log("Device Arrival Error: " + e.Message);
      }
    }




    public static void deviceRemoval(YModule m)
    {

      string serial = m.get_serialNumber();

      LogManager.Log("Device removal " + serial);
      StartForm.DeviceRemoval(serial);
      foreach (CustomYSensor alreadyThereSensor in sensorList)
        if (!(alreadyThereSensor is NullYSensor))
        {
          string hwd = alreadyThereSensor.get_hardwareId();
          if (hwd.Length >= serial.Length)
            if (hwd.Substring(0, serial.Length) == serial)
              alreadyThereSensor.removal();
        }

      if (_changeCallback != null) _changeCallback();

    }

    public static CustomYSensor AddNewSensor(string hwdID)
    {
      foreach (CustomYSensor alreadyThereSensor in sensorList)
        if (alreadyThereSensor != null)
          if (alreadyThereSensor.get_hardwareId() == hwdID) return alreadyThereSensor;

      YSensor s = YSensor.FindSensor(hwdID);
      CustomYSensor cs = new CustomYSensor(s, hwdID, FindSensorLastLocalConfig(hwdID));
      sensorList.Add(cs);
      return cs;
    }


    public static CustomYSensor getNullSensor()
    {
      return NullSensor;
    }


    static sensorsManager()
    {
      NullSensor = new NullYSensor();
      sensorList = new List<CustomYSensor>();
      sensorList.Add(NullSensor);
      YAPI.RegisterDeviceArrivalCallback(deviceArrival);
      YAPI.RegisterDeviceRemovalCallback(deviceRemoval);
      

    }




    public static void run()
    {
      string errmsg = "";

      if (counter == 0)
      {
        YAPI.UpdateDeviceList(ref errmsg);

      }
      else YAPI.HandleEvents(ref errmsg);
      counter = (counter + 1) % 20;
    }

  }
}
