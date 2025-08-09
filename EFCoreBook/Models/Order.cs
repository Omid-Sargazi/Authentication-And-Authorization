namespace EFCoreBook.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;


        public virtual List<Item> Items { get; set; } = new();
    }
}