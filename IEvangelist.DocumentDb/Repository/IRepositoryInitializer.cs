using System.Threading.Tasks;

namespace IEvangelist.DocumentDb.Repository
{
    public interface IRepositoryInitializer
    {
        Task InitializeAsync();
    }
}