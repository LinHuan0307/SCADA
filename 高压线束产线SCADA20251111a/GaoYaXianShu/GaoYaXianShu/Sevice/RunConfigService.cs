using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.UIService;
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

        private UIManeger m_UIManeger;

        public RunConfigService(
            RunConfigHelper runConfigHelper,
            UIManeger uIManeger)
        {
            m_RunConfig = runConfigHelper.RunConfig;
            m_UIManeger = uIManeger;
        }

        /// <summary>
        /// 减去最新批次的一个物料
        /// </summary>
        /// <returns></returns>
        public Result UseMaterial(string name)
        {
            try
            {
                var 最新批次 = m_RunConfig.批次码绑定列表.Where(批次码绑定列表项 => 批次码绑定列表项.批次物料名 == name).FirstOrDefault();

                if (最新批次.已使用 >= 0 && 最新批次.已使用 < 最新批次.物料总数)
                {
                    最新批次.已使用++;
                    m_UIManeger.DeleteAlarmInfo("物料缺失报警!");
                }
                else
                {
                    m_UIManeger.UnValidMetirialNumInputLog();
                    return Result.Fail($"{name}出现意料外的使用数量！请检查已使用物料数量！");
                }

                if (最新批次.已使用 == 最新批次.物料总数)
                {
                    //删除物料用完的批次码
                    m_RunConfig.批次码绑定列表.Remove(最新批次);
                }
                m_UIManeger.ReFresh_Dgv();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
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

                m_UIManeger.ReFresh_Dgv();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public Result<批次码列表项> GetLastBatch(string name)
        {
            return m_RunConfig.批次码绑定列表.Where(批次码绑定列表项 => 批次码绑定列表项.批次物料名 == name).FirstOrDefault();
        }

        /// <summary>
        /// 这里判断是否有两个物料。左右各一个。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Result<bool> IsMatirialAvailableForBinding(string name)
        {
            var 已使用总数量 = m_RunConfig.批次码绑定列表.Where(批次码绑定列表项 => 批次码绑定列表项.批次物料名 == name)
                                                        .Select(批次码绑定列表项 => 批次码绑定列表项.已使用).Count();

            var 物料总数 = m_RunConfig.批次码绑定列表.Where(批次码绑定列表项 => 批次码绑定列表项.批次物料名 == name)
                                                        .Select(批次码绑定列表项 => 批次码绑定列表项.物料总数).Count();


            return 物料总数 >= 已使用总数量 + 2;
        }
    }
}
