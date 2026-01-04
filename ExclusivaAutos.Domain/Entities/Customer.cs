using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExclusivaAutos.Domain.Entities
{
    public class Customer
    {
        public bool Exists { get; private set; } = true;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Age { get; set; }
        public string City { get; set; }
        
        public static Customer FromResponse(string firstName, string lastname,
            string email, int age, string city)
        {
            return new Customer
            {
                FirstName = firstName,
                LastName = lastname,
                Email = email,
                Age = age.ToString(),
                City = city
            };
        }
    }
}
