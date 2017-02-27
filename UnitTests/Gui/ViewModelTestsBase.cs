using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Gui
{
    public class ViewModelTestsBase
    {
        protected static void DeleteAppSettingsFile()
        {
            var appSettingsFilePath
                = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

            if (File.Exists(appSettingsFilePath))
                File.Delete(appSettingsFilePath);
        }
    }
}
