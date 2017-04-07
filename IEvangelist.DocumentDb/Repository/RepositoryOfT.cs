using IEvangelist.DocumentDb.Extensions;
using IEvangelist.DocumentDb.Models;
using IEvangelist.DocumentDb.Settings;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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

        public Repository(IOptions<RepositorySettings> options)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
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
            IDocumentQuery<T> query =
                _client.CreateDocumentQuery<T>(_settings.CreateDocumentCollectionUri(),
                                               new FeedOptions { MaxItemCount = -1 })
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

        public async Task InitializeAsync()
        {
            _client =
                new DocumentClient(
                    new Uri(_settings.Endpoint),
                    _settings.AuthKey,
                    _settings.DefaultConnectionPolicy);

            var (databaseCreated, collectionCreated) =
                await IsFirstInitializationAsync();

            if (databaseCreated && collectionCreated)
            {
                // Seed logic could go here...
            }
        }

        private async Task<(bool databaseCreated, bool collectionCreated)> IsFirstInitializationAsync()
            => (await CreateDatabaseIfNotExistsAsync(),
                await CreateCollectionIfNotExistsAsync());

        private async Task<bool> CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDatabaseAsync(_settings.CreateDatabaseUri());
                return false;
            }
            catch (DocumentClientException e)
            when (e.StatusCode == HttpStatusCode.NotFound)
            {
                await _client.CreateDatabaseAsync(_settings.CreateDatabase());
                return true;
            }
        }

        private async Task<bool> CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDocumentCollectionAsync(
                    _settings.CreateDocumentCollectionUri());
                return false;
            }
            catch (DocumentClientException e)
            when (e.StatusCode == HttpStatusCode.NotFound)
            {
                await _client.CreateDocumentCollectionAsync(
                        _settings.CreateDatabaseUri(),
                        _settings.CreateDocumentCollection(),
                        _settings.DefaultRequestOptions);
                return true;
            }
        }
    }
}