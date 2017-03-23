namespace Vicy.UserManagement.Server.Domain.Shared
{
    public interface IEventDispatcher
    {
        void Dispatch<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    }
}
