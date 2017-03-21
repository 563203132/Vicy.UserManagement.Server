namespace Vicy.UserManagement.Server.Domain
{
    public interface IUserFactory
    {
        User Create(
            string firstName,
            string lastName,
            string email,
            string phoneNumber);
    }
}
