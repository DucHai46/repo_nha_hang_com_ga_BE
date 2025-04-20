using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using auth.service.Context;
using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";


BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var mongoDbConfig = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = "mongodb://admin:admin123@52.64.190.146:27017",
        DatabaseName = "Users"
    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
    }
};

// Sử dụng CustomMongoDbContext
var mongoDbContext = new CustomMongoDbContext(
    mongoDbConfig.MongoDbSettings.ConnectionString,
    mongoDbConfig.MongoDbSettings.DatabaseName
);

builder.Services.ConfigureMongoDbIdentity<MongoUser, MongoIdentityRole<Guid>, Guid>(
    mongoDbConfig,
    mongoDbContext
);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Auth Service API", Version = "v1" });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService v1"));
}

app.UseCors(myAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

[CollectionName("Users")]
public class MongoUser : MongoIdentityUser<Guid>
{
    public string FullName { get; set; } // Add custom fields as needed
}

