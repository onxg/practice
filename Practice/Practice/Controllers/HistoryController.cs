using Practice.Core.Repositories;
using Practice.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Practice.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IHistoryRepository historyRepository;

        public HistoryController(IHistoryRepository _historyRepository)
        {
            historyRepository = _historyRepository;
        }


        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetAllHistoriesAsync(FormCollection form)
        {
            var searchFilters = new SearchFilters(form)
            {
                OrderBy = GetOrderBy(form)
            };

            var result = await historyRepository.GetAllHistoriesAsync(searchFilters);

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
                    orderBy = "Id";
                    break;
                case "1":
                    orderBy = "FirstName";
                    break;
                case "2":
                    orderBy = "LastName";
                    break;
                case "3":
                    orderBy = "Department";
                    break;
                case "4":
                    orderBy = "StartDate";
                    break;
                default:
                    orderBy = "Id";
                    break;
            }

            orderBy = (orderBy + " " + form["order[0][dir]"]).ToUpper();

            return orderBy;
        }
    }
}