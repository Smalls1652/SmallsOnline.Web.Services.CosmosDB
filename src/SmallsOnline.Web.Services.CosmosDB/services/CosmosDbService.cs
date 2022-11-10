using Microsoft.Azure.Cosmos;
using SmallsOnline.Web.Services.CosmosDB.Helpers;

namespace SmallsOnline.Web.Services.CosmosDB;

/// <summary>
/// Service for interacting with Cosmos DB.
/// </summary>
public partial class CosmosDbService : ICosmosDbService
{
    public CosmosDbService(string connectionString, string containerName)
    {
        _containerName = containerName;
        cosmosDbClient = InitService(connectionString, jsonSerializer);
    }

    /// <summary>
    /// The CosmosDB client.
    /// </summary>
    private CosmosClient cosmosDbClient;

    private readonly string _containerName;

    /// <summary>
    /// The JSON serializer for the CosmosDB client.
    /// </summary>
    private readonly CosmosDbSerializer jsonSerializer = new(
        new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = {
                new JsonDateTimeOffsetConverter()
            }
        }
    );

    /// <summary>
    /// Create a CosmosDB client.
    /// </summary>
    /// <param name="dbSerializer">The JSON serializer to use for the CosmosDB client.</param>
    /// <returns>A CosmosDB client.</returns>
    private static CosmosClient InitService(string connectionString, CosmosDbSerializer dbSerializer)
    {
        return new(
            connectionString: connectionString,
            clientOptions: new()
            {
                Serializer = dbSerializer
            }
        );
    }
}