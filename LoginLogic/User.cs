namespace LoginLogic
{
    public class User : Account
    {
        public User(string Name, string userPass)
        {
            UserName = Name;
            Password = userPass;
        }
        public User()
        {

        }
    }
}
