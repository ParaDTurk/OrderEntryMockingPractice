using System.Collections.Generic;

namespace OrderEntryMockingPractice.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new List<OrderItem>();
        }
        
        public int? CustomerId { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public decimal CalculateNetTotal()
        {
            decimal total = 0;
            foreach (OrderItem orderItem in this.OrderItems)
            {
                total += orderItem.Quantity * orderItem.Product.Price;
            }

            return total;
        }

        public decimal CalculateOrderTotal(decimal NetTotal, IEnumerable<TaxEntry> taxes)
        {
            decimal total = NetTotal;
            foreach (TaxEntry taxEntry in taxes)
            {
                total += (taxEntry.Rate * NetTotal);
            }
            return total;
        }
    }
}
