using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExclusivaAutos.Infraestructure.Configuration
{
    public class ExternalApiSettings
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TennantId { get; set; }
        public string Scope { get; set; }
        public string Authority { get; set; }
        public int TimeoutSeconds { get; set; } = 30;

    } 
}
