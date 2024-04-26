

using Application.Interfaces.ContractDbs;
using Application.Interfaces.Repositories.StoredProcedures;
using Infrastructure.ContractDbs;
using Infrastructure.Repositories.StoredProcedures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbsContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnectDB"), b =>
            {
                b.MigrationsAssembly(typeof(DbsContext).Assembly.FullName);
                b.CommandTimeout((int)TimeSpan.FromMinutes(20).TotalSeconds);
                b.EnableRetryOnFailure(5, TimeSpan.FromMinutes(5.0), null);

            }), ServiceLifetime.Transient);

            //services.AddDbContext<DbsContext>(options =>
            //{
            //    options.UseSqlServer(configuration.GetConnectionString("FBBPortalDB_Dev"));
            //});


            services.AddScoped<IDbsFactory>(s =>
            {
                var db = new DbsFactory(s.GetRequiredService<DbsContext>());
                var isCanConect = db.CanConnect();
                return db;
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStoredProcedure, StoredProcedure>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}