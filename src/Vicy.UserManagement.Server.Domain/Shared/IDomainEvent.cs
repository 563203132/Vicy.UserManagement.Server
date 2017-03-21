using System;

namespace Vicy.UserManagement.Server.Domain.Shared
{
    public interface IDomainEvent
    {
        DateTime DateTimeEventOccurred { get; }
    }
}
