using Microsoft.Azure.Cosmos;
using SmallsOnline.Web.Services.CosmosDB.Helpers;

namespace SmallsOnline.Web.Services.CosmosDB;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get a blog entry by a specific ID.
    /// </summary>
    /// <param name="id">The unique ID of the blog entry.</param>
    /// <returns>A blog entry.</returns>
    public async Task<BlogEntry> GetBlogEntryAsync(string id)
    {
        // Get the container where the blog entries are stored.
        Container container = cosmosDbClient.GetContainer(_containerName, "blogs");

        // Get the blog entry for the supplied ID.
        BlogEntry retrievedItem = await container.ReadItemAsync<BlogEntry>(
            id: id,
            partitionKey: new("blog-entry")
        );

        // If the content is not null,
        // then remove the '<!--more-->' tag from the content.
        if (retrievedItem.Content is not null)
        {
            retrievedItem.Content = retrievedItem.Content.Replace("<!--more-->", "");
        }

        return retrievedItem;
    }
}