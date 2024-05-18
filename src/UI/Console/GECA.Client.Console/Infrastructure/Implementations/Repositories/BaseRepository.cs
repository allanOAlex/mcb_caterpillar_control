using GECA.Client.Console.Application.Abstractions.IRepositories;
using System.Linq.Expressions;

namespace GECA.Client.Console.Infrastructure.Implementations.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public BaseRepository()
        {

        }

        public Task<T> Create(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<T> Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<T>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<T?> FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<T> Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
