using Bookly.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Repositories
{
    internal abstract class Repository<T>
        where T : Entity
    {

        protected readonly ApplicationDbContext DbContext;

        protected Repository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<T?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken)
        {
            return await DbContext
                .Set<T>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Add(T entity)
        {
            DbContext.Set<T>().Add(entity);
        }
    }
}
