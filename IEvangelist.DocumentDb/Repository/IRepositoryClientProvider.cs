using Microsoft.Azure.Documents;

namespace IEvangelist.DocumentDb.Repository
{
    public interface IRepositoryClientProvider
    {
        IDocumentClient DocumentClient { get; }
    }
}