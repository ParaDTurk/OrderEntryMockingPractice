using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using NUnit;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Shouldly;
using OrderEntryMockingPractice.Models;
using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPracticeTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private ICustomerRepository _customerRepository;
        private IEmailService _emailService;
        private IOrderFulfillmentService _orderFulfillmentService;
        private IProductRepository _productRepository;
        private ITaxRateService _taxRateService;

        private Order _order;
        private OrderService _orderService;
        private OrderConfirmation _orderConfirmation;
        private TaxEntry _taxEntry;
        private Customer _customer;
        private OrderItem _orderItem1;
        private OrderItem _orderItem2;

        [SetUp]
        public void SetUp()
        {
            _customerRepository = Substitute.For<ICustomerRepository>();
            _emailService = Substitute.For<IEmailService>();
            _orderFulfillmentService = Substitute.For<IOrderFulfillmentService>();
            _productRepository = Substitute.For<IProductRepository>();
            _taxRateService = Substitute.For<ITaxRateService>();

            _orderService = new OrderService(   _customerRepository, 
                                                _emailService, 
                                                _orderFulfillmentService, 
                                                _productRepository, 
                                                _taxRateService);

            _orderConfirmation = new OrderConfirmation
            {   
                CustomerId = 11,
                OrderId = 11,
                OrderNumber = "Eleven"
            };

            _taxEntry = new TaxEntry()
            {
                Description = "Crazu",
                Rate = (decimal) 0.20
            };

            _customer = new Customer
            {
                CustomerId = 11,
                CustomerName = "Bob",
                PostalCode = "90210",
                Country = "USA"
            };

            _orderItem1 = new OrderItem()
            {
                Product = new Product()
                {
                    Price = 100m,
                    Sku = "123"
                },
                Quantity = 1m
            };

            _orderItem2 = new OrderItem()
            {
                Product = new Product()
                {
                    Price = 100m,
                    Sku = "456"
                },
                Quantity = 1m
            };

            _order = new Order
            {
                CustomerId = 11,
                OrderItems = new List<OrderItem>()
            };

            _order.OrderItems.Add(_orderItem1);
            _order.OrderItems.Add(_orderItem2);
        }

        public Order CreateValidOrder()
        {
            _orderFulfillmentService.Fulfill(_order).Returns(_orderConfirmation);
            _taxRateService.GetTaxEntries(Arg.Any<String>(), Arg.Any<String>()).Returns(new[] {_taxEntry});
            _orderFulfillmentService.Fulfill(_order).Returns(_orderConfirmation);
            _productRepository.IsInStock(Arg.Any<String>()).Returns(true);

            return _order;
        }

        [Test]
        public void PlacedValidOrderReturnsOrderSummary()
        {
            //Arrange
            var order = CreateValidOrder();

            //Act
            var placedOrder = _orderService.PlaceOrder(order);

            //Assert
            placedOrder.ShouldBeOfType<OrderSummary>();
        }
    }
}
