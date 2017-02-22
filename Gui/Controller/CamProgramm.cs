using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PostProcessor.IncomeDataParser;
using PostProcessorGui.ViewModels;

namespace PostProcessorGui.Controller
{
    internal class CamProgramm : DependencyObject
    {
        public string OperationName { get; set; }

        //public bool IsOperationOn
        //{
        //    get { return (bool)GetValue(IsOperationOnProperty); }
        //    set { SetValue(IsOperationOnProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsOperationOn.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsOperationOnProperty =
        //    DependencyProperty.Register("IsOperationOn", typeof(bool), typeof(GeneratorViewModel), new PropertyMetadata(true, CheckBoxchanged));

        //private static void CheckBoxchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var generatorViewModel = d as GeneratorViewModel;
        //    if (generatorViewModel != null)
        //    {
        //        generatorViewModel.Items.Refresh();
        //    }
        //}

        public bool IsOperationOn { get; set; }
        public string OperationData { get; set; }

        public static CamProgramm[] GetOperations(string source)
        {
            Parser parser = new Parser();
            var blocks = parser.GetMillOperations(source).ToList();

            var result = new List<CamProgramm>();

            blocks.ForEach((b) =>
            {
                result.Add(new CamProgramm{OperationName = b.Name, IsOperationOn = true, OperationData = b.Data});
            });

            return result.ToArray();
        }
    }
}
