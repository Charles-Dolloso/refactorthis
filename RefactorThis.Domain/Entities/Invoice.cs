using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Enums;
using System.ComponentModel.DataAnnotations;

namespace RefactorThis.Domain.Entities
{
    public class Invoice
    {
        [Key]
        public Guid InvoiceID { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public InvoiceTypeEnum Type { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
