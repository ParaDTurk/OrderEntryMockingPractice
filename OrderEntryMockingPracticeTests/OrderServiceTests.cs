using System.Collections.Generic;
using System.Linq;
using System.Security;
using NUnit;
using NSubstitute;
using NUnit.Framework;
using Shouldly;
using OrderEntryMockingPractice.Models;
using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPracticeTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private ICustomerRepository _fakeCustomerRepository;
        private IEmailService _fakeEmailService;
        private IOrderFulfillmentService _fakeOrderFulfillmentService;
        private IProductRepository _fakeProductRepository;
        private ITaxRateService _fakeTaxRateService;
        private OrderService _orderService;
        private OrderItem _fakeItem1;
        private Order _fakeOrder1;
        private Customer _fakeCustomer;
        private TaxEntry _fakeTaxEntry;

        [SetUp]
        public void SetUp()
        {
            _fakeCustomerRepository = Substitute.For<ICustomerRepository>();
            _fakeEmailService = Substitute.For<IEmailService>();
            _fakeOrderFulfillmentService = Substitute.For<IOrderFulfillmentService>();
            _fakeProductRepository = Substitute.For<IProductRepository>();
            _fakeTaxRateService = Substitute.For<ITaxRateService>();

            _orderService = new OrderService(_fakeCustomerRepository, 
                                             _fakeEmailService, 
                                             _fakeOrderFulfillmentService, 
                                             _fakeProductRepository, 
                                             _fakeTaxRateService);

            _fakeItem1 = new OrderItem
            {
                Product = new Product
                {
                    Price = 10.00m,
                    Sku = "fakeItem1"
                },
                Quantity = 1
            };

            _fakeOrder1 = new Order
            {
                CustomerId = 1,
                OrderItems = new List<OrderItem>()
            };

            _fakeCustomer = new Customer()
            {
                CustomerId = 1,
                CustomerName = "fake",
                City = "Seattle",
                StateOrProvince = "WA",
                Country = "USA",
                PostalCode = "90101"
            };

            _fakeTaxEntry = new TaxEntry
            {
                Description = "fakeTaxEntry1",
                Rate = 3.0m
            };

            _fakeOrder1.OrderItems.Add(_fakeItem1);
        }

        public OrderSummary PlaceOrder()
        {
            _fakeCustomerRepository.Get(1).Returns(_fakeCustomer);

            return _orderService.PlaceOrder(_fakeOrder1);
        }

        [Test]
        public void TestTest()
        {
            int test = 10;

            test.ShouldBe(10);
        }
    }
}
