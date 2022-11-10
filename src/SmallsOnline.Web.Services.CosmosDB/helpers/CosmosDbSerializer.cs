using Azure.Core.Serialization;
using Microsoft.Azure.Cosmos;

namespace SmallsOnline.Web.Services.CosmosDB.Helpers;

public class CosmosDbSerializer : CosmosSerializer
{
    private readonly JsonObjectSerializer systemTextJsonSerializer;

    public CosmosDbSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        systemTextJsonSerializer = new JsonObjectSerializer(jsonSerializerOptions);
    }

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (stream.CanSeek && stream.Length == 0)
            {
                #pragma warning disable CS8603
                return default;
                #pragma warning restore CS8603
            }

            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            return (T)systemTextJsonSerializer.Deserialize(stream, typeof(T), default)!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream streamPayload = new();
        systemTextJsonSerializer.Serialize(streamPayload, input, typeof(T), default);
        streamPayload.Position = 0;
        return streamPayload;
    }
}