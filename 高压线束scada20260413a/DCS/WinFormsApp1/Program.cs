using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Extras.NLog;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Omron;
using HslCommunication.Serial;
using Microsoft.EntityFrameworkCore;
using NLog;
using Sunny.UI;
using System.Configuration;
using System.IO.Ports;
using System.Net;
using System.Net.Http;
using WinFormsApp1.Entity;
using WinFormsApp1.Entitys;
using WinFormsApp1.Flows;
using WinFormsApp1.Forms;
using WinFormsApp1.Interceptor;
using ILogger = NLog.ILogger;

namespace WinFormsApp1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            //异步锁
            Mutex fmutex = new Mutex(true, "1111");
            if (!fmutex.WaitOne(0, false))
            {
                UIMessageBox.Show("程序已在运行中。");
                return;
            }

            ContainerBuilder Build = new ContainerBuilder();

            //注册NLog日记服务
            LogManager.LoadConfiguration(@"Config/NLog.config");
            Build.RegisterModule<NLogModule>();

            #region 注册配置类
            //注册AppConfig类
            Build.Register<AppConfig>(c =>
            {
                var configManager = new XmlConfigManager<AppConfig>(@"Config/AppConfig.xml");
                return configManager.Load();
            }).SingleInstance();
            Build.RegisterType<XmlConfigManager<AppConfig>>()
                .WithParameter("filePath", @"Config/AppConfig.xml")
                .SingleInstance();

            #endregion

            //注册RuntimeContext类
            Build.RegisterType<RuntimeContext>().SingleInstance();

            //注册拦截器
            Build.RegisterType<ExceptionHandlingAsyncInterceptor>();
            Build.Register(c => c.Resolve<ExceptionHandlingAsyncInterceptor>().ToInterceptor())
                .Named<IInterceptor>("exceptionInterceptor");

            //注册流程
            Build.RegisterType<AutoReset>().Keyed<IRunLogic>(自动流程类别.重置).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            Build.RegisterType<MesClosedInstation>().Keyed<IRunLogic>(自动流程类别.MES屏蔽进站);

            Build.RegisterType<MesClosedDataUpload>().Keyed<IRunLogic>(自动流程类别.MES屏蔽数据上传).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            Build.RegisterType<MesClosedPassStation>().Keyed<IRunLogic>(自动流程类别.MES屏蔽出站).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            Build.RegisterType<GetSN>().Keyed<IRunLogic>(自动流程类别.申请SN).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            Build.RegisterType<SNBindTrayCode>().Keyed<IRunLogic>(自动流程类别.Sn绑定托盘号).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            Build.RegisterType<Instation>().Keyed<IRunLogic>(自动流程类别.进站).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            Build.RegisterType<DataUpload>().Keyed<IRunLogic>(自动流程类别.数据上传).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            Build.RegisterType<PassStation>().Keyed<IRunLogic>(自动流程类别.出站).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            Build.RegisterType<SNUnBindTrayCode>().Keyed<IRunLogic>(自动流程类别.Sn解绑托盘号).EnableInterfaceInterceptors()
                .InterceptedBy("exceptionInterceptor");

            //注册数据库上下文
            Build.RegisterType<AppDbContext>().InstancePerLifetimeScope();

            //注册读写PLC驱动
            Build.Register(c =>
            {
                var appconf = c.Resolve<AppConfig>();
                var omron = new OmronFinsUdp(appconf.PLC_IP地址,appconf.PLC_端口);
                omron.SA1 = appconf.PLC_本地连接网段;
                omron.LocalBinding = new IPEndPoint(IPAddress.Any, appconf.PLC_本地连接端口号);
                omron.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;
                omron.ByteTransform.IsStringReverseByteWord = false;
                return omron;
            }).SingleInstance();


            //注册串口扫码枪
            Build.Register<SerialPort>(c =>
            {
                var appconf = c.Resolve<AppConfig>();
                var serial = new SerialPort(appconf.扫码枪串口号, appconf.扫码枪波特率);
                return serial;
            }).InstancePerLifetimeScope();

            //MES
            Build.Register<NetworkWebApiBase>(c =>
            {
                var appconf = c.Resolve<AppConfig>();
                var webapi = new NetworkWebApiBase(appconf.MES的Ip地址, appconf.MES的端口号);
                webapi.DefaultContentType = "application/json";
                webapi.AddRequestHeadersAction = (Headers) =>
                {
                    Headers.Add("appid", "zbzx");
                    Headers.Add("appkey", "123456");
                };
                return webapi;
            }).SingleInstance();

            //注册主窗体
            Build.RegisterType<Form1>().InstancePerLifetimeScope();
            Build.RegisterType<SNInputForm>().InstancePerLifetimeScope(); 

            //实例化主窗体
            IContainer container = Build.Build();



            using (var scope = container.BeginLifetimeScope())
            {
                Form1 Form1 = container.Resolve<Form1>();
                Application.Run(Form1);
            }
        }
    }
}