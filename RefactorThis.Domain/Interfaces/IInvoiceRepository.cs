using RefactorThis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetInvoice(string reference);
        Task SaveInvoice(Invoice invoice);
        Task Add(Invoice invoice);
    }
}
