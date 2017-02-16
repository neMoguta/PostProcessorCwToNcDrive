using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using Gui.Utils;
using Microsoft.Win32;

namespace Gui
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
            DependencyProperty.Register("CamProgrammText", typeof(string), typeof(GeneratorViewModel), new PropertyMetadata(""));   

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
            Items = CollectionViewSource.GetDefaultView(CamProgramm.GetPersons());
            Items.Filter = ItemsFilter;
        }

        /// <summary>
        ///  Do not filter if true
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool ItemsFilter(object obj)
        {
            var currentPerson = obj as CamProgramm;

            if (currentPerson == null)
                return false;

            if (string.IsNullOrWhiteSpace(TextFilter))
                return true;

            return currentPerson.OperationName.ToLower().Contains(TextFilter.ToLower()) ||
                currentPerson.OperationParams.ToLower().Contains(TextFilter.ToLower());
        }

        private DelegateCommand _saveFileCommand;

        public DelegateCommand SaveFileCommand
        {
            get { return _saveFileCommand ?? (_saveFileCommand = new DelegateCommand(SaveFile)); }
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
        
        private DelegateCommand _addCamFileCommand;
        public DelegateCommand AddCamFileCommand
        {
            get
            {
                return _addCamFileCommand ?? (_addCamFileCommand = new DelegateCommand(AddCamFile));
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
        }
    }
}
