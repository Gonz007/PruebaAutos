using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExclusivaAutos.Domain.Entities;

namespace ExclusivaAutos.Domain.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByDocumentAsync(string document);
    }
}
