namespace EFCoreBook.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}