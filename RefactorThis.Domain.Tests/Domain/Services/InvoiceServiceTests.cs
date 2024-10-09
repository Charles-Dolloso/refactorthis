using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;
using RefactorThis.Persistence.Interfaces;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoiceServiceTests
    {
        private readonly IInvoiceRepository _repo;
        private readonly IInvoiceService _service;

        public InvoiceServiceTests(IInvoiceRepository repo, IInvoiceService service)
        {
            _repo = repo;
            _service = service;
        }

        [Test]
        public async Task ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
        {
            var payment = new Payment();
            var failureMessage = "";

            try
            {
                var result = await _service.ProcessPayment(payment);
            }
            catch (InvalidOperationException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual("There is no invoice matching this payment", failureMessage);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            var invoice = new Invoice(_repo)
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = null
            };

            await _repo.Add(invoice);

            var payment = new Payment();

            var result = await _service.ProcessPayment(payment);

            Assert.AreEqual("no payment needed", result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var invoice = new Invoice(_repo)
            {
                Amount = 10,
                AmountPaid = 10,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };
            await _repo.Add(invoice);

            var payment = new Payment();

            var result = await _service.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            var invoice = new Invoice(_repo)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            await _repo.Add(invoice);

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = await _service.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var invoice = new Invoice(_repo)
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            await _repo.Add(invoice);

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = await _service.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            var invoice = new Invoice(_repo)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            await _repo.Add(invoice);

            var payment = new Payment()
            {
                Amount = 5
            };

            var result = await _service.ProcessPayment(payment);

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var invoice = new Invoice(_repo)
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>() { new Payment() { Amount = 10 } }
            };
            await _repo.Add(invoice);

            var payment = new Payment()
            {
                Amount = 10
            };

            var result = await _service.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public async void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            var invoice = new Invoice(_repo)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            await _repo.Add(invoice);

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = await _service.ProcessPayment(payment);

            Assert.AreEqual("another partial payment received, still not fully paid", result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            var invoice = new Invoice(_repo)
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            await _repo.Add(invoice);

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = await _service.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }
    }
}