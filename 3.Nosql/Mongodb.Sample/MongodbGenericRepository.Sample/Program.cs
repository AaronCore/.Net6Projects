using System;
using MongoDB.Driver;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;

namespace MongodbGenericRepository.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ITestRepository testRepository = new TestRepository("mongodb://127.0.0.1:27017", "test");

                var doc = new TestDocument()
                {
                    Text = "123"
                };

                testRepository.AddOne(doc);

                Console.WriteLine("end...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();
        }
    }
}
