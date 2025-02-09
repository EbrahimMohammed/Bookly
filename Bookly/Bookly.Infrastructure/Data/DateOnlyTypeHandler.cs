using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Data
{
    internal sealed class DateOnlyTypeHandler: SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

        public override void SetValue(System.Data.IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = System.Data.DbType.Date;
            parameter.Value = value;
        }
    }
}
