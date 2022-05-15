using BookManager;
using LoginLogic;
using SaveLoadLogic;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BookLibProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    // Developed by Daniel Krigel, Sela 1018.
    public sealed partial class MainPage : Page
    {
        public BookListManage mainPageBookList = new BookListManage(); // Main Book List, public for use in other pages to prevent errors
        UserManager userManager = new UserManager(); // Main user manager is in this page
        XMLLogic xmlLogic = new XMLLogic();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // The following checks for XML Files, will load them in case there are
            // Will not load and will show a small message, but the program can continue to run
            // And if the mainPage loads XML, it will not continue to the Tuple load
            try
            {
                if (xmlLogic.Load() == null || xmlLogic.LoadAccounts() == null)
                {
                    loadBlock.Text = "No Load file yet.";
                }
                else
                {
                    List<AbstractItem> tempItemList = xmlLogic.Load();
                    List<Account> tempAccountList = xmlLogic.LoadAccounts();
                    if (tempItemList != null && tempAccountList != null)
                    {
                        mainPageBookList.Books = tempItemList;
                        userManager.Accounts = tempAccountList;
                        loadBlock.Text = "File loaded successfully.";
                        return;
                    }
                }
            }
            catch (NullReferenceException) { }

            // Although XML is used, the following still needed when going back from Admin page if Admin forgot to save but program still runs.
            if (e.Parameter is Tuple<object, object>) // Navigated from Admin page
            {
                Tuple<object, object> updatedParams = e.Parameter as Tuple<object, object>;
                if (updatedParams.Item1 != null)
                {
                    mainPageBookList = updatedParams.Item1 as BookListManage;
                }
                if (updatedParams.Item2 != null)
                {
                    userManager = updatedParams.Item2 as UserManager;
                }
            }
        }

        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            // Checks if Username Text Box or Password Text Box are empty, will throw a message in the correct control if empty
            if (UserTB.Text == "" || PasswordTB.Password == "")
            {
                ErrorBlock.Text = "Please enter username and password";
                return;
            }
            Account acnt = userManager.GetCurrentAcount(UserTB.Text, userManager.Accounts); // Gets the current account entered in username
            if (acnt == null)
            {
                ErrorBlock.Text = "Wrong username"; // If such account not exists, throws a message
            }
            else if (acnt.VerifyPassword(PasswordTB.Password.ToString()) == false)
            {
                ErrorBlock.Text = "Wrong password"; // If password is wrong, throws a message
            }
            else if (UserTB.Text == acnt.UserName && acnt.VerifyPassword(PasswordTB.Password.ToString()) == true) // Verification function for password
            {
                // Checks what type is Account, if Admin, moves to LibrarianPage, if User, moves to UserUI page.

                if (acnt is Admin)
                    Frame.Navigate(typeof(LibrarianPage), new Tuple<object, object, object>(mainPageBookList, userManager, acnt));

                else if (acnt is User)
                    Frame.Navigate(typeof(UserUI), new Tuple<object, object, object>(mainPageBookList, userManager, acnt));
            }
        }
    }
}
