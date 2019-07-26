namespace Practice.Core.Repositories
{
    using Practice.Core.ViewModels;
    using System.Threading.Tasks;
   public interface IProductsRepository
    {
        Task<DataTablesObject<Product>> GetAllProductsAsync(SearchFilters searchFilters);
        Task<ViewModels.Product> GetProductById(int id,string culture);
        Task UpdateProduct(ViewModels.Product product);
        Task DeleteProduct(int id,string cultureid);
        //Task Description(int id,string culture);
    }
}
