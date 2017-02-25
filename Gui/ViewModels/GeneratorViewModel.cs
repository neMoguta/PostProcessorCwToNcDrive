using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.Win32;
using PostProcessor.CodeGenerator;
using PostProcessor.IncomeDataParser;
using PostProcessorGui.Controller;
using PostProcessorGui.Utils;
using PostProcessorGui.Views;

namespace PostProcessorGui.ViewModels
{
    public class GeneratorViewModel : DependencyObject
    {
        public string NcDriveProgrammText
        {
            get { return (string)GetValue(NcDriveProgrammTextProperty); }
            set { SetValue(NcDriveProgrammTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NcDriveProgrammText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NcDriveProgrammTextProperty =
            DependencyProperty.Register("NcDriveProgrammText", typeof(string), typeof(GeneratorViewModel), new PropertyMetadata(""));

        public string CamProgrammText
        {
            get { return (string)GetValue(CamProgrammTextProperty); }
            set { SetValue(CamProgrammTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CamProgrammText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CamProgrammTextProperty =
            DependencyProperty.Register("CamProgrammText", typeof(string), typeof(GeneratorViewModel), new PropertyMetadata("", CamProgrammLoaded));

        private static void CamProgrammLoaded(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var generatorViewModel = d as GeneratorViewModel;
            if (generatorViewModel == null) return;

            // reserved
        }

        public string TextFilter
        {
            get { return (string)GetValue(TextFilterProperty); }
            set { SetValue(TextFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TestProperty1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFilterProperty =
            DependencyProperty.Register("TextFilter", typeof(string), typeof(GeneratorViewModel), new PropertyMetadata("", FilterTextChanged));

        private static void FilterTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var generatorViewModel = d as GeneratorViewModel;
            if (generatorViewModel == null) return;

            generatorViewModel.Items.Filter = null;
            generatorViewModel.Items.Filter = generatorViewModel.ItemsFilter;
        }

        public ICollectionView Items
        {
            get { return (ICollectionView)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ICollectionView), typeof(GeneratorViewModel), new PropertyMetadata(null));

        public GeneratorViewModel()
        {
            Items = CollectionViewSource.GetDefaultView(CamProgramm.GetOperations(CamProgrammText));
            Items.Filter = ItemsFilter;
        }
        /// <summary>
        ///  Do not filter if true
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ItemsFilter(object obj)
        {
            var operation = obj as CamProgramm;

            if (operation == null)
                return false;

            if (string.IsNullOrWhiteSpace(TextFilter))
                return true;

            return operation.OperationName.ToLower().Contains(TextFilter.ToLower());
        }

        private RelayCommand _saveFileCommand;
        public RelayCommand SaveFileCommand
        {
            get { return _saveFileCommand ?? (_saveFileCommand = new RelayCommand(SaveFile)); }
        }
        private void SaveFile(object obj)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "NcDrive programm (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = "c:\\temp"
            };
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, NcDriveProgrammText);
            }
        }

        public bool NcDriveSelected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Selected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("NcDriveSelected", typeof(bool), typeof(GeneratorViewModel), new PropertyMetadata(false, TabNcDriveOpen));

        private static void TabNcDriveOpen(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var generatorViewModel = d as GeneratorViewModel;
            if (generatorViewModel == null) return;

            generatorViewModel.NcDriveProgrammText = GenerateNcDriveProgram(generatorViewModel);
        }

        private static string GenerateNcDriveProgram(GeneratorViewModel generatorViewModel)
        {
            var buffer = new Queue<Command>();

            foreach (var item in generatorViewModel.Items)
            {
                var camData = item as CamProgramm;
                if (camData == null)
                    throw new ArgumentNullException(@"CamProgramm is null");
                if (!camData.IsOperationOn)
                    continue;

                var blockLines = camData.OperationData.Split(new[] { "\r\n" }, StringSplitOptions.None);

                var blockInstructions = new Parser().GetInstructions(blockLines);
                foreach (var instruction in blockInstructions)
                {
                    buffer.Enqueue(instruction);
                }
            }

            var gen = new Generator(
                Properties.Settings.Default.SettingsAddM00AtTheEnd,
                Properties.Settings.Default.SettingsAddCustomAtTheEnd,
                Properties.Settings.Default.SettingsCustomCommands);

            var result = gen.GenerateMillProgramm(buffer);
            var resultProgramm = result.Aggregate((x, y) => x + Environment.NewLine + y);
            return resultProgramm;
        }

        private RelayCommand _aboutWindowOpenCommand;
        public RelayCommand AboutWindowDelegateCommand
        {
            get { return _aboutWindowOpenCommand ?? (_aboutWindowOpenCommand = new RelayCommand(AboutWindowOpen)); }
        }
        private void AboutWindowOpen(object obj)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = Application.Current.MainWindow;
            aboutWindow.ShowDialog();
        }

        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand
        {
            get
            {
                return _settingsCommand ?? (_settingsCommand = new RelayCommand(OpenSettings));
            }
        }
        private void OpenSettings(object obj)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = Application.Current.MainWindow;
            settingsWindow.ShowDialog();
        }

        private RelayCommand _navigateToYTubeCommand;

        public RelayCommand NavigateToYTubeCommand
        {
            get
            {
                return _navigateToYTubeCommand ?? (_navigateToYTubeCommand = new RelayCommand(NavigateToYTube));
            }
        }

        private void NavigateToYTube(object obj)
        {
            System.Diagnostics.Process.Start(
                new Uri(@"https://youtu.be/YgNbZjzj8Jc").ToString());
        }

        private RelayCommand _navigateCommand;
        public RelayCommand NavigateCommand
        {
            get
            {
                return _navigateCommand ?? (_navigateCommand = new RelayCommand(Navigate));
            }
        }
        private void Navigate(object obj)
        {
            System.Diagnostics.Process.Start(
                new Uri(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4W7P2W2ZQ4RE4").ToString());
        }

        private RelayCommand _addCamFileCommand;
        public RelayCommand AddCamFileCommand
        {
            get
            {
                return _addCamFileCommand ?? (_addCamFileCommand = new RelayCommand(AddCamFile));
            }
        }
        private void AddCamFile(object arg)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\Temp",
                Filter = "All files (*.*)|*.*|CamWorks programms(*.clt)|*.clt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(dialog.FileName);
                using (StreamReader sr = fileInfo.OpenText())
                {
                    CamProgrammText = sr.ReadToEnd();
                }
            }

            Items = CollectionViewSource.GetDefaultView(CamProgramm.GetOperations(CamProgrammText));
        }
    }
}
