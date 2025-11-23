namespace NinhBinhStore.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        // Thêm sản phẩm vào giỏ
        public void AddItem(CartItem newItem)
        {
            // Tìm xem sản phẩm đã có trong giỏ chưa
            var existingItem = Items.FirstOrDefault(x => x.Product.Id == newItem.Product.Id);

            if (existingItem != null)
            {
                // Nếu có rồi thì cộng dồn số lượng
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                // Nếu chưa có thì thêm mới
                Items.Add(newItem);
            }
        }

        // Xóa sản phẩm khỏi giỏ
        public void RemoveItem(int productId)
        {
            Items.RemoveAll(x => x.Product.Id == productId);
        }

        // Cập nhật số lượng (Thường dùng trong trang Giỏ hàng)
        public void UpdateQuantity(int productId, int newQuantity)
        {
            var item = Items.FirstOrDefault(x => x.Product.Id == productId);
            if (item != null)
            {
                if (newQuantity > 0)
                    item.Quantity = newQuantity;
                else
                    RemoveItem(productId);
            }
        }

        // Tổng tiền toàn bộ giỏ hàng
        public decimal TotalCartPrice => Items.Sum(x => x.TotalPrice);

        // Tổng số lượng sản phẩm (để hiển thị trên icon giỏ hàng)
        public int TotalItemCount => Items.Sum(x => x.Quantity);

        // Xóa hết giỏ hàng
        public void Clear()
        {
            Items.Clear();
        }
    }
}
