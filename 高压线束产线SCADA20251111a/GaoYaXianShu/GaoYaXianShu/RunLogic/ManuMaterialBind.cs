using Autofac;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.m_Form;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GaoYaXianShu.RunLogic
{
    public class ManuMaterialBind : IRunLogic
    {
        public ushort 目标流程字 { get ; set ; }
        public bool 允许执行标志位 { get; set; } = true;

        private readonly IComponentContext m_componentContext;
        private PLCService m_pLCService;
        private UIManeger m_UIManeger;
        private MesApiService m_MESApi;
        private RunConfig m_RunConfig;

        public ManuMaterialBind(
            IComponentContext componentContext,
            PLCService pLCService,
            UIManeger UIManeger,
            MesApiService mesApiService,
            RunConfigHelper runConfigHelper)
        {
            m_componentContext = componentContext;
            m_pLCService = pLCService;
            m_UIManeger = UIManeger;
            m_MESApi = mesApiService;
            m_RunConfig = runConfigHelper.RunConfig;

        }
        public async Task RunLogicAsync()
        {
            try
            {
                var 手动绑定物料反馈 = await 手动绑定物料Async();
                if (手动绑定物料反馈.IsFailed)
                {
                    return;
                }
                else
                {
                    m_UIManeger.AppendDataLog("手动绑定物料成功");
                    //成功执行一次不在多次执行
                    允许执行标志位 = false;
                }
            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog("手动绑定物料异常" + ex.Message);
            }
        }

        private async Task<Result<bool>> 手动绑定物料Async()
        {
            try
            {
                //弹出绑定对话框，等待绑定
                using (LSN_materialCodeInputForm m_MaterialCodeInputForm = m_componentContext.Resolve<LSN_materialCodeInputForm>())
                {
                    DialogResult result = m_MaterialCodeInputForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {

                        m_UIManeger.AppendDataLog("左线束添加批次成功");
                        UIMessageTip.ShowOk("添加批次成功");

                    }
                    else if (result == DialogResult.Abort)
                    {
                        m_UIManeger.AppendErrorLog("输入异常，请重新输入");
                        return Result.Fail("false");
                    }
                    else
                    {
                        m_UIManeger.AppendinfoLog("用户取消了操作");
                        return Result.Fail("false");
                    }
                }
                //弹出绑定对话框，等待绑定
                using (RSN_materialCodeInputForm m_MaterialCodeInputForm = m_componentContext.Resolve<RSN_materialCodeInputForm>())
                {
                    DialogResult result = m_MaterialCodeInputForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {

                        m_UIManeger.AppendDataLog("左线束添加批次成功");
                        UIMessageTip.ShowOk("添加批次成功");

                    }
                    else if (result == DialogResult.Abort)
                    {
                        m_UIManeger.AppendErrorLog("输入异常，请重新输入");
                        return Result.Fail("false");
                    }
                    else
                    {
                        m_UIManeger.AppendinfoLog("用户取消了操作");
                        return Result.Fail("false");
                    }
                }

                m_UIManeger.Set_TestStart_OK();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
