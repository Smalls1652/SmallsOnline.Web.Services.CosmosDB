using Microsoft.Azure.Cosmos;
using SmallsOnline.Web.Services.CosmosDB.Helpers;

namespace SmallsOnline.Web.Services.CosmosDB;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get the favorite tracks for a specific year.
    /// </summary>
    /// <param name="listYear">The list year to get the data for.</param>
    /// <returns>A collection of favorite tracks for a year</returns>
    public async Task<List<SongData>> GetFavoriteSongsOfYearAsync(string listYear)
    {
        // Create a list to hold the track items.
        List<SongData> trackItems = new();

        // Get the container where the favorite music entries are stored.
        Container container = cosmosDbClient.GetContainer(_containerName, "favorites-of");

        // Define the query for getting the favorite tracks for a year.
        QueryDefinition query = new($"SELECT * FROM c WHERE c.partitionKey = \"favorites-of-tracks\" AND c.listYear = \"{listYear}\"");

        // Execute the query.
        FeedIterator<SongData> containerQueryIterator = container.GetItemQueryIterator<SongData>(query);
        while (containerQueryIterator.HasMoreResults)
        {
            foreach (SongData item in await containerQueryIterator.ReadNextAsync())
            {
                // Add the track to the list.
                trackItems.Add(item);
            }
        }

        // Dispose of the query iterator.
        containerQueryIterator.Dispose();

        return trackItems;
    }
}