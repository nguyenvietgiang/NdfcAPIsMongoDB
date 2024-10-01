using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.MinimalAPIs
{
    public static class TitleAPIs
    {
        private const string BaseUrl = "api/minimal/v1/titles";

        public static void MapTitleEndpoints(this IEndpointRouteBuilder endpoints, IMongoDatabase database)
        {
            endpoints.MapPost($"{BaseUrl}/create", async context => await CreateTitleAsync(context, database));
            endpoints.MapGet($"{BaseUrl}/getall", async context => await GetTitlesAsync(context, database));
            //endpoints.MapGet($"{BaseUrl}/getbyid/{context.Request.Query["id"]}", async context => await GetTitleByIdAsync(context, database));
            //endpoints.MapPut($"{BaseUrl}/update/{context.Request.Query["id"]}", async context => await UpdateTitleAsync(context, database));
            //endpoints.MapDelete($"{BaseUrl}/delete/{context.Request.Query["id"]}", async context => await DeleteTitleAsync(context, database));
        }

        private static async Task CreateTitleAsync(HttpContext context, IMongoDatabase database)
        {
            var title = await context.Request.ReadFromJsonAsync<Title>();
            if (title != null)
            {
                var collection = GetMongoCollection(database);
                await collection.InsertOneAsync(title);
                context.Response.StatusCode = 201; // Created
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
            }
        }

        private static async Task GetTitlesAsync(HttpContext context, IMongoDatabase database)
        {
            var collection = GetMongoCollection(database);
            var titles = await collection.Find(new BsonDocument()).ToListAsync();
            await context.Response.WriteAsJsonAsync(titles);
        }

        private static async Task GetTitleByIdAsync(HttpContext context, IMongoDatabase database)
        {
            var id = context.Request.RouteValues["id"].ToString();
            var collection = GetMongoCollection(database);
            var title = await collection.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (title != null)
            {
                await context.Response.WriteAsJsonAsync(title);
            }
            else
            {
                context.Response.StatusCode = 404; // Not Found
            }
        }

        private static async Task UpdateTitleAsync(HttpContext context, IMongoDatabase database)
        {
            var id = context.Request.RouteValues["id"].ToString();
            var title = await context.Request.ReadFromJsonAsync<Title>();
            if (title != null)
            {
                var collection = GetMongoCollection(database);
                var result = await collection.ReplaceOneAsync(t => t.Id == id, title);
                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    context.Response.StatusCode = 200; // OK
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
            }
        }

        private static async Task DeleteTitleAsync(HttpContext context, IMongoDatabase database)
        {
            var id = context.Request.RouteValues["id"].ToString();
            var collection = GetMongoCollection(database);
            var result = await collection.DeleteOneAsync(t => t.Id == id);
            if (result.IsAcknowledged && result.DeletedCount > 0)
            {
                context.Response.StatusCode = 200; // OK
            }
            else
            {
                context.Response.StatusCode = 404; // Not Found
            }
        }

        private static IMongoCollection<Title> GetMongoCollection(IMongoDatabase database)
        {
            return database.GetCollection<Title>("Titles");
        }
    }
}


