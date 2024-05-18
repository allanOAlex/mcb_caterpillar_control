using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
