namespace Practice.Repository
{
    using Practice.DAL;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels = Practice.Core.ViewModels;
    using System.Linq.Dynamic;
    using Practice.Core.Repositories;

    class ProductsRepository :IProductsRepository
    {
        private readonly Context context;
        public ProductsRepository()
        {
            context = new Context();
        }
        public async Task<ViewModels.DataTablesObject<ViewModels.Product>> GetAllProductsAsync(ViewModels.SearchFilters searchFilters)
        {
            var productItems = await context.vProductAndDescription
                .Where(e => context.Product.FirstOrDefault(y => y.ProductID.Equals(e.ProductID)).IsDeleted == false)
                .Select(x => new ViewModels.Product()
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
       public async Task<ViewModels.Product> GetProductById(int id,string culture)
        {
            if (id == 0)
                return null;

            var product = await context.vProductAndDescription.SingleAsync(e => e.ProductID == id && e.CultureID==culture);

            return new ViewModels.Product()
            {
                ProductID = product.ProductID,
                Name = product.Name,
                ProductModel = product.ProductModel,
                CultureID = product.CultureID,
                Description = product.Description
            };
        }
        public async Task UpdateProduct(ViewModels.Product product)
        {

           if (product == null || product.ProductID == 0)
               return;

            var productid = await context.Product.SingleOrDefaultAsync(p => p.ProductID == product.ProductID);
            if (productid == null)
                return;
            productid.ProductID = product.ProductID;
            productid.Name = product.Name;

            var pmodel = await context.ProductModel.SingleOrDefaultAsync(pm => pm.ProductModelID == productid.ProductModelID);
            if (pmodel == null)
                return;

            pmodel.Name = product.ProductModel;

            var pmpdc = await context.ProductModelProductDescriptionCulture.SingleOrDefaultAsync(px => px.ProductModelID == pmodel.ProductModelID && px.CultureID == product.CultureID);

            if (pmpdc == null)
                return;
            pmpdc.CultureID = product.CultureID;

            var description = await context.ProductDescription.SingleOrDefaultAsync(d => d.ProductDescriptionID == pmpdc.ProductDescriptionID);
            if (description == null)
                return;

            description.Description = product.Description;


            //var pmodel =  context.ProductModel.Select(m => context.ProductModelProductDescriptionCulture
            //    .SingleOrDefaultAsync(pmx => pmx.ProductModelID == m.ProductModelID));
            //if (pmodel == null)
            //    return;
            //await pmodel.FirstOrDefaultAsync()
                
              
            //  var productid = await context.Product.FirstOrDefaultAsync(x => x.ProductID == product.ProductID);
            //  if (productid == null)
            //      return;
            //  productid.ProductID = product.ProductID;
            //  //Product 
            //  var productmodel = from pm in context.ProductModel
            //                     join p in context.Product
            //                     on pm.ProductModelID equals p.ProductModelID
            //                     where p.ProductID == product.ProductID
            //                     select pm;

            //  if (productmodel == null)
            //      return          
            ////var productm= await  productmodel.FirstOrDefaultAsync(x=>x.Name == product.ProductModel);
            //   = product.ProductModel;

            //  var culture = await context.Culture.FirstOrDefaultAsync(p => p.CultureID == product.CultureID);
            //  if (culture == null)
            //      return;

            //  if (culture != null)
            //  {
            //      culture.CultureID = product.CultureID;
            //  }

            //  var description =  from d in context.ProductDescription
            //                                       join pmx in context.ProductModelProductDescriptionCulture
            //                                       on d.ProductDescriptionID equals pmx.ProductDescriptionID
            //                                       join pm in context.ProductModel
            //                                       on pmx.ProductModelID equals pm.ProductModelID
            //                                       join p in context.Product
            //                                       on pm.ProductModelID equals p.ProductModelID
            //                                       where p.ProductID == product.ProductID
            //                                       select d;
            //  // var description = await context.ProductDescription.FirstOrDefaultAsync(d => d.Description.Equals(product.Description));

            //  if (description == null)
            //      return;
            // var desc= await description.FirstOrDefaultAsync(x => x.Description == product.Description);
            //  if (description != null)
            //  {
            //      desc.Description = product.Description;
            //  }



            await context.SaveChangesAsync();
        }
        public async Task DeleteProduct(int id)
        {
            if (id == 0)
                return;

            var product = await context.Product.SingleAsync(x => x.ProductID==id);
            product.IsDeleted = true;

            await context.SaveChangesAsync();
        }
        //public async Task Description(int id, string culture)
        //{
        //    if(id==0 && culture!=null)
        //        return;
        //    else
        //    {
        //        var description = await context.vProductAndDescription.SingleAsync(x => x.ProductID == id && x.CultureID == culture);
               
        //    }
        //}
    }

}