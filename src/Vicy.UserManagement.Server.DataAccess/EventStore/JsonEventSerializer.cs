using System;
using Newtonsoft.Json;
using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.DataAccess.EventStore
{
    public class JsonEventSerializer : IEventSerializer
    {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };

        public IDomainEvent Deserialize(string json, string eventType)
        {
            var type = Type.GetType(eventType);
            return (IDomainEvent)JsonConvert.DeserializeObject(json, type);
        }

        public string Serialize(IDomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(domainEvent, Formatting.None, _serializerSettings);
        }
    }
}
