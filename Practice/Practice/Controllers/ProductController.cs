namespace Practice.Controllers
{
    using Practice.Core.Repositories;
    using Practice.Core.ViewModels;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ProductController : Controller
    {
        private readonly IProductsRepository productRepository;
        public ProductController(IProductsRepository _productRepository)
        {
            productRepository = _productRepository;
        }
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult>GetAllProductsAsync(FormCollection form)
        {
            var searchFilters = new SearchFilters(form)
            {
                OrderBy = GetOrderBy(form)
            };
            var result = await productRepository.GetAllProductsAsync(searchFilters);
            return this.Json(
                new
                {
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                }, JsonRequestBehavior.AllowGet);
        }
        public string GetOrderBy(FormCollection form)
        {
            string orderBy = form["order[0][column]"];
            switch (orderBy)
            {
                case "0":
                default:
                    orderBy = "ProductID";
                    break;
                case "1":
                    orderBy = "Name";
                    break;
                case "2":
                    orderBy = "ProductModel";
                    break;
                case "3":
                    orderBy = "CultureID";
                    break;
            }
            orderBy = (orderBy + " " + form["order[0][dir]"]).ToUpper();
            return orderBy;
        }
        public async Task<ActionResult> GetProduct(FormCollection formCollection)
        {
            if (!int.TryParse(formCollection["id"], out int id) || id == 0)
                return Json(new { status = "error", message = "Invalid id." });
            string culture = formCollection["culture"];
            if(culture==null)
                return Json(new { status = "error", message = "Invalid cultureID" });

            var product = await productRepository.GetProductById(id, culture);

            return Json(new { status = "success", product });
        }
        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection formCollection)
        {
            
            if ((!int.TryParse(formCollection["id"], out int id) || id == 0))
                return Json(new { status = "error", message = "Invalid id." });
            string culture = formCollection["culture"];
            if (culture == null)
                return Json(new { status = "error", message = "Invalid cultureID" });

            Core.ViewModels.Product product = new Core.ViewModels.Product
            {
                ProductID = id,
                Name = formCollection["Name"],
                ProductModel = formCollection["ProductModel"],
                CultureID = culture,
                Description = formCollection["Description"]
            };
            try
            {
               await productRepository.UpdateProduct(product);
            }
            catch(InvalidOperationException e)
            {
                return Json(new { status = "error", message = e.Message });
            }
            return Json(new { status = "success", message = "Product has been successfully edited." });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(FormCollection formCollection)
        {
           
            if (!int.TryParse(formCollection["id"], out int id) || id == 0)
            {
                return (Json(new { status = "error", message = "Invalid id." }));         
            }
            string cultureid = formCollection["culture"];
            if (cultureid ==null)
            {
                return(Json(new { status = "error", message = "Invalid CultureID." }));
            }
            try
            {
                await productRepository.DeleteProduct(id, cultureid);
            }
            catch(InvalidOperationException e)
            {
                return Json(new { status = "error", message = e.Message });
            }
            return Json(new { status = "success", message = "Product has been successfully deleted." });
        }
    } 
}