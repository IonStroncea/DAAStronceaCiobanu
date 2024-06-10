namespace AuthService.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public DateTime LastModified { get; set; } = DateTime.Now;
    }
}
