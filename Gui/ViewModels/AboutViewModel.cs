using System;
using System.Windows;
using PostProcessorGui.Utils;

namespace PostProcessorGui.ViewModels
{
    class AboutViewModel : DependencyObject
    {
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

        private RelayCommand _navigateToDonateCommand;
        public RelayCommand NavigateToDonateCommand
        {
            get
            {
                return _navigateToDonateCommand ?? (_navigateToDonateCommand = new RelayCommand(NavigateToDonate));
            }
        }

        private void NavigateToDonate(object obj)
        {
            System.Diagnostics.Process.Start(
                new Uri(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4W7P2W2ZQ4RE4").ToString());
        }
    }
}
