using System;

namespace Vicy.UserManagement.Server.Domain.Shared
{
    public abstract class Entity
    {
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public long Id { get; private set; }

        public DateTime CreatedDateTimeUtc { get; internal set; } = DateTime.UtcNow;

        public DateTime? UpdatedDateTimeUtc { get; internal set; } = null;

        public string CreatedBy { get; internal set; }

        public string UpdatedBy { get; internal set; } = null;

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Entity;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Id <= 0 || other.Id <= 0)
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
