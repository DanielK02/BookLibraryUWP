using BookManager;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LoginLogic
{
    [XmlInclude(typeof(Admin))]
    [XmlInclude(typeof(User))]
    public class Account
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        // I am aware that the password should be encrypted / private, but there is no requirment in the project for a login / encrypted login
        // and because of XML + UWP issues and the fact I need to hardcode the password somewhere in the project.
        // To truly hide the password it would require a database, in the current situation the password will be hardcoded somewhere in the project
        // It is Public because of XML issues (we haven't learned to deal with XML or JSON yet)

        [XmlIgnore]
        private List<AbstractItem> booksRented = new List<AbstractItem>();

        // Returns a List of the Books rented by the Account
        public List<AbstractItem> BooksRented
        {
            get { return booksRented; }
            set { booksRented = value; }
        }

        // A method to verify the password on login
        public bool VerifyPassword(string input)
        {
            bool checkIfPassValid = PrivateVerifyPassword(input);
            return checkIfPassValid;
        }
        private bool PrivateVerifyPassword(string input)
        {
            bool checkPass = false;
            if (input != null)
            {
                checkPass = input == Password;
            }
            return checkPass;
        }

        // A function that checks if books were rented 14 days or more ago.
        // If RentedDate on the book + 14 days is smaller or equal than current Date will return true
        // false if not
        public bool BookDueDate()
        {
            if (this.BooksRented != null)
            {
                foreach (AbstractItem book in this.BooksRented)
                {
                    if (book.RentDate.AddDays(14) <= DateTime.Now)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
