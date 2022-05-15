using BookManager;
using ExceptionManager;
using LoginLogic;
using SaveLoadLogic;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BookLibProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibrarianPage : Page
    {
        // Admin needs to manually save! Unsaved changes will not be saved.
        // I did that on purpose in case Admin does any mistake on Admin pages, he can restart the program and changes will not be saved.

        private BookListManage bookListLibManager;
        private XMLLogic xml = new XMLLogic();
        private UserManager userManager;
        private Account currentAdminAccount; // Current Admin logged in

        public LibrarianPage()
        {
            this.InitializeComponent();
        }

        // Navigation back from other pages, so the neccessary objects will be updated
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // From mainPage
            if (e.Parameter is Tuple<object, object, object>)
            {
                Tuple<object, object, object> mainPageParams = e.Parameter as Tuple<object, object, object>;
                if (mainPageParams.Item1 != null)
                {
                    BookListManage updateBL = mainPageParams.Item1 as BookListManage;
                    bookListLibManager = updateBL;
                    BookListView.Items.Clear();
                    foreach (var item in bookListLibManager.Books)
                    {
                        BookListView.Items.Add(item.Name);
                    }
                }
                if (mainPageParams.Item2 != null)
                    userManager = mainPageParams.Item2 as UserManager;
                if (mainPageParams.Item3 != null)
                {
                    currentAdminAccount = mainPageParams.Item3 as Account;
                    CurrentLoginBlock.Text = $"Logged in as: {currentAdminAccount.UserName}";
                }
            }

            // From RentPage
            if (e.Parameter is Tuple<object, object>)
            {
                Tuple<object, object> rentPageParams = e.Parameter as Tuple<object, object>;
                if (rentPageParams.Item1 != null)
                {
                    List<AbstractItem> updateBL = rentPageParams.Item1 as List<AbstractItem>;
                    bookListLibManager.Books = updateBL;
                    BookListView.Items.Clear();
                    foreach (var item in bookListLibManager.Books)
                    {
                        BookListView.Items.Add(item.Name);
                    }
                }
                if (rentPageParams.Item2 != null)
                    userManager.Accounts = rentPageParams.Item2 as List<Account>;
            }

            // From EditPage
            if (e.Parameter is string) // If ListViewItem is returned, Edit Books
            {
                string listViewTemp = e.Parameter as string;
                if (listViewTemp != null)
                {
                    BookListView.Items.Clear();
                    foreach (AbstractItem item in bookListLibManager.Books)
                    {
                        BookListView.Items.Add(item.Name);
                    }
                }
            }

            // From AddPage
            if (e.Parameter is List<AbstractItem>) // If new books were added
            {
                List<AbstractItem> newBooksParameter = e.Parameter as List<AbstractItem>;
                if (newBooksParameter != null)
                {
                    foreach (AbstractItem item in newBooksParameter)
                    {
                        BookListView.Items.Add(item.Name);
                    }
                }
            }

            // Updates controls and checks for due date on books for current user
            RefreshAmount(BookListView);
            RefreshItems(BookListView, bookListLibManager);
            DescriptionTB.Text = " ";
            BookCoverImg.Source = default;
            RentedBox.Text = " ";
            ShowDueRentDate();
        }

        // Updates the controls with the selected book/journal data when the Selection has changed in the ListView
        private void BookListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BookListView.SelectedItem == null) return;

            AbstractItem selectedItem = bookListLibManager.SelectedAbstractItem(bookListLibManager.Books, BookListView.SelectedItem.ToString());
            if (selectedItem != null)
            {
                DescriptionTB.Text = selectedItem.Description; // Book description

                if (selectedItem.ImagePath != null) // Book image, checks if not null
                    BookCoverImg.Source = new BitmapImage(new Uri(selectedItem.ImagePath));

                switch (selectedItem.IsRented)
                {
                    case true:
                        RentedBox.Text = "Rented";
                        RentedBox.Foreground = new SolidColorBrush(Colors.IndianRed);
                        RentDateBox.Text = selectedItem.RentDate.ToShortDateString(); // Rented date, will show if rented
                        break;
                    case false:
                        RentedBox.Text = "Available";
                        RentedBox.Foreground = new SolidColorBrush(Colors.LightGreen);
                        RentDateBox.Text = ""; // Rented date, will not show if not rented
                        break;
                }
            }
        }

        #region Refresh Funcs
        // Gets a list and a BookListManage item to refresh, also used in User page in order to not duplicate code:
        public void RefreshItems(ListView listItem, BookListManage blm)
        {
            listItem.Items.Clear();
            foreach (var item in blm.Books)
            {
                listItem.Items.Add(item.Name);
            }
        }

        // A text block updated with current amounts of books:
        public void RefreshAmount(ListView listViewItems)
        {
            BooksAmountTBlock.Text = $"Amount of book items: {bookListLibManager.GetBookItemsAmount(bookListLibManager.Books)} \n" +
                $"Amount of Books: {bookListLibManager.GetBooksAmount(bookListLibManager.Books)} \n" +
                $"Amount of Journals: {bookListLibManager.GetJournalAmount(bookListLibManager.Books)}\n" +
                $"Rented Books amount: {bookListLibManager.GetRentedBooksAmount(bookListLibManager.Books)}";
        }

        // Checks if a book rent is due date (2 weeks)
        private async void ShowDueRentDate()
        {
            if (currentAdminAccount.BookDueDate()) // If function returns true will display a popup message informing the user
            {
                MessageDialog rentDue = new MessageDialog("One or more of your rented books are rented longer than 14 days! " +
                    "\nPlease return your due date books.");
                await rentDue.ShowAsync();
            }

        }
        #endregion

        #region Filter and SubFilter region

        private void FilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // FilterBy is a ListView


            // If there are no books in the Books List, a safety net for failure
            if (bookListLibManager.CheckIfEmpty(bookListLibManager) == true)
            {
                ErrorBlock.Text = "No books!";
                return;
            }
            // New Filter Manager instance which holds Filtered Lists
            BookFilterManager filterManager = new BookFilterManager(bookListLibManager);

            RefreshItems(BookListView, bookListLibManager);

            if (FilterBy.SelectedIndex < 0) return;

            SubFilter.Items.Clear();
            List<string> filteredList = null;

            // Updates controls on page
            SubFilter.IsEnabled = true;
            NameFilter.IsEnabled = false;
            if (FiltName.IsSelected == true)
            {
                SubFilter.IsEnabled = false;
                NameFilter.IsEnabled = true;
            }
            // Based on filter selection gets the list asked
            else if (FiltGenre.IsSelected == true)
                filteredList = filterManager.GetFilterResultsOfType(filterManager.FilterGenre);
            else if (FiltAuthor.IsSelected == true)
                filteredList = filterManager.GetFilterResultsOfType(filterManager.FilterAuthor);
            else if (FiltPublisher.IsSelected == true)
                filteredList = filterManager.GetFilterResultsOfType(filterManager.FilterPublisher);

            // Adds the items to the Combo Box assocciated
            if (filteredList != null)
            {
                RefreshSubFilter(filteredList, SubFilter);
            }
        }

        // Refreshes SubFilter ComboBox
        public void RefreshSubFilter(List<string> filterList, ComboBox subFilt)
        {
            foreach (string item in filterList)
            {
                subFilt.Items.Add(item);
            }
        }

        // Updates the SubFilter comboBox based on selection of ListViewFilter
        private void SubFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SubFilter is a ComboBox
            try
            {
                if (bookListLibManager.CheckIfEmpty(bookListLibManager) == true) throw new NoBooksException("No books!");
            }
            catch (NoBooksException noBooksExc)
            {
                ErrorBlock.Text = noBooksExc.Message;
            }

            BookListView.Items.Clear();

            BookFilterManager filterManager = new BookFilterManager(bookListLibManager);

            if (SubFilter.SelectedItem == null) return;

            List<string> filteredItems = filterManager.SubFilterResult(bookListLibManager.Books, SubFilter.SelectedItem.ToString());

            if (filteredItems != null)
            {
                foreach (var item in filteredItems)
                {
                    BookListView.Items.Add(item);
                }
            }
        }
        // Filters the ListView based on text input in NameFilter TextBox, case sensitive
        private void NameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (NameFilter.Text == "") return;

            try
            {   // If there are no books in the Books List
                if (bookListLibManager.CheckIfEmpty(bookListLibManager) == true) throw new NoBooksException("No books!");
            }
            catch (NoBooksException noBooksExc)
            {
                ErrorBlock.Text = noBooksExc.Message;
            }

            // Populates the listview based on items that match with the text input
            BookListView.Items.Clear();
            foreach (AbstractItem item in bookListLibManager.Books)
            {
                if (item.Name.StartsWith(NameFilter.Text))
                {
                    BookListView.Items.Add(item.Name);
                }
            }
            // Refreshes all list once string is empty
            if (string.IsNullOrEmpty(NameFilter.Text))
            {
                RefreshItems(BookListView, bookListLibManager);
            }
        }
        #endregion

        #region Buttons

        #region Navigation Buttons
        // Navigates to the Add Book page:
        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddBookPage), bookListLibManager);
        }

        // Navigates to the Book edit page
        private void Editbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BookListView.SelectedItem == null)
                    throw new NoItemSelectedException("No item selected!"); // throw new exception

                AbstractItem selectedItem = bookListLibManager.SelectedAbstractItem(bookListLibManager.Books, BookListView.SelectedItem.ToString());
                if (selectedItem == null) throw new NoItemSelectedException("No item selected!");

                Frame.Navigate(typeof(EditBookPage), new Tuple<object, object, object>
                    (selectedItem, BookListView.SelectedItem, bookListLibManager.Books));
            }
            catch (NoItemSelectedException noItemExc)
            {
                ErrorBlock.Text = noItemExc.Message;
            }
        }

        // Navigates to the check books rented and the users who are renting
        private void ChkRentbtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RentedBooksPage), new Tuple<object, object>(bookListLibManager, userManager));
        }

        // Returns to MainPage and Updates, won't save on program exit if Save as XML hasn't been clicked.
        private void logOutBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), new Tuple<object, object>(bookListLibManager, userManager));
        }
        #endregion



        // Removes a Book from the BookList:
        private void Removebtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RentedBox.Text == "Rented") // Checks if book is rented by the RentedBox Text
                    throw new RentedBookException("Can't remove rented book!");
                else if (BookListView.SelectedIndex < 0) // If no book is selected
                    throw new RentedBookException("Please select a book to remove!");

                if (BookListView.SelectedIndex >= 0 && RentedBox.Text == "Available")
                {   // Goes to the function that removes a book from the Books List of the BookListManage object
                    bookListLibManager.RemoveBook(bookListLibManager.Books, BookListView.SelectedItem.ToString());

                    SubFilter.Items.Clear();
                    RefreshItems(BookListView, bookListLibManager);
                    DescriptionTB.Text = " ";
                    BookCoverImg.Source = default;
                    RentedBox.Text = " ";
                    RentDateBox.Text = "";
                    RefreshAmount(BookListView);
                }

            }
            catch (RentedBookException rentedE)
            {
                ErrorBlock.Text = rentedE.Message;
            }
        }

        // Refreshes the List
        private void ViewAllBTN_Click(object sender, RoutedEventArgs e)
        {
            RefreshItems(BookListView, bookListLibManager);
        }

        // Saves the current status of books, renting and accounts to XML
        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            xml.Save(bookListLibManager.Books);
            xml.SaveAccounts(userManager.Accounts);
        }
        // Loads the current status of books, renting and accounts from XML
        private void LoadBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Account> tempAccountList = xml.LoadAccounts();
                List<AbstractItem> tempBookList = xml.Load();

                if (tempAccountList == null && tempBookList == null) // If no Load files at all
                    throw new NoLoadFileException("No load files found!");
                else if (tempAccountList == null) // If null, no Load File for Accounts
                    throw new NoLoadFileException("No accounts load file found!");
                else if (tempBookList == null) // If null , no Load File for Books
                    throw new NoLoadFileException("No books load file found!");

                // Second verification that account list is not null before action on list
                if (tempAccountList != null)
                {
                    userManager.Accounts = tempAccountList;

                    foreach (Account item in userManager.Accounts)
                    {
                        if (currentAdminAccount.UserName == item.UserName)
                            currentAdminAccount = item;
                    }
                }
                // Second verification that books are not null before action on list
                if (tempBookList != null)
                    bookListLibManager.Books = tempBookList;

                RefreshItems(BookListView, bookListLibManager);
                RefreshAmount(BookListView);
            }
            catch (NoLoadFileException nlE)
            {
                ErrorBlock.Text = nlE.Message;
            }
        }

        // Rent a book for current Admin logged in
        private void Rentbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BookListView.SelectedItem == null) throw new NoItemSelectedException("Select an item to rent!");
                // Gets selected item using function from BookListManage with List<AbstractItem>, string parameters
                AbstractItem selectedItem = bookListLibManager.SelectedAbstractItem(bookListLibManager.Books, BookListView.SelectedItem.ToString());

                if (selectedItem != null)
                {
                    if (selectedItem.IsRented == false) // change to true and display messsage
                    {
                        // Add the book to current Admin logged in
                        currentAdminAccount.BooksRented.Add(selectedItem);

                        // Updates book parameters and returns false on failure
                        bool checkRent = selectedItem.RentBook(currentAdminAccount.UserName);
                        if (checkRent == false)
                        {
                            ErrorBlock.Text = "Error in renting!";
                            return;
                        }

                        // Updates controls on page
                        RentDateBox.Text = selectedItem.RentDate.ToShortDateString();
                        RentedBox.Text = "Rented";
                        RentedBox.Foreground = new SolidColorBrush(Colors.IndianRed);
                        DescriptionTB.Text = selectedItem.Description;
                        RefreshItems(BookListView, bookListLibManager);
                        RefreshAmount(BookListView);
                    }
                    else if (selectedItem.IsRented == true)
                        throw new RentedBookException("Book is already rented!");
                }
            }
            catch (NoItemSelectedException noItemExc)
            {
                ErrorBlock.Text = noItemExc.Message;
            }
            catch (RentedBookException rentExc)
            {
                ErrorBlock.Text = rentExc.Message;
            }
        }

        // Return a book that current Admin has rented
        private void Returnbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BookListView.SelectedItem == null) throw new NoItemSelectedException("Select an item to return!");
                // Gets selected item using function from BookListManage with List<AbstractItem>, string parameters
                AbstractItem selectedItem = bookListLibManager.SelectedAbstractItem(bookListLibManager.Books, BookListView.SelectedItem.ToString());

                if (selectedItem != null && selectedItem.IsRented && userManager.CheckIfBookIsRentedByAccount(selectedItem.Name, currentAdminAccount))
                {
                    // Removes book from current user
                    userManager.RemoveRentedBook(selectedItem.Name, currentAdminAccount);

                    // Updates book parameters
                    selectedItem.ReturnBook();

                    // Updates control on page
                    RentDateBox.Text = "";
                    RentedBox.Text = "Available";
                    RentedBox.Foreground = new SolidColorBrush(Colors.LightGreen);
                    DescriptionTB.Text = selectedItem.Description;
                    RefreshItems(BookListView, bookListLibManager);
                    RefreshAmount(BookListView);
                }
                else if (selectedItem.IsRented && userManager.CheckIfBookIsRentedByAccount(selectedItem.Name, currentAdminAccount) == false)
                    throw new WrongUserRentException("Not rented by you!");
                else if (selectedItem.IsRented == false)
                    throw new RentedBookException("Book is not rented!");
            }
            catch (NoItemSelectedException noItemExc)
            {
                ErrorBlock.Text = noItemExc.Message;
            }
            catch (WrongUserRentException wrongUsExc)
            {
                ErrorBlock.Text = wrongUsExc.Message;
            }
            catch (RentedBookException rentedExc)
            {
                ErrorBlock.Text = rentedExc.Message;
            }

        }
        // Clears text from Error text block
        private void ClearErrorBTN_Click(object sender, RoutedEventArgs e)
        {
            ErrorBlock.Text = "";
        }
        #endregion    
    }
}
