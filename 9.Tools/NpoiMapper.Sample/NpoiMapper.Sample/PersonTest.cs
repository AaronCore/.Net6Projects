using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper;

namespace NpoiMapper.Sample
{
    public class PersonTest
    {
        public static void Test1()
        {
            List<Person> persons = new List<Person>
            {
                new Person{ Id = 1,Name="夫子",Sex="男",BirthDay=new DateTime(1999,10,11) },
                new Person{ Id = 2,Name="余帘",Sex="女",BirthDay=new DateTime(1999,12,12) },
                new Person{ Id = 3,Name="李慢慢",Sex="男",BirthDay=new DateTime(1999,11,11) },
                new Person{ Id = 4,Name="叶红鱼",Sex="女",BirthDay=new DateTime(1999,10,10) }
            };

            //声明mapper操作对象
            var mapper = new Mapper();

            //第一个参数表示导出的列名，第二个表示对应的属性字段
            mapper.Map<Person>("姓名", s => s.Name)
                .Map<Person>("学号", s => s.Id)
                .Map<Person>("性别", s => s.Sex)
                .Map<Person>("生日", s => s.BirthDay)
                //格式化操作，第一个参数表示格式，第二表示对应字段
                //Format不仅仅只支持时间操作，还可以是数字或金额等
                .Format<Person>("yyyy-MM-dd", s => s.BirthDay);

            //第一个参数为导出Excel名称
            //第二个参数为Excel数据来源
            //第三个参数为导出的Sheet名称
            //overwrite参数如果是要覆盖已存在的Excel或者新建Excel则为true，如果在原有Excel上追加数据则为false
            //xlsx参数是用于区分导出的数据格式为xlsx还是xls
            mapper.Save($"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "persons.xlsx")}", persons, "sheet1", overwrite: true, xlsx: true);
        }

        public static void Test2()
        {
            List<Person> persons = new List<Person>
            {
                new Person{ Id = 1,Name="夫子",Sex="男",BirthDay=new DateTime(1999,10,11) },
                new Person{ Id = 2,Name="余帘",Sex="女",BirthDay=new DateTime(1999,12,12) },
                new Person{ Id = 3,Name="李慢慢",Sex="男",BirthDay=new DateTime(1999,11,11) },
                new Person{ Id = 4,Name="叶红鱼",Sex="女",BirthDay=new DateTime(1999,10,10) }
            };

            //声明mapper操作对象
            var mapper = new Mapper();

            mapper.Map<Person>("姓名", s => s.Name)
                .Map<Person>("学号", s => s.Id)
                .Map<Person>("性别", s => s.Sex)
                .Map<Person>("生日", s => s.BirthDay)
                //格式化操作，第一个参数表示格式，第二表示对应字段
                //Format不仅仅只支持时间操作，还可以是数字或金额等
                .Format<Person>("yyyy-MM-dd", s => s.BirthDay);

            //放入Mapper中
            //第一个参数是数据集合，第二个参数是Sheet名称，第三个参数表示是追加数据还是覆盖数据
            mapper.Put<Person>(persons, "person1", true);
            mapper.Put<Person>(persons, "person2", true);

            mapper.Save($"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "persons1.xlsx")}");
        }

        public static void Test3()
        {
            //List<Person> persons = new List<Person>
            //{
            //    new Person{ Id = 1,Name="夫子",Sex="男",BirthDay=new DateTime(1999,10,11) },
            //    new Person{ Id = 2,Name="余帘",Sex="女",BirthDay=new DateTime(1999,12,12) },
            //    new Person{ Id = 3,Name="李慢慢",Sex="男",BirthDay=new DateTime(1999,11,11) },
            //    new Person{ Id = 4,Name="叶红鱼",Sex="女",BirthDay=new DateTime(1999,10,10) }
            //};

            //var mapper = new Mapper();
            //MemoryStream stream = new MemoryStream();
            ////将students集合生成的Excel直接放置到Stream中
            //mapper.Save(stream, persons, "sheet1", overwrite: true, xlsx: true);
            //return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Student.xlsx");
        }

        public static void Test4()
        {
            //Excel文件的路径
            var mapper = new Mapper("persons.xlsx");
            //读取的sheet信息
            var personRows = mapper.Take<Person>("sheet1");
            foreach (var row in personRows)
            {
                //映射的数据保留在value中
                Person student = row.Value;
                Console.WriteLine($"姓名:[{student.Name}],学号:[{student.Id}],性别:[{student.Sex}],生日:[{student.BirthDay:yyyy-MM-dd}]");
            }
        }

        public static void Test5()
        {
            //[HttpPost]
            //public IEnumerable<Person> UploadFile(IFormFile formFile)
            //{
            //    //通过上传文件流初始化Mapper
            //    var mapper = new Mapper(formFile.OpenReadStream());
            //    //读取sheet1的数据
            //    return mapper.Take<Person>("sheet1").Select(i => i.Value);
            //}
        }
    }
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
