namespace NinhBinhStore.Models
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem() { }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        // Tính tổng tiền của item này (Đơn giá * Số lượng)
        public decimal TotalPrice => Product.Dongia * Quantity;
    }
}
