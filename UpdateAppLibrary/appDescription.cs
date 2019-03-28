
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using UpdateAppLibrary;

namespace YoctoVisualisation
{
  public class AppDescription : YAppInterface
  {
    // return the full version string (1.10.1234)
    string YAppInterface.GetVersion() { return "2." + constants.buildVersion; }
    // function called just before starting the MSI
    void YAppInterface.StopRunningProcess() { }
    // return the platform (Windows, Linux, Mac-OS-X, MSI)
    string YAppInterface.GetPlatform()
    {
      if (constants._OSX_Running) return "Mac-OS-X";
      if (constants.MonoRunning) return "Linux";
      return "Windows";
    }

    Icon YAppInterface.getApplicationIcon() { return Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); }

    // return the application name (Yocto-Visualization,....)
    string YAppInterface.GetAppName() { return "Yocto-Visualization"; }
    // setter and getter for the settings parameters        
    // GetCheckUpdateSettings must return the boolean settings
    bool YAppInterface.GetCheckUpdateSettings() { return constants.checkForUpdate; }
    // SetBuildNumberToIgnoreSettings must save the value in the settings 
    void YAppInterface.SetCheckUpdateSettings(bool newval) { constants.checkForUpdate = newval; }
    // GetCheckUpdateSettings must return the integer value 
    int YAppInterface.GetBuildNumberToIgnoreSettings() { return constants.updateIgnoreBuild; }
    // SetBuildNumberToIgnoreSettings must save the value in the settings 
    void YAppInterface.SetBuildNumberToIgnoreSettings(int buildnumber) { constants.updateIgnoreBuild = buildnumber; }

  }
}