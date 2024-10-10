using System.ComponentModel.DataAnnotations;

namespace RefactorThis.Domain.Entities
{
    public class Payment
    {
        [Key]
        public Guid PaymentID { get; set; }
        public Guid InvoiceID { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }

        public Invoice Invoice { get; set; }
    }
}