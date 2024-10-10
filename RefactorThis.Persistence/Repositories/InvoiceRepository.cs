using Microsoft.EntityFrameworkCore;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Context;

namespace RefactorThis.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceDBContext _context;

        public InvoiceRepository(InvoiceDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Invoice?> GetInvoice(string reference)
        {
            var payment = await _context.Payments.Include(i => i.Invoice).Where(c => c.Reference == reference).FirstOrDefaultAsync();
            if (payment != null)
            {
                return payment.Invoice;
            }

            return null;
        }

        public async Task SaveInvoice(Invoice invoice)
        {
            await _context.SaveChangesAsync();
        }

        public async Task Add(Invoice invoice)
        {
            await _context.AddAsync(invoice);
        }
    }
}