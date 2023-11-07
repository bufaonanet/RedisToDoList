using Microsoft.EntityFrameworkCore;
using RedisToDoList.API.Infrastructure.Caching;
using RedisToDoList.API.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDbContext<ToDoListDbContext>(o => 
        o.UseInMemoryDatabase("ToDoListDb"));
    
    builder.Services.AddScoped<ICachingService, CachingService>();
    
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration
            .GetConnectionString("RedisConnectionString");
    });

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
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