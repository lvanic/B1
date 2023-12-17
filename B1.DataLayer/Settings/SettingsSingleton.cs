using B1.DataLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace B1.DataLayer.Settings
{
    internal class SettingsSingleton
    {
        private static readonly object lockObject = new object();
        private static SettingsSingleton? instance;
        private string ConnectionString { get; set; }

        public static SettingsSingleton GetInstance(string connectionString)
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new SettingsSingleton
                        {
                            ConnectionString = connectionString
                        };
                    }
                }
            }
            return instance;
        }
        public DbContextOptions<AppDbContext> GetDbOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(instance?.ConnectionString ?? throw new InvalidOperationException("ConnectionString is not set."));
            return optionsBuilder.Options;
        }
    }
}
