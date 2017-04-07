using IEvangelist.DocumentDb.Models;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IEvangelist.DocumentDb.Repository
{
    public interface IRepository<T> where T : BaseDocument
    {
        Task InitializeAsync();

        Task<T> GetAsync(string id);
        
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);

        Task<Document> CreateAsync(T value);

        Task<Document[]> CreateAsync(IEnumerable<T> values);

        Task<Document> UpdateAsync(string id, T value);

        Task DeleteAsync(string id);
    }
}