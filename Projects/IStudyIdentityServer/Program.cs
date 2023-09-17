using IStudyIdentityServer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt => opt.AllowEmptyInputInBodyModelBinding = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IStudyDataBaseContext>
(opt => opt.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnect"))
);

var authOptionsConfiguration = builder.Configuration.GetSection("Auth");

builder.Services.AddCors(opt => 
    opt.AddDefaultPolicy(build =>
    {
        build.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    }));


var app = builder.Build();

using var scope = app.Services.CreateScope();
using (var context = scope.ServiceProvider.GetService<IStudyDataBaseContext>())
{
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();