using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExclusivaAutos.Application.DTOs;

namespace ExclusivaAutos.Application.Services
{
    public interface ICustomerApplitacionService
    {
        Task<CustomerDto> GetCustomerByDocumentAsync(string document);
    }
}
