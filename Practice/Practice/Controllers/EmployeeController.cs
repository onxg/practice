namespace Practice.Controllers
{
    using Practice.Core.Repositories;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class EmployeeController : Controller
    {
        private readonly IHumanResourcesRepository repository;

        public EmployeeController(IHumanResourcesRepository _repository)
        {
            repository = _repository;
        }

        public async Task<ActionResult> Index()
        {
            var model = await repository.GetEmployees();

            return View(model);
        }


        // GET: Employees/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {

            if(!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            await repository.DeleteEmployee(id.Value);

            return RedirectToAction("Index");
        }
    }
}