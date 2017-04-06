using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;

namespace IEvangelist.DocumentDb.Settings
{
    public class RepositorySettings : IOptions<RepositorySettings>
    {
        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string DatabaseId { get; set; }

        public string CollectionId { get; set; }

        RepositorySettings IOptions<RepositorySettings>.Value => this;

        public Uri GetDocumentCollectionUri()
            => UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);

        public Uri GetDocumentUri(string id)
            => UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);
    }
}