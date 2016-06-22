using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using NUnit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
                Rate = 1.0m
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

            
        }

        public Order CreateValidOrder()
        {
            var order = new Order
            {
                CustomerId = 11,
                OrderItems = new List<OrderItem>()
            };

            order.OrderItems.Add(_orderItem1);
            order.OrderItems.Add(_orderItem2);

            _orderFulfillmentService.Fulfill(order).Returns(_orderConfirmation);
            _taxRateService.GetTaxEntries(Arg.Any<String>(), Arg.Any<String>()).Returns(new[] {_taxEntry});
            _orderFulfillmentService.Fulfill(order).Returns(_orderConfirmation);
            _productRepository.IsInStock(Arg.Any<String>()).Returns(true);
            _customerRepository.Get(Arg.Any<int>()).Returns(_customer);

            return order;
        }

        public Order CreateOrder()
        {
            var order = new Order
            {
                CustomerId = 11,
                OrderItems = new List<OrderItem>()
            };

            return order;
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

        [Test]
        public void PlaceOrderWithDupeSKUThrowsException()
        {
            //Arrange
            var order = CreateOrder();
            order.OrderItems.Add(_orderItem1);
            order.OrderItems.Add(_orderItem1);

            //Act & Assert
            Should.Throw<InvalidOperationException>(() =>_orderService.PlaceOrder(order));
        }

        [Test]
        public void PlaceOrderWithItemsOutOfStockThrowsException()
        {
            //Arrange
            var order = CreateValidOrder();
            _productRepository.IsInStock(_orderItem2.Product.Sku).Returns(false);

            //Act & Assert
            Should.Throw<InvalidOperationException>(() => _orderService.PlaceOrder(order));
        }

        [Test]
        public void PlacedOrderIsSubmittedToOrderFullfillmentService()
        {
            //Arrange
            var order = CreateValidOrder();

            //Act
            var placedOrder = _orderService.PlaceOrder(order);

            //Assert
            _orderFulfillmentService.Received().Fulfill(order);
        }

        [Test]
        public void PlacedOrderSummaryContainsFullfillmentConfirmationNumber()
        {
            //Arrange
            var order = CreateValidOrder();

            //Assert
            var placedOrderSummary = _orderService.PlaceOrder(order);

            //Act
            placedOrderSummary.OrderNumber.ShouldBe("Eleven");
        }

        [Test]
        public void PlacedOrderSummaryContainsId()
        {
            //Arrange
            var order = CreateValidOrder();

            //Assert
            var placedOrderSummary = _orderService.PlaceOrder(order);

            //Act
            placedOrderSummary.OrderId.ShouldBe(11);
        }

        [Test]
        public void PlacedOrderSummaryContainsTaxesFromTaxRateService()
        {
            //Arrange
            var order = CreateValidOrder();
        
            //Assert
            var placedOrderSummary = _orderService.PlaceOrder(order);
        
            //Act
            _taxRateService.Received().GetTaxEntries("90210", "USA");
            placedOrderSummary.Taxes.ShouldBe(new []{_taxEntry});
        }

        [Test]
        public void PlacedOrderGetsCustomerInfoFromCustomerRepository()
        {
            //Arrange
            var order = CreateValidOrder();

            //Assert
            var placedOrder = _orderService.PlaceOrder(order);

            //Act
            _customerRepository.Received().Get(11);
            placedOrder.CustomerId.ShouldBe(11);
        }

        [Test]
        public void PlacedOrderSummaryContainsValidNetTotal()
        {
            //Arrange
            var order = CreateValidOrder();

            //Assert
            var placedOrderSummary = _orderService.PlaceOrder(order);

            //Act
            placedOrderSummary.NetTotal.ShouldBe(200m);
        }

        [Test]
        public void PlacedOrderSummaryContainsValidOrderTotal()
        {
            //Arrange
            var order = CreateValidOrder();

            //Assert
            var placedOrderSummary = _orderService.PlaceOrder(order);

            //Act
            placedOrderSummary.Total.ShouldBe(400m);
        }
    }
}
