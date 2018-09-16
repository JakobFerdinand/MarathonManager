﻿using AutoMapper;
using Core.Models;
using Core.Repositories;
using Data;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using UI.JsonImport.Services;
using static System.Console;

namespace UI.JsonImport
{
    class Program
    {
        private static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            WriteLine("1. Create Database");
            WriteLine("2. Insert Categories");
            WriteLine("3. Import Json Runners");
            WriteLine("exit");

            while (true)
            {
                var input = ReadLine();

                switch (input)
                {
                    case "exit": return;

                    case "1":
                        CreateDatabase();
                        break;

                    case "2":
                        InsertCategories();
                        break;

                    case "3":
                        ImportRunners();
                        break;

                    default:
                        WriteLine("Invalid input");
                        break;
                }
            }
        }

        public static void Startup()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
        }

        private static void CreateDatabase()
        {
            var connectionString = Configuration.GetConnectionString("Default");

            if (connectionString is null)
            {
                WriteLine("Beim Lesen des ConnectionStrings ist ein Fehler aufgetreten. Bitte geben Sie ihn per Hand ein:");
                connectionString = ReadLine();
            }
            var options = new DbContextOptionsBuilder<RunnerDbContext>()
                .UseSqlServer(connectionString)
                .Options;
            using (var context = new RunnerDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        private static void InsertCategories()
        { }

        private static void ImportRunners()
        {
            var path = @"C:\Development\runnerdata.json";

            var filereader = new StreamReader(path);
            var json = filereader.ReadToEnd();

            var options = new DbContextOptionsBuilder<RunnerDbContext>()
                .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MarathonManager;Integrated Security=True")
                .Options;
            using (var context = new RunnerDbContext(options))
            {
                var categoryRepository = new CategoryRepository(context);
                var runnerRepository = new RunnerRepository(context);
                using (var unitOfWork = new UnitOfWork(context, categoryRepository, runnerRepository))
                {
                    var mapperConfiguration = GetMapperConfiguration(unitOfWork.Categories);
                    var mapper = new Mapper(mapperConfiguration);

                    var deserializer = new JsonDeserializer(mapper);

                    var runners = deserializer.Deserialize(json);

                    runnerRepository.AddRange(runners);
                    unitOfWork.Complete();
                }
            }
        }

        private static AutoMapper.IConfigurationProvider GetMapperConfiguration(ICategoryRepository categoryRepository)
        {
            var categories = categoryRepository.GetAll(asNoTracking: true);

            return new MapperConfiguration(c =>
            {
                c.CreateMap<Models.Runner, Runner>()
                    .ForMember(r => r.Gender, map => map.MapFrom(r => r.Geschlecht == "Mann" ? Gender.Mann : Gender.Frau))
                    .ForMember(r => r.Firstname, map => map.MapFrom(r => r.Vorname))
                    .ForMember(r => r.Lastname, map => map.MapFrom(r => r.Nachname))
                    .ForMember(r => r.YearOfBirth, map => map.MapFrom(r => int.Parse(r.Geburtsdatum.Substring(0, 4))))
                    .ForMember(r => r.City, map => map.MapFrom(r => r.Wohnort))
                    .ForMember(r => r.Email, map => map.MapFrom(r => r.eMail))
                    .ForMember(r => r.SportsClub, map => map.MapFrom(r => r.Verein))
                    .ForMember(r => r.CategoryId, map => map.MapFrom(r => categories.Single(category => category.Name == r.Strecken).Id))
                    .ForMember(r => r.Id, map => map.Ignore())
                    .ForMember(r => r.RunningTime, map => map.Ignore())
                    .ForMember(r => r.Category, map => map.Ignore())
                    .ForMember(r => r.ChipId, map => map.Ignore())
                    .ForMember(r => r.RunningTime, map => map.Ignore())
                    .ForMember(r => r.TimeAtDestination, map => map.Ignore())
                    .ForMember(r => r.Startnumber, map => map.Ignore());

            });
        }
    }
}
