

using Microsoft.OpenApi.Models;
using repo_nha_hang_com_ga_BE.Models.Common.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddMongoDbServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MappingProfile)); // Đăng ký AutoMapper


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MongoWebApi", Version = "v1" });
});
var app = builder.Build();

// Configure the HTTP request pipeline.

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MongoWebApi v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();