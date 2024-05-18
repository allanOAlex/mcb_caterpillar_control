using GECA.Client.Console.Application.Abstractions.IRepositories;
using GECA.Client.Console.Domain.Entities;
using System.Linq.Expressions;

namespace GECA.Client.Console.Infrastructure.Implementations.Repositories
{
    internal sealed class InMemoryCaterpillarRepository :IBaseRepository<Caterpillar>, ICaterpillarRepository
    {
        private readonly List<Caterpillar> Caterpillars;


        public InMemoryCaterpillarRepository()
        {
            Caterpillars = new();
        }

        public async Task AddAsync(Caterpillar caterpillar)
        {
            Caterpillars.Add(caterpillar);
        }

        public async Task<Caterpillar> Create(Caterpillar entity)
        {
            Caterpillars.Add(entity);
            return entity;
        }

        public async Task<Caterpillar> Delete(Caterpillar entity)
        {
            var caterpillarToDelete = await FindById(entity.Id);
            if (entity != null)
            {
                Caterpillars.Remove(caterpillarToDelete);
            }
            return caterpillarToDelete;
        }

        public async Task<IQueryable<Caterpillar>> FindAll()
        {
            return (IQueryable<Caterpillar>)Caterpillars;
        }

        public async Task<IQueryable<Caterpillar>> FindByCondition(Expression<Func<Caterpillar, bool>> expression)
        {
            return await Task.FromResult(Caterpillars.AsQueryable().Where(expression));
        }

        public async Task<Caterpillar?> FindById(int Id)
        {
            return Caterpillars.FirstOrDefault(c => c.Id == Id);
        }

        public async Task<Caterpillar> Update(Caterpillar entity)
        {
            var existingCaterpillar = await FindById(entity.Id);
            if (existingCaterpillar != null)
            {
                // Update properties of existingCaterpillar
                existingCaterpillar.Id = entity.Id;
                existingCaterpillar.Segments = entity.Segments;
                existingCaterpillar.MaxSegments = entity.MaxSegments;
            }

            return entity;
        }

    }
}
