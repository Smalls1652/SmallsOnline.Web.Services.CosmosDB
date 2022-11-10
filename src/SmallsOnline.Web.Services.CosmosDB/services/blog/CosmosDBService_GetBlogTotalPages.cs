using Microsoft.Azure.Cosmos;
using SmallsOnline.Web.Services.CosmosDB.Helpers;

namespace SmallsOnline.Web.Services.CosmosDB;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get the total number of pages for blog entries.
    /// </summary>
    /// <returns>The total number of pages available.</returns>
    public async Task<int> GetBlogTotalPagesAsync()
    {
        List<int> totalPagesCount = new();

        // Get the container where the blog entries are stored.
        Container container = cosmosDbClient.GetContainer(_containerName, "blogs");

        // Define the query for getting the total number of blog entries.
        // The query creates a column for the total number of distinct blog entries.
        QueryDefinition query = new("SELECT DISTINCT VALUE n.itemCount FROM (SELECT COUNT(1) AS itemCount FROM c WHERE c.partitionKey = 'blog-entry' AND c.blogIsPublished = true ) n");

        // Run the query.
        // Note: Due to the weird way Cosmos DB handles queries, the query must be run through a loop.
        FeedIterator<int> containerQueryIterator = container.GetItemQueryIterator<int>(query);
        while (containerQueryIterator.HasMoreResults)
        {
            foreach (int item in await containerQueryIterator.ReadNextAsync())
            {
                // Add the total number of blog entries to the list.
                totalPagesCount.Add(item);
            }
        }

        // Reurn the total number of pages by,
        // dividing the total number of blog entries by the number of items per page
        // and rounding up to the nearest whole number.
        return (int)Math.Round((decimal)totalPagesCount[0] / 5, 0, MidpointRounding.ToPositiveInfinity);
    }
}