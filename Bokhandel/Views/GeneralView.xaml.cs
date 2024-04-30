using Labb2_DbFirst_Template.DataAcess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;

namespace Bokhandel.Views
{
    /// <summary>
    /// Interaction logic for GeneralView.xaml
    /// </summary>
    public partial class GeneralView : UserControl
    {
        private BokhandelContext db = new();
        public GeneralView()
        {
            InitializeComponent();

            var books = db.Böckers.OrderBy(book => book.Titel).ToList();
            foreach (var book in books)
            {
                BookComboBox.Items.Add(book);
            }

            var authors = db.Författares.OrderBy(author => author.Förnamn).ToList();
            foreach (var author in authors)
            {
                AuthorComboBox.Items.Add(author);
            }

            var genres = db.Genres.OrderBy(genre => genre.Genre1).ToList();
            foreach (var genre in genres)
            {
                GenreComboBox.Items.Add(genre);
            }

            var publishers = db.Förlags.OrderBy(publisher => publisher.Namn).ToList();
            foreach (var publisher in publishers)
            {
                PublisherComboBox.Items.Add(publisher);
            }
        }

        private void BookInformation(Böcker selectedBook)
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


                BookInformationtextBlock.Text = $"ISBN: {selectedBook.Isbn13}\n\nTitel: {selectedBook.Titel}\n" +
                                            $"Författare: {author}\n\nPris: {bookInformation.IsbnNavigation.Pris}kr\n\nGenre: {bookInformation.Genre.Genre1}\n" +
                                            $"Språk: {bookInformation.IsbnNavigation.Språk}\n\nFörlag: {bookInformation.Förlag.Namn}\n" +
                                            $"Utgivningsdatum: {bookInformation.IsbnNavigation.Utgivningsdatum}";

                var storeInfo = db.LagerSaldos
                    .Where(entry => entry.Isbn == selectedBook.Isbn13)
                    .Join(db.Butikers, entry => entry.ButikId, store => store.Id,
                        (entry, store) => new { StoreId = store.Id, Quantity = entry.Antal })
                    .ToList();

                var storeInfoText = string.Join("\n", storeInfo.Select(info =>
                    $"ButiksID: {info.StoreId} - Antal: {info.Quantity}"));

                StorageInformationTextBlock.Text = $"Lagerinformation för {selectedBook.Isbn13}:\n\n{storeInfoText}";

                IsbnTextBox.Text = selectedBook.Isbn13;
                TitleTextBox.Text = selectedBook.Titel;
                LanguageTextBox.Text = bookInformation.IsbnNavigation.Språk;
                PriceTextBox.Text = bookInformation.IsbnNavigation.Pris.ToString();

                DateTime? releasedateTime = bookInformation.IsbnNavigation.Utgivningsdatum.HasValue ? new DateTime(bookInformation.IsbnNavigation.Utgivningsdatum.Value.Year, bookInformation.IsbnNavigation.Utgivningsdatum.Value.Month, bookInformation.IsbnNavigation.Utgivningsdatum.Value.Day)
                    : null;
                ReleaseDateDatePicker.SelectedDate = releasedateTime;

                GenreTextBox.Text = bookInformation.Genre.Genre1;

                foreach (var item in GenreComboBox.Items)
                {
                    if (item is Genre genreItem && genreItem.Genre1 == GenreTextBox.Text)
                    {
                        GenreComboBox.SelectedItem = item;
                        break;
                    }
                }

                AuthorNameTextBox.Text = bookInformation.Författare.Förnamn;
                AuthorLastnameTextBox.Text = bookInformation.Författare.Efternamn;
                AuthorBirthdateDatePicker.SelectedDate = bookInformation.Författare.Födelsedatum.HasValue
                    ? new DateTime(bookInformation.Författare.Födelsedatum.Value.Year, bookInformation.Författare.Födelsedatum.Value.Month, bookInformation.Författare.Födelsedatum.Value.Day)
                    : null;

                foreach (var item in AuthorComboBox.Items)
                {
                    if (item is Författare authorsItem && authorsItem.Förnamn == AuthorNameTextBox.Text)
                    {
                        AuthorComboBox.SelectedItem = item;
                        break;
                    }
                }

                PublisherNameTextBox.Text = bookInformation.Förlag.Namn;
                PublisherSiteTextBox.Text = bookInformation.Förlag.Webbadress;
                PublisherCityTextBox.Text = bookInformation.Förlag.Stad;
                PublisherCountryTextBox.Text = bookInformation.Förlag.Land;

