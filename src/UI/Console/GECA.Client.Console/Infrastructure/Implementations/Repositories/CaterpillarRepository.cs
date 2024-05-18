using GECA.Client.Console.Application.Abstractions.IRepositories;
using GECA.Client.Console.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Repositories
{
    internal sealed class CaterpillarRepository : IBaseRepository<Caterpillar>, ICaterpillarRepository
    {
        public CaterpillarRepository()
        {

        }

        public Task<Caterpillar> Create(Caterpillar entity)
        {
            throw new NotImplementedException();
        }

        public Task<Caterpillar> Delete(Caterpillar entity)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Caterpillar>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Caterpillar>> FindByCondition(Expression<Func<Caterpillar, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<Caterpillar?> FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<Caterpillar> Update(Caterpillar entity)
        {
            throw new NotImplementedException();
        }
    }
}
