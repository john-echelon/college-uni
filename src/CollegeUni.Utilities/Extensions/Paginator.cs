using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Utilities.Extensions
{
    public static class Paginator
    {
        public static async Task<PaginatedResult<T>> ToPageResultAsync<T>(this IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedResult<T> { TotalResults = query.Count() };
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

        public static PaginatedResult<T> ToPageResult<T>(this IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedResult<T>
            {
                TotalResults = query.Count(),
                Items = limit > 0 ? query.Skip(offset).Take(limit).ToList() : query.ToList()
            };
            return data;
        }
        public static async Task<PaginatedResult<U>> ToPageResultAsync<T, U>(this IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedResult<U> { TotalResults = query.Count() };
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

        public static PaginatedResult<U> ToPageResult<T, U>(this IQueryable<T> query, int offset, int limit)
        {
            var data = new PaginatedResult<U>
            {
                TotalResults = query.Count(),
                Items = Mapper.Map<List<T>, List<U>>(limit > 0 ? query.Skip(offset).Take(limit).ToList() : query.ToList())
            };
            return data;
        }

        public static Task<PaginatedResult<T>> ToPageResult<T>(this IEnumerable<T> query, int offset, int limit)
        {
            var data = new PaginatedResult<T> { TotalResults = query.Count() };
            if (limit > 0)
            {
                data.Items = query.Skip(offset).Take(limit).ToList();
            }
            else
            {
                data.Items = query.ToList();
            }
            return Task.FromResult(data);
        }

        public static Task<PaginatedResult<U>> ToPageResult<T, U>(this IEnumerable<T> query, int offset, int limit)
        {
            var data = new PaginatedResult<U> { TotalResults = query.Count() };
            if (limit > 0)
            {
                data.Items = Mapper.Map<List<T>, List<U>>(query.Skip(offset).Take(limit).ToList());
            }
            else
            {
                data.Items = Mapper.Map<List<T>, List<U>>(query.ToList());
            }
            return Task.FromResult(data);
        }
    }

    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalResults { get; set; }
    }
}
