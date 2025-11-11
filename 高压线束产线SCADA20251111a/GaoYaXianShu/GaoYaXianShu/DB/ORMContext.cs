using FluentResults;
using GaoYaXianShu.Entity;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BydDCS.DB
{
    public class ORMContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<ORMContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        public ORMContext() : base("ORMContext") { } //配置使用的连接名

        /// <summary>
        /// 产品信息。
        /// </summary>
        public DbSet<ProductInfo> ProductSet { get; set; }

        public DbSet<AlarmInfo> AlarmSet { get; set; }

        public DbSet<StateInfo> StateSet { get; set; }
        /// <summary>
        /// 过站信息。
        /// </summary>
        public DbSet<TransitInformation> TransitSet { get; set; }
        /// <summary>
        /// 物料绑定信息
        /// </summary>
        public DbSet<MaterialInfo> MaterialSet { get; set; }
        /// <summary>
        /// 测试数据信息
        /// </summary>
        public DbSet<TestData> TestDataSet { get; set; }

        public Result Exist()
        {
            string err = string.Empty;
            string filename = base.Database.Connection.ConnectionString.Replace("data source=", "");
            if (!System.IO.File.Exists(filename)) 
            {
                err = "数据库文件:" + filename + "不存在.";
                return Result.Fail(err);
            }
            return Result.Ok();
            
        }
    }
}
