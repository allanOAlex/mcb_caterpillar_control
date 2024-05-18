using System.Linq.Expressions;

namespace GECA.Client.Console.Application.Abstractions.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IQueryable<T>> FindAll();
        Task<T?> FindById(int Id);
        Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(T entity);
    }
}
