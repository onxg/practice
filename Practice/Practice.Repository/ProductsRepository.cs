namespace Practice.Repository
{
    using Practice.DAL;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels = Practice.Core.ViewModels;
    using System.Linq.Dynamic;
    using Practice.Core.Repositories;
    using System;

    class ProductsRepository : IProductsRepository
    {
        private readonly Context context;
        public ProductsRepository()
        {
            context = new Context();
        }
        public async Task<ViewModels.DataTablesObject<ViewModels.Product>> GetAllProductsAsync(ViewModels.SearchFilters searchFilters)
        {
            var items = await (from vp in context.vProductAndDescription
                              join p in context.Product
                              on vp.ProductID equals p.ProductID
                              join pm in context.ProductModel
                              on p.ProductModelID equals pm.ProductModelID
                              join pmx in context.ProductModelProductDescriptionCulture
                              on pm.ProductModelID equals pmx.ProductModelID
                              where pmx.IsDeleted == false
                              select vp).ToListAsync();

            var productItems = await context.vProductAndDescription.Select(x => new ViewModels.Product()
            {
                ProductID = x.ProductID,
                Name = x.Name,
                ProductModel = x.ProductModel,
                CultureID = x.CultureID,
                Description = x.Description
            }).ToListAsync();

            if (!string.IsNullOrEmpty(searchFilters.SearchValue))
            {
                var keyword = searchFilters.SearchValue.ToLower().Trim();
                productItems = productItems
                    .Where(s => s.Name.ToLower().Contains(keyword) ||
                    s.ProductID.ToString().Contains(keyword) ||
                    s.Name.ToLower().Contains(keyword) ||
                    s.ProductModel.ToLower().Contains(keyword) ||
                    s.CultureID.ToLower().Contains(keyword))
                    .ToList();
            }
            var rawData = productItems
                .OrderBy(searchFilters.OrderBy)
                .Skip(searchFilters.DisplayStart)
                .Take(searchFilters.DisplayLength)
                .ToList();

            return new ViewModels.DataTablesObject<ViewModels.Product>
            {
                aaData = rawData,
                iTotalDisplayRecords = productItems.Count,
                iTotalRecords = productItems.Count
            };
        }
        public async Task<ViewModels.Product> GetProductById(int id, string culture)
        {
            if (id == 0)
                return null;

            var product = await context.vProductAndDescription.SingleAsync(e => e.ProductID == id && e.CultureID == culture);
            var cultures = await context.Culture.Select(d => d.CultureID).ToListAsync();

            return new ViewModels.Product()
            {
                ProductID = product.ProductID,
                Name = product.Name,
                ProductModel = product.ProductModel,
                CultureID = product.CultureID,
                Description = product.Description,
                Cultures = cultures
            };
        }
        public async Task UpdateProduct(ViewModels.Product product)
        {
            if (product == null || product.ProductID == 0)
                return;

            var productid = await context.Product.SingleOrDefaultAsync(p => p.ProductID == product.ProductID);
            if (productid == null)
                return;
            productid.Name = product.Name;

            var pmodel = await context.ProductModel.SingleOrDefaultAsync(pm => pm.ProductModelID == productid.ProductModelID);
            if (pmodel == null)
                return;

            pmodel.Name = product.ProductModel;

            var pmpdc = await context.ProductModelProductDescriptionCulture.SingleOrDefaultAsync(px => px.ProductModelID == pmodel.ProductModelID && px.CultureID == product.CultureID);

            if (pmpdc == null)
                return;
            context.ProductModelProductDescriptionCulture.Remove(pmpdc);
             var newpmpdc = new ProductModelProductDescriptionCulture
            {
                ProductModelID = pmpdc.ProductModelID,
                ProductDescriptionID = pmpdc.ProductDescriptionID,
                CultureID = product.CultureID,
                ModifiedDate = DateTime.Now
            };

            context.ProductModelProductDescriptionCulture.Add(newpmpdc);

            var description = await context.ProductDescription.SingleOrDefaultAsync(d => d.ProductDescriptionID == pmpdc.ProductDescriptionID);
            if (description == null)
                return;

            description.Description = product.Description;

            await context.SaveChangesAsync();
        }
        public async Task DeleteProduct(int id, string cultureid)
        {
            if (id == 0 && cultureid == null)
                return;

            var prodpmdc = await (from pmx in context.ProductModelProductDescriptionCulture
                                  join pm in context.ProductModel
                                  on pmx.ProductModelID equals pm.ProductModelID
                                  join p in context.Product
                                  on pm.ProductModelID equals p.ProductModelID
                                  where p.ProductID == id & pmx.CultureID == cultureid
                                  select pmx).ToListAsync();
            var product =   prodpmdc.SingleOrDefault(x => x.CultureID == cultureid);
            product.IsDeleted = true;

             await context.SaveChangesAsync();
        }
    }

}