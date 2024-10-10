using RefactorThis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Interfaces
{
    public interface IInvoiceService
    {
        Task<string> ProcessPaymentAsync(Payment payment);
    }
}
