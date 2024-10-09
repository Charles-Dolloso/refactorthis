using System;
using System.Linq;
using System.Threading.Tasks;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;
using RefactorThis.Persistence.Enums;
using RefactorThis.Persistence.Interfaces;

namespace RefactorThis.Domain
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<string> ProcessPayment(Payment payment)
        {
            var invoice = await _invoiceRepository.GetInvoice(payment.Reference);
            if (invoice == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment.");
            }

            if (invoice.Amount == 0)
            {
                return HandleZeroAmountInvoice(invoice);
            }

            if (invoice.Payments != null && invoice.Payments.Any())
            {
                return HandleExistingPayments(invoice, payment);
            }

            return HandleNoPayments(invoice, payment);
        }

        private string HandleZeroAmountInvoice(Invoice invoice)
        {
            if (invoice.Payments == null || !invoice.Payments.Any())
            {
                return "No payment needed.";
            }

            throw new InvalidOperationException("The invoice is in an invalid state: it has an amount of 0 and existing payments.");
        }

        private string HandleExistingPayments(Invoice invoice, Payment payment)
        {
            var totalPaid = invoice.Payments.Sum(x => x.Amount);

            if (totalPaid == invoice.Amount)
            {
                return "Invoice was already fully paid.";
            }

            if (payment.Amount > (invoice.Amount - totalPaid))
            {
                return "The payment is greater than the partial amount remaining.";
            }

            return ProcessPartialPayment(invoice, payment);
        }

        private string HandleNoPayments(Invoice invoice, Payment payment)
        {
            if (payment.Amount > invoice.Amount)
            {
                return "The payment is greater than the invoice amount.";
            }

            if (payment.Amount == invoice.Amount)
            {
                return MarkInvoiceAsFullyPaid(invoice, payment);
            }

            return MarkInvoiceAsPartiallyPaid(invoice, payment);
        }

        private string ProcessPartialPayment(Invoice invoice, Payment payment)
        {
            if (payment.Amount == (invoice.Amount - invoice.AmountPaid))
            {
                return MarkInvoiceAsFullyPaid(invoice, payment);
            }

            return AddPartialPayment(invoice, payment);
        }

        private string MarkInvoiceAsFullyPaid(Invoice invoice, Payment payment)
        {
            ApplyPayment(invoice, payment);
            invoice.AmountPaid += payment.Amount;
            return "Final partial payment received, invoice is now fully paid.";
        }

        private string AddPartialPayment(Invoice invoice, Payment payment)
        {
            ApplyPayment(invoice, payment);
            invoice.AmountPaid += payment.Amount;
            return "Another partial payment received, still not fully paid.";
        }

        private string MarkInvoiceAsPartiallyPaid(Invoice invoice, Payment payment)
        {
            ApplyPayment(invoice, payment);
            invoice.AmountPaid = payment.Amount;
            return "Invoice is now partially paid.";
        }

        private void ApplyPayment(Invoice invoice, Payment payment)
        {
            switch (invoice.Type)
            {
                case InvoiceTypeEnum.Standard:
                    invoice.Payments.Add(payment);
                    break;
                case InvoiceTypeEnum.Commercial:
                    invoice.Payments.Add(payment);
                    invoice.TaxAmount += payment.Amount * 0.14m;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}