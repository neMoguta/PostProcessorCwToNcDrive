using System.Windows;
using PostProcessorGui.Utils;

namespace PostProcessorGui.ViewModels
{
    public class SettingsViewModel : DependencyObject
    {
        public SettingsViewModel()
        {
            AddM00AtTheEnd = Properties.Settings.Default.SettingsAddM00AtTheEnd;
            AutoGenHotkeyOn = Properties.Settings.Default.SettingsEnableHotkeyGeneration;
            AddCustomCommandAtOperationEnd = Properties.Settings.Default.SettingsAddCustomAtTheEnd;
            CustomCommands = Properties.Settings.Default.SettingsCustomCommands;
        }

        public bool AddM00AtTheEnd
        {
            get { return (bool)GetValue(AddM00AtTheEndProperty); }
            set { SetValue(AddM00AtTheEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddM00AtTheEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddM00AtTheEndProperty =
            DependencyProperty.Register("AddM00AtTheEnd", typeof(bool), typeof(SettingsViewModel),
                new PropertyMetadata(Properties.Settings.Default.SettingsAddM00AtTheEnd, SaveAddM00AtTheEnd));

        private static void SaveAddM00AtTheEnd(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var settings = (d as SettingsViewModel);
            if(settings==null) return;
            Properties.Settings.Default.SettingsAddM00AtTheEnd = settings.AddM00AtTheEnd;
        }

        public bool AddCustomCommandAtOperationEnd
        {
            get { return (bool)GetValue(AddCustomCommandAtOperationEndProperty); }
            set { SetValue(AddCustomCommandAtOperationEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddCustomCommandAtOperationEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCustomCommandAtOperationEndProperty =
            DependencyProperty.Register("AddCustomCommandAtOperationEnd", typeof(bool), typeof(SettingsViewModel),
                new PropertyMetadata(Properties.Settings.Default.SettingsAddCustomAtTheEnd, SaveAddCustomCommandAtOpEnd));

        private static void SaveAddCustomCommandAtOpEnd(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var settings = (d as SettingsViewModel);
            if (settings == null) return;
            Properties.Settings.Default.SettingsAddCustomAtTheEnd = settings.AddCustomCommandAtOperationEnd;
        }

        public bool AutoGenHotkeyOn
        {
            get { return (bool)GetValue(AutoGenHotkeyOnProperty); }
            set { SetValue(AutoGenHotkeyOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoGenHotkeyOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoGenHotkeyOnProperty =
            DependencyProperty.Register("AutoGenHotkeyOn", typeof(bool), typeof(SettingsViewModel),
                new PropertyMetadata(Properties.Settings.Default.SettingsEnableHotkeyGeneration, SaveAutoHenHotkeyOn));

        private static void SaveAutoHenHotkeyOn(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var settings = (d as SettingsViewModel);
            if (settings == null) return;
            Properties.Settings.Default.SettingsEnableHotkeyGeneration = settings.AutoGenHotkeyOn;
        }
        
        public string CustomCommands
        {
            get { return (string)GetValue(CustomCommandsProperty); }
            set { SetValue(CustomCommandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomCommands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomCommandsProperty =
            DependencyProperty.Register("CustomCommands", typeof(string), typeof(SettingsViewModel),
                new PropertyMetadata(Properties.Settings.Default.SettingsCustomCommands, SaveCustomCommand));

        private static void SaveCustomCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var settings = (d as SettingsViewModel);
            if (settings == null) return;
            Properties.Settings.Default.SettingsCustomCommands = settings.CustomCommands;
        }

        private RelayCommand _windowClosingCommand;
        public RelayCommand WindowClosingCommand
        {
            get { return _windowClosingCommand ?? (_windowClosingCommand = new RelayCommand(Closing)); }
        }
        private void Closing(object obj)
        {
            Properties.Settings.Default.Save();
        }
    }
}
