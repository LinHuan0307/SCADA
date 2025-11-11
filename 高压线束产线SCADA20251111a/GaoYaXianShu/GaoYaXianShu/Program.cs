using Autofac;
using BydDCS.Helper;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.m_Form;
using GaoYaXianShu.RunLogic;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using NLog;
using System;
using System.IO.Ports;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace GaoYaXianShu
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex fmutex = new Mutex(true, "GaoYaXianShuPiCiMaSaoMa");
            if (!fmutex.WaitOne(0, false))
            {
                MessageBox.Show("程序已在运行中。");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ContainerBuilder Build = new ContainerBuilder();
            //LogManager.LoadConfiguration(@"Config/NLog.config");
            LogManager.Setup().LoadConfigurationFromFile(@"Config/NLog.config");
            //注册Nlog
            Build.Register(t =>
            {
                var type = t.GetType(); // 获取请求服务的类型
                return LogManager.GetLogger(type.FullName);
            }).As<ILogger>();
            
            //注册UI控件操作服务
            Build.RegisterType<UIManeger>().InstancePerLifetimeScope(); 
            //注册配置服务
            Build.RegisterType<RunConfigHelper>().InstancePerLifetimeScope(); 
            //注册PLC读写服务
            Build.RegisterType<PLCService>().InstancePerLifetimeScope();
            //注册TCP客户端驱动
            Build.RegisterType<TCPClientHelper>().InstancePerLifetimeScope();
            //注册TCP客户端服务
            Build.RegisterType<TCPClientService>().InstancePerLifetimeScope();
            //注册hsl欧姆龙读写驱动
            Build.RegisterType<HslAsyncOmronUdpHelper>().InstancePerLifetimeScope();
            //注册MESAPIService
            Build.RegisterType<MesApiService>().InstancePerLifetimeScope();
            //注册复位流程
            Build.RegisterType<ResetAutoFlow>().Keyed<IRunLogic>("重置流程").InstancePerLifetimeScope();
            //注册进站服务
            Build.RegisterType<InStationRunLogic>().Keyed<IRunLogic>("进站").InstancePerLifetimeScope();
            //注册出站服务
            Build.RegisterType<PassStationRunLogic>().Keyed<IRunLogic>("出站").InstancePerLifetimeScope();
            //注册物料绑定服务
            Build.RegisterType<MatirialBindRunLogic>().Keyed<IRunLogic>("物料绑定").InstancePerLifetimeScope();
            //注册数据上传服务
            Build.RegisterType<DataUploadRunLogic>().Keyed<IRunLogic>("数据上传").InstancePerLifetimeScope();
            //注册MES屏蔽进站服务
            Build.RegisterType<MESClosedInStation>().Keyed<IRunLogic>("MES屏蔽进站").InstancePerLifetimeScope();
            //注册MES屏蔽出站服务
            Build.RegisterType<MESClosedPassStation>().Keyed<IRunLogic>("MES屏蔽出站").InstancePerLifetimeScope();
            //注册MES屏蔽物料绑定服务
            Build.RegisterType<MESClosedMatirialBind>().Keyed<IRunLogic>("MES屏蔽物料绑定").InstancePerLifetimeScope();
            //注册MES屏蔽数据上传服务
            Build.RegisterType<MESClosedDataUpload>().Keyed<IRunLogic>("MES屏蔽数据上传").InstancePerLifetimeScope();
            //注册运行逻辑管理者
            Build.RegisterType<RunLogicManeger>().InstancePerLifetimeScope();
            //对全局变量操作的服务
            Build.RegisterType<RuntimeContextService>().SingleInstance();
            //对对运行配置实体操作的服务
            Build.RegisterType<RunConfigService>().SingleInstance();
            //注册主窗体
            Build.RegisterType<MainForm>().InstancePerLifetimeScope();
            //注册批次码输入窗体.瞬时生命周期
            Build.RegisterType<BatchCodeInputForm>().InstancePerDependency();
            //注册手动物料码输入窗体.瞬时生命周期
            Build.RegisterType<LSN_materialCodeInputForm>().InstancePerDependency();
            //注册手动物料码输入窗体.瞬时生命周期
            Build.RegisterType<RSN_materialCodeInputForm>().InstancePerDependency();
            //注册本地数据库驱动
            Build.RegisterType<LocalDbDAL>().InstancePerLifetimeScope();
            //实例化主窗体
            IContainer container = Build.Build();
            MainForm Form1 = container.Resolve<MainForm>();
            Application.Run(Form1);
        }
    }
}
