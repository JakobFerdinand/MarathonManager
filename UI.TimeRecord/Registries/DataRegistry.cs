﻿using Core;
using Core.Repositories;
using Data;
using Data.Repositories;
using Data.Shared;
using Logging;
using StructureMap;

namespace UI.TimeRecord.Registries
{
    internal class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<ICategoryRepository>()
                .Use<CategoryRepository>()
                .AlwaysUnique();

            For<IRunnerRepository>()
                .DecorateAllWith<LoggingRunnerRepository>();

            For<IRunnerRepository>()
                .Use<RunnerRepository>()
                .AlwaysUnique();

            For<IChangesFinder>()
                .Use<ChangesFinder>()
                .Singleton();

            For<IUnitOfWork>()
                .Use<UnitOfWork>()
                .AlwaysUnique();

            For<IDatabase>()
                .Use<Database>()
                .AlwaysUnique();
        }
    }
}
