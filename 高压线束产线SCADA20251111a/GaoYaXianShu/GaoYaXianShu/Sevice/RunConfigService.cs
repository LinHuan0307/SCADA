using Autofac;
using DianJiaoJi.Helper;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GaoYaXianShu.Sevice
{
    public class RunConfigService
    {
        private readonly IComponentContext m_componentContext;
        private readonly RunConfig m_RunConfig;
        private readonly XmlConfigManager<RunConfig> m_XmlConfigManager;

        public RunConfigService(
            IComponentContext   componentContext,
            RunConfig           runConfig,
            XmlConfigManager<RunConfig> xmlConfigManager)
        {
            m_componentContext = componentContext;
            m_RunConfig = runConfig;
            m_XmlConfigManager = xmlConfigManager;
        }

        public void 保存系统配置文件()
        {
            m_XmlConfigManager.Save(m_RunConfig);
        }
    }
}
