using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Application.Exceptions
{
    public sealed class ValidationError(string PorpertyName, string ErrorMessage);
}
