using System.ComponentModel.DataAnnotations;

namespace Vicy.UserManagement.Server.Domain.Shared
{
    public abstract class ConcurrencyAggregateRoot : AggregateRoot
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
