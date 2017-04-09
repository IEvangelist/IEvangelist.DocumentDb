using IEvangelist.DocumentDb.Models;
using IEvangelist.DocumentDb.Settings;
using Microsoft.Extensions.Options;

namespace IEvangelist.DocumentDb.Repository
{
    public class SqlRepository<T> : Repository<T> where T : BaseDocument
    {
        public SqlRepository(IOptions<RepositorySettings> options,
                             IRepositoryClientProvider clientProvider)
            : base(options, clientProvider)
        {
        }
    }
}