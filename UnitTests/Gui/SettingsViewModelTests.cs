using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostProcessorGui.ViewModels;

namespace UnitTests.Gui
{
    [TestClass]
    public class SettingsViewModelTests : ViewModelTestsBase
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DeleteAppSettingsFile();
        }

        [ClassCleanup]
        public static void ClassCleunup()
        {
            DeleteAppSettingsFile();
        }

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CheckDefaultSettingsTest()
        {
            var settingsModel = new SettingsViewModel();

            Assert.AreEqual(settingsModel.AddM00AtTheEnd, true);
            Assert.AreEqual(settingsModel.AddCustomCommandAtOperationEnd, false);
            Assert.AreEqual(settingsModel.AutoGenHotkeyOn, false);
            Assert.AreEqual(settingsModel.CustomCommands, "");
        }

        [TestMethod]
        public void CheckCustomSettingsTest()
        {
            var addM00AtTheEnd = false;
            var addCustomCommandAtOperationEnd = false;
            var autoGenHotkeyOn = false;
            var testCommand = "test command";

            var settingsModel = new SettingsViewModel
            {
                AddM00AtTheEnd = addM00AtTheEnd,
                AddCustomCommandAtOperationEnd = addCustomCommandAtOperationEnd,
                AutoGenHotkeyOn = autoGenHotkeyOn,
                CustomCommands = testCommand
            };

            Assert.AreEqual(settingsModel.AddM00AtTheEnd, addM00AtTheEnd);
            Assert.AreEqual(settingsModel.AddCustomCommandAtOperationEnd, addCustomCommandAtOperationEnd);
            Assert.AreEqual(settingsModel.AutoGenHotkeyOn, autoGenHotkeyOn);
            Assert.AreEqual(settingsModel.CustomCommands, testCommand);
        }
    }
}
