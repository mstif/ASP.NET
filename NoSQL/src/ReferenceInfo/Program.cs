
using GMS.Identity.WebHost.Cache;
using Microsoft.EntityFrameworkCore;
using ReferenceInfoCore.Repositories;
using ReferenceInfoDataAccess;
using ReferenceInfoDataAccess.Data;
using ReferenceInfoDataAccess.Repositories;

namespace ReferenceInfo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            //builder.services.AddScoped<INotificationGateway, NotificationGateway>();
            builder.Services.AddScoped<IDbInitializer, EfDbInitializer>();
            builder.Services.AddDbContext<DataContext>(x =>
            {
                //x.UseSqlite("Filename=PromocodeFactoryGivingToCustomerDb.sqlite");
                x.UseNpgsql(builder.Configuration.GetConnectionString("ReferenceDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var scopedProvider = scope.ServiceProvider;
                try
                {

                    var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
                    if (dbInitializer != null)
                    {
                        dbInitializer.InitializeDb();
                    }
                    else throw new Exception("Ошибка инициализации БД!");

                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "An error occurred seeding the DB.");
                    //Logger<>.Fatal($"The {app.Environment.ApplicationName} error... An error occurred seeding the DB.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
