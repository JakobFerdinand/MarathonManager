using Core;
using Core.Models;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using UI.RunnerManagement.Models;

namespace UI.RunnerManagement.Services
{
    internal interface IRunnersService
    {
        Task<ImmutableList<ImmutableRunner>> GetAll();
    }

    internal class RunnersService : IRunnersService
    {
        private readonly Func<IUnitOfWork> getNewUnitOfWork;

        public RunnersService(Func<IUnitOfWork> getNewUnitOfWork)
            => this.getNewUnitOfWork = getNewUnitOfWork ?? throw new ArgumentNullException(nameof(getNewUnitOfWork));

        public Task<ImmutableList<ImmutableRunner>> GetAll()
        {
            using (var unit = getNewUnitOfWork())
                return Task.FromResult(
                    unit
                    .Runners
                    .GetAllWithCategories(asNoTracking: true)
                    .Select(r => r.ToImmutable())
                    .ToImmutableList());
        }
    }
}
