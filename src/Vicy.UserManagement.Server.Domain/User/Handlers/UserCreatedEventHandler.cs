using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.Domain
{
    public class UserCreatedEventHandler : IHandler<UserCreatedEvent>
    {
        public UserCreatedEventHandler()
        {
        }

        public void Handle(UserCreatedEvent domainEvent)
        {
            // Do Something
        }
    }
}
