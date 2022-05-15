namespace LoginLogic
{
    public class Admin : Account
    {

        public Admin(string adminName, string adminPass)
        {
            UserName = adminName;
            Password = adminPass;
        }
        public Admin()
        {

        }
    }
}
