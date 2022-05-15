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
    public sealed partial class UserUI : Page
    {
        private BookListManage blmanager;
        private MainPage mainPage = new MainPage();
        private LibrarianPage librarianPage = new LibrarianPage();
        private XMLLogic xmlLogic = new XMLLogic();
        private UserManager userManager;
        private Account currentUserAccount;
        private List<AbstractItem> userRentedBooksList;

        public UserUI()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ListViewUser.Items.Clear();

            // 1st parameter is the BookListManage object, 2nd is the UserManager, 3rd is the current Account (logged in)
            // Passed as Tuple from MainPage
            if (e.Parameter is Tuple<object, object, object>)
            {
                Tuple<object, object, object> mainPageParams = e.Parameter as Tuple<object, object, object>;

                if (mainPageParams.Item1 != null)
                {
                    BookListManage updatesBL = mainPageParams.Item1 as BookListManage;
                    if (updatesBL == null)
                        blmanager = mainPage.mainPageBookList;
                    else
                        blmanager = updatesBL;
                }
                else
                    blmanager = mainPage.mainPageBookList;


                if (mainPageParams.Item2 != null)
                    userManager = mainPageParams.Item2 as UserManager;

                if (mainPageParams.Item3 != null)
                {
                    currentUserAccount = mainPageParams.Item3 as Account;
                    CurrentLoginBlock.Text = $"Logged in as: {currentUserAccount.UserName}";
                    userRentedBooksList = userManager.GetRentedBookListByUser(currentUserAccount);
                    RefreshUserRentedBooks(userRentedBooksList);
                }
            }
            // Updates the List on Navigation
            foreach (AbstractItem item in blmanager.Books)
            {
                ListViewUser.Items.Add(item.Name);
            }
            // Checks for due date on books for current user
            ShowDueRentDate();
        }

        // Checks if a book rent is due date (2 weeks)
        private async void ShowDueRentDate()
        {
            if (currentUserAccount.BookDueDate()) // If function returns true will display a popup message informing the user
            {
                MessageDialog rentDue = new MessageDialog("One or more of your rented books are rented longer than 14 days! " +
                    "\nPlease return your due date books.");
                await rentDue.ShowAsync();
            }

        }
        //Book List View:
        private void ListViewUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewUser.SelectedItem == null) return;

            // Gets the correct book / journal:
            AbstractItem selectedItem = blmanager.SelectedAbstractItem(blmanager.Books, ListViewUser.SelectedItem.ToString());
            if (selectedItem != null)
            {
                DescriptionTB.Text = selectedItem.Description;
                if (selectedItem.ImagePath != null)
                    BookCoverIMG.Source = new BitmapImage(new Uri(selectedItem.ImagePath));

                switch (selectedItem.IsRented)
                {
                    case true:
                        RentedBox.Text = "Rented";
                        RentedBox.Foreground = new SolidColorBrush(Colors.IndianRed);
                        RentDateBox.Text = selectedItem.RentDate.ToShortDateString();
                        break;
                    case false:
                        RentedBox.Text = "Available";
                        RentedBox.Foreground = new SolidColorBrush(Colors.LightGreen);
                        RentDateBox.Text = "";
                        break;
                }
            }
        }
        // Refreshes ComboBox of user rented books to let the user see which books he had rented
        private void RefreshUserRentedBooks(List<AbstractItem> bookList)
        {
            if (bookList == null) return;
            UserRentedBooksBox.Items.Clear();
            foreach (AbstractItem item in bookList)
            {
                UserRentedBooksBox.Items.Add(item.Name);
            }
        }

        #region Filter Catergory:

        // Refreshes the ListView based on ListViewFilter parameter selection and SubFilter ComboBox selection
        private void ListViewFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {   // If there are no books in the Books List, a safety net for failure
                if (blmanager.CheckIfEmpty(blmanager) == true)
                    throw new NoBooksException("No books!");
            }
            catch (NoBooksException noBooksExc)
            {
                ErrorBlock.Text = noBooksExc.Message;
            }
            if (ListViewFilter.SelectedIndex < 0) return;

            // Sets an object of Filter Manager
            BookFilterManager filterManager = new BookFilterManager(blmanager);

            // Refreshes the booklist and list view 
            librarianPage.RefreshItems(ListViewUser, blmanager);
            SubFilter.Items.Clear();
            List<string> filteredList = null;

            // Switches between the correct control to the filter selected
            // If Genre/Author/Publisher filter selected, Name filtered disable
            // On the opposite SubFilter ComboBox disabled if Name selected
            SubFilter.IsEnabled = true;
            NameFilter.IsEnabled = false;
            if (FiltName.IsSelected == true) // I've left "==true" for readability
            {
                SubFilter.IsEnabled = false;
                NameFilter.IsEnabled = true;
            }

            // Based on selection of Filter gets the neccessary list for filtering and adding to SubFilter(ComboBox)
            else if (FiltGenre.IsSelected == true)
                filteredList = filterManager.GetFilterResultsOfType(filterManager.FilterGenre);
            else if (FiltAuthor.IsSelected == true)
                filteredList = filterManager.GetFilterResultsOfType(filterManager.FilterAuthor);
            else if (FiltPublisher.IsSelected == true)
                filteredList = filterManager.GetFilterResultsOfType(filterManager.FilterPublisher);

            if (filteredList != null)
            {
                librarianPage.RefreshSubFilter(filteredList, SubFilter);
            }
        }
        // Filters the ListView based on text input in NameFilter TextBox, case sensitive
        private void NameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (NameFilter.Text == " ") return;

            try
            {   // If there are no books in the Books List
                if (blmanager.CheckIfEmpty(blmanager) == true) throw new NoBooksException("No books!");
            }
            catch (NoBooksException noBooksExc)
            {
                ErrorBlock.Text = noBooksExc.Message;
            }

            // Populates the listview based on items that match with the text input
            ListViewUser.Items.Clear();
            foreach (AbstractItem item in blmanager.Books)
            {
                if (item.Name.StartsWith(NameFilter.Text))
                {
                    ListViewUser.Items.Add(item.Name);
                }
            }
            // Refreshes all list once string is empty
            if (string.IsNullOrEmpty(NameFilter.Text))
            {
                librarianPage.RefreshItems(ListViewUser, blmanager);
            }
        }
        // Updates the SubFilter comboBox based on selection of ListViewFilter
        private void SubFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SubFilter is a ComboBox

            ListViewUser.Items.Clear();

            BookFilterManager filterManager = new BookFilterManager(blmanager);

            if (SubFilter.SelectedItem == null) return;

            List<string> filteredItems = filterManager.SubFilterResult(blmanager.Books, SubFilter.SelectedItem.ToString());

            if (filteredItems != null)
            {
                foreach (var item in filteredItems)
                {
                    ListViewUser.Items.Add(item);
                }
            }

        }
        #endregion

        #region Buttons

        // A refresh button to view all after filtering
        private void ViewAllBTN_Click(object sender, RoutedEventArgs e)
        {
            librarianPage.RefreshItems(ListViewUser, blmanager);
        }
        // On logout, saves, either creating new XML or overwrites one.
        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            // No need to pass parameters back as XML is updated on user renting/returning
            // And MainPage loads XML on navigation.
            Frame.Navigate(typeof(MainPage));
        }

        // Rent a book for current User logged in
        private void UserRentBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListViewUser.SelectedItem == null) throw new NoItemSelectedException("Select an item to rent!");
                AbstractItem selectedItem = blmanager.SelectedAbstractItem(blmanager.Books, ListViewUser.SelectedItem.ToString());

                if (selectedItem != null)
                {
                    if (selectedItem.IsRented == false)
                    {
                        // Adds the book rented to the Users book rented list
                        currentUserAccount.BooksRented.Add(selectedItem);

                        // Updates book parameters
                        bool checkRent = selectedItem.RentBook(currentUserAccount.UserName);
                        if (checkRent == false)
                        {
                            ErrorBlock.Text = "Error in renting!";
                            return;
                        }

                        // Updates page controls:
                        RentDateBox.Text = selectedItem.RentDate.ToShortDateString();
                        RentedBox.Text = "Rented";
                        RentedBox.Foreground = new SolidColorBrush(Colors.IndianRed);
                        DescriptionTB.Text = selectedItem.Description;
                        librarianPage.RefreshItems(ListViewUser, blmanager);
                        RefreshUserRentedBooks(userRentedBooksList);

                        // updates XML on user renting
                        xmlLogic.Save(blmanager.Books);
                        xmlLogic.SaveAccounts(userManager.Accounts);

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

        // Return a book current User has rented
        private void UserRtrnBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {   // Checks if there is an item Selected
                if (ListViewUser.SelectedItem == null) throw new NoItemSelectedException("Select an item to return!");

                AbstractItem selectedItem = blmanager.SelectedAbstractItem(blmanager.Books, ListViewUser.SelectedItem.ToString());

                if (selectedItem != null)
                {   // Verifies that item is rented by logged in user
                    if (selectedItem.IsRented == true && userManager.CheckIfBookIsRentedByAccount(selectedItem.Name, currentUserAccount))
                    {
                        // Removes rented book from user
                        userManager.RemoveRentedBook(selectedItem.Name, currentUserAccount);

                        // Updates book parameters
                        selectedItem.ReturnBook();

                        // updates controls on page
                        RentDateBox.Text = "";
                        RentedBox.Text = "Available";
                        RentedBox.Foreground = new SolidColorBrush(Colors.LightGreen);
                        DescriptionTB.Text = selectedItem.Description;
                        librarianPage.RefreshItems(ListViewUser, blmanager);
                        RefreshUserRentedBooks(userRentedBooksList);

                        // updates XML after user return
                        xmlLogic.Save(blmanager.Books);
                        xmlLogic.SaveAccounts(userManager.Accounts);
                    }
                    else if (selectedItem.IsRented && userManager.CheckIfBookIsRentedByAccount(selectedItem.Name, currentUserAccount) == false)
                        throw new WrongUserRentException("Not rented by you!");
                    else if (selectedItem.IsRented == false)
                        throw new RentedBookException("Book is not rented!");
                }
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
        // Clear error TextBlock
        private void ClearErrorBTN_Click(object sender, RoutedEventArgs e)
        {
            ErrorBlock.Text = " ";
        }
        #endregion
    }
}
