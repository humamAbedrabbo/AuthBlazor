namespace AuthBlazor.Authentication
{
    public class UserAccountService
    {
        private List<UserAccount> _users;

        public UserAccountService()
        {
            _users = new List<UserAccount>()
            {
                new UserAccount() { UserName = "admin", Password = "admin", Role = "Administrator" },
                new UserAccount() { UserName = "user", Password = "user", Role = "User" },
            };
        }

        public UserAccount? GetUserAccount(string userName, string password)
        {
            return _users.FirstOrDefault(x => (x.UserName.ToUpper() == userName?.ToUpper()) 
            && (x.Password == password));
        }

    }
}
