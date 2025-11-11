
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GaoYaXianShu.Helper
{
    public class RunConfigHelper
    {
        public RunConfig RunConfig;
        private readonly string RunConfig_configFilePath = @"Config\RunConfig.xml";
        public RuntimeContext RuntimeContext;
        private readonly string RuntimeContext_configFilePath = @"Config\RuntimeContextConfig.xml";

        public RunConfigHelper()
        {
            加载系统配置文件();
        }

        public void 加载系统配置文件()
        {
            var 加载系统配置文件 = LoadConfiguration<RunConfig>(RunConfig_configFilePath);
            if (加载系统配置文件.IsFailed)
            {
                UIMessageBox.ShowError("配置信息读取异常" + string.Join("|", 加载系统配置文件.Errors));
                return;
            }
            RunConfig = 加载系统配置文件.Value;
        }

        public void 保存系统配置文件()
        {
            var 保存系统配置文件反馈 = SaveConfiguration<RunConfig>(RunConfig);
            if (保存系统配置文件反馈.IsFailed)
            {
                UIMessageBox.ShowError("配置信息读取异常" + string.Join("|", 保存系统配置文件反馈.Errors));
                return;
            }
            else
            {
                UIMessageTip.ShowOk("配置文件保存成功");
            }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private  Result<T> LoadConfiguration<T>(string configFilePath)
        {
            try
            {
                if (!File.Exists(configFilePath))
                {
                    return Result.Fail($"根据路径{configFilePath}找不到文件");
                }
                T runConfig;
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (FileStream fs = new FileStream(configFilePath, FileMode.Open))
                {
                    runConfig = (T)serializer.Deserialize(fs);
                }
                return Result.Ok<T>(runConfig);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
            
        }


        /// <summary>
        /// 保存配置文件（重载版本，接受配置对象参数）
        /// </summary>
        private Result SaveConfiguration<T>(T t)
        {
            try
            {
                // 确保目录存在
                string directory = Path.GetDirectoryName(RunConfig_configFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (FileStream fs = new FileStream(RunConfig_configFilePath, FileMode.Create))
                {
                    serializer.Serialize(fs, t);
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}
