
using BydDCS.DB;
using FluentResults;
using GaoYaXianShu.Entity;

using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.Design.WebControls;


namespace BydDCS.Helper
{
    /// <summary>
    /// 用于描述对本地数据的操作。
    /// </summary>
    public class LocalDbDAL
    {
        public ORMContext m_Context;

        /// <summary>
        /// 初始化与本地数据库的连接。
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result Init()
        {
            try
            {
                m_Context = new ORMContext();
                return Result.Ok();
            }
            catch(Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }

        #region AlarmInfos 表
        /// <summary>
        /// 插入一条记录。
        /// </summary>
        /// <param name="pInfo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result AddAlarmInfo(AlarmInfo pInfo)
        {
            try
            {
                m_Context.AlarmSet.Add(pInfo);
                m_Context.SaveChanges();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

            
        }

        /// <summary>
        /// 获取未上传到服务器的全部记录。
        /// </summary>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<AlarmInfo>> GetAlarmInfoNoUpload()
        {
            try
            {
                List<AlarmInfo> pInfos = null;

                pInfos = m_Context.AlarmSet.Where(obj => obj.UploadServer == false).ToList();

                return Result.Ok<List<AlarmInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }

        /// <summary>
        /// 获取指定时间间隔内的全部记录。
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<AlarmInfo>> GetAlarmInfoInterval(DateTime pStart, DateTime pEnd)
        {
            try
            {
                List<AlarmInfo> pInfos = null;

                pInfos = m_Context.AlarmSet.Where(obj => (obj.CreateTime >= pStart && obj.CreateTime <= pEnd)).ToList();

                return Result.Ok<List<AlarmInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

            
        }
        #endregion

        #region StateInfo 表
        /// <summary>
        /// 插入一条记录。
        /// </summary>
        /// <param name="pInfo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result AddStateInfo(StateInfo pInfo)
        {
            try
            {
                m_Context.StateSet.Add(pInfo);
                m_Context.SaveChanges();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

            
        }

        /// <summary>
        /// 获取未上传到服务器的全部记录。
        /// </summary>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<StateInfo>> GetStateInfoNoUpload()
        {
            try
            {
                List<StateInfo> pInfos = null;

                pInfos = m_Context.StateSet.Where(obj => obj.UploadServer == false).ToList();

                return Result.Ok<List<StateInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

            
        }

        /// <summary>
        /// 获取指定时间间隔内的全部记录。
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<StateInfo>> GetStateInfoInterval(DateTime pStart, DateTime pEnd)
        {
            try
            {
                List<StateInfo> pInfos = null;

                pInfos = m_Context.StateSet.Where(obj => (obj.CreateTime >= pStart && obj.CreateTime <= pEnd)).ToList();

                return Result.Ok<List<StateInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

            
        }
        #endregion

        #region 公共
        /// <summary>
        /// 保存更改。
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result SaveChanges()
        {
            try
            {
                m_Context.SaveChanges();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }
        #endregion

        #region ProductInfos 表
        /// <summary>
        /// 插入一条记录。
        /// </summary>
        /// <param name="pAlarmInfo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result AddProductInfo(ProductInfo pInfo)
        {
            try
            {
                m_Context.ProductSet.Add(pInfo);
                m_Context.SaveChanges();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

            
        }

        /// <summary>
        /// 获取未上传到服务器的全部记录。
        /// </summary>
        /// <param name="pAlarmInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<ProductInfo>> GetProductInfoNoUpload()
        {
            try
            {
                List<ProductInfo> pInfos = null;
                pInfos = m_Context.ProductSet.Where(obj => !obj.UploadServer).ToList();
                return Result.Ok<List<ProductInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

           
        }

        /// <summary>
        /// 获取指定Sn的记录。
        /// </summary>
        /// <param name="pSn"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<ProductInfo>> GetProductInfoBySn(string pSn)
        {
            try
            {
                List<ProductInfo> pInfos = null;

                pInfos = m_Context.ProductSet.Where(obj => obj.Sn.ToLower() == pSn.ToLower()).ToList();

                return Result.Ok<List<ProductInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
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
        public  Result<List<ProductInfo>> GetProductInfoInterval(DateTime pStart, DateTime pEnd)
        {
            try
            {
                List<ProductInfo> pInfos = null;

                pInfos = m_Context.ProductSet.Where(obj => (obj.CreateTime >= pStart && obj.CreateTime <= pEnd)).ToList();

                return Result.Ok<List<ProductInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

            
        }

        #endregion

        #region TransitInformation 表
        /// <summary>
        /// 插入一条记录。
        /// </summary>
        /// <param name="pAlarmInfo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result AddTransitInformation(TransitInformation pInfo)
        {
            try
            {
                pInfo.EndTime = DateTime.Now;

                m_Context.TransitSet.Add(pInfo);
                m_Context.SaveChanges();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }

        

        /// <summary>
        /// 获取指定Sn的记录。
        /// </summary>
        /// <param name="pSn"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<TransitInformation>> GetTransitInformationBySn(string pSn)
        {
            try
            {
                List<TransitInformation> pInfos = null;

                if (string.IsNullOrEmpty(pSn))
                {
                    return Result.Fail("产品SN和托盘条码都为空!");
                }

                pInfos = m_Context.TransitSet.Where(obj => obj.Sn.ToLower() == pSn.ToLower()).ToList();

                return Result.Ok<List<TransitInformation>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
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
        public  Result<List<TransitInformation>> GetTransitInformationInterval(DateTime pStart, DateTime pEnd)
        {
            try
            {
                List<TransitInformation> pInfos = null;
                pInfos = m_Context.TransitSet.Where(obj => (obj.StartTime >= pStart && obj.StartTime <= pEnd)).ToList();
                return Result.Ok<List<TransitInformation>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
            
            
        }

        
        #endregion

        #region MaterialInfo表
        /// <summary>
        /// 插入一条记录。
        /// </summary>
        /// <param name="pAlarmInfo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result AddMaterialInfo(MaterialInfo pInfo)
        {
            try
            {
                m_Context.MaterialSet.Add(pInfo);
                m_Context.SaveChanges();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
            
        }


        /// <summary>
        /// 获取指定Sn的记录。
        /// </summary>
        /// <param name="pSn"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<MaterialInfo>> GetMaterialInfoBySn(string pSn)
        {
            try
            {
                List<MaterialInfo> pInfos = null;
                pInfos = m_Context.MaterialSet.Where(obj => obj.SN.ToLower() == pSn.ToLower())
                       .Include(m => m.CodeList).ToList();
                return Result.Ok<List<MaterialInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
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
        public  Result<List<MaterialInfo>> GetMaterialInfoInterval(DateTime pStart, DateTime pEnd)
        {
            try
            {
                List<MaterialInfo> pInfos = null;
                pInfos = m_Context.MaterialSet.Where(obj => (obj.CreateTime >= pStart && obj.CreateTime <= pEnd))
                        .Include(m => m.CodeList).ToList();
                return Result.Ok<List<MaterialInfo>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }  
        }

        #endregion

        #region TestDataInfo表
        /// <summary>
        /// 插入一条记录。
        /// </summary>
        /// <param name="pAlarmInfo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result AddTestDataInfo(TestData pInfo)
        {
            try
            {
                m_Context.TestDataSet.Add(pInfo);
                m_Context.SaveChanges();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }


        /// <summary>
        /// 获取指定Sn的记录。
        /// </summary>
        /// <param name="pSn"></param>
        /// <param name="pInfos"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public  Result<List<TestData>> GetTestDataBySn(string pSn)
        {
            try
            {
                List<TestData> pInfos = null;
                pInfos = m_Context.TestDataSet.Where(obj => obj.SnNumber.ToLower() == pSn.ToLower()).ToList();
                return Result.Ok<List<TestData>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
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
        public  Result<List<TestData>> GetTestDataInfoInterval(DateTime pStart, DateTime pEnd)
        {
            try
            {
                List<TestData> pInfos = null;
                pInfos = m_Context.TestDataSet.Where(obj => (obj.CreateTime >= pStart && obj.CreateTime <= pEnd)).ToList();
                return Result.Ok<List<TestData>>(pInfos);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        #endregion
    }
}
