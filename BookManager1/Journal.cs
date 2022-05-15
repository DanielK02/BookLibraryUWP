using System;

namespace BookManager
{
    public class Journal : AbstractItem
    {

        public Journal(string name, string publisher, string genre,
            DateTime publishDate, string imagePath, int rentPrice, string isbn)
        {
            Name = name;
            Publisher = publisher;
            RentPrice = rentPrice;
            Genre = genre;
            PublishDate = publishDate;
            ImagePath = imagePath;
            ISBN = isbn;
            IsRented = false;
        }
        public Journal() // Empty constructor used for xmlSerialization
        {

        }
    }
}
