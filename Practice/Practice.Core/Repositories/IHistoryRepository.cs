namespace Practice.Core.Repositories
{
    using Practice.Core.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHistoryRepository
    {
        Task<DataTablesObject<History>> GetAllHistoriesAsync(SearchFilters searchFilters);
        Task<ViewModels.History> GetHistory(int id, DateTime date, string department);
        Task Delete(int id, DateTime date, string department);
        Task Update(History history, History oldHistory);
        Task<List<string>> GetDepartmentsList();
        Task CreateHistory(History history);
    }
}
