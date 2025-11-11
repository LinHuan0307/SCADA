using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BydDCS.DB
{
    public class LogContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<LogContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        public LogContext() : base("LogContext") { } //配置使用的连接名

        //public DbSet<LogInfo> LogSet { get; set; }
    }
}
