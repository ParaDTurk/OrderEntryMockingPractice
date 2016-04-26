using NUnit;
using NSubstitute;
using NUnit.Framework;
using OrderEntryMockingPractice.Models;
using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPracticeTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private ICustomerRepository _fakeCustomerRepository;
        private IEmailService _fakeEmailService;
        private OrderService _orderService;
        private IOrderFulfillmentService _fakeOrderFulfillmentService;
        private IProductRepository _fakeProductRepository;
        private ITaxRateService _fakeTaxRateService;

        [SetUp]
        public void SetUp()
        {
            _fakeCustomerRepository = Substitute.For <ICustomerRepository>();
            _fakeEmailService = Substitute.For<IEmailService>();
            _fakeOrderFulfillmentService = Substitute.For<IOrderFulfillmentService>();
            _fakeProductRepository = Substitute.For<IProductRepository>();
            _fakeTaxRateService = Substitute.For<ITaxRateService>();
            _orderService = new OrderService(_fakeCustomerRepository, _fakeEmailService, _fakeOrderFulfillmentService, _fakeProductRepository, _fakeTaxRateService);
        }
    }
}
