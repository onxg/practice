namespace Practice.Core.Repositories
{
    using Practice.Core.ViewModels;
    using System.Threading.Tasks;

    public interface ISalesRepository
    {
        Task<DataTablesObject<Store>> GetAllStoresAsync(SearchFilters searchFilters);
    }
}
