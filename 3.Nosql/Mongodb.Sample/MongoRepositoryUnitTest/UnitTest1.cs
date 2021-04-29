using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoRepository;
using System.Linq;

namespace MongoRepositoryUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        MongoRepository<PersonEntity> repo = new MongoRepository<PersonEntity>();

        [TestMethod]
        public void Add()
        {
            var personEntity = new PersonEntity
            {
                Name = "Aaron",
                Age = 25
            };
            repo.Add(personEntity);
        }

        [TestMethod]
        public void Select()
        {
            var list = repo.Where(x => x.Name == "Aaron");
        }

        [TestMethod]
        public void Update()
        {
            var entity = repo.FirstOrDefault(x => x.Name == "Aaron");
            entity.Name = "Where";
            repo.Update(entity);
        }

        [TestMethod]
        public void Delete()
        {
            var entity = repo.FirstOrDefault(x => x.Name == "Where");
            repo.Delete(entity.Id);
        }

    }
    public class PersonEntity : Entity
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
