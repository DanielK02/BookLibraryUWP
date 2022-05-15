using BookManager;
using LoginLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTestLibProject
{
    [TestClass]
    public class UnitTestBookManager
    {
        BookListManage testBlManager = new BookListManage();
        List<AbstractItem> testBookList;
        BookListManage invalidBLM;
        List<string> testFilteredItems = new List<string>();
        Book testBook = new Book("bookname", "authorname", "publisher", "genre", DateTime.Now, null, 5, "1234");
        Account testAccount = new Admin("Danny", "password");

        // Checks the add book method
        [TestMethod]
        public void AddBookTestMethodValid()
        {
            testBookList = testBlManager.Books;
            int firstBookNum = testBlManager.Books.Count;
            testBlManager.Addbook("bookname", "authorname", "publisher", "genre", "Book", null, DateTime.Now, 5, "1234");

            Assert.IsTrue(testBlManager.Books.Count == (firstBookNum + 1));
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddBookTestMethodInvalid()
        {
            testBlManager.Addbook("bookname", "authorname", null, "genre", "Book", null, DateTime.Now, 5, "1234");
        }

        // Check if parameters are updated in RentBook Func
        [TestMethod]
        public void TestRentBookValid()
        {
            Assert.IsTrue(testBook.RentBook(testAccount.UserName));
        }
        [TestMethod]
        public void TestRentBookInvalid()
        {
            Assert.IsFalse(testBook.RentBook(""));
        }
        // Checks if filter is not null on valid and invalid when null input is written
        [TestMethod]
        public void TestFilterListsValid()
        {
            BookFilterManager testFilterManager = new BookFilterManager(testBlManager);
            List<string> testFilteredItems = new List<string>();
            testFilteredItems = testFilterManager.GetFilterResultsOfType(testFilterManager.FilterGenre);
            Assert.IsNotNull(testFilteredItems);
            foreach (var item in testFilteredItems)
            {
                Assert.IsNotNull(item);
            }
        }
        [TestMethod]
        public void TestFilterListsInvalid()
        {
            BookFilterManager testFilterManager = new BookFilterManager(testBlManager);
            testFilterManager.FilterGenre.Add(null);
            testFilteredItems = testFilterManager.GetFilterResultsOfType(testFilterManager.FilterGenre);
            Assert.IsNotNull(testFilteredItems);
            foreach (var item in testFilteredItems)
            {
                Assert.IsNotNull(item);
            }
        }

        // Tests the function that checks if BookListManage is empty
        [TestMethod]

        public void TestCheckIfEmptyFuncValid()
        {
            Assert.IsFalse(testBlManager.CheckIfEmpty(testBlManager));
        }
        [TestMethod]
        public void TestCheckIfEmptyFuncInvalid()
        {
            Assert.IsTrue(testBlManager.CheckIfEmpty(invalidBLM));
        }

        // Tests SubFilter
        [TestMethod]
        public void TestSubFilterValid()
        {
            BookFilterManager testFilterManager = new BookFilterManager(testBlManager);
            Assert.IsNotNull(testFilterManager.SubFilterResult(testBlManager.Books, "Automotive"));
        }
        [TestMethod]
        public void TestSubFilterInvalid()
        {
            BookFilterManager testFilterManager = new BookFilterManager(testBlManager);
            Assert.IsNull(testFilterManager.SubFilterResult(testBlManager.Books, ""));
        }

        // Test Remove function
        [TestMethod]
        public void TestRemoveBookValid()
        {
            Assert.IsTrue(ForTestingRemoveBook(testBlManager.Books, "Da Vinci Code"));
        }
        [TestMethod]
        public void TestRemoveBookInvalid()
        {
            Assert.IsFalse(ForTestingRemoveBook(testBlManager.Books, "Catcher in the rye"));
        }
        // Recreated as the same func just with a bool return to test
        private bool ForTestingRemoveBook(List<AbstractItem> bookList, string bookName)
        {
            foreach (AbstractItem item in bookList)
            {
                if (item.Name == bookName && item.IsRented == false)
                {
                    bookList.Remove(item);
                    return true;
                }
            }
            return false;
        }
    }
}
