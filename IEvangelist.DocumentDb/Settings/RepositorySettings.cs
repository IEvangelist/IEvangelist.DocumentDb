using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;

namespace IEvangelist.DocumentDb.Settings
{
    public class RepositorySettings : IOptions<RepositorySettings>
    {
        public string Endpoint { get; set; }

        public string AuthKey { get; set; }

        public string DatabaseId { get; set; }

        public string CollectionId { get; set; }

        RepositorySettings IOptions<RepositorySettings>.Value => this;

        public ConnectionPolicy DefaultConnectionPolicy { get; } =
            new ConnectionPolicy { EnableEndpointDiscovery = false };

        public RequestOptions DefaultRequestOptions { get; } =
            new RequestOptions { OfferThroughput = 1000 };

        public FeedOptions DefaultFeedOptions { get; } =
            new FeedOptions { MaxItemCount = -1 };
    }
}