using HslCommunication;
using Microsoft.EntityFrameworkCore;
using Sunny.UI.Win32;
using System.Reflection.Emit;

namespace WinFormsApp1.Entity
{
    public class AppDbContext : DbContext
    {
        public DbSet<ProductInfo> ProductInfos { get; set; }
        public DbSet<PassStationInfo> PassStationInfos { get; set; }

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("data source=d:\\Data\\DCSDb.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<ProductInfo>(entity =>
            {
                entity.ToTable("ProductInfo");
                entity.HasKey(e => e.Id);  // 设置主键
                // SQLite 自增列：Id 默认自增（需要配置为自动增长）
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // 配置字段长度（可选）
                entity.Property(e => e.Sn).HasMaxLength(50);
                entity.Property(e => e.TrayNo).HasMaxLength(50);
                entity.Property(e => e.TestResult).HasMaxLength(20);
                entity.Property(e => e.Remark).HasMaxLength(200);
                entity.Property(e => e.TestValue1).HasMaxLength(100);
                entity.Property(e => e.TestValue2).HasMaxLength(100);
                entity.Property(e => e.TestValue3).HasMaxLength(100);
                entity.Property(e => e.TestValue4).HasMaxLength(100);
                entity.Property(e => e.TestValue5).HasMaxLength(100);

                // 设置默认值（SQLite 不支持 GETDATE()，使用 CURRENT_TIMESTAMP）
                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<PassStationInfo>(entity =>
            {
                entity.ToTable("PassStationInfo");
                entity.HasKey(e => e.Id);  // 设置主键
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Sn).HasMaxLength(50);
                entity.Property(e => e.TrayNo).HasMaxLength(50);
                entity.Property(e => e.LineCode).HasMaxLength(20);
                entity.Property(e => e.StationCode).HasMaxLength(20);
                entity.Property(e => e.PassType).HasMaxLength(20);
                entity.Property(e => e.Remark).HasMaxLength(200);

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }


        #region ProductInfos 表
        /// <summary>
        /// 插入一条记录。
        /// </summary>
        /// <param name="pAlarmInfo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public OperateResult AddProductInfo(ProductInfo pInfo)
        {
            try
            {
                this.ProductInfos.Add(pInfo);
                this.SaveChanges();
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return OperateResult.CreateFailedResult<string>(new OperateResult(ex.Message));
            }


        }


        /// <summary>
        /// 获取指定Sn的记录。
        /// </summary>
        /// <param name="pSn"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public OperateResult<List<ProductInfo>> GetProductInfoBySn(string pSn)
        {
            try
            {
                List<ProductInfo> pInfos = null;

                pInfos = this.ProductInfos.Where(obj => obj.Sn.ToLower() == pSn.ToLower()).ToList();

                return OperateResult.CreateSuccessResult<List<ProductInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return OperateResult.CreateFailedResult<List<ProductInfo>>(new OperateResult(ex.Message));
            }


        }

        /// <summary>
        /// 获取指定TrayNo的记录。
        /// </summary>
        /// <param name="pSn"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public OperateResult<List<ProductInfo>> GetProductInfoByTrayNo(string trayNo)
        {
            try
            {
                List<ProductInfo> pInfos = null;

                pInfos = this.ProductInfos.Where(obj => obj.TrayNo.ToLower() == trayNo.ToLower()).ToList();

                return OperateResult.CreateSuccessResult<List<ProductInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return OperateResult.CreateFailedResult<List<ProductInfo>>(new OperateResult(ex.Message));
            }


        }

        /// <summary>
        /// 获取指定时间间隔内的全部记录。
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <param name="pAlarmInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public OperateResult<List<ProductInfo>> GetProductInfoInterval(DateTime pStart, DateTime pEnd)
        {
            try
            {
                List<ProductInfo> pInfos = null;

                pInfos = this.ProductInfos.Where(obj => (obj.CreateTime >= pStart && obj.CreateTime <= pEnd)).ToList();

                return OperateResult.CreateSuccessResult<List<ProductInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return OperateResult.CreateFailedResult<List<ProductInfo>>(new OperateResult(ex.Message));
            }


        }

        #endregion

        #region PassStationInfos 表
        /// <summary>
        /// 插入一条记录。
        /// </summary>
        /// <param name="pAlarmInfo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public OperateResult AddPassStationInfo(PassStationInfo pInfo)
        {
            try
            {
                this.PassStationInfos.Add(pInfo);
                this.SaveChanges();
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return OperateResult.CreateFailedResult<string>(new OperateResult(ex.Message));
            }


        }


        /// <summary>
        /// 获取指定Sn的记录。
        /// </summary>
        /// <param name="pSn"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public OperateResult<List<PassStationInfo>> GetPassStationInfoBySn(string pSn)
        {
            try
            {
                List<PassStationInfo> pInfos = null;

                pInfos = this.PassStationInfos.Where(obj => obj.Sn.ToLower() == pSn.ToLower()).ToList();

                return OperateResult.CreateSuccessResult<List<PassStationInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return OperateResult.CreateFailedResult<List<PassStationInfo>>(new OperateResult(ex.Message));
            }


        }

        /// <summary>
        /// 获取指定TrayNo的记录。
        /// </summary>
        /// <param name="pSn"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public OperateResult<List<PassStationInfo>> GetPassStationInfoByTrayNo(string trayNo)
        {
            try
            {
                List<PassStationInfo> pInfos = null;

                pInfos = this.PassStationInfos.Where(obj => obj.TrayNo.ToLower() == trayNo.ToLower()).ToList();

                return OperateResult.CreateSuccessResult<List<PassStationInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return OperateResult.CreateFailedResult<List<PassStationInfo>>(new OperateResult(ex.Message));
            }


        }

        /// <summary>
        /// 获取指定时间间隔内的全部记录。
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <param name="pAlarmInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public OperateResult<List<PassStationInfo>> GetPassStationInfoInterval(DateTime pStart, DateTime pEnd)
        {
            try
            {
                List<PassStationInfo> pInfos = null;

                pInfos = this.PassStationInfos.Where(obj => (obj.CreateTime >= pStart && obj.CreateTime <= pEnd)).ToList();

                return OperateResult.CreateSuccessResult<List<PassStationInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return OperateResult.CreateFailedResult<List<PassStationInfo>>(new OperateResult(ex.Message));
            }


        }

        #endregion
    }
}