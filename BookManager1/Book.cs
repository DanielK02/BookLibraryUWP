using System;

namespace BookManager
{
    public class Book : AbstractItem
    {
        public Book(string name, string author, string publisher, string genre,
            DateTime publishDate, string imagePath, int rentPrice, string isbn)
        {
            Name = name;
            Author = author;
            Publisher = publisher;
            Genre = genre;
            PublishDate = publishDate;
            ImagePath = imagePath;
            RentPrice = rentPrice;
            ISBN = isbn;
            IsRented = false;
        }
        public Book() // Empty constructor used for xmlSerialization
        {

        }

    }
}
