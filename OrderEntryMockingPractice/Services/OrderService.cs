using System.Collections.Generic;
using System.Linq;
using OrderEntryMockingPractice.Models;

namespace OrderEntryMockingPractice.Services
{
    public class OrderService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderFulfillmentService _orderFulfillmentService;
        private readonly IEmailService _emailService;
        private readonly IProductRepository _productRepository;
        private readonly ITaxRateService _taxRateService;

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

            var fulfilledOrder = _orderFulfillmentService.Fulfill(order);
            var customer = _customerRepository.Get(fulfilledOrder.CustomerId);
            
            OrderSummary orderSummary = new OrderSummary()
            {
                OrderNumber = fulfilledOrder.OrderNumber,
                OrderId = fulfilledOrder.OrderId,
                CustomerId = fulfilledOrder.CustomerId,
                Taxes = _taxRateService.GetTaxEntries(customer.PostalCode, customer.Country)
            };

            orderSummary.NetTotal = CalculateNetTotal(order);
            orderSummary.Total = CalculateOrderTotal(order, orderSummary.Taxes);

            _emailService.SendOrderConfirmationEmail(orderSummary.CustomerId, orderSummary.OrderId);

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

        private decimal CalculateNetTotal(Order order)
        {
            decimal total = 0;
            foreach (OrderItem orderItem in order.OrderItems)
            {
                total += orderItem.Quantity * orderItem.Product.Price;
            }

            return total;
        }

        private decimal CalculateOrderTotal(Order order, IEnumerable<TaxEntry> taxes)
        {
            decimal total = CalculateNetTotal(order);
            foreach (TaxEntry taxEntry in taxes)
            {
                total += (taxEntry.Rate * total);
            }
            return total;
        }
    }
}
