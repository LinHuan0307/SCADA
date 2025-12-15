using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Sevice
{
    public class RunConfigService
    {
        public RunConfig m_RunConfig { get; set; }

        private RuntimeContextService m_RuntimeContextService;

        public RunConfigService(
            RunConfigHelper runConfigHelper,
            RuntimeContextService runtimeContextService)
        {
            m_RunConfig = runConfigHelper.RunConfig;
            m_RuntimeContextService = runtimeContextService;
        }

        /// <summary>
        /// 减去最新批次的一个物料
        /// </summary>
        /// <returns></returns>
        public Result UseMaterial(string name)
        {
            
            var 最新批次 = m_RunConfig.批次码绑定列表.Where(批次码绑定列表项 => 批次码绑定列表项.批次物料名 == name).FirstOrDefault();

            if (最新批次.已使用 >= 0 && 最新批次.已使用 < 最新批次.物料总数)
            {
                最新批次.已使用++;
                m_RuntimeContextService.删除物料缺失报警();
            }
            else
            {
                m_RuntimeContextService.添加物料缺失报警();
                return Result.Fail($"{name}出现意料外的使用数量！请检查已使用物料数量！");
            }

            if (最新批次.已使用 == 最新批次.物料总数)
            {
                //删除物料用完的批次码
                m_RunConfig.批次码绑定列表.Remove(最新批次);
            }
                

            return Result.Ok();
            
        }
        /// <summary>
        /// 增加一个批次
        /// </summary>
        /// <param name="BatchCode"></param>
        /// <param name="BatchNum"></param>
        /// <returns></returns>
        public Result AddBatch(string MatirialName, string BatchCode, int BatchNum)
        {
            try
            {
                批次码列表项 批次码列表项 = new 批次码列表项()
                {
                    批次物料名 = MatirialName,
                    已使用 = 0,
                    批次码 = BatchCode,
                    物料总数 = BatchNum

                };
                m_RunConfig.批次码绑定列表.Add(批次码列表项);
 
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public Result<批次码列表项> GetLastBatch(string name)
        {
            var res = m_RunConfig.批次码绑定列表.Where(批次码绑定列表项 => 批次码绑定列表项.批次物料名 == name).FirstOrDefault();

            if(res == null)
            {
                return Result.Fail("可用于扣料的批次码为空");
            }
            else
            {
                return Result.Ok();
            }
        }

        /// <summary>
        /// 是否有物料可以绑定
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Result IsMatirialAvailableForBinding(string name)
        {
            var 已使用总数量 = m_RunConfig.批次码绑定列表.Where(批次码绑定列表项 => 批次码绑定列表项.批次物料名 == name)
                                                        .Select(批次码绑定列表项 => 批次码绑定列表项.已使用).Count();

            var 物料总数 = m_RunConfig.批次码绑定列表.Where(批次码绑定列表项 => 批次码绑定列表项.批次物料名 == name)
                                                        .Select(批次码绑定列表项 => 批次码绑定列表项.物料总数).Count();


            return 物料总数 > 已使用总数量 ? Result.Ok() : Result.Fail("物料不足");
        }
    }
}
