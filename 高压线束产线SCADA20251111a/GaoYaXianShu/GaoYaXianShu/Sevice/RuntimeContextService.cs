using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.RunLogic;
using GaoYaXianShu.UIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Sevice
{
    public class RuntimeContextService
    {
        private RuntimeContext m_RuntimeContext;
        private UIManeger m_UIManeger;

        public RuntimeContextService(
            UIManeger uIManeger)
        {
            m_RuntimeContext = new RuntimeContext();
            m_UIManeger = uIManeger;
        }

        public void 重置流程执行标志位()
        {
            foreach (var 运行逻辑对象 in m_RuntimeContext.运行逻辑对象列表)
            {
                运行逻辑对象.允许执行标志位 = true;
            }
        }

        public List<IRunLogic> 获取满足流程号的允许可执行的流程(short autoflow)
        {
            return m_RuntimeContext.运行逻辑对象列表.Where(obj => obj.目标流程字 == autoflow && obj.允许执行标志位).ToList();
        }

        public List<IRunLogic> 获取满足流程号的流程(short autoflow)
        {
            return m_RuntimeContext.运行逻辑对象列表.Where(obj => obj.目标流程字 == autoflow).ToList();
        }

        public List<IRunLogic> 获取全部流程()
        {
            return m_RuntimeContext.运行逻辑对象列表;
        }


    }
}
