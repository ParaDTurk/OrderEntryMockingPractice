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
            OrderSummary orderSummary = new OrderSummary();
            return orderSummary;
        }
    }
}
