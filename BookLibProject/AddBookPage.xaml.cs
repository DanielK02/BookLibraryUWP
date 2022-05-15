using BookManager;
using ImageLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BookLibProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddBookPage : Page
    {
        private BookListManage blmanager;
        private List<AbstractItem> newBooks = new List<AbstractItem>();
        private ImageManager imageMng = new ImageManager();
        private string imagePath;
        public AddBookPage()
        {
            this.InitializeComponent();
        }
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {   // Navigates back to Librarian Page with a list of new books added
            Frame.Navigate(typeof(LibrarianPage), newBooks);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {   // Gets a BookListManage object from Librarian Page
            blmanager = e.Parameter as BookListManage;
        }

        // A function that adds a new book to a list 
        private void AddBookbtn_Click(object sender, RoutedEventArgs e)
        {

            // The following lines check for duplications in Name/ISBN
            foreach (AbstractItem item in blmanager.Books)
            {
                if (BookNameBox.Text == item.Name)
                {
                    ErrorBlock.Text = "There is already a book with that name!";
                    return;
                }
                else if (ISBNBox.Text == item.ISBN)
                {
                    ErrorBlock.Text = "There is already a book with that ISBN!";
                    return;
                }
            }


            // To verify a correct rental price
            int rentPrice;
            int.TryParse(RentPriceBox.Text, out rentPrice);
            if (rentPrice <= 0)
            {
                ErrorBlock.Text = "Please enter correct rent price! (Higher than 0)";
                return;
            }

            // Check for empty fields
            if (CheckEmptyFields() == true)
                return;

            // checks if Journal or Book are selected to save the correct item
            string radioButtonState = "Book";

            if (JourRadioSelect.IsChecked == true)
                radioButtonState = "Journal";
            else if (BookRadioSelect.IsChecked == true)
                radioButtonState = "Book";

            if (imagePath == null)
                imagePath = "ms-appx:///Assets/NoImage.png";
            try
            {
                newBooks.Add(blmanager.Addbook(BookNameBox.Text, AuthorBox.Text, PublisherBox.Text, GenreBox.Text,
                    radioButtonState, imagePath, EditBookPage.ConvertFromDatePicker(PublishDateBox), rentPrice, ISBNBox.Text));

                ErrorBlock.Text = $"{radioButtonState} added successfully!";
            }
            catch (NullReferenceException nullExc)
            {
                ErrorBlock.Text = nullExc.Message;
            }
        }

        // Checks for empty fields, won't let save in case there are, returns true if a field is empty, false if not
        private bool CheckEmptyFields()
        {
            if (string.IsNullOrEmpty(BookNameBox.Text) || string.IsNullOrEmpty(AuthorBox.Text)
                || string.IsNullOrEmpty(PublisherBox.Text) || string.IsNullOrEmpty(GenreBox.Text)
                || PublishDateBox.Date == null || string.IsNullOrEmpty(ISBNBox.Text))
            {
                ErrorBlock.Text = "Please fill all the fields!";
                return true;
            }
            else
                return false;
        }

        // Disable AuthorTextBox in case Journal is selected
        private void JourRadioSelect_Checked(object sender, RoutedEventArgs e)
        {
            AddBookbtn.IsEnabled = true;
            AuthorBox.IsEnabled = false;
        }
        // Enable AuthorTextBox in case Book is selected
        private void BookRadioSelect_Checked(object sender, RoutedEventArgs e)
        {
            AddBookbtn.IsEnabled = true;
            AuthorBox.IsEnabled = true;
        }

        // using ImageLogic changes the book image
        private async void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            string browseFilePath = await imageMng.FileBrowserAsync();
            if (browseFilePath == null) return;
            imagePath = browseFilePath;
            ImgPreview.Source = new BitmapImage(new Uri(imagePath));
        }

        // Disables to ability to type characters other than digits
        private void RentPriceBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }
    }
}
