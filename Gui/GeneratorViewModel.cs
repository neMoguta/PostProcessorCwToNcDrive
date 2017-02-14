using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Gui
{
    public class GeneratorViewModel : DependencyObject
    {
        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TestProperty1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(GeneratorViewModel), new PropertyMetadata("", FilterTextChanged));

        private static void FilterTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var generatorViewModel = d as GeneratorViewModel;
            if (generatorViewModel != null)
            {
                generatorViewModel.Items.Filter = null;
                generatorViewModel.Items.Filter = generatorViewModel.DoNotDeleteItemFromItems;
            }
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
            Items = CollectionViewSource.GetDefaultView(Person.GetPersons());
            Items.Filter = DoNotDeleteItemFromItems;
        }

        private bool DoNotDeleteItemFromItems(object obj)
        {
            var currentPerson = obj as Person;

            if (currentPerson == null)
                return false;

            if (string.IsNullOrWhiteSpace(FilterText))
                return true;

            return currentPerson.FirstName.ToLower().Contains(FilterText.ToLower()) ||
                currentPerson.LastName.ToLower().Contains(FilterText.ToLower());
        }
    }
}
