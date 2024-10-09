using RefactorThis.Persistence.Entities;
using RefactorThis.Persistence.Interfaces;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private Invoice _invoice;

        public async Task<Invoice> GetInvoice(string reference)
        {
            return _invoice;
        }

        public async Task SaveInvoice(Invoice invoice)
        {
            //saves the invoice to the database
        }

        public async Task Add(Invoice invoice)
        {
            _invoice = invoice;
        }
    }
}