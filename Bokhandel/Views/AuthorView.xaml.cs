using System.Windows;
using System.Windows.Controls;
using Labb2_DbFirst_Template.DataAcess;

namespace Bokhandel.Views
{
    /// <summary>
    /// Interaction logic for AuthorView.xaml
    /// </summary>
    public partial class AuthorView : UserControl
    {
        private BokhandelContext db = new();
        public AuthorView()
        {
            InitializeComponent();

            AuthorComboBoxLoad();
        }

        private void AuthorComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AuthorsBooksListView.Items.Clear();

            var selectedAuthor = (Författare)AuthorComboBox.SelectedItem;

            if (selectedAuthor != null)
            {
                var booksByAuthor = db.Böckers
                    .Where(book => book.BokDetaljers.Any(details => details.FörfattareId == selectedAuthor.Id))
                    .ToList();

                foreach (var authorBooks in booksByAuthor)
                {
                    AuthorsBooksListView.Items.Add(authorBooks);
                }

                NameTextBox.Text = selectedAuthor.Förnamn;
                LastnameTextBox.Text = selectedAuthor.Efternamn;

                DateTime? birthDate = selectedAuthor.Födelsedatum.HasValue
                    ? new DateTime(selectedAuthor.Födelsedatum.Value.Year, selectedAuthor.Födelsedatum.Value.Month, selectedAuthor.Födelsedatum.Value.Day)
                    : null;
                BirthdateDatePicker.SelectedDate = birthDate;
            }

        }

        private void AddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var newAuthor = new Författare
            {
                Förnamn = NameTextBox.Text,
                Efternamn = LastnameTextBox.Text,
                Födelsedatum = DateOnly.FromDateTime(BirthdateDatePicker.SelectedDate.GetValueOrDefault())
            };

            db.Författares.Add(newAuthor);
            db.SaveChanges();

            AuthorInformationClear();

            AuthorComboBoxLoad();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (AuthorComboBox.SelectedItem != null)
            {
                var selectedAuthor = (Författare)AuthorComboBox.SelectedItem;
                var authorToUpdate = db.Författares.FirstOrDefault(a => a.Id == selectedAuthor.Id);

                if (authorToUpdate != null)
                {

                    authorToUpdate.Förnamn = NameTextBox.Text;
                    authorToUpdate.Efternamn = LastnameTextBox.Text;
                    authorToUpdate.Födelsedatum =
                        DateOnly.FromDateTime(BirthdateDatePicker.SelectedDate.GetValueOrDefault());

                    db.SaveChanges();

                    MessageBox.Show("Författar informationen har uppdaterats.");

                    AuthorInformationClear();
                    AuthorComboBoxLoad();
                }
            }
        }

        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            AuthorInformationClear();
        }

        private void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (AuthorComboBox.SelectedItem != null)
            {
                var selectedAuthor = (Författare)AuthorComboBox.SelectedItem;

                MessageBoxResult result =
                    MessageBox.Show($"Vill du ta bort författaren {selectedAuthor.Förnamn} {selectedAuthor.Efternamn}?",
                        "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    db.Författares.Remove(selectedAuthor);
                    db.SaveChanges();

                    AuthorComboBox.SelectedItem = null;

                    AuthorInformationClear();

                    AuthorComboBoxLoad();
                }
                
            }
        }

        private void AuthorComboBoxLoad()
        {
            AuthorComboBox.Items.Clear();

            foreach (var author in db.Författares)
            {
                AuthorComboBox.Items.Add(author);
            }
        }

        private void AuthorInformationClear()
        {
            NameTextBox.Text = string.Empty;
            LastnameTextBox.Text = string.Empty;
            BirthdateDatePicker.SelectedDate = null;
            AuthorComboBox.SelectedItem = null;
            AuthorsBooksListView.Items.Clear();
        }
    }
}
