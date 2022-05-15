using BookManager;
using System.Collections.Generic;

namespace LoginLogic
{
    public class UserManager
    {
        List<Account> accounts = new List<Account>();

        public UserManager()
        {
            accounts.Add(new Admin("Admin", "Pass"));
            accounts.Add(new Admin("Daniel", "Pass1"));
            accounts.Add(new User("User", "Pass"));
            accounts.Add(new User("UserDaniel", "Pass123"));
        }
        public List<Account> Accounts
        {
            get { return accounts; }
            set { accounts = value; }
        }

        public List<AbstractItem> GetRentedBookListByUser(Account selectedUser)
        {
            return selectedUser.BooksRented;
        }

        // Get current Account, it's okay if returns Null
        public Account GetCurrentAcount(string userName, List<Account> accountList)
        {
            Account temp = PrivateGetCurrentAcount(userName, accountList);
            return temp;
        }
        private Account PrivateGetCurrentAcount(string userName, List<Account> accountList)
        {
            Account temp = default;
            foreach (Account item in accountList)
            {
                if (userName == item.UserName)
                    temp = item;
            }
            return temp;
        }

        // Remove rented book from Account.RentedBooks List
        public List<AbstractItem> RemoveRentedBook(string book, Account user)
        {
            List<AbstractItem> userBooksRented = PrivateRemoveRentedBook(book, user);
            return userBooksRented;
        }
        private List<AbstractItem> PrivateRemoveRentedBook(string book, Account user)
        {
            if (book == null || user == null) return default;
            foreach (AbstractItem item in user.BooksRented)
            {
                if (book == item.Name)
                {
                    user.BooksRented.Remove(item);
                    break;
                }
            }
            return user.BooksRented;
        }

        // Checks if a book is rented by Account
        public bool CheckIfBookIsRentedByAccount(string bookName, Account user)
        {
            if (bookName == null || user == null) return false;
            bool rentCheck = CheckIfBookRentedByAccountPrivate(bookName, user);
            return rentCheck;
        }
        private bool CheckIfBookRentedByAccountPrivate(string bookName, Account user)
        {
            foreach (AbstractItem item in user.BooksRented)
            {
                if (bookName == item.Name)
                    return true;
            }
            return false;
        }
    }
}
