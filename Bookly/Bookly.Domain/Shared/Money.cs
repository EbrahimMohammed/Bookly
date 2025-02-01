using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Domain.Shared
{
    public record Money(decimal Amount, Currency Currency)
    {
        public static Money operator +(Money first, Money second)
        {
            if (first.Currency != second.Currency)
            {
                throw new ApplicationException("Cannot add money with different currencies");
            }
            return new Money(first.Amount + second.Amount, first.Currency);
        }

        public static Money Zero() => new Money(0, Currency.None);
        public static Money Zero(Currency currency) => new Money(0, currency);
        public bool IsZero() => this == Zero(Currency);
    }

}
