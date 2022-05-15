using System;
using System.Collections.Generic;
using System.Linq;

namespace BookManager
{
    public class BookListManage
    {
        List<AbstractItem> bookList = new List<AbstractItem>();
        public BookListManage()
        {
            bookList.Add(new Book("Da Vinci Code", "Dan Brown", "Doubleday", "Mystery", new DateTime(2003, 04, 01),
                "ms-appx:///Assets/DaVinci.jpg", 7, "0-385-50420-9"));

            bookList.Add(new Book("Angels & Demons", "Dan Brown", "Doubleday", "Mystery", new DateTime(2000, 05, 01),
                 "ms-appx:///Assets/AngelsDemons.jpg", 7, "0-671-02735-2"));

            bookList.Add(new Book("The Lost Symbol", "Dan Brown", "Doubleday", "Mystery", new DateTime(2009, 09, 15),
                 "ms-appx:///Assets/LostSymbol.jpg", 7, "0-385-50422-5"));

            bookList.Add(new Book("The Music Lesson", "Victor Wooten", "Berkley", "Music", new DateTime(2008, 04, 01),
                 "ms-appx:///Assets/MusicLesson.jpg", 7, "0425220931"));

            bookList.Add(new Book("The Hitchhiker's Guide to the Galaxy", "Douglas Adams", "Pan Books", "Comic science fiction", new DateTime(1979, 10, 12),
                 "ms-appx:///Assets/Hitchhiker.jpg", 7, "0-330-25864-8"));

            bookList.Add(new Book("The Master and Margarita", "Mikhail Bulgakov", "YMCA Press", "Novel, Fantasy, Supernatural, Satire", new DateTime(1967, 01, 01),
                 "ms-appx:///Assets/MasterMargarita.jpg", 7, "0679760806"));

            bookList.Add(new Book("Crime and Punishment", "Fyodor Dostoyevsky", "The Russian Messenger", "Crime Fiction", new DateTime(1866, 01, 01),
                 "ms-appx:///Assets/CrimeandPunish.jpg", 7, "0143107631"));

            bookList.Add(new Book("White Fang", "Jack London", "Macmillan", "Adventure", new DateTime(1906, 05, 01),
                 "ms-appx:///Assets/WhiteFang.jpg", 7, "978-1-85813-740-7"));

            bookList.Add(new Journal("Newtype Japan October 2010", "Kadokawa Shoten", "Anime and Manga", new DateTime(2010, 10, 01),
                 "ms-appx:///Assets/newtype.jpg", 5, "012305"));

            bookList.Add(new Journal("Rally Cars Vol.25 / Subaru Imprezza 555", "Saneishobo", "Automotive", new DateTime(2020, 01, 16),
                 "ms-appx:///Assets/RallyCars.jpg", 5, "978-4779640407"));

            bookList.Add(new Journal("Newtype USA November 2004", "AD Vision", "Anime and Manga", new DateTime(2004, 01, 01),
                 "ms-appx:///Assets/newtypeUSA.jpg", 5, "010203"));

            bookList.Add(new Journal("Car and Driver October 21", "Hearst", "Automotive", new DateTime(2021, 10, 01),
                 "ms-appx:///Assets/CarNDriver.jpg", 5, "0008-6002"));
        }

        public List<AbstractItem> Books
        {
            get => bookList;
            set => bookList = value;
        }

        #region AddBook Function, sorry for the mess, a lot of parameters.

        // A function that returns the last saved book in AddBook page, to be able to save a few
        // Will throw Null Exception that is catched in UI if null in fields
        public AbstractItem Addbook(string name, string author, string publisher, string genre,
            string bookType, string imagePath, DateTime dt, int rentPrice, string isbn)
        {
            if (CheckIfFieldsNotNull(name, author, publisher, genre, bookType, imagePath, dt, rentPrice, isbn))
            {
                throw new NullReferenceException("Field / Fields are empty!");
            }

            AbstractItem addedBook = PrivateAddbook(name, author, publisher, genre, bookType, imagePath, dt, rentPrice, isbn);

            if (addedBook == null)
            {
                throw new NullReferenceException("Null book, book wasn't saved.");
            }

            return addedBook;
        }

