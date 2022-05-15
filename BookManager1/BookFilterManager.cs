using System.Collections.Generic;

namespace BookManager
{
    public class BookFilterManager
    {
        BookListManage bookManage;

        List<string> booksGenre = new List<string>();
        List<string> booksAuthor = new List<string>();
        List<string> booksPublisher = new List<string>();

        public BookFilterManager(BookListManage bl)
        {
            bookManage = bl;

            foreach (var item in bookManage.Books)
            {
                if (booksGenre.Contains(item.Genre) == false && item.Genre != null)
                {
                    booksGenre.Add(item.Genre);
                }
                if (booksAuthor.Contains(item.Author) == false && item.Author != null)
                {
                    booksAuthor.Add(item.Author);
                }
                if (booksPublisher.Contains(item.Publisher) == false && item.Publisher != null)
                {
                    booksPublisher.Add(item.Publisher);
                }
            }
        }
        public List<string> FilterGenre
        {
            get { return booksGenre; }
        }
        public List<string> FilterAuthor
        {
            get { return booksAuthor; }
        }
        public List<string> FilterPublisher
        {
            get { return booksPublisher; }
        }

        // Get the types of a selected filter, for example: Author names or Genres
        public List<string> GetFilterResultsOfType(List<string> filterType)
        {
            List<string> filteredItems = new List<string>();

            foreach (string item in filterType)
            {
                if (item == null) continue;
                filteredItems.Add(item);
            }

            return filteredItems;
        }
        // Filter the list by the selected Sub-Filter, for example it will show only books by Author x
        public List<string> SubFilterResult(List<AbstractItem> books, string filter)
        {
            List<string> filteredItems = new List<string>();

            foreach (AbstractItem item in books)
            {
                if (string.IsNullOrEmpty(filter)) return null;
                else if (filter.ToString() == item.Genre)
                    filteredItems.Add(item.Name);
                else if (filter.ToString() == item.Author)
                    filteredItems.Add(item.Name);
                else if (filter.ToString() == item.Publisher)
                    filteredItems.Add(item.Name);
            }
            return filteredItems;
        }

    }



}
