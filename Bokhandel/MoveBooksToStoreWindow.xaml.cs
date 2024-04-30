using System.Windows;
using System.Windows.Controls;
using Labb2_DbFirst_Template.DataAcess;
using Microsoft.EntityFrameworkCore;

namespace Bokhandel
{
    /// <summary>
    /// Interaction logic for MoveBooksToStoreWindow.xaml
    /// </summary>
    public partial class MoveBooksToStoreWindow : Window
    {
        private BokhandelContext db = new BokhandelContext();

        public MoveBooksToStoreWindow()
        {
            InitializeComponent();

            foreach (var store in db.Butikers)
            {
                FromStoreListView.Items.Add(store);
                ToStoreListView.Items.Add(store);
            }
        }

        private void FromStoreListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedStore = FromStoreListView.SelectedItem;

            if (selectedStore != null)
            {
                var storage = db.LagerSaldos
                    .Include(ls => ls.IsbnNavigation)
                    .Where(b => b.Butik == selectedStore)
                    .ToList();

                BookListView.ItemsSource = storage;
            }
        }

        private void ToStoreListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
