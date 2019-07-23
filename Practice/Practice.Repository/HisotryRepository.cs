﻿using Practice.Core.Repositories;
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
            var history = await context.vEmployeeDepartmentHistory.Select(x => new ViewModels.History()
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
    }
}
