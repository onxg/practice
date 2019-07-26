using Practice.Core.Repositories;
using Practice.Core.ViewModels;
using Practice.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels = Practice.Core.ViewModels;
using System.Linq.Dynamic;

namespace Practice.Repository
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly Context context;

        public HistoryRepository()
        {
            context = new Context();
        }

        public async Task<DataTablesObject<History>> GetAllHistoriesAsync(SearchFilters searchFilters)
        {
            var history = await context.vEmployeeDepartmentHistory
                .Where(e => context.EmployeeDepartmentHistory
                .FirstOrDefault(y => y.BusinessEntityID == e.BusinessEntityID && y.StartDate == e.StartDate && y.Department.Name == e.Department)
                .isDeleted == false)
                .Select(x => new ViewModels.History()
                {
                    Id = x.BusinessEntityID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Department = x.Department,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                }).ToListAsync();

            if (!string.IsNullOrEmpty(searchFilters.SearchValue))
            {
                var keyword = searchFilters.SearchValue.ToLower().Trim();
                history = history
                    .Where(s => s.Id.ToString().Contains(keyword) ||
                    s.FirstName.ToLower().Contains(keyword) ||
                    s.LastName.ToLower().Contains(keyword) ||
                    s.Department.ToLower().Contains(keyword))
                    .ToList();
            }

            if (searchFilters.StartDate != DateTime.MinValue || searchFilters.EndDate != DateTime.MaxValue)
            {
                history = history.Where(f => f.StartDate > searchFilters.StartDate && f.StartDate < searchFilters.EndDate).ToList();
            }

            var rawData = history
                .OrderBy(searchFilters.OrderBy)
                .Skip(searchFilters.DisplayStart)
                .Take(searchFilters.DisplayLength)
                .ToList();

            return new ViewModels.DataTablesObject<ViewModels.History>
            {
                aaData = rawData,
                iTotalDisplayRecords = history.Count,
                iTotalRecords = history.Count
            };
        }

        public async Task<History> GetHistory(int id, DateTime date, string department)
        {
            if (id == 0)
                return null;

            var history = await context.vEmployeeDepartmentHistory.SingleAsync(e => e.BusinessEntityID == id && e.StartDate == date && e.Department == department);
            var departments = await context.Department.Select(d => d.Name).ToListAsync();

            return new ViewModels.History()
            {
                Id = history.BusinessEntityID,
                FirstName = history.FirstName,
                LastName = history.LastName,
                Department = history.Department,
                StartDate = history.StartDate,
                EndDate = history.EndDate,
                AllDepartments = departments
            };
        }

        public async Task Delete(int id, DateTime date, string department)
        {
            if (id == 0)
                return;

            var history = await context.EmployeeDepartmentHistory.SingleAsync(x => x.BusinessEntityID == id && x.StartDate == date && x.Department.Name == department);

            history.isDeleted = true;

            await context.SaveChangesAsync();
        }

        public async Task Update(History history, History oldHistory)
        {
            var record = await context.EmployeeDepartmentHistory
                .SingleAsync(x => x.BusinessEntityID == history.Id && x.StartDate == oldHistory.StartDate && x.Department.Name == oldHistory.Department);

            if (record == null)
                return;

            //can't modify StartDate - primary key so I remove original record and add new one updated
            context.EmployeeDepartmentHistory.Remove(record);

            var newRecord = new EmployeeDepartmentHistory
            {
                BusinessEntityID = record.BusinessEntityID,
                ShiftID = record.ShiftID,
                StartDate = history.StartDate,
                EndDate = history.EndDate,
                ModifiedDate = DateTime.Now,
                Employee = context.Employee.Single(e => e.BusinessEntityID == record.BusinessEntityID),
                Department = context.Department.Where(e => e.Name == history.Department).First(),
                Shift = record.Shift
            };

            newRecord.Employee.Person.FirstName = history.FirstName;
            newRecord.Employee.Person.LastName = history.LastName;

            context.EmployeeDepartmentHistory.Add(newRecord);

            await context.SaveChangesAsync();
        }

        public async Task<List<string>> GetDepartmentsList()
        {
            var departments = await context.Department.Select(x => x.Name).ToListAsync();
            return departments;
        }

        public async Task CreateHistory(History history)
        {

            // Check if employee exists in the db
            if (await RecordExists(history))
                throw new InvalidOperationException("This history record already exists.");

            // Create a person entity with all needed relations
            var person = new Person
            {
                BusinessEntity = new BusinessEntity
                {
                    rowguid = Guid.NewGuid(),
                    ModifiedDate = DateTime.Now,
                    BusinessEntityAddress = new List<BusinessEntityAddress>
                    {
                        new BusinessEntityAddress
                        {
                            Address = new Address
                            {
                                AddressLine1 = "Not specified",
                                AddressLine2 = Guid.NewGuid().ToString(), // to avoid bug with 2 different employees having the same address
                                City = "Not specified",
                                PostalCode = "Not specified",
                                rowguid = Guid.NewGuid(),
                                ModifiedDate = DateTime.Now,
                                StateProvince = await context.StateProvince.FirstOrDefaultAsync()
                            },
                            AddressType = await context.AddressType.FirstOrDefaultAsync(at => at.Name == "Home"),
                            ModifiedDate = DateTime.Now,
                            rowguid = Guid.NewGuid()
                        }
                    }
                },
                FirstName = history.FirstName,
                LastName = history.LastName,
                PersonType = "EM",
                NameStyle = false,
                EmailPromotion = 0,
                rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now,
            };

            // Create new employee with random properties
            var random = new Random();
            var employee = new DAL.Employee
            {
                Person = person,
                NationalIDNumber = random.Next().ToString(),
                LoginID = $"adventure-works/{history.FirstName.ToLower() + random.Next().ToString()}",
                JobTitle = "Intern",
                BirthDate = new DateTime(random.Next(1970, 2000), random.Next(1, 12), random.Next(1, 28)),
                MaritalStatus = "S",
                Gender = random.Next(0, 1) == 1 ? "M" : "F",
                HireDate = DateTime.Now - TimeSpan.FromDays(1),
                SalariedFlag = false,
                VacationHours = 25,
                SickLeaveHours = 0,
                CurrentFlag = true,
                rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now
            };

            Department department = await context.Department.Where(e => e.Name == history.Department).FirstAsync();
            Shift shift = await context.Shift.Where(e => e.ShiftID == 1).FirstAsync();

            var record = new EmployeeDepartmentHistory
            {
                ShiftID = shift.ShiftID,
                StartDate = history.StartDate,
                EndDate = history.EndDate,
                ModifiedDate = DateTime.Now,
                Department = department,
                Employee = employee,
                Shift = shift
            };

            context.EmployeeDepartmentHistory.Add(record);
            await context.SaveChangesAsync();
        }

        private async Task<bool> RecordExists(History history)
        {
            var foundRecord = await context.vEmployeeDepartmentHistory.FirstOrDefaultAsync(e => e.FirstName == history.FirstName &&
                                                                            e.LastName == history.LastName &&
                                                                            e.Department == history.Department &&
                                                                            e.StartDate == history.StartDate);
            return foundRecord != null;
        }
    }
}
