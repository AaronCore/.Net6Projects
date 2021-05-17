using System;
using System.Collections.Generic;
using System.Text;
using SysEntity;

namespace SysApplication.Person
{
    public interface IPersonService
    {
        void Add(PersonEntity entity);
        List<PersonEntity> GetPersons();
    }
}
