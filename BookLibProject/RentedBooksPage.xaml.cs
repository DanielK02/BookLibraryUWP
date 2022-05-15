using BookManager;
using ExceptionManager;
using LoginLogic;
using SaveLoadLogic;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BookLibProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RentedBooksPage : Page
    {
        private UserManager userManager;
        private BookListManage rentPageBookList;
        private List<AbstractItem> rentedBooks; // A list that contain the rented book by selectedUser, changed constantly on selection
        private Account selectedUser; // The selected user based on UserListView selection
        private XMLLogic xml = new XMLLogic();
        public RentedBooksPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // On navigation from LibrarianPage gets a Tuple of BookList and Accounts and updates them.
            if (e.Parameter != null)
            {
                Tuple<object, object> tuple = e.Parameter as Tuple<object, object>;
                rentPageBookList = tuple.Item1 as BookListManage;
                userManager = tuple.Item2 as UserManager;
            }
            // Clears the lists
            UserListView.Items.Clear();
            RentedBooksListView.Items.Clear();
            RentAvailableListView.Items.Clear();

            // Verifies Accounts are not null and adds to Account List
            if (userManager.Accounts != null)
            {
                foreach (Account user in userManager.Accounts)
                {
                    UserListView.Items.Add(user.UserName);
                }
            }
            // Verifies Books are not null and adds to Rent Available Books list
            if (rentPageBookList.Books != null)
            {
                foreach (AbstractItem item in rentPageBookList.Books)
                {
                    if (item.IsRented == false)
                        RentAvailableListView.Items.Add(item.Name);
                }
            }

            RefreshAvailableBooks();
        }
        // Gets the selected User and his rented books from UserListView in order to do futher operations (Rent / Return)
        private void UserListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gets the selected user
            selectedUser = userManager.GetCurrentAcount(UserListView.SelectedValue.ToString(), userManager.Accounts);
            // Gets a list of the rented books by the selected user
            rentedBooks = userManager.GetRentedBookListByUser(selectedUser);
            RefreshItems(RentedBooksListView);
        }

        #region Refresh Functions
        // Refreshes a ListView of given items
        private void RefreshItems(ListView listItem)
        {
            listItem.Items.Clear();
            foreach (AbstractItem item in rentedBooks)
            {
                listItem.Items.Add(item.Name);
            }
        }
        // Refreshing books available to rent
        private void RefreshAvailableBooks()
        {
            // Verifies Books are not null
            try
            {
                if (rentPageBookList.Books == null) throw new NoBooksException("No books!");

                RentAvailableListView.Items.Clear();
                foreach (AbstractItem item in rentPageBookList.Books)
                {
                    if (item.IsRented == false)
                        RentAvailableListView.Items.Add(item.Name);
                }
            }
            catch (NoBooksException noBooksExc)
            {
                ErrorBlock.Text = noBooksExc.Message;
            }
        }
        #endregion

        #region Buttons
        // Navigates back to Librarian Page
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LibrarianPage), new Tuple<object, object>(rentPageBookList.Books, userManager.Accounts));
        }
        // Clears ErrorBlock text block
        private void ClearErrorBTN_Click(object sender, RoutedEventArgs e)
        {
            ErrorBlock.Text = "";
        }
        // Returns book for selected user in UserListView
        private void ReturnBTN_Click(object sender, RoutedEventArgs e) // try catch
        {
            // If no user / rented book selected
            if (UserListView.SelectedItem == null || RentedBooksListView.SelectedItem == null)
                ErrorBlock.Text = "Select an item";

            AbstractItem selectedItem = rentPageBookList.SelectedAbstractItem(rentPageBookList.Books, RentedBooksListView.SelectedItem.ToString());

            if (selectedItem != null)
            {
                // Removes book from user
                selectedUser.BooksRented = userManager.RemoveRentedBook(selectedItem.Name, selectedUser);

                // Updates book parameters
                selectedItem.ReturnBook();

                // Updates books by user
                rentedBooks = userManager.GetRentedBookListByUser(selectedUser);

                // Updates lists on page
                RefreshItems(RentedBooksListView);
                RefreshAvailableBooks();
            }

        }
        // Rents a book for selected user in UserListView
        private void RentBTN_Click(object sender, RoutedEventArgs e)
        {
            if (UserListView.SelectedItem == null || RentAvailableListView.SelectedItem == null)
                ErrorBlock.Text = "Select an item";

            AbstractItem selectedItem = rentPageBookList.SelectedAbstractItem(rentPageBookList.Books, RentAvailableListView.SelectedItem.ToString());
            if (selectedItem != null)
            {
                if (selectedItem.IsRented == false) // I know that it's unneccessary, remains for verification
                {
                    // Adds book to user
                    selectedUser.BooksRented.Add(selectedItem);

                    // Updates book parameters will return false on failure
                    bool checkRent = selectedItem.RentBook(selectedUser.UserName);
                    if (checkRent == false)
                    {
                        ErrorBlock.Text = "Error in renting!";
                        return;
                    }

                    // Updates books by user
                    rentedBooks = userManager.GetRentedBookListByUser(selectedUser);

                    // Updates lists on page
                    RefreshItems(RentedBooksListView);
                    RefreshAvailableBooks();
                }
            }
        }

        // Saves to XML, ability to Load is located in Librarian page
        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            xml.Save(rentPageBookList.Books);
            xml.SaveAccounts(userManager.Accounts);
        }

        #endregion
    }
}
