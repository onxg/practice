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
                Id = x.BusinessEntityID,
                Address = x.AddressLine1,
                City = x.City,
                Country = x.CountryRegionName,
                Name = x.Name,
                PostalCode = x.PostalCode
            }).ToListAsync();

            if (!string.IsNullOrEmpty(searchFilters.SearchValue))
            {
                var keyword = searchFilters.SearchValue.ToLower().Trim();
                storeItems = storeItems
                    .Where(s=>s.Name.ToLower().Contains(keyword)||
                    s.Address.ToLower().Contains(keyword)||
                    s.PostalCode.ToLower().Contains(keyword) ||
                    s.City.ToLower().Contains(keyword)||
                    s.Country.ToLower().Contains(keyword))
                    .ToList();
            }
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
        
        public async Task<ViewModels.Store> GetStoreById(int id)
        {
            if (id == 0)
                return null;

            var store = await context.vStoreWithAddresses.SingleAsync(e => e.BusinessEntityID == id);

            return new ViewModels.Store()
            {
                Id = store.BusinessEntityID,
                Name = store.Name,
                Address = store.AddressLine1,
                PostalCode = store.PostalCode,
                City = store.City,
                Country = store.CountryRegionName
            };
        }

        public async Task UpdateStore(ViewModels.Store store)
        {
            if (store == null || store.Id == 0)
                return;

            // to do

            await context.SaveChangesAsync();
        }
    }
}
