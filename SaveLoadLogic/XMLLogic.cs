using BookManager;
using LoginLogic;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SaveLoadLogic
{
    public class XMLLogic
    {
        private string filePath = $@"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\ItemsList.xml";
        private string filePathAccounts = $@"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\AccountsList.xml";

        #region BookListSaveLoad
        // Load method for List of AbstractItems(Books/Journals)
        public List<AbstractItem> Load()
        {
            List<AbstractItem> items = LoadBookList();
            return items;
        }
        private List<AbstractItem> LoadBookList()
        {
            if (File.Exists(filePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<AbstractItem>));
                FileStream stream = new FileStream(filePath, FileMode.Open);
                List<AbstractItem> items = (List<AbstractItem>)xmlSerializer.Deserialize(stream);
                stream.Close();
                return items;
            }
            else return null;
        }

        // Save method for List of AbstractItems(Books/Journals)
        public void Save(List<AbstractItem> items)
        {
            SaveBookList(items);
        }
        private void SaveBookList(List<AbstractItem> items)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<AbstractItem>));
            FileStream stream = new FileStream(filePath, FileMode.Create);
            xmlSerializer.Serialize(stream, items);
            stream.Close();
        }
        #endregion

        #region AccountListSaveLoad
        // Load method for List of Accounts(Admin/User)
        public List<Account> LoadAccounts()
        {
            List<Account> accountList = LoadAccountList();
            return accountList;
        }
        private List<Account> LoadAccountList()
        {
            if (File.Exists(filePathAccounts))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Account>));
                FileStream stream = new FileStream(filePathAccounts, FileMode.Open);
                List<Account> items = (List<Account>)xmlSerializer.Deserialize(stream);
                stream.Close();
                return items;
            }
            else return null;
        }
        // Save method for List of Accounts(Admin/User)

        public void SaveAccounts(List<Account> items)
        {
            SaveAccountList(items);
        }
        private void SaveAccountList(List<Account> items)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Account>));
            FileStream stream = new FileStream(filePathAccounts, FileMode.Create);
            xmlSerializer.Serialize(stream, items);
            stream.Close();
        }
        #endregion
    }
}
