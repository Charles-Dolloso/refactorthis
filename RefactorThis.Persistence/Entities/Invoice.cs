using System.Collections.Generic;
using System.Threading.Tasks;
using RefactorThis.Persistence.Enums;
using RefactorThis.Persistence.Interfaces;
using RefactorThis.Persistence.Repositories;

namespace RefactorThis.Persistence.Entities
{
    public class Invoice
    {
        private readonly IInvoiceRepository _repository;
        public Invoice(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task Save()
        {
            await _repository.SaveInvoice(this);
        }

        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public List<Payment> Payments { get; set; }

        public InvoiceTypeEnum Type { get; set; }
    }
}