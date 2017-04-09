using IEvangelist.DocumentDb.Extensions;
using IEvangelist.DocumentDb.Settings;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IEvangelist.DocumentDb.Repository
{
    public class RepositoryInitializerAndProvider : IRepositoryInitializer, 
                                                    IRepositoryClientProvider
    {
        private readonly RepositorySettings _settings;
        private readonly IDocumentClient _client;

        IDocumentClient IRepositoryClientProvider.DocumentClient => _client;

        public RepositoryInitializerAndProvider(IOptions<RepositorySettings> options)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _client =
                new DocumentClient(
                    new Uri(_settings.Endpoint),
                    _settings.AuthKey,
                    _settings.DefaultConnectionPolicy);
        }

        async Task IRepositoryInitializer.InitializeAsync()
        {
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