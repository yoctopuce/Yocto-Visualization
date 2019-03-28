using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
#if !NET35
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace UpdateAppLibrary
{
    [DataContract]
    public class YAppRelease
    {
        [DataMember] public int version;
        [DataMember] public string link;
        [DataMember] public string date;
        [DataMember] public string what;
    }

  public class YAppReleaseManager
  {
    private readonly string SERVER_NAME = "https://www.yoctopuce.com";
    //private readonly string SERVER_NAME = "http://localhost";

    private readonly int _svnVersion;
    private readonly string _versionPrefix;

    private readonly string _appName;
    private readonly string _platform;
    private YAppInterface _appInterface;
    private System.Net.WebClient _client;
    private NotifyIcon _notifyIcon;

    public YAppReleaseManager(YAppInterface appInterface)
    {
      this._appInterface = appInterface;
      this._appName = appInterface.GetAppName();
      this._platform = appInterface.GetPlatform();
      string fullVersionStr = appInterface.GetVersion();


      int pos = fullVersionStr.LastIndexOf('.');
      if (pos < 0) {
        throw new ArgumentException("Invalid version string:" + fullVersionStr);
      }
      string svnVersionStr = fullVersionStr.Substring(pos + 1);
      if (svnVersionStr == "PATCH_WITH_BUILD") {
        this._versionPrefix = fullVersionStr.Substring(0, pos);
        this._svnVersion = 1234;
      } else {
        this._versionPrefix = fullVersionStr.Substring(0, pos);
        this._svnVersion = Convert.ToInt32(svnVersionStr);
      }
    }


    public void StartBackgroundCheckForUpdates()
    {
      if (!_appInterface.GetCheckUpdateSettings()) return;
      BackgroundWorker bg = new BackgroundWorker();
      bg.DoWork += backgroundCheck_DoWork;

      bg.RunWorkerCompleted += backgroundCheck_Completed;
      bg.RunWorkerAsync();


    }

    public void backgroundCheck_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        e.Result = CheckLatestRelease();
      }
      catch (Exception) { e.Result = null; }

    }

    public void backgroundCheck_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Result != null)
        showBalloon(this._appInterface.GetAppName(), "There is a new version available.\nClick here for more info.");
    }


    private void showBalloon(string title, string body)
    {

           
            if (_appInterface.GetPlatform() == "Windows")
            {
              _notifyIcon = new NotifyIcon();
              _notifyIcon.Visible = true;
              _notifyIcon.BalloonTipTitle = title;
              _notifyIcon.BalloonTipText = body;
              _notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;
              _notifyIcon.Icon = this._appInterface.getApplicationIcon();
              _notifyIcon.ShowBalloonTip(30000);
            } else
       
      new ShortMessage(title, body, NotifyIcon_BalloonTipClicked).Show();
    }

    private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
    {
      _notifyIcon.Visible = false;
     
      new UpdateAppLibrary.NewRelease(_appInterface, this).Show();
    }

    public void OpenNewReleaseForm()
    {
      NewRelease releaseForm = new NewRelease(_appInterface, this);
      releaseForm.Show();
    }

    private static string HTTPRequest(string url)
    {
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

      using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
      using (Stream stream = response.GetResponseStream())
      using (StreamReader reader = new StreamReader(stream)) {
        return reader.ReadToEnd();
      }
    }


    // Deserialize a JSON stream to a User object.
    public List<YAppRelease> CheckAllNewRelease()
    {
#if NET35
#warning Check for new version in not supported with .NET Framework 3.5 (update to .NET Framework 4.0) 
            return new List<YAppRelease>();
#else
      int curVersion = _svnVersion;
      if (curVersion < _appInterface.GetBuildNumberToIgnoreSettings()) {
        curVersion = _appInterface.GetBuildNumberToIgnoreSettings();
      }

      string appname = _appName.Replace("-", "");
      string request = String.Format("{0}/FR/common/getLastFirmwareLink.php?app={1}&version={2}&platform={3}", SERVER_NAME, appname, curVersion, _platform);
      string json = HTTPRequest(request);

      var instance = typeof(List<YAppRelease>);
      List<YAppRelease> newReleases = new List<YAppRelease>();

      MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
      DataContractJsonSerializer ser = new DataContractJsonSerializer(instance);
      newReleases = ser.ReadObject(ms) as List<YAppRelease>;
      ms.Close();
      return newReleases;
#endif
    }

    public YAppRelease CheckLatestRelease()
    {
      List<YAppRelease> allNewRelease = CheckAllNewRelease();
      if (allNewRelease.Count > 0) {
        return allNewRelease[0];
      }

      return null;
    }

    internal string GetReleaseNotes()
    {
      //fixme handle other applications:
      if (_appName == "VirtualHub") {
        return SERVER_NAME + "/EN/virtualhub.php";
      } else {
        return SERVER_NAME + "/EN/tools.php";
      }
    }

    internal string DownloadInstaller(string msi_url, object progressChanged)
    {
      int lastIndexOf = msi_url.LastIndexOf("/");
      string filename = msi_url.Substring(lastIndexOf + 1);
      string msi_temp_file = Path.Combine(Path.GetTempPath(), filename);
      System.Net.WebClient Client = new WebClient();
      Client.DownloadFile(msi_url, msi_temp_file);
      return msi_temp_file;
    }

    internal string DownloadInstaller(string msi_url, DownloadProgressChangedEventHandler DownloadProgressChanged, AsyncCompletedEventHandler DownloadFileCompleted)
    {
      
      int lastIndexOf = msi_url.LastIndexOf("/");
      string filename = msi_url.Substring(lastIndexOf + 1);
      string msi_temp_file = Path.Combine(Path.GetTempPath(), filename);
      _client = new WebClient();
      _client.DownloadFileCompleted += DownloadFileCompleted;
      _client.DownloadProgressChanged += DownloadProgressChanged;
      _client.DownloadFileAsync(new Uri(msi_url), msi_temp_file);
      return msi_temp_file;

    }


    public void  CancelDownload()
    {   if ( _client!=null) _client.CancelAsync(); }

#if (!NET40 && !NET35)


    public async Task StartPeriodicTask(Action action, TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) {
                await Task.Delay(period, cancellationToken);

                if (_appInterface.GetCheckUpdateSettings()) {
                    Debug.WriteLine("Check for update");
                    YAppRelease release = CheckLatestRelease();
                    if (release != null) {
                        action();
                    }
                }
            }
        }
#endif

        internal string FormatVersion(int buildNumber)
        {
            return String.Format("{0}.{1}", _versionPrefix, buildNumber);
        }
    }



  public interface YAppInterface
  {
    // return the full version string (1.10.1234)
    string GetVersion();
    // function called just before starting the MSI
    void StopRunningProcess();
    // return the platform (Windows, Linux, Mac-OS-X, MSI)
    string GetPlatform();
    // return the application name (Yocto-Visualization,....)
    string GetAppName();
    // setter and getter for the settings parameters        
    // GetCheckUpdateSettings must return the boolean settings
    bool GetCheckUpdateSettings();
    // SetBuildNumberToIgnoreSettings must save the value in the settings 
    void SetCheckUpdateSettings(bool newval);
    // GetCheckUpdateSettings must return the integer value 
    int GetBuildNumberToIgnoreSettings();
    // SetBuildNumberToIgnoreSettings must save the value in the settings 
    void SetBuildNumberToIgnoreSettings(int buildnumber);
    // returns the application icon
    Icon getApplicationIcon();
  }

}