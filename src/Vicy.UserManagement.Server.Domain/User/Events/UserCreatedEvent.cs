using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.Domain
{
    public class UserCreatedEvent : DomainEvent
    {
        public long UserId { get; set; }
    }
}
