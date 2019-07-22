namespace Practice.Repository
{
    using Practice.Core.Repositories;
    using Practice.DAL;
    using System;
    using System.Collections.Generic;
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
            var storeItems = await context.vStoreWithAddresses
                .Where(e => context.Store.FirstOrDefault(y => y.BusinessEntityID == e.BusinessEntityID).IsDeleted == false)
                .Select(x => new ViewModels.Store()
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
                    .Where(s => s.Name.ToLower().Contains(keyword) ||
                    s.Address.ToLower().Contains(keyword) ||
                    s.PostalCode.ToLower().Contains(keyword) ||
                    s.City.ToLower().Contains(keyword) ||
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

            var shop = await context.Store.SingleOrDefaultAsync(x => x.BusinessEntityID == store.Id);
            if (shop == null)
                return;

            shop.Name = store.Name;

            var address = shop.BusinessEntity.BusinessEntityAddress.FirstOrDefault().Address;

            if (address != null)
            {
                address.AddressLine1 = store.Address;
                address.City = store.City;
                address.PostalCode = store.PostalCode;
            }

            // brakuje możliwości zmiany kraju
            await context.SaveChangesAsync();
        }

        public async Task DeleteStore(int id)
        {
            if (id == 0)
                return;

            var store = await context.Store.SingleAsync(x => x.BusinessEntityID == id);

            store.IsDeleted = true;

            await context.SaveChangesAsync();
        }

        public async Task CreateStore(ViewModels.Store store)
        {
            var shop = new Store
            {
                Name = store.Name,
                rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now,

                BusinessEntity = new BusinessEntity
                {
                    rowguid = Guid.NewGuid(),
                    ModifiedDate = DateTime.Now,
                    BusinessEntityAddress = new List<BusinessEntityAddress>
                    {
                        new BusinessEntityAddress
                        {
                            Address = new Address
                            {
                                AddressLine1 = store.Address,
                                AddressLine2 = Guid.NewGuid().ToString(), // to avoid bug with 2 different employees having the same address
                                City = store.City,
                                PostalCode = store.PostalCode,
                                rowguid = Guid.NewGuid(),
                                ModifiedDate = DateTime.Now,
                                StateProvince = await context.StateProvince.FirstOrDefaultAsync()
                            },
                            AddressType = await context.AddressType.FirstOrDefaultAsync(at => at.Name == "Home"),
                            ModifiedDate = DateTime.Now,
                            rowguid = Guid.NewGuid()
                        }
                    }

                }
            };
        }
    }
}
