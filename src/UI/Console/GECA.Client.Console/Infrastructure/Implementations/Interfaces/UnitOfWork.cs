using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Application.Abstractions.IRepositories;
using GECA.Client.Console.Persistence.DataContext;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Interfaces
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICaterpillarRepository CaterpillarRepository { get; private set; }
        public ISpiceRepository SpiceRepository { get; private set; }

        private readonly DBContext Context;


        public UnitOfWork(ICaterpillarRepository caterpillarRepository, ISpiceRepository spiceRepository)
        {
            CaterpillarRepository = caterpillarRepository;
            SpiceRepository = spiceRepository;
            
        }

        public Task<int> CompleteAsync()
        {
            var result = Context.SaveChangesAsync();
            return result!;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
        }
    }
}
