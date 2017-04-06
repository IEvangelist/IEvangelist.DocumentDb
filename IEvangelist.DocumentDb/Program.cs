using Microsoft.AspNetCore.Hosting;
using static IEvangelist.DocumentDb.Startup;

namespace IEvangelist.DocumentDb
{
    public class Program
    {
        public static void Main(string[] args) => BuildWebHost().Run();
    }
}