namespace EFCoreBook.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual List<Order> Orders { get; set; } = new();
    }
}