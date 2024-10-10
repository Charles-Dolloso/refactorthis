using Microsoft.EntityFrameworkCore;
using RefactorThis.Domain.Entities;

namespace RefactorThis.Persistence.Context
{
    public class InvoiceDBContext : DbContext
    {
        public InvoiceDBContext(DbContextOptions<InvoiceDBContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
