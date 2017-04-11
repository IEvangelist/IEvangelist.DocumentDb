using IEvangelist.DocumentDb.Extensions;
using IEvangelist.DocumentDb.Settings;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
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
            var (databaseCreated, (collectionCreated, documentCollection)) =
                await IsFirstInitializationAsync();

            if (databaseCreated && collectionCreated)
            {
                await CreatedStoreProceduresAsync(documentCollection);
            }
        }

        private async Task<(bool dbCreated, (bool colCreated, DocumentCollection col))> IsFirstInitializationAsync()
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

        private async Task<(bool, DocumentCollection)> CreateCollectionIfNotExistsAsync()
        {
            try
            {
                var documentCollection = 
                    await _client.ReadDocumentCollectionAsync(
                        _settings.CreateDocumentCollectionUri());
                return (false, documentCollection);
            }
            catch (DocumentClientException e)
            when (e.StatusCode == HttpStatusCode.NotFound)
            {
                var documentCollection =
                    await _client.CreateDocumentCollectionAsync(
                        _settings.CreateDatabaseUri(),
                        _settings.CreateDocumentCollection(),
                        _settings.DefaultRequestOptions);
                return (true, documentCollection);
            }
        }

        private async Task CreatedStoreProceduresAsync(DocumentCollection collection)
        {
            var assembly = Assembly.GetEntryAssembly();
            
            foreach (var (resource, id) in assembly.GetManifestResourceNames()
                                                   .Where(name => name.EndsWith(".js"))
                                                   .Select(AsIdentifier))
            {
                using (var stream = assembly.GetManifestResourceStream(resource))
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var storedProcedure = new StoredProcedure
                        {
                            Id = id,
                            Body = await reader.ReadToEndAsync()
                        };

                        await _client.CreateStoredProcedureAsync(collection.StoredProceduresLink, storedProcedure);
                    }
                }
            }

            (string, string) AsIdentifier(string resourceName)
                => (resourceName,
                    resourceName.Replace("IEvangelist.DocumentDb.StoredProcedures.", "")
                                .Replace(".js", ""));
        }
    }
}