using Moq;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Repositories;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoiceServiceTests
    {
        private Mock<IInvoiceRepository> _mockInvoiceRepository;
        private InvoiceService _invoiceService;

        [SetUp]
        public void SetUp()
        {
            _mockInvoiceRepository = new Mock<IInvoiceRepository>();
            _invoiceService = new InvoiceService(_mockInvoiceRepository.Object);
        }

        [Test]
        public async Task ProcessPayment_InvoiceNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = new Payment { Reference = "123", Amount = 100 };
            _mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).ReturnsAsync((Invoice)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _invoiceService.ProcessPaymentAsync(payment));
            Assert.AreEqual("There is no invoice matching this payment", ex.Message);
        }

        [Test]
        public async Task ProcessPayment_InvoiceAmountIsZero_ReturnsNoPaymentNeeded()
        {
            // Arrange
            var invoice = new Invoice { Amount = 0, AmountPaid = 0, Payments = new List<Payment>() };
            var payment = new Payment { Reference = "123", Amount = 100 };
            _mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual("No payment needed.", result);
        }

        [Test]
        public async Task ProcessPayment_InvoiceFullyPaid_ReturnsInvoiceAlreadyFullyPaid()
        {
            // Arrange
            var invoice = new Invoice
            {
                Amount = 100,
                AmountPaid = 100,
                Payments = new List<Payment> { new Payment { Amount = 100 } }
            };
            var payment = new Payment { Reference = "123", Amount = 100 };
            _mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual("Invoice was already fully paid", result);
        }

        [Test]
        public async Task ProcessPayment_PaymentGreaterThanRemaining_ReturnsPaymentGreaterThanRemaining()
        {
            // Arrange
            var invoice = new Invoice
            {
                Amount = 200,
                AmountPaid = 100,
                Payments = new List<Payment> { new Payment { Amount = 100 } }
            };
            var payment = new Payment { Reference = "123", Amount = 150 };
            _mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual("The payment is greater than the partial amount remaining", result);
        }

        [Test]
        public async Task ProcessPayment_PartialPayment_UpdatesInvoiceAndReturnsPartialPaymentMessage()
        {
            // Arrange
            var invoice = new Invoice
            {
                Amount = 200,
                Payments = new List<Payment> { new Payment { Amount = 100 } },
                AmountPaid = 100
            };
            var payment = new Payment { Reference = "123", Amount = 50 };
            _mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual("Another partial payment received, still not fully paid", result);
        }

        [Test]
        public async Task ProcessPayment_PaymentEqualsRemaining_InvoiceFullyPaid()
        {
            // Arrange
            var invoice = new Invoice
            {
                Amount = 200,
                Payments = new List<Payment> { new Payment { Amount = 100 } },
                AmountPaid = 100
            };
            var payment = new Payment { Reference = "123", Amount = 100 };
            _mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual("Final partial payment received, invoice is now fully paid", result);
        }

        [Test]
        public async Task ProcessPayment_PaymentGreaterThanInvoiceAmount_ReturnsPaymentGreaterThanInvoiceAmount()
        {
            // Arrange
            var invoice = new Invoice { Amount = 100, AmountPaid = 100, Payments = new List<Payment>() };
            var payment = new Payment { Reference = "123", Amount = 150 };
            _mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual("The payment is greater than the invoice amount", result);
        }
    }
}