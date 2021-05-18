using System;

namespace MessagePack.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var peopleInfo = new People
            {
                Age = 25,
                FirstName = "xiaoxiao",
                LastName = "wu",
            };
            byte[] bytes = MessagePackSerializer.Serialize(peopleInfo);
            var people = MessagePackSerializer.Deserialize<People>(bytes);

            var json = MessagePackSerializer.ConvertToJson(bytes);
            Console.WriteLine(people);
        }
    }
    [MessagePackObject]
    public class People
    {
        [Key(0)]
        public int Age { get; set; }

        [Key(1)]
        public string FirstName { get; set; }

        [Key(2)]
        public string LastName { get; set; }

        [IgnoreMember]
        public string FullName => FirstName + LastName;
    }
}
