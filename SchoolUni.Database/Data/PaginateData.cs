using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolUni.Database.Data
{
    public class PaginatedData<T>
    {
        public List<T> Items { get; set; }
        public int TotalResults { get; set; }

        public static async Task<PaginatedData<T>> GetPagedDataAsync(IQueryable<T> query, int offset, int limit)
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

        public static PaginatedData<T> GetPagedData(IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedData<T>
            {
                TotalResults = query.Count(),
                Items = limit > 0 ? query.Skip(offset).Take(limit).ToList() : query.ToList()
            };
            return data;
        }
    }
}
