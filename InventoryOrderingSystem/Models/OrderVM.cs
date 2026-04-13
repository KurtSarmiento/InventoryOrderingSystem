namespace InventoryOrderingSystem.Models
{
    public class OrderVM
    {
        public int CustomerId { get; set; }
        public List<OrderItemVM> Items { get; set; } = new List<OrderItemVM>();
        public List<InventoryOrderingSystem.Models.Database.Customer>? AllCustomers { get; set; }
        public List<InventoryOrderingSystem.Models.Database.Product>? AllProducts { get; set; }
    }

    public class OrderItemVM
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}