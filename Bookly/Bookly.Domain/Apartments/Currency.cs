using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Domain.Apartments
{
    public class Currency
    {
        internal static readonly Currency None = new Currency("");
        public static readonly Currency Usd = new Currency("USD");
        public static readonly Currency Eur = new Currency("EUR");

        public static Currency FromCode(string code)
        {
            return All.FirstOrDefault(x => x.Code == code)
                 ?? throw new ApplicationException($"Currency with code {code} not found");
        }
        private Currency(string code)
        {
            Code = code;
        }

        public string Code { get; init; }


        public static readonly IReadOnlyCollection<Currency> All = new[] { Usd, Eur };
    }
}