                foreach (var item in PublisherComboBox.Items)
                {
                    if (item is Förlag publisherItem && publisherItem.Namn == PublisherNameTextBox.Text)
                    {
                        PublisherComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void BookComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BookComboBox.SelectedItem is Böcker selectedBookFromComboBox)
            {
                BookInformation(selectedBookFromComboBox);
            }
        }

        private void AuthorComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AuthorComboBox.SelectedItem is Författare selectedAuthorFromComboBox)
            {
                if (selectedAuthorFromComboBox != null)
                {
                    
                    var titles = db.TitlarPerFörfattares
                            .Where(t => t.Namn == $"{selectedAuthorFromComboBox.Förnamn} {selectedAuthorFromComboBox.Efternamn}")
                            .Select(t => t.Titlar)
                            .FirstOrDefault();

                    var storageValue = db.TitlarPerFörfattares
                            .Where(sv => sv.Namn == $"{selectedAuthorFromComboBox.Förnamn} {selectedAuthorFromComboBox.Efternamn}")
                            .Select(sv => sv.Lagervärde)
                            .FirstOrDefault();

                    AuthorInformationTextBlock.Text =
                        $"Förnamn: {selectedAuthorFromComboBox.Förnamn}\nEfternamn: {selectedAuthorFromComboBox.Efternamn}\n\n" +
                        $"Födelsedatum: {selectedAuthorFromComboBox.Födelsedatum}\n\n" +
                        $"Titlar: {titles}\n" +
                        $"Lagervärd: {storageValue}kr";

                    AuthorNameTextBox.Text = selectedAuthorFromComboBox.Förnamn;
                    AuthorLastnameTextBox.Text = selectedAuthorFromComboBox.Efternamn;

                    DateTime? birthdateDateTime = selectedAuthorFromComboBox.Födelsedatum.HasValue
                        ? new DateTime(selectedAuthorFromComboBox.Födelsedatum.Value.Year, selectedAuthorFromComboBox.Födelsedatum.Value.Month, selectedAuthorFromComboBox.Födelsedatum.Value.Day) : null;

                    AuthorBirthdateDatePicker.SelectedDate = birthdateDateTime;
                }
            }
        }

        private void PublisherComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PublisherComboBox.SelectedItem is Förlag selectedPublisherFromComboBox)
            {
                if (selectedPublisherFromComboBox != null)
                {
                    PublisherTextBlock.Text =
                        $"Förlagsnamn: {selectedPublisherFromComboBox.Namn}\n\n" +
                        $"Webbadress:\n{selectedPublisherFromComboBox.Webbadress}\n\n" +
                        $"Stad: {selectedPublisherFromComboBox.Stad}\n" +
                        $"Land: {selectedPublisherFromComboBox.Land}";

                    PublisherNameTextBox.Text = selectedPublisherFromComboBox.Namn;
                    PublisherSiteTextBox.Text = selectedPublisherFromComboBox.Webbadress;
                    PublisherCityTextBox.Text = selectedPublisherFromComboBox.Stad;
                    PublisherCountryTextBox.Text = selectedPublisherFromComboBox.Land;
                }
            }
        }

        private void GenreComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GenreComboBox.SelectedItem is Genre selectedGenreFromComboBox)
            {
                GenreTextBox.Text = selectedGenreFromComboBox.Genre1;
            }
        }

        private void ResetBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ClearAllTextBoxes();

            BookInformationtextBlock.Text = "";
            AuthorInformationTextBlock.Text = "";
            PublisherTextBlock.Text = "";

            BookComboBox.SelectedItem = null;
            AuthorComboBox.SelectedItem = null;
            PublisherComboBox.SelectedItem = null;
            GenreComboBox.SelectedItem = null;

            ReleaseDateDatePicker.SelectedDate = null;
            AuthorBirthdateDatePicker.SelectedDate = null;
        }

        private void ClearAllTextBoxes()
        {
            foreach (var control in GetAllControls(this))
            {

                if (control is TextBox textBox)
                {
                    textBox.Clear();
                }
            }
        }

        private IEnumerable<Control> GetAllControls(DependencyObject parent)
        {
            var controls = new List<Control>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Control control)
                {
                    controls.Add(control);
                }

                controls.AddRange(GetAllControls(child));
            }

            return controls;
        }
    }
    
}
