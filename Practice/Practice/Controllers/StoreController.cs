namespace Practice.Controllers
{
    using Practice.Core.Repositories;
    using Practice.Core.ViewModels;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class StoreController : Controller
    {
        private readonly ISalesRepository saleRepository;

        public StoreController(ISalesRepository _saleRepository)
        {
            saleRepository = _saleRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetAllStoresAsync(FormCollection form)
        {
            var searchFilters = new SearchFilters(form)
            {
                OrderBy = GetOrderBy(form)
            };

            var result = await saleRepository.GetAllStoresAsync(searchFilters);

            return this.Json(
                new
                {
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                }, JsonRequestBehavior.AllowGet);
        }

        private string GetOrderBy(FormCollection form)
        {
            string orderBy = form["order[0][column]"];

            switch (orderBy)
            {
                case "0":
                default:
                    orderBy = "Name";
                    break;
                case "1":
                    orderBy = "Address";
                    break;
                case "2":
                    orderBy = "PostalCode";
                    break;
                case "3":
                    orderBy = "City";
                    break;
                case "4":
                    orderBy = "Country";
                    break;
            }

            orderBy = (orderBy + " " + form["order[0][dir]"]).ToUpper();

            return orderBy;
        }

        
        public async Task<ActionResult> GetStore(FormCollection formCollection)
        {
            if (!int.TryParse(formCollection["id"], out int id) || id == 0)
                return Json(new { status = "error", message = "Invalid id." });

            var store = await saleRepository.GetStoreById(id);

            return Json(new { status = "success", store });
        }


        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection formCollection)
        {
            if (!int.TryParse(formCollection["id"], out int id) || id == 0)
                return Json(new { status = "error", message = "Invalid id." });

            Core.ViewModels.Store store = new Core.ViewModels.Store
            {
                Id = id,
                Name = formCollection["Name"],
                Address = formCollection["Address"],
                PostalCode = formCollection["PostalCode"],
                City = formCollection["City"],
                Country = formCollection["Country"]
            };

            try
            {
                await saleRepository.UpdateStore(store);
            }
            catch (InvalidOperationException e)
            {
                return Json(new { status = "error", message = e.Message });
            }

            return Json(new { status = "success", message = "Store has been successfully edited." });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(FormCollection formCollection)
        {
            string idStr = formCollection["id"];
            if (!int.TryParse(formCollection["id"], out int id) || id == 0)
                return Json(new { status = "error", message = "Invalid id." });

            try
            {
                await saleRepository.DeleteStore(id);
            }
            catch (InvalidOperationException e)
            {
                return Json(new { status = "error", message = e.Message });
            }

            return Json(new { status = "success", message = "Store has been successfully deleted." });
        }
    }
}