using BookManager;
using LoginLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestLibProject
{
    [TestClass]
    public class UnitTestUser
    {
        UserManager testUserManager = new UserManager();
        Account testAccount = new Admin("Daniel", "Pass");
        Book testBook = new Book("bookname", "authorname", "publisher", "genre", DateTime.Now, null, 5, "1234");

        // Login Verify Password test
        [TestMethod]
        public void TestLoginValid()
        {
            Assert.IsTrue(testAccount.VerifyPassword(testAccount.Password));
        }
        [TestMethod]
        public void TestLoginInvalid()
        {
            Assert.IsFalse(testAccount.VerifyPassword("test"));
        }


        // Check get current account, first check by Username that it gets the valid account
        // Second by Account from Accounts List
        [TestMethod]
        public void TestGetAccountValid()
        {
            Account temp = testUserManager.GetCurrentAcount("Admin", testUserManager.Accounts);
            Assert.AreEqual("Admin", temp.UserName);
        }
        [TestMethod]
        public void TestGetAccountValid2()
        {
            testUserManager.Accounts.Add(testAccount);
            Account temp = testUserManager.GetCurrentAcount("Daniel", testUserManager.Accounts);
            Assert.AreEqual(testAccount, temp);

        }
        [TestMethod]
        public void TestGetAccountInvalid()
        {
            Account temp = testUserManager.GetCurrentAcount("test", testUserManager.Accounts);
            Assert.IsNull(temp);
        }

        // Tests if returns false on searching for a non existing book in list
        [TestMethod]
        public void TestFalseBookRentedByAccountValid()
        {
            Assert.IsFalse(testUserManager.CheckIfBookIsRentedByAccount(testBook.Name, testAccount));
        }
        // Tests if returns true on existing bok in list
        [TestMethod]
        public void TestTrueBookRentedByAccountValid()
        {
            testAccount.BooksRented.Add(testBook);
            Assert.IsTrue(testUserManager.CheckIfBookIsRentedByAccount(testBook.Name, testAccount));
        }
        // Checks for false on invalid book name
        [TestMethod]
        public void TestFalseBookNameRentedByAccountInvalid()
        {
            Assert.IsFalse(testUserManager.CheckIfBookIsRentedByAccount("test", testAccount));
        }

        // Checks that the DueRent function (which popus a message on late book return over 14 days) works.
        [TestMethod]
        public void CheckNotDueRentDate()
        {
            testBook.RentDate = DateTime.Now;
            testAccount.BooksRented.Add(testBook);
            Assert.IsFalse(BookDueDateTest(testAccount));
        }
        [TestMethod]
        public void CheckDueRentPassedDate()
        {
            testBook.RentDate = DateTime.Now.AddDays(-14);
            testAccount.BooksRented.Add(testBook);
            Assert.IsTrue(BookDueDateTest(testAccount));
        }
        // Similiar to the function found in Account
        private bool BookDueDateTest(Account testAccount)
        {
            if (testAccount.BooksRented != null)
            {
                foreach (AbstractItem book in testAccount.BooksRented)
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
