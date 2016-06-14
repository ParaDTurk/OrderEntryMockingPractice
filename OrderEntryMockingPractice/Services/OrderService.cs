using System.Linq;
using OrderEntryMockingPractice.Models;

namespace OrderEntryMockingPractice.Services
{
    public class OrderService
    {
        private ICustomerRepository _customerRepository;
        private IOrderFulfillmentService _orderFulfillmentService;
        private IEmailService _emailService;
        private IProductRepository _productRepository;
        private ITaxRateService _taxRateService;

        public OrderService (ICustomerRepository customerRepository, 
                             IEmailService emailService, 
                             IOrderFulfillmentService orderFulfillmentService, 
                             IProductRepository productRepository, 
                             ITaxRateService taxRateService)
        {
            _customerRepository = customerRepository;
            _orderFulfillmentService = orderFulfillmentService;
            _emailService = emailService;
            _productRepository = productRepository;
            _taxRateService = taxRateService;
        }

        public OrderSummary PlaceOrder(Order order)
        {
            ValidateOrder(order);

            OrderSummary orderSummary = new OrderSummary();

            return orderSummary;
        }

        private void ValidateOrder(Order order)
        {
            if (!SKUsAreUnique(order))
            {
                throw new System.InvalidOperationException("Duplicate SKU found.");
            }

            if (!ProductsInStock(order))
            {
                throw new System.InvalidOperationException("Product not in stock.");
            }
        }

        private bool ProductsInStock(Order order)
        {
            return order.OrderItems.All(item => _productRepository.IsInStock(item.Product.Sku));
        }

        private bool SKUsAreUnique(Order order)
        {
            return order.OrderItems.Distinct().Count() == order.OrderItems.Count;
        }
    }
}
