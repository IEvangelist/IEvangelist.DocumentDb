using IEvangelist.DocumentDb.Extensions;
using IEvangelist.DocumentDb.Models;
using IEvangelist.DocumentDb.Settings;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Linq.Enumerable;
using static Newtonsoft.Json.JsonConvert;

namespace IEvangelist.DocumentDb.Repository
{
    public class SqlRepository<T> : Repository<T> where T : BaseDocument
    {
        public SqlRepository(IOptions<RepositorySettings> options,
                             IRepositoryClientProvider clientProvider)
            : base(options, clientProvider)
        {
        }

        public async override Task DeleteAsync(string id)
        {
            var query = $"SELECT * FROM docs WHERE docs.id = '{id}'";
            var delete = await GetStoredProcedureAsync("Delete");

            var continueDelete = true;
            while (continueDelete)
            {
                var obj =
                    await _client.ExecuteStoredProcedureAsync<object>(
                        delete.SelfLink,
                        query);

                var response = DeserializeObject<dynamic>(obj.Response.ToString());
                continueDelete = (bool)response.continuation;
            }
        }

        public async override Task<Document> CreateAsync(T document)
        {
            var docs = await CreateAsync(new[] { document });
            return docs.FirstOrDefault();
        }

        public async override Task<Document[]> CreateAsync(IEnumerable<T> documents)
        {
            var upsert = await GetStoredProcedureAsync("Upsert");
            var response = 
                await _client.ExecuteStoredProcedureAsync<dynamic>(upsert.SelfLink, documents);

            return await Task.FromResult(Empty<Document>().ToArray());
        }

        private async Task<StoredProcedure> GetStoredProcedureAsync(string name)
        {
            DocumentCollection documentCollection =
                await _client.ReadDocumentCollectionAsync(
                    _settings.CreateDocumentCollectionUri());

            var storeProcedures =
                _client.CreateStoredProcedureQuery(documentCollection.StoredProceduresLink)
                       .ToList();

            return storeProcedures.FirstOrDefault(sp => sp.Id == name);
        }
    }
}