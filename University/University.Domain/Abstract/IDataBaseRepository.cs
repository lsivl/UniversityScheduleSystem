using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Domain.Models;

namespace University.Domain.Abstract
{
    public interface IDataBaseRepository
    {
        UniversityContext DataBase { get; }
    }
}
