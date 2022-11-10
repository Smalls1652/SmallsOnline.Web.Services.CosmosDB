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

        BlogEntry? retrievedItem = null;

        QueryDefinition query = new($"SELECT * FROM c WHERE c.partitionKey = \"blog-entry\" AND c.blogUrlId =\"{id}\"");

        // Execute the Cosmos DB SQL query and get the results.
        FeedIterator<BlogEntry> containerQueryIterator = container.GetItemQueryIterator<BlogEntry>(query);
        while (containerQueryIterator.HasMoreResults)
        {
            // Loop through each database entry retrieved.
            foreach (BlogEntry item in await containerQueryIterator.ReadNextAsync())
            {
                retrievedItem = item;
            }
        }

        if (retrievedItem is null)
        {
            // Get the blog entry for the supplied ID.
            retrievedItem = await container.ReadItemAsync<BlogEntry>(
                id: id,
                partitionKey: new("blog-entry")
            );

            // If the content is not null,
            // then remove the '<!--more-->' tag from the content.
            if (retrievedItem.Content is not null)
            {
                retrievedItem.Content = retrievedItem.Content.Replace("<!--more-->", "");
            }
        }

        return retrievedItem;
    }
}