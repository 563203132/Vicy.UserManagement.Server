namespace Vicy.UserManagement.Server.Domain.Shared
{
    public interface IHandler<in TEvent> where TEvent : IDomainEvent
    {
        void Handle(TEvent domainEvent);
    }
}
