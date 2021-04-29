using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDbGenericRepository;

namespace MongodbGenericRepository.Sample
{
    public interface ITestRepository : IBaseMongoRepository
    {
        void DropTestCollection<TDocument>();
        void DropTestCollection<TDocument>(string partitionKey);
    }
    public class TestRepository : BaseMongoRepository, ITestRepository
    {
        public TestRepository(string connectionString, string databaseName) : base(connectionString, databaseName)
        {

        }
        public void DropTestCollection<TDocument>()
        {
            MongoDbContext.DropCollection<TDocument>();
        }
        public void DropTestCollection<TDocument>(string partitionKey)
        {
            MongoDbContext.DropCollection<TDocument>(partitionKey);
        }
    }
}
