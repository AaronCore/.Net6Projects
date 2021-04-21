using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongodb.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MongoClient("mongodb://192.168.90.210:27017/irpdb");
            var dataBase = client.GetDatabase("irpdb");
            var collection = dataBase.GetCollection<BsonDocument>("MDA021");

            // equal
            var filter = new BsonDocument()
            {
                {"MDD014.Text","00841" },
                {"MDD015","18818682813" }
            };

            // not equal
            //var filter = new BsonDocument()
            //{
            //    {"MDD014.Text",new BsonDocument() { { "$ne", "123" } } }
            //};

            // contains
            //var filter = new BsonDocument()
            //{
            //    {"MDD014.Text",new BsonRegularExpression("123","i")  }
            //};

            // or
            //var filter = new BsonDocument()
            //{
            //    {"$or",
            //        new  BsonArray(){
            //             new BsonDocument() { { "MDD014.Text", "00841" } },
            //             new BsonDocument() { { "MDD015", "18818682813" } },
            //        }
            //    }
            //};

            // in
            //var filter = new BsonDocument()
            //{
            //    {"MDD014.Text",new BsonDocument() { { "$in", new BsonArray() { "1","2","3"} } } }
            //};

            // exists
            //var filter = new BsonDocument()
            //{
            //    {"MDD014.Text",new BsonDocument() { { "$exists", true } } }
            //};

            var sort = new BsonDocument()
            {
                {"_id",1 }
            };
            var options = new FindOptions<BsonDocument>()
            {
                Sort = sort,
                Limit = 100
            };
            using (var cursor = await collection.FindAsync(filter, options))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        Console.WriteLine(document.GetElement("_id"));
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
