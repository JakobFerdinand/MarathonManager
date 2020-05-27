using Core;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using UI.RunnerManagement.Models;

namespace UI.RunnerManagement.Services
{
    internal interface ICategoriesService
    {
        Task<ImmutableList<ImmutableCategory>> GetAll();
    }

    internal class CategoriesService : ICategoriesService
    {
        private readonly Func<IUnitOfWork> getNewUnitOfWork;

        public CategoriesService(Func<IUnitOfWork> getNewUnitOfWork)
            => this.getNewUnitOfWork = getNewUnitOfWork ?? throw new ArgumentNullException(nameof(getNewUnitOfWork));

        public Task<ImmutableList<ImmutableCategory>> GetAll()
        {
            using (var unit = getNewUnitOfWork())
                return Task.FromResult(unit.Categories
                    .GetAll(asNoTracking: true)
                    .Select(c => c.ToImmutable())
                    .ToImmutableList());
        }
    }
}
