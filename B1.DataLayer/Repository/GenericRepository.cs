using B1.DataLayer.Data;
using B1.DataLayer.Models;
using B1.DataLayer.Settings;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace B1.DataLayer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly AppDbContext _context;
        private readonly SettingsSingleton _settingsSingleton;
        private readonly object _lockObject = new object();
        public GenericRepository(string connectionString)
        {
            _settingsSingleton = SettingsSingleton.GetInstance(connectionString) ?? throw new ArgumentNullException();
            _context = new AppDbContext(_settingsSingleton.GetDbOptions()) ?? throw new ArgumentNullException();

        }
        public bool Add(T item)
        {
            lock (_lockObject)
            {
                _context.Set<T>().Add(item);
                _context.SaveChanges();
                return true;
            }
        }
        public bool AddRange(IEnumerable<T> range)
        {
            lock (_lockObject)
            {
                _context.Set<T>().AddRange(range);
                _context.SaveChanges();
                return true;
            }
        }

        public bool Delete(T item)
        {
            lock (_lockObject)
            {
                _context.Set<T>().Remove(item);
                _context.SaveChanges();
                return true;
            }
        }

        public List<T> GetAll()
        {
            lock (_lockObject)
            {
                return _context.Set<T>().ToList();
            }
        }

        public T GetById(int id)
        {
            lock (_lockObject)
            {
                return _context.Set<T>().Find(id);
            }
        }

        public T GetById(string id)
        {
            lock (_lockObject)
            {
                return _context.Set<T>().Find(id);
            }
        }

        public bool Update(T item)
        {
            lock (_lockObject)
            {
                _context.Set<T>().Update(item);
                _context.SaveChanges();
                return true;
            }
        }
        public void CallStoredProcedure(string procedureName, params object[] parameters)
        {
            _context.Database.ExecuteSqlRaw($"EXEC {procedureName} {GetSqlParameters(parameters)}");
        }
        private string GetSqlParameters(params object[] parameters)
        {
            var sqlParams = string.Join(", ", parameters.Select(p => p.ToString()));
            return sqlParams;
        }
    }
}
