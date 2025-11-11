using Autofac;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.RunLogic
{
    public class RunLogicManeger
    {
        private PLCService m_PLCService;
        private RunConfig m_RunConfig;
        private UIManeger m_UIManeger;
        private RuntimeContextService m_RuntimeContextService;
        private IComponentContext m_componentContext;

        public RunLogicManeger(
            IComponentContext componentContext,
            RunConfigHelper runConfigHelper,
            RuntimeContextService runtimeContextService,
            UIManeger uIManeger,
            PLCService pLCService)
        {
            //注入依赖
            m_componentContext = componentContext;
            m_PLCService = pLCService;
            m_RunConfig = runConfigHelper.RunConfig;
            m_UIManeger = uIManeger;
            m_RuntimeContextService = runtimeContextService;
            根据配置文件添加运行流程();
        }
        /// <summary>
        /// 处理流程字
        /// </summary>
        /// <param></param>
        public async Task HandleAutoFlowNum()
        {

            //获取最新流程字
            var 获取流程字反馈 = await m_PLCService.Get流程字();
            if (获取流程字反馈.IsFailed)
            {
                return;
            }
            m_UIManeger.Set_Tb_AutoFlow(获取流程字反馈.Value.ToString());
            //流程字满足允许执行。允许执行标志位根据具体的流程自行决定是否禁止执行。
            //需要在同一个流程号多次执行的流程，例如采集曲线数据，可以在流程内继续允许执行
            //在一个流程号周期内只需要执行一次的流程，例如进出站，可以在流程内禁止允许执行。
            foreach (var runlogic in 
                m_RuntimeContextService.获取满足流程号的允许可执行的流程(获取流程字反馈.Value))
            {
                await runlogic.RunLogicAsync();
            }
        }

        /// <summary>
        /// 再次处理流程字
        /// </summary>
        /// <param></param>
        public async Task ReHandleAutoFlowNum()
        {
            var 获取流程字反馈 = await m_PLCService.Get流程字();
            if (获取流程字反馈.IsFailed)
            {
                return;
            }

            //不需要判断运行执行标志位
            foreach (var runlogic in
                m_RuntimeContextService.获取满足流程号的流程(获取流程字反馈.Value))
            {
                await runlogic.RunLogicAsync();
            }
        }

        private void 根据配置文件添加运行流程()
        {
            foreach (var 运行逻辑配方实体类 in m_RunConfig.流程配置列表)
            {
                //根据配置信息设置流程并添加到观察者列表中
                
                var runlogic = (IRunLogic)m_componentContext.ResolveKeyed<IRunLogic>(运行逻辑配方实体类.流程字对应操作);
                runlogic.目标流程字 = 运行逻辑配方实体类.目标流程字;
                m_RuntimeContextService.获取全部流程().Add(runlogic);
            }
        }

    }
}
