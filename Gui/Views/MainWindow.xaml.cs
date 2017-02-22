using System.Windows;

namespace PostProcessorGui.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Loaded += MainWindowLoaded;
        }

        //private void MainWindowLoaded(object sender, RoutedEventArgs e)
        //{
        //    DataContext = new GeneratorViewModel();
        //}
    }
}
