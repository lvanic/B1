using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B1.DataLayer.Repository
{
    public interface IGenericRepository<T>
    {
        public T GetById(int id);
        public T GetById(string id);
        public List<T> GetAll();
        public bool Add(T item);
        public bool AddRange(IEnumerable<T> range);
        public bool Update(T item);
        public bool Delete(T item);
        public void CallStoredProcedure(string procedureName, params object[] parameters);
    }
}
