using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Domain.Abstract;
using University.Domain.Models;

namespace University.Domain.Repositries
{
    public class DataBaseRepository : IDataBaseRepository
    {
        UniversityContext Data = new UniversityContext();

        public UniversityContext DataBase
        {
            get { return Data; }
        }
    }
}
