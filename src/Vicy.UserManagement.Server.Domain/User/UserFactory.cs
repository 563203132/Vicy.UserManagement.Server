namespace Vicy.UserManagement.Server.Domain
{
    public class UserFactory : IUserFactory
    {
        public User Create(string firstName, string lastName, string email, string phoneNumber)
        {
            return new User(firstName, lastName, email, phoneNumber);
        }
    }
}
