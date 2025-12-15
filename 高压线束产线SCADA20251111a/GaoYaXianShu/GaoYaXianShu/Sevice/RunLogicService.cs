using Autofac;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.RunLogic
{
    public class RunLogicService
    {
        private PLCService m_PLCService;
        private RunConfig m_RunConfig;
        private RuntimeContextService m_RuntimeContextService;
        private IComponentContext m_componentContext;

        public RunLogicService(
            IComponentContext componentContext,
            RunConfigHelper runConfigHelper,
            RuntimeContextService runtimeContextService,
            PLCService pLCService)
        {
            //注入依赖
            m_componentContext = componentContext;
            m_PLCService = pLCService;
            m_RunConfig = runConfigHelper.RunConfig;
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
            m_RuntimeContextService.设置流程号(获取流程字反馈.Value);
            
            //运行时根据流程号执行对应的流程
            foreach (var runlogic in 
                m_RuntimeContextService.获取流程号对应的且允许可执行的流程(获取流程字反馈.Value))
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
            foreach (var 运行逻辑配方 in m_RunConfig.流程配置列表)
            {
                //初始化时，根据配方添加流程到观察者列表中
                
                var runlogic = (IRunLogic)m_componentContext.ResolveKeyed<IRunLogic>(运行逻辑配方.流程字对应操作);
                runlogic.目标流程字 = 运行逻辑配方.目标流程字;
                runlogic.允许执行标志位 = true;
                m_RuntimeContextService.添加运行逻辑(runlogic);
            }
        }

    }
}
