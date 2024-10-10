using Microsoft.EntityFrameworkCore;
using RefactorThis.Domain;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Context;
using RefactorThis.Persistence.Repositories;

namespace RefactorThis.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region Services and Repositories
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
            builder.Services.AddDbContext<InvoiceDBContext>(options => options.UseSqlServer(connectionString));
            #endregion

            var app = builder.Build();

            #region Database Migration using Code First
            using (var Scope = app.Services.CreateScope())
            {
                var context = Scope.ServiceProvider.GetRequiredService<InvoiceDBContext>();
                context.Database.Migrate();
            }
            #endregion

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