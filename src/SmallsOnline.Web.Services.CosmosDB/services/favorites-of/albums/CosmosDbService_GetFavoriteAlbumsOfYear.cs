using Microsoft.Azure.Cosmos;
using SmallsOnline.Web.Services.CosmosDB.Helpers;

namespace SmallsOnline.Web.Services.CosmosDB;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get the favorite albums for a specific year.
    /// </summary>
    /// <param name="listYear">The list year to get the data for.</param>
    /// <returns>A collection of favorite albums for a year.</returns>
    public async Task<List<AlbumData>> GetFavoriteAlbumsOfYearAsync(string listYear)
    {
        // Create a list to hold the album items.
        List<AlbumData> albumItems = new();

        // Get the container where the favorite music entries are stored.
        Container container = cosmosDbClient.GetContainer(_containerName, "favorites-of");

        // Define the query for getting the favorite albums for a year.
        QueryDefinition query = new($"SELECT * FROM c WHERE c.partitionKey = \"favorites-of-albums\" AND c.listYear = \"{listYear}\"");

        // Execute the query.
        FeedIterator<AlbumData> containerQueryIterator = container.GetItemQueryIterator<AlbumData>(query);
        while (containerQueryIterator.HasMoreResults)
        {
            foreach (AlbumData item in await containerQueryIterator.ReadNextAsync())
            {
                // Add the album to the list.
                albumItems.Add(item);
            }
        }

        // Dispose of the query iterator.
        containerQueryIterator.Dispose();

        return albumItems;
    }
}