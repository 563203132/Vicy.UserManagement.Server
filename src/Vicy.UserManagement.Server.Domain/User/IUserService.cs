namespace Vicy.UserManagement.Server.Domain
{
    public interface IUserService
    {
        User Create(string firstName, string lastName, string email, string phoneNumber);

        User Edit(long Id, string firstName, string lastName, string email, string phoneNumber);
    }
}
