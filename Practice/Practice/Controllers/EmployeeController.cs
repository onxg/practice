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




        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            await repository.DeleteEmployee(id.Value);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue || id == 0)
                return RedirectToAction("Index");

            var model = await repository.GetEmployeeById(id.Value);
            if(model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Core.ViewModels.Employee employee)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { id = employee.Id });

            await repository.UpdateEmployee(employee);
            return RedirectToAction("Index");
        }
    }
}