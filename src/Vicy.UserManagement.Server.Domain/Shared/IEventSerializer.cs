namespace Vicy.UserManagement.Server.Domain.Shared
{
    public interface IEventSerializer
    {
        string Serialize(IDomainEvent domainEvent);

        IDomainEvent Deserialize(string json, string eventType);
    }
}
