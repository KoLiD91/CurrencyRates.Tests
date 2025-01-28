using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRates.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
        public string TableType { get; set; }
        public DateTime FetchDate { get; set; }
    }
}