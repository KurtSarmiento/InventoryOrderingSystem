namespace InventoryOrderingSystem.Models
{
    public class OrderVM
    {
        public int CustomerId { get; set; }
        public List<OrderItemVM> Items { get; set; } = new List<OrderItemVM>();
    }

    public class OrderItemVM
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
