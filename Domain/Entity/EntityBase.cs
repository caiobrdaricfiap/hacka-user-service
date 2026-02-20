namespace Domain.Entity
{
    public class EntityBase
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}