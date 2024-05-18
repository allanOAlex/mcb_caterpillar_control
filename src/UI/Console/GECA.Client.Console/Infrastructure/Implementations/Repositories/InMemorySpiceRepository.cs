using GECA.Client.Console.Application.Abstractions.IRepositories;
using GECA.Client.Console.Domain.Entities;

namespace GECA.Client.Console.Infrastructure.Implementations.Repositories
{
    internal sealed class InMemorySpiceRepository : IBaseRepository<Spice>, ISpiceRepository
    {
        private readonly List<Spice> SpiceList;

        public InMemorySpiceRepository()
        {
            SpiceList = new();
        }

        public async Task<Spice> Create(Spice entity)
        {
            SpiceList.Add(entity);
            return entity;
        }

        public async Task<Spice> Delete(Spice entity)
        {
            var spiceToDelete = await FindById(entity.Id);
            if (entity != null)
            {
                SpiceList.Remove(spiceToDelete);
            }
            return spiceToDelete;
        }

        public async Task<IQueryable<Spice>> FindAll()
        {
            return (IQueryable<Spice>)SpiceList;
        }

        public async Task<IQueryable<Spice>> FindByCondition(System.Linq.Expressions.Expression<Func<Spice, bool>> expression)
        {
            return await Task.FromResult(SpiceList.AsQueryable().Where(expression));
        }

        public async Task<Spice?> FindById(int Id)
        {
            return SpiceList.FirstOrDefault(c => c.Id == Id);
        }

        public async Task<Spice> Update(Spice entity)
        {
            var existingCaterpillar = await FindById(entity.Id);
            if (existingCaterpillar != null)
            {
                // Update properties of existingCaterpillar
                existingCaterpillar.Id = entity.Id;
                existingCaterpillar.Row = entity.Row;
                existingCaterpillar.Column = entity.Column;
            }

            return entity;
        }
    }
}
