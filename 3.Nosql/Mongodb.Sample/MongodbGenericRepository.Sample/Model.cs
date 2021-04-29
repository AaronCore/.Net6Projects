using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;

namespace MongodbGenericRepository.Sample
{
    [CollectionName("TEST1")]
    public class TestDocument : Document
    {
        public string Text { get; set; }
    }
}
