using System;
using System.Data.Common;
using ECommerceApp.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ECommerceApp.IntegrationTests.Helpers
{
    public abstract class DatabaseTestBase : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly DbConnection _connection;
        protected readonly ApplicationDbContext DbContext;

        protected DatabaseTestBase()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            DbContext = new ApplicationDbContext(options);
            DbContext.Database.EnsureCreated();

            SeedDatabase();
        }

        protected virtual void SeedDatabase()
        {
            // Override this method in derived classes to seed the database
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
            _connection.Dispose();
        }
    }
} 