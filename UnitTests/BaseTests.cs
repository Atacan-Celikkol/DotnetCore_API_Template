using Core.Extensions;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class BaseTests
    {
        protected static ProjectConfiguration projectConfiguration;
        protected static DataContext dataContext;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            testContext.WriteLine("AssemblyInitialize is working");
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
            projectConfiguration = new ProjectConfiguration(configuration, "UnitTest", false, false, false, false, false);
            var dbBuilder = new DbContextOptionsBuilder<DataContext>().UseSqlServer(projectConfiguration.DefaultConnection, opt => opt.UseNetTopologySuite()).Options;
            dataContext = new DataContext(dbBuilder);
        }
    }
}