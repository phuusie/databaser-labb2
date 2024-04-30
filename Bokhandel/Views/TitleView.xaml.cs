using System.Windows;
using Labb2_DbFirst_Template.DataAcess;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Bokhandel.Views
{
    /// <summary>
    /// Interaction logic for TitleView.xaml
    /// </summary>
    public partial class TitleView : UserControl
    {

        private BokhandelContext db = new BokhandelContext();

        public TitleView()
        {
            InitializeComponent();

            foreach (var genre in db.Genres)
            {
                GenreComboBox.Items.Add(genre);
            }

            foreach (var author in db.Författares)
            {
                AuthorComboBox.Items.Add(author);
            }

            foreach (var publisher in db.Förlags)
            {
                PublisherComboBox.Items.Add(publisher);
            }

            LoadBookDetails();

        }

        private void LoadBookDetails()
        {
            BooksListView.Items.Clear();

            foreach (var item in db.BokDetaljers
                         .Include(bd => bd.IsbnNavigation)
                         .Include(bd => bd.Författare)
                         .Include(bd => bd.Genre)
                         .Include(bd => bd.Förlag)
                         .ToList())
            {
                BooksListView.Items.Add(item);
            }


        }


        private void AddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var newBook = new Böcker
            {
                Isbn13 = IsbnTextBox.Text,
                Titel = TitleTextBox.Text,
                Språk = LanguageTextBox.Text,
                Pris = decimal.Parse(PriceTextBox.Text),
                Utgivningsdatum = DateOnly.FromDateTime(ReleaseDateDatePicker.SelectedDate.GetValueOrDefault())

            };

            db.Böckers.Add(newBook);
            db.SaveChanges();

            var bookDetails = new BokDetaljer
            {
                Isbn = IsbnTextBox.Text,
                FörfattareId = ((Författare)AuthorComboBox.SelectedItem).Id,
                FörlagId = ((Förlag)PublisherComboBox.SelectedItem).FörlagId,
                GenreId = ((Genre)GenreComboBox.SelectedItem).GenreId
            };

            db.BokDetaljers.Add(bookDetails);
            db.SaveChanges();

            BookInformationClear();

            LoadBookDetails();

        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (BooksListView.SelectedItem != null)
            {
                var selectedBookDetails = (BokDetaljer)BooksListView.SelectedItem;
                var bookToUpdate = db.Böckers.FirstOrDefault(b => b.Isbn13 == selectedBookDetails.Isbn);

                if (bookToUpdate != null)
                {
                    bookToUpdate.Titel = TitleTextBox.Text;
                    bookToUpdate.Språk = LanguageTextBox.Text;
                    bookToUpdate.Pris = decimal.Parse(PriceTextBox.Text);
                    bookToUpdate.Utgivningsdatum = DateOnly.FromDateTime(ReleaseDateDatePicker.SelectedDate.GetValueOrDefault());

                    selectedBookDetails.FörfattareId = ((Författare)AuthorComboBox.SelectedItem).Id;
                    selectedBookDetails.FörlagId = ((Förlag)PublisherComboBox.SelectedItem).FörlagId;
                    selectedBookDetails.GenreId = ((Genre)GenreComboBox.SelectedItem).GenreId;

                    db.SaveChanges();

                    MessageBox.Show("Informationen för boken har uppdaterats.");

                    LoadBookDetails();

                    BooksListView.SelectedItem = selectedBookDetails;
                }
            }
        }

        private void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (BooksListView.SelectedItem != null)
            {
                var selectedBookDetails = (BokDetaljer)BooksListView.SelectedItem;
                
                var bookToRemove = db.Böckers.FirstOrDefault(b => b.Isbn13 == selectedBookDetails.Isbn);

                var bookInStorage = db.LagerSaldos.FirstOrDefault(b => b.Isbn == selectedBookDetails.Isbn);

                if (bookToRemove != null)
                {
                    MessageBoxResult result =
                        MessageBox.Show(
                            $"Vill du ta bort titeln {selectedBookDetails.Isbn}: {selectedBookDetails.IsbnNavigation.Titel}",
                            "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        db.LagerSaldos.Remove(bookInStorage);

                        db.BokDetaljers.Remove(selectedBookDetails);

                        db.Böckers.Remove(bookToRemove);

                        db.SaveChanges();

                        LoadBookDetails();
                    }
                }
            }
        }

        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            BookInformationClear();
        }

        private void BookInformationClear()
        {
            IsbnTextBox.Text = string.Empty;
            TitleTextBox.Text = string.Empty;
            LanguageTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            ReleaseDateDatePicker.SelectedDate = null;

            AuthorComboBox.SelectedItem = null;
            PublisherComboBox.SelectedItem = null;
            GenreComboBox.SelectedItem = null;
        }

        private void BooksListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBook = (BokDetaljer)BooksListView.SelectedItem;

            if (selectedBook != null)
            {
                IsbnTextBox.Text = selectedBook.IsbnNavigation.Isbn13;
                TitleTextBox.Text = selectedBook.IsbnNavigation.Titel;
                LanguageTextBox.Text = selectedBook.IsbnNavigation.Språk;
                PriceTextBox.Text = selectedBook.IsbnNavigation.Pris.ToString();

                DateTime? releasedateTime = selectedBook.IsbnNavigation.Utgivningsdatum.HasValue 
                    ? new DateTime(selectedBook.IsbnNavigation.Utgivningsdatum.Value.Year, selectedBook.IsbnNavigation.Utgivningsdatum.Value.Month, selectedBook.IsbnNavigation.Utgivningsdatum.Value.Day)
                    : null;

                ReleaseDateDatePicker.SelectedDate = releasedateTime;

                AuthorComboBox.SelectedItem = selectedBook.Författare;
                PublisherComboBox.SelectedItem = selectedBook.Förlag;
                GenreComboBox.SelectedItem = selectedBook.Genre;
            }
        }
    }
}
