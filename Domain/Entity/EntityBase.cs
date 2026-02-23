using System;

namespace Domain.Entity
{
    public class EntityBase
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}