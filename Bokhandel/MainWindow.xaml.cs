using System.Windows;
using System.Windows.Controls;
using Labb2_DbFirst_Template.DataAcess;
using Microsoft.EntityFrameworkCore;

namespace Bokhandel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BokhandelContext db = new();

        public MainWindow()
        {
            InitializeComponent();

            foreach (var store in db.Butikers)
            {
                StoreListView.Items.Add(store);
            }

            foreach (var books in db.Böckers)
            {
                BookComboBox.Items.Add(books);
            }
        }

        public void BookInformation(Böcker selectedBook)
        {
            if (selectedBook != null)
            {

                    var authors = db.BokDetaljers
                    .Where(a => a.Isbn == selectedBook.Isbn13)
                    .Join(db.Författares, a => a.FörfattareId, author => author.Id,
                        (a, author) => $"{author.Förnamn} {author.Efternamn}")
                    .ToList();

                    var author = string.Join(", ", authors);

                    var bookInformation = db.BokDetaljers
                        .Include(a => a.Författare)
                        .Include(b => b.IsbnNavigation)
                        .Include(g => g.Genre)
                        .Include(p => p.Förlag)
                        .FirstOrDefault(bi => bi.Isbn == selectedBook.Isbn13);


                BookDetailsTextBlock.Text = $"ISBN: {selectedBook.Isbn13}\n\nTitel: {selectedBook.Titel}\n" +
                                                $"Författare: {author}\n\nPris: {bookInformation.IsbnNavigation.Pris}kr\n\nGenre: {bookInformation.Genre.Genre1}\n" +
                                                $"Språk: {bookInformation.IsbnNavigation.Språk}\n\nFörlag: {bookInformation.Förlag.Namn}\n" +
                                                $"Utgivningsdatum: {bookInformation.IsbnNavigation.Utgivningsdatum}";
            }
        }

        private void StoreListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedStore = StoreListView.SelectedItem;

            if (selectedStore != null)
            {
                var storage = db.LagerSaldos
                    .Include(ls => ls.IsbnNavigation)
                    .Where(b => b.Butik == selectedStore)
                    .ToList();

                StorageListView.ItemsSource = storage;
            }
        }

        private void StorageListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StorageListView.SelectedItem is LagerSaldo selectedLagerSaldo)
            {
                foreach (var book in BookComboBox.Items)
                {
                    if (book is Böcker böcker && selectedLagerSaldo.Isbn == böcker.Isbn13)
                    {
                        BookComboBox.SelectedItem = book;
                        break;
                    }
                }
            }
        }

        private void BookComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BookComboBox.SelectedItem is Böcker selectedBookFromComboBox && selectedBookFromComboBox != null)
            {
                BookInformation(selectedBookFromComboBox);
            }
        }

        private void AddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (BookComboBox.SelectedItem is Böcker selectedBook)
            {
                if (StoreListView.SelectedItem is Butiker selectedStore)
                {
                    bool isValidInput = false;
                    int quantity = 0;

                    do
                    {
                        string input = Microsoft.VisualBasic.Interaction.InputBox(
                            $"Butik: {selectedStore.Id} - {selectedStore.Butiksnamn}\n" +
                            $"{selectedStore.Adress}\n{selectedStore.Postadress} {selectedStore.Stad}\n{selectedStore.Land}\n\n" +
                            $"ISBN: {selectedBook.Isbn13} - {selectedBook.Titel}\n\n" +
                            $"Fyll in ett antal", "Lägg till bok");

                        if (string.IsNullOrWhiteSpace(input))
                        {
                            MessageBox.Show("Avbruten");
                            break;
                        }

                        if (int.TryParse(input, out quantity) && quantity > 0)
                        {
                            isValidInput = true;
                        }
                        else
                        {
                            MessageBox.Show("Ogiltigt antal.");
                        }



                    } while (!isValidInput);

                    var existingEntry = db.LagerSaldos.FirstOrDefault(entry =>
                        entry.Isbn == selectedBook.Isbn13 && entry.ButikId == selectedStore.Id);

                    if (existingEntry != null)
                    {
                        existingEntry.Antal += quantity;
                    }
                    else
                    {
                        var newEntry = new LagerSaldo
                        {
                            Isbn = selectedBook.Isbn13,
                            ButikId = selectedStore.Id,
                            Antal = quantity
                        };

                        db.LagerSaldos.Add(newEntry);
                    }

                    db.SaveChanges();

                    StoreListView_OnSelectionChanged(null, null);
                }
            }
        }

        private void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedEntry = StorageListView.SelectedItem;

            if (StoreListView.SelectedItem is Butiker selectedStore)
            {
                if (selectedEntry is LagerSaldo selectedBook)
                {
                    var existingEntry = db.LagerSaldos.FirstOrDefault(entry =>
                        entry.Isbn == selectedBook.Isbn && entry.ButikId == selectedStore.Id);

                    string title = db.Böckers
                        .Where(b => b.Isbn13 == existingEntry.Isbn)
                        .Select(b => b.Titel)
                        .FirstOrDefault();

                    MessageBoxResult result = MessageBox.Show($"ISBN: {existingEntry.Isbn}\n" +
                                                              $"Titel: {title}\n\n" +
                                                              $"Vill du ta bort boken från butikens lager?", "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        db.LagerSaldos.Remove(existingEntry);
                    }
                }

                db.SaveChanges();

                StoreListView_OnSelectionChanged(null, null);
            }
        }

        private void MoveBookBtn_OnClick(object sender, RoutedEventArgs e)
        {
            MoveBooksToStoreWindow moveBooks = new MoveBooksToStoreWindow();
            moveBooks.Show();
        }

        private void StorageBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ManageTitlesWindow titlesWindow = new ManageTitlesWindow();
            titlesWindow.Show();
        }

        private void StoreBtn_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}