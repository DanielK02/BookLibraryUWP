using System;
using System.Xml.Serialization;

namespace BookManager
{
    [XmlInclude(typeof(Book))]
    [XmlInclude(typeof(Journal))]
    //[Serializable]

    public abstract class AbstractItem
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int RentPrice { get; set; }
        public string Genre { get; set; }
        public DateTime RentDate { get; set; }
        public DateTime PublishDate { get; set; }
        public string ImagePath { get; set; }
        public string ISBN { get; set; }
        public bool IsRented { get; set; }
        public string UserRenting { get; set; }
        public string Description
        {
            get
            {   // Description to show in a TextBlock about the book both in Librarian or User page.
                if (BookType == "Book")
                {
                    return $"{Name} is a {BookType} by author {Author}\nOriginally published on {PublishDate.ToShortDateString()} by publisher {Publisher}\n" +
                $"Genre: {Genre} \nRent price: {RentPrice} \n\nUser renting: {UserRenting}";

                }
                else
                {
                    return $"{Name} is a {BookType} Originally published on {PublishDate.ToShortDateString()} by publisher {Publisher}\n" + $"Genre: {Genre}\n" +
                        $"\nRent price: {RentPrice} \n\nUser renting: {UserRenting}";
                }
            }
        }
        private string BookType
        {
            get { return GetType().Name; }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool RentBook(string userRenting)
        {
            if (string.IsNullOrEmpty(userRenting)) return false;
            this.IsRented = true;
            this.UserRenting = userRenting;
            this.RentDate = DateTime.Now;
            return true;
        }

        public void ReturnBook()
        {
            this.IsRented = false;
            this.UserRenting = default;
            this.RentDate = default;
        }
    }
}
