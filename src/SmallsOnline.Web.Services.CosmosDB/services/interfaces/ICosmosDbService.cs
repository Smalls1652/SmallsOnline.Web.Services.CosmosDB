namespace SmallsOnline.Web.Services.CosmosDB;

public interface ICosmosDbService
{
    Task AddOrUpdateBlogEntryAsync(BlogEntry blogEntry);
    Task<List<BlogEntry>> GetBlogEntriesAsync(int pageNumber = 1);
    Task<BlogEntry> GetBlogEntryAsync(string id);
    Task<int> GetBlogTotalPagesAsync();

    Task<List<AlbumData>> GetFavoriteAlbumsOfYearAsync(string listYear);
    Task<List<SongData>> GetFavoriteSongsOfYearAsync(string listYear);
}