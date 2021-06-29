using System;
using System.Collections.Generic;
using System.Text;
using SysEntity;
using SysEntityFrameworkCore;
using SysRepository;

namespace SysApplication.Person
{
    public class PersonService : IPersonService
    {
        private readonly IBaseRepository<PersonEntity> _personRepository;
        public PersonService(IBaseRepository<PersonEntity> personRepository)
        {
            _personRepository = personRepository;
        }
        public void Add(PersonEntity entity)
        {
            _personRepository.Insert(entity);
            _personRepository.Commit();
        }
        public List<PersonEntity> GetPersons()
        {
            return _personRepository.GetAll();
        }
    }
}
