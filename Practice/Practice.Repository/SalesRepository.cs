namespace Practice.Repository
{
    using Practice.Core.Repositories;
    using Practice.DAL;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Threading.Tasks;
    using ViewModels = Core.ViewModels;

    public class SalesRepository : ISalesRepository
    {
        private readonly Context context;

        public SalesRepository()
        {
            context = new Context();
        }

        public async Task<ViewModels.DataTablesObject<ViewModels.Store>> GetAllStoresAsync(ViewModels.SearchFilters searchFilters)
        {
            var storeItems = await context.vStoreWithAddresses.Select(x => new ViewModels.Store()
            {
                Address = x.AddressLine1,
                City = x.City,
                Country = x.CountryRegionName,
                Name = x.Name,
                PostalCode = x.PostalCode
            }).ToListAsync();

            var rawData = storeItems
                .OrderBy(searchFilters.OrderBy)
                .Skip(searchFilters.DisplayStart)
                .Take(searchFilters.DisplayLength)
                .ToList();

            return new ViewModels.DataTablesObject<ViewModels.Store>
            {
                aaData = rawData,
                iTotalDisplayRecords = storeItems.Count,
                iTotalRecords = storeItems.Count
            };
        }
    }
}