        // Private function for AddBook
        private AbstractItem PrivateAddbook(string name, string author, string publisher, string genre,
            string bookType, string imagePath, DateTime dt, int rentPrice, string isbn)
        {

            // Checks for null in BookType and if different bookType isn't Book/Journal, safety net.
            if (bookType == null)
            {
                throw new NullReferenceException("Book type string is null");
            }

            else if (bookType == "Book")
            {
                bookList.Add(new Book(name, author, publisher, genre, dt, imagePath, rentPrice, isbn));
            }
            else if (bookType == "Journal")
            {
                bookList.Add(new Journal(name, publisher, genre, dt, imagePath, rentPrice, isbn));
            }
            else
            {
                throw new NullReferenceException("Error in parameters, null book, book wasn't saved.");
            }

            if (bookList.Last() == null) throw new NullReferenceException("Error in parameters, null book, wasn't saved.");
            return bookList.Last();
        }

        // Checks for empty fields in AddBook function, returns true if empty or null, false if okay
        private bool CheckIfFieldsNotNull(string name, string author, string publisher, string genre,
            string bookType, string imagePath, DateTime dt, int rentPrice, string isbn)
        {

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(author) || string.IsNullOrEmpty(publisher)
                || string.IsNullOrEmpty(genre) || string.IsNullOrEmpty(bookType) || dt.Date == default
                || rentPrice <= 0 || string.IsNullOrEmpty(isbn))
            {
                return true;
            }
            else
                return false;

        }
        #endregion

        #region Remove Book Function
        // A Function that removes a book from the BookList
        public void RemoveBook(List<AbstractItem> bookList, string bookName)
        {
            if (bookList == null || string.IsNullOrEmpty(bookName)) return;
            PrivateRemoveBook(bookList, bookName);
        }
        private void PrivateRemoveBook(List<AbstractItem> bookList, string bookName)
        {
            foreach (AbstractItem item in bookList)
            {
                if (item.Name == bookName && item.IsRented == false)
                {
                    bookList.Remove(item);
                    break;
                }
            }
        }
        #endregion


        #region Selected AbstractItem
        // Function that gets the selected item in the listview
        // UI handles if null and shows an error
        public AbstractItem SelectedAbstractItem(List<AbstractItem> books, string selectedItem)
        {
            if (selectedItem == null || books == null) return null;

            AbstractItem selectedBook = PrivateSelectedAbstractItem(books, selectedItem);
            return selectedBook;
        }
        private AbstractItem PrivateSelectedAbstractItem(List<AbstractItem> books, string selectedItem)
        {
            if (selectedItem == null || books == null) return null;

            AbstractItem temp = null;
            foreach (AbstractItem item in books)
            {
                if (selectedItem == item.Name)
                    temp = item;
            }
            if (temp == null) throw new Exception();
            return temp;
        }
        #endregion

        // Checks if Books of BookListManage object is empty
        #region Check If BookList empty
        public bool CheckIfEmpty(BookListManage bookListManage)
        {
            if (bookListManage == null) return true;
            return PrivateCheckIfEmpty(Books);
        }
        private bool PrivateCheckIfEmpty(List<AbstractItem> books)
        {
            if (books.Count <= 0 || books == null) return true;
            else
                return false;
        }
        #endregion

        // Public functions because the following functions are Getters with a simple logic only to get an amount of books
        #region Functions that get amounts
        public int GetBookItemsAmount(List<AbstractItem> itemList)
        {
            int bookItemNum = 0;
            foreach (AbstractItem item in itemList)
            {
                bookItemNum++;
            }
            return bookItemNum;
        }
        public int GetBooksAmount(List<AbstractItem> itemList)
        {
            int bookNum = 0;
            foreach (AbstractItem item in itemList)
            {
                if (item is Book)
                    bookNum++;
            }
            return bookNum;
        }
        public int GetJournalAmount(List<AbstractItem> itemList)
        {
            int journalNum = 0;
            foreach (AbstractItem item in itemList)
            {
                if (item is Journal)
                    journalNum++;
            }
            return journalNum;
        }
        public int GetRentedBooksAmount(List<AbstractItem> books)
        {
            int rentedlNum = 0;
            foreach (AbstractItem item in books)
            {
                if (item.IsRented == true)
                    rentedlNum++;
            }
            return rentedlNum;
        }
        #endregion
    }

}
