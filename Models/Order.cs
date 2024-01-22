namespace backend.Models
{
    public enum OrderStatus
    {
        Pending,
        Shipped
    }

    public class Order
    {
        public int OrderID { get; set; }
        public DateTime DateTime { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public int UserID { get; set; }
        public Order() { }

        public Order(int orderID, DateTime OrderDateTime, decimal TotalPrice, OrderStatus OrderStatus, int UserID)
        {
            OrderID = orderID;
            DateTime = OrderDateTime;
            TotalPrice = TotalPrice;
            Status = OrderStatus;
            UserID = UserID;
        }
    }
}
