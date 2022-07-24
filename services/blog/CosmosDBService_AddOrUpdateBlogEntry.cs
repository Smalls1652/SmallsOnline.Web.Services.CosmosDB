using Microsoft.Azure.Cosmos;

namespace SmallsOnline.Web.Services.CosmosDB;

public partial class CosmosDbService : ICosmosDbService
{
    public async Task AddOrUpdateBlogEntryAsync(BlogEntry blogEntry)
    {
        Container container = cosmosDbClient.GetContainer(_containerName, "blogs");

        try
        {
            await container.ReplaceItemAsync(
                item: blogEntry,
                id: blogEntry.Id,
                partitionKey: new("blog-entry")
            );
        }
        catch (CosmosException dbException)
        {
            if (dbException.StatusCode == HttpStatusCode.NotFound)
            {
                await container.CreateItemAsync(
                item: blogEntry,
                partitionKey: new("blog-entry")
            );
            }
        }
    }
}