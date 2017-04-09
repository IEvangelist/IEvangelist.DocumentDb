using IEvangelist.DocumentDb.Extensions;
using IEvangelist.DocumentDb.Models;
using IEvangelist.DocumentDb.Settings;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace IEvangelist.DocumentDb.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseDocument
    {
        private readonly RepositorySettings _settings;
        private IDocumentClient _client;

        public Repository(IOptions<RepositorySettings> options,
                          IRepositoryClientProvider clientProvider)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _client = clientProvider?.DocumentClient ?? throw new ArgumentNullException(nameof(clientProvider));
        }

        public async Task<T> GetAsync(string id)
        {
            try
            {
                Document document =
                    await _client.ReadDocumentAsync(_settings.CreateDocumentUri(id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e) 
            when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            // TODO: limited linq and demo "SELECT blah"
            
            IDocumentQuery<T> query =
                _client.CreateDocumentQuery<T>(_settings.CreateDocumentCollectionUri(),
                                               _settings.DefaultFeedOptions)
                       .Where(predicate)
                       .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<Document> CreateAsync(T value)
            => await _client.CreateDocumentAsync(_settings.CreateDocumentCollectionUri(), value);

        public Task<Document[]> CreateAsync(IEnumerable<T> values)
            => Task.WhenAll(values.Select(CreateAsync));

        public async Task<Document> UpdateAsync(string id, T value)
            => await _client.ReplaceDocumentAsync(_settings.CreateDocumentUri(id), value);

        public Task DeleteAsync(string id)
            => _client.DeleteDocumentAsync(_settings.CreateDocumentUri(id));
    }
}