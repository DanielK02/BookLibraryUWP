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
    /// 

    public sealed partial class EditBookPage : Page
    {
        private AbstractItem bookItem;
        private List<AbstractItem> bookListTemp; // Created seperately to remove edited book to check for duplicates
        private string listViewItem;
        private ImageManager imgMng = new ImageManager();
        private string imagePath;

        public EditBookPage()
        {
            this.InitializeComponent();
        }

        // On navigation to page, updates the controls with the book data
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Tuple<object, object, object> parameter = e.Parameter as Tuple<object, object, object>;

            bookItem = parameter.Item1 as AbstractItem;
            listViewItem = parameter.Item2 as string;
            bookListTemp = parameter.Item3 as List<AbstractItem>;

            if (listViewItem == null || bookItem == null)
                throw new NullReferenceException();
            else
            {
                BookNameBox.Text = bookItem.Name;
                PublisherBox.Text = bookItem.Publisher;
                GenreBox.Text = bookItem.Genre;
                ImgPreview.Source = new BitmapImage(new Uri(bookItem.ImagePath));
                ISBNBox.Text = bookItem.ISBN;
                RentPriceBox.Text = bookItem.RentPrice.ToString();
                listViewItem = bookItem.Name;
                if (bookItem.PublishDate != default)
                    PublishDateBox.Date = bookItem.PublishDate;
                if (bookItem.Author == null)
                    AuthorBox.IsEnabled = false;
                else
                    AuthorBox.Text = bookItem.Author;
            }
            try
            {
                if (bookListTemp == null)
                    throw new NullReferenceException();
            }
            catch (NullReferenceException)
            {
                // Navigates back to previous page in case one of the passed object is null
                // I've made sure that objects are passed on previous page.
                Frame.Navigate(typeof(LibrarianPage));
            }

        }

        // Returns to Librarian(Admin) page and updates the ListView
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LibrarianPage), listViewItem);
        }

        // Saves the Edited book
        private void Save_Click(object sender, RoutedEventArgs e)
        {

            //The following lines check for duplications in Name / ISBN
            foreach (AbstractItem item in bookListTemp)
            {
                // Checks that item is not duplicate with the list, but different than edited item
                // First for Name, then ISBN
                if (BookNameBox.Text == item.Name && BookNameBox.Text != bookItem.Name)
                {
                    ErrorBlock.Text = "There is already a book with that name!";
                    return;
                }
                else if (ISBNBox.Text == item.ISBN && ISBNBox.Text != bookItem.ISBN)
                {
                    ErrorBlock.Text = "There is already a book with that ISBN!";
                    return;
                }
            }

            int rentPrice;
            int.TryParse(RentPriceBox.Text, out rentPrice);
            if (rentPrice <= 0)
            {
                ErrorBlock.Text = "Please enter correct rent price!";
                return;
            }



            if (listViewItem == null || bookItem == null)
                ErrorBlock.Text = "No book in this page! Return to previous page and select a book";
            else if (CheckEmptyFields() == true)
                ErrorBlock.Text = "Please make sure to fill all fields!";
            else
            {
                listViewItem = bookItem.Name = BookNameBox.Text;
                bookItem.PublishDate = ConvertFromDatePicker(PublishDateBox);
                bookItem.Publisher = PublisherBox.Text;
                bookItem.Genre = GenreBox.Text;
                bookItem.ISBN = ISBNBox.Text;


                bookItem.RentPrice = rentPrice;
                if (AuthorBox.IsEnabled == true)
                    bookItem.Author = AuthorBox.Text;

                if (imagePath != null)
                    bookItem.ImagePath = imagePath;

                ErrorBlock.Text = "Saved successfully";
            }
        }

        // Checks for empty fields, won't let save in case there are, returns true if a field is empty, false if not
        private bool CheckEmptyFields()
        {


            if (string.IsNullOrEmpty(BookNameBox.Text) || string.IsNullOrEmpty(AuthorBox.Text)
                || string.IsNullOrEmpty(PublisherBox.Text) || string.IsNullOrEmpty(GenreBox.Text)
                || PublishDateBox.Date == null || string.IsNullOrEmpty(ISBNBox.Text))
            {
                return true;
            }
            else
                return false;
        }

        // As the name says, converts from DateTimePicker/DateTimeOffset to DateTime:
        public static DateTime ConvertFromDatePicker(CalendarDatePicker datePicker)
        {
            if (datePicker.Date == null) return default;
            DateTimeOffset dateTimeOff;
            DateTime dt;
            dateTimeOff = datePicker.Date.Value;
            dt = dateTimeOff.DateTime;
            return dt;
        }

        // using ImageLogic changes the book image
        private async void chngImgBTN_Click(object sender, RoutedEventArgs e)
        {
            string browseFilePath = await imgMng.FileBrowserAsync();
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
