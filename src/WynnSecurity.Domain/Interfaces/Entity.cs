using System;

namespace WynnSecurity.Domain.Interfaces
{
    public abstract class Entity
    {
        public long Id { get; private set; }

        public DateTime CreatedDateTimeUtc { get; internal set; } = DateTime.UtcNow;

        public DateTime? UpdatedDateTimeUtc { get; internal set; }

        public long CreatedBy { get; internal set; }

        public long UpdatedBy { get; internal set; }
    }
}
