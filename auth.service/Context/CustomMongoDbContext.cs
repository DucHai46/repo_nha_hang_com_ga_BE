using MongoDB.Bson;
using MongoDbGenericRepository;

namespace auth.service.Context;

public class CustomMongoDbContext: MongoDbContext
{
    public CustomMongoDbContext(string connectionString, string dbName)
        : base(connectionString, dbName)
    {
        // Override để tránh lỗi liên quan tới GuidRepresentationMode
        // KHÔNG gọi InitializeGuidRepresentation hay SetGuidRepresentation
    }

    protected override void InitializeGuidRepresentation()
    {
        // Ghi đè và bỏ trống để tránh lỗi
    }

    public override void SetGuidRepresentation(GuidRepresentation guidRepresentation)
    {
        // Ghi đè và bỏ trống để tránh lỗi
    }
}