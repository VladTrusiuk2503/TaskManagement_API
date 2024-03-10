using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace TaskManagement_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<TaskContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("TaskContext") ?? throw new InvalidOperationException("Connection string 'TaskContext' not found.")));

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagementApi");
                });
                    
            }
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}