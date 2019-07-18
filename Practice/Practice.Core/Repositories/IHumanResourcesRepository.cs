using Practice.Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Practice.Core.Repositories
{
    public interface IHumanResourcesRepository
    {
        Task<DataTablesObject<Employee>> GetEmployeesAsync(SearchFilters searchFilters);
        Task<ViewModels.Employee> GetEmployeeById(int id);
        Task CreateEmployee(ViewModels.Employee employee);
        Task DeleteEmployee(int id);
        Task UpdateEmployee(ViewModels.Employee employee);
    }
}
