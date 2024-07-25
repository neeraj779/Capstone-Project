using SongService.API.Interfaces;
using SongService.API.Services;

namespace SongService.API
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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("https://neeraj779.github.io", "http://127.0.0.1:3000")
                                      .WithMethods("GET"));
            });


            #region Services
            builder.Services.AddScoped<ISongDataService, SongDataService>();
            builder.Services.AddHttpClient<ISongDataService, SongDataService>();
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.UseCors("CorsPolicy");


            app.MapControllers();

            app.Run();
        }
    }
}
