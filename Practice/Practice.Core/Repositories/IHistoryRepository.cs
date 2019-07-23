namespace Practice.Core.Repositories
{
    using Practice.Core.ViewModels;
    using System.Threading.Tasks;

    public interface IHistoryRepository
    {
        Task<DataTablesObject<History>> GetAllHistoriesAsync(SearchFilters searchFilters);
    }
}
