﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Practice.Core.Repositories
{
    public interface IHumanResourcesRepository
    {
        Task<List<ViewModels.Employee>> GetEmployees();
        Task<ViewModels.Employee> GetEmployeeById(int id);
        Task CreateEmployee(ViewModels.Employee employee);
        Task DeleteEmployee(int id);
        Task UpdateEmployee(ViewModels.Employee employee);
    }
}
