using System.Text;
using Microsoft.Azure.Cosmos;
using SmallsOnline.Web.Services.CosmosDB.Helpers;

namespace SmallsOnline.Web.Services.CosmosDB;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get a list of blog entries for a given page number
    /// </summary>
    /// <param name="pageNumber">The page number to get.</param>
    /// <returns>A collection of blog entries.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<BlogEntry>> GetBlogEntriesAsync(int pageNumber = 1)
    {
        // Initialize a list to hold the blog entries.
        List<BlogEntry> blogEntries = new();

        // Set the offset count for the Cosmos DB SQL query.
        int offsetNum = pageNumber switch
        {
            0 => throw new Exception("Invalid page number."), // Page number cannot be zero.
            1 => 0, // Offset always starts at 0 for the first page.
            _ => (pageNumber - 1) * 5 // Offset starts at the number of items per page * the page number.
        };

        // Get the container where the blog entries are stored.
        Container container = cosmosDbClient.GetContainer(_containerName, "blogs");

        // Build the Cosmos DB SQL query.
        QueryDefinition query = new($"SELECT c.id, c.partitionKey, c.blogUrlId, c.blogTitle, c.blogPostedDate, c.blogContent, c.blogTags, c.blogIsPublished FROM c WHERE c.partitionKey = \"blog-entry\" AND c.blogIsPublished = true ORDER BY c.blogPostedDate DESC OFFSET {offsetNum} LIMIT 5");

        // Execute the Cosmos DB SQL query and get the results.
        FeedIterator<BlogEntry> containerQueryIterator = container.GetItemQueryIterator<BlogEntry>(query);
        while (containerQueryIterator.HasMoreResults)
        {
            // Loop through each database entry retrieved.
            foreach (BlogEntry item in await containerQueryIterator.ReadNextAsync())
            {
                // If the content of the blog post is not null,
                // then attempt to shorten the content.
                if (item.Content is not null)
                {
                    // Intialize a StringBuilder to hold the shortened content.
                    StringBuilder markdownShort = new();

                    // Use StringReader to read the content.
                    using (StringReader stringReader = new(item.Content))
                    {
                        // Loop through each line until 'moreLineFound' is set to true.
                        bool moreLineFound = false;
                        while (!moreLineFound)
                        {
                            string? line = stringReader.ReadLine();

                            if (line == "<!--more-->")
                            {
                                // If the line is '<!--more-->',
                                // then set 'moreLineFound' to true and exit the loop.
                                moreLineFound = true;
                            }
                            else if (line is not null)
                            {
                                // If the line is not null and is not '<!--more-->',
                                // then add the line to the StringBuilder.
                                markdownShort.AppendLine(line);
                            }
                            else
                            {
                                // If we've reached the end of the content,
                                // then set 'moreLineFound' to true and exit the loop.
                                moreLineFound = true;
                                break;
                            }
                        }
                    }

                    // Set the content property to the shortened content.
                    item.Content = markdownShort.ToString();
                }

                // Add the blog entry to the list.
                blogEntries.Add(item);
            }
        }

        // Dispose of the container query iterator.
        containerQueryIterator.Dispose();

        return blogEntries;
    }
}