using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeUni.Data
{
    public class Paginator
    {
        public static async Task<PaginatedData<T>> GetPagedDataAsync<T>(IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedData<T> { TotalResults = query.Count() };
            if (limit > 0)
            {
                data.Items = await query.Skip(offset).Take(limit).ToListAsync();
            }
            else
            {
                data.Items = await query.ToListAsync();
            }
            return data;
        }

        public static PaginatedData<T> GetPagedData<T>(IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedData<T>
            {
                TotalResults = query.Count(),
                Items = limit > 0 ? query.Skip(offset).Take(limit).ToList() : query.ToList()
            };
            return data;
        }
        public static async Task<PaginatedData<U>> GetPagedDataAsync<T, U>(IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedData<U> { TotalResults = query.Count() };
            if (limit > 0)
            {
                data.Items = Mapper.Map<List<T>, List<U>>(await query.Skip(offset).Take(limit).ToListAsync());
            }
            else
            {
                data.Items = Mapper.Map<List<T>, List<U>>(await query.ToListAsync());
            }
            return data;
        }

        public static PaginatedData<U> GetPagedData<T, U>(IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedData<U>
            {
                TotalResults = query.Count(),
                Items = Mapper.Map<List<T>, List<U>>(limit > 0 ? query.Skip(offset).Take(limit).ToList() : query.ToList())
            };
            return data;
        }
    }

    public class PaginatedData<T>
    {
        public List<T> Items { get; set; }
        public int TotalResults { get; set; }

    }
}
