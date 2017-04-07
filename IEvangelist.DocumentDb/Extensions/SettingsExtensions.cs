using IEvangelist.DocumentDb.Settings;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;

namespace IEvangelist.DocumentDb.Extensions
{
    public static class SettingsExtensions
    {
        public static Database CreateDatabase(this RepositorySettings settings)
            => new Database { Id = settings.DatabaseId };

        public static Uri CreateDatabaseUri(this RepositorySettings settings)
            => UriFactory.CreateDatabaseUri(settings.DatabaseId);

        public static DocumentCollection CreateDocumentCollection(this RepositorySettings settings)
            => new DocumentCollection { Id = settings.CollectionId };

        public static Uri CreateDocumentUri(this RepositorySettings settings, string id)
            => UriFactory.CreateDocumentUri(settings.DatabaseId, settings.CollectionId, id);

        public static Uri CreateDocumentCollectionUri(this RepositorySettings settings)
            => UriFactory.CreateDocumentCollectionUri(settings.DatabaseId, settings.CollectionId);
    }
}